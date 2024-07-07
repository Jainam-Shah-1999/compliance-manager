using KpaFinAdvisors.Common.Enums;
using KpaFinAdvisors.Common.Helpers;
using KpaFinAdvisors.Common.Models;
using KpaFinAdvisors.Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KpaFinAdvisors.Common.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly KpaFinAdvisorsDbContext _context;

        public DashboardService(KpaFinAdvisorsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserWithTaskStatus>> GetPendingTaskCountForEachUserAsync()
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

            return await pendingTaskCountForEachUsers.ToListAsync();
        }

        public async Task<IEnumerable<TaskWithStatus>> GetUserTaskStatusAsync(int userId, string status)
        {
            if (status == "Pending")
            {
                var taskGeneratedWithTaskStatus = await GetTaskGeneratedWithTaskStatus(userId);
                return GetFinalTaskStatusData(taskGeneratedWithTaskStatus).Where(x => TaskStatusHelper.AnyPendingStatus(x));
            }
            else if (status == "Completed")
            {
                var taskGeneratedWithTaskStatus = await GetTaskStatusWithTaskGenerated(userId);
                return GetCompletedTaskStatusList(taskGeneratedWithTaskStatus).Where(x => TaskStatusHelper.AllCompletedOrNAStatus(x));
            }

            return Enumerable.Empty<TaskWithStatus>();
        }

        private async Task<IQueryable<TaskWithStatus>> GetTaskGeneratedWithTaskStatus(int userId)
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
            return await Task.FromResult(taskGeneratedWithTaskStatus);
        }

        private async Task<IQueryable<TaskWithStatus>> GetTaskStatusWithTaskGenerated(int userId)
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
            return await Task.FromResult(taskGeneratedWithTaskStatus);
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
