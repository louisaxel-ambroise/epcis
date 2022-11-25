using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsQuantity",
                schema: "Epcis",
                table: "Epc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsQuantity",
                schema: "Epcis",
                table: "Epc",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
