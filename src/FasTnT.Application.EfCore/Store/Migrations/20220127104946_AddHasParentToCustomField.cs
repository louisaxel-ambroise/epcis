using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    public partial class AddHasParentToCustomField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasParent",
                schema: "Epcis",
                table: "CustomField",
                type: "bit",
                nullable: false,
                computedColumnSql: "(CASE WHEN [ParentId] IS NOT NULL THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END)",
                stored: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasParent",
                schema: "Epcis",
                table: "CustomField");
        }
    }
}
