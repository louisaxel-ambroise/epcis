using FasTnT.Application.Store.Configuration;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Model.Users;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Store;

public class EpcisContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Request> Requests { get; init; }
    public DbSet<Event> Events { get; init; }
    public DbSet<MasterData> MasterData { get; init; }
    public DbSet<Subscription> Subscriptions { get; init; }
    public DbSet<PendingRequest> PendingRequests { get; init; }
    public DbSet<StoredQuery> Queries { get; init; }

    public EpcisContext(DbContextOptions<EpcisContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) => EpcisModelConfiguration.Apply(modelBuilder, Database);
}
