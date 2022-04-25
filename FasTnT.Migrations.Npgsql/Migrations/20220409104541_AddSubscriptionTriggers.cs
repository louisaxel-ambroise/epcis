using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FasTnT.Migrations.Npgsql.Migrations
{
    public partial class AddSubscriptionTriggers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE FUNCTION ""Subscription"".update_subscriptions()
    RETURNS trigger
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE NOT LEAKPROOF
AS $BODY$
    BEGIN
        INSERT INTO ""Subscriptions"".""Pendingrequest""(""SubscriptionId"", ""RequestId"")
        SELECT s.id, NEW.id
        FROM ""Subscription"".""Subscription"" s;

        RETURN NULL;
    END;
$BODY$;");
            migrationBuilder.Sql(@"CREATE TRIGGER add_pending_request AFTER INSERT ON ""Epcis"".""Request""
FOR EACH ROW EXECUTE PROCEDURE ""Subscription"".update_subscriptions();");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS add_pending_request ON ""Epcis"".""Request"" CASCADE;");
            migrationBuilder.Sql(@"DROP FUNCTION ""Subscription"".update_subscriptions();");
        }
    }
}
