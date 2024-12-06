using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleToContactsAndRemoveOrgEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "healthcare_organizations");

            migrationBuilder.AddColumn<string>(
                name: "title",
                table: "healthcare_organization_contacts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "title",
                table: "healthcare_organization_contacts");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "healthcare_organizations",
                type: "text",
                nullable: true);
        }
    }
}
