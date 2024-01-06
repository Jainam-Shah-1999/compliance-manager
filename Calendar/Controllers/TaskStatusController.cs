using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Calendar.Models;
using TaskStatus = Calendar.Models.TaskStatus;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Calendar.Models.Enums;

namespace Calendar.Controllers
{
    [Authorize]
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
            var _userId = int.Parse(User.Claims.First(claim => claim.Type == ClaimTypes.SerialNumber).Value);
            var taskGeneratedWithTaskStatus = from taskStatus in _context.TaskStatus.Where(taskStatus => taskStatus.UserId == _userId)
                                              join taskGenerated in _context.TaskGenerated on taskStatus.GeneratedTaskId equals taskGenerated.Id 
                                              into newjoin
                                              from newjoinresult in newjoin.DefaultIfEmpty()
                                              select new TaskWithStatus
                                              {
                                                  Name = string.Empty,
                                                  TaskDescription = string.Empty,
                                                  StartDate = newjoinresult.StartDate,
                                                  EndDate = newjoinresult.EndDate,
                                                  OriginalTaskId = taskStatus.OriginalTaskId,
                                                  GeneratedTaskId = taskStatus.GeneratedTaskId,
                                                  TaskStatus = taskStatus.Status,
                                                  TaskStatusId = taskStatus.Id,
                                              };

            var taskWithAllData = from result in taskGeneratedWithTaskStatus
                                  join tasks in _context.Tasks on result.OriginalTaskId equals tasks.Id
                                  into resultjoin
                                  from resultjoinresult in resultjoin.DefaultIfEmpty()
                                  select new TaskWithStatus
                                  {
                                      Name = resultjoinresult.Name,
                                      TaskDescription = resultjoinresult.TaskDescription,
                                      StartDate = result.StartDate,
                                      EndDate = result.EndDate,
                                      OriginalTaskId = result.OriginalTaskId,
                                      GeneratedTaskId = result.GeneratedTaskId,
                                      TaskStatus = result.TaskStatus,
                                      TaskStatusId = result.TaskStatusId
                                  };
            return View(taskWithAllData.AsEnumerable());

            //return _context.TaskStatus != null ? 
            //              View(await _context.TaskStatus.Where(x => x.UserId == _userId).ToListAsync()) :
            //              Problem("Entity set 'CalendarDbContext.TaskStatus'  is null.");
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
        public async Task<IActionResult> Create([Bind("Id,OriginalTaskId,GeneratedTaskId,Status,UserId")] TaskStatus taskStatus)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,OriginalTaskId,GeneratedTaskId,Status,UserId")] TaskStatus taskStatus)
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
