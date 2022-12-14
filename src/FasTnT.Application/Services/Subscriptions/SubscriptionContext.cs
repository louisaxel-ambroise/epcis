using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Subscriptions;

public enum SubscriptionMethod
{
    Triggered,
    Scheduled
}

public class SubscriptionContext
{
    public Subscription Subscription { get; }
    public IResultSender ResultSender { get; }

    public SubscriptionMethod SubscriptionMethod => Subscription.Trigger is null ? SubscriptionMethod.Scheduled : SubscriptionMethod.Triggered;

    public SubscriptionContext(Subscription subscription, IResultSender resultSender)
    {
        Subscription = subscription;
        ResultSender = resultSender;
    }

    public Task<bool> SendQueryResults(QueryResponse response, CancellationToken cancellationToken)
    {
        if (response.EventList.Count > 0 || Subscription.ReportIfEmpty)
        {
            return ResultSender.SendResultAsync(Subscription, response, cancellationToken);
        }

        return Task.FromResult(true);
    }

    public Task<bool> SendExceptionResult(EpcisException response, CancellationToken cancellationToken)
    {
        return ResultSender.SendErrorAsync(Subscription, response, cancellationToken);
    }
}