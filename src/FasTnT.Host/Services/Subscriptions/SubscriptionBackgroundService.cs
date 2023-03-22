using FasTnT.Application.Database;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Host.Services.Subscriptions;

public class SubscriptionBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionBackgroundService(IServiceProvider services, ISubscriptionService subscriptionService)
    {
        _services = services;
        _subscriptionService = subscriptionService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Initialize(stoppingToken);

        return Task.Run(() => _subscriptionService.Run(stoppingToken), stoppingToken);
    }

    private void Initialize(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        var resultSenders = scope.ServiceProvider.GetServices<IResultSender>();
        var subscriptions = context.Set<Subscription>().ToList();

        foreach (var subscription in subscriptions)
        {
            var resultSender = resultSenders.SingleOrDefault(x => x.Name == subscription.FormatterName);

            if (resultSender is not null)
            {
                var subscriptionContext = new SubscriptionContext(subscription, resultSender);
                _subscriptionService.RegisterAsync(subscriptionContext, stoppingToken).Wait(stoppingToken);
            }
            else
            {
                // This is a websocket subscription that was not properly removed.
                // As the connection is lost at this point, it's safe to delete the subscription
                context.Remove(subscription);
            }
        }

        context.SaveChanges();
    }
}
