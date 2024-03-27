using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Notifications;

public interface IEventNotifier
{
    public void RequestCaptured(Request request);
    public void SubscriptionRegistered(Subscription subscription);
    public void SubscriptionRemoved(Subscription subscription);
}
