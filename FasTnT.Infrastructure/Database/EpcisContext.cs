using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Infrastructure.Database
{
    public class EpcisContext : DbContext
    {
        private int _generationIndex = 0;
        
        public DbSet<Request> Requests { get; private set; }
        public DbSet<Event> Events { get; private set; }

        public EpcisContext(DbContextOptions<EpcisContext> options) : base(options) 
        { 
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            { 
                var entity = modelBuilder.Entity<Request>();
                entity.ToTable(nameof(Request), nameof(EpcisSchema.Epcis));
                entity.HasKey(x => x.Id);
                entity.Property(x => x.DocumentTime);
                entity.Property(x => x.CaptureDate);
                entity.Property(x => x.SchemaVersion).IsRequired(true);
                entity.HasMany(x => x.Events).WithOne(x => x.Request).HasForeignKey("RequestId");
                entity.HasMany(x => x.Masterdata).WithOne(x => x.Request).HasForeignKey("RequestId");
                entity.HasOne(x => x.StandardBusinessHeader).WithOne(x => x.Request).HasForeignKey<StandardBusinessHeader>("RequestId").IsRequired(false);
            }
            {
                var entity = modelBuilder.Entity<StandardBusinessHeader>();
                entity.ToTable(nameof(StandardBusinessHeader), nameof(EpcisSchema.Epcis));
                entity.HasKey("RequestId");
                entity.Property<int>("RequestId");
                entity.Property(x => x.Version).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Standard).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.TypeVersion).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.InstanceIdentifier).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.CreationDateTime).IsRequired(false);
                entity.HasOne(x => x.Request).WithOne(x => x.StandardBusinessHeader).HasForeignKey<StandardBusinessHeader>("RequestId").IsRequired(true);
                entity.HasMany(x => x.ContactInformations).WithOne(x => x.Header).HasForeignKey("RequestId");
            }
            {
                var entity = modelBuilder.Entity<ContactInformation>();
                entity.ToTable(nameof(ContactInformation), nameof(EpcisSchema.Epcis));
                entity.HasKey("RequestId", nameof(ContactInformation.Type), nameof(ContactInformation.Identifier));
                entity.Property<int>("RequestId");
                entity.Property(x => x.Type).HasMaxLength(256).HasConversion(x => x.Id, x => Enumeration.GetById<ContactInformationType>(x)).IsRequired(true);
                entity.Property(x => x.Identifier).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Contact).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.EmailAddress).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.FaxNumber).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.TelephoneNumber).IsRequired(false);
                entity.Property(x => x.ContactTypeIdentifier).IsRequired(false);
                entity.HasOne(x => x.Header).WithMany(x => x.ContactInformations).HasForeignKey("RequestId");
            }
            {
                var entity = modelBuilder.Entity<MasterData>();
                entity.ToTable(nameof(MasterData), nameof(EpcisSchema.Epcis));
                entity.HasKey("RequestId", nameof(MasterData.Type), nameof(MasterData.Id));
                entity.Property<int>("RequestId");
                entity.Property(x => x.Type).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
                entity.HasMany(x => x.Attributes).WithOne(x => x.MasterData).HasForeignKey("RequestId", "MasterdataType", "MasterdataId");
                entity.Ignore(x => x.Children); // TODO: map
            }
            {
                var entity = modelBuilder.Entity<MasterDataAttribute>();
                entity.ToTable(nameof(MasterDataAttribute), nameof(EpcisSchema.Epcis));
                entity.HasKey("RequestId", "MasterdataType", "MasterdataId", nameof(MasterDataAttribute.Id));
                entity.Property<int>("RequestId");
                entity.Property<string>("MasterdataType").HasMaxLength(256);
                entity.Property<string>("MasterdataId").HasMaxLength(256);
                entity.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Value).HasMaxLength(256).IsRequired(true);
                entity.HasOne(x => x.MasterData).WithMany(x => x.Attributes).HasForeignKey("RequestId", "MasterdataType", "MasterdataId");
                entity.HasMany(x => x.Fields).WithOne(x => x.Attribute).HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId");
            }
            {
                var entity = modelBuilder.Entity<MasterDataField>();
                entity.ToTable(nameof(MasterDataField), nameof(EpcisSchema.Epcis));
                entity.HasKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId", nameof(MasterDataField.Namespace), nameof(MasterDataField.Name));
                entity.Property<int>("RequestId");
                entity.Property<string>("MasterdataType").HasMaxLength(256);
                entity.Property<string>("MasterdataId").HasMaxLength(256);
                entity.Property<string>("AttributeId").HasMaxLength(256);
                entity.Property(x => x.Namespace).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Value).HasMaxLength(256).IsRequired(true);
                entity.HasOne(x => x.Attribute).WithMany(x => x.Fields).HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId");
                entity.Ignore(x => x.Children); // TODO: map
            }
            {
                var entity = modelBuilder.Entity<Event>();
                entity.ToTable(nameof(Event), nameof(EpcisSchema.Epcis));
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).IsRequired(true).UseIdentityColumn(1, 1);
                entity.Property(x => x.EventTime).IsRequired(true);
                entity.Property(x => x.Type).IsRequired(true).HasConversion(x => x.Id, x => Enumeration.GetById<EventType>(x));
                entity.Property(x => x.EventTimeZoneOffset).IsRequired(true).HasConversion(x => x.Value, x => new TimeZoneOffset { Value = x });
                entity.Property(x => x.Action).IsRequired(true).HasConversion(x => x.Id, x => Enumeration.GetById<EventAction>(x));
                entity.Property(x => x.EventId).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.ReadPoint).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.BusinessLocation).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.BusinessStep).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.Disposition).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.TransformationId).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.CorrectiveDeclarationTime).IsRequired(false);
                entity.Property(x => x.CorrectiveReason).HasMaxLength(256).IsRequired(false);
                entity.HasMany(x => x.Epcs).WithOne(x => x.Event).HasForeignKey("EventId");
                entity.HasMany(x => x.Transactions).WithOne(x => x.Event).HasForeignKey("EventId");
                entity.HasMany(x => x.SourceDests).WithOne(x => x.Event).HasForeignKey("EventId");
                entity.HasMany(x => x.CustomFields).WithOne(x => x.Event).HasForeignKey("EventId");
                entity.HasMany(x => x.CorrectiveEventIds).WithOne(x => x.Event).HasForeignKey("EventId");
                entity.HasOne(x => x.Request).WithMany(x => x.Events).HasForeignKey("RequestId");
            }
            {
                var entity = modelBuilder.Entity<Epc>();
                entity.ToTable(nameof(Epc), nameof(EpcisSchema.Epcis));
                entity.HasKey("EventId", nameof(Epc.Type), nameof(Epc.Id));
                entity.Property<long>("EventId");
                entity.Property(x => x.Type).IsRequired(true).HasConversion(x => x.Id, x => Enumeration.GetById<EpcType>(x));
                entity.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.IsQuantity).IsRequired(true).HasDefaultValue(false);
                entity.Property(x => x.Quantity).IsRequired(false);
                entity.Property(x => x.UnitOfMeasure).IsRequired(false).HasMaxLength(10);
                entity.HasOne(x => x.Event).WithMany(x => x.Epcs).HasForeignKey("EventId");
            }
            {
                var entity = modelBuilder.Entity<CorrectiveEventId>();
                entity.ToTable(nameof(CorrectiveEventId), nameof(EpcisSchema.Epcis));
                entity.HasKey("EventId", nameof(CorrectiveEventId.CorrectiveId));
                entity.Property<long>("EventId");
                entity.Property(x => x.CorrectiveId).IsRequired(true).HasMaxLength(256);
            }
            {
                var entity = modelBuilder.Entity<SourceDestination>();
                entity.ToTable(nameof(SourceDestination), nameof(EpcisSchema.Epcis));
                entity.HasKey("EventId", nameof(SourceDestination.Type), nameof(SourceDestination.Id));
                entity.Property<long>("EventId");
                entity.Property(x => x.Type).IsRequired(true);
                entity.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Direction).IsRequired(true).HasConversion(x => x.Id, x => Enumeration.GetById<SourceDestinationType>(x));
                entity.HasOne(x => x.Event).WithMany(x => x.SourceDests).HasForeignKey("EventId");
            }
            {
                var entity = modelBuilder.Entity<BusinessTransaction>();
                entity.ToTable(nameof(BusinessTransaction), nameof(EpcisSchema.Epcis));
                entity.HasKey("EventId", nameof(BusinessTransaction.Type), nameof(BusinessTransaction.Id));
                entity.Property<long>("EventId");
                entity.Property(x => x.Type).HasConversion<short>().IsRequired(true);
                entity.Property(x => x.Id).HasMaxLength(256).IsRequired(true);
                entity.HasOne(x => x.Event).WithMany(x => x.Transactions).HasForeignKey("EventId");
            }
            {
                var entity = modelBuilder.Entity<CustomField>();
                entity.ToTable(nameof(CustomField), nameof(EpcisSchema.Epcis));
                entity.Property<int>("FieldId").IsRequired(true).HasValueGenerator<CustomFieldValueGenerator>();
                entity.Property<long>("EventId");
                entity.Property<int?>("ParentId").IsRequired(false);
                entity.HasKey("EventId", "FieldId");
                entity.Property(x => x.Type).IsRequired(true).HasConversion(x => x.Id, x => Enumeration.GetById<FieldType>(x)); ;
                entity.Property(x => x.Name).HasMaxLength(256).IsRequired(true);
                entity.Property(x => x.Namespace).HasMaxLength(256).IsRequired(false);
                entity.Property(x => x.TextValue).IsRequired(false);
                entity.Property(x => x.NumericValue).IsRequired(false);
                entity.Property(x => x.DateValue).IsRequired(false);
                entity.HasOne(x => x.Event).WithMany(x => x.CustomFields).HasForeignKey("EventId");
                entity.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey("EventId", "ParentId").OnDelete(DeleteBehavior.NoAction).IsRequired(false);
            }
        }

        internal int NextInteger() { return _generationIndex += 1; }

        internal class CustomFieldValueGenerator : ValueGenerator<int>
        {
            public override bool GeneratesTemporaryValues { get; }

            public override int Next([NotNull] EntityEntry entry)
            {
                if (entry.Context is not EpcisContext context)
                    throw new ArgumentNullException($"{nameof(CustomFieldValueGenerator)} should be used only from {nameof(EpcisContext)} DB Context");

                return context.NextInteger();
            }
        }
    }
}
