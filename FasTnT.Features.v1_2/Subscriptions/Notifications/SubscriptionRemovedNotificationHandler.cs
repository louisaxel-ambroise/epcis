using FasTnT.Domain.Notifications;
using MediatR;

namespace FasTnT.Features.v1_2.Subscriptions.Notifications;

public class SubscriptionRemovedNotificationHandler : INotificationHandler<SubscriptionRemovedNotification>
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionRemovedNotificationHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public Task Handle(SubscriptionRemovedNotification notification, CancellationToken cancellationToken)
    {
        return Task.Run(() => _subscriptionService.Remove(notification.SubscriptionId), cancellationToken);
    }
}
