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
    public class QuizAnswersController : Controller
    {
        private readonly QuizMasterContext _context;

        public QuizAnswersController(QuizMasterContext context)
        {
            _context = context;
        }

        // GET: Admin/QuizAnswers
        public async Task<IActionResult> Index()
        {
            var quizMasterContext = _context.Answers.Include(q => q.Question);
            return View(await quizMasterContext.ToListAsync());
        }

        // GET: Admin/QuizAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Answers == null)
            {
                return NotFound();
            }

            var quizAnswer = await _context.Answers
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quizAnswer == null)
            {
                return NotFound();
            }

            return View(quizAnswer);
        }

        // GET: Admin/QuizAnswers/Create
        public IActionResult Create()
        {
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id");
            return View();
        }

        // POST: Admin/QuizAnswers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Correct,CreatedAt,UpdatedAt,Content,QuestionId")] QuizAnswer quizAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quizAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", quizAnswer.QuestionId);
            return View(quizAnswer);
        }

        // GET: Admin/QuizAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Answers == null)
            {
                return NotFound();
            }

            var quizAnswer = await _context.Answers.FindAsync(id);
            if (quizAnswer == null)
            {
                return NotFound();
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", quizAnswer.QuestionId);
            return View(quizAnswer);
        }

        // POST: Admin/QuizAnswers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Correct,CreatedAt,UpdatedAt,Content,QuestionId")] QuizAnswer quizAnswer)
        {
            if (id != quizAnswer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quizAnswer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuizAnswerExists(quizAnswer.Id))
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
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", quizAnswer.QuestionId);
            return View(quizAnswer);
        }

        // GET: Admin/QuizAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Answers == null)
            {
                return NotFound();
            }

            var quizAnswer = await _context.Answers
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quizAnswer == null)
            {
                return NotFound();
            }

            return View(quizAnswer);
        }

        // POST: Admin/QuizAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Answers == null)
            {
                return Problem("Entity set 'QuizMasterContext.Answers'  is null.");
            }
            var quizAnswer = await _context.Answers.FindAsync(id);
            if (quizAnswer != null)
            {
                _context.Answers.Remove(quizAnswer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuizAnswerExists(int id)
        {
          return (_context.Answers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
