using KpaFinAdvisors.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpaFinAdvisors.Common.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<UserWithTaskStatus>> GetPendingTaskCountForEachUserAsync();
        Task<IEnumerable<TaskWithStatus>> GetUserTaskStatusAsync(int userId, string status);
    }
}
