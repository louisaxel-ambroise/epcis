using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Migrations.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class ViewsAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"CREATE VIEW ""Cbv"".""CurrentHierarchy""
AS
SELECT MAX(""MasterDataRequestId"") AS ""MasterDataRequestId"", ""MasterDataType"", ""MasterDataId"", ""ChildrenId"" FROM ""Cbv"".""MasterDataChildren"" GROUP BY ""MasterDataType"", ""MasterDataId"", ""ChildrenId"";");
            
            migrationBuilder.Sql(@"CREATE VIEW ""Cbv"".""CurrentMasterdata""
AS
SELECT MAX(""RequestId"") AS ""RequestId"", ""Type"", ""Id"" FROM ""Cbv"".""MasterData"" GROUP BY ""Type"", ""Id"";");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION ""Subscription"".""SubscriptionInitialRequests""()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    DECLARE
        c_requests CURSOR FOR SELECT * FROM ""Epcis"".""Request"" WHERE ""CaptureDate"" >= NEW.""InitialRecordTime"";
        r_request ""Epcis"".""Request""%ROWTYPE;
    BEGIN
        FOR r_request IN c_requests LOOP
            INSERT INTO ""Subscription"".""PendingRequest"" (""RequestId"", ""SubscriptionId"") VALUES (r_request.""Id"", NEW.""Id"");
        END LOOP;

        RETURN NULL;
    END;
$BODY$;
DROP TRIGGER IF EXISTS ""SubscriptionInitialRequests"" ON ""Subscription"".""Subscription"" CASCADE;
CREATE TRIGGER ""SubscriptionInitialRequests"" AFTER INSERT ON ""Subscription"".""Subscription""
FOR EACH ROW EXECUTE PROCEDURE ""Subscription"".""SubscriptionInitialRequests""();");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION ""Subscription"".""InsertPendingRequests""()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    DECLARE
        c_subscription CURSOR FOR SELECT * FROM ""Subscription"".""Subscription"";
        r_request ""Epcis"".""Request""%ROWTYPE;
    BEGIN
        FOR r_subscription IN c_subscription LOOP
            INSERT INTO ""Subscription"".""PendingRequest"" (""RequestId"", ""SubscriptionId"") VALUES (NEW.""Id"", r_subscription.""Id"");
        END LOOP;

        RETURN NULL;
    END;
$BODY$;
DROP TRIGGER IF EXISTS ""InsertPendingRequests"" ON ""Epcis"".""Request"" CASCADE;
CREATE TRIGGER ""InsertPendingRequests"" AFTER INSERT ON ""Epcis"".""Request""
FOR EACH ROW EXECUTE PROCEDURE ""Subscription"".""InsertPendingRequests""();");

            migrationBuilder.Sql(@"CREATE VIEW ""Cbv"".""MasterDataProperty""
AS
SELECT md.""Id"", md.""Type"", att.""Id"" as ""Attribute"", att.""Value"" from ""Cbv"".""CurrentMasterdata"" md
JOIN ""Cbv"".""MasterDataAttribute"" att ON att.""MasterdataId"" = md.""Id"" AND att.""MasterdataType"" = md.""Type"" AND att.""RequestId"" = md.""RequestId"";");

            migrationBuilder.Sql(@"CREATE VIEW ""Cbv"".""MasterDataHierarchy""
AS
WITH RECURSIVE rec AS (
	SELECT md.""Id"" AS ""Root"", md.""Type"", md.""Id""
	FROM ""Cbv"".""CurrentMasterdata"" md
	UNION ALL
	SELECT rec.""Id"", ""MasterDataType"", ""ChildrenId""
	FROM ""Cbv"".""CurrentHierarchy""
	JOIN rec ON ""MasterDataType"" = rec.""Type"" and ""MasterDataId"" = rec.""Id""
)
SELECT rec.""Root"", rec.""Type"", rec.""Id"" 
FROM rec;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW ""Cbv"".""MasterDataHierarchy"";");
            migrationBuilder.Sql(@"DROP VIEW ""Cbv"".""MasterDataProperty"";");
            migrationBuilder.Sql(@"DROP FUNCTION ""Subscription"".""InsertPendingRequests"";");
            migrationBuilder.Sql(@"DROP FUNCTION ""Subscription"".""SubscriptionInitialRequests"";");
            migrationBuilder.Sql(@"DROP VIEW ""Cbv"".""CurrentMasterdata"";");
            migrationBuilder.Sql(@"DROP VIEW ""Cbv"".""CurrentHierarchy"";");

        }
    }
}
