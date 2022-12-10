using FasTnT.Application;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Host.Services.Database;

public static class DatabaseMigrator
{
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder application)
    {
        using var scope = application.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<EpcisContext>();

        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }
        else
        {
            context.Database.EnsureCreated();
        }

        return application;
    }
}
