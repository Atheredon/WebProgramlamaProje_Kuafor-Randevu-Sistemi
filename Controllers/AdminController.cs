using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuaförRandevuSistemi.Controllers
{
    public class AdminController : BaseAdminController
    {
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }


        public IActionResult Services()
        {
            using (var db = new SalonDbContext())
            {
                var services = db.Services.ToList();
                return View(services);
            }
        }

        [HttpGet]
        public IActionResult AddService()
        {
            return View(new Service());
        }
        [HttpPost]
        public IActionResult AddService(Service service)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SalonDbContext())
                {
                    db.Services.Add(service);
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = service.Name + " Service added successfully!";
                return RedirectToAction("Services");
            }

            return View(service);
        }

        [HttpGet]
        public IActionResult DeleteService(int id)
        {
            // Example validation for later
            //var hasAppointments = db.Appointments.Any(a => a.ServiceId == id);
            //if (hasAppointments)
            //{
            //    TempData["ErrorMessage"] = "This service cannot be deleted because it is associated with one or more appointments.";
            //    return RedirectToAction("Services");
            //}
            using (var db = new SalonDbContext())
            {
                var service = db.Services.FirstOrDefault(s => s.Id == id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("Services");
                }

                // TODO: Add logic to check if the service is linked to appointments.

                db.Services.Remove(service);
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Service '{service.Name}' deleted successfully!";
            }

            return RedirectToAction("Services");
        }

        [HttpGet]
        public IActionResult EditService(int id)
        {
            // Example validation for later
            //var hasAppointments = db.Appointments.Any(a => a.ServiceId == id);
            //if (hasAppointments)
            //{
            //    TempData["ErrorMessage"] = "This service cannot be deleted because it is associated with one or more appointments.";
            //    return RedirectToAction("Services");
            //}
            using (var db = new SalonDbContext())
            {
                var service = db.Services.FirstOrDefault(s => s.Id == id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("Services");
                }

                return View(service);
            }
        }
        [HttpPost]
        public IActionResult EditService(Service service)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SalonDbContext())
                {
                    var existingService = db.Services.FirstOrDefault(s => s.Id == service.Id);
                    if (existingService != null)
                    {
                        existingService.Name = service.Name;
                        existingService.Description = service.Description;
                        existingService.Duration = service.Duration;
                        existingService.Price = service.Price;

                        db.SaveChanges();
                        TempData["SuccessMessage"] = service.Name + " Service updated successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Service not found.";
                    }
                }

                return RedirectToAction("Services");
            }

            return View(service);
        }


        public IActionResult Staff()
        {
            using (var db = new SalonDbContext())
            {
                var staff = db.Set<Staff>().Include(s => s.Specialty).Include(s => s.Services).ToList();
                return View(staff);
            }
        }

        [HttpGet]
        public IActionResult AddStaff()
        {
            using (var db = new SalonDbContext())
            {
                ViewBag.Services = db.Services.ToList();
            }

            return View(new Staff());
        }
        [HttpPost]
        public IActionResult AddStaff(Staff staff, int? SpecialtyId, List<int> ServiceIds)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SalonDbContext())
                {
                    staff.Role = "Staff";

                    if (SpecialtyId.HasValue)
                    {
                        staff.Specialty = db.Services.FirstOrDefault(s => s.Id == SpecialtyId);
                    }

                    staff.Services = db.Services.Where(s => ServiceIds.Contains(s.Id)).ToList();

                    db.Set<Staff>().Add(staff);
                    db.SaveChanges();
                }

                TempData["SuccessMessage"] = "Staff member added successfully!";
                return RedirectToAction("Staff");
            }
            Console.WriteLine("Model state is invalid.");
            Console.WriteLine(staff.Id + " " + staff.Name + " " + staff.Surname + " " + staff.Email + " " + staff.Role + " ");
            using (var db = new SalonDbContext())
            {
                ViewBag.Services = db.Services.ToList();
            }

            return View(staff);
        }


        [HttpGet]
        public IActionResult DeleteStaff(int id)
        {
            using (var db = new SalonDbContext())
            {
                // Find the staff member
                var staff = db.Set<Staff>()
                              .Include(s => s.Services)
                              .FirstOrDefault(s => s.Id == id);

                if (staff == null)
                {
                    TempData["ErrorMessage"] = "Staff member not found.";
                    return RedirectToAction("Staff");
                }

                // Remove staff
                db.Set<Staff>().Remove(staff);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Staff member deleted successfully!";
                return RedirectToAction("Staff");
            }
        }


        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new SalonDbContext())
            {
                var staff = db.Set<Staff>()
                              .Include(s => s.Specialty)
                              .Include(s => s.Services)
                              .FirstOrDefault(s => s.Id == id);

                if (staff == null)
                {
                    TempData["ErrorMessage"] = "Staff member not found.";
                    return RedirectToAction("Staff");
                }

                ViewBag.Services = db.Services.ToList();
                return View(staff);
            }
        }
        [HttpPost]
        public IActionResult EditStaff(Staff staff, int? SpecialtyId, List<int> ServiceIds)
        {
            if (ModelState.IsValid)
            {
                using (var db = new SalonDbContext())
                {
                    var existingStaff = db.Set<Staff>()
                                          .Include(s => s.Services)
                                          .FirstOrDefault(s => s.Id == staff.Id);

                    if (existingStaff == null)
                    {
                        TempData["ErrorMessage"] = "Staff member not found.";
                        return RedirectToAction("Staff");
                    }

                    // Update basic staff details
                    existingStaff.Name = staff.Name;
                    existingStaff.Surname = staff.Surname;
                    existingStaff.Email = staff.Email;

                    // Update specialty
                    existingStaff.Specialty = SpecialtyId.HasValue ? db.Services.FirstOrDefault(s => s.Id == SpecialtyId) : null;

                    // Update services
                    existingStaff.Services.Clear(); // Clear existing relationships
                    if (ServiceIds != null && ServiceIds.Any())
                    {
                        existingStaff.Services = db.Services.Where(s => ServiceIds.Contains(s.Id)).ToList();
                    }

                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Staff member updated successfully!";
                    return RedirectToAction("Staff");
                }
            }
            using (var db = new SalonDbContext())
            {
                ViewBag.Services = db.Services.ToList();
            }
            return View(staff);
        }


        public IActionResult Appointments()
        {
            using (var db = new SalonDbContext())
            {
                var appointments = db.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Staff)
                    .Include(a => a.Customer)
                    .ToList();
                return View(appointments);
            }
        }


    }
}
