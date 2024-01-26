using Calendar.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly CalendarDbContext _context;

        public DashboardController(CalendarDbContext context)
        {
            _context = context;
        }

        // GET: DashboardController
        public ActionResult Index()
        {
            var clients = _context.Users.Where(x => x.UserType == Models.Enums.UserTypeEnum.Client);
            var taskGeneratedWithTaskStatus = from taskStatus in _context.TaskStatus
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
                                                  UserName = clients.Where(x => x.Id == taskStatus.UserId).First().Username
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
                                      TaskStatusId = result.TaskStatusId,
                                      UserName = result.UserName
                                  };
            return View(taskWithAllData.AsEnumerable());
        }

        // GET: DashboardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DashboardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashboardController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashboardController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DashboardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashboardController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
