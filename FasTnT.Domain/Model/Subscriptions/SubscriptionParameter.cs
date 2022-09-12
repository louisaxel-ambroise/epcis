namespace FasTnT.Domain.Model;

public class SubscriptionParameter
{
    public Subscription Subscription { get; set; }
    public string Name { get; set; }
    public string[] Value { get; set; }
}
