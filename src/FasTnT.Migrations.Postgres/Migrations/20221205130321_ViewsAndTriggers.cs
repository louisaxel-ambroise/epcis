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
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION ""Cbv"".""MasterdataProperty""(id character varying, type character varying, field character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    DECLARE
        value character varying;
    BEGIN
        SELECT att.""Value"" INTO value
		FROM ""Cbv"".""MasterDataAttribute"" att
		JOIN ""Cbv"".""CurrentMasterdata"" md ON att.""RequestId"" = md.""RequestId"" AND md.""Type"" = att.""MasterdataType"" AND md.""Id"" = att.""MasterdataId""
		WHERE md.""Id"" = id AND md.""Type"" = type AND att.""Id"" = field;
		
		RETURN value;
    END;
$BODY$;");
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION ""Cbv"".""MasterdataHierarchy""(parentid character varying, type character varying)
	RETURNS TABLE(""Type"" character varying, ""Id"" character varying)
	LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    BEGIN
	RETURN QUERY WITH RECURSIVE rec
	AS (
		SELECT md.""Id"" AS ""Root"", md.""Type"", md.""Id""
		FROM ""Cbv"".""CurrentMasterdata"" md
		UNION ALL
		SELECT rec.""Id"", ""MasterDataType"", ""ChildrenId""
		FROM ""Cbv"".""CurrentHierarchy""
		JOIN rec ON ""MasterDataType"" = rec.""Type"" and ""MasterDataId"" = rec.""Id""
	)
	SELECT rec.""Type"", rec.""Id"" 
	FROM rec
	WHERE ""Root"" = parentid and rec.""Type"" = type;
	
	END;
$BODY$;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
