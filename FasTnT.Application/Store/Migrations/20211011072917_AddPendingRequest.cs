using Microsoft.EntityFrameworkCore.Migrations;

namespace FasTnT.Application.Migrations
{
    public partial class AddPendingRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestSubscription");

            migrationBuilder.CreateTable(
                name: "PendingRequest",
                schema: "Subscription",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    SubscriptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingRequest", x => new { x.RequestId, x.SubscriptionId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingRequest",
                schema: "Subscription");

            migrationBuilder.CreateTable(
                name: "RequestSubscription",
                columns: table => new
                {
                    PendingRequestsId = table.Column<int>(type: "int", nullable: false),
                    PendingSubscriptionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSubscription", x => new { x.PendingRequestsId, x.PendingSubscriptionsId });
                    table.ForeignKey(
                        name: "FK_RequestSubscription_Request_PendingRequestsId",
                        column: x => x.PendingRequestsId,
                        principalSchema: "Epcis",
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestSubscription_Subscription_PendingSubscriptionsId",
                        column: x => x.PendingSubscriptionsId,
                        principalSchema: "Subscription",
                        principalTable: "Subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestSubscription_PendingSubscriptionsId",
                table: "RequestSubscription",
                column: "PendingSubscriptionsId");
        }
    }
}
