using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Features.v2_0.Subscriptions
{
    public record SubscriptionExecutionContext(CustomSubscription Subscription, DateTime DateTime);
}
