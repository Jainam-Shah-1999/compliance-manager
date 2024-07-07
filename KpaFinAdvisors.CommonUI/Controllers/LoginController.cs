using KpaFinAdvisors.Common.Enums;
using KpaFinAdvisors.Common.Models;
using KpaFinAdvisors.Common.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Claims;

namespace KpaFinAdvisors.CommonUI.Controllers
{
    [Authorize]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        private readonly IAuthenticationService _authenticationService;

        public LoginController(ILogger<LoginController> logger, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var token = Request.Cookies["AuthToken"];
            if (token != null)
            {
                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
                LogoutAsync();
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(string username, string password)
        {
            string token = string.Empty;
            User user = null;
            try
            {
                (user, token) = _authenticationService.Login(username, password, User);
                Response.Headers.Add("Authorization", token);
                // Set the token as a cookie
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to true if using HTTPS
                    SameSite = SameSiteMode.Strict,
                });
                //SetAuthToken(token);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                return View();
            }
            Thread.Sleep(1000);
            if (user == null)
            {
                ViewData["ErrorMessage"] = "Unable to determine user identity, login again.";
                _authenticationService.Logout(token);
                return View();
            }

            if (user.UserType == UserTypeEnum.Client)
            {
                return RedirectToAction(nameof(Index), "Home");
            }
            if (user.UserType == UserTypeEnum.Admin)
            {
#if DEBUG
                if (Request.GetDisplayUrl().Contains("7284"))
#else
                if (Request.GetDisplayUrl().Contains("compliance"))
#endif
                {
                    ViewData["ErrorMessage"] = "Use admin portal for administration work";
                    _authenticationService.Logout(token);
                    return View();
                }
                return RedirectToAction(nameof(Index), "Dashboard");
            }
            ViewData["ErrorMessage"] = "An error occurred while logging you in, try again.";
            _authenticationService.Logout(token);
            return View();
        }

        private void SetAuthToken(string token)
        {
            Response.Headers.Add("Authorization", "Bearer " + token);
            // Set the token as a cookie
            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true if using HTTPS
                SameSite = SameSiteMode.Strict
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogoutAsync()
        {
            var token = Request.Cookies["AuthToken"];
            if (token != null)
            {
                _authenticationService.Logout(token);
                Response.Headers.Remove("Authorization");
                Response.Cookies.Delete("AuthToken");
            }
            return RedirectToAction(nameof(Index));
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
