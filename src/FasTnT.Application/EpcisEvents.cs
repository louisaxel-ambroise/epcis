using FasTnT.Application.Domain.Model;
using FasTnT.Application.Domain.Model.Subscriptions;

namespace FasTnT.Application;

public sealed class EpcisEvents
{
    public static event Action<Request> OnRequestCaptured;
    public static event Action<Subscription> OnSubscriptionRegistered;
    public static event Action<Subscription> OnSubscriptionRemoved;
    public static event Action<IEnumerable<string>> OnSubscriptionTriggered;

    private EpcisEvents() { }

    public static void RequestCaptured(Request request)
    {
        OnRequestCaptured?.Invoke(request);
    }
    public static void SubscriptionRegistered(Subscription subscription)
    {
        OnSubscriptionRegistered?.Invoke(subscription);
    }
    public static void SubscriptionRemoved(Subscription subscription)
    {
        OnSubscriptionRemoved?.Invoke(subscription);
    }
    public static void SubscriptionTriggered(IEnumerable<string> triggers)
    {
        OnSubscriptionTriggered?.Invoke(triggers);
    }
}
