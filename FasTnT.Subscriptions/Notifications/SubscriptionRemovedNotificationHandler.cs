using FasTnT.Domain.Notifications;
using MediatR;

namespace FasTnT.Subscriptions.Notifications
{
    public class SubscriptionRemovedNotificationHandler : INotificationHandler<SubscriptionRemovedNotification>
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionRemovedNotificationHandler(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public Task Handle(SubscriptionRemovedNotification notification, CancellationToken cancellationToken)
        {
            _subscriptionService.Remove(notification.SubscriptionId);

            return Task.CompletedTask;
        }
    }
}
