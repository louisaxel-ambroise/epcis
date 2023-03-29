using FasTnT.Application;
using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Services.Storage;
using FasTnT.Host.Services.Subscriptions.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace FasTnT.Host.Services.Subscriptions;

public class SubscriptionBackgroundService : BackgroundService
{
    private static readonly object _monitor = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionBackgroundService> _logger;

    private readonly ConcurrentDictionary<Subscription, DateTime> _scheduledExecutions = new();
    private readonly ConcurrentDictionary<string, List<Subscription>> _triggeredSubscriptions = new();
    private readonly ConcurrentQueue<string> _triggeredValues = new();

    public SubscriptionBackgroundService(IServiceProvider serviceProvider, ILogger<SubscriptionBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => Run(stoppingToken), stoppingToken);
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        Initialize();
        cancellationToken.Register(() => Pulse(() => { })); // Stop background process on cancellation.

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteAsync(DateTime.UtcNow, cancellationToken);
            }
            finally
            {
                WaitUntilNextExecution();
            }
        }
    }

    public async Task ExecuteAsync(DateTime executionDate, CancellationToken cancellationToken)
    {
        var triggeredSubscriptions = GetScheduledSubscriptions(executionDate).Union(GetTriggeredSubscriptions());

        await RunSubscriptionsAsync(triggeredSubscriptions.ToArray(), cancellationToken);
    }

    private IEnumerable<Subscription> GetTriggeredSubscriptions()
    {
        var subscriptions = new List<Subscription>();

        while (_triggeredValues.TryDequeue(out string trigger))
        {
            subscriptions.AddRange(_triggeredSubscriptions.TryGetValue(trigger, out var sub) ? sub : Array.Empty<Subscription>());
        }

        return subscriptions;
    }

    private IEnumerable<Subscription> GetScheduledSubscriptions(DateTime executionDate)
    {
        var plannedExecutions = _scheduledExecutions.Where(x => x.Value <= executionDate).ToArray();

        foreach (var plannedExecution in plannedExecutions)
        {
            var nextOccurence = GetNextOccurence(plannedExecution.Key, plannedExecution.Value);

            _scheduledExecutions.TryUpdate(plannedExecution.Key, nextOccurence, plannedExecution.Value);
        }

        return plannedExecutions.Select(x => x.Key);
    }

    private async Task RunSubscriptionsAsync(Subscription[] subscriptions, CancellationToken cancellationToken)
    {
        var executionTime = DateTime.UtcNow;
        var subscriptionTasks = subscriptions.Select(subscription => RunSubscriptionAsync(subscription, executionTime, cancellationToken));

        try
        {
            await Task.WhenAll(subscriptionTasks);
        }
        catch (Exception ex)
        {
            // Don't throw the exception as we want the background process to continue
            _logger.LogError(ex, "An error occured while executing subscriptions");
        }
    }

    private async Task RunSubscriptionAsync(Subscription subscription, DateTime executionTime, CancellationToken cancellationToken)
    {
        var resultSender = subscription.FormatterName == XmlResultSender.Instance.Name
            ? XmlResultSender.Instance
            : JsonResultSender.Instance;

        using var scope = _serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        var resultsSent = false;
        var executionRecord = new SubscriptionExecutionRecord { ExecutionTime = executionTime, ResultsSent = true, Successful = true, SubscriptionId = subscription.Id };
        var pendingRequests = await context.Set<PendingRequest>()
            .Where(x => x.SubscriptionId == subscription.Id)
            .OrderBy(x => x.RequestId)
            .Take(100)
            .ToListAsync(cancellationToken);

        try
        {
            var response = new QueryResponse(subscription.QueryName, subscription.Name, QueryData.Empty);

            if (pendingRequests.Any())
            {
                var queryData = await context
                    .QueryEvents(subscription.Parameters)
                    .Where(x => pendingRequests.Select(x => x.RequestId).Contains(x.Request.Id))
                    .ToListAsync(cancellationToken);

                response = new QueryResponse(subscription.QueryName, subscription.Name, queryData);
            }

            if (response.EventList.Count > 0 || subscription.ReportIfEmpty)
            {
                resultsSent = await resultSender.SendResultAsync(subscription, response, cancellationToken);
            }
        }
        catch (EpcisException ex)
        {
            resultsSent = await resultSender.SendErrorAsync(subscription, ex, cancellationToken);
        }

        if (resultsSent)
        {
            context.RemoveRange(pendingRequests);
        }
        else
        {
            executionRecord.Successful = false;
            executionRecord.Reason = "Failed to send subscription result";
        }

        context.Add(executionRecord);
        await context.SaveChangesAsync(cancellationToken);
    }

    public void Register(Subscription subscription)
    {
        Pulse(() =>
        {
            if (subscription.Trigger is null)
            {
                _scheduledExecutions[subscription] = GetNextOccurence(subscription, DateTime.UtcNow);
            }
            else
            {
                if (!_triggeredSubscriptions.ContainsKey(subscription.Trigger))
                {
                    _triggeredSubscriptions[subscription.Trigger] = new();
                }

                _triggeredSubscriptions[subscription.Trigger].Add(subscription);
            }
        });
    }

    public void Remove(Subscription subscription)
    {
        Pulse(() =>
        {
            if (_scheduledExecutions.Any(x => x.Key.Name == subscription.Name))
            {
                _scheduledExecutions.TryRemove(_scheduledExecutions.Single(x => x.Key.Name == subscription.Name).Key, out DateTime value);
            }
            else
            {
                foreach (var triggered in _triggeredSubscriptions)
                {
                    triggered.Value.Remove(triggered.Value.SingleOrDefault(s => s.Name == subscription.Name));
                }
            }
        });
    }

    public void Trigger(IEnumerable<string> triggers)
    {
        Pulse(() =>
        {
            foreach (var trigger in triggers)
            {
                _triggeredValues.Enqueue(trigger);
            }
        });
    }

    private void Initialize()
    {
        EpcisEvents.OnSubscriptionRegistered += Register;
        EpcisEvents.OnSubscriptionRemoved += Remove;
        EpcisEvents.OnSubscriptionTriggered += Trigger;

        using var scope = _serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        foreach (var subscription in context.Set<Subscription>().ToList())
        {
            Register(subscription);
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

    private void WaitUntilNextExecution()
    {
        lock (_monitor)
        {
            _ = _scheduledExecutions.Any()
                ? Monitor.Wait(_monitor, GetNextExecutionDelay(_scheduledExecutions.Values.Min()))
                : Monitor.Wait(_monitor);
        }
    }

    private DateTime GetNextOccurence(Subscription subscription, DateTime executionTime)
    {
        if (subscription.Schedule.IsEmpty())
        {
            throw new Exception("Triggered subscription can't compute next occurence");
        }

        return SubscriptionSchedule.GetNextOccurence(subscription.Schedule, executionTime);
    }

    private static TimeSpan GetNextExecutionDelay(DateTime executionTime)
    {
        return executionTime < DateTime.UtcNow ? TimeSpan.Zero : executionTime - DateTime.UtcNow;
    }
}
