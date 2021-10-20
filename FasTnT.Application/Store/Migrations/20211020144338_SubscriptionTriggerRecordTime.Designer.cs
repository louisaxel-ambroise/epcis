﻿// <auto-generated />
using System;
using FasTnT.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FasTnT.Application.Migrations
{
    [DbContext(typeof(EpcisContext))]
    [Migration("20211020144338_SubscriptionTriggerRecordTime")]
    partial class SubscriptionTriggerRecordTime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FasTnT.Domain.Model.BusinessTransaction", b =>
                {
                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Id")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("EventId", "Type", "Id");

                    b.ToTable("BusinessTransaction", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.ContactInformation", b =>
                {
                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<short>("Type")
                        .HasMaxLength(256)
                        .HasColumnType("smallint");

                    b.Property<string>("Identifier")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Contact")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("ContactTypeIdentifier")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailAddress")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("FaxNumber")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("TelephoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RequestId", "Type", "Identifier");

                    b.ToTable("ContactInformation", "Sbdh");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.CorrectiveEventId", b =>
                {
                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<string>("CorrectiveId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("EventId", "CorrectiveId");

                    b.ToTable("CorrectiveEventId", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.CustomField", b =>
                {
                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<int>("FieldId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateValue")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Namespace")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<double?>("NumericValue")
                        .HasColumnType("float");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("TextValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.HasKey("EventId", "FieldId");

                    b.HasIndex("EventId", "ParentId");

                    b.ToTable("CustomField", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Destination", b =>
                {
                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Id")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("EventId", "Type", "Id");

                    b.ToTable("Destination", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Epc", b =>
                {
                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.Property<string>("Id")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("IsQuantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<float?>("Quantity")
                        .HasColumnType("real");

                    b.Property<string>("UnitOfMeasure")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("EventId", "Type", "Id");

                    b.ToTable("Epc", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Event", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:IdentityIncrement", 1)
                        .HasAnnotation("SqlServer:IdentitySeed", 1)
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Action")
                        .HasColumnType("smallint");

                    b.Property<string>("BusinessLocation")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("BusinessStep")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("CaptureTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CorrectiveDeclarationTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("CorrectiveReason")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Disposition")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("EventId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<DateTime>("EventTime")
                        .HasColumnType("datetime2");

                    b.Property<short>("EventTimeZoneOffset")
                        .HasColumnType("smallint");

                    b.Property<string>("ReadPoint")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<int?>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("TransformationId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<short>("Type")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("Event", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterData", b =>
                {
                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Id")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("RequestId", "Type", "Id");

                    b.ToTable("MasterData", "Cbv");

                    b
                        .HasAnnotation("Relational:SqlQuery", "SELECT MAX(RequestId) AS RequestId, type, id FROM [Cbv].[MasterData] GROUP BY type, id");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataAttribute", b =>
                {
                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("MasterdataType")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("MasterdataId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Id")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("RequestId", "MasterdataType", "MasterdataId", "Id");

                    b.ToTable("MasterDataAttribute", "Cbv");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataChildren", b =>
                {
                    b.Property<int?>("MasterDataRequestId")
                        .HasColumnType("int");

                    b.Property<string>("MasterDataType")
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("MasterDataId")
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("ChildrenId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("MasterDataRequestId", "MasterDataType", "MasterDataId", "ChildrenId");

                    b.ToTable("MasterDataChildren", "Cbv");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataField", b =>
                {
                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("MasterdataType")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("MasterdataId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("AttributeId")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Namespace")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("ParentName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("ParentNamespace")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Value")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId", "Namespace", "Name");

                    b.ToTable("MasterDataField", "Cbv");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataHierarchy", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("ParentId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RequestId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("RequestId", "Type", "Id");

                    b.ToView("MasterDataHierarchy", "Cbv");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.PendingRequest", b =>
                {
                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<int>("SubscriptionId")
                        .HasColumnType("int");

                    b.HasKey("RequestId", "SubscriptionId");

                    b.ToTable("PendingRequest", "Subscription");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CaptureDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DocumentTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("SchemaVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Request", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Source", b =>
                {
                    b.Property<long>("EventId")
                        .HasColumnType("bigint");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Id")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("EventId", "Type", "Id");

                    b.ToTable("Source", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.StandardBusinessHeader", b =>
                {
                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreationDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("InstanceIdentifier")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Standard")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("TypeVersion")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("RequestId");

                    b.ToTable("StandardBusinessHeader", "Sbdh");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Subscription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Destination")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("InitialRecordTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("QueryName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("RecordIfEmpty")
                        .HasColumnType("bit");

                    b.Property<int?>("ScheduleId")
                        .HasColumnType("int");

                    b.Property<string>("Trigger")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId")
                        .IsUnique()
                        .HasFilter("[ScheduleId] IS NOT NULL");

                    b.ToTable("Subscription", "Subscription");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.SubscriptionCallback", b =>
                {
                    b.Property<int>("RequestId")
                        .HasColumnType("int");

                    b.Property<short>("CallbackType")
                        .HasColumnType("smallint");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubscriptionId")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("RequestId");

                    b.ToTable("SubscriptionCallback", "Epcis");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.SubscriptionExecutionRecord", b =>
                {
                    b.Property<int>("SubscriptionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ExecutionTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ResultsSent")
                        .HasColumnType("bit");

                    b.Property<bool>("Successful")
                        .HasColumnType("bit");

                    b.HasKey("SubscriptionId", "ExecutionTime");

                    b.ToTable("SubscriptionExecutionRecord", "Subscription");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.SubscriptionParameter", b =>
                {
                    b.Property<int>("SubscriptionId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SubscriptionId", "Name");

                    b.ToTable("SubscriptionParameter", "Subscription");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.SubscriptionSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DayOfMonth")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("DayOfWeek")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Hour")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Minute")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Month")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Second")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.ToTable("SubscriptionSchedule", "Subscription");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("CanCapture")
                        .HasColumnType("bit");

                    b.Property<bool>("CanQuery")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RegisteredOn")
                        .HasColumnType("datetime2");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varbinary(20)");

                    b.Property<string>("SecuredKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User", "Users");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.UserDefaultQueryParameter", b =>
                {
                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Values")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "Name");

                    b.ToTable("UserDefaultQueryParameter", "Users");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.BusinessTransaction", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Event", "Event")
                        .WithMany("Transactions")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.ContactInformation", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.StandardBusinessHeader", "Header")
                        .WithMany("ContactInformations")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Header");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.CorrectiveEventId", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Event", "Event")
                        .WithMany("CorrectiveEventIds")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.CustomField", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Event", "Event")
                        .WithMany("CustomFields")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FasTnT.Domain.Model.CustomField", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("EventId", "ParentId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Event");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Destination", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Event", "Event")
                        .WithMany("Destinations")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Epc", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Event", "Event")
                        .WithMany("Epcs")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Event", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Request", "Request")
                        .WithMany("Events")
                        .HasForeignKey("RequestId");

                    b.Navigation("Request");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterData", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Request", "Request")
                        .WithMany("Masterdata")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataAttribute", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.MasterData", "MasterData")
                        .WithMany("Attributes")
                        .HasForeignKey("RequestId", "MasterdataType", "MasterdataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MasterData");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataChildren", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.MasterData", "MasterData")
                        .WithMany("Children")
                        .HasForeignKey("MasterDataRequestId", "MasterDataType", "MasterDataId");

                    b.Navigation("MasterData");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataField", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.MasterDataAttribute", "Attribute")
                        .WithMany("Fields")
                        .HasForeignKey("RequestId", "MasterdataType", "MasterdataId", "AttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attribute");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataHierarchy", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.MasterData", "MasterData")
                        .WithMany("Hierarchies")
                        .HasForeignKey("RequestId", "Type", "Id");

                    b.Navigation("MasterData");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Request", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Source", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Event", "Event")
                        .WithMany("Sources")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.StandardBusinessHeader", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Request", "Request")
                        .WithOne("StandardBusinessHeader")
                        .HasForeignKey("FasTnT.Domain.Model.StandardBusinessHeader", "RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Subscription", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.SubscriptionSchedule", "Schedule")
                        .WithOne("Subscription")
                        .HasForeignKey("FasTnT.Domain.Model.Subscription", "ScheduleId");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.SubscriptionCallback", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Request", "Request")
                        .WithOne("SubscriptionCallback")
                        .HasForeignKey("FasTnT.Domain.Model.SubscriptionCallback", "RequestId");

                    b.Navigation("Request");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.SubscriptionExecutionRecord", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Subscription", "Subscription")
                        .WithMany("ExecutionRecords")
                        .HasForeignKey("SubscriptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subscription");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.SubscriptionParameter", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.Subscription", "Subscription")
                        .WithMany("Parameters")
                        .HasForeignKey("SubscriptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subscription");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.UserDefaultQueryParameter", b =>
                {
                    b.HasOne("FasTnT.Domain.Model.User", "User")
                        .WithMany("DefaultQueryParameters")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.CustomField", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Event", b =>
                {
                    b.Navigation("CorrectiveEventIds");

                    b.Navigation("CustomFields");

                    b.Navigation("Destinations");

                    b.Navigation("Epcs");

                    b.Navigation("Sources");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterData", b =>
                {
                    b.Navigation("Attributes");

                    b.Navigation("Children");

                    b.Navigation("Hierarchies");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.MasterDataAttribute", b =>
                {
                    b.Navigation("Fields");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Request", b =>
                {
                    b.Navigation("Events");

                    b.Navigation("Masterdata");

                    b.Navigation("StandardBusinessHeader");

                    b.Navigation("SubscriptionCallback");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.StandardBusinessHeader", b =>
                {
                    b.Navigation("ContactInformations");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.Subscription", b =>
                {
                    b.Navigation("ExecutionRecords");

                    b.Navigation("Parameters");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.SubscriptionSchedule", b =>
                {
                    b.Navigation("Subscription");
                });

            modelBuilder.Entity("FasTnT.Domain.Model.User", b =>
                {
                    b.Navigation("DefaultQueryParameters");
                });
#pragma warning restore 612, 618
        }
    }
}
