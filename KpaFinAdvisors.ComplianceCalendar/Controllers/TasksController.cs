using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using KpaFinAdvisors.Common.DatabaseContext;
using KpaFinAdvisors.Common.Models;

namespace KpaFinAdvisors.ComplianceCalendar.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly KpaFinAdvisorsDbContext _context;

        public TasksController(KpaFinAdvisorsDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index()
        {
            return _context.Tasks != null ?
                        View(await _context.Tasks.ToListAsync()) :
                        Problem("Entity set 'CalendarDbContext.Tasks'  is null.");
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Name,TaskDescription,RecurrenceFrequency,DueDays,BusinessDays,DueCompletion,StartDate,IsNSE,DelaySubmissionNSE,NonSubmissionNSE,IsBSE,DelaySubmissionBSE,NonSubmissionBSE,IsMCX,DelaySubmissionMCX,NonSubmissionMCX,IsNCDEX,DelaySubmissionNCDEX,NonSubmissionNCDEX,IsCDSL,DelaySubmissionCDSL,NonSubmissionCDSL,IsNSDL,DelaySubmissionNSDL,NonSubmissionNSDL,Inactive,InactiveDate")] Tasks task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,TaskDescription,RecurrenceFrequency,DueDays,BusinessDays,DueCompletion,StartDate,IsNSE,DelaySubmissionNSE,NonSubmissionNSE,IsBSE,DelaySubmissionBSE,NonSubmissionBSE,IsMCX,DelaySubmissionMCX,NonSubmissionMCX,IsNCDEX,DelaySubmissionNCDEX,NonSubmissionNCDEX,IsCDSL,DelaySubmissionCDSL,NonSubmissionCDSL,IsNSDL,DelaySubmissionNSDL,NonSubmissionNSDL,Inactive,InactiveDate")] Tasks task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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
            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'CalendarDbContext.Tasks'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return (_context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
