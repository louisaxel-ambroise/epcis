using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Notifications;

public interface INotificationSender
{
    Task RequestCapturedAsync(Request request, CancellationToken cancellationToken);
    Task SubscriptionRegisteredAsync(Subscription subscription, CancellationToken cancellationToken);
    Task SubscriptionRemovedAsync(Subscription subscription, CancellationToken cancellationToken);
}
