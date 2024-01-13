using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_accessions_healthcare_organizations_healthcare_organization_id",
                table: "accessions");

            migrationBuilder.CreateTable(
                name: "hipaa_audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    concept = table.Column<string>(type: "text", nullable: true),
                    data = table.Column<string>(type: "jsonb", nullable: true),
                    action_by = table.Column<string>(type: "text", nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    identifier = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hipaa_audit_logs", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_accessions_healthcare_organizations_healthcare_organization",
                table: "accessions",
                column: "healthcare_organization_id",
                principalTable: "healthcare_organizations",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_accessions_healthcare_organizations_healthcare_organization",
                table: "accessions");

            migrationBuilder.DropTable(
                name: "hipaa_audit_logs");

            migrationBuilder.AddForeignKey(
                name: "fk_accessions_healthcare_organizations_healthcare_organization_id",
                table: "accessions",
                column: "healthcare_organization_id",
                principalTable: "healthcare_organizations",
                principalColumn: "id");
        }
    }
}
