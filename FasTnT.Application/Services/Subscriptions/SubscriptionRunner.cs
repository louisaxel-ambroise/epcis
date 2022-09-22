using FasTnT.Application.Services.Queries;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FasTnT.Application.Services.Subscriptions;

public class SubscriptionRunner : ISubscriptionRunner
{
    private readonly IEnumerable<IEpcisDataSource> _dataSources;
    private readonly EpcisContext _context;
    private readonly ILogger<SubscriptionRunner> _logger;

    public SubscriptionRunner(IEnumerable<IEpcisDataSource> dataSources, EpcisContext context, ILogger<SubscriptionRunner> logger)
    {
        _dataSources = dataSources;
        _context = context;
        _logger = logger;
    }

    public async Task RunAsync(SubscriptionContext executionContext, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .Include(x => x.Parameters)
            .Include(x => x.Query)
            .SingleOrDefaultAsync(x => x.Id == executionContext.Id);

        _logger.LogInformation("Running Subscription {Name} ({Id})", subscription.Name, subscription.Id);

        var executionRecord = new SubscriptionExecutionRecord { ExecutionTime = DateTime.UtcNow, ResultsSent = true, Successful = true, SubscriptionId = subscription.Id };
        var dataSource = _dataSources.Single(x => x.Name == subscription.Query.DataSource);
        var pendingRequests = await _context.PendingRequests.Where(x => x.SubscriptionId == subscription.Id).ToListAsync(cancellationToken);
        var resultsSent = false;

        try
        {
            var response = new QueryResponse(dataSource.Name, subscription.Name, new (), null);

            if (pendingRequests.Any())
            {
                var parameters = subscription.Parameters
                    .Append(QueryParameter.Create("EQ_requestId", pendingRequests.Select(x => x.RequestId.ToString()).ToArray()))
                    .ToArray();
                var epcisData = await dataSource.ExecuteAsync(_context, parameters, cancellationToken);

                response = new QueryResponse(subscription.QueryName, subscription.Name, epcisData.EventList, epcisData.VocabularyList);
            }

            resultsSent = await SendQueryResults(subscription, response, executionContext.ResultSender, cancellationToken).ConfigureAwait(false);
        }
        catch (EpcisException ex)
        {
            ex.SubscriptionId = subscription.Name;

            resultsSent = await SendExceptionResult(subscription, ex, executionContext.ResultSender, cancellationToken).ConfigureAwait(false);
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

        _context.Add(executionRecord);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static Task<bool> SendQueryResults(Subscription context, QueryResponse response, IResultSender resultSender, CancellationToken cancellationToken)
    {
        if (response.EventList.Count > 0 || context.ReportIfEmpty)
        {
            return resultSender.SendResultAsync(context, response, cancellationToken);
        }

        return Task.FromResult(true);
    }

    private static Task<bool> SendExceptionResult(Subscription context, EpcisException response, IResultSender resultSender, CancellationToken cancellationToken)
    {
        return resultSender.SendErrorAsync(context, response, cancellationToken);
    }
}