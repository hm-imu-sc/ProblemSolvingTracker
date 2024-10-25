using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemSolvingTracker.Migrations
{
    /// <inheritdoc />
    public partial class removedifficultymodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Difficulties_DifficultyId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Tags_TagId",
                table: "Topics");

            migrationBuilder.DropTable(
                name: "Difficulties");

            migrationBuilder.DropIndex(
                name: "IX_Topics_DifficultyId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_TagId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "DifficultyId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Topics");

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "Tags",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_TopicId",
                table: "Tags",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Topics_TopicId",
                table: "Tags",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Topics_TopicId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_TopicId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "DifficultyId",
                table: "Topics",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Topics",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Difficulties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Level = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Difficulties", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_DifficultyId",
                table: "Topics",
                column: "DifficultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_TagId",
                table: "Topics",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Difficulties_DifficultyId",
                table: "Topics",
                column: "DifficultyId",
                principalTable: "Difficulties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Tags_TagId",
                table: "Topics",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");
        }
    }
}
