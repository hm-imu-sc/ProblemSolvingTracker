using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemSolvingTracker.Migrations
{
    /// <inheritdoc />
    public partial class add_topic_study_material_tag_relation_db_context : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TopicStudyMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TopicId = table.Column<int>(type: "INTEGER", nullable: true),
                    StudyMaterialId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicStudyMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicStudyMaterials_StudyMaterials_StudyMaterialId",
                        column: x => x.StudyMaterialId,
                        principalTable: "StudyMaterials",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TopicStudyMaterials_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TopicTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TopicId = table.Column<int>(type: "INTEGER", nullable: true),
                    TagId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TopicTags_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicStudyMaterials_StudyMaterialId",
                table: "TopicStudyMaterials",
                column: "StudyMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicStudyMaterials_TopicId",
                table: "TopicStudyMaterials",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicTags_TagId",
                table: "TopicTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicTags_TopicId",
                table: "TopicTags",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicStudyMaterials");

            migrationBuilder.DropTable(
                name: "TopicTags");
        }
    }
}
