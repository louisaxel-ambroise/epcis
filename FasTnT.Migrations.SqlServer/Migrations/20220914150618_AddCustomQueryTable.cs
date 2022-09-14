using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    public partial class AddCustomQueryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Queries");

            migrationBuilder.RenameColumn(
                name: "Value",
                schema: "Subscription",
                table: "SubscriptionParameter",
                newName: "Values");

            migrationBuilder.RenameColumn(
                name: "RecordIfEmpty",
                schema: "Subscription",
                table: "Subscription",
                newName: "ReportIfEmpty");

            migrationBuilder.CreateTable(
                name: "CustomQuery",
                schema: "Queries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomQuery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomQueryParameter",
                schema: "Subscription",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomQueryParameter", x => new { x.QueryId, x.Name });
                    table.ForeignKey(
                        name: "FK_CustomQueryParameter_CustomQuery_QueryId",
                        column: x => x.QueryId,
                        principalSchema: "Queries",
                        principalTable: "CustomQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomQueryParameter",
                schema: "Subscription");

            migrationBuilder.DropTable(
                name: "CustomQuery",
                schema: "Queries");

            migrationBuilder.RenameColumn(
                name: "Values",
                schema: "Subscription",
                table: "SubscriptionParameter",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "ReportIfEmpty",
                schema: "Subscription",
                table: "Subscription",
                newName: "RecordIfEmpty");
        }
    }
}
