using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Infrastructure.Database
{
    public class EpcisContext : DbContext
    {
        public DbSet<Request> Requests { get; init; }
        public DbSet<Event> Events { get; init; }
        public DbSet<MasterData> MasterData { get; set; }

        public EpcisContext(DbContextOptions<EpcisContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => EpcisModelConfiguration.Apply(modelBuilder);
    }
}
