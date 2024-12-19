using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult BookAppointment(int ServiceId, int StaffId, DateTime AppointmentDate)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please log in to book an appointment.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new SalonDbContext())
            {
                var service = db.Services.FirstOrDefault(s => s.Id == ServiceId);
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToAction("BookAppointment");
                }

                // Calculate end time of the service
                var appointmentEndTime = AppointmentDate.ToUniversalTime().AddMinutes(service.Duration);

                // Check if the selected staff has overlapping appointments
                var overlappingAppointments = db.Appointments
                    .Where(a => a.StaffId == StaffId &&
                                a.AppointmentDate < appointmentEndTime &&
                                a.AppointmentDate.AddMinutes(a.Service.Duration) > AppointmentDate.ToUniversalTime())
                    .Any();

                if (overlappingAppointments)
                {
                    TempData["ErrorMessage"] = "The selected staff is not available at this time.";
                    return RedirectToAction("BookAppointment");
                }

                // Check if the shop is open
                var openingTime = new TimeSpan(9, 0, 0); // 9:00 AM
                var closingTime = new TimeSpan(18, 0, 0); // 6:00 PM

                if (AppointmentDate.TimeOfDay < openingTime || appointmentEndTime.TimeOfDay > closingTime)
                {
                    TempData["ErrorMessage"] = "The appointment time is outside of shop hours.";
                    return RedirectToAction("BookAppointment");
                }

                // Create a new appointment
                var appointment = new Appointment
                {
                    ServiceId = ServiceId,
                    StaffId = StaffId,
                    AppointmentDate = AppointmentDate.ToUniversalTime(),
                    CustomerId = int.Parse(userId),
                    Status = "Pending"
                };

                db.Appointments.Add(appointment);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Appointment booked successfully!";
                return RedirectToAction("MyAppointments");
            }
        }


    }
}
