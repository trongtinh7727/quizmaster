using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizMaster.Migrations
{
    public partial class AddUserQuizFixFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TakeAnswers_Answers_AnswerId",
                table: "TakeAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_TakeAnswers_Questions_QuestionId",
                table: "TakeAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_TakeQuizzes_Quizzes_QuizId",
                table: "TakeQuizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_TakeAnswers_Answers_AnswerId",
                table: "TakeAnswers",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TakeAnswers_Questions_QuestionId",
                table: "TakeAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TakeQuizzes_Quizzes_QuizId",
                table: "TakeQuizzes",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TakeAnswers_Answers_AnswerId",
                table: "TakeAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_TakeAnswers_Questions_QuestionId",
                table: "TakeAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_TakeQuizzes_Quizzes_QuizId",
                table: "TakeQuizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_TakeAnswers_Answers_AnswerId",
                table: "TakeAnswers",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TakeAnswers_Questions_QuestionId",
                table: "TakeAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TakeQuizzes_Quizzes_QuizId",
                table: "TakeQuizzes",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
