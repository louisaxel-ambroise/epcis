using FasTnT.Infrastructure.Store;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Host;

public static class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        MigrateDatabase(host);

        host.Run();
    }

    private static void MigrateDatabase(IHost host)
    {
        using var scope = host.Services.CreateScope();

        scope.ServiceProvider.GetService<EpcisContext>().Database.Migrate();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder
                .UseStartup<Startup>()
                .ConfigureKestrel(s => s.AddServerHeader = false));
}
