using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FasTnT.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class InitialV2_5_0 : Migration
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
                name: "Queries");

            migrationBuilder.EnsureSchema(
                name: "Subscriptions");

            migrationBuilder.CreateTable(
                name: "Request",
                schema: "Epcis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaptureId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RecordTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DocumentTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SchemaVersion = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredQuery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                schema: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SignatureToken = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    QueryName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    FormatterName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Trigger = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ReportIfEmpty = table.Column<bool>(type: "boolean", nullable: false),
                    Destination = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    InitialRecordTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastExecutedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BufferRequestIds = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "Epcis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestId = table.Column<int>(type: "integer", nullable: true),
                    EventTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventTimeZoneOffset = table.Column<short>(type: "smallint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Action = table.Column<short>(type: "smallint", nullable: false),
                    EventId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CertificationInfo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ReadPoint = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BusinessLocation = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BusinessStep = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Disposition = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TransformationId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CorrectiveDeclarationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CorrectiveReason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
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
                    Type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "integer", nullable: false)
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
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Standard = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    TypeVersion = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    InstanceIdentifier = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CallbackType = table.Column<short>(type: "smallint", nullable: false),
                    Reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
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
                schema: "Queries",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    QueryId = table.Column<int>(type: "integer", nullable: false),
                    Values = table.Column<string>(type: "text", nullable: true)
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
                name: "SubscriptionParameter",
                schema: "Subscriptions",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SubscriptionId = table.Column<int>(type: "integer", nullable: false),
                    Values = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionParameter", x => new { x.SubscriptionId, x.Name });
                    table.ForeignKey(
                        name: "FK_SubscriptionParameter_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "Subscriptions",
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionSchedule",
                schema: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "integer", nullable: false),
                    Second = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Minute = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Hour = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DayOfMonth = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Month = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DayOfWeek = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionSchedule", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_SubscriptionSchedule_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "Subscriptions",
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessTransaction",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
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
                    CorrectiveId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
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
                    Type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
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
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true)
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
                name: "Field",
                schema: "Epcis",
                columns: table => new
                {
                    Index = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Namespace = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TextValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NumericValue = table.Column<double>(type: "double precision", nullable: true),
                    DateValue = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EntityIndex = table.Column<int>(type: "integer", nullable: true),
                    ParentIndex = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => new { x.EventId, x.Index });
                    table.ForeignKey(
                        name: "FK_Field_Event_EventId",
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
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
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
                    Index = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeviceId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeviceMetadata = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RawData = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataProcessingMethod = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    BizRules = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorElement", x => new { x.EventId, x.Index });
                    table.ForeignKey(
                        name: "FK_SensorElement_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorReport",
                schema: "Epcis",
                columns: table => new
                {
                    Index = table.Column<int>(type: "integer", nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    SensorIndex = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    DeviceId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RawData = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    DataProcessingMethod = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CoordinateReferenceSystem = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Microorganism = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ChemicalSubstance = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Value = table.Column<float>(type: "real", nullable: true),
                    Component = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    StringValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    BooleanValue = table.Column<bool>(type: "boolean", nullable: false),
                    HexBinaryValue = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    UriValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    MinValue = table.Column<float>(type: "real", nullable: true),
                    MaxValue = table.Column<float>(type: "real", nullable: true),
                    MeanValue = table.Column<float>(type: "real", nullable: true),
                    PercRank = table.Column<float>(type: "real", nullable: true),
                    PercValue = table.Column<float>(type: "real", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SDev = table.Column<float>(type: "real", nullable: true),
                    DeviceMetadata = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReport", x => new { x.EventId, x.Index });
                    table.ForeignKey(
                        name: "FK_SensorReport_Event_EventId",
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
                    Type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
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
                    Index = table.Column<int>(type: "integer", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    MasterdataType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MasterdataId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataAttribute", x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.Index });
                    table.ForeignKey(
                        name: "FK_MasterDataAttribute_MasterData_RequestId_MasterdataType_Mas~",
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
                    ChildrenId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MasterDataRequestId = table.Column<int>(type: "integer", nullable: false),
                    MasterDataType = table.Column<string>(type: "character varying(256)", nullable: false),
                    MasterDataId = table.Column<string>(type: "character varying(256)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataChildren", x => new { x.MasterDataRequestId, x.MasterDataType, x.MasterDataId, x.ChildrenId });
                    table.ForeignKey(
                        name: "FK_MasterDataChildren_MasterData_MasterDataRequestId_MasterDat~",
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
                    Identifier = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    Contact = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailAddress = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    FaxNumber = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TelephoneNumber = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContactTypeIdentifier = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
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
                name: "MasterDataField",
                schema: "Cbv",
                columns: table => new
                {
                    Index = table.Column<int>(type: "integer", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    MasterdataType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MasterdataId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ParentIndex = table.Column<int>(type: "integer", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Namespace = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    AttributeIndex = table.Column<int>(type: "integer", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataField", x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.Index });
                    table.ForeignKey(
                        name: "FK_MasterDataField_MasterDataAttribute_RequestId_MasterdataTyp~",
                        columns: x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.AttributeIndex },
                        principalSchema: "Cbv",
                        principalTable: "MasterDataAttribute",
                        principalColumns: new[] { "RequestId", "MasterdataType", "MasterdataId", "Index" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_RequestId",
                schema: "Epcis",
                table: "Event",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_MasterDataField_RequestId_MasterdataType_MasterdataId_Attri~",
                schema: "Cbv",
                table: "MasterDataField",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId", "AttributeIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_StoredQuery_Name",
                schema: "Queries",
                table: "StoredQuery",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_Name",
                schema: "Subscriptions",
                table: "Subscription",
                column: "Name",
                unique: true);

            migrationBuilder.Sql(@"CREATE VIEW ""Cbv"".""CurrentMasterdata""
AS
SELECT MAX(""RequestId"") AS ""RequestId"", ""Type"", ""Id"" FROM ""Cbv"".""MasterData"" GROUP BY ""Type"", ""Id"";");

            migrationBuilder.Sql(@"CREATE VIEW ""Cbv"".""MasterdataHierarchy"" AS
WITH RECURSIVE hierarchy(""Root"", ""Type"", ""Id"")
AS (
	SELECT ""Id"", ""Type"", ""Id""
	FROM ""Cbv"".""CurrentMasterdata""
	UNION ALL
	SELECT h.""Id"", ""MasterDataType"", ""ChildrenId""
	FROM ""Cbv"".""MasterDataChildren"" children
	JOIN ""Cbv"".""CurrentMasterdata"" cur ON cur.""Type"" = children.""MasterDataType"" AND cur.""Id"" = ""ChildrenId"" 
	JOIN hierarchy h ON ""MasterDataType"" = h.""Type"" AND ""MasterDataId"" = h.""Id""
)
SELECT ""Root"", ""Type"", ""Id"" 
FROM hierarchy h");
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
                name: "PersistentDisposition",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "SensorElement",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "SensorReport",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Source",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "StoredQueryParameter",
                schema: "Queries");

            migrationBuilder.DropTable(
                name: "SubscriptionCallback",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "SubscriptionParameter",
                schema: "Subscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionSchedule",
                schema: "Subscriptions");

            migrationBuilder.DropTable(
                name: "StandardBusinessHeader",
                schema: "Sbdh");

            migrationBuilder.DropTable(
                name: "MasterDataAttribute",
                schema: "Cbv");

            migrationBuilder.DropTable(
                name: "Event",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "StoredQuery",
                schema: "Queries");

            migrationBuilder.DropTable(
                name: "Subscription",
                schema: "Subscriptions");

            migrationBuilder.DropTable(
                name: "MasterData",
                schema: "Cbv");

            migrationBuilder.DropTable(
                name: "Request",
                schema: "Epcis");
        }
    }
}
