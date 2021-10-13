using MediatR;

namespace FasTnT.Domain.Notifications
{
    public class SubscriptionCreatedNotification : INotification
    {
        public int SubscriptionId { get; }

        public SubscriptionCreatedNotification(int subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}
