using Microsoft.AspNetCore.Mvc;

namespace KuaförRandevuSistemi.Controllers
{
    public class AdminController : BaseController
    {
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home"); // Redirect non-admin users to the home page
            }

            return View();
        }

        [HttpGet]
        public IActionResult ManageUsers()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            // Logic for managing users
            return View();
        }

        // Additional actions for managing users, staff, appointments, etc.
    }
}
