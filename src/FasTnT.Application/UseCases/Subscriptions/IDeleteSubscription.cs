using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.Subscriptions;

public interface IDeleteSubscription
{
    Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken);
}
