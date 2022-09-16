using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.Subscriptions;

public interface IGetSubscriptionDetailsHandler
{
    Task<Subscription> GetSubscriptionDetailsAsync(string name, CancellationToken cancellationToken);
}
