using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Areas.Admin.Viewmodel;
using QuizMaster.Data;
using QuizMaster.Models;

namespace QuizMaster.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    
    public class QuizzesController : Controller
    {
        private readonly QuizMasterContext _context;

        public QuizzesController(QuizMasterContext context)
        {
            _context = context;
        }

        // GET: Admin/Quizzes
        public async Task<IActionResult> Index()
        {
            var quizMasterContext = _context.Quizzes.Include(q => q.Author);
            return View(await quizMasterContext.ToListAsync());
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
            foreach (TakeQuiz takeQuiz in quiz.TakeQuizs)
            {
                _context.TakeAnswers.RemoveRange(takeQuiz.TakeAnswers);

            }
            _context.TakeQuizzes.RemoveRange(quiz.TakeQuizs);
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Statistics()
        {

            var quizCountsByLevel = new List<int>();
            for (int i = 1; i < 4; i++)
            {
                var counter = _context.Quizzes
                    .Where(u => u.Level == i)
                    .Count();
                quizCountsByLevel.Add(counter);
            }

            var currentDate = DateTime.Now;
            var currentWeekStart = currentDate.Date.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday); 
            var currentWeekEnd = currentWeekStart.AddDays(6);

            var quizCountsByDay = new List<int>();
            for (var date = currentWeekStart; date <= currentWeekEnd; date = date.AddDays(1))
            {
                var accountsOnDay = _context.Quizzes
                    .Where(u => u.CreatedAt.Date == date.Date)
                    .Count();
                quizCountsByDay.Add(accountsOnDay);
            }

            var data = new QuizStaticsViewModel
            {
                QuizCountsByLevel = quizCountsByLevel,
                QuizCountsByDay = quizCountsByDay
            };

            return View(data);
        }
    }
}
