namespace FasTnT.Application.Services.Subscriptions;

public interface ISubscriptionRunner
{
    Task RunAsync(SubscriptionContext executionContext, CancellationToken cancellationToken);
}
