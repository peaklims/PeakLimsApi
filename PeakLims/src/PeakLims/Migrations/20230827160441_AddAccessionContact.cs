using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessionContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_healthcare_organization_contacts_accessions_accession_id",
                table: "healthcare_organization_contacts");

            migrationBuilder.DropIndex(
                name: "ix_healthcare_organization_contacts_accession_id",
                table: "healthcare_organization_contacts");

            migrationBuilder.DropColumn(
                name: "accession_id",
                table: "healthcare_organization_contacts");

            migrationBuilder.CreateTable(
                name: "accession_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    targettype = table.Column<string>(name: "target_type", type: "text", nullable: true),
                    targetvalue = table.Column<string>(name: "target_value", type: "text", nullable: true),
                    accessionid = table.Column<Guid>(name: "accession_id", type: "uuid", nullable: true),
                    healthcareorganizationcontactid = table.Column<Guid>(name: "healthcare_organization_contact_id", type: "uuid", nullable: true),
                    createdon = table.Column<DateTime>(name: "created_on", type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<string>(name: "created_by", type: "text", nullable: true),
                    lastmodifiedon = table.Column<DateTime>(name: "last_modified_on", type: "timestamp with time zone", nullable: true),
                    lastmodifiedby = table.Column<string>(name: "last_modified_by", type: "text", nullable: true),
                    isdeleted = table.Column<bool>(name: "is_deleted", type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accession_contacts", x => x.id);
                    table.ForeignKey(
                        name: "fk_accession_contacts_accessions_accession_id",
                        column: x => x.accessionid,
                        principalTable: "accessions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_accession_contacts_healthcare_organization_contacts_healthc",
                        column: x => x.healthcareorganizationcontactid,
                        principalTable: "healthcare_organization_contacts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_accession_contacts_accession_id",
                table: "accession_contacts",
                column: "accession_id");

            migrationBuilder.CreateIndex(
                name: "ix_accession_contacts_healthcare_organization_contact_id",
                table: "accession_contacts",
                column: "healthcare_organization_contact_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accession_contacts");

            migrationBuilder.AddColumn<Guid>(
                name: "accession_id",
                table: "healthcare_organization_contacts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_healthcare_organization_contacts_accession_id",
                table: "healthcare_organization_contacts",
                column: "accession_id");

            migrationBuilder.AddForeignKey(
                name: "fk_healthcare_organization_contacts_accessions_accession_id",
                table: "healthcare_organization_contacts",
                column: "accession_id",
                principalTable: "accessions",
                principalColumn: "id");
        }
    }
}
