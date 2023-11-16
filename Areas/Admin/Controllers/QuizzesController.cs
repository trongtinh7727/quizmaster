using System;
using System.Collections.Generic;
using System.Linq;
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
