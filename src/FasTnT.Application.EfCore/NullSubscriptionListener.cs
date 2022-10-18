using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.EfCore;

public static partial class EpcisConfiguration
{
    public class NullSubscriptionListener : ISubscriptionListener
    {
        public Task RegisterAsync(Subscription subscription, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task RemoveAsync(Subscription subscription, CancellationToken cancellationToken) => Task.CompletedTask;
        public Task TriggerAsync(string[] triggers, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
