using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessionAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accession_attachments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: true),
                    s3bucket = table.Column<string>(type: "text", nullable: true),
                    s3key = table.Column<string>(type: "text", nullable: true),
                    filename = table.Column<string>(type: "text", nullable: true),
                    comments = table.Column<string>(type: "text", nullable: true),
                    accession_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accession_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_accession_attachments_accessions_accession_id",
                        column: x => x.accession_id,
                        principalTable: "accessions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_accession_attachments_accession_id",
                table: "accession_attachments",
                column: "accession_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accession_attachments");
        }
    }
}
