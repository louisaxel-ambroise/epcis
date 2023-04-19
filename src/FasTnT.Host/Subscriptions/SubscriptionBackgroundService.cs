using FasTnT.Application;
using FasTnT.Application.Database;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Subscriptions.Jobs;
using System.Collections.Concurrent;

namespace FasTnT.Host.Subscriptions;

public class SubscriptionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<int, CancellationTokenSource> _runningSubscriptions = new();
    private CancellationToken _stoppingToken;

    public SubscriptionBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            var resetEvent = new ManualResetEvent(false);

            _stoppingToken = stoppingToken;
            _stoppingToken.Register(() => resetEvent.Set());

            EpcisEvents.OnSubscriptionRegistered += RegisterSubscription;
            EpcisEvents.OnSubscriptionRemoved += RemoveSubscription;

            LoadSubscriptions(); resetEvent.WaitOne();
        }, stoppingToken);
    }

    private void LoadSubscriptions()
    {
        using var scope = _serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        context.Set<Subscription>().ToList().ForEach(RegisterSubscription);
    }

    private void RegisterSubscription(int subscriptionId)
    {
        using var scope = _serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        var subscription = context.Find<Subscription>(subscriptionId);
        RegisterSubscription(subscription);
    }

    private void RegisterSubscription(Subscription subscription)
    {
        var backgroundTask = new PersistentSubscriptionJob(subscription);
        var cancellationSource = new CancellationTokenSource();

        if (_runningSubscriptions.TryAdd(subscription.Id, cancellationSource))
        {
            _stoppingToken.Register(cancellationSource.Cancel);
            _ = Task.Run(() => backgroundTask.RunAsync(_serviceProvider, cancellationSource.Token), _stoppingToken);
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
