using FasTnT.Application.Domain.Model;
using FasTnT.Application.Domain.Model.Subscriptions;

namespace FasTnT.Application;

public sealed class EpcisEvents
{
    public static event EventHandler<Request> OnRequestCaptured;
    public static event EventHandler<Subscription> OnSubscriptionRegistered;
    public static event EventHandler<Subscription> OnSubscriptionRemoved;
    public static event EventHandler<IEnumerable<string>> OnSubscriptionTriggered;

    private EpcisEvents() { }

    public static void RequestCaptured(Request request)
    {
        OnRequestCaptured?.Invoke(null, request);
    }
    public static void SubscriptionRegistered(Subscription subscription)
    {
        OnSubscriptionRegistered?.Invoke(null, subscription);
    }
    public static void SubscriptionRemoved(Subscription subscription)
    {
        OnSubscriptionRemoved?.Invoke(null, subscription);
    }
    public static void SubscriptionTriggered(IEnumerable<string> triggers)
    {
        OnSubscriptionTriggered?.Invoke(null, triggers);
    }
}
