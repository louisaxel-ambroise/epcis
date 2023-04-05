using FasTnT.Application.Services.Notifications;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Host.Services.Notifications;

public sealed class InMemoryNotificationManager : INotificationSender, INotificationReceiver
{
    public static readonly InMemoryNotificationManager Instance = new();

    private InMemoryNotificationManager()
    {
    }

    public event Action<Request> OnRequestCaptured;
    public event Action<Subscription> OnSubscriptionRegistered;
    public event Action<Subscription> OnSubscriptionRemoved;

    public Task RequestCapturedAsync(Request request, CancellationToken cancellationToken)
    {
        return Task.Run(() => OnRequestCaptured?.Invoke(request), cancellationToken);
    }

    public Task SubscriptionRegisteredAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        return Task.Run(() => OnSubscriptionRegistered?.Invoke(subscription), cancellationToken);
    }

    public Task SubscriptionRemovedAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        return Task.Run(() => OnSubscriptionRemoved?.Invoke(subscription), cancellationToken);
    }
}
