using MediatR;

namespace FasTnT.Domain.Notifications
{
    public class SubscriptionRemovedNotification : INotification
    {
        public int SubscriptionId { get; }

        public SubscriptionRemovedNotification(int subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}
