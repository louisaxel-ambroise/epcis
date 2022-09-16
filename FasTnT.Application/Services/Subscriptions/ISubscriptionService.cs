namespace FasTnT.Application.Services.Subscriptions;

public interface ISubscriptionService : ISubscriptionListener
{
    void Execute(CancellationToken cancellationToken);
}
