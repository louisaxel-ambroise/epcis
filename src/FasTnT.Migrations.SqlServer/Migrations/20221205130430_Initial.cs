﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FasTnT.Migrations.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Epcis");

            migrationBuilder.EnsureSchema(
                name: "Sbdh");

            migrationBuilder.EnsureSchema(
                name: "Cbv");

            migrationBuilder.EnsureSchema(
                name: "Subscription");

            migrationBuilder.EnsureSchema(
                name: "Queries");

            migrationBuilder.CreateTable(
                name: "PendingRequest",
                schema: "Subscription",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRequest", x => new { x.RequestId, x.SubscriptionId });
                });

            migrationBuilder.CreateTable(
                name: "Request",
                schema: "Epcis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaptureId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CaptureDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DocumentTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SchemaVersion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredQuery",
                schema: "Queries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DataSource = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredQuery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "Epcis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    CaptureTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventTimeZoneOffset = table.Column<short>(type: "smallint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Action = table.Column<short>(type: "smallint", nullable: false),
                    EventId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CertificationInfo = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReadPoint = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BusinessLocation = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BusinessStep = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Disposition = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TransformationId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CorrectiveDeclarationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CorrectiveReason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Request_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "Epcis",
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterData",
                schema: "Cbv",
                columns: table => new
                {
                    Type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterData", x => new { x.RequestId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_MasterData_Request_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "Epcis",
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandardBusinessHeader",
                schema: "Sbdh",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Standard = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TypeVersion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    InstanceIdentifier = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreationDateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardBusinessHeader", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_StandardBusinessHeader_Request_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "Epcis",
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionCallback",
                schema: "Epcis",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CallbackType = table.Column<short>(type: "smallint", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionCallback", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_SubscriptionCallback_Request_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "Epcis",
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredQueryParameter",
                schema: "Subscription",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredQueryParameter", x => new { x.QueryId, x.Name });
                    table.ForeignKey(
                        name: "FK_StoredQueryParameter_StoredQuery_QueryId",
                        column: x => x.QueryId,
                        principalSchema: "Queries",
                        principalTable: "StoredQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                schema: "Subscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    QueryName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SignatureToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FormatterName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Trigger = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReportIfEmpty = table.Column<bool>(type: "bit", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QueryId = table.Column<int>(type: "int", nullable: true),
                    InitialRecordTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscription_StoredQuery_QueryId",
                        column: x => x.QueryId,
                        principalSchema: "Queries",
                        principalTable: "StoredQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessTransaction",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessTransaction", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_BusinessTransaction_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorrectiveEventId",
                schema: "Epcis",
                columns: table => new
                {
                    CorrectiveId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectiveEventId", x => new { x.EventId, x.CorrectiveId });
                    table.ForeignKey(
                        name: "FK_CorrectiveEventId_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Destination",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_Destination_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Epc",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Epc", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_Epc_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersistentDisposition",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistentDisposition", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_PersistentDisposition_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorElement",
                schema: "Epcis",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    SensorId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceMetadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DataProcessingMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BizRules = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorElement", x => new { x.EventId, x.SensorId });
                    table.ForeignKey(
                        name: "FK_SensorElement_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_Source_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataAttribute",
                schema: "Cbv",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    MasterdataType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MasterdataId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataAttribute", x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.Id });
                    table.ForeignKey(
                        name: "FK_MasterDataAttribute_MasterData_RequestId_MasterdataType_MasterdataId",
                        columns: x => new { x.RequestId, x.MasterdataType, x.MasterdataId },
                        principalSchema: "Cbv",
                        principalTable: "MasterData",
                        principalColumns: new[] { "RequestId", "Type", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataChildren",
                schema: "Cbv",
                columns: table => new
                {
                    ChildrenId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MasterDataRequestId = table.Column<int>(type: "int", nullable: false),
                    MasterDataType = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    MasterDataId = table.Column<string>(type: "nvarchar(256)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataChildren", x => new { x.MasterDataRequestId, x.MasterDataType, x.MasterDataId, x.ChildrenId });
                    table.ForeignKey(
                        name: "FK_MasterDataChildren_MasterData_MasterDataRequestId_MasterDataType_MasterDataId",
                        columns: x => new { x.MasterDataRequestId, x.MasterDataType, x.MasterDataId },
                        principalSchema: "Cbv",
                        principalTable: "MasterData",
                        principalColumns: new[] { "RequestId", "Type", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactInformation",
                schema: "Sbdh",
                columns: table => new
                {
                    Type = table.Column<short>(type: "smallint", maxLength: 256, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FaxNumber = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TelephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactTypeIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInformation", x => new { x.RequestId, x.Type, x.Identifier });
                    table.ForeignKey(
                        name: "FK_ContactInformation_StandardBusinessHeader_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "Sbdh",
                        principalTable: "StandardBusinessHeader",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionExecutionRecord",
                schema: "Subscription",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    ExecutionTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Successful = table.Column<bool>(type: "bit", nullable: false),
                    ResultsSent = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionExecutionRecord", x => new { x.SubscriptionId, x.ExecutionTime });
                    table.ForeignKey(
                        name: "FK_SubscriptionExecutionRecord_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "Subscription",
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionParameter",
                schema: "Subscription",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionParameter", x => new { x.SubscriptionId, x.Name });
                    table.ForeignKey(
                        name: "FK_SubscriptionParameter_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "Subscription",
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionSchedule",
                schema: "Subscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionId = table.Column<int>(type: "int", nullable: true),
                    Second = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Minute = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Hour = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DayOfMonth = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Month = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DayOfWeek = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionSchedule_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "Subscription",
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorReport",
                schema: "Epcis",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    SensorId = table.Column<int>(type: "int", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataProcessingMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Microorganism = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChemicalSubstance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<float>(type: "real", nullable: true),
                    Component = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BooleanValue = table.Column<bool>(type: "bit", nullable: false),
                    HexBinaryValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UriValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinValue = table.Column<float>(type: "real", nullable: true),
                    MaxValue = table.Column<float>(type: "real", nullable: true),
                    MeanValue = table.Column<float>(type: "real", nullable: true),
                    PercRank = table.Column<float>(type: "real", nullable: true),
                    PercValue = table.Column<float>(type: "real", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SDev = table.Column<float>(type: "real", nullable: true),
                    DeviceMetadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReport", x => new { x.EventId, x.SensorId, x.ReportId });
                    table.ForeignKey(
                        name: "FK_SensorReport_SensorElement_EventId_SensorId",
                        columns: x => new { x.EventId, x.SensorId },
                        principalSchema: "Epcis",
                        principalTable: "SensorElement",
                        principalColumns: new[] { "EventId", "SensorId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataField",
                schema: "Cbv",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Namespace = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    MasterdataType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MasterdataId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AttributeId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ParentName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ParentNamespace = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataField", x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.AttributeId, x.Namespace, x.Name });
                    table.ForeignKey(
                        name: "FK_MasterDataField_MasterDataAttribute_RequestId_MasterdataType_MasterdataId_AttributeId",
                        columns: x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.AttributeId },
                        principalSchema: "Cbv",
                        principalTable: "MasterDataAttribute",
                        principalColumns: new[] { "RequestId", "MasterdataType", "MasterdataId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                schema: "Epcis",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    SensorId = table.Column<int>(type: "int", nullable: true),
                    ReportId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Namespace = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    DateValue = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => new { x.EventId, x.FieldId });
                    table.ForeignKey(
                        name: "FK_Field_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Field_Field_EventId_ParentId",
                        columns: x => new { x.EventId, x.ParentId },
                        principalSchema: "Epcis",
                        principalTable: "Field",
                        principalColumns: new[] { "EventId", "FieldId" });
                    table.ForeignKey(
                        name: "FK_Field_SensorElement_EventId_SensorId",
                        columns: x => new { x.EventId, x.SensorId },
                        principalSchema: "Epcis",
                        principalTable: "SensorElement",
                        principalColumns: new[] { "EventId", "SensorId" });
                    table.ForeignKey(
                        name: "FK_Field_SensorReport_EventId_SensorId_ReportId",
                        columns: x => new { x.EventId, x.SensorId, x.ReportId },
                        principalSchema: "Epcis",
                        principalTable: "SensorReport",
                        principalColumns: new[] { "EventId", "SensorId", "ReportId" });
                });

            migrationBuilder.InsertData(
                schema: "Queries",
                table: "StoredQuery",
                columns: new[] { "Id", "DataSource", "Name", "UserId" },
                values: new object[,]
                {
                    { -2, "SimpleEventQuery", "SimpleEventQuery", null },
                    { -1, "SimpleMasterDataQuery", "SimpleMasterDataQuery", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_RequestId",
                schema: "Epcis",
                table: "Event",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Field_EventId_ParentId",
                schema: "Epcis",
                table: "Field",
                columns: new[] { "EventId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Field_EventId_SensorId_ReportId",
                schema: "Epcis",
                table: "Field",
                columns: new[] { "EventId", "SensorId", "ReportId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoredQuery_Name",
                schema: "Queries",
                table: "StoredQuery",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_QueryId",
                schema: "Subscription",
                table: "Subscription",
                column: "QueryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionSchedule_SubscriptionId",
                schema: "Subscription",
                table: "SubscriptionSchedule",
                column: "SubscriptionId",
                unique: true,
                filter: "[SubscriptionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessTransaction",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "ContactInformation",
                schema: "Sbdh");

            migrationBuilder.DropTable(
                name: "CorrectiveEventId",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Destination",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Epc",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Field",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "MasterDataChildren",
                schema: "Cbv");

            migrationBuilder.DropTable(
                name: "MasterDataField",
                schema: "Cbv");

            migrationBuilder.DropTable(
                name: "PendingRequest",
                schema: "Subscription");

            migrationBuilder.DropTable(
                name: "PersistentDisposition",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Source",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "StoredQueryParameter",
                schema: "Subscription");

            migrationBuilder.DropTable(
                name: "SubscriptionCallback",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "SubscriptionExecutionRecord",
                schema: "Subscription");

            migrationBuilder.DropTable(
                name: "SubscriptionParameter",
                schema: "Subscription");

            migrationBuilder.DropTable(
                name: "SubscriptionSchedule",
                schema: "Subscription");

            migrationBuilder.DropTable(
                name: "StandardBusinessHeader",
                schema: "Sbdh");

            migrationBuilder.DropTable(
                name: "SensorReport",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "MasterDataAttribute",
                schema: "Cbv");

            migrationBuilder.DropTable(
                name: "Subscription",
                schema: "Subscription");

            migrationBuilder.DropTable(
                name: "SensorElement",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "MasterData",
                schema: "Cbv");

            migrationBuilder.DropTable(
                name: "StoredQuery",
                schema: "Queries");

            migrationBuilder.DropTable(
                name: "Event",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Request",
                schema: "Epcis");
        }
    }
}
