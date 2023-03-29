using FasTnT.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Tests.Application.Context;

public static class EpcisTestContext
{
    public static EpcisContext GetContext(string databaseName, bool reset = true)
    {
        var context = new EpcisContext(new DbContextOptionsBuilder<EpcisContext>().UseSqlite($"Data Source={databaseName}", x =>
        {
            x.MigrationsAssembly(typeof(SqliteProvider).Assembly.FullName);
            x.CommandTimeout(30);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }).Options);

        if (reset)
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        return context;
    }
}
