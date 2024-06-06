using Calendar.Models;
using Calendar.Models.Enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Calendar.HelperMethods;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Calendar.Migrations;
using NuGet.Protocol.Plugins;

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
        public IActionResult Index(string returnToHome = "false")
        {
            var filteredTaskWithStatus = HttpContext.Session.GetObject<List<TaskWithStatus>>("FilteredDueTask");
            if (filteredTaskWithStatus?.Any() == true && !bool.Parse(returnToHome))
            {
                DueTaskList filtered = new()
                {
                    PastDue = new List<TaskWithStatus>(),
                    DueToday = new List<TaskWithStatus>(),
                    DueThisWeek = new List<TaskWithStatus>(),
                    FilteredTasks = filteredTaskWithStatus.Where(x => TaskStatusHelper.AnyPendingStatus(x))
                };
                return View(filtered);
            }
            HttpContext.Session.Remove("FilteredDueTask");

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

            var taskIds = new List<int>();
            var pastDue = taskWithAllData
                    .Where(x => x.EndDate.Date < DateTime.Today.Date && TaskStatusHelper.AnyPendingStatus(x)).ToList();
            taskIds.AddRange(pastDue.Select(x => x.GeneratedTaskId));

            var dueToday = taskWithAllData
                    .Where(x => !taskIds.Contains(x.GeneratedTaskId) &&
                            (x.StartDate.Date == DateTime.Today.Date || (x.StartDate.Date < DateTime.Today.Date && x.EndDate.Date >= DateTime.Today.Date))
                            && TaskStatusHelper.AnyPendingStatus(x)).ToList();
            taskIds.AddRange(dueToday.Select(x => x.GeneratedTaskId));

            var weekEnd = DateTime.Today.AddDays(6);
            var dueThisWeek = taskWithAllData
                    .Where(x =>
                            !taskIds.Contains(x.GeneratedTaskId) &&
                            x.EndDate.Date <= weekEnd.Date &&
                            TaskStatusHelper.AnyPendingStatus(x)).ToList();
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
                FilteredTasks = new List<TaskWithStatus>(),
            };
            return View(dueTaskList);
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
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex.Message);
                    user = null;
                    ViewData["ErrorMessage"] = "The username or password is incorrect, try again.";
                    return View();
                }

                if (user?.Inactive == true)
                {
                    ViewData["ErrorMessage"] = "The account is disabled, contact administrator.";
                    return View();
                }

                if (user != null)
                {
                    UserTypeEnum userRole = user.UserType;

                    //    // Create claims for the user
                    //    var claims = new List<Claim>
                    //{
                    //    new(ClaimTypes.SerialNumber, user.Id.ToString()),
                    //    new(ClaimTypes.Name, user.CompanyName),
                    //    new(ClaimTypes.GivenName, user.RepresentativeName),
                    //    new(ClaimTypes.Email, user.Email),
                    //    new(ClaimTypes.MobilePhone, user.ContactNumber.ToString()),
                    //    new(ClaimTypes.PrimarySid, user.Username),
                    //    new(ClaimTypes.Sid, user.Password),
                    //    new(ClaimTypes.Role, userRole.ToString()), // Add user role as a claim
                    //};

                    //    // Create ClaimsIdentity
                    //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    //    // Create ClaimsPrincipal
                    //    var principal = new ClaimsPrincipal(identity);

                    //    // Sign in the user
                    //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true, AllowRefresh = true });

                    var token = GenerateJwtToken(user);
                    Response.Headers.Add("Authorization", "Bearer " + token);
                    // Set the token as a cookie
                    Response.Cookies.Append("AuthToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true, // Set to true if using HTTPS
                        SameSite = SameSiteMode.Strict
                    });

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

        private string GenerateJwtToken(User user)
        {
            UserTypeEnum userRole = user.UserType;
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.SerialNumber, user.Id.ToString()),
                new(ClaimTypes.Name, user.CompanyName),
                new(ClaimTypes.GivenName, user.RepresentativeName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.MobilePhone, user.ContactNumber.ToString()),
                new(ClaimTypes.PrimarySid, user.Username),
                new(ClaimTypes.Sid, user.Password),
                new(ClaimTypes.Role, userRole.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourVeryLongSecureSecretKeyHere1234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "JainamShah",
                audience: "KPAFinAdvisors",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
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