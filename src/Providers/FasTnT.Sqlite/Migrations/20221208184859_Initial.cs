using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FasTnT.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendingRequest",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRequest", x => new { x.SubscriptionId, x.RequestId });
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CaptureId = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CaptureDate = table.Column<long>(type: "INTEGER", nullable: false),
                    DocumentTime = table.Column<long>(type: "INTEGER", nullable: false),
                    SchemaVersion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoredQuery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 80, nullable: true),
                    DataSource = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredQuery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: true),
                    CaptureTime = table.Column<long>(type: "INTEGER", nullable: false),
                    EventTime = table.Column<long>(type: "INTEGER", nullable: false),
                    EventTimeZoneOffset = table.Column<short>(type: "INTEGER", nullable: false),
                    Type = table.Column<short>(type: "INTEGER", nullable: false),
                    Action = table.Column<short>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    EventId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CertificationInfo = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ReadPoint = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    BusinessLocation = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    BusinessStep = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Disposition = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    TransformationId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    CorrectiveDeclarationTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    CorrectiveReason = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterData",
                columns: table => new
                {
                    Type = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Id = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterData", x => new { x.RequestId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_MasterData_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StandardBusinessHeader",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Standard = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    TypeVersion = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    InstanceIdentifier = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CreationDateTime = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandardBusinessHeader", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_StandardBusinessHeader_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionCallback",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscriptionId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    CallbackType = table.Column<short>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionCallback", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_SubscriptionCallback_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredQueryParameter",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    QueryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Values = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredQueryParameter", x => new { x.QueryId, x.Name });
                    table.ForeignKey(
                        name: "FK_StoredQueryParameter_StoredQuery_QueryId",
                        column: x => x.QueryId,
                        principalTable: "StoredQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    QueryName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    SignatureToken = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    FormatterName = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Trigger = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ReportIfEmpty = table.Column<bool>(type: "INTEGER", nullable: false),
                    Destination = table.Column<string>(type: "TEXT", nullable: true),
                    QueryId = table.Column<int>(type: "INTEGER", nullable: true),
                    InitialRecordTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscription_StoredQuery_QueryId",
                        column: x => x.QueryId,
                        principalTable: "StoredQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessTransaction",
                columns: table => new
                {
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessTransaction", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_BusinessTransaction_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorrectiveEventId",
                columns: table => new
                {
                    CorrectiveId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectiveEventId", x => new { x.EventId, x.CorrectiveId });
                    table.ForeignKey(
                        name: "FK_CorrectiveEventId_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Destination",
                columns: table => new
                {
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_Destination_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Epc",
                columns: table => new
                {
                    Type = table.Column<short>(type: "INTEGER", nullable: false),
                    Id = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<float>(type: "REAL", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Epc", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_Epc_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<short>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Namespace = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    TextValue = table.Column<string>(type: "TEXT", nullable: true),
                    NumericValue = table.Column<double>(type: "REAL", nullable: true),
                    DateValue = table.Column<long>(type: "INTEGER", nullable: true),
                    EntityIndex = table.Column<int>(type: "INTEGER", nullable: true),
                    ParentIndex = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => new { x.EventId, x.Index });
                    table.ForeignKey(
                        name: "FK_Field_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersistentDisposition",
                columns: table => new
                {
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistentDisposition", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_PersistentDisposition_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorElement",
                columns: table => new
                {
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: true),
                    DeviceMetadata = table.Column<string>(type: "TEXT", nullable: true),
                    RawData = table.Column<string>(type: "TEXT", nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    EndTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    DataProcessingMethod = table.Column<string>(type: "TEXT", nullable: true),
                    BizRules = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorElement", x => new { x.EventId, x.Index });
                    table.ForeignKey(
                        name: "FK_SensorElement_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                columns: table => new
                {
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_Source_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataAttribute",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    MasterdataType = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    MasterdataId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataAttribute", x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.Id });
                    table.ForeignKey(
                        name: "FK_MasterDataAttribute_MasterData_RequestId_MasterdataType_MasterdataId",
                        columns: x => new { x.RequestId, x.MasterdataType, x.MasterdataId },
                        principalTable: "MasterData",
                        principalColumns: new[] { "RequestId", "Type", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataChildren",
                columns: table => new
                {
                    ChildrenId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    MasterDataRequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    MasterDataType = table.Column<string>(type: "TEXT", nullable: false),
                    MasterDataId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataChildren", x => new { x.MasterDataRequestId, x.MasterDataType, x.MasterDataId, x.ChildrenId });
                    table.ForeignKey(
                        name: "FK_MasterDataChildren_MasterData_MasterDataRequestId_MasterDataType_MasterDataId",
                        columns: x => new { x.MasterDataRequestId, x.MasterDataType, x.MasterDataId },
                        principalTable: "MasterData",
                        principalColumns: new[] { "RequestId", "Type", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactInformation",
                columns: table => new
                {
                    Type = table.Column<short>(type: "INTEGER", maxLength: 256, nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    Contact = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailAddress = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    FaxNumber = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    TelephoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    ContactTypeIdentifier = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactInformation", x => new { x.RequestId, x.Type, x.Identifier });
                    table.ForeignKey(
                        name: "FK_ContactInformation_StandardBusinessHeader_RequestId",
                        column: x => x.RequestId,
                        principalTable: "StandardBusinessHeader",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionExecutionRecord",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutionTime = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Successful = table.Column<bool>(type: "INTEGER", nullable: false),
                    ResultsSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionExecutionRecord", x => new { x.SubscriptionId, x.ExecutionTime });
                    table.ForeignKey(
                        name: "FK_SubscriptionExecutionRecord_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionParameter",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Values = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionParameter", x => new { x.SubscriptionId, x.Name });
                    table.ForeignKey(
                        name: "FK_SubscriptionParameter_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Second = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Minute = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Hour = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    DayOfMonth = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Month = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    DayOfWeek = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionSchedule_Subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorReport",
                columns: table => new
                {
                    Index = table.Column<int>(type: "INTEGER", nullable: false),
                    EventId = table.Column<int>(type: "INTEGER", nullable: false),
                    SensorIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: true),
                    DeviceId = table.Column<string>(type: "TEXT", nullable: true),
                    RawData = table.Column<string>(type: "TEXT", nullable: true),
                    DataProcessingMethod = table.Column<string>(type: "TEXT", nullable: true),
                    Time = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Microorganism = table.Column<string>(type: "TEXT", nullable: true),
                    ChemicalSubstance = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<float>(type: "REAL", nullable: true),
                    Component = table.Column<string>(type: "TEXT", nullable: true),
                    StringValue = table.Column<string>(type: "TEXT", nullable: true),
                    BooleanValue = table.Column<bool>(type: "INTEGER", nullable: false),
                    HexBinaryValue = table.Column<string>(type: "TEXT", nullable: true),
                    UriValue = table.Column<string>(type: "TEXT", nullable: true),
                    MinValue = table.Column<float>(type: "REAL", nullable: true),
                    MaxValue = table.Column<float>(type: "REAL", nullable: true),
                    MeanValue = table.Column<float>(type: "REAL", nullable: true),
                    PercRank = table.Column<float>(type: "REAL", nullable: true),
                    PercValue = table.Column<float>(type: "REAL", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "TEXT", nullable: true),
                    SDev = table.Column<float>(type: "REAL", nullable: true),
                    DeviceMetadata = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReport", x => new { x.EventId, x.SensorIndex, x.Index });
                    table.ForeignKey(
                        name: "FK_SensorReport_SensorElement_EventId_SensorIndex",
                        columns: x => new { x.EventId, x.SensorIndex },
                        principalTable: "SensorElement",
                        principalColumns: new[] { "EventId", "Index" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataField",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Namespace = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false),
                    MasterdataType = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    MasterdataId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    AttributeId = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    ParentName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ParentNamespace = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Value = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataField", x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.AttributeId, x.Namespace, x.Name });
                    table.ForeignKey(
                        name: "FK_MasterDataField_MasterDataAttribute_RequestId_MasterdataType_MasterdataId_AttributeId",
                        columns: x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.AttributeId },
                        principalTable: "MasterDataAttribute",
                        principalColumns: new[] { "RequestId", "MasterdataType", "MasterdataId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StoredQuery",
                columns: new[] { "Id", "DataSource", "Name", "UserId" },
                values: new object[,]
                {
                    { -2, "SimpleEventQuery", "SimpleEventQuery", null },
                    { -1, "SimpleMasterDataQuery", "SimpleMasterDataQuery", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_RequestId",
                table: "Event",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredQuery_Name",
                table: "StoredQuery",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_QueryId",
                table: "Subscription",
                column: "QueryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionSchedule_SubscriptionId",
                table: "SubscriptionSchedule",
                column: "SubscriptionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessTransaction");

            migrationBuilder.DropTable(
                name: "ContactInformation");

            migrationBuilder.DropTable(
                name: "CorrectiveEventId");

            migrationBuilder.DropTable(
                name: "Destination");

            migrationBuilder.DropTable(
                name: "Epc");

            migrationBuilder.DropTable(
                name: "Field");

            migrationBuilder.DropTable(
                name: "MasterDataChildren");

            migrationBuilder.DropTable(
                name: "MasterDataField");

            migrationBuilder.DropTable(
                name: "PendingRequest");

            migrationBuilder.DropTable(
                name: "PersistentDisposition");

            migrationBuilder.DropTable(
                name: "SensorReport");

            migrationBuilder.DropTable(
                name: "Source");

            migrationBuilder.DropTable(
                name: "StoredQueryParameter");

            migrationBuilder.DropTable(
                name: "SubscriptionCallback");

            migrationBuilder.DropTable(
                name: "SubscriptionExecutionRecord");

            migrationBuilder.DropTable(
                name: "SubscriptionParameter");

            migrationBuilder.DropTable(
                name: "SubscriptionSchedule");

            migrationBuilder.DropTable(
                name: "StandardBusinessHeader");

            migrationBuilder.DropTable(
                name: "MasterDataAttribute");

            migrationBuilder.DropTable(
                name: "SensorElement");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "MasterData");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "StoredQuery");

            migrationBuilder.DropTable(
                name: "Request");
        }
    }
}
