using FasTnT.Application.Relational;
using FasTnT.Migrations.Postgres;
using FasTnT.Migrations.Sqlite;
using FasTnT.Migrations.SqlServer;

namespace FasTnT.Host.Services.Database;

public static class EpcisStorageConfiguration
{
    public static IServiceCollection AddEpcisRelationalStorage(this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("FasTnT.Database");
        var commandTimeout = configuration.GetValue("FasTnT.Database.CommandTimeout", 30);
        var provider = configuration.GetValue("FasTnT.Database.Provider", nameof(Migrations.SqlServer));
        var configureAction = GetConfigurationAction(provider);

        configureAction(services, connectionString, commandTimeout);
        services.AddHealthChecks().AddDbContextCheck<EpcisContext>();

        return services;
    }

    private static Action<IServiceCollection, string, int> GetConfigurationAction(string provider)
    {
        return provider switch
        {
            nameof(Migrations.SqlServer) => SqlServerConfiguration.Configure,
            nameof(Migrations.Postgres) => PostgresConfiguration.Configure,
            nameof(Migrations.Sqlite) => SqliteConfiguration.Configure,
            _ => throw new Exception($"Unsupported provider: {provider}")
        };
    }
}
