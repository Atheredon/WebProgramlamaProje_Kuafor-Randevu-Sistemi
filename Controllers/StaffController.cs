using KuaförRandevuSistemi.Filters;
using KuaförRandevuSistemi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KuaförRandevuSistemi.Controllers
{
    [RoleAuthorize("Staff")]
    public class StaffController : BaseController
    {
        [HttpGet]
        public IActionResult ManageMyAppointments()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please log in to view your appointments.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new SalonDbContext())
            {
                var staffId = int.Parse(userId);

                var appointments = db.Appointments
                    .Include(a => a.Service)
                    .Include(a => a.Customer)
                    .Where(a => a.StaffId == staffId && a.Status != "Cancelled")
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();

                return View(appointments);
            }
        }

        [HttpPost]
        public IActionResult CancelAppointment(int appointmentId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please log in to manage appointments.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new SalonDbContext())
            {
                var appointment = db.Appointments.FirstOrDefault(a => a.Id == appointmentId && a.StaffId == int.Parse(userId));
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found or you are not authorized to cancel it.";
                    return RedirectToAction("ManageMyAppointments");
                }

                if (appointment.AppointmentDate <= DateTime.UtcNow)
                {
                    TempData["ErrorMessage"] = "You cannot cancel past or ongoing appointments.";
                    return RedirectToAction("ManageMyAppointments");
                }

                // Mark the appointment as cancelled
                appointment.Status = "Cancelled";
                db.SaveChanges();

                TempData["SuccessMessage"] = "Appointment canceled successfully.";
                return RedirectToAction("ManageMyAppointments");
            }
        }

        [HttpPost]
        public IActionResult ConfirmAppointment(int appointmentId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Please log in to manage appointments.";
                return RedirectToAction("Login", "Account");
            }

            using (var db = new SalonDbContext())
            {
                var appointment = db.Appointments.FirstOrDefault(a => a.Id == appointmentId && a.StaffId == int.Parse(userId));
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Appointment not found or you are not authorized to confirm it.";
                    return RedirectToAction("ManageMyAppointments");
                }

                if (appointment.AppointmentDate <= DateTime.UtcNow)
                {
                    TempData["ErrorMessage"] = "You cannot confirm past or ongoing appointments.";
                    return RedirectToAction("ManageMyAppointments");
                }

                // Mark the appointment as confirmed
                appointment.Status = "Confirmed";
                db.SaveChanges();

                TempData["SuccessMessage"] = "Appointment confirmed successfully.";
                return RedirectToAction("ManageMyAppointments");
            }
        }

    }
}
