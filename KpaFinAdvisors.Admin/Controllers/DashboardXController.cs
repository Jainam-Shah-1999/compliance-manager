using KpaFinAdvisors.Common.Controllers;
using KpaFinAdvisors.Common.Models;
using KpaFinAdvisors.Common.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KpaFinAdvisors.Admin.Controllers
{
    public class DashboardXController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardXController(IDashboardService dashboardService, ILogger<DashboardXController> logger)
            : base(logger)
        {
            _dashboardService = dashboardService;
        }

        // GET: DashboardController
        public async Task<IActionResult> Index()
        {
            try
            {
                var totalCount = await _dashboardService.GetPendingTaskCountForEachUserAsync();
                return View(totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }

        }

        // GET: DashboardController/Details/5
        public async Task<IActionResult> UserTaskStatus(int userId, string status)
        {
            try
            {
                var userTaskStatus = await _dashboardService.GetUserTaskStatusAsync(userId, status);
                return View(userTaskStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
