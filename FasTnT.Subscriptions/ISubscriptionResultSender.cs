namespace FasTnT.Subscriptions;

public interface ISubscriptionResultSender
{
    Task<bool> Send<T>(SubscriptionExecutionContext context, T epcisResponse, CancellationToken cancellationToken);
}
