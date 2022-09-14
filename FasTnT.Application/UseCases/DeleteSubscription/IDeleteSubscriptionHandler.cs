namespace FasTnT.Application.UseCases.DeleteSubscription;

public interface IDeleteSubscriptionHandler
{
    Task DeleteSubscriptionAsync(string name, CancellationToken cancellationToken);
}
