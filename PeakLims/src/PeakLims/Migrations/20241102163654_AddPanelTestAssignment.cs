using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class AddPanelTestAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "panel_test");

            migrationBuilder.CreateTable(
                name: "panel_test_assignment",
                columns: table => new
                {
                    test_id = table.Column<Guid>(type: "uuid", nullable: false),
                    panel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    test_count = table.Column<int>(type: "integer", nullable: false),
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_panel_test_assignment", x => new { x.panel_id, x.test_id });
                    table.ForeignKey(
                        name: "fk_panel_test_assignment_panels_panel_id",
                        column: x => x.panel_id,
                        principalTable: "panels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_panel_test_assignment_tests_test_id",
                        column: x => x.test_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_panel_test_assignment_test_id",
                table: "panel_test_assignment",
                column: "test_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "panel_test_assignment");

            migrationBuilder.CreateTable(
                name: "panel_test",
                columns: table => new
                {
                    panels_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tests_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_panel_test", x => new { x.panels_id, x.tests_id });
                    table.ForeignKey(
                        name: "fk_panel_test_panels_panels_id",
                        column: x => x.panels_id,
                        principalTable: "panels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_panel_test_tests_tests_id",
                        column: x => x.tests_id,
                        principalTable: "tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_panel_test_tests_id",
                table: "panel_test",
                column: "tests_id");
        }
    }
}
