using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemSolvingTracker.Migrations
{
    /// <inheritdoc />
    public partial class topic_study_material_tag_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudyMaterials_Topics_TopicId",
                table: "StudyMaterials");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Topics_TopicId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TopicId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_StudyMaterials_TopicId",
                table: "StudyMaterials");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "StudyMaterials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "Tags",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "StudyMaterials",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TopicId",
                table: "Tags",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_TopicId",
                table: "StudyMaterials",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyMaterials_Topics_TopicId",
                table: "StudyMaterials",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Topics_TopicId",
                table: "Tags",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id");
        }
    }
}
