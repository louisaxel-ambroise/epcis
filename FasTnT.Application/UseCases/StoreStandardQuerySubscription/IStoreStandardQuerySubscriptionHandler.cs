using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.StoreStandardQuerySubscription;

public interface IStoreStandardQuerySubscriptionHandler
{
    Task<Subscription> StoreSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken);
}
