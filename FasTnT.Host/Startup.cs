using Carter;
using FasTnT.Application.Queries;
using FasTnT.Application.Services;
using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Domain.Infrastructure.Behaviors;
using FasTnT.Host.Authorization;
using FasTnT.Host.Services.Subscriptions;
using FasTnT.Host.Services.User;
using FasTnT.Infrastructure.Configuration;
using FasTnT.Infrastructure.Store;
using FasTnT.Subscriptions;
using FasTnT.Subscriptions.Notifications;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Host;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(BasicAuthenticationHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, null);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(nameof(ICurrentUser.CanQuery), policy => policy.RequireClaim(nameof(ICurrentUser.CanQuery), bool.TrueString));
            options.AddPolicy(nameof(ICurrentUser.CanCapture), policy => policy.RequireClaim(nameof(ICurrentUser.CanCapture), bool.TrueString));
        });
        services.AddHttpLogging(httpLogging =>
        {
            httpLogging.LoggingFields = HttpLoggingFields.All;
            httpLogging.MediaTypeOptions.AddText("application/xml");
            httpLogging.MediaTypeOptions.AddText("application/text+xml");
            httpLogging.RequestBodyLogLimit = 4096;
            httpLogging.ResponseBodyLogLimit = 4096;
        });
        services.AddHttpContextAccessor();
        services.AddDbContext<EpcisContext>(ContextOptionBuilder);

        services.AddMediatR(typeof(PollQueryHandler), typeof(SubscriptionCreatedNotificationHandler));
        services.AddCarter();
        services.AddValidatorsFromAssemblyContaining(typeof(CommandValidationBehavior<,>));
        services.AddScoped<IncrementGenerator.Identity>();
        services.AddTransient<IUserProvider, UserProvider>();
        services.AddTransient<IEpcisQuery, SimpleEventQuery>();
        services.AddTransient<IEpcisQuery, SimpleMasterDataQuery>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandLoggerBehavior<,>));
        services.AddTransient<ICurrentUser, HttpContextCurrentUser>();
        services.AddScoped<SubscriptionRunner>();
        services.AddSingleton<ISubscriptionService, SubscriptionBackgroundService>();
        services.AddHostedService(s => s.GetRequiredService<ISubscriptionService>() as SubscriptionBackgroundService);
        services.AddScoped<ISubscriptionResultSender, HttpSubscriptionResultSender>();

        var constantsSection = _configuration.GetSection(nameof(Constants));
        if (constantsSection.Exists())
        {
            Constants.MaxEventsReturnedInQuery = constantsSection.GetValue(nameof(Constants.MaxEventsReturnedInQuery), Constants.MaxEventsReturnedInQuery);
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseDefaultFiles().UseStaticFiles();
        }

        app.UseExceptionHandler("/epciserror");
        app.UseRouting();
        app.UseHttpLogging();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(builder =>
        {
            builder.MapCarter();
        });
    }

    private void ContextOptionBuilder(DbContextOptionsBuilder builder)
    {
        var connectionString = _configuration.GetConnectionString("FasTnT.Database");
        var commandTimeout = _configuration.GetValue("FasTnT.Database.ConnectionTimeout", 60);
        var sqlProvider = _configuration.GetValue("FasTnT.Database.SqlProvider", "SqlServer");

        if (sqlProvider.Equals("Npgsql", StringComparison.OrdinalIgnoreCase))
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            builder.UseNpgsql(connectionString, opt => opt
                .MigrationsAssembly("FasTnT.Migrations.Npgsql")
                .EnableRetryOnFailure()
                .CommandTimeout(commandTimeout));
        }
        else
        {
            builder.UseSqlServer(connectionString, opt => opt
                .MigrationsAssembly("FasTnT.Migrations.SqlServer")
                .EnableRetryOnFailure()
                .CommandTimeout(commandTimeout));
        }
    }
}
