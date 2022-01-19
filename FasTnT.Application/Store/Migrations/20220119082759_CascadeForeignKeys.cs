using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    public partial class CascadeForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Request_RequestId",
                schema: "Epcis",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterDataChildren_MasterData_MasterDataRequestId_MasterDataType_MasterDataId",
                schema: "Cbv",
                table: "MasterDataChildren");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_SubscriptionSchedule_ScheduleId",
                schema: "Subscription",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionCallback_Request_RequestId",
                schema: "Epcis",
                table: "SubscriptionCallback");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Request_RequestId",
                schema: "Epcis",
                table: "Event",
                column: "RequestId",
                principalSchema: "Epcis",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MasterDataChildren_MasterData_MasterDataRequestId_MasterDataType_MasterDataId",
                schema: "Cbv",
                table: "MasterDataChildren",
                columns: new[] { "MasterDataRequestId", "MasterDataType", "MasterDataId" },
                principalSchema: "Cbv",
                principalTable: "MasterData",
                principalColumns: new[] { "RequestId", "Type", "Id" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_SubscriptionSchedule_ScheduleId",
                schema: "Subscription",
                table: "Subscription",
                column: "ScheduleId",
                principalSchema: "Subscription",
                principalTable: "SubscriptionSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionCallback_Request_RequestId",
                schema: "Epcis",
                table: "SubscriptionCallback",
                column: "RequestId",
                principalSchema: "Epcis",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Request_RequestId",
                schema: "Epcis",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_MasterDataChildren_MasterData_MasterDataRequestId_MasterDataType_MasterDataId",
                schema: "Cbv",
                table: "MasterDataChildren");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_SubscriptionSchedule_ScheduleId",
                schema: "Subscription",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_SubscriptionCallback_Request_RequestId",
                schema: "Epcis",
                table: "SubscriptionCallback");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Request_RequestId",
                schema: "Epcis",
                table: "Event",
                column: "RequestId",
                principalSchema: "Epcis",
                principalTable: "Request",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MasterDataChildren_MasterData_MasterDataRequestId_MasterDataType_MasterDataId",
                schema: "Cbv",
                table: "MasterDataChildren",
                columns: new[] { "MasterDataRequestId", "MasterDataType", "MasterDataId" },
                principalSchema: "Cbv",
                principalTable: "MasterData",
                principalColumns: new[] { "RequestId", "Type", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_SubscriptionSchedule_ScheduleId",
                schema: "Subscription",
                table: "Subscription",
                column: "ScheduleId",
                principalSchema: "Subscription",
                principalTable: "SubscriptionSchedule",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubscriptionCallback_Request_RequestId",
                schema: "Epcis",
                table: "SubscriptionCallback",
                column: "RequestId",
                principalSchema: "Epcis",
                principalTable: "Request",
                principalColumn: "Id");
        }
    }
}
