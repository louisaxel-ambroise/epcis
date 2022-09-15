using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.Subscriptions;

public interface ICustomQuerySubscriptionHandler
{
    Task<Subscription> CustomQuerySubscriptionAsync(CustomSubscription subscription, CancellationToken cancellationToken);
}
