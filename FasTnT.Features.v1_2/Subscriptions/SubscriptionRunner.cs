using FasTnT.Application.Services.Queries;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FasTnT.Features.v1_2.Subscriptions;

public class SubscriptionRunner
{
    private readonly IEnumerable<IStandardQuery> _epcisQueries;
    private readonly EpcisContext _context;
    private readonly ISubscriptionResultSender _resultSender;
    private readonly ILogger<SubscriptionRunner> _logger;

    public SubscriptionRunner(IEnumerable<IStandardQuery> epcisQueries, EpcisContext context, ISubscriptionResultSender resultSender, ILogger<SubscriptionRunner> logger)
    {
        _epcisQueries = epcisQueries;
        _context = context;
        _resultSender = resultSender;
        _logger = logger;
    }

    public async Task Run(SubscriptionExecutionContext executionContext, CancellationToken cancellationToken)
    {
        var subscription = executionContext.Subscription;

        _logger.LogInformation("Running Subscription {Name} ({Id})", subscription.Name, subscription.Id);
        _context.Attach(subscription);

        var executionRecord = new SubscriptionExecutionRecord { ExecutionTime = DateTime.UtcNow, ResultsSent = true, Successful = true };
        var query = _epcisQueries.Single(x => x.Name == subscription.QueryName);
        var pendingRequests = await _context.PendingRequests.Where(x => x.SubscriptionId == subscription.Id).ToListAsync(cancellationToken);
        var resultsSent = false;

        try
        {
            var response = QueryResponse.Empty(query.Name);

            if (pendingRequests.Any())
            {
                var parameters = subscription.Parameters
                    .Append(QueryParameter.Create("EQ_requestId", pendingRequests.Select(x => x.RequestId.ToString())))
                    .ToArray();

                response = await query.ExecuteAsync(_context, parameters, cancellationToken);
            }

            response.SubscriptionId = subscription.Name;
            resultsSent = await SendQueryResults(executionContext, response, cancellationToken).ConfigureAwait(false);
        }
        catch (EpcisException ex)
        {
            ex.SubscriptionId = subscription.Name;

            resultsSent = await SendExceptionResult(executionContext, ex, cancellationToken).ConfigureAwait(false);
        }


        if (resultsSent)
        {
            _logger.LogInformation("Results for subscription {Name} successfully sent", subscription.Name);
            _context.PendingRequests.RemoveRange(pendingRequests);
        }
        else
        {
            _logger.LogInformation("Failed to send results for subscription {Name}", subscription.Name);

            executionRecord.Successful = false;
            executionRecord.Reason = "Failed to send subscription result";
        }

        subscription.ExecutionRecords.Add(executionRecord);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> SendQueryResults(SubscriptionExecutionContext context, QueryResponse response, CancellationToken cancellationToken)
    {
        var successful = true;

        if(response.EventList.Count > 0 || context.Subscription.ReportIfEmpty)
        {
            successful = await _resultSender.Send(context, response, cancellationToken).ConfigureAwait(false);
        }

        return successful;
    }

    private async Task<bool> SendExceptionResult(SubscriptionExecutionContext context, EpcisException response, CancellationToken cancellationToken)
    {
        return await _resultSender.Send(context, response, cancellationToken);
    }
}
