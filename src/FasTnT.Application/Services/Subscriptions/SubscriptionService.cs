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

    private readonly ConcurrentDictionary<ISubscriptionContext, DateTime> _scheduledExecutions = new();
    private readonly ConcurrentDictionary<string, List<ISubscriptionContext>> _triggeredSubscriptions = new();
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

    private IEnumerable<ISubscriptionContext> GetTriggeredSubscriptions()
    {
        var subscriptions = new List<ISubscriptionContext>();

        while (_triggeredValues.TryDequeue(out string trigger))
        {
            subscriptions.AddRange(_triggeredSubscriptions.TryGetValue(trigger, out var sub) ? sub : Array.Empty<ISubscriptionContext>());
        }

        return subscriptions;
    }

    private IEnumerable<ISubscriptionContext> GetScheduledSubscriptions(DateTime executionDate)
    {
        var plannedExecutions = _scheduledExecutions.Where(x => x.Value <= executionDate).ToArray();

        foreach (var plannedExecution in plannedExecutions)
        {
            var nextOccurence = plannedExecution.Key.GetNextOccurence(plannedExecution.Value);

            _scheduledExecutions.TryUpdate(plannedExecution.Key, nextOccurence, plannedExecution.Value);
        }

        return plannedExecutions.Select(x => x.Key);
    }

    private void Execute(ISubscriptionContext[] subscriptions, CancellationToken cancellationToken)
    {
        var executionTime = DateTime.UtcNow;
        var subscriptionTasks = subscriptions
            .Select(subscription =>
            {
                var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetService<EpcisContext>();

                return subscription
                    .ExecuteAsync(context, executionTime, cancellationToken)
                    .ContinueWith(_ => { scope.Dispose(); }, cancellationToken);
            })
            .ToArray();

        try
        {
            Task.WaitAll(subscriptionTasks, cancellationToken);
        }
        catch (Exception ex)
        {
            // Don't throw the exception as we want the background process to continue
            _logger.LogError(ex, "An error occured while executing subscriptions");
        }
    }

    public void Register(ISubscriptionContext context)
    {
        Pulse(() =>
        {
            if (context.IsScheduled())
            {
                _scheduledExecutions[context] = context.GetNextOccurence(DateTime.UtcNow);
            }
            else
            {
                if (!_triggeredSubscriptions.ContainsKey(context.Trigger))
                {
                    _triggeredSubscriptions[context.Trigger] = new();
                }

                _triggeredSubscriptions[context.Trigger].Add(context);
            }
        });
    }

    public void Remove(string subscriptionName)
    {
        Pulse(() =>
        {
            if (_scheduledExecutions.Any(x => x.Key.Name == subscriptionName))
            {
                _scheduledExecutions.TryRemove(_scheduledExecutions.Single(x => x.Key.Name == subscriptionName).Key, out DateTime value);
            }
            else
            {
                foreach (var triggered in _triggeredSubscriptions)
                {
                    triggered.Value.Remove(triggered.Value.SingleOrDefault(s => s.Name == subscriptionName));
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
        EpcisEvents.Instance.OnSubscriptionRegistered += (_, context) => Register(context);
        EpcisEvents.Instance.OnSubscriptionRemoved += (_, subscriptionName) => Remove(subscriptionName);
        EpcisEvents.Instance.OnSubscriptionTriggered += (_, triggers) => Trigger(triggers);

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

            Register(new PersistentSubscriptionContext(subscription, resultSender));
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
