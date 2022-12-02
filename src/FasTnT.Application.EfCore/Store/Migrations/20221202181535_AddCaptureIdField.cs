using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddCaptureIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasParent",
                schema: "Epcis",
                table: "Field");

            migrationBuilder.AddColumn<string>(
                name: "CaptureId",
                schema: "Epcis",
                table: "Request",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaptureId",
                schema: "Epcis",
                table: "Request");

            migrationBuilder.AddColumn<bool>(
                name: "HasParent",
                schema: "Epcis",
                table: "Field",
                type: "bit",
                nullable: false,
                computedColumnSql: "(CASE WHEN [ParentId] IS NOT NULL THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END)",
                stored: true);
        }
    }
}
