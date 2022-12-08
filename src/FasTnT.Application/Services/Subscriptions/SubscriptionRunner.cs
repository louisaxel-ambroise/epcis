using FasTnT.Application.Services.Queries;
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

    public async Task RunAsync(SubscriptionContext context, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running Subscription {Name} ({Id})", context.Subscription.Name, context.Subscription.Id);

        var executionRecord = new SubscriptionExecutionRecord { ExecutionTime = DateTimeOffset.UtcNow, ResultsSent = true, Successful = true, SubscriptionId = context.Subscription.Id };
        var dataSource = _dataSources.Single(x => x.Name == context.Subscription.Query.DataSource);
        var pendingRequests = await _context.Set<PendingRequest>().Where(x => x.SubscriptionId == context.Subscription.Id).ToListAsync(cancellationToken);
        var resultsSent = false;

        try
        {
            var response = new QueryResponse(dataSource.Name, context.Subscription.Name, new(), null);

            if (pendingRequests.Any())
            {
                var parameters = context.Subscription.Parameters
                    .Append(QueryParameter.Create("EQ_requestId", pendingRequests.Select(x => x.RequestId.ToString()).ToArray()))
                    .ToArray();
                var epcisData = await dataSource.ExecuteAsync(parameters, cancellationToken);

                response = new QueryResponse(context.Subscription.QueryName, context.Subscription.Name, epcisData.EventList, epcisData.VocabularyList);
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