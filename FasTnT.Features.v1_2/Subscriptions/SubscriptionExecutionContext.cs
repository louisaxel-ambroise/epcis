using FasTnT.Domain.Model;

namespace FasTnT.Subscriptions
{
    public record SubscriptionExecutionContext(Subscription Subscription, DateTime DateTime);
}
