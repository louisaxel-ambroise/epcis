using FasTnT.Application;
using FasTnT.Application.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Postgres;

public static class PostgresProvider
{
    public static void Configure(IServiceCollection services, string connectionString, int commandTimeout)
    {
        services.AddSingleton<IModelConfiguration, PostgresModelConfiguration>();
        services.AddDbContext<EpcisContext>(o => o.UseNpgsql(connectionString, x =>
        {
            x.MigrationsAssembly(typeof(PostgresProvider).Assembly.FullName);
            x.CommandTimeout(commandTimeout);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }));
    }
}
