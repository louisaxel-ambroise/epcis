using FasTnT.Application.Database.DataSources;
using FasTnT.Application.Services.Notifications;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FasTnT.Application.Database;

public sealed class EpcisContext : DbContext
{
    public EpcisContext(DbContextOptions<EpcisContext> options, IEventNotifier eventNotifier) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        EventNotifier = eventNotifier;
    }

    public IEventNotifier EventNotifier { get; }

    public IQueryable<Event> QueryEvents(IEnumerable<QueryParameter> parameters)
    {
        var eventContext = new EventQueryContext(this, parameters);

        return eventContext.ApplyTo(Set<Event>());
    }

    public IQueryable<MasterData> QueryMasterData(IEnumerable<QueryParameter> parameters)
    {
        var masterdataContext = new MasterDataQueryContext(this, parameters);

        return masterdataContext.ApplyTo(Set<MasterData>());
    }

    public async Task AddSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        Add(subscription);
        await SaveChangesAsync(cancellationToken);

        EventNotifier.SubscriptionRegistered(subscription);
    }

    public async Task RemoveSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        Remove(subscription);
        await SaveChangesAsync(cancellationToken);

        EventNotifier.SubscriptionRemoved(subscription);
    }

    public async Task StoreRequestAsync(Request request, CancellationToken cancellationToken)
    {
        using (var transaction = await Database.BeginTransactionAsync(cancellationToken))
        {
            Add(request);
            await SaveChangesAsync(cancellationToken);

            request.RecordTime = DateTime.UtcNow;
            await SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        EventNotifier.RequestCaptured(request);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EpcisModelConfiguration.Apply(modelBuilder);
    }
}