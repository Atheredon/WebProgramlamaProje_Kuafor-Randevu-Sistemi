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
                    if (db.Users.Any(u => u.Email == staff.Email))
                    {
                        TempData["ErrorMessage"] = "This email is already registered.";
                        return View(staff);
                    }

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

        [HttpPost]
        public IActionResult ToggleAvailability(int staffId)
        {
            using (var db = new SalonDbContext())
            {
                var staff = db.Staffs.FirstOrDefault(s => s.Id == staffId);
                if (staff == null)
                {
                    TempData["ErrorMessage"] = "Staff not found.";
                    return RedirectToAction("Staff");
                }

                // Toggle availability
                staff.Available = !staff.Available;
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Staff availability updated successfully. {(staff.Available ? "Available" : "Unavailable")}";
                return RedirectToAction("Staff");
            }
        }

        [HttpGet]
        public IActionResult Analytics()
        {
            using (var db = new SalonDbContext())
            {
                var now = DateTime.UtcNow; // Use UTC explicitly
                var startOfWeek = DateTime.SpecifyKind(now.AddDays(-(int)now.DayOfWeek), DateTimeKind.Utc); // Start of week in UTC
                var startOfMonth = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, 1), DateTimeKind.Utc); // Start of month in UTC

                // Total appointments
                var totalAppointments = db.Appointments.Count();
                var weeklyAppointments = db.Appointments
                    .Where(a => a.AppointmentDate >= startOfWeek && a.AppointmentDate <= now)
                    .Count();
                var monthlyAppointments = db.Appointments
                    .Where(a => a.AppointmentDate >= startOfMonth && a.AppointmentDate <= now)
                    .Count();

                // Total revenue
                var totalRevenue = db.Appointments
                    .Where(a => a.Status == "Confirmed")
                    .Sum(a => a.Service.Price);
                var weeklyRevenue = db.Appointments
                    .Where(a => (a.Status == "Confirmed") && (a.AppointmentDate >= startOfWeek && a.AppointmentDate <= now))
                    .Sum(a => a.Service.Price);
                var monthlyRevenue = db.Appointments
                    .Where(a => (a.Status == "Confirmed") && (a.AppointmentDate >= startOfMonth && a.AppointmentDate <= now))
                    .Sum(a => a.Service.Price);

                // Popular services
                var popularServices = db.Services
                    .Select(s => new
                    {
                        ServiceName = s.Name,
                        Bookings = db.Appointments.Count(a => a.ServiceId == s.Id)
                    })
                    .OrderByDescending(s => s.Bookings)
                    .Take(5)
                    .ToList();
                var weeklyPopularServices = db.Services
                    .Select(s => new
                    {
                        ServiceName = s.Name,
                        Bookings = db.Appointments.Count(a => a.ServiceId == s.Id && a.AppointmentDate >= startOfWeek && a.AppointmentDate <= now && a.Status == "Confirmed")
                    })
                    .OrderByDescending(s => s.Bookings)
                    .Take(5)
                    .ToList();
                var monthlyPopularServices = db.Services
                    .Select(s => new
                    {
                        ServiceName = s.Name,
                        Bookings = db.Appointments.Count(a => a.ServiceId == s.Id && a.AppointmentDate >= startOfMonth && a.AppointmentDate <= now && a.Status == "Confirmed")
                    })
                    .OrderByDescending(s => s.Bookings)
                    .Take(5)
                    .ToList();

                // Staff performance
                var staffPerformance = db.Staffs
                    .Select(s => new
                    {
                        StaffName = s.Name + " " + s.Surname,
                        Appointments = db.Appointments.Count(a => a.StaffId == s.Id)
                    })
                    .OrderByDescending(s => s.Appointments)
                    .ToList();
                var weeklyStaffPerformance = db.Staffs
                    .Select(s => new
                    {
                        StaffName = s.Name + " " + s.Surname,
                        Appointments = db.Appointments.Count(a => a.StaffId == s.Id && a.AppointmentDate >= startOfWeek && a.AppointmentDate <= now && a.Status == "Confirmed")
                    })
                    .OrderByDescending(s => s.Appointments)
                    .ToList();
                var monthlyStaffPerformance = db.Staffs
                    .Select(s => new
                    {
                        StaffName = s.Name + " " + s.Surname,
                        Appointments = db.Appointments.Count(a => a.StaffId == s.Id && a.AppointmentDate >= startOfMonth && a.AppointmentDate <= now && a.Status == "Confirmed")
                    })
                    .OrderByDescending(s => s.Appointments)
                    .ToList();

                // Pass data to the view
                ViewBag.TotalAppointments = totalAppointments;
                ViewBag.WeeklyAppointments = weeklyAppointments;
                ViewBag.MonthlyAppointments = monthlyAppointments;

                ViewBag.TotalRevenue = totalRevenue;
                ViewBag.WeeklyRevenue = weeklyRevenue;
                ViewBag.MonthlyRevenue = monthlyRevenue;

                ViewBag.PopularServices = popularServices;
                ViewBag.WeeklyPopularServices = weeklyPopularServices;
                ViewBag.MonthlyPopularServices = monthlyPopularServices;

                ViewBag.StaffPerformance = staffPerformance;
                ViewBag.WeeklyStaffPerformance = weeklyStaffPerformance;
                ViewBag.MonthlyStaffPerformance = monthlyStaffPerformance;

                return View();
            }
        }


        [HttpGet]
        public IActionResult Customers()
        {
            using (var db = new SalonDbContext())
            {
                var customers = db.Users
                    .Where(u => u.Role == "Customer")
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Surname,
                        c.Email
                    })
                    .ToList();

                return View(customers);
            }
        }

        [HttpGet]
        public IActionResult CustomerDetails(int customerId)
        {
            using (var db = new SalonDbContext())
            {
                var customer = db.Users.FirstOrDefault(u => u.Id == customerId && u.Role == "Customer");
                if (customer == null)
                {
                    TempData["ErrorMessage"] = "Customer not found.";
                    return RedirectToAction("Customers");
                }

                var appointments = db.Appointments
                    .Where(a => a.CustomerId == customerId)
                    .Include(a => a.Service)
                    .Include(a => a.Staff)
                    .ToList();

                ViewBag.Customer = customer;
                return View(appointments);
            }
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
