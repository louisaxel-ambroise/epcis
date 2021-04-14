using FasTnT.Domain.Model;
using FasTnT.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Infrastructure.Database
{
    public class EpcisContext : DbContext
    {
        private int _generationIndex = 0;
        
        public DbSet<Request> Requests { get; init; }
        public DbSet<Event> Events { get; init; }

        public EpcisContext(DbContextOptions<EpcisContext> options) : base(options) { }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => EpcisModelConfiguration.Apply(modelBuilder);

        internal int NextInteger() => _generationIndex += 1;
    }
}
