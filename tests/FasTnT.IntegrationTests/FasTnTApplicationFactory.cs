using FasTnT.Application.Database;
using FasTnT.Sqlite;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace FasTnT.IntegrationTests;

internal class FasTnTApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName;

    public FasTnTApplicationFactory(string dbName)
    {
        _dbName = dbName;

        if (File.Exists($"{_dbName}.db"))
        {
            File.Delete($"{_dbName}.db");
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(svc =>
        {
            svc.RemoveAll(typeof(DbContextOptions<EpcisContext>));
            svc.RemoveAll(typeof(EpcisContext));

            SqliteProvider.Configure(svc, $"Data Source={_dbName}.db;", 1000);

            using var provider = svc.BuildServiceProvider();
            using var scope = provider.CreateScope();
            using var context = scope.ServiceProvider.GetService<EpcisContext>();

            context.Database.Migrate();
        });

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists($"{_dbName}.db"))
        {
            File.Delete($"{_dbName}.db");
        }
    }
}
