namespace FasTnT.Application.Services.Subscriptions;

public interface ISubscriptionService : ISubscriptionListener
{
    void Run(CancellationToken cancellationToken);
    void Execute(DateTime executionDate, CancellationToken cancellationToken);
}
