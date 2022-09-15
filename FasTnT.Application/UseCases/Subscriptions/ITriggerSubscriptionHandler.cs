namespace FasTnT.Application.UseCases.Subscriptions;

public interface ITriggerSubscriptionHandler
{
    Task TriggerSubscriptionAsync(string[] triggers, CancellationToken cancellationToken);
}
