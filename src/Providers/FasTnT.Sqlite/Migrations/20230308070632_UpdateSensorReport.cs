using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSensorReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReport_SensorElement_EventId_SensorIndex",
                schema: "Epcis",
                table: "SensorReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorReport",
                schema: "Epcis",
                table: "SensorReport");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorReport",
                schema: "Epcis",
                table: "SensorReport",
                columns: new[] { "EventId", "Index" });

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReport_Event_EventId",
                schema: "Epcis",
                table: "SensorReport",
                column: "EventId",
                principalSchema: "Epcis",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SensorReport_Event_EventId",
                schema: "Epcis",
                table: "SensorReport");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SensorReport",
                schema: "Epcis",
                table: "SensorReport");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SensorReport",
                schema: "Epcis",
                table: "SensorReport",
                columns: new[] { "EventId", "SensorIndex", "Index" });

            migrationBuilder.AddForeignKey(
                name: "FK_SensorReport_SensorElement_EventId_SensorIndex",
                schema: "Epcis",
                table: "SensorReport",
                columns: new[] { "EventId", "SensorIndex" },
                principalSchema: "Epcis",
                principalTable: "SensorElement",
                principalColumns: new[] { "EventId", "Index" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
