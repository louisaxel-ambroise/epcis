using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Infrastructure.Database;

public class EpcisContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Request> Requests { get; init; }
    public DbSet<Event> Events { get; init; }
    public DbSet<MasterData> MasterData { get; init; }
    public DbSet<Subscription> Subscriptions { get; init; }
    public DbSet<PendingRequest> PendingRequests { get; init; }

    public EpcisContext(DbContextOptions<EpcisContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => EpcisModelConfiguration.Apply(modelBuilder);
}
