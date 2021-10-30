namespace FasTnT.Subscriptions;

public interface ISubscriptionResultSender
{
    Task<bool> Send<T>(string destination, T epcisResponse, CancellationToken cancellationToken);
}
