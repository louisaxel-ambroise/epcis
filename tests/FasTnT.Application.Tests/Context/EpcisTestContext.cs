using FasTnT.Application.Database;
using FasTnT.Application.Services.Notifications;
using FasTnT.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Tests.Context;

public sealed class EpcisTestContext(EpcisContext dbContext, EpcisEvents events)
{
    public EpcisContext DbContext => dbContext;
    public EpcisEvents Events => events;

    public static EpcisTestContext GetContext(string databaseName, bool reset = true)
    {
        var events = new EpcisEvents();
        var options = new DbContextOptionsBuilder<EpcisContext>().UseSqlite($"Data Source={databaseName}", x =>
        {
            x.MigrationsAssembly(typeof(SqliteProvider).Assembly.FullName);
            x.CommandTimeout(5);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }).Options;

        var context = new EpcisContext(options, events);

        if (reset)
        {
            context.Database.EnsureDeleted();
            context.Database.Migrate();
        }

        return new(context, events);
    }
}
