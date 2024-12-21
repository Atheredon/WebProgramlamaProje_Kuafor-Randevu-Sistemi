using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;

namespace KuaförRandevuSistemi.Controllers
{
    public class AccountController : BaseController
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
                        // Check if the email is already registered
                        if (db.Users.Any(u => u.Email == user.Email))
                        {
                            TempData["ErrorMessage"] = "Email is already registered.";
                            return View(user); // Return the same view with the error message
                        }

                        // Save the user to the database
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
                    Console.WriteLine(ex.Message);
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
                        HttpContext.Session.SetString("UserName", user.Name + " " + user.Surname);

                        // Create cookies if "Remember Me" is checked
                        if (rememberMe)
                        {
                            var cookieOptions = new CookieOptions
                            {
                                Expires = DateTime.Now.AddDays(30),
                                HttpOnly = true,
                                Secure = true,
                            };

                            Response.Cookies.Append("UserId", user.Id.ToString(), cookieOptions);
                            Response.Cookies.Append("UserRole", user.Role, cookieOptions);
                            Response.Cookies.Append("UserName", user.Name + " " + user.Surname, cookieOptions);
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

        public IActionResult ViewDetails()
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            using (var db = new SalonDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                // Pass user data to the layout via ViewData
                ViewData["UserName"] = user.Name + " " + user.Surname;
                ViewData["UserRole"] = user.Role;

                return View(user);
            }
        }

    }
}
