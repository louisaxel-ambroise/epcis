using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER [SubscriptionInitialRequests];");
            migrationBuilder.Sql(@"DROP TRIGGER [InsertPendingRequests];");

            migrationBuilder.DropTable(
                name: "PendingRequest",
                schema: "Subscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionExecutionRecord",
                schema: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "CaptureTime",
                schema: "Epcis",
                table: "Event");

            migrationBuilder.RenameColumn(
                name: "CaptureTime",
                schema: "Epcis",
                table: "Request",
                newName: "RecordTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InitialRecordTime",
                schema: "Subscriptions",
                table: "Subscription",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BufferRequestIds",
                schema: "Subscriptions",
                table: "Subscription",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastExecutedTime",
                schema: "Subscriptions",
                table: "Subscription",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BufferRequestIds",
                schema: "Subscriptions",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "LastExecutedTime",
                schema: "Subscriptions",
                table: "Subscription");

            migrationBuilder.RenameColumn(
                name: "RecordTime",
                schema: "Epcis",
                table: "Request",
                newName: "CaptureTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InitialRecordTime",
                schema: "Subscriptions",
                table: "Subscription",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "CaptureTime",
                schema: "Epcis",
                table: "Event",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "PendingRequest",
                schema: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    RequestId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRequest", x => new { x.SubscriptionId, x.RequestId });
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionExecutionRecord",
                schema: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExecutionTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: true),
                    ResultsSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    Successful = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionExecutionRecord", x => new { x.SubscriptionId, x.ExecutionTime });
                });
        }
    }
}
