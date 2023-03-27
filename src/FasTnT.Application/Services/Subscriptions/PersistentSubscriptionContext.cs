using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Services.Storage;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Services.Subscriptions;

public class PersistentSubscriptionContext : ISubscriptionContext
{
    private readonly Subscription _subscription;
    private readonly IResultSender _resultSender;

    public PersistentSubscriptionContext(Subscription subscription, IResultSender resultSender)
    {
        _subscription = subscription;
        _resultSender = resultSender;
    }

    public string Name => _subscription.Name;
    public string Trigger => _subscription.Trigger;
    public bool IsScheduled() => _subscription.Trigger is null;

    public Task<bool> SendQueryResults(QueryResponse response, CancellationToken cancellationToken)
    {
        if (response.EventList.Count > 0 || _subscription.ReportIfEmpty)
        {
            return _resultSender.SendResultAsync(_subscription, response, cancellationToken);
        }

        return Task.FromResult(true);
    }

    public Task<bool> SendExceptionResult(EpcisException response, CancellationToken cancellationToken)
    {
        return _resultSender.SendErrorAsync(_subscription, response, cancellationToken);
    }

    public async Task ExecuteAsync(EpcisContext context, DateTime executionTime, CancellationToken cancellationToken)
    {
        var resultsSent = false;
        var executionRecord = new SubscriptionExecutionRecord { ExecutionTime = executionTime, ResultsSent = true, Successful = true, SubscriptionId = _subscription.Id };
        var pendingRequests = await context.Set<PendingRequest>()
            .Where(x => x.SubscriptionId == _subscription.Id)
            .OrderBy(x => x.RequestId)
            .Take(100)
            .ToListAsync(cancellationToken);

        try
        {
            var response = new QueryResponse(_subscription.QueryName, _subscription.Name, QueryData.Empty);

            if (pendingRequests.Any())
            {
                var queryData = await context
                    .QueryEvents(_subscription.Parameters)
                    .Where(x => pendingRequests.Select(x => x.RequestId).Contains(x.Request.Id))
                    .ToListAsync(cancellationToken);

                response = new QueryResponse(_subscription.QueryName, _subscription.Name, queryData);
            }

            resultsSent = await SendQueryResults(response, cancellationToken);
        }
        catch (EpcisException ex)
        {
            resultsSent = await SendExceptionResult(ex, cancellationToken);
        }

        if (resultsSent)
        {
            context.RemoveRange(pendingRequests);
        }
        else
        {
            executionRecord.Successful = false;
            executionRecord.Reason = "Failed to send subscription result";
        }

        context.Add(executionRecord);
        await context.SaveChangesAsync(cancellationToken);
    }

    public DateTime GetNextOccurence(DateTime executionTime)
    {
        if (_subscription.Schedule.IsEmpty())
        {
            throw new Exception("Triggered subscription can't compute next occurence");
        }

        return SubscriptionSchedule.GetNextOccurence(_subscription.Schedule, executionTime);
    }
}