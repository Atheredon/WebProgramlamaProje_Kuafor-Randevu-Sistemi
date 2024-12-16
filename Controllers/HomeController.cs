using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KuaförRandevuSistemi.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Pass session data to the view
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            ViewData["UserRole"] = HttpContext.Session.GetString("UserRole");
            ViewData["UserId"] = HttpContext.Session.GetString("UserId");

            return View();
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
        public void CheckUserLogin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                var userId = Request.Cookies["UserId"];
                var userRole = Request.Cookies["UserRole"];
                var userName = Request.Cookies["UserName"];

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userRole) && !string.IsNullOrEmpty(userName))
                {
                    // Restore session data from cookies
                    HttpContext.Session.SetString("UserId", userId);
                    HttpContext.Session.SetString("UserRole", userRole);
                    HttpContext.Session.SetString("UserName", userName);
                }
            }
        }


    }
}
