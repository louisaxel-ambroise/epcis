using FasTnT.Application.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application;

public class EpcisContext : DbContext
{
    private readonly IModelConfiguration _modelConfiguration;

    public EpcisContext(DbContextOptions<EpcisContext> options, IModelConfiguration modelConfiguration) : base(options)
    {
        _modelConfiguration = modelConfiguration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _modelConfiguration.Apply(modelBuilder);
    }
}