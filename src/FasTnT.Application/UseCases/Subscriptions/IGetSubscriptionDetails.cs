using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.Subscriptions;

public interface IGetSubscriptionDetails
{
    Task<Subscription> GetSubscriptionDetailsAsync(string name, CancellationToken cancellationToken);
}
