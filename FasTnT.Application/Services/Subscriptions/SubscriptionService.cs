using FasTnT.Domain.Model.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FasTnT.Application.Services.Subscriptions;

public sealed class SubscriptionService : ISubscriptionService, ISubscriptionListener
{
    private static readonly object _monitor = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IResultSender> _resultSenders;
    private readonly ILogger<SubscriptionService> _logger;

    private readonly ConcurrentDictionary<Subscription, DateTime> _scheduledExecutions = new();
    private readonly ConcurrentDictionary<string, List<Subscription>> _triggeredSubscriptions = new();
    private readonly ConcurrentQueue<string> _triggeredValues = new();

    public SubscriptionService(IServiceProvider serviceProvider, IEnumerable<IResultSender> resultSenders, ILogger<SubscriptionService> logger)
    {
        _serviceProvider = serviceProvider;
        _resultSenders = resultSenders;
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

    private IEnumerable<ExecutionContext> GetTriggeredSubscriptions()
    {
        var subscriptions = new List<ExecutionContext>();

        while (_triggeredValues.TryDequeue(out string trigger))
        {
            subscriptions.AddRange(_triggeredSubscriptions.TryGetValue(trigger, out var sub)
                ? sub.Select(x => new ExecutionContext(x, DateTime.UtcNow))
                : Array.Empty<ExecutionContext>());
        }

        return subscriptions;
    }

    private IEnumerable<ExecutionContext> GetScheduledSubscriptions(DateTime executionDate)
    {
        var plannedExecutions = _scheduledExecutions.Where(x => x.Value <= executionDate).ToArray();

        foreach (var plannedExecution in plannedExecutions)
        {
            var nextOccurence = SubscriptionSchedule.GetNextOccurence(plannedExecution.Key.Schedule, plannedExecution.Value);

            _scheduledExecutions.TryUpdate(plannedExecution.Key, nextOccurence, plannedExecution.Value);
        }

        return plannedExecutions.Select(x => new ExecutionContext(x.Key, x.Value));
    }

    private void Execute(ExecutionContext[] subscriptions, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var subscriptionTasks = new Task[subscriptions.Length];

        for (var i = 0; i < subscriptions.Length; i++)
        {
            var subscriptionRunner = scope.ServiceProvider.GetService<ISubscriptionRunner>();
            var resultSender = _resultSenders.FirstOrDefault(x => x.Name == subscriptions[i].Subscription.FormatterName);

            subscriptionTasks[i] = subscriptionRunner.RunAsync(subscriptions[i], resultSender, cancellationToken);
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

    public Task RegisterAsync(Subscription subscription, CancellationToken _)
    {
        if (subscription is Subscription standardSubscription)
        {
            Pulse(() =>
            {
                if (string.IsNullOrEmpty(standardSubscription.Trigger))
                {
                    _scheduledExecutions[standardSubscription] = SubscriptionSchedule.GetNextOccurence(standardSubscription.Schedule, DateTime.UtcNow);
                }
                else
                {
                    if (!_triggeredSubscriptions.ContainsKey(standardSubscription.Trigger))
                    {
                        _triggeredSubscriptions[standardSubscription.Trigger] = new();
                    }

                    _triggeredSubscriptions[standardSubscription.Trigger].Add(standardSubscription);
                }
            });
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(Subscription subscription, CancellationToken _)
    {
        Pulse(() =>
        {
            if (_scheduledExecutions.Any(x => x.Key.Id == subscription.Id))
            {
                _scheduledExecutions.TryRemove(_scheduledExecutions.Single(x => x.Key.Id == subscription.Id).Key, out DateTime value);
            }
            else
            {
                foreach (var triggered in _triggeredSubscriptions)
                {
                    triggered.Value.Remove(triggered.Value.SingleOrDefault(s => s.Id == subscription.Id));
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
