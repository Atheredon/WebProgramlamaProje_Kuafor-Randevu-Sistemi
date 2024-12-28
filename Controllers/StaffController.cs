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

            using (var db = new SalonDbContext())
            {
                var appointment = db.Appointments.FirstOrDefault(a => a.Id == appointmentId && a.StaffId == int.Parse(userId));
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Randevu bulunamadı ya da iptal etme yetkiniz yok.";
                    return RedirectToAction("ManageMyAppointments");
                }

                if (appointment.AppointmentDate <= DateTime.UtcNow)
                {
                    TempData["ErrorMessage"] = "Geçmiş veya devam eden randevuları iptal edemezsiniz.";
                    return RedirectToAction("ManageMyAppointments");
                }

                // Mark the appointment as cancelled
                appointment.Status = "Cancelled";
                db.SaveChanges();

                TempData["SuccessMessage"] = "Randevu başarıyla iptal edildi.";
                return RedirectToAction("ManageMyAppointments");
            }
        }

        [HttpPost]
        public IActionResult ConfirmAppointment(int appointmentId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            using (var db = new SalonDbContext())
            {
                var appointment = db.Appointments.FirstOrDefault(a => a.Id == appointmentId && a.StaffId == int.Parse(userId));
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "Randevu bulunamadı ya da iptal etme yetkiniz yok.";
                    return RedirectToAction("ManageMyAppointments");
                }

                if (appointment.AppointmentDate <= DateTime.UtcNow)
                {
                    TempData["ErrorMessage"] = "Geçmiş veya devam eden randevuları kabul edemezsiniz.";
                    return RedirectToAction("ManageMyAppointments");
                }

                // Mark the appointment as confirmed
                appointment.Status = "Confirmed";
                db.SaveChanges();

                TempData["SuccessMessage"] = "Randevu başarıyla onaylandı.";
                return RedirectToAction("ManageMyAppointments");
            }
        }

    }
}
