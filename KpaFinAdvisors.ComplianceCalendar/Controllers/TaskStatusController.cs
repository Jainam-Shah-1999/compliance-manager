using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using KpaFinAdvisors.Common.Enums;
using KpaFinAdvisors.ComplianceCalendar;
using KpaFinAdvisors.Common.Models;
using TaskStatus = KpaFinAdvisors.Common.Models.TaskStatus;
using KpaFinAdvisors.ComplianceCalendar.HelperMethods;

namespace KpaFinAdvisors.ComplianceCalendar.Controllers
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
        public IActionResult Index(string returnToList = "false")
        {
            var filteredTaskWithStatus = HttpContext.Session.GetObject<List<TaskWithStatus>>("FilteredTaskStatus");
            if (filteredTaskWithStatus?.Any() == true && !bool.Parse(returnToList))
            {
                ViewData["Filter"] = "true";
                return View(filteredTaskWithStatus);
            }
            HttpContext.Session.Remove("FilteredTaskStatus");
            ViewData["Filter"] = "false";
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
                                                  BSEStatus = taskStatus.BSEStatus,
                                                  NSEStatus = taskStatus.NSEStatus,
                                                  MCXStatus = taskStatus.MCXStatus,
                                                  NCDEXStatus = taskStatus.NCDEXStatus,
                                                  CDSLStatus = taskStatus.CDSLStatus,
                                                  NSDLStatus = taskStatus.NSDLStatus,
                                                  TaskStatusId = taskStatus.Id,
                                              };

            var taskWithAllData = from result in taskGeneratedWithTaskStatus
                                  join tasks in _context.Tasks on result.OriginalTaskId equals tasks.Id
                                  into resultjoin
                                  from resultjoinresult in resultjoin.DefaultIfEmpty()
                                  select new TaskWithStatus
                                  {
                                      Name = resultjoinresult.Name,
                                      TaskDescription = resultjoinresult.TaskDescription ?? string.Empty,
                                      StartDate = result.StartDate,
                                      EndDate = result.EndDate,
                                      OriginalTaskId = result.OriginalTaskId,
                                      GeneratedTaskId = result.GeneratedTaskId,
                                      BSEStatus = result.BSEStatus,
                                      NSEStatus = result.NSEStatus,
                                      MCXStatus = result.MCXStatus,
                                      NCDEXStatus = result.NCDEXStatus,
                                      CDSLStatus = result.CDSLStatus,
                                      NSDLStatus = result.NSDLStatus,
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

            SetViewData(taskStatus.OriginalTaskId, taskStatus.GeneratedTaskId);
            return View(taskStatus);
        }

        // GET: TaskStatus/Create
        [HttpGet]
        public IActionResult Create(int generatedId, int originalId)
        {
            var task = _context.Tasks.First(x => x.Id == originalId);
            SetViewData(task.Id, generatedId);
            var model = new TaskStatus
            {
                GeneratedTaskId = generatedId,
                OriginalTaskId = originalId,
                BSEStatus = task.IsBSE ? TaskStatusEnum.Pending : TaskStatusEnum.NotApplicable,
                NSEStatus = task.IsNSE ? TaskStatusEnum.Pending : TaskStatusEnum.NotApplicable,
                MCXStatus = task.IsMCX ? TaskStatusEnum.Pending : TaskStatusEnum.NotApplicable,
                NCDEXStatus = task.IsNCDEX ? TaskStatusEnum.Pending : TaskStatusEnum.NotApplicable,
                CDSLStatus = task.IsCDSL ? TaskStatusEnum.Pending : TaskStatusEnum.NotApplicable,
                NSDLStatus = task.IsNSDL ? TaskStatusEnum.Pending : TaskStatusEnum.NotApplicable,
            };
            return View(model);
        }

        // POST: TaskStatus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(int generatedId, int originalId, [Bind("Id,OriginalTaskId,GeneratedTaskId,BSEStatus,BSEDelayDays,NSEStatus,NSEDelayDays,MCXStatus,MCXDelayDays,NCDEXStatus,NCDEXDelayDays,CDSLStatus,CDSLDelayDays,NSDLStatus,NSDLDelayDays,UserId,FormSubmittedFrom")] TaskStatus taskStatus)
        {
            if (ModelState.IsValid && !IsPendingStatus(taskStatus))
            {
                _context.Add(taskStatus);
                await _context.SaveChangesAsync();
                UpdateFilteredDataInSession(taskStatus, taskStatus.GeneratedTaskId, "FilteredDueTask");
                UpdateFilteredDataInSession(taskStatus, taskStatus.GeneratedTaskId, "FilteredTaskStatus");
                if (taskStatus.FormSubmittedFrom == "Pending" || taskStatus.FormSubmittedFrom == "Completed")
                {
                    return RedirectToAction("UserTaskStatus", "Dashboard", new { userId = taskStatus.UserId, status = taskStatus.FormSubmittedFrom });
                }
                return RedirectToAction(nameof(Index), "Home");
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        private static bool IsPendingStatus(TaskStatus task)
        {
            return task.BSEStatus == TaskStatusEnum.Pending &&
                   task.NSEStatus == TaskStatusEnum.Pending &&
                   task.MCXStatus == TaskStatusEnum.Pending &&
                   task.NCDEXStatus == TaskStatusEnum.Pending &&
                   task.CDSLStatus == TaskStatusEnum.Pending &&
                   task.NSDLStatus == TaskStatusEnum.Pending;
        }

        // GET: TaskStatus/Edit/5
        public async Task<IActionResult> Edit(int? id, string redirectTo)
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

            SetViewData(taskStatus.OriginalTaskId, taskStatus.GeneratedTaskId);
            ViewData["RedirectTo"] = redirectTo ?? "Edit";
            return View(taskStatus);
        }

        private void SetViewData(int originalTaskId, int generatedTaskId)
        {
            var task = _context.Tasks.First(x => x.Id == originalTaskId);
            var taskGenerated = _context.TaskGenerated.First(x => x.Id == generatedTaskId);
            ViewData["TaskName"] = task.Name;
            ViewData["TaskDescription"] = task.TaskDescription;
            ViewData["TaskStartDate"] = taskGenerated.StartDate.ToShortDateString();
            ViewData["TaskEndDate"] = taskGenerated.EndDate.ToShortDateString();
        }

        // POST: TaskStatus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OriginalTaskId,GeneratedTaskId,BSEStatus,BSEDelayDays,NSEStatus,NSEDelayDays,MCXStatus,MCXDelayDays,NCDEXStatus,NCDEXDelayDays,CDSLStatus,CDSLDelayDays,NSDLStatus,NSDLDelayDays,UserId,FormSubmittedFrom")] TaskStatus taskStatus)
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
                    UpdateFilteredDataInSession(taskStatus, null, "FilteredTaskStatus");
                    UpdateFilteredDataInSession(taskStatus, null, "FilteredDueTask");
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

                if (taskStatus.FormSubmittedFrom == "Edit")
                {
                    return RedirectToAction(nameof(Index));
                }
                if (taskStatus.FormSubmittedFrom == "Pending" || taskStatus.FormSubmittedFrom == "Completed")
                {
                    return RedirectToAction("UserTaskStatus", "Dashboard", new { userId = taskStatus.UserId, status = taskStatus.FormSubmittedFrom });
                }
                return RedirectToAction(nameof(Index), "Home");
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

            SetViewData(taskStatus.OriginalTaskId, taskStatus.GeneratedTaskId);
            return View(taskStatus);
        }

        // POST: TaskStatus/Delete/5
        [HttpPost, ActionName("Delete")]
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

        private void UpdateFilteredDataInSession(TaskStatus taskStatus, int? generatedTaskId, string key)
        {
            var filteredTaskWithStatus = HttpContext.Session.GetObject<List<TaskWithStatus>>(key);

            if (filteredTaskWithStatus?.Any() == true)
            {
                filteredTaskWithStatus = filteredTaskWithStatus?
                .Select(x => x.TaskStatusId == taskStatus.Id || x.GeneratedTaskId == generatedTaskId ? UpdateTask(x, taskStatus) : x)
                .ToList();
                HttpContext.Session.SetObject(key, filteredTaskWithStatus);
            }
        }

        private static TaskWithStatus UpdateTask(TaskWithStatus x, TaskStatus taskStatus)
        {
            x.BSEStatus = taskStatus.BSEStatus;
            x.NSEStatus = taskStatus.NSEStatus;
            x.MCXStatus = taskStatus.MCXStatus;
            x.NCDEXStatus = taskStatus.NCDEXStatus;
            x.CDSLStatus = taskStatus.CDSLStatus;
            x.NSDLStatus = taskStatus.NSDLStatus;
            return x;
        }
    }
}
