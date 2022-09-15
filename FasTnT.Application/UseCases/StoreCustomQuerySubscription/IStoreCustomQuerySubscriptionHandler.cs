using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.StoreCustomQuerySubscription;

public interface IStoreCustomQuerySubscriptionHandler
{
    Task<Subscription> StoreSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken);
}
