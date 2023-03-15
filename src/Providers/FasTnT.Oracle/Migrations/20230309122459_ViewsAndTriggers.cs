using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Oracle.Migrations
{
    /// <inheritdoc />
    public partial class ViewsAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE VIEW ""Cbv"".""CurrentMasterdata""
AS
SELECT MAX(""RequestId"") AS ""RequestId"", ""Type"", ""Id"" FROM ""Cbv"".""MasterData"" GROUP BY ""Type"", ""Id"";");

            migrationBuilder.Sql(@"CREATE TRIGGER  ""Subscriptions"".""SubscriptionInitialRequests""
ON ""Subscriptions"".""Subscription""
AFTER INSERT 
AS 
    INSERT INTO ""Subscriptions"".""PendingRequest"" SELECT ""Id"" AS ""RequestId"", :new.""Id"" AS ""SubscriptionId"" 
    FROM ""Rpcis"".""Request"" 
    WHERE :new.""InitialRecordTime"" IS NOT NULL AND ""CaptureTime"" >= :new.""InitialRecordTime"";");

            migrationBuilder.Sql(@"CREATE TRIGGER  ""Epcis"".""InsertPendingRequests""
ON ""Epcis"".""Request""
AFTER INSERT 
AS 
    INSERT INTO ""Subscriptions"".""PendingRequest"" SELECT :new.""Id"" AS ""RequestId"", ""Id"" AS ""SubscriptionId"" 
    FROM ""Subscriptions"".""Subscription"";");

            migrationBuilder.Sql(@"CREATE VIEW ""Cbv"".""MasterDataHierarchy""
AS
WITH RECURSIVE rec AS (
	SELECT md.""Id"" AS root, md.""Type"" as type, md.""Id"" AS id
	FROM ""Cbv"".""CurrentMasterdata"" md
	UNION ALL
	SELECT rec.id as root, ""MasterDataType"" AS type, ""ChildrenId"" AS id
	FROM ""Cbv"".""MasterDataChildren""
	JOIN rec ON ""MasterDataType"" = rec.type and ""MasterDataId"" = rec.id
    WHERE ""MasterDataRequestId"" = (SELECT MAX(""MasterDataRequestId"") FROM ""Cbv"".""MasterDataChildren"" WHERE ""MasterDataType"" = rec.type AND ""MasterDataId"" = rec.id)
)
SELECT rec.root AS ""Root"", rec.type AS ""Type"", rec.id AS ""Id""
FROM rec; ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW ""Cbv"".""MasterDataHierarchy"";");
            migrationBuilder.Sql(@"DROP TRIGGER ""Subscriptions"".""SubscriptionInitialRequests"";");
            migrationBuilder.Sql(@"DROP TRIGGER ""Epcis"".""InsertPendingRequests"";");
            migrationBuilder.Sql(@"DROP VIEW ""Cbv"".""CurrentMasterdata"";");
        }
    }
}
