using FasTnT.Application.EfCore.Services.Queries;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FasTnT.Application.EfCore.Store.Configuration;

internal static class EpcisModelConfiguration
{
    internal static void Apply(ModelBuilder modelBuilder, DatabaseFacade database)
    {
        modelBuilder.HasDbFunction(typeof(EpcisContext).GetMethod(nameof(EpcisContext.MasterdataHierarchy), new[] { typeof(string), typeof(string) }), builder => builder.HasSchema(nameof(Schemas.Cbv)));
        modelBuilder.HasDbFunction(typeof(EpcisContext).GetMethod(nameof(EpcisContext.MasterdataProperty), new[] { typeof(string), typeof(string), typeof(string) }), builder => builder.HasSchema(nameof(Schemas.Cbv)));

        var request = modelBuilder.Entity<Request>();
        request.ToTable(nameof(Request), nameof(Schemas.Epcis), builder => builder.HasTrigger("SubscriptionPendingRequests"));
        request.Property<int>("Id");
        request.HasKey("Id");
        request.Property(x => x.UserId).HasMaxLength(50);
        request.Property(x => x.DocumentTime);
        request.Property(x => x.CaptureDate);
        request.Property(x => x.SchemaVersion).IsRequired(true);
        request.HasMany(x => x.Events).WithOne(x => x.Request).HasForeignKey("RequestId");
        request.HasMany(x => x.Masterdata).WithOne(x => x.Request).HasForeignKey("RequestId");
        request.HasOne(x => x.StandardBusinessHeader).WithOne(x => x.Request).HasForeignKey<StandardBusinessHeader>("RequestId").IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        request.HasOne(x => x.SubscriptionCallback).WithOne(x => x.Request).HasForeignKey<SubscriptionCallback>("RequestId").IsRequired(false).OnDelete(DeleteBehavior.Cascade);

        var header = modelBuilder.Entity<StandardBusinessHeader>();
        header.ToTable(nameof(StandardBusinessHeader), nameof(Schemas.Sbdh));
        header.HasKey("RequestId");
        header.Property<int>("RequestId");
        header.Property(x => x.Version).HasMaxLength(256).IsRequired(true);
        header.Property(x => x.Standard).HasMaxLength(256).IsRequired(true);
        header.Property(x => x.TypeVersion).HasMaxLength(256).IsRequired(true);
        header.Property(x => x.InstanceIdentifier).HasMaxLength(256).IsRequired(true);
        header.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
        header.Property(x => x.CreationDateTime).IsRequired(false);
        header.HasOne(x => x.Request).WithOne(x => x.StandardBusinessHeader).HasForeignKey<StandardBusinessHeader>("RequestId").IsRequired(true).OnDelete(DeleteBehavior.Cascade);
        header.HasMany(x => x.ContactInformations).WithOne(x => x.Header).HasForeignKey("RequestId");

        var contactInfo = modelBuilder.Entity<ContactInformation>();
        contactInfo.ToTable(nameof(ContactInformation), nameof(Schemas.Sbdh));
        contactInfo.HasKey("RequestId", nameof(ContactInformation.Type), nameof(ContactInformation.Identifier));
        contactInfo.Property<int>("RequestId");
        contactInfo.Property(x => x.Type).HasMaxLength(256).HasConversion<short>().IsRequired(true);
        contactInfo.Property(x => x.Identifier).HasMaxLength(256).IsRequired(true);
        contactInfo.Property(x => x.Contact).HasMaxLength(256).IsRequired(false);
        contactInfo.Property(x => x.EmailAddress).HasMaxLength(256).IsRequired(false);
        contactInfo.Property(x => x.FaxNumber).HasMaxLength(256).IsRequired(false);
        contactInfo.Property(x => x.TelephoneNumber).IsRequired(false);
        contactInfo.Property(x => x.ContactTypeIdentifier).IsRequired(false);
        contactInfo.HasOne(x => x.Header).WithMany(x => x.ContactInformations).HasForeignKey("RequestId").OnDelete(DeleteBehavior.Cascade);

        var masterData = modelBuilder.Entity<MasterData>();
        masterData.ToView("CurrentMasterdata", nameof(Schemas.Cbv));
        masterData.ToTable(nameof(MasterData), nameof(Schemas.Cbv));
        masterData.HasKey("RequestId", nameof(MasterData.Type), nameof(MasterData.Id));
        masterData.Property<int>("RequestId");
        masterData.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
        masterData.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        masterData.HasMany(x => x.Attributes).WithOne(x => x.MasterData).HasForeignKey("RequestId", "MasterdataType", "MasterdataId");
        masterData.HasMany(x => x.Children).WithOne(x => x.MasterData).IsRequired(false);
        masterData.HasMany(x => x.Hierarchies).WithOne(x => x.MasterData).IsRequired(false);

        var mdHierarchy = modelBuilder.Entity<MasterDataHierarchy>();
        mdHierarchy.ToView(nameof(MasterDataHierarchy), nameof(Schemas.Cbv));
        mdHierarchy.HasOne(x => x.MasterData).WithMany(x => x.Hierarchies).HasForeignKey("RequestId", "Type", "Id").OnDelete(DeleteBehavior.Cascade);
        mdHierarchy.Property(x => x.ParentId);

        var mdChildren = modelBuilder.Entity<MasterDataChildren>();
        mdChildren.ToTable(nameof(MasterDataChildren), nameof(Schemas.Cbv));
        mdChildren.HasKey("MasterDataRequestId", "MasterDataType", "MasterDataId", "ChildrenId");
        mdChildren.HasOne(x => x.MasterData).WithMany(x => x.Children).OnDelete(DeleteBehavior.Cascade);
        mdChildren.Property(x => x.ChildrenId).HasMaxLength(256);

        var mdAttribute = modelBuilder.Entity<MasterDataAttribute>();
        mdAttribute.ToTable(nameof(MasterDataAttribute), nameof(Schemas.Cbv));
        mdAttribute.HasKey("RequestId", "MasterdataType", "MasterdataId", nameof(MasterDataAttribute.Id));
        mdAttribute.Property<int>("RequestId");
        mdAttribute.Property<string>("MasterdataType").HasMaxLength(256);
        mdAttribute.Property<string>("MasterdataId").HasMaxLength(256);
        mdAttribute.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        mdAttribute.Property(x => x.Value).HasMaxLength(256).IsRequired(true);
        mdAttribute.HasOne(x => x.MasterData).WithMany(x => x.Attributes).HasForeignKey("RequestId", "MasterdataType", "MasterdataId").OnDelete(DeleteBehavior.Cascade);
        mdAttribute.HasMany(x => x.Fields).WithOne(x => x.Attribute).HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId");

        var mdField = modelBuilder.Entity<MasterDataField>();
        mdField.ToTable(nameof(MasterDataField), nameof(Schemas.Cbv));
        mdField.HasKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId", nameof(MasterDataField.Namespace), nameof(MasterDataField.Name));
        mdField.Property<int>("RequestId");
        mdField.Property<string>("MasterdataType").HasMaxLength(256);
        mdField.Property<string>("MasterdataId").HasMaxLength(256);
        mdField.Property<string>("AttributeId").HasMaxLength(256);
        mdField.Property(x => x.ParentName).HasMaxLength(256).IsRequired(false);
        mdField.Property(x => x.ParentNamespace).HasMaxLength(256).IsRequired(false);
        mdField.Property(x => x.Namespace).HasMaxLength(256).IsRequired(true);
        mdField.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
        mdField.Property(x => x.Value).HasMaxLength(256).IsRequired(false);
        mdField.HasOne(x => x.Attribute).WithMany(x => x.Fields).HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId").OnDelete(DeleteBehavior.Cascade);

        var callback = modelBuilder.Entity<SubscriptionCallback>();
        callback.ToTable(nameof(SubscriptionCallback), nameof(Schemas.Epcis));
        callback.Property<int>("RequestId");
        callback.HasKey("RequestId");
        callback.HasOne(x => x.Request).WithOne(x => x.SubscriptionCallback).HasForeignKey<SubscriptionCallback>("RequestId").OnDelete(DeleteBehavior.Cascade);
        callback.Property(x => x.CallbackType).IsRequired(true).HasConversion<short>();
        callback.Property(x => x.Reason).IsRequired(false);
        callback.Property(x => x.SubscriptionId).IsRequired(true).HasMaxLength(256);

        var evt = modelBuilder.Entity<Event>();
        evt.ToTable(nameof(Event), nameof(Schemas.Epcis));
        evt.Property<long>("Id").IsRequired(true).UseIdentityColumn(1, 1);
        evt.HasKey("Id");
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
        evt.HasMany(x => x.Epcs).WithOne(x => x.Event).HasForeignKey("EventId");
        evt.HasMany(x => x.Sources).WithOne(x => x.Event).HasForeignKey("EventId");
        evt.HasMany(x => x.Destinations).WithOne(x => x.Event).HasForeignKey("EventId");
        evt.HasMany(x => x.Transactions).WithOne(x => x.Event).HasForeignKey("EventId");
        evt.HasMany(x => x.PersistentDispositions).WithOne(x => x.Event).HasForeignKey("EventId");
        evt.HasMany(x => x.SensorElements).WithOne(x => x.Event).HasForeignKey("EventId");
        evt.HasMany(x => x.Fields).WithOne(x => x.Event).HasForeignKey("EventId");
        evt.HasMany(x => x.CorrectiveEventIds).WithOne(x => x.Event).HasForeignKey("EventId");
        evt.HasOne(x => x.Request).WithMany(x => x.Events).HasForeignKey("RequestId").OnDelete(DeleteBehavior.Cascade);

        var epc = modelBuilder.Entity<Epc>();
        epc.ToTable(nameof(Epc), nameof(Schemas.Epcis));
        epc.HasKey("EventId", nameof(Epc.Type), nameof(Epc.Id));
        epc.Property<long>("EventId");
        epc.Property(x => x.Type).IsRequired(true).HasConversion<short>();
        epc.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        epc.Property(x => x.Quantity).IsRequired(false);
        epc.Property(x => x.UnitOfMeasure).IsRequired(false).HasMaxLength(10);
        epc.HasOne(x => x.Event).WithMany(x => x.Epcs).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);

        var eventId = modelBuilder.Entity<CorrectiveEventId>();
        eventId.ToTable(nameof(CorrectiveEventId), nameof(Schemas.Epcis));
        eventId.HasKey("EventId", nameof(CorrectiveEventId.CorrectiveId));
        eventId.Property<long>("EventId");
        eventId.Property(x => x.CorrectiveId).IsRequired(true).HasMaxLength(256);

        var source = modelBuilder.Entity<Source>();
        source.ToTable(nameof(Source), nameof(Schemas.Epcis));
        source.Property<long>("EventId");
        source.HasKey("EventId", nameof(Source.Type), nameof(Source.Id));
        source.Property(x => x.Type).IsRequired(true);
        source.Property(x => x.Id).HasMaxLength(256).IsRequired(true);

        var dest = modelBuilder.Entity<Destination>();
        dest.ToTable(nameof(Destination), nameof(Schemas.Epcis));
        dest.Property<long>("EventId");
        dest.HasKey("EventId", nameof(Destination.Type), nameof(Destination.Id));
        dest.Property(x => x.Type).IsRequired(true);
        dest.Property(x => x.Id).HasMaxLength(256).IsRequired(true);

        var bizTrans = modelBuilder.Entity<BusinessTransaction>();
        bizTrans.ToTable(nameof(BusinessTransaction), nameof(Schemas.Epcis));
        bizTrans.HasKey("EventId", nameof(BusinessTransaction.Type), nameof(BusinessTransaction.Id));
        bizTrans.Property<long>("EventId");
        bizTrans.Property(x => x.Type).IsRequired(true);
        bizTrans.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        bizTrans.HasOne(x => x.Event).WithMany(x => x.Transactions).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);

        var customField = modelBuilder.Entity<Field>();
        customField.ToTable(nameof(Field), nameof(Schemas.Epcis));
        customField.Property<int>("FieldId").IsRequired(true).HasValueGenerator<IncrementGenerator>();
        customField.Property<long>("EventId");
        customField.Property<int?>("ParentId");
        customField.HasKey("EventId", "FieldId");
        customField.Property(x => x.Type).IsRequired(true).HasConversion<short>();
        customField.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
        customField.Property(x => x.Namespace).HasMaxLength(256).IsRequired(false);
        customField.Property(x => x.TextValue).IsRequired(false);
        customField.Property(x => x.NumericValue).IsRequired(false);
        customField.Property(x => x.DateValue).IsRequired(false);
        customField.HasOne(x => x.Event).WithMany(x => x.Fields).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);
        customField.HasOne(x => x.Report).WithMany(x => x.Fields).IsRequired(false).HasForeignKey("EventId", "SensorId", "ReportId").OnDelete(DeleteBehavior.NoAction);
        customField.HasOne(x => x.Element).WithMany(x => x.Fields).IsRequired(false).HasForeignKey("EventId", "SensorId").OnDelete(DeleteBehavior.NoAction);
        customField.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey("EventId", "ParentId").OnDelete(DeleteBehavior.NoAction).IsRequired(false);

        var persistentDisposition = modelBuilder.Entity<PersistentDisposition>();
        persistentDisposition.ToTable(nameof(PersistentDisposition), nameof(Schemas.Epcis));
        persistentDisposition.Property<long>("EventId");
        persistentDisposition.HasKey("EventId", nameof(PersistentDisposition.Type), nameof(PersistentDisposition.Id));
        persistentDisposition.Property(x => x.Type).IsRequired(true);
        persistentDisposition.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
        persistentDisposition.HasOne(x => x.Event).WithMany(x => x.PersistentDispositions).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);

        var sensorElement = modelBuilder.Entity<SensorElement>();
        sensorElement.ToTable(nameof(SensorElement), nameof(Schemas.Epcis));
        sensorElement.Property<long>("EventId");
        sensorElement.Property<int>("SensorId").IsRequired(true).HasValueGenerator<IncrementGenerator>();
        sensorElement.HasKey("EventId", "SensorId");
        sensorElement.HasOne(x => x.Event).WithMany(x => x.SensorElements).HasForeignKey("EventId").OnDelete(DeleteBehavior.Cascade);
        sensorElement.HasMany(x => x.Reports).WithOne(x => x.SensorElement).HasForeignKey("EventId");

        var sensorReport = modelBuilder.Entity<SensorReport>();
        sensorReport.ToTable(nameof(SensorReport), nameof(Schemas.Epcis));
        sensorReport.Property<long>("EventId");
        sensorReport.Property<int>("SensorId").IsRequired(true).HasValueGenerator<IncrementGenerator>();
        sensorReport.Property<int>("ReportId").IsRequired(true).HasValueGenerator<IncrementGenerator>();
        sensorReport.HasKey("EventId", "SensorId", "ReportId");
        sensorReport.HasOne(x => x.SensorElement).WithMany(x => x.Reports).HasForeignKey("EventId", "SensorId").OnDelete(DeleteBehavior.Cascade);

        var subscription = modelBuilder.Entity<Subscription>();
        subscription.ToTable(nameof(Subscription), nameof(Schemas.Subscription), builder => builder.HasTrigger("SubscriptionInitialRequests"));
        subscription.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.QueryName).IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.ReportIfEmpty).IsRequired(true);
        subscription.Property(x => x.Trigger).IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.SignatureToken).IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.FormatterName).IsRequired(true).HasMaxLength(30);
        subscription.HasMany(x => x.Parameters).WithOne(x => x.Subscription).HasForeignKey("SubscriptionId");
        subscription.HasOne(x => x.Schedule).WithOne(x => x.Subscription).HasForeignKey<SubscriptionSchedule>("SubscriptionId").IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        subscription.HasOne(x => x.Query).WithMany().HasForeignKey("QueryId").IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        subscription.HasMany(x => x.ExecutionRecords).WithOne(x => x.Subscription);

        var subscriptionParam = modelBuilder.Entity<SubscriptionParameter>();
        subscriptionParam.ToTable(nameof(SubscriptionParameter), nameof(Schemas.Subscription));
        subscriptionParam.HasKey("SubscriptionId", nameof(SubscriptionParameter.Name));
        subscriptionParam.HasOne(x => x.Subscription).WithMany(x => x.Parameters).HasForeignKey("SubscriptionId").OnDelete(DeleteBehavior.Cascade);
        subscriptionParam.Property(x => x.Values).IsRequired(false).HasConversion<ArrayConverter, ArrayComparer>();

        var subscriptionSchedule = modelBuilder.Entity<SubscriptionSchedule>();
        subscriptionSchedule.ToTable(nameof(SubscriptionSchedule), nameof(Schemas.Subscription));
        subscriptionSchedule.Property<int>("Id").IsRequired(true);
        subscriptionSchedule.HasKey("Id");
        subscriptionSchedule.Property(x => x.Second).HasMaxLength(256).IsRequired(false);
        subscriptionSchedule.Property(x => x.Minute).HasMaxLength(256).IsRequired(false);
        subscriptionSchedule.Property(x => x.Hour).HasMaxLength(256).IsRequired(false);
        subscriptionSchedule.Property(x => x.DayOfWeek).HasMaxLength(256).IsRequired(false);
        subscriptionSchedule.Property(x => x.DayOfMonth).HasMaxLength(256).IsRequired(false);
        subscriptionSchedule.Property(x => x.Month).HasMaxLength(256).IsRequired(false);
        subscriptionSchedule.HasOne(x => x.Subscription).WithOne(x => x.Schedule).HasForeignKey<SubscriptionSchedule>("SubscriptionId").OnDelete(DeleteBehavior.Cascade);

        var subscriptionExecutionRecord = modelBuilder.Entity<SubscriptionExecutionRecord>();
        subscriptionExecutionRecord.ToTable(nameof(SubscriptionExecutionRecord), nameof(Schemas.Subscription));
        subscriptionExecutionRecord.HasKey("SubscriptionId", "ExecutionTime");
        subscriptionExecutionRecord.Property(x => x.ExecutionTime).IsRequired(true);
        subscriptionExecutionRecord.Property(x => x.ResultsSent).IsRequired(true);
        subscriptionExecutionRecord.Property(x => x.Successful).IsRequired(true);
        subscriptionExecutionRecord.Property(x => x.Reason).IsRequired(false);
        subscriptionExecutionRecord.HasOne(x => x.Subscription).WithMany(x => x.ExecutionRecords).HasForeignKey("SubscriptionId").OnDelete(DeleteBehavior.Cascade);

        var pendingRequest = modelBuilder.Entity<PendingRequest>();
        pendingRequest.ToTable(nameof(PendingRequest), nameof(Schemas.Subscription));
        pendingRequest.HasKey(x => new { x.RequestId, x.SubscriptionId });

        var storedQuery = modelBuilder.Entity<StoredQuery>();
        storedQuery.ToTable(nameof(StoredQuery), nameof(Schemas.Queries));
        storedQuery.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
        storedQuery.Property(x => x.DataSource).IsRequired(true).HasMaxLength(30);
        storedQuery.Property(x => x.UserId).IsRequired(false).HasMaxLength(80);
        storedQuery.HasMany(x => x.Parameters).WithOne(x => x.Query).HasForeignKey("QueryId");
        storedQuery.HasIndex(x => x.Name).IsUnique();
        storedQuery.HasMany(x => x.Subscriptions).WithOne(x => x.Query);
        storedQuery.HasData
        (
            new StoredQuery { Id = -2, Name = nameof(SimpleEventQuery), DataSource = nameof(SimpleEventQuery) },
            new StoredQuery { Id = -1, Name = nameof(SimpleMasterDataQuery), DataSource = nameof(SimpleMasterDataQuery) }
        );

        var customQueryParam = modelBuilder.Entity<StoredQueryParameter>();
        customQueryParam.ToTable(nameof(StoredQueryParameter), nameof(Schemas.Subscription));
        customQueryParam.HasKey("QueryId", nameof(StoredQueryParameter.Name));
        customQueryParam.Property(x => x.Values).IsRequired(false).HasConversion<ArrayConverter, ArrayComparer>();
    }
}
