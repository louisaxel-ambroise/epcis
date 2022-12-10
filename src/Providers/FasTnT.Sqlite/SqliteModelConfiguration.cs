﻿using FasTnT.Application.Configuration;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FasTnT.Sqlite;

internal class SqliteModelConfiguration : IModelConfiguration
{
    public void Apply(ModelBuilder modelBuilder)
    {
        var request = modelBuilder.Entity<Request>();
        request.ToTable(nameof(Request), builder => builder.HasTrigger("SubscriptionPendingRequests"));
        request.HasKey(x => x.Id);
        request.Property(x => x.Id).ValueGeneratedOnAdd();
        request.Property(x => x.UserId).HasMaxLength(50);
        request.Property(x => x.DocumentTime).HasConversion(new DateTimeOffsetToBinaryConverter());
        request.Property(x => x.CaptureTime).HasConversion(new DateTimeOffsetToBinaryConverter());
        request.Property(x => x.SchemaVersion).IsRequired(true);
        request.HasMany(x => x.Events).WithOne(x => x.Request).HasForeignKey("RequestId");
        request.HasMany(x => x.Masterdata).WithOne(x => x.Request).HasForeignKey("RequestId");
        request.OwnsOne(x => x.StandardBusinessHeader, c =>
        {
            c.ToTable(nameof(StandardBusinessHeader));
            c.HasKey("RequestId");
            c.Property<int>("RequestId");
            c.Property(x => x.Version).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Standard).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.TypeVersion).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.InstanceIdentifier).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.CreationDateTime).IsRequired(false).HasConversion(new DateTimeOffsetToBinaryConverter());
            c.HasOne(x => x.Request).WithOne(x => x.StandardBusinessHeader).HasForeignKey<StandardBusinessHeader>("RequestId").IsRequired(true).OnDelete(DeleteBehavior.Cascade);
            c.OwnsMany(x => x.ContactInformations, c =>
            {
                c.ToTable(nameof(ContactInformation));
                c.Property<int>("RequestId");
                c.HasKey("RequestId", nameof(ContactInformation.Type), nameof(ContactInformation.Identifier));
                c.Property(x => x.Type).HasMaxLength(256).HasConversion<short>().IsRequired(true);
                c.Property(x => x.Identifier).HasMaxLength(256).IsRequired(true);
                c.Property(x => x.Contact).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.EmailAddress).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.FaxNumber).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.TelephoneNumber).IsRequired(false);
                c.Property(x => x.ContactTypeIdentifier).IsRequired(false);
                c.HasOne(x => x.Header).WithMany(x => x.ContactInformations).HasForeignKey("RequestId").OnDelete(DeleteBehavior.Cascade);
            });
        });
        request.OwnsOne(x => x.SubscriptionCallback, c =>
        {
            c.ToTable(nameof(SubscriptionCallback));
            c.Property<int>("RequestId");
            c.HasKey("RequestId");
            c.HasOne(x => x.Request).WithOne(x => x.SubscriptionCallback).HasForeignKey<SubscriptionCallback>("RequestId").OnDelete(DeleteBehavior.Cascade);
            c.Property(x => x.CallbackType).IsRequired(true).HasConversion<short>();
            c.Property(x => x.Reason).IsRequired(false);
            c.Property(x => x.SubscriptionId).IsRequired(true).HasMaxLength(256);
        });

        var masterData = modelBuilder.Entity<MasterData>();
        masterData.ToView("CurrentMasterdata");
        masterData.ToTable(nameof(MasterData));
        masterData.Property<int>("RequestId");
        masterData.HasKey("RequestId", nameof(MasterData.Type), nameof(MasterData.Id));
        masterData.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
        masterData.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        masterData.OwnsMany(x => x.Attributes, c =>
        {
            c.ToTable(nameof(MasterDataAttribute));
            c.Property<int>("RequestId");
            c.Property<string>("MasterdataType").HasMaxLength(256);
            c.Property<string>("MasterdataId").HasMaxLength(256);
            c.HasKey("RequestId", "MasterdataType", "MasterdataId", nameof(MasterDataAttribute.Id));
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Value).HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.MasterData).WithMany(x => x.Attributes).HasForeignKey("RequestId", "MasterdataType", "MasterdataId").OnDelete(DeleteBehavior.Cascade);
            c.OwnsMany(x => x.Fields, c =>
            {
                c.ToTable(nameof(MasterDataField));
                c.Property<int>("RequestId");
                c.Property<string>("MasterdataType").HasMaxLength(256);
                c.Property<string>("MasterdataId").HasMaxLength(256);
                c.Property<string>("AttributeId").HasMaxLength(256);
                c.HasKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId", nameof(MasterDataField.Namespace), nameof(MasterDataField.Name));
                c.Property(x => x.ParentName).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.ParentNamespace).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.Namespace).HasMaxLength(256).IsRequired(true);
                c.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
                c.Property(x => x.Value).HasMaxLength(256).IsRequired(false);
                c.HasOne(x => x.Attribute).WithMany(x => x.Fields).HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId").OnDelete(DeleteBehavior.Cascade);
            });
        });
        masterData.OwnsMany(x => x.Children, c =>
        {
            c.ToTable(nameof(MasterDataChildren));
            c.HasKey("MasterDataRequestId", "MasterDataType", "MasterDataId", "ChildrenId");
            c.HasOne(x => x.MasterData).WithMany(x => x.Children).OnDelete(DeleteBehavior.Cascade);
            c.Property(x => x.ChildrenId).HasMaxLength(256);
        });

        var mdHierarchy = modelBuilder.Entity<MasterDataHierarchy>();
        mdHierarchy.ToView(nameof(MasterDataHierarchy));
        mdHierarchy.Property(x => x.Root).IsRequired();
        mdHierarchy.Property(x => x.Id).IsRequired();
        mdHierarchy.Property(x => x.Type).IsRequired();
        mdHierarchy.HasNoKey();

        var evt = modelBuilder.Entity<Event>();
        evt.ToTable(nameof(Event));
        evt.HasKey(x => x.Id);
        evt.Property(x => x.Id).ValueGeneratedOnAdd();
        evt.Property(x => x.UserId).HasMaxLength(36);
        evt.Property(x => x.CaptureTime).IsRequired(true).HasConversion(new DateTimeOffsetToBinaryConverter());
        evt.Property(x => x.EventTime).IsRequired(true).HasConversion(new DateTimeOffsetToBinaryConverter());
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
        evt.Property(x => x.CorrectiveDeclarationTime).IsRequired(false).HasConversion(new DateTimeOffsetToBinaryConverter()); ;
        evt.Property(x => x.CorrectiveReason).HasMaxLength(256).IsRequired(false);
        evt.HasOne(x => x.Request).WithMany(x => x.Events).HasForeignKey("RequestId").OnDelete(DeleteBehavior.Cascade);
        evt.OwnsMany(x => x.Epcs, c =>
        {
            c.ToTable(nameof(Epc));
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(Epc.Type), nameof(Epc.Id));
            c.Property(x => x.Type).IsRequired(true).HasConversion<short>();
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Quantity).IsRequired(false);
            c.Property(x => x.UnitOfMeasure).IsRequired(false).HasMaxLength(10);
            c.HasOne(x => x.Event).WithMany(x => x.Epcs).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.Sources, c =>
        {
            c.ToTable(nameof(Source));
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(Source.Type), nameof(Source.Id));
            c.Property(x => x.Type).IsRequired(true);
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        });
        evt.OwnsMany(x => x.Destinations, c =>
        {
            c.ToTable(nameof(Destination));
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(Destination.Type), nameof(Destination.Id));
            c.Property(x => x.Type).IsRequired(true);
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        });
        evt.OwnsMany(x => x.Transactions, c =>
        {
            c.ToTable(nameof(BusinessTransaction));
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(BusinessTransaction.Type), nameof(BusinessTransaction.Id));
            c.Property(x => x.Type).IsRequired(true);
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.Event).WithMany(x => x.Transactions).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.PersistentDispositions, c =>
        {
            c.ToTable(nameof(PersistentDisposition));
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(PersistentDisposition.Type), nameof(PersistentDisposition.Id));
            c.Property(x => x.Type).IsRequired(true);
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.Event).WithMany(x => x.PersistentDispositions).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.SensorElements, c =>
        {
            c.ToTable(nameof(SensorElement));
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(SensorElement.Index));
            c.Property(x => x.Index).IsRequired(true).ValueGeneratedNever();
            c.HasOne(x => x.Event).WithMany(x => x.SensorElements).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);
            c.OwnsMany(x => x.Reports, c =>
            {
                c.ToTable(nameof(SensorReport));
                c.Property<int>("EventId");
                c.Property<int>("SensorIndex").IsRequired(true);
                c.HasKey("EventId", "SensorIndex", nameof(SensorReport.Index));
                c.Property(x => x.Index).IsRequired(true).ValueGeneratedNever();
                c.HasOne(x => x.SensorElement).WithMany(x => x.Reports).HasForeignKey("EventId", "SensorIndex").OnDelete(DeleteBehavior.Cascade);
            });
        });
        evt.OwnsMany(x => x.Fields, c =>
        {
            c.ToTable(nameof(Field));
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(Field.Index));
            c.Property(x => x.Index).IsRequired(true).ValueGeneratedNever();
            c.Property(x => x.Type).IsRequired(true).HasConversion<short>();
            c.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Namespace).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.TextValue).IsRequired(false);
            c.Property(x => x.NumericValue).IsRequired(false);
            c.Property(x => x.DateValue).IsRequired(false).HasConversion(new DateTimeOffsetToBinaryConverter());
            c.HasOne(x => x.Event).WithMany(x => x.Fields).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.CorrectiveEventIds, c =>
        {
            c.ToTable(nameof(CorrectiveEventId));
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(CorrectiveEventId.CorrectiveId));
            c.Property(x => x.CorrectiveId).IsRequired(true).HasMaxLength(256);
        });

        var subscription = modelBuilder.Entity<Subscription>();
        subscription.ToTable(nameof(Subscription), builder => builder.HasTrigger("SubscriptionInitialRequests"));
        subscription.HasKey(x => x.Id);
        subscription.Property(x => x.Id).ValueGeneratedOnAdd();
        subscription.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.QueryName).IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.ReportIfEmpty).IsRequired(true);
        subscription.Property(x => x.Trigger).IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.SignatureToken).IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.FormatterName).IsRequired(true).HasMaxLength(30);
        subscription.OwnsOne(x => x.Schedule, c =>
        {
            c.ToTable(nameof(SubscriptionSchedule));
            c.Property(x => x.Second).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Minute).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Hour).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.DayOfWeek).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.DayOfMonth).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Month).HasMaxLength(256).IsRequired(false);
        });
        subscription.OwnsMany(x => x.Parameters, c =>
        {
            c.ToTable(nameof(SubscriptionParameter));
            c.Property<int>("SubscriptionId");
            c.HasKey("SubscriptionId", nameof(SubscriptionParameter.Name));
            c.HasOne(x => x.Subscription).WithMany(x => x.Parameters).HasForeignKey("SubscriptionName").OnDelete(DeleteBehavior.Cascade);
            c.Property(x => x.Values).IsRequired(false).HasConversion<ArrayConverter, ArrayComparer>();
        });
        subscription.HasIndex(x => x.Name).IsUnique();

        var executionRecord = modelBuilder.Entity<SubscriptionExecutionRecord>();
        executionRecord.ToTable(nameof(SubscriptionExecutionRecord));
        executionRecord.HasKey("SubscriptionId", "ExecutionTime");
        executionRecord.Property(x => x.SubscriptionId).IsRequired(true);
        executionRecord.Property(x => x.ExecutionTime).IsRequired(true).HasConversion(new DateTimeOffsetToBinaryConverter()); ;
        executionRecord.Property(x => x.ResultsSent).IsRequired(true);
        executionRecord.Property(x => x.Successful).IsRequired(true);
        executionRecord.Property(x => x.Reason).IsRequired(false);

        var pendingRequests = modelBuilder.Entity<PendingRequest>();
        pendingRequests.ToTable(nameof(PendingRequest));
        pendingRequests.Property(x => x.SubscriptionId);
        pendingRequests.Property(x => x.RequestId);
        pendingRequests.HasKey(nameof(PendingRequest.SubscriptionId), nameof(PendingRequest.RequestId));

        var storedQuery = modelBuilder.Entity<StoredQuery>();
        storedQuery.ToTable(nameof(StoredQuery));
        storedQuery.HasKey(x => x.Id);
        storedQuery.Property(x => x.Id).ValueGeneratedOnAdd();
        storedQuery.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
        storedQuery.HasIndex(x => x.Name).IsUnique();
        storedQuery.Property(x => x.DataSource).IsRequired(true).HasMaxLength(30);
        storedQuery.Property(x => x.UserId).IsRequired(false).HasMaxLength(80);
        storedQuery.OwnsMany(x => x.Parameters, c =>
        {
            c.ToTable(nameof(StoredQueryParameter));
            c.Property<string>("QueryName");
            c.HasKey("QueryName", nameof(StoredQueryParameter.Name));
            c.Property(x => x.Values).IsRequired(false).HasConversion<ArrayConverter, ArrayComparer>();
        });
        storedQuery.HasData
        (
            new { Id = -2, Name = "SimpleEventQuery", DataSource = "SimpleEventQuery" },
            new { Id = -1, Name = "SimpleMasterDataQuery", DataSource = "SimpleMasterDataQuery" }
        );
        storedQuery.HasIndex(x => x.Name).IsUnique();
    }
}