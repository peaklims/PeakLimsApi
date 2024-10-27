using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class RemoveContainerStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "containers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "containers",
                type: "text",
                nullable: true);
        }
    }
}
