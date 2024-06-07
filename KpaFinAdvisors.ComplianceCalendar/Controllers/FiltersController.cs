using KpaFinAdvisors.Common.Enums;
using KpaFinAdvisors.Common.Models;
using KpaFinAdvisors.Common.DatabaseContext;
using KpaFinAdvisors.ComplianceCalendar.HelperMethods;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KpaFinAdvisors.ComplianceCalendar.Controllers
{
    public class FiltersController : Controller
    {
        private readonly KpaFinAdvisorsDbContext _context;

        public FiltersController(KpaFinAdvisorsDbContext context)
        {
            _context = context;
        }

        // POST: FiltersController/Create
        [HttpPost]
        public ActionResult Filter(Filter filter)
        {
            var _userId = int.Parse(User.Claims.First(claim => claim.Type == ClaimTypes.SerialNumber).Value);
            UserTypeEnum userType = (UserTypeEnum)Enum.Parse(typeof(UserTypeEnum), User.Claims.First(claim => claim.Type == ClaimTypes.Role).Value);

            if (filter.RedirectTo == "Home")
            {
                var taskGeneratedWithTaskStatus = GetTaskGeneratedWithTaskStatus(_userId, filter);
                HttpContext.Session.SetObject("FilteredDueTask",
                    GetFinalTaskStatusData(taskGeneratedWithTaskStatus, filter).Where(x => TaskStatusHelper.AnyPendingStatus(x)));
                return RedirectToAction(nameof(Index), filter.RedirectTo);
            }
            else if (filter.RedirectTo == "TaskStatus")
            {
                var taskGeneratedWithTaskStatus = GetTaskStatusWithTaskGenerated(_userId, filter);
                HttpContext.Session.SetObject("FilteredTaskStatus",
                    GetFinalTaskStatusDataForTaskStatusList(taskGeneratedWithTaskStatus, filter));
                return RedirectToAction(nameof(Index), filter.RedirectTo);
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        private IQueryable<TaskWithStatus>? GetTaskGeneratedWithTaskStatus(int userId, Filter filter)
        {
            var taskGeneratedWithTaskStatus = from taskGenerated in _context.TaskGenerated
                                              .Where(x => filter.StartDate != DateTime.MinValue && filter.EndDate != DateTime.MinValue &&
                                              (x.StartDate >= filter.StartDate && x.StartDate <= filter.EndDate ||
                                              x.EndDate >= filter.StartDate && x.EndDate <= filter.EndDate))
                                              join taskStatus in _context.TaskStatus.Where(taskStatus => taskStatus.UserId == userId)
                                              on taskGenerated.Id equals taskStatus.GeneratedTaskId
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

        private IQueryable<TaskWithStatus>? GetTaskStatusWithTaskGenerated(int userId, Filter filter)
        {
            var taskGeneratedWithTaskStatus = from taskStatus in _context.TaskStatus.Where(taskStatus => taskStatus.UserId == userId)
                                              join taskGenerated in _context.TaskGenerated
                                              .Where(x => filter.StartDate != DateTime.MinValue && filter.EndDate != DateTime.MinValue &&
                                              (x.StartDate >= filter.StartDate && x.StartDate <= filter.EndDate ||
                                              x.EndDate >= filter.StartDate && x.EndDate <= filter.EndDate))
                                              on taskStatus.GeneratedTaskId equals taskGenerated.Id
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

        private IOrderedEnumerable<TaskWithStatus> GetFinalTaskStatusData(IQueryable<TaskWithStatus>? taskGeneratedWithTaskStatus, Filter filter)
        {
            var taskWithAllData = (from result in taskGeneratedWithTaskStatus
                                   join tasks in _context.Tasks
                                   .Where(x => string.IsNullOrEmpty(filter.TaskName) || !string.IsNullOrEmpty(filter.TaskName) && x.Name.Contains(filter.TaskName))
                                   on result.OriginalTaskId equals tasks.Id
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

        private IEnumerable<TaskWithStatus> GetFinalTaskStatusDataForTaskStatusList(IQueryable<TaskWithStatus>? taskGeneratedWithTaskStatus, Filter filter)
        {
            var taskWithAllData = from result in taskGeneratedWithTaskStatus
                                  join tasks in _context.Tasks
                                  .Where(x => string.IsNullOrEmpty(filter.TaskName) || !string.IsNullOrEmpty(filter.TaskName) && x.Name.Contains(filter.TaskName))
                                  on result.OriginalTaskId equals tasks.Id
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
