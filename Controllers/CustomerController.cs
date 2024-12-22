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
                var openingTime = new TimeSpan(9, 0, 0); // 9:00 AM
                var closingTime = new TimeSpan(18, 0, 0); // 6:00 PM
                var slotDuration = 15; // 15 minutes
                var totalSlots = (int)((closingTime - openingTime).TotalMinutes / slotDuration); // Total number of slots

                // Initialize boolean list
                var available = new bool[totalSlots];
                for (int i = 0; i < totalSlots; i++)
                {
                    available[i] = true;
                }

                // Ensure date is UTC
                date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

                // Fetch all appointments for the selected staff on the given date
                var appointments = db.Appointments
                    .Where(a => a.StaffId == staffId && a.AppointmentDate.Date == date.Date)
                    .Select(a => new
                    {
                        StartIndex = (int)((a.AppointmentDate.TimeOfDay.Subtract(openingTime)).TotalMinutes / slotDuration),
                        SlotCount = (int)Math.Ceiling(a.Service.Duration / (double)slotDuration)
                    })
                    .ToList();

                // Mark slots as unavailable
                foreach (var appointment in appointments)
                {
                    Console.WriteLine(appointment.SlotCount + "  " + appointment.StartIndex);
                    for (int i = 0; i < appointment.SlotCount; i++)
                    {
                        int indexToMark = appointment.StartIndex + i;
                        Console.WriteLine(indexToMark + "  " + appointment.StartIndex);
                        if (indexToMark >= 0 && indexToMark < totalSlots)
                        {
                            available[indexToMark] = false;
                        }
                    }
                }

                // Check for consecutive available slots
                var availableStartTimes = new List<TimeSpan>();
                for (int i = 0; i <= totalSlots - (duration / slotDuration); i++)
                {
                    bool isAvailable = true;
                    for (int j = 0; j < duration / slotDuration; j++)
                    {
                        if (!available[i + j])
                        {
                            isAvailable = false;
                            break;
                        }
                    }

                    if (isAvailable)
                    {
                        availableStartTimes.Add(openingTime.Add(TimeSpan.FromMinutes(i * slotDuration)));
                    }
                }

                // Return available start times as "hh:mm"
                foreach (var musaitlik in available) Console.WriteLine(musaitlik);
                return Json(availableStartTimes.Select(s => s.ToString(@"hh\:mm")));
            }
        }





    }
}
