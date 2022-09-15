using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    public partial class UpdateSubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignatureToken",
                schema: "Subscription",
                table: "Subscription",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                schema: "Subscription",
                table: "Subscription",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "standard");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureToken",
                schema: "Subscription",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "Subscription",
                table: "Subscription");
        }
    }
}
