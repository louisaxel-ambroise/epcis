using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FasTnT.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "epcis");

            migrationBuilder.EnsureSchema(
                name: "sbdh");

            migrationBuilder.EnsureSchema(
                name: "cbv");

            migrationBuilder.EnsureSchema(
                name: "subscriptions");

            migrationBuilder.EnsureSchema(
                name: "queries");

            migrationBuilder.CreateTable(
                name: "pending_request",
                schema: "subscriptions",
                columns: table => new
                {
                    requestid = table.Column<int>(name: "request_id", type: "integer", nullable: false),
                    subscriptionid = table.Column<int>(name: "subscription_id", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pending_request", x => new { x.subscriptionid, x.requestid });
                });

            migrationBuilder.CreateTable(
                name: "request",
                schema: "epcis",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaptureId = table.Column<string>(type: "text", nullable: true),
                    userid = table.Column<string>(name: "user_id", type: "character varying(50)", maxLength: 50, nullable: true),
                    capturedate = table.Column<DateTimeOffset>(name: "capture_date", type: "timestamp with time zone", nullable: false),
                    documenttime = table.Column<DateTimeOffset>(name: "document_time", type: "timestamp with time zone", nullable: false),
                    schemaversion = table.Column<string>(name: "schema_version", type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stored_query",
                schema: "queries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    userid = table.Column<string>(name: "user_id", type: "character varying(80)", maxLength: 80, nullable: true),
                    datasource = table.Column<string>(name: "data_source", type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stored_query", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "event",
                schema: "epcis",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    requestid = table.Column<int>(name: "request_id", type: "integer", nullable: true),
                    CaptureTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    eventtime = table.Column<DateTimeOffset>(name: "event_time", type: "timestamp with time zone", nullable: false),
                    eventtimezoneoffset = table.Column<short>(name: "event_timezone_offset", type: "smallint", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    action = table.Column<short>(type: "smallint", nullable: false),
                    userid = table.Column<string>(name: "user_id", type: "character varying(36)", maxLength: 36, nullable: true),
                    eventid = table.Column<string>(name: "event_id", type: "character varying(256)", maxLength: 256, nullable: true),
                    certificationinfo = table.Column<string>(name: "certification_info", type: "character varying(256)", maxLength: 256, nullable: true),
                    readpoint = table.Column<string>(name: "read_point", type: "character varying(256)", maxLength: 256, nullable: true),
                    businesslocation = table.Column<string>(name: "business_location", type: "character varying(256)", maxLength: 256, nullable: true),
                    businessstep = table.Column<string>(name: "business_step", type: "character varying(256)", maxLength: 256, nullable: true),
                    disposition = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    transformationid = table.Column<string>(name: "transformation_id", type: "character varying(256)", maxLength: 256, nullable: true),
                    correctivedeclarationtime = table.Column<DateTimeOffset>(name: "corrective_declaration_time", type: "timestamp with time zone", nullable: true),
                    correctivereason = table.Column<string>(name: "corrective_reason", type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_request_request_id",
                        column: x => x.requestid,
                        principalSchema: "epcis",
                        principalTable: "request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "masterdata",
                schema: "cbv",
                columns: table => new
                {
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    requestid = table.Column<int>(name: "request_id", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_masterdata", x => new { x.requestid, x.type, x.id });
                    table.ForeignKey(
                        name: "FK_masterdata_request_request_id",
                        column: x => x.requestid,
                        principalSchema: "epcis",
                        principalTable: "request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "standard_business_header",
                schema: "sbdh",
                columns: table => new
                {
                    requestid = table.Column<int>(name: "request_id", type: "integer", nullable: false),
                    version = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    standard = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    typeversion = table.Column<string>(name: "type_version", type: "character varying(256)", maxLength: 256, nullable: false),
                    instanceidentifier = table.Column<string>(name: "instance_identifier", type: "character varying(256)", maxLength: 256, nullable: false),
                    type = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    creationdatetime = table.Column<DateTimeOffset>(name: "creation_date_time", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_standard_business_header", x => x.requestid);
                    table.ForeignKey(
                        name: "FK_standard_business_header_request_request_id",
                        column: x => x.requestid,
                        principalSchema: "epcis",
                        principalTable: "request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_callback",
                schema: "epcis",
                columns: table => new
                {
                    requestid = table.Column<int>(name: "request_id", type: "integer", nullable: false),
                    subscriptionid = table.Column<string>(name: "subscription_id", type: "character varying(256)", maxLength: 256, nullable: false),
                    callbacktype = table.Column<short>(name: "callback_type", type: "smallint", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_callback", x => x.requestid);
                    table.ForeignKey(
                        name: "FK_subscription_callback_request_request_id",
                        column: x => x.requestid,
                        principalSchema: "epcis",
                        principalTable: "request",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stored_query_parameter",
                schema: "subscriptions",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    queryid = table.Column<int>(name: "query_id", type: "integer", nullable: false),
                    Queryid = table.Column<int>(type: "integer", nullable: false),
                    values = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stored_query_parameter", x => new { x.queryid, x.name });
                    table.ForeignKey(
                        name: "FK_stored_query_parameter_stored_query_Queryid",
                        column: x => x.Queryid,
                        principalSchema: "queries",
                        principalTable: "stored_query",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription",
                schema: "subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    queryname = table.Column<string>(name: "query_name", type: "character varying(256)", maxLength: 256, nullable: false),
                    signaturetoken = table.Column<string>(name: "signature_token", type: "character varying(256)", maxLength: 256, nullable: true),
                    formattername = table.Column<string>(name: "formatter_name", type: "character varying(30)", maxLength: 30, nullable: false),
                    trigger = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    reportifempty = table.Column<bool>(name: "report_if_empty", type: "boolean", nullable: false),
                    Destination = table.Column<string>(type: "text", nullable: true),
                    queryid = table.Column<int>(name: "query_id", type: "integer", nullable: true),
                    InitialRecordTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_stored_query_query_id",
                        column: x => x.queryid,
                        principalSchema: "queries",
                        principalTable: "stored_query",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "business_transaction",
                schema: "epcis",
                columns: table => new
                {
                    type = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_business_transaction", x => new { x.eventid, x.type, x.id });
                    table.ForeignKey(
                        name: "FK_business_transaction_event_event_id",
                        column: x => x.eventid,
                        principalSchema: "epcis",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "corrective_event_id",
                schema: "epcis",
                columns: table => new
                {
                    correctiveid = table.Column<string>(name: "corrective_id", type: "character varying(256)", maxLength: 256, nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_corrective_event_id", x => new { x.eventid, x.correctiveid });
                    table.ForeignKey(
                        name: "FK_corrective_event_id_event_event_id",
                        column: x => x.eventid,
                        principalSchema: "epcis",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "destination",
                schema: "epcis",
                columns: table => new
                {
                    type = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_destination", x => new { x.eventid, x.type, x.id });
                    table.ForeignKey(
                        name: "FK_destination_event_event_id",
                        column: x => x.eventid,
                        principalSchema: "epcis",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "epc",
                schema: "epcis",
                columns: table => new
                {
                    type = table.Column<short>(type: "smallint", nullable: false),
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false),
                    quantity = table.Column<float>(type: "real", nullable: true),
                    unitofmeasure = table.Column<string>(name: "unit_of_measure", type: "character varying(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_epc", x => new { x.eventid, x.type, x.id });
                    table.ForeignKey(
                        name: "FK_epc_event_event_id",
                        column: x => x.eventid,
                        principalSchema: "epcis",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "field",
                schema: "epcis",
                columns: table => new
                {
                    index = table.Column<int>(type: "integer", nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    @namespace = table.Column<string>(name: "namespace", type: "character varying(256)", maxLength: 256, nullable: true),
                    textvalue = table.Column<string>(name: "text_value", type: "text", nullable: true),
                    numericvalue = table.Column<double>(name: "numeric_value", type: "double precision", nullable: true),
                    datevalue = table.Column<DateTimeOffset>(name: "date_value", type: "timestamp with time zone", nullable: true),
                    EntityIndex = table.Column<int>(type: "integer", nullable: true),
                    ParentIndex = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_field", x => new { x.eventid, x.index });
                    table.ForeignKey(
                        name: "FK_field_event_event_id",
                        column: x => x.eventid,
                        principalSchema: "epcis",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persistent_disposition",
                schema: "epcis",
                columns: table => new
                {
                    type = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persistent_disposition", x => new { x.eventid, x.type, x.id });
                    table.ForeignKey(
                        name: "FK_persistent_disposition_event_event_id",
                        column: x => x.eventid,
                        principalSchema: "epcis",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sensor_element",
                schema: "epcis",
                columns: table => new
                {
                    index = table.Column<int>(type: "integer", nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false),
                    time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deviceid = table.Column<string>(name: "device_id", type: "text", nullable: true),
                    devicemetadata = table.Column<string>(name: "device_metadata", type: "text", nullable: true),
                    rawdata = table.Column<string>(name: "raw_data", type: "text", nullable: true),
                    starttime = table.Column<DateTimeOffset>(name: "start_time", type: "timestamp with time zone", nullable: true),
                    endtime = table.Column<DateTimeOffset>(name: "end_time", type: "timestamp with time zone", nullable: true),
                    dataprocessingmethod = table.Column<string>(name: "data_processing_method", type: "text", nullable: true),
                    bizrules = table.Column<string>(name: "biz_rules", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensor_element", x => new { x.eventid, x.index });
                    table.ForeignKey(
                        name: "FK_sensor_element_event_event_id",
                        column: x => x.eventid,
                        principalSchema: "epcis",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "source",
                schema: "epcis",
                columns: table => new
                {
                    type = table.Column<string>(type: "text", nullable: false),
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_source", x => new { x.eventid, x.type, x.id });
                    table.ForeignKey(
                        name: "FK_source_event_event_id",
                        column: x => x.eventid,
                        principalSchema: "epcis",
                        principalTable: "event",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "masterdata_children",
                schema: "cbv",
                columns: table => new
                {
                    childrenid = table.Column<string>(name: "children_id", type: "character varying(256)", maxLength: 256, nullable: false),
                    masterdatarequestid = table.Column<int>(name: "masterdata_request_id", type: "integer", nullable: false),
                    masterdatatype = table.Column<string>(name: "masterdata_type", type: "character varying(256)", maxLength: 256, nullable: false),
                    masterdataid = table.Column<string>(name: "masterdata_id", type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_masterdata_children", x => new { x.masterdatarequestid, x.masterdatatype, x.masterdataid, x.childrenid });
                    table.ForeignKey(
                        name: "FK_masterdata_children_masterdata_masterdata_request_id_master~",
                        columns: x => new { x.masterdatarequestid, x.masterdatatype, x.masterdataid },
                        principalSchema: "cbv",
                        principalTable: "masterdata",
                        principalColumns: new[] { "request_id", "type", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MasterDataAttribute",
                schema: "cbv",
                columns: table => new
                {
                    id = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    requestid = table.Column<int>(name: "request_id", type: "integer", nullable: false),
                    masterdatatype = table.Column<string>(name: "masterdata_type", type: "character varying(256)", maxLength: 256, nullable: false),
                    masterdataid = table.Column<string>(name: "masterdata_id", type: "character varying(256)", maxLength: 256, nullable: false),
                    value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterDataAttribute", x => new { x.requestid, x.masterdatatype, x.masterdataid, x.id });
                    table.ForeignKey(
                        name: "FK_MasterDataAttribute_masterdata_request_id_masterdata_type_m~",
                        columns: x => new { x.requestid, x.masterdatatype, x.masterdataid },
                        principalSchema: "cbv",
                        principalTable: "masterdata",
                        principalColumns: new[] { "request_id", "type", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_information",
                schema: "sbdh",
                columns: table => new
                {
                    type = table.Column<short>(type: "smallint", maxLength: 256, nullable: false),
                    identifier = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    requestid = table.Column<int>(name: "request_id", type: "integer", nullable: false),
                    contact = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    emailaddress = table.Column<string>(name: "email_address", type: "character varying(256)", maxLength: 256, nullable: true),
                    faxnumber = table.Column<string>(name: "fax_number", type: "character varying(256)", maxLength: 256, nullable: true),
                    telephonenumber = table.Column<string>(name: "telephone_number", type: "text", nullable: true),
                    contacttypeidentifier = table.Column<string>(name: "contact_type_identifier", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_information", x => new { x.requestid, x.type, x.identifier });
                    table.ForeignKey(
                        name: "FK_contact_information_standard_business_header_request_id",
                        column: x => x.requestid,
                        principalSchema: "sbdh",
                        principalTable: "standard_business_header",
                        principalColumn: "request_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_execution_record",
                schema: "subscriptions",
                columns: table => new
                {
                    executiontime = table.Column<DateTimeOffset>(name: "execution_time", type: "timestamp with time zone", nullable: false),
                    subscriptionid = table.Column<int>(name: "subscription_id", type: "integer", nullable: false),
                    SubscriptionId = table.Column<int>(type: "integer", nullable: false),
                    successful = table.Column<bool>(type: "boolean", nullable: false),
                    resultssent = table.Column<bool>(name: "results_sent", type: "boolean", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_execution_record", x => new { x.subscriptionid, x.executiontime });
                    table.ForeignKey(
                        name: "FK_subscription_execution_record_subscription_subscription_id",
                        column: x => x.subscriptionid,
                        principalSchema: "subscriptions",
                        principalTable: "subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_parameter",
                schema: "subscriptions",
                columns: table => new
                {
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    subscriptionid = table.Column<int>(name: "subscription_id", type: "integer", nullable: false),
                    SubscriptionId = table.Column<int>(type: "integer", nullable: false),
                    values = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_parameter", x => new { x.subscriptionid, x.name });
                    table.ForeignKey(
                        name: "FK_subscription_parameter_subscription_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "subscriptions",
                        principalTable: "subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subscription_schedule",
                schema: "subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subscriptionid = table.Column<int>(name: "subscription_id", type: "integer", nullable: false),
                    second = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    minute = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    hour = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    dayofmonth = table.Column<string>(name: "day_of_month", type: "character varying(256)", maxLength: 256, nullable: true),
                    month = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    dayofweek = table.Column<string>(name: "day_of_week", type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_schedule", x => x.id);
                    table.ForeignKey(
                        name: "FK_subscription_schedule_subscription_subscription_id",
                        column: x => x.subscriptionid,
                        principalSchema: "subscriptions",
                        principalTable: "subscription",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sensor_report",
                schema: "epcis",
                columns: table => new
                {
                    index = table.Column<int>(type: "integer", nullable: false),
                    eventid = table.Column<int>(name: "event_id", type: "integer", nullable: false),
                    sensorindex = table.Column<int>(name: "sensor_index", type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: true),
                    deviceid = table.Column<string>(name: "device_id", type: "text", nullable: true),
                    rawdata = table.Column<string>(name: "raw_data", type: "text", nullable: true),
                    dataprocessingmethod = table.Column<string>(name: "data_processing_method", type: "text", nullable: true),
                    time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    microorganism = table.Column<string>(type: "text", nullable: true),
                    chemicalsubstance = table.Column<string>(name: "chemical_substance", type: "text", nullable: true),
                    value = table.Column<float>(type: "real", nullable: true),
                    component = table.Column<string>(type: "text", nullable: true),
                    stringvalue = table.Column<string>(name: "string_value", type: "text", nullable: true),
                    booleanvalue = table.Column<bool>(name: "boolean_value", type: "boolean", nullable: false),
                    hexbinaryvalue = table.Column<string>(name: "hex_binary_value", type: "text", nullable: true),
                    urivalue = table.Column<string>(name: "uri_value", type: "text", nullable: true),
                    minvalue = table.Column<float>(name: "min_value", type: "real", nullable: true),
                    maxvalue = table.Column<float>(name: "max_value", type: "real", nullable: true),
                    meanvalue = table.Column<float>(name: "mean_value", type: "real", nullable: true),
                    percrank = table.Column<float>(name: "perc_rank", type: "real", nullable: true),
                    percvalue = table.Column<float>(name: "perc_value", type: "real", nullable: true),
                    unitofmeasure = table.Column<string>(name: "unit_of_measure", type: "text", nullable: true),
                    sdev = table.Column<float>(type: "real", nullable: true),
                    devicemetadata = table.Column<string>(name: "device_metadata", type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sensor_report", x => new { x.eventid, x.sensorindex, x.index });
                    table.ForeignKey(
                        name: "FK_sensor_report_sensor_element_event_id_sensor_index",
                        columns: x => new { x.eventid, x.sensorindex },
                        principalSchema: "epcis",
                        principalTable: "sensor_element",
                        principalColumns: new[] { "event_id", "index" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "masterdata_field",
                schema: "cbv",
                columns: table => new
                {
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    @namespace = table.Column<string>(name: "namespace", type: "character varying(256)", maxLength: 256, nullable: false),
                    requestid = table.Column<int>(name: "request_id", type: "integer", nullable: false),
                    masterdatatype = table.Column<string>(name: "masterdata_type", type: "character varying(256)", maxLength: 256, nullable: false),
                    masterdataid = table.Column<string>(name: "masterdata_id", type: "character varying(256)", maxLength: 256, nullable: false),
                    attributeid = table.Column<string>(name: "attribute_id", type: "character varying(256)", maxLength: 256, nullable: false),
                    parentname = table.Column<string>(name: "parent_name", type: "character varying(256)", maxLength: 256, nullable: true),
                    parentnamespace = table.Column<string>(name: "parent_namespace", type: "character varying(256)", maxLength: 256, nullable: true),
                    value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_masterdata_field", x => new { x.requestid, x.masterdatatype, x.masterdataid, x.attributeid, x.@namespace, x.name });
                    table.ForeignKey(
                        name: "FK_masterdata_field_MasterDataAttribute_request_id_masterdata_~",
                        columns: x => new { x.requestid, x.masterdatatype, x.masterdataid, x.attributeid },
                        principalSchema: "cbv",
                        principalTable: "MasterDataAttribute",
                        principalColumns: new[] { "request_id", "masterdata_type", "masterdata_id", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "queries",
                table: "stored_query",
                columns: new[] { "id", "data_source", "name", "user_id" },
                values: new object[,]
                {
                    { -2, "SimpleEventQuery", "SimpleEventQuery", null },
                    { -1, "SimpleMasterDataQuery", "SimpleMasterDataQuery", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_request_id",
                schema: "epcis",
                table: "event",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_stored_query_name",
                schema: "queries",
                table: "stored_query",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stored_query_parameter_Queryid",
                schema: "subscriptions",
                table: "stored_query_parameter",
                column: "Queryid");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_query_id",
                schema: "subscriptions",
                table: "subscription",
                column: "query_id");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_parameter_SubscriptionId",
                schema: "subscriptions",
                table: "subscription_parameter",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_subscription_schedule_subscription_id",
                schema: "subscriptions",
                table: "subscription_schedule",
                column: "subscription_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "business_transaction",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "contact_information",
                schema: "sbdh");

            migrationBuilder.DropTable(
                name: "corrective_event_id",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "destination",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "epc",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "field",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "masterdata_children",
                schema: "cbv");

            migrationBuilder.DropTable(
                name: "masterdata_field",
                schema: "cbv");

            migrationBuilder.DropTable(
                name: "pending_request",
                schema: "subscriptions");

            migrationBuilder.DropTable(
                name: "persistent_disposition",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "sensor_report",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "source",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "stored_query_parameter",
                schema: "subscriptions");

            migrationBuilder.DropTable(
                name: "subscription_callback",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "subscription_execution_record",
                schema: "subscriptions");

            migrationBuilder.DropTable(
                name: "subscription_parameter",
                schema: "subscriptions");

            migrationBuilder.DropTable(
                name: "subscription_schedule",
                schema: "subscriptions");

            migrationBuilder.DropTable(
                name: "standard_business_header",
                schema: "sbdh");

            migrationBuilder.DropTable(
                name: "MasterDataAttribute",
                schema: "cbv");

            migrationBuilder.DropTable(
                name: "sensor_element",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "subscription",
                schema: "subscriptions");

            migrationBuilder.DropTable(
                name: "masterdata",
                schema: "cbv");

            migrationBuilder.DropTable(
                name: "event",
                schema: "epcis");

            migrationBuilder.DropTable(
                name: "stored_query",
                schema: "queries");

            migrationBuilder.DropTable(
                name: "request",
                schema: "epcis");
        }
    }
}
