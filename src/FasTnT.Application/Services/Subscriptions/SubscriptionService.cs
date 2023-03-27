using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Services.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FasTnT.Application.Services.Subscriptions;

public sealed class SubscriptionService : ISubscriptionService
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

    public void Run(CancellationToken cancellationToken)
    {
        Initialize();
        cancellationToken.Register(() => Pulse(() => { })); // Stop background process on cancellation.

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                Execute(DateTime.UtcNow, cancellationToken);
            }
            finally
            {
                WaitTillNextExecution();
            }
        }
    }

    public void Execute(DateTime executionDate, CancellationToken cancellationToken)
    {
        var triggeredSubscriptions = GetScheduledSubscriptions(executionDate).Union(GetTriggeredSubscriptions());

        Execute(triggeredSubscriptions.ToArray(), cancellationToken);
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
        var executionTime = DateTime.UtcNow;
        var subscriptionTasks = new Task[subscriptions.Length];

        for (var i = 0; i < subscriptions.Length; i++)
        {
            var scope = _serviceProvider.CreateScope();
            var subscriptionRunner = scope.ServiceProvider.GetService<ISubscriptionRunner>();

            subscriptionTasks[i] = subscriptionRunner
                .RunAsync(subscriptions[i], executionTime, cancellationToken)
                .ContinueWith(_ => scope.Dispose(), cancellationToken);
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

    public void Register(SubscriptionContext context)
    {
        Pulse(() =>
        {
            if (context.IsScheduled())
            {
                _scheduledExecutions[context] = SubscriptionSchedule.GetNextOccurence(context.Subscription.Schedule, DateTime.UtcNow);
            }
            else
            {
                if (!_triggeredSubscriptions.ContainsKey(context.Subscription.Trigger))
                {
                    _triggeredSubscriptions[context.Subscription.Trigger] = new();
                }

                _triggeredSubscriptions[context.Subscription.Trigger].Add(context);
            }
        });
    }

    public void Remove(string subscriptionName)
    {
        Pulse(() =>
        {
            if (_scheduledExecutions.Any(x => x.Key.Subscription.Name == subscriptionName))
            {
                _scheduledExecutions.TryRemove(_scheduledExecutions.Single(x => x.Key.Subscription.Name == subscriptionName).Key, out DateTime value);
            }
            else
            {
                foreach (var triggered in _triggeredSubscriptions)
                {
                    triggered.Value.Remove(triggered.Value.SingleOrDefault(s => s.Subscription.Name == subscriptionName));
                }
            }
        });
    }

    public Task Trigger(IEnumerable<string> triggers)
    {
        Pulse(() =>
        {
            foreach(var trigger in triggers)
            {
                _triggeredValues.Enqueue(trigger);
            }
        });

        return Task.CompletedTask;
    }

    private void Initialize()
    {
        EpcisEvents.Instance.OnSubscriptionRegistered += (sender, context) => Register(context);
        EpcisEvents.Instance.OnSubscriptionRemoved += (sender, subscriptionName) => Remove(subscriptionName);
        EpcisEvents.Instance.OnSubscriptionTriggered += (sender, triggers) => Trigger(triggers);

        using var scope = _serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        var resultSenders = scope.ServiceProvider.GetServices<IResultSender>();
        var subscriptions = context.Set<Subscription>().ToList();

        foreach (var subscription in subscriptions)
        {
            var resultSender = resultSenders.SingleOrDefault(x => x.Name == subscription.FormatterName);

            // If no ResultSender matches the subscription's one, it means that it is a websocket subscription. 
            // These subscriptions can't be registered in the background handler so we can skip this row.
            if (resultSender is null)
            {
                continue;
            }

            Register(new SubscriptionContext(subscription, resultSender));
        }
    }

    private static void Pulse(Action action)
    {
        lock (_monitor)
        {
            action();
            Monitor.Pulse(_monitor);
        }
    }

    private void WaitTillNextExecution()
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
