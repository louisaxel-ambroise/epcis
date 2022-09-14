using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.StoreStandardQuerySubscription;

public interface IStoreStandardQuerySubscriptionHandler
{
    Task StoreSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken);
}
