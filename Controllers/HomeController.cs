using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.Models;
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
                    Level = 1,
                    Published = true,
                    PublishedAt = DateTime.Now,
                    Score = 0,
                    Summary = "No cam`men",
                    Tag = "anh"
                };
                
                _context.Quizzes.Add(quiz);
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
                        Score = 5
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