using FasTnT.Application.Queries;
using FasTnT.Application.Services;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Infrastructure.Behaviors;
using FasTnT.Infrastructure.Configuration;
using FasTnT.Infrastructure.Store;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Host.Features.v2_0;

public static class EpcisConfiguration
{
    public static IServiceCollection AddEpcisServices(this IServiceCollection services) => AddEpcisServices(services, null);

    public static IServiceCollection AddEpcisServices(this IServiceCollection services, Action<EpcisOptions> configure)
    {
        var options = new EpcisOptions();
        if (configure is not null)
        {
            configure(options);
        }

        services.AddMediatR(typeof(PollQueryHandler));
        services.AddValidatorsFromAssemblyContaining(typeof(CommandValidationBehavior<,>));
        services.AddScoped<IncrementGenerator.Identity>();
        services.AddScoped(options.CurrentUser);
        services.AddScoped(options.UserProvider);
        services.AddTransient<IEpcisQuery, SimpleEventQuery>();
        services.AddTransient<IEpcisQuery, SimpleMasterDataQuery>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandLoggerBehavior<,>));
        services.AddSqlServer<EpcisContext>(options.ConnectionString, opt => opt.MigrationsAssembly("FasTnT.Migrations.SqlServer").EnableRetryOnFailure().CommandTimeout(options.CommandTimeout));

        return services;
    }
}

public class EpcisOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 60;
    public Func<IServiceProvider, ICurrentUser> CurrentUser { get; set; } = _ => null;
    public Func<IServiceProvider, IUserProvider> UserProvider { get; set; } = svc => new UserProvider(svc.GetService<EpcisContext>());
}