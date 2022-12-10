using FasTnT.Application.Configuration;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.CosmosDb;

public class CosmosModelConfiguration : IModelConfiguration
{
    public virtual void Apply(ModelBuilder modelBuilder)
    {
        var request = modelBuilder.Entity<Request>();
        request.ToContainer("requests");
        request.Property<Guid>("Id");
        request.HasKey("Id");
        request.Property(x => x.UserId).HasMaxLength(50);
        request.Property(x => x.DocumentTime);
        request.Property(x => x.CaptureDate);
        request.Property(x => x.SchemaVersion).IsRequired(true);
        request.HasMany(x => x.Events);
        request.HasMany(x => x.Masterdata);
        request.OwnsOne(x => x.StandardBusinessHeader, c =>
        {
            c.OwnsMany(x => x.ContactInformations);
        });
        request.OwnsOne(x => x.SubscriptionCallback);

        var masterData = modelBuilder.Entity<MasterData>();
        masterData.ToContainer("masterdata");
        masterData.HasKey("RequestId", nameof(MasterData.Type), nameof(MasterData.Id));
        masterData.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
        masterData.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        masterData.OwnsMany(x => x.Attributes, c =>
        {
            c.OwnsMany(x => x.Fields);
        });
        masterData.OwnsMany(x => x.Children);

        var evt = modelBuilder.Entity<Event>();
        evt.ToContainer("events");
        evt.Property<Guid>("Id").IsRequired(true).ValueGeneratedOnAdd();
        evt.Property(x => x.EventTime).IsRequired(true);
        evt.Property(x => x.Type).IsRequired(true).HasConversion<short>();
        evt.Property(x => x.EventTimeZoneOffset).IsRequired(true).HasConversion(x => x.Value, x => x);
        evt.Property(x => x.Action).IsRequired(true).HasConversion<short>();
        evt.Property(x => x.CertificationInfo).HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.EventId).HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.ReadPoint).HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.BusinessLocation).HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.BusinessStep).HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.Disposition).HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.TransformationId).HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.CorrectiveDeclarationTime).IsRequired(false);
        evt.Property(x => x.CorrectiveReason).HasMaxLength(256).IsRequired(false);
        evt.OwnsMany(x => x.Epcs, c => c.HasKey("EventId", nameof(Epc.Type), nameof(Epc.Id)));
        evt.OwnsMany(x => x.Sources, c => c.HasKey("EventId", nameof(Source.Type), nameof(Source.Id)));
        evt.OwnsMany(x => x.Destinations, c => c.HasKey("EventId", nameof(Destination.Type), nameof(Destination.Id)));
        evt.OwnsMany(x => x.Transactions, c => c.HasKey("EventId", nameof(BusinessTransaction.Type), nameof(BusinessTransaction.Id)));
        evt.OwnsMany(x => x.PersistentDispositions, c => c.HasKey("EventId", nameof(PersistentDisposition.Type), nameof(PersistentDisposition.Id)));
        evt.OwnsMany(x => x.SensorElements, c =>
        {
            c.OwnsMany(x => x.Reports);
        });
        evt.OwnsMany(x => x.CorrectiveEventIds);
        evt.HasOne(x => x.Request).WithMany(x => x.Events).HasForeignKey("RequestId").OnDelete(DeleteBehavior.Cascade);

        var subscription = modelBuilder.Entity<Subscription>();
        subscription.ToContainer("subscriptions");
        subscription.HasKey(nameof(Subscription.Name));
        subscription.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.QueryName).IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.ReportIfEmpty).IsRequired(true);
        subscription.Property(x => x.Trigger).IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.SignatureToken).IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.FormatterName).IsRequired(true).HasMaxLength(30);
        subscription.OwnsMany(x => x.Parameters);
        subscription.OwnsOne(x => x.Schedule);

        var executionRecords = modelBuilder.Entity<SubscriptionExecutionRecord>();
        executionRecords.ToContainer("subscriptions");
        executionRecords.HasKey("SubscriptionName", "ExecutionTime");

        var storedQuery = modelBuilder.Entity<StoredQuery>();
        storedQuery.ToContainer("queries");
        storedQuery.HasKey(nameof(StoredQuery.Name));
        storedQuery.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
        storedQuery.Property(x => x.DataSource).IsRequired(true).HasMaxLength(30);
        storedQuery.Property(x => x.UserId).IsRequired(false).HasMaxLength(80);
        storedQuery.OwnsMany(x => x.Parameters);
        storedQuery.HasData
        (
            new { Name = "SimpleEventQuery", DataSource = "SimpleEventQuery" },
            new { Name = "SimpleMasterDataQuery", DataSource = "SimpleMasterDataQuery" }
        );

        var pendingRequests = modelBuilder.Entity<PendingRequest>();
        storedQuery.ToContainer("subscriptions");
        pendingRequests.HasKey(nameof(PendingRequest.SubscriptionName), nameof(PendingRequest.RequestId));
    }
}
