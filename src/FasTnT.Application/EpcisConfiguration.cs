using FasTnT.Application.Domain.Format.v1_2.Subscriptions;
using FasTnT.Application.Domain.Format.v2_0.Subscriptions;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Storage;
using FasTnT.Application.Services.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Application;

public static class EpcisConfiguration
{
    public static IServiceCollection AddEpcisServices(this IServiceCollection services)
    {
        services.AddTransient<DataRetrieverHandler>();
        services.AddTransient<DataRetrieverHandler>();
        services.AddTransient<CaptureHandler>();
        services.AddTransient<QueriesHandler>();
        services.AddTransient<SubscriptionsHandler>();
        services.AddTransient<TopLevelResourceHandler>();
        services.AddHealthChecks().AddDbContextCheck<EpcisContext>();

        return services;
    }

    public static IServiceCollection AddEpcisSubscriptionServices(this IServiceCollection services)
    {
        services.AddTransient<ISubscriptionRunner, SubscriptionRunner>();
        services.AddSingleton<ISubscriptionService, SubscriptionService>();
        services.AddSingleton<ISubscriptionListener>(ctx => ctx.GetService<ISubscriptionService>());
        services.AddSingleton(XmlResultSender.Instance);
        services.AddSingleton(JsonResultSender.Instance);

        return services;
    }
}
