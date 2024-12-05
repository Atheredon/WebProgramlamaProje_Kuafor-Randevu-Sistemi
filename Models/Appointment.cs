using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public User User { get; set; }
        public Employee Employee { get; set; }
        public Service Service { get; set; }
    }
}
