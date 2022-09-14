using FasTnT.Domain.Model.Queries;

namespace FasTnT.Domain.Model.Subscriptions;

public class SubscriptionParameter : QueryParameter
{
    public Subscription Subscription { get; set; }
}
