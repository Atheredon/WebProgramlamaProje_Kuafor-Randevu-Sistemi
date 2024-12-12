using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace KuaförRandevuSistemi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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

        private void CheckUserLogin()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                var userId = Request.Cookies["UserId"];
                var userRole = Request.Cookies["UserRole"];

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userRole))
                {
                    HttpContext.Session.SetString("UserId", userId);
                    HttpContext.Session.SetString("UserRole", userRole);
                }
            }
        }
    }
}
