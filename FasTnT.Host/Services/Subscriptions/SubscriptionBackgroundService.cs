using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Database;
using FasTnT.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Host.Services.Subscriptions
{
    public sealed class SubscriptionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly object _monitor = new object();
        private readonly ConcurrentDictionary<Subscription, DateTime> _scheduledExecutions = new();
        private readonly ConcurrentDictionary<string, IList<Subscription>> _triggeredSubscriptions = new();
        private readonly ConcurrentQueue<string> _triggeredValues = new();

        public SubscriptionBackgroundService(IServiceProvider services)
        {
            _services = services;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async () => {
                await Initialize(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                while (!cancellationToken.IsCancellationRequested)
                {
                    var triggeredSubscriptions = new List<Subscription>();
                    try
                    {
                        // Get all subscriptions where the next execution time is reached
                        var subscriptions = _scheduledExecutions.Where(x => x.Value <= DateTime.UtcNow).ToArray();

                        foreach(var subscription in subscriptions)
                        {
                            var nextOccurence = subscription.Key.Schedule.GetNextOccurence(DateTime.UtcNow);

                            _scheduledExecutions.TryUpdate(subscription.Key, nextOccurence, subscription.Value);
                        }

                        triggeredSubscriptions.AddRange(subscriptions.Select(x => x.Key));

                        // Get all subscriptions scheduled by a trigger
                        while (_triggeredValues.TryDequeue(out string trigger))
                        {
                            triggeredSubscriptions.AddRange(_triggeredSubscriptions.TryGetValue(trigger, out IList<Subscription> sub) ? sub : Array.Empty<Subscription>());
                        }

                        await Execute(triggeredSubscriptions, cancellationToken);
                    }
                    finally
                    {
                        WaitTillNextExecutionOrNotification();
                    }
                }
            }, cancellationToken);
        }

        private async Task Execute(IEnumerable<Subscription> subscriptions, CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var subscriptionRunner = scope.ServiceProvider.GetService<SubscriptionRunner>();

            foreach (var subscription in subscriptions)
            {
                await subscriptionRunner.Run(subscription, cancellationToken);
            }
        }

        private async Task Initialize(CancellationToken cancellationToken)
        {
            var initialized = false;

            while (!initialized)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var subscriptionManager = scope.ServiceProvider.GetService<EpcisContext>();
                    var subscriptions = await subscriptionManager.Subscriptions
                        .Include(x => x.Schedule)
                        .Include(x => x.Parameters)
                        .AsNoTracking().ToListAsync(cancellationToken);

                    subscriptions.ForEach(Register);
                    initialized = true;
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10000), cancellationToken);
                }
            }
        }

        public void Register(Subscription subscription)
        {
            Pulse(() =>
            {
                if (string.IsNullOrEmpty(subscription.Trigger))
                {
                    _scheduledExecutions[subscription] = subscription.Schedule.GetNextOccurence(DateTime.UtcNow);
                }
                else
                {
                    if (!_triggeredSubscriptions.ContainsKey(subscription.Trigger))
                    {
                        _triggeredSubscriptions[subscription.Trigger] = new List<Subscription>();
                    }

                    _triggeredSubscriptions[subscription.Trigger].Add(subscription);
                }
            });
        }

        public void Remove(Subscription subscription)
        {
            Pulse(() =>
            {
                if (string.IsNullOrEmpty(subscription.Trigger))
                {
                    if (_scheduledExecutions.Any(x => x.Key.Id == subscription.Id))
                    {
                        _scheduledExecutions.TryRemove(_scheduledExecutions.FirstOrDefault(x => x.Key.Id == subscription.Id).Key, out DateTime value);
                    }
                }
                else
                {
                    _triggeredSubscriptions[subscription.Trigger].Remove(_triggeredSubscriptions[subscription.Trigger].FirstOrDefault(x => x.Id == subscription.Id));
                }
            });
        }

        public void Trigger(string triggerName)
        {
            Pulse(() => _triggeredValues.Enqueue(triggerName));
        }

        private void Pulse(Action action)
        {
            lock (_monitor)
            {
                action();
                Monitor.Pulse(_monitor);
            }
        }

        private void WaitTillNextExecutionOrNotification()
        {
            lock (_monitor)
            {
                _ = _scheduledExecutions.Any()
                 ? Monitor.Wait(_monitor, GetNextExecutionWaitTime(_scheduledExecutions.Values.Min()))
                 : Monitor.Wait(_monitor);
            }
        }

        private static TimeSpan GetNextExecutionWaitTime(DateTime executionTime)
            => executionTime < DateTime.UtcNow ? TimeSpan.Zero : executionTime - DateTime.UtcNow;
    }
}
