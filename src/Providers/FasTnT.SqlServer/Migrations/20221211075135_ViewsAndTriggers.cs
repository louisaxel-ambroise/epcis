using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ViewsAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW [Cbv].[CurrentMasterdata]
AS
SELECT MAX([RequestId]) AS [RequestId], [Type], [Id] FROM [Cbv].[MasterData] GROUP BY [Type], [Id];");

            migrationBuilder.Sql(@"CREATE VIEW [Cbv].[MasterdataHierarchy] AS
WITH hierarchy([requestId], [root], [type], [id])
AS (
	SELECT [requestId], [id], [type], [id]
	FROM [Cbv].[CurrentMasterdata]
	UNION ALL
	SELECT cur.[RequestId], [hierarchy].[Id], [MasterDataType], [ChildrenId]
	FROM [Cbv].[MasterdataChildren] children
	JOIN [Cbv].[CurrentMasterdata] cur ON cur.[Type] = children.[MasterDataType] AND cur.[Id] = [ChildrenId] 
	JOIN hierarchy ON [MasterDataType] = hierarchy.[type] AND [MasterDataId] = hierarchy.[id] AND [MasterdataRequestId] = hierarchy.[RequestId]
)
SELECT [root], [type], [id] 
FROM [hierarchy]");

            migrationBuilder.Sql(@"CREATE TRIGGER [Epcis].[InsertPendingRequests] 
ON [Epcis].[Request] 
AFTER INSERT 
AS 
	INSERT INTO [Subscriptions].[PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT i.[Id], s.[Id] 
	FROM inserted i, [Subscriptions].[Subscription] s;");

            migrationBuilder.Sql(@"CREATE TRIGGER [Subscriptions].[SubscriptionInitialRequests] 
ON [Subscriptions].[Subscription] 
AFTER INSERT 
AS 
	INSERT INTO [Subscriptions].[PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT r.[Id], i.[Id] 
	FROM inserted i, [Epcis].[Request] r
    WHERE i.[InitialRecordTime] IS NOT NULL AND r.[CaptureTime] >= i.[InitialRecordTime];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER [Subscriptions].[SubscriptionInitialRequests];");
            migrationBuilder.Sql(@"DROP TRIGGER [Subscriptions].[InsertPendingRequests];");
            migrationBuilder.Sql(@"DROP VIEW [Cbv].[MasterdataHierarchy];");
            migrationBuilder.Sql(@"DROP VIEW [Cbv].[CurrentMasterdata];");
        }
    }
}
