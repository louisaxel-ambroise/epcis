using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Notifications;

public interface INotificationReceiver
{
    public event Action<Request> OnRequestCaptured;
    public event Action<Subscription> OnSubscriptionRegistered;
    public event Action<Subscription> OnSubscriptionRemoved;
}
