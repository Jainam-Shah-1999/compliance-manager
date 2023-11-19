using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Calendar.Models;
using TaskStatus = Calendar.Models.TaskStatus;

namespace Calendar.Controllers
{
    public class TaskStatusController : Controller
    {
        private readonly CalendarDbContext _context;

        public TaskStatusController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: TaskStatus
        public async Task<IActionResult> Index()
        {
              return _context.TaskStatus != null ? 
                          View(await _context.TaskStatus.ToListAsync()) :
                          Problem("Entity set 'CalendarDbContext.TaskStatus'  is null.");
        }

        // GET: TaskStatus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TaskStatus == null)
            {
                return NotFound();
            }

            var taskStatus = await _context.TaskStatus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskStatus == null)
            {
                return NotFound();
            }

            return View(taskStatus);
        }

        // GET: TaskStatus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TaskStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,OriginalTaskId,GeneratedTaskId,Status")] TaskStatus taskStatus)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskStatus);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Home");
            }
            return View(taskStatus);
        }

        // GET: TaskStatus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TaskStatus == null)
            {
                return NotFound();
            }

            var taskStatus = await _context.TaskStatus.FindAsync(id);
            if (taskStatus == null)
            {
                return NotFound();
            }
            return View(taskStatus);
        }

        // POST: TaskStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OriginalTaskId,GeneratedTaskId,Status")] TaskStatus taskStatus)
        {
            if (id != taskStatus.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskStatus);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskStatusExists(taskStatus.Id))
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
            return View(taskStatus);
        }

        // GET: TaskStatus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TaskStatus == null)
            {
                return NotFound();
            }

            var taskStatus = await _context.TaskStatus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskStatus == null)
            {
                return NotFound();
            }

            return View(taskStatus);
        }

        // POST: TaskStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TaskStatus == null)
            {
                return Problem("Entity set 'CalendarDbContext.TaskStatus'  is null.");
            }
            var taskStatus = await _context.TaskStatus.FindAsync(id);
            if (taskStatus != null)
            {
                _context.TaskStatus.Remove(taskStatus);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskStatusExists(int id)
        {
          return (_context.TaskStatus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
