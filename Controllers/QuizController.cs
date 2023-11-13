using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.Models;
using QuizMaster.Services;
using QuizMaster.ViewModel;
using System.Security.Claims;

namespace QuizMaster.Controllers
{
    public class QuizController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuizMasterContext _context;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<QuizMasterUser> _userManager;

        public QuizController(ILogger<HomeController> logger, QuizMasterContext context, UserManager<QuizMasterUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [Authorize]
        public async Task<IActionResult> enrollQuiz(string enrollCode)
        {
            var quiz = _context.Quizzes
            .Include(q => q.QuizQuestions)
            .ThenInclude(qq => qq.Answers)
            .SingleOrDefault(q => q.EnrollCode == enrollCode);

            if (quiz == null)
            {
                /*return NotFound("Không tìm thấy bài quiz tương ứng");*/
                return View("NoQuizFound");
            }
            var takeQuiz = new TakeQuizViewModel
            {
                Quiz = quiz,
                StartedAt = DateTime.Now
            };

            return View("TakeQuiz", takeQuiz);
        }

        public async Task<IActionResult> QuizResults(int id)
        {
            var quiz = await _context.Quizzes
                         .Include(q => q.TakeQuizs)
                            .ThenInclude(tq => tq.User)
                         .FirstOrDefaultAsync(q => q.Id == id);
            if (quiz == null)
            {
                /*return NotFound("Không tìm thấy bài quiz tương ứng");*/
                return View("NoQuizFound");
            }

            return View("QuizResults", quiz);
        }
        public IActionResult TakeQuiz(QuizViewModel quizViewModel)
        {
            if (quizViewModel == null)
            {
                return View("NoQuizFound");
            }
            return View(quizViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TakeQuiz(TakeQuizViewModel takeQuizViewModel)
        {
            if (takeQuizViewModel == null || takeQuizViewModel.QuizID == null)
            {
                return RedirectToAction("Index");
            }
            var takeQuiz = new TakeQuiz
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                QuizId = takeQuizViewModel.QuizID,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                StartedAt = takeQuizViewModel.StartedAt,
                Score = 0,
                FinishedAt = DateTime.Now,
            };

            _context.TakeQuizzes.Add(takeQuiz);
            await _context.SaveChangesAsync();

            int totalScore = 0;
            foreach (var kvp in takeQuizViewModel.Answers)
            {
                var selectedAnswer = await _context.Answers.FindAsync(kvp.Value);
                var QuizQuestion = await _context.Questions.FindAsync(kvp.Key);
                if (selectedAnswer != null)
                {
                    if (selectedAnswer.Correct)
                    {
                        totalScore += QuizQuestion.Score;
                    }
                }

                var takeAnswer = new TakeAnswer
                {
                    TakeId = takeQuiz.Id,
                    QuestionId = selectedAnswer.QuestionId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Answer = selectedAnswer
                };

                _context.TakeAnswers.Add(takeAnswer);
            }

            takeQuiz.Score = totalScore;
            takeQuiz.FinishedAt = DateTime.Now;
            _context.TakeQuizzes.Update(takeQuiz);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult CreateQuiz()
        {
            var model = new QuizViewModel();
            return View(model);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateQuiz(QuizViewModel model)
        {
            if (ModelState.IsValid)
            {

                var quiz = new Quiz
                {
                    Title = model.QuizTitle,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Level = model.QuizLevel,
                    Published = model.QuizPublished,
                    PublishedAt = DateTime.Now,
                    Score = 0,
                    Summary = model.QuizSummary,
                    Tag = model.QuizTag
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

                quiz.EnrollCode = ApplicationServices.GenerateEnrollCode(quiz.Id.ToString());
                await _context.SaveChangesAsync();
                var sumScore = 0;
                //tao cau hoi 
                foreach (QuestionViewModel question in model.Questions)
                {
                    var quizQuestion = new QuizQuestion
                    {
                        Content = question.QuestionText,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        QuizId = quiz.Id,
                        Score = question.Score
                    };
                    sumScore += question.Score;
                    _context.Questions.Add(quizQuestion);
                    await _context.SaveChangesAsync();
                    for (int i = 0; i < question.Answers.Count; i++)
                    {
                        var answer = question.Answers[i];

                        var quizAnsw = new QuizAnswer
                        {
                            Content = answer.Content,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            QuestionId = quizQuestion.Id,
                            Correct = i == getCorrectIndex(question.CorrectAnswer)
                        };
                        _context.Answers.Add(quizAnsw);
                        await _context.SaveChangesAsync();
                    }

                }
                quiz.Score = sumScore;
                _context.Quizzes.Update(quiz);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index)); // Chuyển hướng sau khi xử lý thành công.
            }

            // Nếu ModelState không hợp lệ, quay lại view với lỗi.
            return View(model);
        }

        private int getCorrectIndex(string CorrectAnswer)
        {
            var x = CorrectAnswer;
            switch (CorrectAnswer)
            {
                case "B":
                    return 1;
                    break;
                case "C":
                    return 2;
                    break;
                case "D":
                    return 3;
                    break;
            }
            return 0;
        }

        // Controller: QuizzesController (For Edit Functionality)
        [Authorize]
        public async Task<IActionResult> EditQuiz(int id)
        {
            var quiz = await _context.Quizzes
                                     .Include(q => q.QuizQuestions)
                                     .ThenInclude(qq => qq.Answers)
                                     .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
            {
                return View("NoQuizFound");
            }

            var model = new QuizViewModel
            {
                Id = quiz.Id,
                QuizTitle = quiz.Title,
                QuizSummary = quiz.Summary,
                QuizLevel = quiz.Level,
                QuizTag = quiz.Tag,
                QuizPublished = quiz.Published,
                Questions = quiz.QuizQuestions.Select(qq => new QuestionViewModel
                {
                    Id = qq.Id,
                    QuestionText = qq.Content,
                    Answers = qq.Answers.Select(a => new AnswerViewModel
                    {
                        Id = a.Id,
                        Content = a.Content,
                    }).ToList(),
                    CorrectAnswer = qq.Answers.FirstOrDefault(a => a.Correct)?.Content,
                    Score = qq.Score
                }).ToList(),
                quiz = quiz
            };

            return View("EditQuiz", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuiz(int id, QuizViewModel model)
        {
            if (id != model.Id)
            {
                return View("NoQuizFound");
            }

            if (ModelState.IsValid)
            {
                var quiz = await _context.Quizzes
                                         .Include(q => q.QuizQuestions)
                                            .ThenInclude(qq => qq.Answers)
                                         .Include(q => q.TakeQuizs)
                                            .ThenInclude(tq => tq.TakeAnswers)
                                         .Include(q => q.TakeQuizs)
                                            .ThenInclude(tq => tq.User)
                                         .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                {
                    return View("NoQuizFound");
                }

                // Update the quiz properties
                quiz.Title = model.QuizTitle;
                quiz.Summary = model.QuizSummary;
                quiz.Level = model.QuizLevel;
                quiz.Tag = model.QuizTag;
                quiz.Published = model.QuizPublished;
                quiz.UpdatedAt = DateTime.Now;

                _context.Quizzes.Update(quiz);

                // Clear all record of take quiz
                List<string> sentEmail = new List<string>();
                foreach (TakeQuiz takeQuiz in quiz.TakeQuizs)
                {
                    _context.TakeAnswers.RemoveRange(takeQuiz.TakeAnswers);
                    var email = takeQuiz.User.Email;
                    if (!sentEmail.Contains(email))
                    {
                        var subject = $"Thông báo thay đổi về bài quiz {quiz.Title}";
                        var body = $"<html>\r\n<head>\r\n    <style>\r\n        body {{\r\n            font-family: Arial, sans-serif;\r\n            line-height: 1.6;\r\n        }}\r\n        .container {{\r\n            width: 80%;\r\n            margin: 20px auto;\r\n            padding: 20px;\r\n            border: 1px solid #ddd;\r\n            border-radius: 5px;\r\n            background-color: #f9f9f9;\r\n        }}\r\n        .header {{\r\n            font-size: 20px;\r\n            color: #333;\r\n            margin-bottom: 20px;\r\n        }}\r\n        .content {{\r\n            font-size: 16px;\r\n            color: #555;\r\n        }}\r\n        .footer {{\r\n            font-size: 14px;\r\n            color: #777;\r\n            margin-top: 20px;\r\n        }}\r\n        a {{\r\n            color: #0275d8;\r\n            text-decoration: none;\r\n        }}\r\n        a:hover {{\r\n            text-decoration: underline;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n            Thông báo thay đổi về bài Quiz: {quiz.Title}\r\n        </div>\r\n        <div class=\"content\">\r\n            Kính gửi {takeQuiz.User.FirstName} {takeQuiz.User.LastName},<br><br>\r\n\r\n            Chúng tôi xin thông báo rằng bài Quiz \"<strong>{quiz.Title}</strong>\" mà bạn đã tham gia đã được cập nhật. Do những thay đổi này, các bản ghi bài làm của bạn đã bị xóa bỏ.<br><br>\r\n\r\n            Nếu bạn muốn tham gia làm bài quiz này lại, vui lòng sử dụng mã <strong>EnrollCode: {quiz.EnrollCode}</strong> để bắt đầu.<br><br>\r\n\r\n            Chúng tôi xin lỗi vì bất kỳ sự bất tiện nào mà điều này có thể gây ra và cảm ơn bạn đã tham gia bài quiz của chúng tôi.<br><br>\r\n\r\n            Trân trọng,<br>\r\n            Đội ngũ QuizMaster\r\n        </div>\r\n        <div class=\"footer\">\r\n            Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua email: trongtinh7727@gmail.com\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>\r\n";
                        await _emailSender.SendEmailAsync(email, subject, body);
                        sentEmail.Add(email);
                    }
                }
                _context.TakeQuizzes.RemoveRange(quiz.TakeQuizs);
                await _context.SaveChangesAsync();
                var totalScore = 0;
                // Update existing questions and answers
                foreach (var question in model.Questions)
                {
                    var existingQuestion = quiz.QuizQuestions.FirstOrDefault(qq => qq.Id == question.Id);

                    if (existingQuestion != null)
                    {
                        // Update existing question
                        existingQuestion.Content = question.QuestionText;
                        existingQuestion.Score = question.Score;
                        existingQuestion.UpdatedAt = DateTime.Now;
                        totalScore += existingQuestion.Score;
                        var i = 0;
                        foreach (var answer in question.Answers)
                        {
                            var existingAnswer = existingQuestion.Answers.FirstOrDefault(a => a.Id == answer.Id);

                            if (existingAnswer != null)
                            {
                                // Update existing answer
                                existingAnswer.Content = answer.Content;
                                existingAnswer.UpdatedAt = DateTime.Now;
                                existingAnswer.Correct = i == getCorrectIndex(question.CorrectAnswer);
                            }
                            else
                            {
                                // Add new answer
                                var newAnswer = new QuizAnswer
                                {
                                    Content = answer.Content,
                                    CreatedAt = DateTime.Now,
                                    UpdatedAt = DateTime.Now,
                                    QuestionId = existingQuestion.Id,
                                    Correct = i == getCorrectIndex(question.CorrectAnswer)
                                };

                                _context.Answers.Add(newAnswer);
                                await _context.SaveChangesAsync();
                            }
                            i++;
                        }
                    }
                    else
                    {
                        // Add new question
                        totalScore += question.Score;
                        var newQuestion = new QuizQuestion
                        {
                            Content = question.QuestionText,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            QuizId = quiz.Id,
                            Score = question.Score
                        };
                        _context.Questions.Add(newQuestion);
                        await _context.SaveChangesAsync();

                        var i = 0;
                        foreach (var answer in question.Answers)
                        {
                            // Add new answer
                            var newAnswer = new QuizAnswer
                            {
                                Content = answer.Content,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                QuestionId = newQuestion.Id,
                                Correct = i == getCorrectIndex(question.CorrectAnswer)
                            };
                            _context.Answers.Add(newAnswer);
                            await _context.SaveChangesAsync();
                            i++;
                        }
                    }
                }
                // Check and remove questions not in model.Questions
                foreach (var questionToRemove in quiz.QuizQuestions)
                {
                    if (!model.Questions.Any(q => q.Id == questionToRemove.Id))
                    {
                        _context.Questions.Remove(questionToRemove);
                    }
                }
                //update score
                quiz.Score = totalScore;
                _context.Quizzes.Update(quiz);

                await _context.SaveChangesAsync();



                return RedirectToAction("Library", "Home");
            }
            return View("EditQuiz", model);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Quizzes == null)
            {
                return Problem("Entity set 'QuizMasterContext.Quizzes'  is null.");
            }
            var quiz = await _context.Quizzes
                         .Include(q => q.QuizQuestions)
                            .ThenInclude(qq => qq.Answers)
                         .Include(q => q.TakeQuizs)
                            .ThenInclude(tq => tq.TakeAnswers)
                         .Include(q => q.TakeQuizs)
                            .ThenInclude(tq => tq.User)
                         .FirstOrDefaultAsync(q => q.Id == id);
            if (quiz == null)
            {
                return View("NoQuizFound");
            }
            if (quiz.AuthorId == User.FindFirstValue(ClaimTypes.NameIdentifier)) {
                foreach (TakeQuiz takeQuiz in quiz.TakeQuizs)
                {
                    _context.TakeAnswers.RemoveRange(takeQuiz.TakeAnswers);

                }
                _context.TakeQuizzes.RemoveRange(quiz.TakeQuizs);
                _context.Quizzes.Remove(quiz);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
