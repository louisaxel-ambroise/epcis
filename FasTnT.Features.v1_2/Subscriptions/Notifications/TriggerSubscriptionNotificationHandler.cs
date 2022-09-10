using FasTnT.Domain.Notifications;
using MediatR;

namespace FasTnT.Features.v1_2.Subscriptions.Notifications;

public class TriggerSubscriptionNotificationHandler : INotificationHandler<TriggerSubscriptionNotification>
{
    private readonly ISubscriptionService _subscriptionService;

    public TriggerSubscriptionNotificationHandler(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public Task Handle(TriggerSubscriptionNotification notification, CancellationToken cancellationToken)
    {
        return Task.Run(() => _subscriptionService.Trigger(notification.Triggers), cancellationToken);
    }
}
