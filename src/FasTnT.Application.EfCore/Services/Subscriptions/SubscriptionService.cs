using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FasTnT.Application.EfCore.Services.Subscriptions;

public sealed class SubscriptionService : ISubscriptionService, ISubscriptionListener
{
    private static readonly object _monitor = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionService> _logger;

    private readonly ConcurrentDictionary<SubscriptionContext, DateTime> _scheduledExecutions = new();
    private readonly ConcurrentDictionary<string, List<SubscriptionContext>> _triggeredSubscriptions = new();
    private readonly ConcurrentQueue<string> _triggeredValues = new();

    public SubscriptionService(IServiceProvider serviceProvider, ILogger<SubscriptionService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void Execute(CancellationToken cancellationToken)
    {
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
    }

    private IEnumerable<SubscriptionContext> GetTriggeredSubscriptions()
    {
        var subscriptions = new List<SubscriptionContext>();

        while (_triggeredValues.TryDequeue(out string trigger))
        {
            subscriptions.AddRange(_triggeredSubscriptions.TryGetValue(trigger, out var sub) ? sub : Array.Empty<SubscriptionContext>());
        }

        return subscriptions;
    }

    private IEnumerable<SubscriptionContext> GetScheduledSubscriptions(DateTime executionDate)
    {
        var plannedExecutions = _scheduledExecutions.Where(x => x.Value <= executionDate).ToArray();

        foreach (var plannedExecution in plannedExecutions)
        {
            var nextOccurence = SubscriptionSchedule.GetNextOccurence(plannedExecution.Key.Subscription.Schedule, plannedExecution.Value);

            _scheduledExecutions.TryUpdate(plannedExecution.Key, nextOccurence, plannedExecution.Value);
        }

        return plannedExecutions.Select(x => x.Key);
    }

    private void Execute(SubscriptionContext[] subscriptions, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var subscriptionTasks = new Task[subscriptions.Length];

        for (var i = 0; i < subscriptions.Length; i++)
        {
            var subscriptionRunner = scope.ServiceProvider.GetService<ISubscriptionRunner>();

            subscriptionTasks[i] = subscriptionRunner.RunAsync(subscriptions[i], cancellationToken);
        }

        try
        {
            Task.WaitAll(subscriptionTasks, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while executing subscriptions");
            // Don't throw the exception as we want the background process to continue
        }
    }

    public Task RegisterAsync(SubscriptionContext context, CancellationToken _)
    {
        Pulse(() =>
        {
            if (context.SubscriptionMethod == SubscriptionMethod.Scheduled)
            {
                _scheduledExecutions[context] = SubscriptionSchedule.GetNextOccurence(context.Subscription.Schedule, DateTime.UtcNow);
            }
            else if(context.SubscriptionMethod == SubscriptionMethod.Triggered)
            {
                if (!_triggeredSubscriptions.ContainsKey(context.Subscription.Trigger))
                {
                    _triggeredSubscriptions[context.Subscription.Trigger] = new();
                }

                _triggeredSubscriptions[context.Subscription.Trigger].Add(context);
            }
        });

        return Task.CompletedTask;
    }

    public Task RemoveAsync(Subscription subscription, CancellationToken _)
    {
        Pulse(() =>
        {
            if (_scheduledExecutions.Any(x => x.Key.Subscription.Id == subscription.Id))
            {
                _scheduledExecutions.TryRemove(_scheduledExecutions.Single(x => x.Key.Subscription.Id == subscription.Id).Key, out DateTime value);
            }
            else
            {
                foreach (var triggered in _triggeredSubscriptions)
                {
                    triggered.Value.Remove(triggered.Value.SingleOrDefault(s => s.Subscription.Id == subscription.Id));
                }
            }
        });

        return Task.CompletedTask;
    }

    public Task TriggerAsync(string[] triggers, CancellationToken cancellationToken)
    {
        Pulse(() =>
        {
            for (var i = 0; i < triggers.Length; i++)
            {
                _triggeredValues.Enqueue(triggers[i]);
            }
        });

        return Task.CompletedTask;
    }

    private static void Pulse(Action action)
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
