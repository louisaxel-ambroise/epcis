using FasTnT.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Oracle;

public static class OracleProvider
{
    public static void Configure(IServiceCollection services, string connectionString, int commandTimeout)
    {
        services.AddDbContextPool<EpcisContext>(o => o.UseOracle(connectionString, x =>
        {
            x.MigrationsAssembly(typeof(OracleProvider).Assembly.FullName);
            x.CommandTimeout(commandTimeout);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }));
    }
}
