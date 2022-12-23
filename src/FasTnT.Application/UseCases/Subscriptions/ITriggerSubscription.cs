namespace FasTnT.Application.UseCases.Subscriptions;

public interface ITriggerSubscription
{
    Task TriggerSubscriptionAsync(string[] triggers, CancellationToken cancellationToken);
}
