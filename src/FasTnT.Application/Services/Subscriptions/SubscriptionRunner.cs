using FasTnT.Application.Database;
using FasTnT.Application.Services.DataSources.Utils;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FasTnT.Application.Services.Subscriptions;

public class SubscriptionRunner : ISubscriptionRunner
{
    private readonly EpcisContext _context;
    private readonly ILogger<SubscriptionRunner> _logger;

    public SubscriptionRunner(EpcisContext context, ILogger<SubscriptionRunner> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task RunAsync(SubscriptionContext context, DateTime executionTime, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running Subscription {Name}", context.Subscription.Name);

        var executionRecord = new SubscriptionExecutionRecord { ExecutionTime = executionTime, ResultsSent = true, Successful = true, SubscriptionId = context.Subscription.Id };
        var pendingRequests = await _context.Set<PendingRequest>().Where(x => x.SubscriptionId == context.Subscription.Id).ToListAsync(cancellationToken);
        var resultsSent = false;

        try
        {
            var response = new QueryResponse(context.Subscription.QueryName, context.Subscription.Name, QueryData.Empty);

            if (pendingRequests.Any())
            {
                var queryData = await _context.QueryEvents()
                    .WithParameters(context.Subscription.Parameters)
                    .WithParameters(new[] { QueryParameter.Create("EQ_requestID", pendingRequests.Select(x => x.RequestId.ToString()).ToArray()) } )
                    .ExecuteAsync(cancellationToken);

                response = new QueryResponse(context.Subscription.QueryName, context.Subscription.Name, queryData);
            }

            resultsSent = await context.SendQueryResults(response, cancellationToken);
        }
        catch (EpcisException ex)
        {
            ex.SubscriptionId = context.Subscription.Name;

            resultsSent = await context.SendExceptionResult(ex, cancellationToken);
        }

        if (resultsSent)
        {
            _logger.LogInformation("Results for context.Subscription {Name} successfully sent", context.Subscription.Name);
            _context.RemoveRange(pendingRequests);
        }
        else
        {
            _logger.LogInformation("Failed to send results for context.Subscription {Name}", context.Subscription.Name);

            executionRecord.Successful = false;
            executionRecord.Reason = "Failed to send context.Subscription result";
        }

        _context.Add(executionRecord);
        await _context.SaveChangesAsync(cancellationToken);
    }
}