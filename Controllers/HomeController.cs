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
                                  .Include(q=>q.TakeQuizs)
                                  .ToList();

            return View(quizzes);
        }

        public async Task<IActionResult> CommunityQuiz()
        {
            var quizMasterContext = _context.Quizzes.Include(q => q.QuizQuestions);
            return View(await quizMasterContext.ToListAsync());
        }

        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}