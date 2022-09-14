using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Model.Subscriptions;

public class SubscriptionParameter : QueryParameter
{
    public Subscription Subscription { get; set; }
}
