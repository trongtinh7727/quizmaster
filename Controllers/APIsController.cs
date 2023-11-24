using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;

namespace QuizMaster.Controllers
{

    public class APIsController : ControllerBase
    {
        private readonly QuizMasterContext _context;

        public APIsController(QuizMasterContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetCommunityQuiz(int difficultyFilter = 0, string searchQuery = "", int pageIndex = 1, int pageSize = 16)
        {
            // Search and filter logic remains the same
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

            var response = new
            {
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                CurrentPage = pageIndex,
                PageSize = pageSize,
                Quizzes = results
            };

            return Ok(response);
        }

    }
}
