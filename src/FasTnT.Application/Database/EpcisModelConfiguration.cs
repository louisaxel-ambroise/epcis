using FasTnT.Application.Database;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;

namespace FasTnT.Application.Database;

public static class EpcisModelConfiguration
{
    const string Epcis = nameof(Epcis);
    const string Sbdh = nameof(Sbdh);
    const string Cbv = nameof(Cbv);
    const string Subscriptions = nameof(Subscriptions);
    const string Queries = nameof(Queries);

    public static void Apply(ModelBuilder modelBuilder)
    {
        var request = modelBuilder.Entity<Request>();
        request.ToTable(nameof(Request), Epcis, builder => builder.HasTrigger("SubscriptionPendingRequests"));
        request.HasKey(x => x.Id);
        request.Property(x => x.Id).ValueGeneratedOnAdd();
        request.Property(x => x.UserId).HasMaxLength(50);
        request.Property(x => x.DocumentTime).IsRequired();
        request.Property(x => x.CaptureTime).IsRequired();
        request.Property(x => x.CaptureId).IsRequired().HasMaxLength(256);
        request.Property(x => x.SchemaVersion).IsRequired(true).HasMaxLength(10);
        request.HasMany(x => x.Events).WithOne(x => x.Request).HasForeignKey("RequestId").OnDelete(DeleteBehavior.Cascade);
        request.HasMany(x => x.Masterdata).WithOne(x => x.Request).HasForeignKey("RequestId").OnDelete(DeleteBehavior.Cascade);
        request.OwnsOne(x => x.StandardBusinessHeader, c =>
        {
            c.ToTable(nameof(StandardBusinessHeader), Sbdh);
            c.HasKey("RequestId");
            c.Property<int>("RequestId");
            c.Property(x => x.Version).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Standard).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.TypeVersion).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.InstanceIdentifier).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.CreationDateTime).IsRequired(false);
            c.OwnsMany(x => x.ContactInformations, c =>
            {
                c.ToTable(nameof(ContactInformation), Sbdh);
                c.Property<int>("RequestId");
                c.HasKey("RequestId", nameof(ContactInformation.Type), nameof(ContactInformation.Identifier));
                c.Property(x => x.Type).HasMaxLength(256).HasConversion<short>().IsRequired(true);
                c.Property(x => x.Identifier).HasMaxLength(256).IsRequired(true);
                c.Property(x => x.Contact).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.EmailAddress).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.FaxNumber).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.TelephoneNumber).HasMaxLength(256).IsRequired(false);
                c.Property(x => x.ContactTypeIdentifier).HasMaxLength(256).IsRequired(false);
            });
        });
        request.OwnsOne(x => x.SubscriptionCallback, c =>
        {
            c.ToTable(nameof(SubscriptionCallback), Epcis);
            c.Property<int>("RequestId");
            c.HasKey("RequestId");
            c.HasOne(x => x.Request).WithOne(x => x.SubscriptionCallback).HasForeignKey<SubscriptionCallback>("RequestId");
            c.Property(x => x.CallbackType).IsRequired(true).HasConversion<short>();
            c.Property(x => x.Reason).IsRequired(false).HasMaxLength(256);
            c.Property(x => x.SubscriptionId).IsRequired(true).HasMaxLength(256);
        });

        var masterData = modelBuilder.Entity<MasterData>();
        masterData.ToView("CurrentMasterdata", Cbv);
        masterData.ToTable(nameof(MasterData), Cbv);
        masterData.Property<int>("RequestId");
        masterData.HasKey("RequestId", nameof(MasterData.Type), nameof(MasterData.Id));
        masterData.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
        masterData.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        masterData.OwnsMany(x => x.Attributes, c =>
        {
            c.ToTable(nameof(MasterDataAttribute), Cbv);
            c.Property<int>("RequestId");
            c.Property<string>("MasterdataType").HasMaxLength(256);
            c.Property<string>("MasterdataId").HasMaxLength(256);
            c.HasKey("RequestId", "MasterdataType", "MasterdataId", nameof(MasterDataAttribute.Id));
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Value).HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.MasterData).WithMany(x => x.Attributes).HasForeignKey("RequestId", "MasterdataType", "MasterdataId");
            c.OwnsMany(x => x.Fields, c =>
            {
                c.ToTable(nameof(MasterDataField), Cbv);
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
                c.HasOne(x => x.Attribute).WithMany(x => x.Fields).HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId");
            });
        });
        masterData.OwnsMany(x => x.Children, c =>
        {
            c.ToTable(nameof(MasterDataChildren), Cbv);
            c.HasKey("MasterDataRequestId", "MasterDataType", "MasterDataId", "ChildrenId");
            c.HasOne(x => x.MasterData).WithMany(x => x.Children);
            c.Property(x => x.ChildrenId).HasMaxLength(256);
        });
        masterData.HasDiscriminator(x => x.Type)
            .HasValue<BizLocation>("urn:epcglobal:epcis:vtype:BusinessLocation")
            .HasValue<ReadPoint>("urn:epcglobal:epcis:vtype:ReadPoint")
            .IsComplete(false);

        var mdHierarchy = modelBuilder.Entity<MasterDataHierarchy>();
        mdHierarchy.ToView(nameof(MasterDataHierarchy), Cbv);
        mdHierarchy.HasNoKey();
        mdHierarchy.Property(x => x.Root).IsRequired(true);
        mdHierarchy.Property(x => x.Id).IsRequired(true);
        mdHierarchy.Property(x => x.Type).IsRequired(true);
        mdHierarchy.HasDiscriminator(x => x.Type)
            .HasValue<BizLocationHierarchy>("urn:epcglobal:epcis:vtype:BusinessLocation")
            .HasValue<ReadPointHierarchy>("urn:epcglobal:epcis:vtype:ReadPoint")
            .IsComplete(false);

        var evt = modelBuilder.Entity<Event>();
        evt.ToTable(nameof(Event), Epcis);
        evt.HasKey(x => x.Id);
        evt.Property(x => x.Id).ValueGeneratedOnAdd();
        evt.Property(x => x.Type).IsRequired(true).HasConversion<short>();
        evt.Property(x => x.EventTime).IsRequired(true);
        evt.Property(x => x.CaptureTime).IsRequired(true);
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
        evt.HasOne(x => x.Request).WithMany(x => x.Events).HasForeignKey("RequestId");
        evt.OwnsMany(x => x.Epcs, c =>
        {
            c.ToTable(nameof(Epc), Epcis);
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(Epc.Type), nameof(Epc.Id));
            c.Property(x => x.Type).IsRequired(true).HasConversion<short>();
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Quantity).IsRequired(false);
            c.Property(x => x.UnitOfMeasure).IsRequired(false).HasMaxLength(10);
        });
        evt.OwnsMany(x => x.Sources, c =>
        {
            c.ToTable(nameof(Source), Epcis);
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(Source.Type), nameof(Source.Id));
            c.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        });
        evt.OwnsMany(x => x.Destinations, c =>
        {
            c.ToTable(nameof(Destination), Epcis);
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(Destination.Type), nameof(Destination.Id));
            c.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        });
        evt.OwnsMany(x => x.Transactions, c =>
        {
            c.ToTable(nameof(BusinessTransaction), Epcis);
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(BusinessTransaction.Type), nameof(BusinessTransaction.Id));
            c.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        });
        evt.OwnsMany(x => x.PersistentDispositions, c =>
        {
            c.ToTable(nameof(PersistentDisposition), Epcis);
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(PersistentDisposition.Type), nameof(PersistentDisposition.Id));
            c.Property(x => x.Type).HasConversion<short>().IsRequired(true);
            c.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        });
        evt.OwnsMany(x => x.SensorElements, c =>
        {
            c.ToTable(nameof(SensorElement), Epcis);
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(SensorElement.Index));
            c.Property(x => x.Index).IsRequired(true).ValueGeneratedNever();
            c.Property(x => x.DeviceMetadata).HasMaxLength(256);
            c.Property(x => x.RawData).HasMaxLength(2048);
            c.Property(x => x.DataProcessingMethod).HasMaxLength(256);
            c.Property(x => x.DeviceMetadata).HasMaxLength(256);
            c.Property(x => x.BizRules).HasMaxLength(256);
            c.Property(x => x.DeviceId).HasMaxLength(256);
            c.Property(x => x.DeviceMetadata).HasMaxLength(256);
            c.OwnsMany(x => x.Reports, c =>
            {
                c.ToTable(nameof(SensorReport), Epcis);
                c.Property<int>("EventId");
                c.Property<int>("SensorIndex").IsRequired(true);
                c.HasKey("EventId", "SensorIndex", nameof(SensorReport.Index));
                c.Property(x => x.Index).IsRequired(true).ValueGeneratedNever();
                c.Property(x => x.DataProcessingMethod).HasMaxLength(256);
                c.Property(x => x.Type).HasMaxLength(256);
                c.Property(x => x.HexBinaryValue).HasMaxLength(256);
                c.Property(x => x.DeviceMetadata).HasMaxLength(256);
                c.Property(x => x.ChemicalSubstance).HasMaxLength(256);
                c.Property(x => x.Component).HasMaxLength(256);
                c.Property(x => x.DeviceId).HasMaxLength(256);
                c.Property(x => x.DeviceMetadata).HasMaxLength(256);
                c.Property(x => x.Microorganism).HasMaxLength(256);
                c.Property(x => x.RawData).HasMaxLength(2048);
                c.Property(x => x.StringValue).HasMaxLength(2048);
                c.Property(x => x.Type).HasMaxLength(256);
                c.Property(x => x.UnitOfMeasure).HasMaxLength(256);
                c.Property(x => x.UriValue).HasMaxLength(2048);
            });
        });
        evt.OwnsMany(x => x.Fields, c =>
        {
            c.ToTable(nameof(Field), Epcis);
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(Field.Index));
            c.Property(x => x.Index).IsRequired(true).ValueGeneratedNever();
            c.Property(x => x.Type).HasConversion<short>().IsRequired(true);
            c.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Namespace).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.TextValue).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.NumericValue).IsRequired(false);
            c.Property(x => x.DateValue).IsRequired(false);
        });
        evt.OwnsMany(x => x.CorrectiveEventIds, c =>
        {
            c.ToTable(nameof(CorrectiveEventId), Epcis);
            c.Property<int>("EventId");
            c.HasKey("EventId", nameof(CorrectiveEventId.CorrectiveId));
            c.Property(x => x.CorrectiveId).IsRequired(true).HasMaxLength(256);
        });

        var subscription = modelBuilder.Entity<Subscription>();
        subscription.HasKey(x => x.Id);
        subscription.Property(x => x.Id).ValueGeneratedOnAdd();
        subscription.ToTable(nameof(Subscription), Subscriptions, builder => builder.HasTrigger("SubscriptionInitialRequests"));
        subscription.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.QueryName).IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.Destination).IsRequired(true).HasMaxLength(2048);
        subscription.Property(x => x.InitialRecordTime);
        subscription.Property(x => x.ReportIfEmpty).IsRequired(true);
        subscription.Property(x => x.Trigger).IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.SignatureToken).IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.FormatterName).IsRequired(true).HasMaxLength(30);
        subscription.OwnsOne(x => x.Schedule, c =>
        {
            c.ToTable(nameof(SubscriptionSchedule), Subscriptions);
            c.Property<int>("SubscriptionId");
            c.HasKey("SubscriptionId");
            c.Property(x => x.Second).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Minute).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Hour).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.DayOfWeek).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.DayOfMonth).HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Month).HasMaxLength(256).IsRequired(false);
        });
        subscription.OwnsMany(x => x.Parameters, c =>
        {
            c.ToTable("SubscriptionParameter", Subscriptions);
            c.WithOwner().HasForeignKey("SubscriptionId");
            c.HasKey("SubscriptionId", nameof(QueryParameter.Name));
            c.Property(x => x.Values).IsRequired(false).HasJsonArrayConversion();
            c.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
        });
        subscription.HasIndex(x => x.Name).IsUnique();

        var executionRecord = modelBuilder.Entity<SubscriptionExecutionRecord>();
        executionRecord.ToTable(nameof(SubscriptionExecutionRecord), Subscriptions);
        executionRecord.HasKey("SubscriptionId", "ExecutionTime");
        executionRecord.Property(x => x.SubscriptionId).IsRequired(true);
        executionRecord.Property(x => x.ExecutionTime).IsRequired(true);
        executionRecord.Property(x => x.ResultsSent).IsRequired(true);
        executionRecord.Property(x => x.Successful).IsRequired(true);
        executionRecord.Property(x => x.Reason).IsRequired(false);

        var pendingRequests = modelBuilder.Entity<PendingRequest>();
        pendingRequests.ToTable(nameof(PendingRequest), Subscriptions);
        pendingRequests.Property(x => x.SubscriptionId).IsRequired();
        pendingRequests.Property(x => x.RequestId).IsRequired();
        pendingRequests.HasKey("SubscriptionId", "RequestId");

        var storedQuery = modelBuilder.Entity<StoredQuery>();
        storedQuery.ToTable(nameof(StoredQuery), Queries);
        storedQuery.HasKey(x => x.Id);
        storedQuery.Property(x => x.Id).ValueGeneratedOnAdd();
        storedQuery.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
        storedQuery.Property(x => x.UserId).IsRequired(false).HasMaxLength(50);
        storedQuery.OwnsMany(x => x.Parameters, c =>
        {
            c.ToTable("StoredQueryParameter", Queries);
            c.WithOwner().HasForeignKey("QueryId");
            c.HasKey("QueryId", nameof(QueryParameter.Name));
            c.Property(x => x.Values).IsRequired(false).HasJsonArrayConversion();
            c.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
        });
        storedQuery.HasIndex(x => x.Name).IsUnique();
    }

    private static PropertyBuilder<T[]> HasJsonArrayConversion<T>(this PropertyBuilder<T[]> builder, JsonSerializerOptions options = null)
    {
        var converter = new ValueConverter<T[], string>(v => Serialize(v, options), v => Deserialize<T[]>(v, options));
        var comparer = new ValueComparer<T[]>((l, r) => Equals(l, r), v => Serialize(v ?? Array.Empty<T>(), options).GetHashCode(), v => v);

        builder.HasConversion(converter);
        builder.Metadata.SetValueConverter(converter);
        builder.Metadata.SetValueComparer(comparer);

        return builder;
    }
}
