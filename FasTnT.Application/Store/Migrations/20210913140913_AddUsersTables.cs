using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FasTnT.Application.Migrations
{
    public partial class AddUsersTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "Epcis",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    SecuredKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CanCapture = table.Column<bool>(type: "bit", nullable: false),
                    CanQuery = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDefaultQueryParameter",
                schema: "Users",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDefaultQueryParameter", x => new { x.UserId, x.Name });
                    table.ForeignKey(
                        name: "FK_UserDefaultQueryParameter_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_UserId",
                schema: "Epcis",
                table: "Request",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_User_UserId",
                schema: "Epcis",
                table: "Request",
                column: "UserId",
                principalSchema: "Users",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_User_UserId",
                schema: "Epcis",
                table: "Request");

            migrationBuilder.DropTable(
                name: "UserDefaultQueryParameter",
                schema: "Users");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Request_UserId",
                schema: "Epcis",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Epcis",
                table: "Request");
        }
    }
}
