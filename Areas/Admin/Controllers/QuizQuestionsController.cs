using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizMaster.Data;
using QuizMaster.Models;

namespace QuizMaster.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class QuizQuestionsController : Controller
    {
        private readonly QuizMasterContext _context;

        public QuizQuestionsController(QuizMasterContext context)
        {
            _context = context;
        }

        // GET: Admin/QuizQuestions
        public async Task<IActionResult> Index()
        {
            var quizMasterContext = _context.Questions.Include(q => q.Quiz);
            return View(await quizMasterContext.ToListAsync());
        }

        // GET: Admin/QuizQuestions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var quizQuestion = await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quizQuestion == null)
            {
                return NotFound();
            }

            return View(quizQuestion);
        }

        // GET: Admin/QuizQuestions/Create
        public IActionResult Create()
        {
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Id");
            return View();
        }

        // POST: Admin/QuizQuestions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Score,CreatedAt,UpdatedAt,Content,QuizId")] QuizQuestion quizQuestion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quizQuestion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Id", quizQuestion.QuizId);
            return View(quizQuestion);
        }

        // GET: Admin/QuizQuestions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var quizQuestion = await _context.Questions.FindAsync(id);
            if (quizQuestion == null)
            {
                return NotFound();
            }
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Id", quizQuestion.QuizId);
            return View(quizQuestion);
        }

        // POST: Admin/QuizQuestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Score,CreatedAt,UpdatedAt,Content,QuizId")] QuizQuestion quizQuestion)
        {
            if (id != quizQuestion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quizQuestion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizQuestionExists(quizQuestion.Id))
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
            ViewData["QuizId"] = new SelectList(_context.Quizzes, "Id", "Id", quizQuestion.QuizId);
            return View(quizQuestion);
        }

        // GET: Admin/QuizQuestions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var quizQuestion = await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quizQuestion == null)
            {
                return NotFound();
            }

            return View(quizQuestion);
        }

        // POST: Admin/QuizQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Questions == null)
            {
                return Problem("Entity set 'QuizMasterContext.Questions'  is null.");
            }
            var quizQuestion = await _context.Questions.FindAsync(id);
            if (quizQuestion != null)
            {
                _context.Questions.Remove(quizQuestion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuizQuestionExists(int id)
        {
          return (_context.Questions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
