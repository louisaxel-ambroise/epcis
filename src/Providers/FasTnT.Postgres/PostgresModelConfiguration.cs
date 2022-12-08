using FasTnT.Application.Configuration;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Postgres;

public class PostgresModelConfiguration : IModelConfiguration
{
    const string Epcis = "epcis";
    const string Sbdh = "sbdh";
    const string Cbv = "cbv";
    const string Subscriptions = "subscriptions";
    const string Queries = "queries";

    public void Apply(ModelBuilder modelBuilder)
    {
        var request = modelBuilder.Entity<Request>();
        request.ToTable("request", Epcis, builder => builder.HasTrigger("subscription_pending_requests"));
        request.Property<int>("id");
        request.HasKey("id");
        request.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(50);
        request.Property(x => x.DocumentTime).HasColumnName("document_time");
        request.Property(x => x.CaptureDate).HasColumnName("capture_date");
        request.Property(x => x.SchemaVersion).HasColumnName("schema_version").IsRequired(true);
        request.HasMany(x => x.Events).WithOne(x => x.Request).HasForeignKey("request_id");
        request.HasMany(x => x.Masterdata).WithOne(x => x.Request).HasForeignKey("request_id");
        request.OwnsOne(x => x.StandardBusinessHeader, c =>
        {
            c.ToTable("standard_business_header", Sbdh);
            c.Property<int>("request_id");
            c.HasKey("request_id");
            c.Property(x => x.Version).HasColumnName("version").HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Standard).HasColumnName("standard").HasMaxLength(256).IsRequired(true);
            c.Property(x => x.TypeVersion).HasColumnName("type_version").HasMaxLength(256).IsRequired(true);
            c.Property(x => x.InstanceIdentifier).HasColumnName("instance_identifier").HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Type).HasColumnName("type").HasMaxLength(256).IsRequired(true);
            c.Property(x => x.CreationDateTime).HasColumnName("creation_date_time").IsRequired(false);
            c.HasOne(x => x.Request).WithOne(x => x.StandardBusinessHeader).HasForeignKey<StandardBusinessHeader>("request_id").IsRequired(true).OnDelete(DeleteBehavior.Cascade);
            c.OwnsMany(x => x.ContactInformations, c =>
            {
                c.ToTable("contact_information", Sbdh);
                c.Property<int>("request_id");
                c.HasKey("request_id", "Type", "Identifier");
                c.Property(x => x.Type).HasColumnName("type").HasMaxLength(256).HasConversion<short>().IsRequired(true);
                c.Property(x => x.Identifier).HasColumnName("identifier").HasMaxLength(256).IsRequired(true);
                c.Property(x => x.Contact).HasColumnName("contact").HasMaxLength(256).IsRequired(false);
                c.Property(x => x.EmailAddress).HasColumnName("email_address").HasMaxLength(256).IsRequired(false);
                c.Property(x => x.FaxNumber).HasColumnName("fax_number").HasMaxLength(256).IsRequired(false);
                c.Property(x => x.TelephoneNumber).HasColumnName("telephone_number").IsRequired(false);
                c.Property(x => x.ContactTypeIdentifier).HasColumnName("contact_type_identifier").IsRequired(false);
                c.HasOne(x => x.Header).WithMany(x => x.ContactInformations).HasForeignKey("request_id").OnDelete(DeleteBehavior.Cascade);
            });
        });
        request.OwnsOne(x => x.SubscriptionCallback, c =>
        {
            c.ToTable("subscription_callback", Epcis);
            c.Property<int>("request_id");
            c.HasKey("request_id");
            c.HasOne(x => x.Request).WithOne(x => x.SubscriptionCallback).HasForeignKey<SubscriptionCallback>("request_id").OnDelete(DeleteBehavior.Cascade);
            c.Property(x => x.CallbackType).HasColumnName("callback_type").IsRequired(true).HasConversion<short>();
            c.Property(x => x.Reason).HasColumnName("reason").IsRequired(false);
            c.Property(x => x.SubscriptionId).HasColumnName("subscription_id").IsRequired(true).HasMaxLength(256);
        });

        var masterData = modelBuilder.Entity<MasterData>();
        masterData.ToView("current_masterdata", Cbv);
        masterData.ToTable("masterdata", Cbv);
        masterData.HasKey("request_id", "Type", "Id");
        masterData.Property<int>("request_id");
        masterData.Property(x => x.Type).HasColumnName("type").HasMaxLength(256).IsRequired(true);
        masterData.Property(x => x.Id).HasColumnName("id").HasMaxLength(256).IsRequired(true);
        masterData.OwnsMany(x => x.Attributes, c =>
        {
            c.ToTable(nameof(MasterDataAttribute), Cbv);
            c.Property<int>("request_id");
            c.Property<string>("masterdata_type").HasMaxLength(256);
            c.Property<string>("masterdata_id").HasMaxLength(256);
            c.HasKey("request_id", "masterdata_type", "masterdata_id", "Id");
            c.Property(x => x.Id).HasColumnName("id").HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Value).HasColumnName("value").HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.MasterData).WithMany(x => x.Attributes).HasForeignKey("request_id", "masterdata_type", "masterdata_id").OnDelete(DeleteBehavior.Cascade);
            c.OwnsMany(x => x.Fields, c =>
            {
                c.ToTable("masterdata_field", Cbv);
                c.Property<int>("request_id");
                c.Property<string>("masterdata_type").HasMaxLength(256);
                c.Property<string>("masterdata_id").HasMaxLength(256);
                c.Property<string>("attribute_id").HasMaxLength(256);
                c.HasKey("request_id", "masterdata_type", "masterdata_id", "attribute_id", "Namespace", "Name");
                c.Property(x => x.ParentName).HasColumnName("parent_name").HasMaxLength(256).IsRequired(false);
                c.Property(x => x.ParentNamespace).HasColumnName("parent_namespace").HasMaxLength(256).IsRequired(false);
                c.Property(x => x.Namespace).HasColumnName("namespace").HasMaxLength(256).IsRequired(true);
                c.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired(true);
                c.Property(x => x.Value).HasColumnName("value").HasMaxLength(256).IsRequired(false);
                c.HasOne(x => x.Attribute).WithMany(x => x.Fields).HasForeignKey("request_id", "masterdata_type", "masterdata_id", "attribute_id").OnDelete(DeleteBehavior.Cascade);
            });
        });
        masterData.OwnsMany(x => x.Children, c =>
        {
            c.ToTable("masterdata_children", Cbv);
            c.Property<int>("masterdata_request_id");
            c.Property<string>("masterdata_type").HasMaxLength(256);
            c.Property<string>("masterdata_id").HasMaxLength(256);
            c.HasKey("masterdata_request_id", "masterdata_type", "masterdata_id", "ChildrenId");
            c.HasOne(x => x.MasterData).WithMany(x => x.Children).HasForeignKey("masterdata_request_id", "masterdata_type", "masterdata_id").OnDelete(DeleteBehavior.Cascade);
            c.Property(x => x.ChildrenId).HasColumnName("children_id").HasMaxLength(256);
        });

        var mdHierarchy = modelBuilder.Entity<MasterDataHierarchy>();
        mdHierarchy.ToView("masterdata_hierarchy", Cbv);
        mdHierarchy.Property(x => x.Root).HasColumnName("root").IsRequired();
        mdHierarchy.Property(x => x.Id).HasColumnName("id").IsRequired();
        mdHierarchy.Property(x => x.Type).HasColumnName("type").IsRequired();
        mdHierarchy.HasNoKey();

        var mdProperty = modelBuilder.Entity<MasterDataProperty>();
        mdProperty.ToView("masterdata_property", Cbv);
        mdProperty.HasNoKey();
        mdProperty.Property(x => x.Id).HasColumnName("id").IsRequired();
        mdProperty.Property(x => x.Type).HasColumnName("type").IsRequired();
        mdProperty.Property(x => x.Attribute).HasColumnName("attribute").IsRequired();

        var evt = modelBuilder.Entity<Event>();
        evt.ToTable("event", Epcis);
        evt.Property<int>("id").ValueGeneratedOnAdd();
        evt.HasKey("id");
        evt.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(36);
        evt.Property(x => x.EventTime).HasColumnName("event_time").IsRequired(true);
        evt.Property(x => x.Type).HasColumnName("type").IsRequired(true).HasConversion<short>();
        evt.Property(x => x.EventTimeZoneOffset).HasColumnName("event_timezone_offset").IsRequired(true).HasConversion(x => x.Value, x => x);
        evt.Property(x => x.Action).HasColumnName("action").IsRequired(true).HasConversion<short>();
        evt.Property(x => x.CertificationInfo).HasColumnName("certification_info").HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.EventId).HasColumnName("event_id").HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.ReadPoint).HasColumnName("read_point").HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.BusinessLocation).HasColumnName("business_location").HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.BusinessStep).HasColumnName("business_step").HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.Disposition).HasColumnName("disposition").HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.TransformationId).HasColumnName("transformation_id").HasMaxLength(256).IsRequired(false);
        evt.Property(x => x.CorrectiveDeclarationTime).HasColumnName("corrective_declaration_time").IsRequired(false);
        evt.Property(x => x.CorrectiveReason).HasColumnName("corrective_reason").HasMaxLength(256).IsRequired(false);
        evt.HasOne(x => x.Request).WithMany(x => x.Events).HasForeignKey("request_id").OnDelete(DeleteBehavior.Cascade);
        evt.OwnsMany(x => x.Epcs, c =>
        {
            c.ToTable("epc", Epcis);
            c.Property<int>("event_id");
            c.HasKey("event_id", "Type", "Id");
            c.Property(x => x.Type).HasColumnName("type").IsRequired(true).HasConversion<short>();
            c.Property(x => x.Id).HasColumnName("id").HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Quantity).HasColumnName("quantity").IsRequired(false);
            c.Property(x => x.UnitOfMeasure).HasColumnName("unit_of_measure").IsRequired(false).HasMaxLength(10);
            c.HasOne(x => x.Event).WithMany(x => x.Epcs).HasForeignKey("event_id").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.Sources, c =>
        {
            c.ToTable("source", Epcis);
            c.Property<int>("event_id");
            c.HasKey("event_id", "Type", "Id");
            c.Property(x => x.Type).HasColumnName("type").IsRequired(true);
            c.Property(x => x.Id).HasColumnName("id").HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.Event).WithMany(x => x.Sources).HasForeignKey("event_id").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.Destinations, c =>
        {
            c.ToTable("destination", Epcis);
            c.Property<int>("event_id");
            c.HasKey("event_id", "Type", "Id");
            c.Property(x => x.Type).HasColumnName("type").IsRequired(true);
            c.Property(x => x.Id).HasColumnName("id").HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.Event).WithMany(x => x.Destinations).HasForeignKey("event_id").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.Transactions, c =>
        {
            c.ToTable("business_transaction", Epcis);
            c.Property<int>("event_id");
            c.HasKey("event_id", "Type", "Id");
            c.Property(x => x.Type).HasColumnName("type").IsRequired(true);
            c.Property(x => x.Id).HasColumnName("id").HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.Event).WithMany(x => x.Transactions).HasForeignKey("event_id").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.PersistentDispositions, c =>
        {
            c.ToTable("persistent_disposition", Epcis);
            c.Property<int>("event_id");
            c.HasKey("event_id", "Type", "Id");
            c.Property(x => x.Type).HasColumnName("type").IsRequired(true);
            c.Property(x => x.Id).HasColumnName("id").HasMaxLength(256).IsRequired(true);
            c.HasOne(x => x.Event).WithMany(x => x.PersistentDispositions).HasForeignKey("event_id").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.SensorElements, c =>
        {
            c.ToTable("sensor_element", Epcis);
            c.Property<int>("event_id");
            c.HasKey("event_id", "Index");
            c.Property(x => x.Index).HasColumnName("index").IsRequired(true).ValueGeneratedNever();
            c.Property(x => x.Time).HasColumnName("time");
            c.Property(x => x.DeviceId).HasColumnName("device_id");
            c.Property(x => x.DeviceMetadata).HasColumnName("device_metadata");
            c.Property(x => x.RawData).HasColumnName("raw_data");
            c.Property(x => x.StartTime).HasColumnName("start_time");
            c.Property(x => x.EndTime).HasColumnName("end_time");
            c.Property(x => x.DataProcessingMethod).HasColumnName("data_processing_method");
            c.Property(x => x.BizRules).HasColumnName("biz_rules");
            c.HasOne(x => x.Event).WithMany(x => x.SensorElements).HasForeignKey("event_id").OnDelete(DeleteBehavior.Cascade);
            c.OwnsMany(x => x.Reports, c =>
            {
                c.ToTable("sensor_report", Epcis);
                c.Property<int>("event_id");
                c.Property<int>("sensor_index").IsRequired(true);
                c.HasKey("event_id", "sensor_index", "Index");
                c.Property(x => x.Index).HasColumnName("index").IsRequired(true).ValueGeneratedNever();
                c.Property(x => x.Type).HasColumnName("type");
                c.Property(x => x.DeviceId).HasColumnName("device_id");
                c.Property(x => x.RawData).HasColumnName("raw_data");
                c.Property(x => x.DataProcessingMethod).HasColumnName("data_processing_method");
                c.Property(x => x.Time).HasColumnName("time");
                c.Property(x => x.Microorganism).HasColumnName("microorganism");
                c.Property(x => x.ChemicalSubstance).HasColumnName("chemical_substance");
                c.Property(x => x.Value).HasColumnName("value");
                c.Property(x => x.Component).HasColumnName("component");
                c.Property(x => x.StringValue).HasColumnName("string_value");
                c.Property(x => x.BooleanValue).HasColumnName("boolean_value");
                c.Property(x => x.HexBinaryValue).HasColumnName("hex_binary_value");
                c.Property(x => x.UriValue).HasColumnName("uri_value");
                c.Property(x => x.MinValue).HasColumnName("min_value");
                c.Property(x => x.MaxValue).HasColumnName("max_value");
                c.Property(x => x.MeanValue).HasColumnName("mean_value");
                c.Property(x => x.PercRank).HasColumnName("perc_rank");
                c.Property(x => x.PercValue).HasColumnName("perc_value");
                c.Property(x => x.UnitOfMeasure).HasColumnName("unit_of_measure");
                c.Property(x => x.SDev).HasColumnName("sdev");
                c.Property(x => x.DeviceMetadata).HasColumnName("device_metadata");
                c.HasOne(x => x.SensorElement).WithMany(x => x.Reports).HasForeignKey("event_id", "sensor_index").OnDelete(DeleteBehavior.Cascade);
            });
        });
        evt.OwnsMany(x => x.Fields, c =>
        {
            c.ToTable("field", Epcis);
            c.Property<int>("event_id");
            c.HasKey("event_id", "Index");
            c.Property(x => x.Index).HasColumnName("index").IsRequired(true).ValueGeneratedNever();
            c.Property(x => x.Type).HasColumnName("type").IsRequired(true).HasConversion<short>();
            c.Property(x => x.Name).HasColumnName("name").HasMaxLength(256).IsRequired(true);
            c.Property(x => x.Namespace).HasColumnName("namespace").HasMaxLength(256).IsRequired(false);
            c.Property(x => x.TextValue).HasColumnName("text_value").IsRequired(false);
            c.Property(x => x.NumericValue).HasColumnName("numeric_value").IsRequired(false);
            c.Property(x => x.DateValue).HasColumnName("date_value").IsRequired(false);
            c.HasOne(x => x.Event).WithMany(x => x.Fields).HasForeignKey("event_id").OnDelete(DeleteBehavior.Cascade);
        });
        evt.OwnsMany(x => x.CorrectiveEventIds, c =>
        {
            c.ToTable("corrective_event_id", Epcis);
            c.Property<int>("event_id");
            c.HasKey("event_id", "CorrectiveId");
            c.Property(x => x.CorrectiveId).HasColumnName("corrective_id").IsRequired(true).HasMaxLength(256);
            c.HasOne(x => x.Event).WithMany(x => x.CorrectiveEventIds).HasForeignKey("event_id").OnDelete(DeleteBehavior.Cascade);
        });

        var subscription = modelBuilder.Entity<Subscription>();
        subscription.ToTable("subscription", Subscriptions, builder => builder.HasTrigger("subscription_initial_requests"));
        subscription.Property(x => x.Name).HasColumnName("name").IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.QueryName).HasColumnName("query_name").IsRequired(true).HasMaxLength(256);
        subscription.Property(x => x.ReportIfEmpty).HasColumnName("report_if_empty").IsRequired(true);
        subscription.Property(x => x.Trigger).HasColumnName("trigger").IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.SignatureToken).HasColumnName("signature_token").IsRequired(false).HasMaxLength(256);
        subscription.Property(x => x.FormatterName).HasColumnName("formatter_name").IsRequired(true).HasMaxLength(30);
        subscription.HasOne(x => x.Query).WithMany().HasForeignKey("query_id").IsRequired(false).OnDelete(DeleteBehavior.Cascade);
        subscription.OwnsOne(x => x.Schedule, c =>
        {
            c.ToTable("subscription_schedule", Subscriptions);
            c.Property<int>("id").IsRequired(true);
            c.HasKey("id");
            c.Property(x => x.Second).HasColumnName("second").HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Minute).HasColumnName("minute").HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Hour).HasColumnName("hour").HasMaxLength(256).IsRequired(false);
            c.Property(x => x.DayOfWeek).HasColumnName("day_of_week").HasMaxLength(256).IsRequired(false);
            c.Property(x => x.DayOfMonth).HasColumnName("day_of_month").HasMaxLength(256).IsRequired(false);
            c.Property(x => x.Month).HasColumnName("month").HasMaxLength(256).IsRequired(false);
            c.HasOne(x => x.Subscription).WithOne(x => x.Schedule).HasForeignKey<SubscriptionSchedule>("subscription_id").OnDelete(DeleteBehavior.Cascade);
        });
        subscription.OwnsMany(x => x.Parameters, c =>
        {
            c.ToTable("subscription_parameter", Subscriptions);
            c.Property<int>("subscription_id");
            c.HasKey("subscription_id", "Name");
            c.HasOne(x => x.Subscription).WithMany(x => x.Parameters).HasForeignKey("SubscriptionId").OnDelete(DeleteBehavior.Cascade);
            c.Property(x => x.Values).HasColumnName("values").IsRequired(false).HasConversion<ArrayConverter, ArrayComparer>();
            c.Property(x => x.Name).HasColumnName("name").IsRequired(true).HasMaxLength(256);
        });
        subscription.OwnsMany(x => x.ExecutionRecords, c =>
        {
            c.ToTable("subscription_execution_record", Subscriptions);
            c.Property<int>("subscription_id");
            c.HasKey("subscription_id", "ExecutionTime");
            c.Property(x => x.ExecutionTime).HasColumnName("execution_time").IsRequired(true);
            c.Property(x => x.ResultsSent).HasColumnName("results_sent").IsRequired(true);
            c.Property(x => x.Successful).HasColumnName("successful").IsRequired(true);
            c.Property(x => x.Reason).HasColumnName("reason").IsRequired(false);
            c.HasOne(x => x.Subscription).WithMany(x => x.ExecutionRecords).HasForeignKey("subscription_id").OnDelete(DeleteBehavior.Cascade);
        });

        var pendingRequests = modelBuilder.Entity<PendingRequest>();
        pendingRequests.ToTable("pending_request", Subscriptions);
        pendingRequests.HasKey("SubscriptionId", "RequestId");
        pendingRequests.Property(x => x.SubscriptionId).HasColumnName("subscription_id").IsRequired(true);
        pendingRequests.Property(x => x.RequestId).HasColumnName("request_id").IsRequired(true);

        var storedQuery = modelBuilder.Entity<StoredQuery>();
        storedQuery.ToTable("stored_query", Queries);
        storedQuery.Property<int>("id").ValueGeneratedOnAdd();
        storedQuery.HasKey("id");
        storedQuery.Property(x => x.Name).HasColumnName("name").IsRequired(true).HasMaxLength(256);
        storedQuery.HasIndex(x => x.Name).IsUnique();
        storedQuery.Property(x => x.DataSource).HasColumnName("data_source").IsRequired(true).HasMaxLength(30);
        storedQuery.Property(x => x.UserId).HasColumnName("user_id").IsRequired(false).HasMaxLength(80);
        storedQuery.HasMany(x => x.Subscriptions).WithOne(x => x.Query);
        storedQuery.OwnsMany(x => x.Parameters, c =>
        {
            c.ToTable("stored_query_parameter", Subscriptions);
            c.Property<int>("query_id");
            c.HasKey("query_id", "Name");
            c.Property(x => x.Name).HasColumnName("name");
            c.Property(x => x.Values).HasColumnName("values").IsRequired(false).HasConversion<ArrayConverter, ArrayComparer>();
        });
        storedQuery.HasData
        (
            new { id = -2, Name = "SimpleEventQuery", DataSource = "SimpleEventQuery" },
            new { id = -1, Name = "SimpleMasterDataQuery", DataSource = "SimpleMasterDataQuery" }
        );
    }
}
