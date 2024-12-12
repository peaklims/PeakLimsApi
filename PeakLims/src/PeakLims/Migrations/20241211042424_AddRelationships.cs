using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class AddRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "patient_relationships",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_patient_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_relationship = table.Column<string>(type: "text", nullable: true),
                    to_relationship = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patient_relationships", x => x.id);
                    table.ForeignKey(
                        name: "fk_patient_relationships_patients_from_patient_id",
                        column: x => x.from_patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_patient_relationships_patients_to_patient_id",
                        column: x => x.to_patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_patient_relationships_from_patient_id",
                table: "patient_relationships",
                column: "from_patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_relationships_to_patient_id",
                table: "patient_relationships",
                column: "to_patient_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patient_relationships");
        }
    }
}
