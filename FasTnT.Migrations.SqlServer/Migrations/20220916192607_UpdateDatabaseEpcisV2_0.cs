﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    public partial class UpdateDatabaseEpcisV2_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_SubscriptionSchedule_ScheduleId",
                schema: "Subscription",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_ScheduleId",
                schema: "Subscription",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                schema: "Subscription",
                table: "Subscription");

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

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                schema: "Users",
                table: "User",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                schema: "Subscription",
                table: "SubscriptionSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormatterName",
                schema: "Subscription",
                table: "Subscription",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignatureToken",
                schema: "Subscription",
                table: "Subscription",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "FieldType",
                schema: "Epcis",
                table: "CustomField",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                schema: "Epcis",
                table: "CustomField",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SensorId",
                schema: "Epcis",
                table: "CustomField",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SensorReportCustomField_SensorId",
                schema: "Epcis",
                table: "CustomField",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersistentDisposition",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistentDisposition", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_PersistentDisposition_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorElement",
                schema: "Epcis",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    SensorId = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceMetadata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataProcessingMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BizRules = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorElement", x => new { x.EventId, x.SensorId });
                    table.ForeignKey(
                        name: "FK_SensorElement_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredQuery",
                schema: "Queries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    DataSource = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredQuery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SensorReport",
                schema: "Epcis",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    SensorId = table.Column<int>(type: "int", nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RawData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataProcessingMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Microorganism = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChemicalSubstance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Component = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StringValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BooleanValue = table.Column<bool>(type: "bit", nullable: false),
                    HexBinaryValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UriValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinValue = table.Column<float>(type: "real", nullable: true),
                    MaxValue = table.Column<float>(type: "real", nullable: true),
                    MeanValue = table.Column<float>(type: "real", nullable: true),
                    PercRank = table.Column<float>(type: "real", nullable: true),
                    PercValue = table.Column<float>(type: "real", nullable: true),
                    UnitOfMeasure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SDev = table.Column<float>(type: "real", nullable: true),
                    DeviceMetadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReport", x => new { x.EventId, x.SensorId, x.ReportId });
                    table.ForeignKey(
                        name: "FK_SensorReport_SensorElement_EventId_SensorId",
                        columns: x => new { x.EventId, x.SensorId },
                        principalSchema: "Epcis",
                        principalTable: "SensorElement",
                        principalColumns: new[] { "EventId", "SensorId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoredQueryParameter",
                schema: "Subscription",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QueryId = table.Column<int>(type: "int", nullable: false),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredQueryParameter", x => new { x.QueryId, x.Name });
                    table.ForeignKey(
                        name: "FK_StoredQueryParameter_StoredQuery_QueryId",
                        column: x => x.QueryId,
                        principalSchema: "Queries",
                        principalTable: "StoredQuery",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Queries",
                table: "StoredQuery",
                columns: new[] { "Id", "DataSource", "Name", "Username" },
                values: new object[] { -2, "SimpleEventQuery", "SimpleEventQuery", null });

            migrationBuilder.InsertData(
                schema: "Queries",
                table: "StoredQuery",
                columns: new[] { "Id", "DataSource", "Name", "Username" },
                values: new object[] { -1, "SimpleMasterDataQuery", "SimpleMasterDataQuery", null });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionSchedule_SubscriptionId",
                schema: "Subscription",
                table: "SubscriptionSchedule",
                column: "SubscriptionId",
                unique: true,
                filter: "[SubscriptionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomField_EventId_SensorId",
                schema: "Epcis",
                table: "CustomField",
                columns: new[] { "EventId", "SensorId" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomField_EventId_SensorReportCustomField_SensorId_ReportId",
                schema: "Epcis",
                table: "CustomField",
                columns: new[] { "EventId", "SensorReportCustomField_SensorId", "ReportId" });

            migrationBuilder.CreateIndex(
                name: "IX_StoredQuery_Name",
                schema: "Queries",
                table: "StoredQuery",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomField_SensorElement_EventId_SensorId",
                schema: "Epcis",
                table: "CustomField",
                columns: new[] { "EventId", "SensorId" },
                principalSchema: "Epcis",
                principalTable: "SensorElement",
                principalColumns: new[] { "EventId", "SensorId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CustomField_SensorReport_EventId_SensorReportCustomField_SensorId_ReportId",
                schema: "Epcis",
                table: "CustomField",
                columns: new[] { "EventId", "SensorReportCustomField_SensorId", "ReportId" },
                principalSchema: "Epcis",
                principalTable: "SensorReport",
                principalColumns: new[] { "EventId", "SensorId", "ReportId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionSchedule_Subscription_SubscriptionId",
                schema: "Subscription",
                table: "SubscriptionSchedule",
                column: "SubscriptionId",
                principalSchema: "Subscription",
                principalTable: "Subscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomField_SensorElement_EventId_SensorId",
                schema: "Epcis",
                table: "CustomField");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomField_SensorReport_EventId_SensorReportCustomField_SensorId_ReportId",
                schema: "Epcis",
                table: "CustomField");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionSchedule_Subscription_SubscriptionId",
                schema: "Subscription",
                table: "SubscriptionSchedule");

            migrationBuilder.DropTable(
                name: "PersistentDisposition",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "SensorReport",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "StoredQueryParameter",
                schema: "Subscription");

            migrationBuilder.DropTable(
                name: "SensorElement",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "StoredQuery",
                schema: "Queries");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionSchedule_SubscriptionId",
                schema: "Subscription",
                table: "SubscriptionSchedule");

            migrationBuilder.DropIndex(
                name: "IX_CustomField_EventId_SensorId",
                schema: "Epcis",
                table: "CustomField");

            migrationBuilder.DropIndex(
                name: "IX_CustomField_EventId_SensorReportCustomField_SensorId_ReportId",
                schema: "Epcis",
                table: "CustomField");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                schema: "Subscription",
                table: "SubscriptionSchedule");

            migrationBuilder.DropColumn(
                name: "FormatterName",
                schema: "Subscription",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "SignatureToken",
                schema: "Subscription",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "FieldType",
                schema: "Epcis",
                table: "CustomField");

            migrationBuilder.DropColumn(
                name: "ReportId",
                schema: "Epcis",
                table: "CustomField");

            migrationBuilder.DropColumn(
                name: "SensorId",
                schema: "Epcis",
                table: "CustomField");

            migrationBuilder.DropColumn(
                name: "SensorReportCustomField_SensorId",
                schema: "Epcis",
                table: "CustomField");

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

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                schema: "Users",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                schema: "Subscription",
                table: "Subscription",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_ScheduleId",
                schema: "Subscription",
                table: "Subscription",
                column: "ScheduleId",
                unique: true,
                filter: "[ScheduleId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_SubscriptionSchedule_ScheduleId",
                schema: "Subscription",
                table: "Subscription",
                column: "ScheduleId",
                principalSchema: "Subscription",
                principalTable: "SubscriptionSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
