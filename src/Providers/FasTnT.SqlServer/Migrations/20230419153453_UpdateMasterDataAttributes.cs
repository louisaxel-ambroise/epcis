using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMasterDataAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "AttributeId",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "ParentName",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "ParentNamespace",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.AddColumn<int>(
                name: "AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField",
                type: "int",
                maxLength: 256,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Index",
                schema: "Cbv",
                table: "MasterDataField",
                type: "int",
                maxLength: 256,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentIndex",
                schema: "Cbv",
                table: "MasterDataField",
                type: "int",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Index",
                schema: "Cbv",
                table: "MasterDataAttribute",
                type: "int",
                maxLength: 256,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasterDataField",
                schema: "Cbv",
                table: "MasterDataField",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId", "AttributeIndex", "Index" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_MasterDataAttribute",
                schema: "Cbv",
                table: "MasterDataAttribute",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId", "Index" });

            migrationBuilder.AddForeignKey(
                name: "FK_MasterDataField_MasterDataAttribute_RequestId_MasterdataType_MasterdataId_AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField",
                columns: new[] { "RequestId", "MasterdataType", "MasterdataId", "AttributeIndex" },
                principalSchema: "Cbv",
                principalTable: "MasterDataAttribute",
                principalColumns: new[] { "RequestId", "MasterdataType", "MasterdataId", "Index" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MasterDataField_MasterDataAttribute_RequestId_MasterdataType_MasterdataId_AttributeIndex",
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
                name: "AttributeIndex",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "Index",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "ParentIndex",
                schema: "Cbv",
                table: "MasterDataField");

            migrationBuilder.DropColumn(
                name: "Index",
                schema: "Cbv",
                table: "MasterDataAttribute");

            migrationBuilder.AddColumn<string>(
                name: "AttributeId",
                schema: "Cbv",
                table: "MasterDataField",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ParentName",
                schema: "Cbv",
                table: "MasterDataField",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentNamespace",
                schema: "Cbv",
                table: "MasterDataField",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

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
