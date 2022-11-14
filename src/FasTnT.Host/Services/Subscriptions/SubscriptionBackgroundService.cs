using FasTnT.Application.EfCore.Store;
using FasTnT.Application.Services.Subscriptions;
using Microsoft.EntityFrameworkCore;

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

        return Task.Run(() => _subscriptionService.Execute(stoppingToken), stoppingToken);
    }

    private void Initialize(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        using var context = scope.ServiceProvider.GetService<EpcisContext>();

        var resultSenders = scope.ServiceProvider.GetServices<IResultSender>();
        var subscriptions = context.Subscriptions
            .Include(x => x.Query).ThenInclude(x => x.Parameters)
            .Include(x => x.Parameters)
            .Include(x => x.Schedule)
            .ToList();

        foreach (var subscription in subscriptions)
        {
            var resultSender = resultSenders.SingleOrDefault(x => x.Name == subscription.FormatterName);

            if (resultSender is not null)
            {
                var subscriptionContext = new SubscriptionContext(subscription, null);
                _subscriptionService.RegisterAsync(subscriptionContext, stoppingToken).Wait(stoppingToken);
            }
            else
            {
                // This is a websocket subscription that was not properly removed. As the connection is lost at this point, it's safe to remove the subscription from the DB
                context.Remove(subscription);
            }
        }

        context.SaveChanges();
    }
}
