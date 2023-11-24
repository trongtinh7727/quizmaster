using Microsoft.EntityFrameworkCore;
using QuizMaster.Models;
using QuizMaster.Services;
using System.Security.Claims;

namespace QuizMaster.Data
{
    public static class DatabaseSeeder
    {
        public static async Task Seed(QuizMasterContext _context, string authorID)
        {
            // Check if the database already has quizzes
            if (_context.Quizzes.Count()<20)
            {
                // Create a list to hold all quizzes
                var quizzes = new List<Quiz>();

                // Generate 20 quizzes
                for (int i = 1; i <= 20; i++)
                {
                    var quiz = new Quiz
                    {
                        Title = $"Bài quiz sô {i}",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        AuthorId = authorID,
                        Level = i%3 + 1,
                        Published = true,
                        PublishedAt = DateTime.Now,
                        Score = 100,
                        Summary = $"Chi la data de test thoi {i}",
                        Tag = $"test"
                    };
                    _context.Quizzes.Add(quiz);
                    await _context.SaveChangesAsync();

                    quiz.EnrollCode = ApplicationServices.GenerateEnrollCode(quiz.Id.ToString());
                    await _context.SaveChangesAsync();

                    // Add questions and answers
                    var questions = new List<QuizQuestion>();
                    for (int q = 0; q < 5; q++) // assuming 5 questions per quiz
                    {
                        var quizQuestion = new QuizQuestion
                        {
                            Content = $"Cau hoi so {q}",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            QuizId = quiz.Id,
                            Score = 20
                        };
                        _context.Questions.Add(quizQuestion);
                        await _context.SaveChangesAsync();

                        for (int a = 0; a < 4; a++)
                        {
                            var quizAnsw = new QuizAnswer
                            {
                                Content = $"Dap an {a}",
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                QuestionId = quizQuestion.Id,
                                Correct = a == 1
                            };
                            _context.Answers.Add(quizAnsw);
                            await _context.SaveChangesAsync();

                        }
                    }

                   
                }
            }
        }
    }

}
