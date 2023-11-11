using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.Models;
using QuizMaster.Services;
using QuizMaster.ViewModel;
using System.Diagnostics;
using System.Security.Claims;

namespace QuizMaster.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QuizMasterContext _context;
        private readonly UserManager<QuizMasterUser> _userManager;

        public HomeController(ILogger<HomeController> logger, QuizMasterContext context, UserManager<QuizMasterUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var quizMasterContext = _context.Quizzes.Include(q => q.QuizQuestions);
            return View(await quizMasterContext.ToListAsync());
        }

        public IActionResult History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the user's ID
            var takeQuizzes = _context.TakeQuizzes
                                      .Where(t => t.UserId == userId)
                                      .Include(t => t.Quiz)
                                      .ThenInclude(q => q.QuizQuestions)
                                      .ToList();

            return View(takeQuizzes);
        }

        public IActionResult Library()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var quizzes = _context.Quizzes
                                  .Where(q => q.AuthorId == userId)
                                  .Include(q => q.QuizQuestions)
                                  .ToList();

            return View(quizzes);
        }

        public async Task<IActionResult> CommunityQuiz()
        {
            var quizMasterContext = _context.Quizzes.Include(q => q.QuizQuestions);
            return View(await quizMasterContext.ToListAsync());
        }

        [HttpPost]
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

        public IActionResult TakeQuiz(QuizViewModel quizViewModel)
        {
            if (quizViewModel == null)
            {
                return NotFound("Không tìm thấy bài quiz tương ứng"); 
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
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
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
                    _context.Questions.Add(quizQuestion);
                    await _context.SaveChangesAsync();

                    foreach (AnswerViewModel answ in question.Answers)
                    {

                        var quizAnsw = new QuizAnswer
                        {
                            Content=answ.Content,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            QuestionId = quizQuestion.Id,
                            Correct = answ.Content == question.CorrectAnswer
                        };
                        _context.Answers.Add(quizAnsw);
                        await _context.SaveChangesAsync();
                    }
                }


                return RedirectToAction(nameof(Index)); // Chuyển hướng sau khi xử lý thành công.
            }

            // Nếu ModelState không hợp lệ, quay lại view với lỗi.
            return View(model);
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
                return NotFound();
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
                    Answers = qq.Answers.Select(a => new AnswerViewModel {
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var quiz = await _context.Quizzes
                                         .Include(q => q.QuizQuestions)
                                         .ThenInclude(qq => qq.Answers)
                                         .FirstOrDefaultAsync(q => q.Id == id);

                if (quiz == null)
                {
                    return NotFound();
                }

                // Update the quiz properties
                quiz.Title = model.QuizTitle;
                quiz.Summary = model.QuizSummary;
                quiz.Level = model.QuizLevel;
                quiz.Tag = model.QuizTag;
                quiz.Published = model.QuizPublished;
                quiz.UpdatedAt = DateTime.Now;

                _context.Quizzes.Update(quiz);
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

                        foreach (var answer in question.Answers)
                        {
                            var existingAnswer = existingQuestion.Answers.FirstOrDefault(a => a.Id == answer.Id);

                            if (existingAnswer != null)
                            {
                                // Update existing answer
                                existingAnswer.Content = answer.Content;
                                existingAnswer.UpdatedAt = DateTime.Now;
                                existingAnswer.Correct = answer.Content == question.CorrectAnswer;
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
                                    Correct = answer.Content == question.CorrectAnswer
                                };
                                _context.Answers.Add(newAnswer);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    else
                    {
                        // Add new question
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


                        foreach (var answer in question.Answers)
                        {
                            // Add new answer
                            var newAnswer = new QuizAnswer
                            {
                                Content = answer.Content,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                QuestionId = newQuestion.Id,
                                Correct = answer.Content == question.CorrectAnswer
                            };
                            _context.Answers.Add(newAnswer);
                            await _context.SaveChangesAsync();

                        }
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index)); // Or the appropriate action to go to after editing
            }

            // If ModelState is not valid, return to the edit view with validation errors
            return View("EditQuiz", model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}