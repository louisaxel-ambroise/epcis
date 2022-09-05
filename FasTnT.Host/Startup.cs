using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Host.Authorization;
using FasTnT.Host.Features.v1_2;
using FasTnT.Infrastructure.Store;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Host;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;

    public Startup(IConfiguration configuration, IHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
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

        services.AddEpcis12Endpoints();
        services.AddEpcis12SubscriptionService();
        services.AddScoped<IUserProvider, UserProvider>();

        if (_environment.IsDevelopment())
        {
            services.AddScoped<IUserProvider, DefaultUserProvider>();
        }

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

        app.UseRouting();
        app.UseHttpLogging();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(builder =>
        {
            builder.MapEpcis12Endpoints();
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
