namespace FasTnT.Features.v1_2.Subscriptions;

public interface ISubscriptionResultSender
{
    Task<bool> Send<T>(SubscriptionExecutionContext context, T epcisResponse, CancellationToken cancellationToken);
}
