using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.UseCases.GetSubscriptionDetails
{
    public interface IGetSubscriptionDetailsHandler
    {
        Task<Subscription> GetCustomQueryDetailsAsync(string name, CancellationToken cancellationToken);
    }
}
