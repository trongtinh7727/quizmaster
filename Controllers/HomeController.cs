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
            return View();
        }

        public IActionResult Library()
        {
            return View();
        }

        public async Task<IActionResult> CommunityQuiz()
        {
            var quizMasterContext = _context.Quizzes.Include(q => q.QuizQuestions);
            return View(await quizMasterContext.ToListAsync());
        }

        [Authorize]
        public IActionResult CreateQuiz()
        {
            var model = new QuizViewModel();
            return View(model);
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

                    foreach (string answ in question.Answers)
                    {

                        var quizAnsw = new QuizAnswer
                        {
                            Content=answ,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            QuestionId = quizQuestion.Id,
                            Correct = answ == question.CorrectAnswer
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}