using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.Subscriptions;

public interface IDeleteSubscriptionHandler
{
    Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken);
}
