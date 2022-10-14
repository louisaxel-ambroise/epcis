using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    public partial class RemoveUserTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "Username",
                schema: "Queries",
                table: "StoredQuery",
                newName: "UserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                schema: "Epcis",
                table: "Request",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.DropSchema(
                name: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "Queries",
                table: "StoredQuery",
                newName: "Username");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                schema: "Epcis",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanCapture = table.Column<bool>(type: "bit", nullable: false),
                    CanQuery = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Salt = table.Column<byte[]>(type: "varbinary(20)", maxLength: 20, nullable: false),
                    SecuredKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
    }
}
