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
            request.HasKey(x => x.Id);
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
            events.HasKey(x => x.Id);
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
            subscription.HasKey(x => x.Id);
            subscription.OwnsOne(x => x.Schedule);
            subscription.OwnsMany(x => x.Parameters, c =>
            {
                c.Property(x => x.Values).HasConversion<ArrayConverter, ArrayComparer>();
            });

            var executionRecords = modelBuilder.Entity<SubscriptionExecutionRecord>();
            executionRecords.HasKey("SubscriptionId", "ExecutionTime");

            var query = modelBuilder.Entity<StoredQuery>();
            query.HasKey(x => x.Id);
            query.OwnsMany(x => x.Parameters, c =>
            {
                c.Property(x => x.Values).HasConversion<ArrayConverter, ArrayComparer>();
            });
            query.HasData
            (
                new { Id = -2, Name = "SimpleEventQuery", DataSource = "SimpleEventQuery" },
                new { Id = -1, Name = "SimpleMasterDataQuery", DataSource = "SimpleMasterDataQuery" }
            );
        }
    }
}
