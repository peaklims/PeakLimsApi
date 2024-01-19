using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class PanelOrdersOffOfAccession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "accession_id",
                table: "panel_orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_panel_orders_accession_id",
                table: "panel_orders",
                column: "accession_id");

            migrationBuilder.AddForeignKey(
                name: "fk_panel_orders_accessions_accession_id",
                table: "panel_orders",
                column: "accession_id",
                principalTable: "accessions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_panel_orders_accessions_accession_id",
                table: "panel_orders");

            migrationBuilder.DropIndex(
                name: "ix_panel_orders_accession_id",
                table: "panel_orders");

            migrationBuilder.DropColumn(
                name: "accession_id",
                table: "panel_orders");
        }
    }
}
