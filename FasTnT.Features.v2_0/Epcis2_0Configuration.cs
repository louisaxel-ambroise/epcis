using FasTnT.Application.Services.Subscriptions;
using FasTnT.Features.v2_0.Endpoints;
using FasTnT.Features.v2_0.Subscriptions;
using Microsoft.Extensions.Hosting;

namespace FasTnT.Features.v2_0;

public static class Epcis2_0Configuration
{
    public static IServiceCollection AddEpcis20SubscriptionService(this IServiceCollection services)
    {
        services.AddScoped<SubscriptionRunner>();
        services.AddScoped<ISubscriptionResultSender, HttpSubscriptionResultSender>();
        services.AddSingleton<SubscriptionBackgroundService>();
        services.AddSingleton<ISubscriptionListener>(s => s.GetRequiredService<SubscriptionBackgroundService>());
        services.AddHostedService(s => s.GetRequiredService<SubscriptionBackgroundService>());

        return services;
    }

    public static IEndpointRouteBuilder MapEpcis20Endpoints(this IEndpointRouteBuilder endpoints)
    {
        CaptureEndpoints.AddRoutes(endpoints);
        EventsEndpoints.AddRoutes(endpoints);
        QueriesEndpoints.AddRoutes(endpoints);
        TopLevelEndpoints.AddRoutes(endpoints);
        SubscriptionEndpoints.AddRoutes(endpoints);

        return endpoints;
    }
}
