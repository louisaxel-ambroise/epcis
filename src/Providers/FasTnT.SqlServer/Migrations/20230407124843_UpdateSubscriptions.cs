using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER [Subscriptions].[SubscriptionInitialRequests];");
            migrationBuilder.Sql(@"DROP TRIGGER [Epcis].[InsertPendingRequests];");

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
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BufferRequestIds",
                schema: "Subscriptions",
                table: "Subscription",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastExecutedTime",
                schema: "Subscriptions",
                table: "Subscription",
                type: "datetime2",
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
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CaptureTime",
                schema: "Epcis",
                table: "Event",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "PendingRequest",
                schema: "Subscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false)
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
                    SubscriptionId = table.Column<int>(type: "int", nullable: false),
                    ExecutionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultsSent = table.Column<bool>(type: "bit", nullable: false),
                    Successful = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionExecutionRecord", x => new { x.SubscriptionId, x.ExecutionTime });
                });
        }
    }
}
