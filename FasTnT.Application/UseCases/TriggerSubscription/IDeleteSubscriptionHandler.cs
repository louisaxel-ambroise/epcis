namespace FasTnT.Application.UseCases.TriggerSubscription;

public interface ITriggerSubscriptionHandler
{
    Task TriggerSubscriptionAsync(string[] triggers, CancellationToken cancellationToken);
}
