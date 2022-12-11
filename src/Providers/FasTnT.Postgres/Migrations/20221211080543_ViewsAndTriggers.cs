using FasTnT.Domain.Model.Masterdata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Postgres.Migrations
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

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION ""Subscriptions"".""SubscriptionInitialRequests""()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    BEGIN
        INSERT INTO ""Subscriptions"".""PendingRequest"" SELECT ""Id"" AS ""RequestId"", NEW.""Id"" AS ""SubscriptionId"" FROM ""Rpcis"".""Request"" WHERE ""CaptureTime"" >= NEW.""InitialRecordTime"";
        RETURN NULL;
    END;
$BODY$;
CREATE TRIGGER ""SubscriptionInitialRequests"" AFTER INSERT ON ""Subscriptions"".""Subscription""
FOR EACH ROW EXECUTE PROCEDURE ""Subscriptions"".""SubscriptionInitialRequests""();");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION ""Subscriptions"".""InsertPendingRequests""()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    BEGIN
        INSERT INTO ""Subscriptions"".""PendingRequest"" SELECT NEW.""Id"" AS ""RequestId"", ""Id"" AS ""SubscriptionId"" FROM ""Subscriptions"".""Subscription"";
        RETURN NULL;
    END;
$BODY$;
CREATE TRIGGER ""InsertPendingRequests"" AFTER INSERT ON ""Epcis"".""Request""
FOR EACH ROW EXECUTE PROCEDURE ""Subscriptions"".""InsertPendingRequests""();");

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
            migrationBuilder.Sql(@"DROP TRIGGER ""InsertPendingRequests"" ON ""Epcis"".""Request"" CASCADE;");
            migrationBuilder.Sql(@"DROP FUNCTION ""Subscriptions"".""InsertPendingRequests"";");
            migrationBuilder.Sql(@"DROP TRIGGER ""SubscriptionInitialRequests"" ON ""Subscriptions"".""Subscription"" CASCADE;");
            migrationBuilder.Sql(@"DROP FUNCTION ""Subscriptions"".""SubscriptionInitialRequests"";");
            migrationBuilder.Sql(@"DROP VIEW ""Cbv"".""CurrentMasterdata"";");
        }
    }
}
