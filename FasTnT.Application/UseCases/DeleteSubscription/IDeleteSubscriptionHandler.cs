using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.DeleteSubscription;

public interface IDeleteSubscriptionHandler
{
    Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken);
}
