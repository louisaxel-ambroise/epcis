using FasTnT.Domain.Notifications;
using FasTnT.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Subscriptions.Notifications
{
    public class SubscriptionCreatedNotificationHandler : INotificationHandler<SubscriptionCreatedNotification>
    {
        private readonly EpcisContext _context;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionCreatedNotificationHandler(EpcisContext context, ISubscriptionService subscriptionService)
        {
            _context = context;
            _subscriptionService = subscriptionService;
        }

        public async Task Handle(SubscriptionCreatedNotification notification, CancellationToken cancellationToken)
        {
            var subscription = await _context.Subscriptions
                .AsNoTracking()
                .Include(x => x.Parameters)
                .Include(x => x.Schedule)
                .SingleOrDefaultAsync(x => x.Id == notification.SubscriptionId, cancellationToken);

            _subscriptionService.Register(subscription);
        }
    }

    public class TriggerSubscriptionNotificationHandler : INotificationHandler<TriggerSubscriptionNotification>
    {
        private readonly ISubscriptionService _subscriptionService;

        public TriggerSubscriptionNotificationHandler(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public Task Handle(TriggerSubscriptionNotification notification, CancellationToken cancellationToken)
        {
            _subscriptionService.Trigger(notification.Triggers);
            return Task.CompletedTask;
        }
    }
}
