using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KpaFinAdvisors.Common.Enums;
using KpaFinAdvisors.Common.Models;

namespace KpaFinAdvisors.ComplianceCalendar.Controllers
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

        // GET: TaskGenerated
        public async Task<IActionResult> Index()
        {
            var taskWithAllData = from taskGenerated in _context.TaskGenerated
                                  join tasks in _context.Tasks on taskGenerated.OriginalTaskId equals tasks.Id
                                  into resultjoin
                                  from resultjoinresult in resultjoin.DefaultIfEmpty()
                                  select new TaskWithStatus
                                  {
                                      Name = resultjoinresult.Name,
                                      TaskDescription = resultjoinresult.TaskDescription ?? string.Empty,
                                      StartDate = taskGenerated.StartDate,
                                      EndDate = taskGenerated.EndDate,
                                      OriginalTaskId = taskGenerated.OriginalTaskId,
                                      GeneratedTaskId = taskGenerated.Id,
                                  };
            var result = taskWithAllData.OrderBy(x => x.EndDate).ThenBy(x => x.OriginalTaskId);
            return _context.TaskGenerated != null ?
                        View(await result.ToListAsync()) :
                        Problem("Entity set 'CalendarDbContext.TaskGenerated'  is null.");
        }

        // GET: TaskGenerated/Details/5
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

        // GET: TaskGenerated/Create
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
                                      TaskDescription = resultjoinresult.TaskDescription ?? string.Empty,
                                      StartDate = taskGenerated.StartDate,
                                      EndDate = taskGenerated.EndDate,
                                      OriginalTaskId = taskGenerated.OriginalTaskId,
                                      GeneratedTaskId = taskGenerated.Id,
                                  };

            return await Task.FromResult<IActionResult>(View(taskWithAllData));
        }

        // POST: TaskGenerated/Create
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OriginalTaskId,StartDate,EndDate")] TaskGenerated taskGenerated)
        {
            var currentDate = DateTime.Now.Date;
            var tasksToBeRemoved = from generatedTask in _context.TaskGenerated
                                   join inactiveTask in _context.Tasks
                                   on generatedTask.OriginalTaskId equals inactiveTask.Id
                                   where generatedTask.StartDate >= DateTime.Now.Date
                                   select generatedTask;

            var taskStatusToBeRemoved = from taskStatus in _context.TaskStatus
                                        join generatedTask in tasksToBeRemoved on taskStatus.GeneratedTaskId equals generatedTask.Id
                                        select taskStatus;

            _context.TaskGenerated.RemoveRange(tasksToBeRemoved);
            _context.TaskStatus.RemoveRange(taskStatusToBeRemoved);

            var generatedTasks = CreateGeneratedTask(tasksToBeRemoved.Any());
            _context.AddRange(generatedTasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public List<TaskGenerated> CreateGeneratedTask(bool regenerateTask = false)
        {
            var tasks = _context.Tasks.Where(x => !x.Inactive);
            holidays.AddRange(_context.Holidays.Where(date => date.HolidayDate.Year == DateTime.Now.Year).Select(x => x.HolidayDate));
            var tasksGenerated = new List<TaskGenerated>();
            foreach (var task in tasks)
            {
                tasksGenerated.AddRange(GenerateRecurringTasks(task, regenerateTask));
            }

            //return Task.FromResult<IActionResult>(View(tasksGenerated));
            return tasksGenerated;
        }

        private IEnumerable<TaskGenerated> GenerateRecurringTasks(Tasks task, bool regenerateTask)
        {
            DateTime currentDate = regenerateTask && task.StartDate < DateTime.Now.Date ? DateTime.Now.Date : task.StartDate;
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
    }
}
