using FasTnT.Application.Database;
using FasTnT.Application.Events;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Subscriptions.Jobs;
using System.Collections.Concurrent;

namespace FasTnT.Host.Subscriptions;

public class SubscriptionBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    private readonly ConcurrentDictionary<int, CancellationTokenSource> _runningSubscriptions = new();
    private readonly EpcisEvents _events = serviceProvider.GetRequiredService<EpcisEvents>();
    private CancellationToken _stoppingToken;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            var resetEvent = new ManualResetEvent(false);

            _stoppingToken = stoppingToken;
            _stoppingToken.Register(() => resetEvent.Set());

            _events.OnSubscriptionRegistered += RegisterSubscription;
            _events.OnSubscriptionRemoved += RemoveSubscription;

            LoadSubscriptions(); resetEvent.WaitOne();
        }, stoppingToken);
    }

    private void LoadSubscriptions()
    {
        using var scope = serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        context.Set<Subscription>().ToList().ForEach(RegisterSubscription);
    }

    private void RegisterSubscription(int subscriptionId)
    {
        using var scope = serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        var subscription = context.Find<Subscription>(subscriptionId);
        RegisterSubscription(subscription);
    }

    private void RegisterSubscription(Subscription subscription)
    {
        var backgroundTask = new PersistentSubscriptionJob(subscription, _events);
        var cancellationSource = new CancellationTokenSource();

        if (_runningSubscriptions.TryAdd(subscription.Id, cancellationSource))
        {
            _stoppingToken.Register(cancellationSource.Cancel);
            _ = Task.Run(() => backgroundTask.RunAsync(serviceProvider, cancellationSource.Token), _stoppingToken);
        }
    }

    private void RemoveSubscription(int subscriptionId)
    {
        if (_runningSubscriptions.Remove(subscriptionId, out var cancellationSource))
        {
            cancellationSource.Cancel();
        }
    }
}
