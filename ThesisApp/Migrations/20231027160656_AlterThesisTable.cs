using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThesisApp.Migrations
{
    public partial class AlterThesisTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Theses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StudentId",
                table: "Theses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
