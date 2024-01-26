using Calendar.Models;
using Calendar.Models.Enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Calendar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly CalendarDbContext _context;

        public HomeController(ILogger<HomeController> logger, CalendarDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            //return _context.TaskGenerated != null && _context.TaskStatus != null ?
            //              View(await _context.TaskGenerated.Join(
            //                  _context.Tasks, 
            //                  tg => tg.OriginalTaskId,
            //                  ts => ts.Id,
            //                  (tg, ts) => new TaskWithStatus { Name = ts.Name, TaskDescription = ts.TaskDescription, StartDate = tg.StartDate, EndDate = tg.EndDate})
            //              .ToListAsync()) :
            //              Problem("Entity set 'CalendarDbContext.TaskGenerated'  is null.");

            //var query = from taskGenerated in _context.TaskGenerated
            //            join tasks in _context.Tasks on taskGenerated.OriginalTaskId equals tasks.Id
            //            join taskStatus in _context.TaskStatus on taskGenerated.Id equals taskStatus.Id
            //            select new TaskWithStatus { Name = tasks.Name, TaskDescription = tasks.TaskDescription, StartDate = taskGenerated.StartDate, EndDate = taskGenerated.EndDate, TaskStatus = taskStatus.Status };
            //return View(query.ToList());

            //var leftJoin = from taskGenerated in _context.TaskGenerated
            //               join tasks in _context.Tasks on taskGenerated.OriginalTaskId equals tasks.Id
            //               into leftjoin
            //               from leftjoinresult in leftjoin.DefaultIfEmpty()
            //               select new TaskWithStatus
            //               {
            //                   Name = leftjoinresult.Name,
            //                   TaskDescription = leftjoinresult.TaskDescription,
            //                   StartDate = taskGenerated.StartDate,
            //                   EndDate = taskGenerated.EndDate,
            //                   OriginalTaskId = leftjoinresult.Id,
            //                   GeneratedTaskId = taskGenerated.Id,
            //                   TaskStatus = TaskStatusEnum.None,
            //               };

            //var rightJoin = from tasks in _context.Tasks
            //                join taskStatus in _context.TaskStatus on tasks.Id equals taskStatus.OriginalTaskId
            //                into rightjoin
            //                from rightjoinresult in rightjoin.DefaultIfEmpty()
            //                select new TaskWithStatus
            //                {
            //                    Name = tasks.Name,
            //                    TaskDescription = tasks.TaskDescription,
            //                    StartDate = DateTime.Now,
            //                    EndDate = DateTime.Now,
            //                    OriginalTaskId = rightjoinresult == null ? 0 : rightjoinresult.OriginalTaskId,
            //                    GeneratedTaskId = rightjoinresult == null ? 0 : rightjoinresult.GeneratedTaskId,
            //                    TaskStatus = rightjoinresult == null ? TaskStatusEnum.None : rightjoinresult.Status,
            //                };
            //var fullJoin = leftJoin.Union(rightJoin);

            var _userId = int.Parse(User.Claims.First(claim => claim.Type == ClaimTypes.SerialNumber).Value);

            var taskGeneratedWithTaskStatus = from taskGenerated in _context.TaskGenerated
                                              join taskStatus in _context.TaskStatus.Where(taskStatus => taskStatus.UserId == _userId) on taskGenerated.Id equals taskStatus.GeneratedTaskId
                                              into newjoin
                                              from newjoinresult in newjoin.DefaultIfEmpty()
                                              select new TaskWithStatus
                                              {
                                                  Name = string.Empty,
                                                  TaskDescription = string.Empty,
                                                  StartDate = taskGenerated.StartDate,
                                                  EndDate = taskGenerated.EndDate,
                                                  OriginalTaskId = taskGenerated.OriginalTaskId,
                                                  GeneratedTaskId = taskGenerated.Id,
                                                  TaskStatusId = newjoinresult == null ? 0 : newjoinresult.Id,
                                                  BSEStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.BSEStatus,
                                                  NSEStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NSEStatus,
                                                  MCXStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.MCXStatus,
                                                  NCDEXStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NCDEXStatus,
                                                  CDSLStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.CDSLStatus,
                                                  NSDLStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NSDLStatus
                                              };

            var taskWithAllData = (from result in taskGeneratedWithTaskStatus
                                  join tasks in _context.Tasks on result.OriginalTaskId equals tasks.Id
                                  into resultjoin
                                  from resultjoinresult in resultjoin.DefaultIfEmpty()
                                  select new TaskWithStatus
                                  {
                                      Name = resultjoinresult.Name,
                                      TaskDescription = resultjoinresult.TaskDescription ?? String.Empty,
                                      StartDate = result.StartDate,
                                      EndDate = result.EndDate,
                                      OriginalTaskId = result.OriginalTaskId,
                                      GeneratedTaskId = result.GeneratedTaskId,
                                      TaskStatusId = result.TaskStatusId,
                                      BSEStatus = resultjoinresult.IsBSE ? result.BSEStatus : TaskStatusEnum.NotApplicable,
                                      NSEStatus = resultjoinresult.IsNSE ? result.NSEStatus : TaskStatusEnum.NotApplicable,
                                      MCXStatus = resultjoinresult.IsMCX ? result.MCXStatus : TaskStatusEnum.NotApplicable,
                                      NCDEXStatus = resultjoinresult.IsNCDEX ? result.NCDEXStatus : TaskStatusEnum.NotApplicable,
                                      CDSLStatus = resultjoinresult.IsCDSL ? result.CDSLStatus : TaskStatusEnum.NotApplicable,
                                      NSDLStatus = resultjoinresult.IsNSDL ? result.NSDLStatus : TaskStatusEnum.NotApplicable
                                  }).AsEnumerable().OrderBy(x => x.EndDate);

            //var finalData = taskWithAllData
            //        .Where(x =>
            //                (x.StartDate.Date == DateTime.Today.Date || (x.StartDate.Date < DateTime.Today.Date && x.EndDate.Date >= DateTime.Today.Date))
            //                && x.TaskStatus == TaskStatusEnum.None);
            var taskIds = new List<int>();
            var pastDue = taskWithAllData
                    .Where(x => x.EndDate.Date < DateTime.Today.Date && IsPendingStatus(x)).ToList();
            taskIds.AddRange(pastDue.Select(x => x.GeneratedTaskId));

            var dueToday = taskWithAllData
                    .Where(x => !taskIds.Contains(x.GeneratedTaskId) &&
                            (x.StartDate.Date == DateTime.Today.Date || (x.StartDate.Date < DateTime.Today.Date && x.EndDate.Date >= DateTime.Today.Date))
                            && IsPendingStatus(x)).ToList();
            taskIds.AddRange(dueToday.Select(x => x.GeneratedTaskId));

            var weekEnd = DateTime.Today.AddDays(6 - ((double)DateTime.Today.DayOfWeek) + 1);
            var dueThisWeek = taskWithAllData
                    .Where(x =>
                            !taskIds.Contains(x.GeneratedTaskId) &&
                            x.EndDate.Date <= weekEnd.Date &&
                            IsPendingStatus(x)).ToList();
            taskIds.AddRange(dueThisWeek.Select(x => x.GeneratedTaskId));

            //var monthEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);
            //var dueThisMonth = taskWithAllData
            //        .Where(x =>
            //                !taskIds.Contains(x.GeneratedTaskId) &&
            //                x.EndDate.Date <= monthEnd.Date &&
            //                IsPendingStatus(x)).ToList();
            //taskIds.AddRange(dueThisMonth.Select(x => x.GeneratedTaskId));

            //var sixMonthEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(6).AddDays(-1);
            //var dueNextSixMonth = taskWithAllData
            //        .Where(x =>
            //                !taskIds.Contains(x.GeneratedTaskId) &&
            //                x.EndDate.Date <= sixMonthEnd.Date &&
            //                IsPendingStatus(x)).ToList();
            //taskIds.AddRange(dueNextSixMonth.Select(x => x.GeneratedTaskId));

            //var yearEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(12).AddDays(-1);
            //var dueThisYear = taskWithAllData
            //        .Where(x =>
            //                !taskIds.Contains(x.GeneratedTaskId) &&
            //                x.EndDate.Date <= yearEnd.Date &&
            //                IsPendingStatus(x)).ToList();
            //taskIds.AddRange(dueThisYear.Select(x => x.GeneratedTaskId));

            DueTaskList dueTaskList = new()
            {
                PastDue = pastDue,
                DueToday = dueToday,
                DueThisWeek = dueThisWeek,
            };
            return View(dueTaskList);
        }

        private static bool IsPendingStatus(TaskWithStatus task)
        {
            return task.BSEStatus == TaskStatusEnum.Pending ||
                   task.NSEStatus == TaskStatusEnum.Pending ||
                   task.MCXStatus == TaskStatusEnum.Pending ||
                   task.NCDEXStatus == TaskStatusEnum.Pending ||
                   task.CDSLStatus == TaskStatusEnum.Pending ||
                   task.NSDLStatus == TaskStatusEnum.Pending;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(string userName, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new User();
                try
                {
                    user = _context.Users.Where(user => user.Username == userName && user.Password == password).ToList().Single();
                }
                catch 
                {
                    user = null;
                    ViewData["ErrorMessage"] = "The username or password is incorrect, try again.";
                    return View();
                }

                if (user != null)
                {
                    UserTypeEnum userRole = user.UserType;

                    // Create claims for the user
                    var claims = new List<Claim>
                {
                    new(ClaimTypes.SerialNumber, user.Id.ToString()),
                    new(ClaimTypes.Name, user.CompanyName),
                    new(ClaimTypes.GivenName, user.RepresentativeName),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.MobilePhone, user.ContactNumber.ToString()),
                    new(ClaimTypes.PrimarySid, user.Username),
                    new(ClaimTypes.Sid, user.Password),
                    new(ClaimTypes.Role, userRole.ToString()), // Add user role as a claim
                };

                    // Create ClaimsIdentity
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Create ClaimsPrincipal
                    var principal = new ClaimsPrincipal(identity);

                    // Sign in the user
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    if (userRole == UserTypeEnum.Client)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    if (userRole == UserTypeEnum.Admin)
                    {
                        return RedirectToAction(nameof(Index), "Dashboard");
                    }
                }
            }
            ViewData["ErrorMessage"] = "An error occurred while logging you in, please try again.";

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}