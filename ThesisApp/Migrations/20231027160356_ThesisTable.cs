using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThesisApp.Migrations
{
    public partial class ThesisTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RecommendedThesisId",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SelectedThesisId",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Theses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TitleKg = table.Column<string>(type: "TEXT", nullable: false),
                    TitleTr = table.Column<string>(type: "TEXT", nullable: false),
                    DescriptionKg = table.Column<string>(type: "TEXT", nullable: false),
                    DescriptionTr = table.Column<string>(type: "TEXT", nullable: false),
                    CuratorId = table.Column<long>(type: "INTEGER", nullable: false),
                    StudentId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Theses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RecommendedThesisId",
                table: "Users",
                column: "RecommendedThesisId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SelectedThesisId",
                table: "Users",
                column: "SelectedThesisId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Theses_RecommendedThesisId",
                table: "Users",
                column: "RecommendedThesisId",
                principalTable: "Theses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Theses_SelectedThesisId",
                table: "Users",
                column: "SelectedThesisId",
                principalTable: "Theses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Theses_RecommendedThesisId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Theses_SelectedThesisId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Theses");

            migrationBuilder.DropIndex(
                name: "IX_Users_RecommendedThesisId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SelectedThesisId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RecommendedThesisId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SelectedThesisId",
                table: "Users");
        }
    }
}
