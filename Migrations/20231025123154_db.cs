using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizMaster.Migrations
{
    public partial class db : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "level",
                table: "Quizzes",
                newName: "Level");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Quizzes",
                newName: "level");
        }
    }
}
