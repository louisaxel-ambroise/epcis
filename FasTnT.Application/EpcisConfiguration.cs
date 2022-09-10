using FasTnT.Application.Queries.Poll;
using FasTnT.Application.Services;
using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Services.Users.Providers;
using FasTnT.Application.Store;
using FasTnT.Application.Store.Configuration;
using FasTnT.Domain.Infrastructure.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Application;

public static class EpcisConfiguration
{
    public static IServiceCollection AddEpcisServices(this IServiceCollection services) => services.AddEpcisServices(null);

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