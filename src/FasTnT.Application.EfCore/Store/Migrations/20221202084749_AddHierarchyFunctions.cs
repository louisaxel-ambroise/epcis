using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Application.Migrations
{
    public partial class AddHierarchyFunctions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW [Cbv].[CurrentMasterdata]
AS
SELECT MAX([RequestId]) AS [RequestId], [Type], [Id] FROM [Cbv].[MasterData] GROUP BY [Type], [Id];");

            migrationBuilder.Sql(@"CREATE VIEW [Cbv].[CurrentHierarchy]
AS
SELECT MAX([MasterDataRequestId]) AS [MasterDataRequestId], [MasterDataType], [MasterDataId], [ChildrenId] FROM [Cbv].[MasterDataChildren] GROUP BY [MasterDataType], [MasterDataId], [ChildrenId];");

            migrationBuilder.Sql(@"CREATE FUNCTION [Cbv].[MasterdataHierarchy](@parentid nvarchar(max), @type nvarchar(max))
RETURNS TABLE
AS
RETURN WITH hierarchy([root], [type], [id])
AS (
	SELECT [id], [type], [id]
	FROM [CurrentMasterdata]
	UNION ALL
	SELECT [hierarchy].[Id], [MasterDataType], [ChildrenId]
	FROM [CurrentHierarchy]
	JOIN [hierarchy] ON [MasterDataType] = [hierarchy].[type] and [MasterDataId] = [hierarchy].[id]
)
SELECT [type], [id] 
FROM [hierarchy]
WHERE [root] = @parentid and [type] = @type;");

            migrationBuilder.Sql(@"CREATE FUNCTION [Cbv].[MasterdataProperty](@id nvarchar(max), @type nvarchar(max), @field nvarchar(max))
RETURNS nvarchar(max)
AS
BEGIN
DECLARE @value nvarchar(max);
SELECT TOP(1) @value = att.[Value] 
FROM [Cbv].[MasterDataAttribute] att
JOIN [Cbv].[CurrentMasterdata] md ON att.RequestId = md.RequestId AND md.Type = att.MasterdataType AND md.Id = att.MasterdataId
WHERE md.[id] = @id AND md.[type] = @type AND att.Id = @field;
RETURN @value;
END;");

            migrationBuilder.Sql(@"DROP TRIGGER [Epcis].[InsertPendingRequests]"); 
            migrationBuilder.Sql(@"CREATE TRIGGER [InsertPendingRequests] 
ON [Epcis].[Request] 
AFTER INSERT 
AS 
	INSERT INTO [Subscription].[PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT i.[Id], s.[Id] 
	FROM inserted i, [Subscription].[Subscription] s;");

            migrationBuilder.Sql(@"DROP TRIGGER [Subscription].[SubscriptionInitialRequests];");
            migrationBuilder.Sql(@"CREATE TRIGGER [SubscriptionInitialRequests] 
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
            migrationBuilder.Sql("DROP FUNCTION [Cbv].[MasterdataHierarchy];");
            migrationBuilder.Sql("DROP FUNCTION [Cbv].[CurrentMasterdata];");
            migrationBuilder.Sql("DROP FUNCTION [Cbv].[CurrentHierarchy];");

            migrationBuilder.Sql(@"DROP TRIGGER [InsertPendingRequests]");
            migrationBuilder.Sql(@"CREATE TRIGGER [Epcis].[InsertPendingRequests] 
ON [Epcis].[Request] 
AFTER INSERT 
AS 
	INSERT INTO [Subscription].[PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT i.[Id], s.[Id] 
	FROM inserted i, [Subscription].[Subscription] s;");

            migrationBuilder.Sql(@"DROP TRIGGER [SubscriptionInitialRequests];");
            migrationBuilder.Sql(@"CREATE TRIGGER [Subscription].[SubscriptionInitialRequests] 
ON [Subscription].[Subscription] 
AFTER INSERT 
AS 
	INSERT INTO [Subscription].[PendingRequest]([RequestId], [SubscriptionId]) 
	SELECT r.[Id], i.[Id] 
	FROM inserted i, [Epcis].[Request] r
    WHERE i.[InitialRecordTime] IS NOT NULL AND r.[CaptureDate] >= i.[InitialRecordTime];");
        }
    }
}
