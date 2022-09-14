using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.ListSubscriptions;

public interface IListSubscriptionsHandler
{
    Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string queryName, CancellationToken cancellationToken);
}
