using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuaförRandevuSistemi.Controllers
{
    public class CustomerController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BookAppointment()
        {
            using (var db = new SalonDbContext())
            {
                ViewBag.Services = db.Services.ToList();
                ViewBag.Staff = db.Users.OfType<Staff>().ToList();
            }
            return View();
        }
        [HttpPost]
        public IActionResult BookAppointment(int ServiceId, int StaffId, DateTime AppointmentDate, string TimeSlot)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["ErrorMessage"] = "Please log in to book an appointment.";
                    return RedirectToAction("Login", "Account");
                }

                // Ensure valid date and time slot
                if (AppointmentDate == default(DateTime) || string.IsNullOrEmpty(TimeSlot))
                {
                    TempData["ErrorMessage"] = "Please select a valid date and time slot.";
                    return RedirectToAction("BookAppointment");
                }

                var selectedTime = DateTime.Parse($"{AppointmentDate:yyyy-MM-dd} {TimeSlot}").ToUniversalTime();

                using (var db = new SalonDbContext())
                {
                    var service = db.Services.FirstOrDefault(s => s.Id == ServiceId);
                    if (service == null)
                    {
                        TempData["ErrorMessage"] = "Service not found.";
                        return RedirectToAction("BookAppointment");
                    }

                    // Create a new appointment
                    var appointment = new Appointment
                    {
                        ServiceId = ServiceId,
                        StaffId = StaffId,
                        AppointmentDate = selectedTime,
                        CustomerId = int.Parse(userId),
                        Status = "Pending"
                    };

                    db.Appointments.Add(appointment);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Appointment booked successfully!";
                    return RedirectToAction("MyAppointments");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error booking appointment: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while booking the appointment.";
                return RedirectToAction("BookAppointment");
            }
        }


        [HttpGet]
        public JsonResult GetAvailableSlots(int staffId, DateTime date, int duration)
        {
            using (var db = new SalonDbContext())
            {
                var staff = db.Staffs.FirstOrDefault(s => s.Id == staffId);
                if (staff == null || !staff.Available)
                {
                    return Json(new List<string>()); // No slots available
                }
                int slotDuration = 15; // 15-minute intervals
                var openingTime = new TimeSpan(9, 0, 0); // 9:00 AM
                var closingTime = new TimeSpan(18, 0, 0); // 6:00 PM
                int totalSlots = (int)((closingTime - openingTime).TotalMinutes / slotDuration); // Total number of slots

                // Initialize boolean list for availability
                var available = new bool[totalSlots];
                for (int i = 0; i < totalSlots; i++)
                {
                    available[i] = true;
                }

                // Convert input date to UTC
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

                // Fetch all appointments for the selected staff on the given date
                var appointments = db.Appointments
                    .Where(a => a.StaffId == staffId && a.AppointmentDate.Date == date.Date && a.Status != "Cancelled")
                    .Select(a => new
                    {
                        StartTime = a.AppointmentDate.ToLocalTime().TimeOfDay,
                        Duration = a.Service.Duration
                    });

                // Mark slots as unavailable
                foreach (var appointment in appointments)
                {
                    int startIndex = (int)((appointment.StartTime - openingTime).TotalMinutes / slotDuration);
                    int slotsToMark = (int)Math.Ceiling(appointment.Duration / (double)slotDuration);

                    for (int i = 0; i < slotsToMark; i++)
                    {
                        int indexToMark = startIndex + i;
                        if (indexToMark >= 0 && indexToMark < totalSlots)
                        {
                            available[indexToMark] = false;
                        }
                    }
                }

                // Check for consecutive available slots
                var availableStartTimes = new List<int>();
                for (int i = 0; i <= totalSlots - (duration / slotDuration); i++)
                {
                    bool isAvailable = true;

                    // Check all slots needed for this service
                    for (int j = 0; j < duration / slotDuration; j++)
                    {
                        if (i + j >= totalSlots || !available[i + j])
                        {
                            isAvailable = false;
                            break;
                        }
                    }

                    if (isAvailable)
                    {
                        // Ensure no collisions with adjacent appointments
                        int endSlot = i + (duration / slotDuration) - 1;
                        if (endSlot < totalSlots && available[endSlot])
                        {
                            availableStartTimes.Add((int)(openingTime.TotalMinutes + i * slotDuration));
                        }
                    }
                }


                // Convert available start times to "HH:mm" format
                return Json(availableStartTimes.Select(t =>
                {
                    int hours = t / 60;
                    int minutes = t % 60;
                    return $"{hours:D2}:{minutes:D2}";
                }));
            }
        }


        public IActionResult MyAppointments()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please log in to view your appointments.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new SalonDbContext())
            {
                var customerId = int.Parse(userId);
                var now = DateTime.UtcNow;

                // Fetch future appointments with related data
                var futureAppointments = db.Appointments
                    .Include(a => a.Service) // Include Service details
                    .Include(a => a.Staff)   // Include Staff details
                    .Where(a => a.CustomerId == customerId && a.AppointmentDate > now)
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();

                // Fetch past appointments with related data
                var pastAppointments = db.Appointments
                    .Include(a => a.Service) // Include Service details
                    .Include(a => a.Staff)   // Include Staff details
                    .Where(a => a.CustomerId == customerId && a.AppointmentDate <= now)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToList();

                ViewData["FutureAppointments"] = futureAppointments;
                ViewData["PastAppointments"] = pastAppointments;
            }

            return View();
        }


        [HttpPost]
        public IActionResult CancelAppointment(int appointmentId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please log in to manage your appointments.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new SalonDbContext())
            {
                var appointment = db.Appointments.FirstOrDefault(a => a.Id == appointmentId && a.CustomerId == int.Parse(userId));
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found.";
                    return RedirectToAction("MyAppointments");
                }

                if (appointment.AppointmentDate <= DateTime.UtcNow)
                {
                    TempData["ErrorMessage"] = "You cannot cancel a past or ongoing appointment.";
                    return RedirectToAction("MyAppointments");
                }

                appointment.Status = "Cancelled";
                db.SaveChanges();

                TempData["SuccessMessage"] = "Appointment canceled successfully.";
                return RedirectToAction("MyAppointments");
            }
        }



    }
}
