using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Subscriptions;

public interface ISubscriptionListener
{
    Task RegisterAsync(Subscription subscription, CancellationToken cancellationToken) => Task.CompletedTask;
    Task RemoveAsync(Subscription subscription, CancellationToken cancellationToken) => Task.CompletedTask;
    Task TriggerAsync(string[] triggers, CancellationToken cancellationToken) => Task.CompletedTask;
}
