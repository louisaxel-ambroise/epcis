using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Subscriptions;

public interface ISubscriptionListener
{
    Task RegisterAsync(Subscription subscription, CancellationToken cancellationToken);
    Task RemoveAsync(Subscription subscription, CancellationToken cancellationToken);
    Task TriggerAsync(string[] triggers, CancellationToken cancellationToken);
}
