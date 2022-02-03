using FasTnT.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Tests.Context;

public static class TestContext
{
    public static DbContextOptions<EpcisContext> GetOptions(string databaseName)
    {
        return new DbContextOptionsBuilder<EpcisContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }

    public static EpcisContext GetContext(string databaseName, bool reset = true)
    {
        var context = new EpcisContext(GetOptions(databaseName));

        if (reset)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        return context;
    }
}
