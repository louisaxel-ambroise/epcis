using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application;

public sealed class EpcisEvents
{
    public static event Action<int> OnRequestCaptured;
    public static event Action<int> OnSubscriptionRegistered;
    public static event Action<int> OnSubscriptionRemoved;

    private EpcisEvents() { }

    public static void RequestCaptured(Request request)
    {
        OnRequestCaptured?.Invoke(request.Id);
    }

    public static void SubscriptionRegistered(Subscription subscription)
    {
        OnSubscriptionRegistered?.Invoke(subscription.Id);
    }

    public static void SubscriptionRemoved(Subscription subscription)
    {
        OnSubscriptionRemoved?.Invoke(subscription.Id);
    }
}
