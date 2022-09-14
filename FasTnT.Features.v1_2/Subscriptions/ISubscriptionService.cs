using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Features.v1_2.Subscriptions;

public interface ISubscriptionService
{
    void Register(Subscription subscription);
    void Remove(int subscriptionId);
    void Trigger(params string[] triggers);
}
