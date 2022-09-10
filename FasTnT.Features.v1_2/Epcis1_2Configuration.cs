using FasTnT.Subscriptions;
using FasTnT.Subscriptions.Notifications;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FasTnT.Host.Features.v1_2;

public static class Epcis1_2Configuration
{
    public static IServiceCollection AddEpcis12SubscriptionService<T>(this IServiceCollection services)
        where T : class, ISubscriptionService, IHostedService
    {
        services.AddMediatR(typeof(SubscriptionCreatedNotificationHandler));
        services.AddScoped<SubscriptionRunner>();
        services.AddScoped<SubscriptionRunner>();
        services.AddScoped<ISubscriptionResultSender, HttpSubscriptionResultSender>();
        services.AddSingleton<ISubscriptionService, T>();
        services.AddHostedService(s => s.GetRequiredService<ISubscriptionService>() as T);

        return services;
    }

    public static IEndpointRouteBuilder MapEpcis12Endpoints(this IEndpointRouteBuilder endpoints)
    {
        Endpoints1_2.AddRoutes(endpoints);

        return endpoints;
    }
}
