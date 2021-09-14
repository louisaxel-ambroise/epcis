using FasTnT.Domain.Model;
using FasTnT.Domain.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace FasTnT.Infrastructure.Configuration
{
    internal static class EpcisModelConfiguration
    {
        internal static void Apply(ModelBuilder modelBuilder)
        {
            var user = modelBuilder.Entity<User>();
            user.ToTable(nameof(User), nameof(EpcisSchema.Users));
            user.HasKey(x => x.Id);
            user.Property(x => x.Username).IsRequired(true);
            user.Property(x => x.Salt).IsRequired(true).HasMaxLength(20);
            user.Property(x => x.SecuredKey).IsRequired(true);
            user.Property(x => x.RegisteredOn).IsRequired(true);
            user.HasMany(x => x.DefaultQueryParameters).WithOne(x => x.User);

            var userParameter = modelBuilder.Entity<UserDefaultQueryParameter>();
            userParameter.ToTable(nameof(UserDefaultQueryParameter), nameof(EpcisSchema.Users));
            userParameter.HasKey("UserId", "Name");
            userParameter.Property(x => x.Name).IsRequired(true);
            userParameter.Property(x => x.Values).IsRequired(false).HasConversion(
                x => JsonConvert.SerializeObject(x),
                x => JsonConvert.DeserializeObject<string[]>(x),
                new ValueComparer<string[]>((c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => c.ToArray()));
            userParameter.HasOne(x => x.User).WithMany(x => x.DefaultQueryParameters);

            var request = modelBuilder.Entity<Request>();
            request.ToTable(nameof(Request), nameof(EpcisSchema.Epcis));
            request.HasKey(x => x.Id);
            request.Property(x => x.DocumentTime);
            request.Property(x => x.CaptureDate);
            request.Property(x => x.SchemaVersion).IsRequired(true);
            request.HasMany(x => x.Events).WithOne(x => x.Request).HasForeignKey("RequestId");
            request.HasMany(x => x.Masterdata).WithOne(x => x.Request).HasForeignKey("RequestId");
            request.HasOne(x => x.StandardBusinessHeader).WithOne(x => x.Request).HasForeignKey<StandardBusinessHeader>("RequestId").IsRequired(false);
            request.HasOne(x => x.SubscriptionCallback).WithOne(x => x.Request).HasForeignKey<SubscriptionCallback>("RequestId").IsRequired(false);
            request.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);

            var header = modelBuilder.Entity<StandardBusinessHeader>();
            header.ToTable(nameof(StandardBusinessHeader), nameof(EpcisSchema.Sbdh));
            header.HasKey("RequestId");
            header.Property<int>("RequestId");
            header.Property(x => x.Version).HasMaxLength(256).IsRequired(true);
            header.Property(x => x.Standard).HasMaxLength(256).IsRequired(true);
            header.Property(x => x.TypeVersion).HasMaxLength(256).IsRequired(true);
            header.Property(x => x.InstanceIdentifier).HasMaxLength(256).IsRequired(true);
            header.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
            header.Property(x => x.CreationDateTime).IsRequired(false);
            header.HasOne(x => x.Request).WithOne(x => x.StandardBusinessHeader).HasForeignKey<StandardBusinessHeader>("RequestId").IsRequired(true);
            header.HasMany(x => x.ContactInformations).WithOne(x => x.Header).HasForeignKey("RequestId");

            var contactInfo = modelBuilder.Entity<ContactInformation>();
            contactInfo.ToTable(nameof(ContactInformation), nameof(EpcisSchema.Sbdh));
            contactInfo.HasKey("RequestId", nameof(ContactInformation.Type), nameof(ContactInformation.Identifier));
            contactInfo.Property<int>("RequestId");
            contactInfo.Property(x => x.Type).HasMaxLength(256).HasConversion<short>().IsRequired(true);
            contactInfo.Property(x => x.Identifier).HasMaxLength(256).IsRequired(true);
            contactInfo.Property(x => x.Contact).HasMaxLength(256).IsRequired(false);
            contactInfo.Property(x => x.EmailAddress).HasMaxLength(256).IsRequired(false);
            contactInfo.Property(x => x.FaxNumber).HasMaxLength(256).IsRequired(false);
            contactInfo.Property(x => x.TelephoneNumber).IsRequired(false);
            contactInfo.Property(x => x.ContactTypeIdentifier).IsRequired(false);
            contactInfo.HasOne(x => x.Header).WithMany(x => x.ContactInformations).HasForeignKey("RequestId");

            var masterData = modelBuilder.Entity<MasterData>();
            masterData.ToTable(nameof(MasterData), nameof(EpcisSchema.Cbv));
            masterData.HasKey("RequestId", nameof(MasterData.Type), nameof(MasterData.Id));
            masterData.Property<int>("RequestId");
            masterData.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
            masterData.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            masterData.HasMany(x => x.Attributes).WithOne(x => x.MasterData).HasForeignKey("RequestId", "MasterdataType", "MasterdataId");
            masterData.HasMany(x => x.Children).WithOne(x => x.MasterData).IsRequired(false);
            masterData.HasMany(x => x.Hierarchies).WithOne(x => x.MasterData).IsRequired(false);
            masterData.ToSqlQuery("SELECT MAX(RequestId) AS RequestId, type, id FROM [Cbv].[MasterData] GROUP BY type, id");

            var mdHierarchy = modelBuilder.Entity<MasterDataHierarchy>();
            mdHierarchy.ToView(nameof(MasterDataHierarchy), nameof(EpcisSchema.Cbv));
            mdHierarchy.HasOne(x => x.MasterData).WithMany(x => x.Hierarchies).HasForeignKey("RequestId", "Type", "Id");
            mdHierarchy.Property(x => x.ParentId);

            var mdChildren = modelBuilder.Entity<MasterDataChildren>();
            mdChildren.ToTable(nameof(MasterDataChildren), nameof(EpcisSchema.Cbv));
            mdChildren.HasKey("MasterDataRequestId", "MasterDataType", "MasterDataId", "ChildrenId");
            mdChildren.HasOne(x => x.MasterData).WithMany(x => x.Children);
            mdChildren.Property(x => x.ChildrenId).HasMaxLength(256);

            var mdAttribute = modelBuilder.Entity<MasterDataAttribute>();
            mdAttribute.ToTable(nameof(MasterDataAttribute), nameof(EpcisSchema.Cbv));
            mdAttribute.HasKey("RequestId", "MasterdataType", "MasterdataId", nameof(MasterDataAttribute.Id));
            mdAttribute.Property<int>("RequestId");
            mdAttribute.Property<string>("MasterdataType").HasMaxLength(256);
            mdAttribute.Property<string>("MasterdataId").HasMaxLength(256);
            mdAttribute.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            mdAttribute.Property(x => x.Value).HasMaxLength(256).IsRequired(true);
            mdAttribute.HasOne(x => x.MasterData).WithMany(x => x.Attributes).HasForeignKey("RequestId", "MasterdataType", "MasterdataId");
            mdAttribute.HasMany(x => x.Fields).WithOne(x => x.Attribute).HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId");

            var mdField = modelBuilder.Entity<MasterDataField>();
            mdField.ToTable(nameof(MasterDataField), nameof(EpcisSchema.Cbv));
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
            mdField.HasOne(x => x.Attribute).WithMany(x => x.Fields).HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId");

            var callback = modelBuilder.Entity<SubscriptionCallback>();
            callback.ToTable(nameof(SubscriptionCallback), nameof(EpcisSchema.Epcis));
            callback.Property<int>("RequestId");
            callback.HasKey("RequestId");
            callback.HasOne(x => x.Request).WithOne(x => x.SubscriptionCallback).HasForeignKey<SubscriptionCallback>("RequestId");
            callback.Property(x => x.CallbackType).IsRequired(true).HasConversion<short>();
            callback.Property(x => x.Reason).IsRequired(false);
            callback.Property(x => x.SubscriptionId).IsRequired(true).HasMaxLength(256);

            var evt = modelBuilder.Entity<Event>();
            evt.ToTable(nameof(Event), nameof(EpcisSchema.Epcis));
            evt.HasKey(x => x.Id);
            evt.Property(x => x.Id).IsRequired(true).UseIdentityColumn(1, 1);
            evt.Property(x => x.EventTime).IsRequired(true);
            evt.Property(x => x.Type).IsRequired(true).HasConversion<short>();
            evt.Property(x => x.EventTimeZoneOffset).IsRequired(true).HasConversion(x => x.Value, x => new TimeZoneOffset { Value = x });
            evt.Property(x => x.Action).IsRequired(true).HasConversion<short>();
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
            evt.HasMany(x => x.CustomFields).WithOne(x => x.Event).HasForeignKey("EventId");
            evt.HasMany(x => x.CorrectiveEventIds).WithOne(x => x.Event).HasForeignKey("EventId");
            evt.HasOne(x => x.Request).WithMany(x => x.Events).HasForeignKey("RequestId");

            var epc = modelBuilder.Entity<Epc>();
            epc.ToTable(nameof(Epc), nameof(EpcisSchema.Epcis));
            epc.HasKey("EventId", nameof(Epc.Type), nameof(Epc.Id));
            epc.Property<long>("EventId");
            epc.Property(x => x.Type).IsRequired(true).HasConversion<short>();
            epc.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            epc.Property(x => x.IsQuantity).IsRequired(true).HasDefaultValue(false);
            epc.Property(x => x.Quantity).IsRequired(false);
            epc.Property(x => x.UnitOfMeasure).IsRequired(false).HasMaxLength(10);
            epc.HasOne(x => x.Event).WithMany(x => x.Epcs).HasForeignKey("EventId");

            var eventId = modelBuilder.Entity<CorrectiveEventId>();
            eventId.ToTable(nameof(CorrectiveEventId), nameof(EpcisSchema.Epcis));
            eventId.HasKey("EventId", nameof(CorrectiveEventId.CorrectiveId));
            eventId.Property<long>("EventId");
            eventId.Property(x => x.CorrectiveId).IsRequired(true).HasMaxLength(256);

            var source = modelBuilder.Entity<Source>();
            source.ToTable(nameof(Source), nameof(EpcisSchema.Epcis));
            source.Property<long>("EventId");
            source.HasKey("EventId", nameof(Source.Type), nameof(Source.Id));
            source.Property(x => x.Type).IsRequired(true);
            source.Property(x => x.Id).HasMaxLength(256).IsRequired(true);

            var dest = modelBuilder.Entity<Destination>();
            dest.ToTable(nameof(Destination), nameof(EpcisSchema.Epcis));
            dest.Property<long>("EventId");
            dest.HasKey("EventId", nameof(Destination.Type), nameof(Destination.Id));
            dest.Property(x => x.Type).IsRequired(true);
            dest.Property(x => x.Id).HasMaxLength(256).IsRequired(true);

            var bizTrans = modelBuilder.Entity<BusinessTransaction>();
            bizTrans.ToTable(nameof(BusinessTransaction), nameof(EpcisSchema.Epcis));
            bizTrans.HasKey("EventId", nameof(BusinessTransaction.Type), nameof(BusinessTransaction.Id));
            bizTrans.Property<long>("EventId");
            bizTrans.Property(x => x.Type).IsRequired(true);
            bizTrans.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
            bizTrans.HasOne(x => x.Event).WithMany(x => x.Transactions).HasForeignKey("EventId");

            var customField = modelBuilder.Entity<CustomField>();
            customField.ToTable(nameof(CustomField), nameof(EpcisSchema.Epcis));
            customField.Property<int>("FieldId").IsRequired(true).HasValueGenerator<IncrementGenerator>();
            customField.Property<long>("EventId");
            customField.Property<int?>("ParentId").IsRequired(false);
            customField.HasKey("EventId", "FieldId");
            customField.Property(x => x.Type).IsRequired(true).HasConversion<short>();
            customField.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
            customField.Property(x => x.Namespace).HasMaxLength(256).IsRequired(false);
            customField.Property(x => x.TextValue).IsRequired(false);
            customField.Property(x => x.NumericValue).IsRequired(false);
            customField.Property(x => x.DateValue).IsRequired(false);
            customField.HasOne(x => x.Event).WithMany(x => x.CustomFields).HasForeignKey("EventId");
            customField.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey("EventId", "ParentId").OnDelete(DeleteBehavior.NoAction).IsRequired(false);

            var subscription = modelBuilder.Entity<Subscription>();
            subscription.ToTable(nameof(Subscription), nameof(EpcisSchema.Subscription));
            subscription.Property(x => x.Name).IsRequired(true).HasMaxLength(256);
            subscription.Property(x => x.QueryName).IsRequired(true).HasMaxLength(256);
            subscription.Property(x => x.RecordIfEmpty).IsRequired(true);
            subscription.Property(x => x.Trigger).IsRequired(false).HasMaxLength(256);
            subscription.HasMany(x => x.Parameters).WithOne(x => x.Subscription).HasForeignKey("SubscriptionId");
            subscription.HasOne(x => x.Schedule).WithOne(x => x.Subscription).HasForeignKey<Subscription>("ScheduleId").IsRequired(false);

            var subscriptionParam = modelBuilder.Entity<SubscriptionParameter>();
            subscriptionParam.ToTable(nameof(SubscriptionParameter), nameof(EpcisSchema.Subscription));
            subscriptionParam.HasKey("SubscriptionId", nameof(SubscriptionParameter.Name));
            subscriptionParam.HasOne(x => x.Subscription).WithMany(x => x.Parameters).HasForeignKey("SubscriptionId");
            subscriptionParam.Property(x => x.Value).IsRequired(false).HasConversion(
                x => JsonConvert.SerializeObject(x), 
                x => JsonConvert.DeserializeObject<string[]>(x), 
                new ValueComparer<string[]>((c1, c2) => c1.SequenceEqual(c2), c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), c => c.ToArray()));
        }
    }
}
