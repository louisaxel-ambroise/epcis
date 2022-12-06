using FasTnT.Application.Relational;
using FasTnT.Application.Relational.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Migrations.Postgres;

public static class PostgresConfiguration
{
    public static void Configure(IServiceCollection services, string connectionString, int commandTimeout)
    {
        services.AddSingleton<IModelConfiguration, PostgresModelConfiguration>();
        services.AddDbContext<EpcisContext>(o => o.UseNpgsql(connectionString, x =>
        {
            x.MigrationsAssembly(typeof(PostgresConfiguration).Assembly.FullName);
            x.CommandTimeout(commandTimeout);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        }));
    }
}
