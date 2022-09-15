namespace FasTnT.Features.v2_0.Subscriptions;

public interface ISubscriptionResultSender
{
    Task<bool> Send<T>(SubscriptionExecutionContext context, T epcisResponse, CancellationToken cancellationToken);
}
