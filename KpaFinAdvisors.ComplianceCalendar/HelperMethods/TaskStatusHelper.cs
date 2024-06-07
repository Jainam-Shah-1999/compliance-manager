using KpaFinAdvisors.Common.Enums;
using KpaFinAdvisors.Common.Models;

namespace KpaFinAdvisors.ComplianceCalendar.HelperMethods
{
    public static class TaskStatusHelper
    {
        public static bool AnyPendingStatus<T>(T task) where T : TaskStatusBase
        {
            return task.BSEStatus == TaskStatusEnum.Pending ||
                   task.NSEStatus == TaskStatusEnum.Pending ||
                   task.MCXStatus == TaskStatusEnum.Pending ||
                   task.NCDEXStatus == TaskStatusEnum.Pending ||
                   task.CDSLStatus == TaskStatusEnum.Pending ||
                   task.NSDLStatus == TaskStatusEnum.Pending;
        }

        public static bool AllPendingStatus<T>(T task) where T : TaskStatusBase
        {
            return task.BSEStatus == TaskStatusEnum.Pending &&
                   task.NSEStatus == TaskStatusEnum.Pending &&
                   task.MCXStatus == TaskStatusEnum.Pending &&
                   task.NCDEXStatus == TaskStatusEnum.Pending &&
                   task.CDSLStatus == TaskStatusEnum.Pending &&
                   task.NSDLStatus == TaskStatusEnum.Pending;
        }

        public static bool AnyCompletedOrNAStatus<T>(T task) where T : TaskStatusBase
        {
            return task.BSEStatus != TaskStatusEnum.Pending ||
                   task.NSEStatus != TaskStatusEnum.Pending ||
                   task.MCXStatus != TaskStatusEnum.Pending ||
                   task.NCDEXStatus != TaskStatusEnum.Pending ||
                   task.CDSLStatus != TaskStatusEnum.Pending ||
                   task.NSDLStatus != TaskStatusEnum.Pending;
        }

        public static bool AllCompletedOrNAStatus<T>(T task) where T : TaskStatusBase
        {
            return task.BSEStatus != TaskStatusEnum.Pending &&
                   task.NSEStatus != TaskStatusEnum.Pending &&
                   task.MCXStatus != TaskStatusEnum.Pending &&
                   task.NCDEXStatus != TaskStatusEnum.Pending &&
                   task.CDSLStatus != TaskStatusEnum.Pending &&
                   task.NSDLStatus != TaskStatusEnum.Pending;
        }
    }
}
