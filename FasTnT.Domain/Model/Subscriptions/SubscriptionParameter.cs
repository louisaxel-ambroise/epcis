namespace FasTnT.Domain.Model.Subscriptions;

public class SubscriptionParameter
{
    public Subscription Subscription { get; set; }
    public string Name { get; set; }
    public string[] Value { get; set; }
}
