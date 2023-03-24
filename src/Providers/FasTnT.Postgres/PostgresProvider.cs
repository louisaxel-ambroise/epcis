using FasTnT.Application.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Postgres;

public static class PostgresProvider
{
    public static void Configure(IServiceCollection services, string connectionString, int commandTimeout)
    {
        services.AddDbContextPool<EpcisContext>(o => o.UseNpgsql(connectionString, x =>
        {
            x.MigrationsAssembly(typeof(PostgresProvider).Assembly.FullName);
            x.CommandTimeout(commandTimeout);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }));
    }
}
