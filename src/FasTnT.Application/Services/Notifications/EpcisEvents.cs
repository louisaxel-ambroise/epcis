using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Notifications;

public sealed class EpcisEvents : IEventNotifier, ISubscriptionListener, ICaptureListener
{
    public event Action<int> OnSubscription, OnUnsubscribe, OnCapture;

    public void RequestCaptured(Request request) => OnCapture?.Invoke(request.Id);
    public void SubscriptionRegistered(Subscription subscription) => OnSubscription?.Invoke(subscription.Id);
    public void SubscriptionRemoved(Subscription subscription) => OnUnsubscribe?.Invoke(subscription.Id);
}
