using FasTnT.Application.Queries;
using FasTnT.Application.Services;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Infrastructure.Behaviors;
using FasTnT.Host.Services.Subscriptions;
using FasTnT.Host.Services.User;
using FasTnT.Infrastructure.Configuration;
using FasTnT.Subscriptions;
using FasTnT.Subscriptions.Notifications;
using FluentValidation;
using MediatR;

namespace FasTnT.Host.Features.v1_2;

public static class Epcis1_2Configuration
{
    public static IServiceCollection AddEpcis12Services(this IServiceCollection services)
    {
        services.AddMediatR(typeof(PollQueryHandler), typeof(SubscriptionCreatedNotificationHandler));
        services.AddValidatorsFromAssemblyContaining(typeof(CommandValidationBehavior<,>));
        services.AddScoped<IncrementGenerator.Identity>();
        services.AddTransient<IEpcisQuery, SimpleEventQuery>();
        services.AddTransient<IEpcisQuery, SimpleMasterDataQuery>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandLoggerBehavior<,>));
        services.AddTransient<ICurrentUser, HttpContextCurrentUser>();

        return services;
    }

    public static IServiceCollection AddEpcis12SubscriptionService(this IServiceCollection services)
    {
        services.AddScoped<SubscriptionRunner>();
        services.AddSingleton<ISubscriptionService, SubscriptionBackgroundService>();
        services.AddScoped<ISubscriptionResultSender, HttpSubscriptionResultSender>();

        services.AddHostedService(s => s.GetRequiredService<ISubscriptionService>() as SubscriptionBackgroundService);

        return services;
    }

    public static IEndpointRouteBuilder MapEpcis12Endpoints(this IEndpointRouteBuilder endpoints)
    {
        Endpoints1_2.AddRoutes(endpoints);

        return endpoints;
    }
}
