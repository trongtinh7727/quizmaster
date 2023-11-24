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
            var quizzes = await _context.Quizzes
                                        .Include(q => q.QuizQuestions)
                                        .Include(q => q.TakeQuizs) 
                                        .ThenInclude(tq => tq.User)
                                        .Where(q => q.Published)
                                        .OrderByDescending(q => q.CreatedAt) 
                                        .ToListAsync();

            return View(quizzes);
        }

        [Authorize]
        public async Task<IActionResult> History(int pageIndex = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int pageSize = 16;

            var totalTakeQuizzes = await _context.TakeQuizzes
                                                    .Where(t => t.UserId == userId)
                                                    .CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalTakeQuizzes / pageSize);

            var takeQuizzes = await _context.TakeQuizzes
                                        .Where(t => t.UserId == userId)
                                        .Include(t => t.Quiz)
                                        .ThenInclude(q => q.QuizQuestions)
                                        .OrderByDescending(t => t.Id)
                                        .Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageIndex;
            ViewBag.PageSize = pageSize;

            return View(takeQuizzes);
        }

        [Authorize]
        public async Task<IActionResult> Library(int pageIndex = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int pageSize = 16;

            var totalQuizzes = await _context.Quizzes
                                                .Where(q => q.AuthorId == userId)
                                                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalQuizzes / pageSize);

            var quizzes = await _context.Quizzes
                                        .Where(q => q.AuthorId == userId)
                                        .Include(q => q.QuizQuestions)
                                        .Include(q => q.TakeQuizs)
                                        .OrderBy(q => q.Title)
                                        .Skip((pageIndex - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = pageIndex;
            ViewBag.PageSize = pageSize;

            return View(quizzes);
        }

        public async Task<IActionResult> CommunityQuiz(int difficultyFilter = 0, string searchQuery = "", int pageIndex = 1, int pageSize = 16)
        {
            var query = _context.Quizzes
                 .Include(q => q.QuizQuestions)
                .Include(q => q.TakeQuizs)
                .ThenInclude(tq => tq.User)
                .Where(q => q.Published)
                .OrderByDescending(q => q.CreatedAt)
                .AsQueryable();

            if (difficultyFilter != 0)
            {
                query = query.Where(q => q.Level == difficultyFilter);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerCaseSearchQuery = searchQuery.ToLower();
                query = query.Where(q => q.Title.ToLower().Contains(lowerCaseSearchQuery)
                                      || q.Summary.ToLower().Contains(lowerCaseSearchQuery)
                                      || q.Tag.ToLower().Contains(lowerCaseSearchQuery)
                                      );
            }

            // Pager
            var totalRecords = await query.CountAsync();
            var results = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            ViewBag.CurrentPage = pageIndex;
            ViewBag.PageSize = pageSize;

            return View(results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Celebrate(int takeQuizID)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var quiz = _context.TakeQuizzes
                .Where(t=>t.UserId == userId)
                .Include(q => q.Quiz)
                .Include(tq => tq.TakeAnswers)
                    .ThenInclude(tq => tq.Answer)
                .Include(tq => tq.TakeAnswers)
                    .ThenInclude(qq => qq.QuizQuestion)
                        .ThenInclude(qqq => qqq.Answers)
                .SingleOrDefault(tq => tq.Id == takeQuizID);
            if (quiz == null)
            {
                return View("NoQuizFound");
            }
            return View(quiz);
        }
    }
}