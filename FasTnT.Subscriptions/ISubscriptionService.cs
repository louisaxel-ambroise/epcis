using FasTnT.Domain.Model;

namespace FasTnT.Subscriptions
{
    public interface ISubscriptionService
    {
        void Register(Subscription subscription);
        void Remove(int subscriptionId);
        void Trigger(params string[] triggers);
    }
}
