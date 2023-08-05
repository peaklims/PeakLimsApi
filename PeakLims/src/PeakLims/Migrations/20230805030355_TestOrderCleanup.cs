using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class TestOrderCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "test_id",
                table: "test_orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_test_orders_associated_panel_id",
                table: "test_orders",
                column: "associated_panel_id");

            migrationBuilder.CreateIndex(
                name: "ix_test_orders_test_id",
                table: "test_orders",
                column: "test_id");

            migrationBuilder.AddForeignKey(
                name: "fk_test_orders_panels_associated_panel_id",
                table: "test_orders",
                column: "associated_panel_id",
                principalTable: "panels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_test_orders_tests_test_id",
                table: "test_orders",
                column: "test_id",
                principalTable: "tests",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_test_orders_panels_associated_panel_id",
                table: "test_orders");

            migrationBuilder.DropForeignKey(
                name: "fk_test_orders_tests_test_id",
                table: "test_orders");

            migrationBuilder.DropIndex(
                name: "ix_test_orders_associated_panel_id",
                table: "test_orders");

            migrationBuilder.DropIndex(
                name: "ix_test_orders_test_id",
                table: "test_orders");

            migrationBuilder.DropColumn(
                name: "test_id",
                table: "test_orders");
        }
    }
}
