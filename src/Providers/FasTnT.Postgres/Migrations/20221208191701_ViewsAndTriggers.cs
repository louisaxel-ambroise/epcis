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
            migrationBuilder.Sql(@"CREATE VIEW cbv.current_hierarchy
AS
SELECT MAX(masterdata_request_id) AS masterdata_request_id, masterdata_type, masterdata_id, children_id FROM cbv.masterdata_children GROUP BY masterdata_type, masterdata_id, children_id;");

            migrationBuilder.Sql(@"CREATE VIEW cbv.current_masterdata
AS
SELECT MAX(request_id) AS request_id, type, id FROM cbv.masterdata GROUP BY type, id;");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION subscriptions.subscription_initial_requests()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    DECLARE
        c_requests CURSOR FOR SELECT * FROM epcis.request WHERE capture_date >= NEW.intial_record_time;
        r_request epcis.request%ROWTYPE;
    BEGIN
        FOR r_request IN c_requests LOOP
            INSERT INTO subscription.pending_request (request_id, subscription_id) VALUES (r_request.id, NEW.id);
        END LOOP;

        RETURN NULL;
    END;
$BODY$;
DROP TRIGGER IF EXISTS subscription_initial_requests ON subscriptions.subscription CASCADE;
CREATE TRIGGER subscription_initial_requests AFTER INSERT ON subscriptions.subscription
FOR EACH ROW EXECUTE PROCEDURE subscriptions.subscription_initial_requests();");

            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION subscriptions.insert_pending_requests()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF 
AS $BODY$
    DECLARE
        c_subscription CURSOR FOR SELECT * FROM subscriptions.subscription;
        r_request epcis.request%ROWTYPE;
    BEGIN
        FOR r_subscription IN c_subscription LOOP
            INSERT INTO subscriptions.pending_request (request_id, subscription_id) VALUES (NEW.Id, r_subscription.id);
        END LOOP;

        RETURN NULL;
    END;
$BODY$;
DROP TRIGGER IF EXISTS insert_pending_requests ON epcis.request CASCADE;
CREATE TRIGGER insert_pending_requests AFTER INSERT ON epcis.request
FOR EACH ROW EXECUTE PROCEDURE subscriptions.insert_pending_requests();");

            migrationBuilder.Sql(@"CREATE VIEW cbv.masterdata_property
AS
SELECT md.id, md.type, att.id as attribute, att.value from cbv.current_masterdata md
JOIN cbv.masterdata_attribute att ON att.masterdata_id = md.id AND att.masterdata_type = md.type AND att.request_id = md.request_id;");

            migrationBuilder.Sql(@"CREATE VIEW cbv.masterdata_hierarchy
AS
WITH RECURSIVE rec AS (
	SELECT md.id AS root, md.type, md.id
	FROM cbv.current_masterdata md
	UNION ALL
	SELECT rec.id, masterdata_type, children_id
	FROM cbv.current_hierarchy
	JOIN rec ON masterdata_type = rec.type and masterdata_id = rec.id
)
SELECT rec.root, rec.type, rec.id 
FROM rec;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
