using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Calendar.Models;
using Calendar.Models.Enums;

namespace Calendar.Controllers
{
    public class TaskGeneratedController : Controller
    {
        private readonly CalendarDbContext _context;

        private List<DateTime> holidays;

        public TaskGeneratedController(CalendarDbContext context)
        {
            _context = context;
            holidays = new List<DateTime>();
        }

        // GET: TaskGenerateds
        public async Task<IActionResult> Index()
        {
            var taskWithAllData = from taskGenerated in _context.TaskGenerated
                                  join tasks in _context.Tasks on taskGenerated.OriginalTaskId equals tasks.Id
                                  into resultjoin
                                  from resultjoinresult in resultjoin.DefaultIfEmpty()
                                  select new TaskWithStatus
                                  {
                                      Name = resultjoinresult.Name,
                                      TaskDescription = resultjoinresult.TaskDescription,
                                      StartDate = taskGenerated.StartDate,
                                      EndDate = taskGenerated.EndDate,
                                      OriginalTaskId = taskGenerated.OriginalTaskId,
                                      GeneratedTaskId = taskGenerated.Id,
                                  };
            return _context.TaskGenerated != null ?
                        View(await taskWithAllData.ToListAsync()) :
                        Problem("Entity set 'CalendarDbContext.TaskGenerated'  is null.");
        }

        // GET: TaskGenerateds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TaskGenerated == null)
            {
                return NotFound();
            }

            var taskGenerated = await _context.TaskGenerated
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskGenerated == null)
            {
                return NotFound();
            }

            return View(taskGenerated);
        }

        // GET: TaskGenerateds/Create
        public async Task<IActionResult> Create()
        {
            var generatedTasks = CreateGeneratedTask();
            var taskWithAllData = from taskGenerated in generatedTasks
                                  join tasks in _context.Tasks on taskGenerated.OriginalTaskId equals tasks.Id
                                  into resultjoin
                                  from resultjoinresult in resultjoin.DefaultIfEmpty()
                                  select new TaskWithStatus
                                  {
                                      Name = resultjoinresult.Name,
                                      TaskDescription = resultjoinresult.TaskDescription,
                                      StartDate = taskGenerated.StartDate,
                                      EndDate = taskGenerated.EndDate,
                                      OriginalTaskId = taskGenerated.OriginalTaskId,
                                      GeneratedTaskId = taskGenerated.Id,
                                  };
            
            return await Task.FromResult<IActionResult>(View(taskWithAllData));
        }

        // POST: TaskGenerateds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OriginalTaskId,StartDate,EndDate")] TaskGenerated taskGenerated)
        {
            var currentDate = DateTime.Now.Date;
            var tasksToBeRemoved = _context.TaskGenerated.Where(x => x.StartDate >= currentDate);
            var taskStats = _context.TaskStatus.Where(x => tasksToBeRemoved.Select(y => y.Id).Contains(x.GeneratedTaskId));
            if (taskStats.Any())
            {

            }
            else
            {
                _context.TaskGenerated.RemoveRange(tasksToBeRemoved);
            }
            var generatedTasks = CreateGeneratedTask();
            _context.AddRange(generatedTasks);
            await _context.SaveChangesAsync();
            //if (ModelState.IsValid)
            //{
            //    _context.Add(taskGenerated);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            return RedirectToAction(nameof(Index));
        }

        // GET: TaskGenerateds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TaskGenerated == null)
            {
                return NotFound();
            }

            var taskGenerated = await _context.TaskGenerated.FindAsync(id);
            if (taskGenerated == null)
            {
                return NotFound();
            }
            return View(taskGenerated);
        }

        // POST: TaskGenerateds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OriginalTaskId,StartDate,EndDate")] TaskGenerated taskGenerated)
        {
            if (id != taskGenerated.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskGenerated);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskGeneratedExists(taskGenerated.Id))
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
            return View(taskGenerated);
        }

        // GET: TaskGenerateds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TaskGenerated == null)
            {
                return NotFound();
            }

            var taskGenerated = await _context.TaskGenerated
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskGenerated == null)
            {
                return NotFound();
            }

            return View(taskGenerated);
        }

        // POST: TaskGenerateds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TaskGenerated == null)
            {
                return Problem("Entity set 'CalendarDbContext.TaskGenerated'  is null.");
            }
            var taskGenerated = await _context.TaskGenerated.FindAsync(id);
            if (taskGenerated != null)
            {
                _context.TaskGenerated.Remove(taskGenerated);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GeneratedIndex()
        {
            return _context.TaskGenerated != null ?
                        View(await _context.TaskGenerated.ToListAsync()) :
                        Problem("Entity set 'CalendarDbContext.TaskGenerated'  is null.");
        }

        public List<TaskGenerated> CreateGeneratedTask()
        {
            var tasks = _context.Tasks;
            holidays.AddRange(_context.Holidays.Where(date => date.HolidayDate.Year == DateTime.Now.Year).Select(x => x.HolidayDate));
            var tasksGenerated = new List<TaskGenerated>();
            foreach (var task in tasks)
            {
                tasksGenerated.AddRange(GenerateRecurringTasks(task));
            }

            //return Task.FromResult<IActionResult>(View(tasksGenerated));
            return tasksGenerated;
        }

        private IEnumerable<TaskGenerated> GenerateRecurringTasks(Tasks task)
        {
            DateTime currentDate = task.StartDate;
            while (IsHoliday(currentDate))
            {
                currentDate = currentDate.AddDays(1);
            }
            DateTime endDate = new(DateTime.Now.Year, 12, 31); // Assuming generating tasks for one year

            List<TaskGenerated> recurringTasks = new();

            while (currentDate < endDate)
            {
                while (IsHoliday(currentDate) || !IsBusinessDay(currentDate, task.BusinessDays))
                {
                    currentDate = currentDate.AddDays(1);
                }
                // Check if the current date is not a holiday and is a business day based on the specified BusinessDaysEnum
                if (!IsHoliday(currentDate) && IsBusinessDay(currentDate, task.BusinessDays))
                {
                    recurringTasks.Add(new TaskGenerated
                    {
                        OriginalTaskId = task.Id,
                        StartDate = currentDate,
                        EndDate = CalculateEndDate(currentDate, task.DueDays, task.DueCompletion, task.BusinessDays),
                    });
                }
                if (task.RecurrenceFrequency == RecurrenceFrequencyEnum.Single)
                {
                    break;
                }
                currentDate = GetNextDate(currentDate, task.RecurrenceFrequency);
            }

            return recurringTasks;
        }

        private static DateTime GetNextDate(DateTime currentDate, RecurrenceFrequencyEnum recurrence)
        {
            return recurrence switch
            {
                RecurrenceFrequencyEnum.Daily => currentDate.AddDays(1),
                RecurrenceFrequencyEnum.Weekly => currentDate.AddDays(7),
                RecurrenceFrequencyEnum.BiWeekly => currentDate.AddDays(15),
                RecurrenceFrequencyEnum.Monthly => new DateTime(currentDate.AddMonths(1).Year, currentDate.AddMonths(1).Month, 1),
                RecurrenceFrequencyEnum.BiMonthly => new DateTime(currentDate.AddMonths(2).Year, currentDate.AddMonths(2).Month, 1),
                RecurrenceFrequencyEnum.Quarterly => new DateTime(currentDate.AddMonths(3).Year, currentDate.AddMonths(3).Month, 1),
                RecurrenceFrequencyEnum.HalfYearly => new DateTime(currentDate.AddMonths(6).Year, currentDate.AddMonths(6).Month, 1),
                RecurrenceFrequencyEnum.Yearly => currentDate.AddYears(1),
                _ => currentDate,
            };
        }

        private DateTime CalculateEndDate(DateTime currentDate, int dueDays, DueCompletionEnum dueCompletion, BusinessDaysEnum businessDaysEnum)
        {
            var endDate = currentDate.AddDays(dueDays - 1);
            while (IsHoliday(endDate) || !IsBusinessDay(endDate, businessDaysEnum))
            {
                switch (dueCompletion)
                {
                    case DueCompletionEnum.Precede:
                        endDate = endDate.AddDays(-1);
                        break;
                    case DueCompletionEnum.Succeed:
                        endDate = endDate.AddDays(1);
                        break;
                }
            }
            return endDate;
        }

        private static bool IsBusinessDay(DateTime currentDate, BusinessDaysEnum businessDays)
        {
            if (businessDays == BusinessDaysEnum.TradingDays)
            {
                return currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday;
            }
            return true;
        }

        private bool IsHoliday(DateTime currentDate)
        {
            return holidays.Contains(currentDate.Date);
        }

        private bool TaskGeneratedExists(int id)
        {
            return (_context.TaskGenerated?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
