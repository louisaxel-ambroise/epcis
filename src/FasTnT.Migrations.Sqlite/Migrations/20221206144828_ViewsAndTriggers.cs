using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Migrations.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class ViewsAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"CREATE VIEW [CurrentHierarchy]
AS
SELECT MAX([MasterDataRequestId]) AS [MasterDataRequestId], [MasterDataType], [MasterDataId], [ChildrenId] FROM [MasterDataChildren] GROUP BY [MasterDataType], [MasterDataId], [ChildrenId];");

            migrationBuilder.Sql(@"CREATE VIEW [CurrentMasterdata]
AS
SELECT MAX([RequestId]) AS [RequestId], [Type], [Id] FROM [MasterData] GROUP BY [Type], [Id];");

            migrationBuilder.Sql(@"CREATE VIEW [MasterdataHierarchy] AS
WITH hierarchy([root], [type], [id])
AS (
	SELECT [id], [type], [id]
	FROM [CurrentMasterdata]
	UNION ALL
	SELECT [hierarchy].[Id], [MasterDataType], [ChildrenId]
	FROM [CurrentHierarchy]
	JOIN [hierarchy] ON [MasterDataType] = [hierarchy].[type] and [MasterDataId] = [hierarchy].[id]
)
SELECT [root], [type], [id] 
FROM [hierarchy];");

            migrationBuilder.Sql(@"CREATE VIEW [MasterDataProperty]
AS
SELECT md.[Id], md.[Type], att.[Id] as [Attribute], att.[Value] from [CurrentMasterdata] md
JOIN [MasterDataAttribute] att on att.[MasterdataId] = md.[Id] and att.[MasterdataType] = md.[Type] AND att.[RequestId] = md.[RequestId];");

            migrationBuilder.Sql(@"CREATE TRIGGER [InsertPendingRequests] 
AFTER INSERT ON [Request] 
BEGIN 
	INSERT INTO [PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT NEW.[Id], s.[Id] 
	FROM [Subscription] s;
END; ");

            migrationBuilder.Sql(@"CREATE TRIGGER [SubscriptionInitialRequests] 
AFTER INSERT ON [Subscription]
BEGIN 
	INSERT INTO [PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT r.[Id], NEW.[Id] 
	FROM [Request] r
    WHERE NEW.[InitialRecordTime] IS NOT NULL AND r.[CaptureDate] >= NEW.[InitialRecordTime];
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
