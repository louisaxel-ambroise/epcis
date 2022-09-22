using Microsoft.EntityFrameworkCore.Migrations;

namespace FasTnT.Application.Migrations
{
    public partial class SubscriptionTriggerRecordTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InitialRecordTime",
                schema: "Subscription",
                table: "Subscription",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"CREATE TRIGGER [Subscription].[SubscriptionInitialRequests] 
ON [Subscription].[Subscription] 
AFTER INSERT 
AS 
	INSERT INTO [Subscription].[PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT r.[Id], i.[Id] 
	FROM inserted i, [Epcis].[Request] r
    WHERE i.[InitialRecordTime] IS NOT NULL AND r.[CaptureDate] >= i.[InitialRecordTime];");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER [Subscription].[SubscriptionInitialRequests];");

            migrationBuilder.DropColumn(
                name: "InitialRecordTime",
                schema: "Subscription",
                table: "Subscription");
        }
    }
}
