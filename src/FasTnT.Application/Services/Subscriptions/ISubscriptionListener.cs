namespace FasTnT.Application.Services.Subscriptions;

public interface ISubscriptionListener
{
    Task RegisterAsync(SubscriptionContext context, CancellationToken cancellationToken) => Task.CompletedTask;
    Task RemoveAsync(int subscriptionId, CancellationToken cancellationToken) => Task.CompletedTask;
    Task TriggerAsync(string[] triggers, CancellationToken cancellationToken) => Task.CompletedTask;
}
