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
                        db.Users.Add(user);
                        db.SaveChanges();
                    }

                    // Set a success message
                    TempData["SuccessMessage"] = "Account created successfully! Please log in.";

                    // Redirect to login page
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    // Log error and display a message
                    TempData["ErrorMessage"] = "An error occurred. Please try again.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, bool rememberMe)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Email and password are required.";
                return View();
            }

            try
            {
                using (var db = new SalonDbContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

                    if (user != null)
                    {
                        // Store user info in session
                        HttpContext.Session.SetString("UserId", user.Id.ToString());
                        HttpContext.Session.SetString("UserRole", user.Role);

                        // Create a cookie if "Remember Me" is checked
                        if (rememberMe)
                        {
                            var cookieOptions = new CookieOptions
                            {
                                Expires = DateTime.Now.AddDays(30), // Set cookie expiration to 30 days
                                HttpOnly = true, // Prevent access via JavaScript
                                Secure = true, // Use secure cookies if your site is HTTPS
                            };

                            Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);
                            Response.Cookies.Append("UserRole", user.Role, cookieOptions);
                        }

                        TempData["SuccessMessage"] = "Login successful!";
                        return RedirectToAction("Index", "Home");
                    }
                }

                TempData["ErrorMessage"] = "Invalid email or password.";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred during login.";
                Console.WriteLine(ex.Message);
                return View();
            }
        }
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Clear cookies
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("UserRole");

            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Index", "Home");
        }

    }
}
