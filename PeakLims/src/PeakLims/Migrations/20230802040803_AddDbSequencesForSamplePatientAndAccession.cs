using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeakLims.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSequencesForSamplePatientAndAccession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ACC",
                startValue: 10005702L);

            migrationBuilder.CreateSequence(
                name: "PAT",
                startValue: 10045702L);

            migrationBuilder.CreateSequence(
                name: "SAM",
                startValue: 10000202L);

            migrationBuilder.AlterColumn<string>(
                name: "sample_number",
                table: "samples",
                type: "text",
                nullable: false,
                defaultValueSql: "concat('SAM', nextval('\"SAM\"'))",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "internal_id",
                table: "patients",
                type: "text",
                nullable: false,
                defaultValueSql: "concat('PAT', nextval('\"PAT\"'))",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "accession_number",
                table: "accessions",
                type: "text",
                nullable: false,
                defaultValueSql: "concat('ACC', nextval('\"ACC\"'))",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "ACC");

            migrationBuilder.DropSequence(
                name: "PAT");

            migrationBuilder.DropSequence(
                name: "SAM");

            migrationBuilder.AlterColumn<string>(
                name: "sample_number",
                table: "samples",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValueSql: "concat('SAM', nextval('\"SAM\"'))");

            migrationBuilder.AlterColumn<string>(
                name: "internal_id",
                table: "patients",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValueSql: "concat('PAT', nextval('\"PAT\"'))");

            migrationBuilder.AlterColumn<string>(
                name: "accession_number",
                table: "accessions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValueSql: "concat('ACC', nextval('\"ACC\"'))");
        }
    }
}
