using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Store;
using FasTnT.Subscriptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace FasTnT.Host.Services.Subscriptions;

public sealed class SubscriptionBackgroundService : BackgroundService, ISubscriptionService
{
    private static readonly object _monitor = new();
    private readonly IServiceProvider _services;
    private readonly ILogger<SubscriptionBackgroundService> _logger;
    private readonly ConcurrentDictionary<Subscription, DateTime> _scheduledExecutions = new();
    private readonly ConcurrentDictionary<string, IList<Subscription>> _triggeredSubscriptions = new();
    private readonly ConcurrentQueue<string> _triggeredValues = new();

    public SubscriptionBackgroundService(IServiceProvider services)
    {
        _services = services;
        _logger = services.GetService<ILogger<SubscriptionBackgroundService>>();
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.Run(async () => {
            await Initialize(cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var executionDate = DateTime.UtcNow;
                    var triggeredSubscriptions = GetScheduledSubscriptions(executionDate).Union(GetTriggeredSubscriptions());

                    Execute(triggeredSubscriptions.ToArray(), cancellationToken);
                }
                finally
                {
                    WaitTillNextExecutionOrNotification();
                }
            }
        }, cancellationToken);
    }

    private IEnumerable<SubscriptionExecutionContext> GetTriggeredSubscriptions()
    {
        var subscriptions = new List<SubscriptionExecutionContext>();

        while (_triggeredValues.TryDequeue(out string trigger))
        {
            subscriptions.AddRange(_triggeredSubscriptions.TryGetValue(trigger, out IList<Subscription> sub) 
                ? sub.Select(x => new SubscriptionExecutionContext(x, DateTime.UtcNow))
                : Array.Empty<SubscriptionExecutionContext>());
        }

        return subscriptions;
    }

    private IEnumerable<SubscriptionExecutionContext> GetScheduledSubscriptions(DateTime executionDate)
    {
        var plannedExecutions = _scheduledExecutions.Where(x => x.Value <= executionDate).ToArray();

        foreach (var plannedExecution in plannedExecutions)
        {
            var nextOccurence = plannedExecution.Key.Schedule.GetNextOccurence(plannedExecution.Value);

            _scheduledExecutions.TryUpdate(plannedExecution.Key, nextOccurence, plannedExecution.Value);
        }

        return plannedExecutions.Select(x => new SubscriptionExecutionContext(x.Key, x.Value));
    }

    private void Execute(SubscriptionExecutionContext[] subscriptions, CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var subscriptionRunner = scope.ServiceProvider.GetService<SubscriptionRunner>();
        var subscriptionTasks = new Task[subscriptions.Length];

        for (var i = 0; i < subscriptions.Length; i++)
        {
            subscriptionTasks[i] = subscriptionRunner.Run(subscriptions[i], cancellationToken);
        }

        try
        {
            Task.WaitAll(subscriptionTasks, cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occured while executing subscriptions");
            // Don't throw the exception as we want the background process to continue
        }
    }

    private async Task Initialize(CancellationToken cancellationToken)
    {
        var initialized = false;

        do
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
                await Task.Delay(TimeSpan.FromMilliseconds(10000), cancellationToken).ConfigureAwait(false);
            }
        }
        while (!initialized);
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

    public void Remove(int subscriptionId)
    {
        Pulse(() =>
        {
            if (_scheduledExecutions.Any(x => x.Key.Id == subscriptionId))
            {
                _scheduledExecutions.TryRemove(_scheduledExecutions.FirstOrDefault(x => x.Key.Id == subscriptionId).Key, out DateTime value);
            }
            else
            {
                foreach(var subscription in _triggeredSubscriptions)
                {
                    subscription.Value.Remove(subscription.Value.SingleOrDefault(s => s.Id == subscriptionId));
                }
            }
        });
    }
            
    public void Trigger(params string[] triggers)
    {
        Pulse(() =>
        {
            for(var i=0; i<triggers.Length; i++)
            {
                _triggeredValues.Enqueue(triggers[i]);
            }
        });
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
