using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMasterdataTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MasterDataAttribute_MasterData_RequestId_MasterdataType_MasterdataId",
                schema: "Cbv",
                table: "MasterDataAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterDataField_MasterDataAttribute_RequestId_MasterdataType_MasterdataId_AttributeId",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MasterDataField",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MasterDataAttribute",
                schema: "Cbv",
                table: "MasterDataAttribute");

            migrationBuilder.DropColumn(
                name: "MasterdataType",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "MasterdataId",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "AttributeId",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                schema: "Cbv",
                table: "MasterDataField",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "MasterdataId",
                schema: "Cbv",
                table: "MasterDataAttribute",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "MasterdataType",
                schema: "Cbv",
                table: "MasterDataAttribute",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<int>(
                name: "Index",
                schema: "Cbv",
                table: "MasterDataAttribute",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasterDataField",
                schema: "Cbv",
                table: "MasterDataField",
                columns: new[] { "RequestId", "Index" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasterDataAttribute",
                schema: "Cbv",
                table: "MasterDataAttribute",
                columns: new[] { "RequestId", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_MasterDataField_RequestId_AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField",
                columns: new[] { "RequestId", "AttributeIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_MasterDataAttribute_RequestId_MasterdataType_MasterdataId",
                schema: "Cbv",
                table: "MasterDataAttribute",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MasterDataAttribute_MasterData_RequestId_MasterdataType_MasterdataId",
                schema: "Cbv",
                table: "MasterDataAttribute",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId" },
                principalSchema: "Cbv",
                principalTable: "MasterData",
                principalColumns: new[] { "RequestId", "Type", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_MasterDataField_MasterDataAttribute_RequestId_AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField",
                columns: new[] { "RequestId", "AttributeIndex" },
                principalSchema: "Cbv",
                principalTable: "MasterDataAttribute",
                principalColumns: new[] { "RequestId", "Index" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MasterDataAttribute_MasterData_RequestId_MasterdataType_MasterdataId",
                schema: "Cbv",
                table: "MasterDataAttribute");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterDataField_MasterDataAttribute_RequestId_AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MasterDataField",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropIndex(
                name: "IX_MasterDataField_RequestId_AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MasterDataAttribute",
                schema: "Cbv",
                table: "MasterDataAttribute");

            migrationBuilder.DropIndex(
                name: "IX_MasterDataAttribute_RequestId_MasterdataType_MasterdataId",
                schema: "Cbv",
                table: "MasterDataAttribute");

            migrationBuilder.DropColumn(
                name: "Index",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "Index",
                schema: "Cbv",
                table: "MasterDataAttribute");

            migrationBuilder.AddColumn<string>(
                name: "MasterdataType",
                schema: "Cbv",
                table: "MasterDataField",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MasterdataId",
                schema: "Cbv",
                table: "MasterDataField",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AttributeId",
                schema: "Cbv",
                table: "MasterDataField",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "MasterdataType",
                schema: "Cbv",
                table: "MasterDataAttribute",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MasterdataId",
                schema: "Cbv",
                table: "MasterDataAttribute",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasterDataField",
                schema: "Cbv",
                table: "MasterDataField",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId", "AttributeId", "Namespace", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasterDataAttribute",
                schema: "Cbv",
                table: "MasterDataAttribute",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_MasterDataAttribute_MasterData_RequestId_MasterdataType_MasterdataId",
                schema: "Cbv",
                table: "MasterDataAttribute",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId" },
                principalSchema: "Cbv",
                principalTable: "MasterData",
                principalColumns: new[] { "RequestId", "Type", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MasterDataField_MasterDataAttribute_RequestId_MasterdataType_MasterdataId_AttributeId",
                schema: "Cbv",
                table: "MasterDataField",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId", "AttributeId" },
                principalSchema: "Cbv",
                principalTable: "MasterDataAttribute",
                principalColumns: new[] { "RequestId", "MasterdataType", "MasterdataId", "Id" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
