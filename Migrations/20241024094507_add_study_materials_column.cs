using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemSolvingTracker.Migrations
{
    /// <inheritdoc />
    public partial class add_study_materials_column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudyMaterials",
                table: "Topics",
                type: "TEXT",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudyMaterials",
                table: "Topics");
        }
    }
}
