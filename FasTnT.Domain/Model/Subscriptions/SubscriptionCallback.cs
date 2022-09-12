using FasTnT.Domain.Enumerations;

namespace FasTnT.Domain.Model.Subscriptions;

public class SubscriptionCallback
{
    public Request Request { get; set; }
    public string SubscriptionId { get; set; }
    public QueryCallbackType CallbackType { get; set; }
    public string Reason { get; set; }
}
