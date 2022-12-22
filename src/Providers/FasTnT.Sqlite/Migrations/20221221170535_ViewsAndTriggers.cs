using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class ViewsAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
	FROM [MasterdataChildren] children
	JOIN [CurrentMasterdata] cur ON cur.[Type] = children.[MasterDataType] AND cur.[Id] = [ChildrenId] 
	JOIN hierarchy ON [MasterDataType] = hierarchy.[type] AND [MasterDataId] = hierarchy.[id]
)
SELECT [root], [type], [id] 
FROM [hierarchy];");

            migrationBuilder.Sql(@"CREATE TRIGGER [InsertPendingRequests] 
AFTER INSERT ON [Request] 
BEGIN 
	INSERT INTO [PendingRequest]([RequestId], [SubscriptionId]) SELECT NEW.[Id], s.[Id] 
	FROM [Subscription] s;
END; ");

            migrationBuilder.Sql(@"CREATE TRIGGER [SubscriptionInitialRequests] 
AFTER INSERT ON [Subscription]
BEGIN 
	INSERT INTO [PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT r.[Id], NEW.[Id] 
	FROM [Request] r
    WHERE NEW.[InitialRecordTime] IS NOT NULL AND r.[CaptureTime] >= NEW.[InitialRecordTime];
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER [SubscriptionInitialRequests];");
            migrationBuilder.Sql(@"DROP TRIGGER [InsertPendingRequests];");
            migrationBuilder.Sql(@"DROP VIEW [MasterdataHierarchy];");
            migrationBuilder.Sql(@"DROP VIEW [CurrentMasterdata];");
        }
    }
}
