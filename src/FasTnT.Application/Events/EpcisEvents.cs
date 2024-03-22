using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Events;

public sealed class EpcisEvents : IEventNotifier, IEventListener
{
    public event Action<int> OnRequestCaptured;
    public event Action<int> OnSubscriptionRegistered;
    public event Action<int> OnSubscriptionRemoved;

    public void RequestCaptured(Request request) => OnRequestCaptured?.Invoke(request.Id);
    public void SubscriptionRegistered(Subscription subscription) => OnSubscriptionRegistered?.Invoke(subscription.Id);
    public void SubscriptionRemoved(Subscription subscription) => OnSubscriptionRemoved?.Invoke(subscription.Id);
}
