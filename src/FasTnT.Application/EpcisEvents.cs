using FasTnT.Application.Domain.Model;
using FasTnT.Application.Domain.Model.Subscriptions;

namespace FasTnT.Application;

public static class EpcisEvents
{
    public static event Action<Request> OnRequestCaptured;
    public static event Action<Subscription> OnSubscriptionRegistered;
    public static event Action<Subscription> OnSubscriptionRemoved;
    public static event Action<IEnumerable<string>> OnSubscriptionTriggered;

    public static void RequestCaptured(Request request)
    {
        if (OnRequestCaptured is not null)
        {
            OnRequestCaptured(request);
        }
    }
    public static void SubscriptionRegistered(Subscription subscription)
    {
        if (OnSubscriptionRegistered is not null)
        {
            OnSubscriptionRegistered(subscription);
        }
    }
    public static void SubscriptionRemoved(Subscription subscription)
    {
        if (OnSubscriptionRemoved is not null)
        {
            OnSubscriptionRemoved(subscription);
        }
    }
    public static void SubscriptionTriggered(IEnumerable<string> triggers)
    {
        if (OnSubscriptionTriggered is not null)
        {
            OnSubscriptionTriggered(triggers);
        }
    }
}
