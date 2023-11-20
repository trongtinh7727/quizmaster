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
                                        .OrderByDescending(q => q.CreatedAt) 
                                        .ToListAsync();

            return View(quizzes);
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
                                  .Include(q => q.TakeQuizs)
                                  .ToList();

            return View(quizzes);
        }

        public async Task<IActionResult> CommunityQuiz(int difficultyFilter = 0, string searchQuery = "", int pageIndex = 1, int pageSize = 16)
        {
            // Search and filter
            var query = _context.Quizzes.Include(q => q.QuizQuestions).AsQueryable();

            if (difficultyFilter != 0)
            {
                query = query.Where(q => q.Level == difficultyFilter);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerCaseSearchQuery = searchQuery.ToLower();

                query = query.Where(q => q.Title.ToLower().Contains(lowerCaseSearchQuery)
                                      || q.Summary.ToLower().Contains(lowerCaseSearchQuery)
                                      || q.Tag.ToLower().Contains(lowerCaseSearchQuery));
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
    }
}