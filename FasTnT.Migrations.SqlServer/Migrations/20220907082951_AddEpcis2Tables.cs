using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    public partial class AddEpcis2Tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceMetadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Microorganism = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChemicalSubstance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersistentDisposition",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "SensorReport",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "SensorElement",
                schema: "Epcis");
        }
    }
}
