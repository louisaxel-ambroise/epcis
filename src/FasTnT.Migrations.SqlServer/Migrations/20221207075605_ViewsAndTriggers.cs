using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Migrations.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ViewsAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW [Cbv].[CurrentHierarchy]
AS
SELECT MAX([MasterDataRequestId]) AS [MasterDataRequestId], [MasterDataType], [MasterDataId], [ChildrenId] FROM [Cbv].[MasterDataChildren] GROUP BY [MasterDataType], [MasterDataId], [ChildrenId];");

            migrationBuilder.Sql(@"CREATE VIEW [Cbv].[CurrentMasterdata]
AS
SELECT MAX([RequestId]) AS [RequestId], [Type], [Id] FROM [Cbv].[MasterData] GROUP BY [Type], [Id];");

            migrationBuilder.Sql(@"CREATE VIEW [Cbv].[MasterdataHierarchy] AS
WITH hierarchy([root], [type], [id])
AS (
	SELECT [id], [type], [id]
	FROM [Cbv].[CurrentMasterdata]
	UNION ALL
	SELECT [hierarchy].[Id], [MasterDataType], [ChildrenId]
	FROM [Cbv].[CurrentHierarchy]
	JOIN [hierarchy] ON [MasterDataType] = [hierarchy].[type] and [MasterDataId] = [hierarchy].[id]
)
SELECT [root], [type], [id] 
FROM [hierarchy];");

            migrationBuilder.Sql(@"CREATE VIEW [Cbv].[MasterDataProperty]
AS
SELECT md.[Id], md.[Type], att.[Id] as [Attribute], att.[Value] from [Cbv].[CurrentMasterdata] md
JOIN [Cbv].[MasterDataAttribute] att on att.[MasterdataId] = md.[Id] and att.[MasterdataType] = md.[Type] AND att.[RequestId] = md.[RequestId];");

            migrationBuilder.Sql(@"CREATE TRIGGER [Epcis].[InsertPendingRequests] 
ON [Epcis].[Request] 
AFTER INSERT 
AS 
	INSERT INTO [Subscription].[PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT i.[Id], s.[Id] 
	FROM inserted i, [Subscription].[Subscription] s;");

            migrationBuilder.Sql(@"CREATE TRIGGER [Subscription].[SubscriptionInitialRequests] 
ON [Subscription].[Subscription] 
AFTER INSERT 
AS 
	INSERT INTO [Subscription].[PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT r.[Id], i.[Id] 
	FROM inserted i, [Epcis].[Request] r
    WHERE i.[InitialRecordTime] IS NOT NULL AND r.[CaptureDate] >= i.[InitialRecordTime];");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
