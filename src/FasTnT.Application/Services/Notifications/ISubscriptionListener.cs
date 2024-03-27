namespace FasTnT.Application.Services.Notifications;

public interface ISubscriptionListener
{
    event Action<int> OnSubscription;
    event Action<int> OnUnsubscribe;
}