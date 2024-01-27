using Calendar.HelperMethods;
using Calendar.Models;
using Calendar.Models.Enums;
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
        public IActionResult Index()
        {
            var pendingTaskCountForEachUsers = from user in _context.Users.Where(x => x.UserType == UserTypeEnum.Client && !x.Inactive)
                                               let taskStatusQuery = _context.TaskStatus
                                                                     .Where(ts => ts.UserId == user.Id).ToList()
                                               select new UserWithTaskStatus
                                               {
                                                   Id = user.Id,
                                                   CompanyName = user.CompanyName,
                                                   RepresentativeName = user.RepresentativeName,
                                                   ContactNumber = user.ContactNumber,
                                                   PastDue = (from tg in _context.TaskGenerated.Where(tg => tg.EndDate.Date < DateTime.Today.Date)
                                                                   join ts in taskStatusQuery on tg.Id equals ts.GeneratedTaskId into newjoin
                                                                   from newjoinresult in newjoin.DefaultIfEmpty()
                                                                   select new TaskWithStatus
                                                                   {
                                                                       BSEStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.BSEStatus,
                                                                       NSEStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NSEStatus,
                                                                       MCXStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.MCXStatus,
                                                                       NCDEXStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NCDEXStatus,
                                                                       CDSLStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.CDSLStatus,
                                                                       NSDLStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NSDLStatus
                                                                   }).AsEnumerable()
                                                                       .Where(x => x.CDSLStatus == TaskStatusEnum.Pending ||
                                                                       x.NSDLStatus == TaskStatusEnum.Pending ||
                                                                       x.BSEStatus == TaskStatusEnum.Pending ||
                                                                       x.NSEStatus == TaskStatusEnum.Pending ||
                                                                       x.MCXStatus == TaskStatusEnum.Pending ||
                                                                       x.NCDEXStatus == TaskStatusEnum.Pending).Count(),
                                                   CompletedTasks = (from tg in _context.TaskGenerated
                                                                     join ts in taskStatusQuery on tg.Id equals ts.GeneratedTaskId into newjoin
                                                                     from newjoinresult in newjoin.DefaultIfEmpty()
                                                                     select new TaskWithStatus
                                                                     {
                                                                         BSEStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.BSEStatus,
                                                                         NSEStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NSEStatus,
                                                                         MCXStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.MCXStatus,
                                                                         NCDEXStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NCDEXStatus,
                                                                         CDSLStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.CDSLStatus,
                                                                         NSDLStatus = newjoinresult == null ? TaskStatusEnum.Pending : newjoinresult.NSDLStatus
                                                                     }).AsEnumerable().Where(x => x.CDSLStatus != TaskStatusEnum.Pending &&
                                                                       x.NSDLStatus != TaskStatusEnum.Pending &&
                                                                       x.BSEStatus != TaskStatusEnum.Pending &&
                                                                       x.NSEStatus != TaskStatusEnum.Pending &&
                                                                       x.MCXStatus != TaskStatusEnum.Pending &&
                                                                       x.NCDEXStatus != TaskStatusEnum.Pending).Count()
                                               };

            var totalCount = pendingTaskCountForEachUsers.AsEnumerable();
            return View(totalCount);
        }

        // GET: DashboardController/Details/5
        public IActionResult UserTaskStatus(int userId, string status)
        {
            var user = _context.Users.First(x => x.UserType == UserTypeEnum.Client && !x.Inactive && x.Id == userId);
            ViewData["CompanyName"] = user.CompanyName;
            ViewData["RepresentativeName"] = user.RepresentativeName;
            ViewData["TaskStatus"] = status;
            if (status == "Pending")
            {
                var taskGeneratedWithTaskStatus = GetTaskGeneratedWithTaskStatus(userId);
                var result = GetFinalTaskStatusData(taskGeneratedWithTaskStatus);
                return View(result.Where(x => TaskStatusHelper.AnyPendingStatus(x)));
            }
            else if (status == "Completed")
            {
                var taskGeneratedWithTaskStatus = GetTaskStatusWithTaskGenerated(userId);
                var result = GetCompletedTaskStatusList(taskGeneratedWithTaskStatus);
                return View(result.Where(x => TaskStatusHelper.AllCompletedOrNAStatus(x)));
            }

            return View();
        }

        private IQueryable<TaskWithStatus>? GetTaskGeneratedWithTaskStatus(int userId)
        {
            var taskGeneratedWithTaskStatus = from taskGenerated in _context.TaskGenerated
                                              .Where(x => x.EndDate.Date < DateTime.Today.Date)
                                              join taskStatus in _context.TaskStatus.Where(taskStatus => taskStatus.UserId == userId) on taskGenerated.Id equals taskStatus.GeneratedTaskId
                                              into joinedTaskStatus
                                              from taskStatusResult in joinedTaskStatus.DefaultIfEmpty()
                                              select new TaskWithStatus
                                              {
                                                  Name = string.Empty,
                                                  TaskDescription = string.Empty,
                                                  StartDate = taskGenerated.StartDate,
                                                  EndDate = taskGenerated.EndDate,
                                                  OriginalTaskId = taskGenerated.OriginalTaskId,
                                                  GeneratedTaskId = taskGenerated.Id,
                                                  TaskStatusId = taskStatusResult == null ? 0 : taskStatusResult.Id,
                                                  BSEStatus = taskStatusResult == null ? TaskStatusEnum.Pending : taskStatusResult.BSEStatus,
                                                  NSEStatus = taskStatusResult == null ? TaskStatusEnum.Pending : taskStatusResult.NSEStatus,
                                                  MCXStatus = taskStatusResult == null ? TaskStatusEnum.Pending : taskStatusResult.MCXStatus,
                                                  NCDEXStatus = taskStatusResult == null ? TaskStatusEnum.Pending : taskStatusResult.NCDEXStatus,
                                                  CDSLStatus = taskStatusResult == null ? TaskStatusEnum.Pending : taskStatusResult.CDSLStatus,
                                                  NSDLStatus = taskStatusResult == null ? TaskStatusEnum.Pending : taskStatusResult.NSDLStatus
                                              };
            return taskGeneratedWithTaskStatus;
        }

        private IQueryable<TaskWithStatus>? GetTaskStatusWithTaskGenerated(int userId)
        {
            var taskGeneratedWithTaskStatus = from taskStatus in _context.TaskStatus.Where(taskStatus => taskStatus.UserId == userId &&
                                                                                        taskStatus.CDSLStatus != TaskStatusEnum.Pending &&
                                                                                        taskStatus.NSDLStatus != TaskStatusEnum.Pending &&
                                                                                        taskStatus.BSEStatus != TaskStatusEnum.Pending &&
                                                                                        taskStatus.NSEStatus != TaskStatusEnum.Pending &&
                                                                                        taskStatus.MCXStatus != TaskStatusEnum.Pending &&
                                                                                        taskStatus.NCDEXStatus != TaskStatusEnum.Pending)
                                              join taskGenerated in _context.TaskGenerated on taskStatus.GeneratedTaskId equals taskGenerated.Id
                                              into joinedTaskStatus
                                              from taskStatusResult in joinedTaskStatus.DefaultIfEmpty()
                                              select new TaskWithStatus
                                              {
                                                  Name = string.Empty,
                                                  TaskDescription = string.Empty,
                                                  StartDate = taskStatusResult.StartDate,
                                                  EndDate = taskStatusResult.EndDate,
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
            return taskGeneratedWithTaskStatus;
        }

        private IOrderedEnumerable<TaskWithStatus> GetFinalTaskStatusData(IQueryable<TaskWithStatus>? taskGeneratedWithTaskStatus)
        {
            var taskWithAllData = (from result in taskGeneratedWithTaskStatus
                                   join tasks in _context.Tasks on result.OriginalTaskId equals tasks.Id
                                   into joinTaskStatus
                                   from taskResult in joinTaskStatus.DefaultIfEmpty()
                                   select new TaskWithStatus
                                   {
                                       Name = taskResult.Name,
                                       TaskDescription = taskResult.TaskDescription ?? string.Empty,
                                       StartDate = result.StartDate,
                                       EndDate = result.EndDate,
                                       OriginalTaskId = result.OriginalTaskId,
                                       GeneratedTaskId = result.GeneratedTaskId,
                                       TaskStatusId = result.TaskStatusId,
                                       BSEStatus = taskResult.IsBSE ? result.BSEStatus : TaskStatusEnum.NotApplicable,
                                       NSEStatus = taskResult.IsNSE ? result.NSEStatus : TaskStatusEnum.NotApplicable,
                                       MCXStatus = taskResult.IsMCX ? result.MCXStatus : TaskStatusEnum.NotApplicable,
                                       NCDEXStatus = taskResult.IsNCDEX ? result.NCDEXStatus : TaskStatusEnum.NotApplicable,
                                       CDSLStatus = taskResult.IsCDSL ? result.CDSLStatus : TaskStatusEnum.NotApplicable,
                                       NSDLStatus = taskResult.IsNSDL ? result.NSDLStatus : TaskStatusEnum.NotApplicable
                                   }).AsEnumerable().OrderBy(x => x.EndDate);
            return taskWithAllData;
        }

        private IEnumerable<TaskWithStatus> GetCompletedTaskStatusList(IQueryable<TaskWithStatus>? taskGeneratedWithTaskStatus)
        {
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
            return taskWithAllData.AsEnumerable();
        }
    }
}
