using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProblemSolvingTracker.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Difficulties_DifficultyId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Tags_TagId",
                table: "Topics");

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
        }
    }
}
