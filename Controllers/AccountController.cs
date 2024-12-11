using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;

namespace KuaförRandevuSistemi.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new User());
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var db = new SalonDbContext())
                    {
                        // Save the user to the database
                        db.Users.Add(user);
                        db.SaveChanges();
                    }

                    // Redirect to the login page or a success page
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception ex)
                {
                    // Log the exception (optional)
                    ViewBag.ErrorMessage = "An unexpected error occurred. Please try again later.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Please correct the errors below and try again.";
            }

            // Return the same view with error message
            return View(user);
        }
    }
}
