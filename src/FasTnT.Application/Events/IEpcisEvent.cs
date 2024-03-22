using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Events;

public interface IEventListener
{
    event Action<int> OnRequestCaptured;
    event Action<int> OnSubscriptionRegistered;
    event Action<int> OnSubscriptionRemoved;
}

public interface IEventNotifier
{
    public void RequestCaptured(Request request);
    public void SubscriptionRegistered(Subscription subscription);
    public void SubscriptionRemoved(Subscription subscription);
}