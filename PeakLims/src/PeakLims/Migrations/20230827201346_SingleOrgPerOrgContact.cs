using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class SingleOrgPerOrgContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "healthcare_organization_healthcare_organization_contact");

            migrationBuilder.AddColumn<Guid>(
                name: "healthcare_organization_id",
                table: "healthcare_organization_contacts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_healthcare_organization_contacts_healthcare_organization_id",
                table: "healthcare_organization_contacts",
                column: "healthcare_organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_healthcare_organization_contacts_healthcare_organizations_h",
                table: "healthcare_organization_contacts",
                column: "healthcare_organization_id",
                principalTable: "healthcare_organizations",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_healthcare_organization_contacts_healthcare_organizations_h",
                table: "healthcare_organization_contacts");

            migrationBuilder.DropIndex(
                name: "ix_healthcare_organization_contacts_healthcare_organization_id",
                table: "healthcare_organization_contacts");

            migrationBuilder.DropColumn(
                name: "healthcare_organization_id",
                table: "healthcare_organization_contacts");

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
                name: "ix_healthcare_organization_healthcare_organization_contact_hea",
                table: "healthcare_organization_healthcare_organization_contact",
                column: "healthcare_organizations_id");
        }
    }
}
