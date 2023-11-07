using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizMaster.Migrations
{
    public partial class deleContentOfTakeQuiz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TakeAnswers_TakeQuizzes_TakeId",
                table: "TakeAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_TakeQuizzes_AspNetUsers_UserId",
                table: "TakeQuizzes");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "TakeQuizzes");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "TakeAnswers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TakeQuizzes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "QuizId",
                table: "TakeQuizzes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TakeId",
                table: "TakeAnswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QuestionId",
                table: "TakeAnswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AnswerId",
                table: "TakeAnswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TakeAnswers_TakeQuizzes_TakeId",
                table: "TakeAnswers",
                column: "TakeId",
                principalTable: "TakeQuizzes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TakeQuizzes_AspNetUsers_UserId",
                table: "TakeQuizzes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TakeAnswers_TakeQuizzes_TakeId",
                table: "TakeAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_TakeQuizzes_AspNetUsers_UserId",
                table: "TakeQuizzes");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TakeQuizzes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuizId",
                table: "TakeQuizzes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "TakeQuizzes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "TakeId",
                table: "TakeAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "QuestionId",
                table: "TakeAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AnswerId",
                table: "TakeAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "TakeAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_TakeAnswers_TakeQuizzes_TakeId",
                table: "TakeAnswers",
                column: "TakeId",
                principalTable: "TakeQuizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TakeQuizzes_AspNetUsers_UserId",
                table: "TakeQuizzes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
