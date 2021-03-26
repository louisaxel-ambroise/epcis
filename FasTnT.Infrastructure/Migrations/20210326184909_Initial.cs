using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FasTnT.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Epcis");

            migrationBuilder.CreateTable(
                name: "Request",
                schema: "Epcis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaptureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SchemaVersion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "Epcis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: true),
                    CaptureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventTimeZoneOffset = table.Column<short>(type: "smallint", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Action = table.Column<short>(type: "smallint", nullable: false),
                    EventId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ReadPoint = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BusinessLocation = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BusinessStep = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Disposition = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TransformationId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CorrectiveDeclarationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MasterData",
                schema: "Epcis",
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
                schema: "Epcis",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Standard = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    TypeVersion = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    InstanceIdentifier = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "BusinessTransaction",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<short>(type: "smallint", nullable: false),
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
                name: "CustomField",
                schema: "Epcis",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Namespace = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TextValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    DateValue = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomField", x => new { x.EventId, x.FieldId });
                    table.ForeignKey(
                        name: "FK_CustomField_CustomField_EventId_ParentId",
                        columns: x => new { x.EventId, x.ParentId },
                        principalSchema: "Epcis",
                        principalTable: "CustomField",
                        principalColumns: new[] { "EventId", "FieldId" });
                    table.ForeignKey(
                        name: "FK_CustomField_Event_EventId",
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
                    IsQuantity = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
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
                name: "SourceDestination",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    Direction = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceDestination", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_SourceDestination_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataAttribute",
                schema: "Epcis",
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
                        principalSchema: "Epcis",
                        principalTable: "MasterData",
                        principalColumns: new[] { "RequestId", "Type", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactInformation",
                schema: "Epcis",
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
                        principalSchema: "Epcis",
                        principalTable: "StandardBusinessHeader",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataField",
                schema: "Epcis",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Namespace = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    MasterdataType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MasterdataId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    AttributeId = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataField", x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.AttributeId, x.Namespace, x.Name });
                    table.ForeignKey(
                        name: "FK_MasterDataField_MasterDataAttribute_RequestId_MasterdataType_MasterdataId_AttributeId",
                        columns: x => new { x.RequestId, x.MasterdataType, x.MasterdataId, x.AttributeId },
                        principalSchema: "Epcis",
                        principalTable: "MasterDataAttribute",
                        principalColumns: new[] { "RequestId", "MasterdataType", "MasterdataId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomField_EventId_ParentId",
                schema: "Epcis",
                table: "CustomField",
                columns: new[] { "EventId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Event_RequestId",
                schema: "Epcis",
                table: "Event",
                column: "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessTransaction",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "ContactInformation",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "CustomField",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Epc",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "MasterDataField",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "SourceDestination",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "StandardBusinessHeader",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "MasterDataAttribute",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Event",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "MasterData",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Request",
                schema: "Epcis");
        }
    }
}
