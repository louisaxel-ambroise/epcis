using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MissingSensorReportField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoordinateReferenceSystem",
                schema: "Epcis",
                table: "SensorReport",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoordinateReferenceSystem",
                schema: "Epcis",
                table: "SensorReport");
        }
    }
}
