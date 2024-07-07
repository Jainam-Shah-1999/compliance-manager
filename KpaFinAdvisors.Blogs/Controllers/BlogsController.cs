using KpaFinAdvisors.Blogs.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KpaFinAdvisors.Blogs.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(ILogger<BlogsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult FormBlog(string editor1)
        {
            ViewBag.Editor1 = editor1.Replace(Environment.NewLine, string.Empty);
            return View("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
