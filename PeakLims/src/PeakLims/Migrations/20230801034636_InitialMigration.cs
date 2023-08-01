using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "containers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usedfor = table.Column<string>(name: "used_for", type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_containers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "healthcare_organizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_healthcare_organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "panels",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    panelcode = table.Column<string>(name: "panel_code", type: "text", nullable: true),
                    panelname = table.Column<string>(name: "panel_name", type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_panels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    firstname = table.Column<string>(name: "first_name", type: "text", nullable: true),
                    lastname = table.Column<string>(name: "last_name", type: "text", nullable: true),
                    dateofbirth = table.Column<DateOnly>(name: "date_of_birth", type: "date", nullable: true),
                    age = table.Column<int>(type: "integer", nullable: true),
                    sex = table.Column<string>(type: "text", nullable: true),
                    race = table.Column<string>(type: "text", nullable: true),
                    ethnicity = table.Column<string>(type: "text", nullable: true),
                    internalid = table.Column<string>(name: "internal_id", type: "text", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "text", nullable: true),
                    permission = table.Column<string>(type: "text", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    testcode = table.Column<string>(name: "test_code", type: "text", nullable: true),
                    testname = table.Column<string>(name: "test_name", type: "text", nullable: true),
                    methodology = table.Column<string>(type: "text", nullable: true),
                    platform = table.Column<string>(type: "text", nullable: true),
                    version = table.Column<int>(type: "integer", nullable: false),
                    turnaroundtime = table.Column<int>(name: "turn_around_time", type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    firstname = table.Column<string>(name: "first_name", type: "text", nullable: true),
                    lastname = table.Column<string>(name: "last_name", type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    username = table.Column<string>(type: "text", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "accessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    accessionnumber = table.Column<string>(name: "accession_number", type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    patientid = table.Column<Guid>(name: "patient_id", type: "uuid", nullable: true),
                    healthcareorganizationid = table.Column<Guid>(name: "healthcare_organization_id", type: "uuid", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_accessions_healthcare_organizations_healthcare_organization_id",
                        column: x => x.healthcareorganizationid,
                        principalTable: "healthcare_organizations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_accessions_patients_patient_id",
                        column: x => x.patientid,
                        principalTable: "patients",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "samples",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    samplenumber = table.Column<string>(name: "sample_number", type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric", nullable: true),
                    collectiondate = table.Column<DateOnly>(name: "collection_date", type: "date", nullable: true),
                    receiveddate = table.Column<DateOnly>(name: "received_date", type: "date", nullable: true),
                    collectionsite = table.Column<string>(name: "collection_site", type: "text", nullable: true),
                    parentsampleid = table.Column<Guid>(name: "parent_sample_id", type: "uuid", nullable: true),
                    containerid = table.Column<Guid>(name: "container_id", type: "uuid", nullable: true),
                    patientid = table.Column<Guid>(name: "patient_id", type: "uuid", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_samples", x => x.id);
                    table.ForeignKey(
                        name: "fk_samples_containers_container_id",
                        column: x => x.containerid,
                        principalTable: "containers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_samples_patients_patient_id",
                        column: x => x.patientid,
                        principalTable: "patients",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_samples_samples_parent_sample_id",
                        column: x => x.parentsampleid,
                        principalTable: "samples",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "panel_test",
                columns: table => new
                {
                    panelsid = table.Column<Guid>(name: "panels_id", type: "uuid", nullable: false),
                    testsid = table.Column<Guid>(name: "tests_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_panel_test", x => new { x.panelsid, x.testsid });
                    table.ForeignKey(
                        name: "fk_panel_test_panels_panels_id",
                        column: x => x.panelsid,
                        principalTable: "panels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_panel_test_tests_tests_id",
                        column: x => x.testsid,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: true),
                    role = table.Column<string>(type: "text", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.userid,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "accession_comments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: true),
                    accessionid = table.Column<Guid>(name: "accession_id", type: "uuid", nullable: true),
                    parentcommentid = table.Column<Guid>(name: "parent_comment_id", type: "uuid", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accession_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_accession_comments_accession_comments_parent_comment_id",
                        column: x => x.parentcommentid,
                        principalTable: "accession_comments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_accession_comments_accessions_accession_id",
                        column: x => x.accessionid,
                        principalTable: "accessions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "healthcare_organization_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    npi = table.Column<string>(type: "text", nullable: true),
                    accessionid = table.Column<Guid>(name: "accession_id", type: "uuid", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_healthcare_organization_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_healthcare_organization_contacts_accessions_accession_id",
                        column: x => x.accessionid,
                        principalTable: "accessions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "test_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: true),
                    duedate = table.Column<DateOnly>(name: "due_date", type: "date", nullable: true),
                    tatsnapshot = table.Column<int>(name: "tat_snapshot", type: "integer", nullable: true),
                    cancellationreason = table.Column<string>(name: "cancellation_reason", type: "text", nullable: true),
                    cancellationcomments = table.Column<string>(name: "cancellation_comments", type: "text", nullable: true),
                    associatedpanelid = table.Column<Guid>(name: "associated_panel_id", type: "uuid", nullable: true),
                    sampleid = table.Column<Guid>(name: "sample_id", type: "uuid", nullable: true),
                    accessionid = table.Column<Guid>(name: "accession_id", type: "uuid", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_test_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_test_orders_accessions_accession_id",
                        column: x => x.accessionid,
                        principalTable: "accessions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_test_orders_samples_sample_id",
                        column: x => x.sampleid,
                        principalTable: "samples",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "healthcare_organization_healthcare_organization_contact",
                columns: table => new
                {
                    healthcareorganizationcontactsid = table.Column<Guid>(name: "healthcare_organization_contacts_id", type: "uuid", nullable: false),
                    healthcareorganizationsid = table.Column<Guid>(name: "healthcare_organizations_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_healthcare_organization_healthcare_organization_contact", x => new { x.healthcareorganizationcontactsid, x.healthcareorganizationsid });
                    table.ForeignKey(
                        name: "fk_healthcare_organization_healthcare_organization_contact_hea",
                        column: x => x.healthcareorganizationcontactsid,
                        principalTable: "healthcare_organization_contacts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_healthcare_organization_healthcare_organization_contact_hea1",
                        column: x => x.healthcareorganizationsid,
                        principalTable: "healthcare_organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accession_comments_accession_id",
                table: "accession_comments",
                column: "accession_id");

            migrationBuilder.CreateIndex(
                name: "ix_accession_comments_parent_comment_id",
                table: "accession_comments",
                column: "parent_comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_accessions_healthcare_organization_id",
                table: "accessions",
                column: "healthcare_organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_accessions_patient_id",
                table: "accessions",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_healthcare_organization_contacts_accession_id",
                table: "healthcare_organization_contacts",
                column: "accession_id");

            migrationBuilder.CreateIndex(
                name: "ix_healthcare_organization_healthcare_organization_contact_hea",
                table: "healthcare_organization_healthcare_organization_contact",
                column: "healthcare_organizations_id");

            migrationBuilder.CreateIndex(
                name: "ix_panel_test_tests_id",
                table: "panel_test",
                column: "tests_id");

            migrationBuilder.CreateIndex(
                name: "ix_samples_container_id",
                table: "samples",
                column: "container_id");

            migrationBuilder.CreateIndex(
                name: "ix_samples_parent_sample_id",
                table: "samples",
                column: "parent_sample_id");

            migrationBuilder.CreateIndex(
                name: "ix_samples_patient_id",
                table: "samples",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_orders_accession_id",
                table: "test_orders",
                column: "accession_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_orders_sample_id",
                table: "test_orders",
                column: "sample_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id",
                table: "user_roles",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accession_comments");

            migrationBuilder.DropTable(
                name: "healthcare_organization_healthcare_organization_contact");

            migrationBuilder.DropTable(
                name: "panel_test");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "test_orders");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "healthcare_organization_contacts");

            migrationBuilder.DropTable(
                name: "panels");

            migrationBuilder.DropTable(
                name: "tests");

            migrationBuilder.DropTable(
                name: "samples");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "accessions");

            migrationBuilder.DropTable(
                name: "containers");

            migrationBuilder.DropTable(
                name: "healthcare_organizations");

            migrationBuilder.DropTable(
                name: "patients");
        }
    }
}
