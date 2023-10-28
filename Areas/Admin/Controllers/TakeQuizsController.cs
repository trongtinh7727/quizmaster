using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.Models;

namespace QuizMaster.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TakeQuizsController : Controller
    {
        private readonly QuizMasterContext _context;

        public TakeQuizsController(QuizMasterContext context)
        {
            _context = context;
        }

        // GET: Admin/TakeQuizs
        public async Task<IActionResult> Index()
        {
            var quizMasterContext = _context.TakeQuizzes.Include(t => t.Quiz).Include(t => t.User);
            return View(await quizMasterContext.ToListAsync());
        }

        // GET: Admin/TakeQuizs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TakeQuizzes == null)
            {
                return NotFound();
            }

            var takeQuiz = await _context.TakeQuizzes
                .Include(t => t.Quiz)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (takeQuiz == null)
            {
                return NotFound();
            }

            return View(takeQuiz);
        }

        // GET: Admin/TakeQuizs/Create
        public IActionResult Create()
        {
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/TakeQuizs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Status,Score,CreatedAt,UpdatedAt,StartedAt,FinishedAt,UserId,QuizId")] TakeQuiz takeQuiz)
        {
            if (ModelState.IsValid)
            {
                _context.Add(takeQuiz);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Id", takeQuiz.QuizId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", takeQuiz.UserId);
            return View(takeQuiz);
        }

        // GET: Admin/TakeQuizs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TakeQuizzes == null)
            {
                return NotFound();
            }

            var takeQuiz = await _context.TakeQuizzes.FindAsync(id);
            if (takeQuiz == null)
            {
                return NotFound();
            }
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Id", takeQuiz.QuizId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", takeQuiz.UserId);
            return View(takeQuiz);
        }

        // POST: Admin/TakeQuizs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,Score,CreatedAt,UpdatedAt,StartedAt,FinishedAt,UserId,QuizId")] TakeQuiz takeQuiz)
        {
            if (id != takeQuiz.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(takeQuiz);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TakeQuizExists(takeQuiz.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Id", takeQuiz.QuizId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", takeQuiz.UserId);
            return View(takeQuiz);
        }

        // GET: Admin/TakeQuizs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TakeQuizzes == null)
            {
                return NotFound();
            }

            var takeQuiz = await _context.TakeQuizzes
                .Include(t => t.Quiz)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (takeQuiz == null)
            {
                return NotFound();
            }

            return View(takeQuiz);
        }

        // POST: Admin/TakeQuizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TakeQuizzes == null)
            {
                return Problem("Entity set 'QuizMasterContext.TakeQuizzes'  is null.");
            }
            var takeQuiz = await _context.TakeQuizzes.FindAsync(id);
            if (takeQuiz != null)
            {
                _context.TakeQuizzes.Remove(takeQuiz);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TakeQuizExists(int id)
        {
          return (_context.TakeQuizzes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
