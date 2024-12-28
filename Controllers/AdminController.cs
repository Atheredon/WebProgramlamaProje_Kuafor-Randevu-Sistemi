using KuaförRandevuSistemi.Filters;
using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuaförRandevuSistemi.Controllers
{
    [RoleAuthorize("Admin")]
    public class AdminController : BaseController
    {
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

                TempData["SuccessMessage"] = service.Name + " servisi başarıyla eklendi!";
                return RedirectToAction("Services");
            }

            return View(service);
        }

        [HttpGet]
        public IActionResult DeleteService(int id)
        {
            using (var db = new SalonDbContext())
            {
                // Fetch the service
                var service = db.Services.FirstOrDefault(s => s.Id == id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Servis bulunamadı.";
                    return RedirectToAction("Services");
                }

                // Check if the service is associated with any appointments
                var hasAppointments = db.Appointments.Any(a => a.ServiceId == id);
                if (hasAppointments)
                {
                    TempData["ErrorMessage"] = $"Servis '{service.Name}' silinemez çünkü başka bir randevuyla bağlantılı.";
                    return RedirectToAction("Services");
                }

                // Remove the service
                db.Services.Remove(service);
                db.SaveChanges();

                TempData["SuccessMessage"] = $"Servis '{service.Name}' başarıyla silindi!";
            }

            return RedirectToAction("Services");
        }


        [HttpGet]
        public IActionResult EditService(int id)
        {
            using (var db = new SalonDbContext())
            {
                var service = db.Services.FirstOrDefault(s => s.Id == id);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Servis bulunamadı.";
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
                        TempData["SuccessMessage"] = service.Name + " Servisi başarıyla değiştirildi!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Servis bulunamadı.";
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
                        TempData["ErrorMessage"] = "Bu Email çoktan kayıtlı.";
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

                TempData["SuccessMessage"] = "Çalışan " +staff.Name + " " +staff.Surname + " başarıyla eklendi!";
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
                    TempData["ErrorMessage"] = "Çalışan bulunamadı.";
                    return RedirectToAction("Staff");
                }

                // Remove staff
                db.Set<Staff>().Remove(staff);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Çalışan başarıyla silindi!";
                return RedirectToAction("Staff");
            }
        }


        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            using (var db = new SalonDbContext())
            {
                var staff = db.Set<Staff>()
                              .Include(s => s.Specialty)
                              .Include(s => s.Services)
                              .FirstOrDefault(s => s.Id == id);

                if (staff == null)
                {
                    TempData["ErrorMessage"] = "Çalışan bulunamadı.";
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
                        TempData["ErrorMessage"] = "Çalışan bulunamadı.";
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

                    TempData["SuccessMessage"] = "Çalışan bilgileri başarıyla güncellendi!";
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
                    TempData["ErrorMessage"] = "Çalışan bulunamadı.";
                    return RedirectToAction("Staff");
                }

                // Toggle availability
                staff.Available = !staff.Available;
                db.SaveChanges();

                return RedirectToAction("Staff");
            }
        }

        [HttpGet]
        public IActionResult Analytics()
        {
            using (var db = new SalonDbContext())
            {
                var now = DateTime.UtcNow; // Use UTC explicitly

                //Appointments by time
                var appointmentsAllTimes = (from app in db.Appointments where (app.AppointmentDate <= now) select app).ToList();
                var appointmentsThisWeek = (from app in appointmentsAllTimes
                                            where (app.AppointmentDate.Year == now.Year && app.AppointmentDate.Month == now.Month && 
                                             now.Date - app.AppointmentDate.Date <= new TimeSpan(7,0,0,0)) 
                                            select app).ToList();
                var appointmentsThisMonth = (from app in appointmentsAllTimes where (app.AppointmentDate.Year == now.Year && app.AppointmentDate.Month == now.Month) select app).ToList();

                // Total appointments
                var totalAppointments = appointmentsAllTimes.Count();
                var weeklyAppointments = appointmentsThisWeek.Count();
                var monthlyAppointments = appointmentsThisMonth.Count();

                // Total revenue
                var totalRevenue = 0;
                foreach( var appointment in (from app in appointmentsAllTimes join srvs in db.Services on app.ServiceId equals srvs.Id where (app.Status == "Confirmed") select app).ToList())
                {
                    totalRevenue += appointment.Service.Price;
                }

                var weeklyRevenue = 0;
                foreach (var appointment in (from app in appointmentsThisWeek join srvs in db.Services on app.ServiceId equals srvs.Id where (app.Status == "Confirmed") select app).ToList())
                {
                    weeklyRevenue += appointment.Service.Price;
                }

                var monthlyRevenue = 0;
                foreach (var appointment in (from app in appointmentsThisMonth join srvs in db.Services on app.ServiceId equals srvs.Id where(app.Status == "Confirmed") select app).ToList())
                {
                    monthlyRevenue += appointment.Service.Price;
                }

                // Popular services
                var popularServices = (from app in appointmentsAllTimes
                                       join service in db.Services on app.ServiceId equals service.Id
                                       group service by service.Name into serviceGroup
                                       orderby serviceGroup.Count() descending
                                       select new
                                       {
                                           ServiceName = serviceGroup.Key,
                                           Bookings = serviceGroup.Count()
                                       })
                                       .Take(5)
                                       .ToList();

                var weeklyPopularServices = (from app in appointmentsThisWeek
                                             join service in db.Services on app.ServiceId equals service.Id
                                             group service by service.Name into serviceGroup
                                             orderby serviceGroup.Count() descending
                                             select new
                                             {
                                                 ServiceName = serviceGroup.Key,
                                                 Bookings = serviceGroup.Count()
                                             })
                                             .Take(5)
                                             .ToList();

                var monthlyPopularServices = (from app in appointmentsThisMonth
                                              join service in db.Services on app.ServiceId equals service.Id
                                              group service by service.Name into serviceGroup
                                              orderby serviceGroup.Count() descending
                                              select new
                                              {
                                                  ServiceName = serviceGroup.Key,
                                                  Bookings = serviceGroup.Count()
                                              })
                                              .Take(5)
                                              .ToList();


                // Staff performance
                var staffPerformance = (from app in appointmentsAllTimes
                                        join staff in db.Staffs on app.StaffId equals staff.Id
                                        where app.Status == "Confirmed"
                                        group staff by new { staff.Name, staff.Surname } into staffGroup
                                        orderby staffGroup.Count() descending
                                        select new
                                        {
                                            StaffName = staffGroup.Key.Name + " " + staffGroup.Key.Surname,
                                            Appointments = staffGroup.Count()
                                        })
                                        .ToList();

                var weeklyStaffPerformance = (from app in appointmentsThisWeek
                                              join staff in db.Staffs on app.StaffId equals staff.Id
                                              where app.Status == "Confirmed"
                                              group staff by new { staff.Name, staff.Surname } into staffGroup
                                              orderby staffGroup.Count() descending
                                              select new
                                              {
                                                  StaffName = staffGroup.Key.Name + " " + staffGroup.Key.Surname,
                                                  Appointments = staffGroup.Count()
                                              })
                                              .ToList();

                var monthlyStaffPerformance = (from app in appointmentsThisMonth
                                               join staff in db.Staffs on app.StaffId equals staff.Id
                                               where app.Status == "Confirmed"
                                               group staff by new { staff.Name, staff.Surname } into staffGroup
                                               orderby staffGroup.Count() descending
                                               select new
                                               {
                                                   StaffName = staffGroup.Key.Name + " " + staffGroup.Key.Surname,
                                                   Appointments = staffGroup.Count()
                                               })
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
                    TempData["ErrorMessage"] = "Müşteri bulunamadı.";
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
