using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class OrgContactHasSplitName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "healthcare_organization_contacts",
                newName: "last_name");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                table: "healthcare_organization_contacts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_name",
                table: "healthcare_organization_contacts");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "healthcare_organization_contacts",
                newName: "name");
        }
    }
}
