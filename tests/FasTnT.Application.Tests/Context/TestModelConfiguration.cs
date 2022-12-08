using FasTnT.Application.Configuration;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Tests.Context;

public static partial class EpcisTestContext
{
    public class TestModelConfiguration : IModelConfiguration
    {
        public void Apply(ModelBuilder modelBuilder)
        {
            var request = modelBuilder.Entity<Request>();
            request.Property<int>("Id");
            request.HasMany(x => x.Events);
            request.HasMany(x => x.Masterdata);
            request.OwnsOne(x => x.SubscriptionCallback);
            request.OwnsOne(x => x.StandardBusinessHeader, c =>
            {
                c.OwnsMany(x => x.ContactInformations);
            });

            var masterdata = modelBuilder.Entity<MasterData>();
            masterdata.OwnsMany(x => x.Children);
            masterdata.OwnsMany(x => x.Attributes, c =>
            {
                c.OwnsMany(x => x.Fields);
            });

            var events = modelBuilder.Entity<Event>();
            events.Property<int>("Id");
            events.HasKey("Id");
            events.Property(x => x.EventTimeZoneOffset).HasConversion(x => x.Value, x => x);
            events.OwnsMany(x => x.Epcs);
            events.OwnsMany(x => x.Sources);
            events.OwnsMany(x => x.Destinations);
            events.OwnsMany(x => x.Transactions);
            events.OwnsMany(x => x.CorrectiveEventIds);
            events.OwnsMany(x => x.PersistentDispositions);
            events.OwnsMany(x => x.Fields);
            events.OwnsMany(x => x.SensorElements, c =>
            {
                c.OwnsMany(x => x.Reports);
            });

            var subscription = modelBuilder.Entity<Subscription>();
            subscription.Property<int>("Id");
            subscription.HasKey("Id");
            subscription.OwnsOne(x => x.Schedule);
            subscription.OwnsMany(x => x.ExecutionRecords);
            subscription.OwnsMany(x => x.Parameters, c =>
            {
                c.Property(x => x.Values).HasConversion<ArrayConverter, ArrayComparer>();
            });

            var query = modelBuilder.Entity<StoredQuery>();
            query.Property<int>("Id");
            query.HasKey("Id");
            query.OwnsMany(x => x.Parameters, c =>
            {
                c.Property(x => x.Values).HasConversion<ArrayConverter, ArrayComparer>();
            });
            query.Ignore(x => x.Subscriptions);
            query.HasData
            (
                new { Id = -2, Name = "SimpleEventQuery", DataSource = "SimpleEventQuery" },
                new { Id = -1, Name = "SimpleMasterDataQuery", DataSource = "SimpleMasterDataQuery" }
            );
        }
    }
}
