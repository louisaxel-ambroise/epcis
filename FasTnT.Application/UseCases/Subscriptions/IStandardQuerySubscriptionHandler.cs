using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.Subscriptions;

public interface IStandardQuerySubscriptionHandler
{
    Task<Subscription> StandardQuerySubscriptionAsync(StandardSubscription subscription, CancellationToken cancellationToken);
}
