using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Database;

public class EpcisContext : DbContext
{
    public EpcisContext(DbContextOptions<EpcisContext> options) : base(options)
    {
        SavedChanges += (_, _) => ChangeTracker.Clear();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EpcisModelConfiguration.Apply(modelBuilder);
    }
}