using Microsoft.AspNetCore.Mvc;

namespace KpaFinAdvisors.Common.Controllers
{
   public abstract class BaseController : Controller
    {
        protected readonly ILogger _logger;

        protected BaseController(ILogger logger)
        {
            _logger = logger;
        }

        protected async Task<IActionResult> Execute<T>(Func<T> action)
        {
            try
            {
                return (IActionResult)action();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
