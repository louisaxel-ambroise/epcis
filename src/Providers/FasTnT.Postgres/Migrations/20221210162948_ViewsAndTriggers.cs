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
            migrationBuilder.Sql(@"CREATE VIEW cbv.current_masterdata
AS
SELECT MAX(request_id) AS request_id, type, id FROM cbv.masterdata GROUP BY type, id;");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION subscriptions.subscription_initial_requests()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    BEGIN
        INSERT INTO subscriptions.pending_request SELECT id AS request_id, NEW.id AS subscription_id FROM epcis.request WHERE capture_time >= NEW.initial_record_time;
        RETURN NULL;
    END;
$BODY$;
CREATE TRIGGER subscription_initial_requests AFTER INSERT ON subscriptions.subscription
FOR EACH ROW EXECUTE PROCEDURE subscriptions.subscription_initial_requests();");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION subscriptions.insert_pending_requests()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    BEGIN
        INSERT INTO subscriptions.pending_request SELECT NEW.id AS request_id, id AS subscription_id FROM subscriptions.subscription;
        RETURN NULL;
    END;
$BODY$;
CREATE TRIGGER insert_pending_requests AFTER INSERT ON epcis.request
FOR EACH ROW EXECUTE PROCEDURE subscriptions.insert_pending_requests();");

            migrationBuilder.Sql(@"CREATE VIEW cbv.masterdata_hierarchy
AS
WITH RECURSIVE rec AS (
	SELECT md.id AS root, md.type, md.id
	FROM cbv.current_masterdata md
	UNION ALL
	SELECT rec.id, masterdata_type, children_id
	FROM cbv.masterdata_children
	JOIN rec ON masterdata_type = rec.type and masterdata_id = rec.id
    WHERE masterdata_request_id = (SELECT MAX(masterdata_request_id) FROM cbv.masterdata_children WHERE masterdata_type = rec.type AND masterdata_id = rec.id)
)
SELECT rec.root, rec.type, rec.id 
FROM rec;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW cbv.masterdata_hierarchy;");
            migrationBuilder.Sql(@"DROP TRIGGER insert_pending_requests ON epcis.request CASCADE;");
            migrationBuilder.Sql(@"DROP FUNCTION subscriptions.insert_pending_requests;");
            migrationBuilder.Sql(@"DROP TRIGGER subscription_initial_requests ON subscriptions.subscription CASCADE;");
            migrationBuilder.Sql(@"DROP FUNCTION subscriptions.subscription_initial_requests;");
            migrationBuilder.Sql(@"DROP VIEW cbv.current_masterdata;");
        }
    }
}
