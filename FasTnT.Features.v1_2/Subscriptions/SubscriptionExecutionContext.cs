using FasTnT.Domain.Model;

namespace FasTnT.Features.v1_2.Subscriptions
{
    public record SubscriptionExecutionContext(Subscription Subscription, DateTime DateTime);
}
