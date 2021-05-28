using Microsoft.EntityFrameworkCore.Migrations;

namespace FasTnT.Infrastructure.Migrations
{
    public partial class SplitSourceDestinationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SourceDestination",
                schema: "Epcis");

            migrationBuilder.CreateTable(
                name: "Destination",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destination", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_Destination_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Source",
                schema: "Epcis",
                columns: table => new
                {
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Source", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_Source_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Destination",
                schema: "Epcis");

            migrationBuilder.DropTable(
                name: "Source",
                schema: "Epcis");

            migrationBuilder.CreateTable(
                name: "SourceDestination",
                schema: "Epcis",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Direction = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SourceDestination", x => new { x.EventId, x.Type, x.Id });
                    table.ForeignKey(
                        name: "FK_SourceDestination_Event_EventId",
                        column: x => x.EventId,
                        principalSchema: "Epcis",
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
