using FasTnT.Application.Relational.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Relational;

public class EpcisContext : DbContext
{
    private readonly IModelConfiguration _modelConfiguration;

    public EpcisContext(DbContextOptions<EpcisContext> options, IModelConfiguration modelConfiguration) : base(options)
    {
        _modelConfiguration = modelConfiguration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => _modelConfiguration.Apply(modelBuilder);
}