using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.Subscriptions;

public interface IListSubscriptions
{
    Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string queryName, CancellationToken cancellationToken);
}
