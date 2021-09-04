using MediatR;

namespace FasTnT.Domain.Notifications
{
    public class SubscriptionRemovedNotification : INotification
    {
        public int SubscriptionId { get; init; }

        public SubscriptionRemovedNotification(int subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}
