using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        [Required]
        public int UserId { get; set; }  // Relationship with User

        [Required]
        [MaxLength(100)]
        public string Specialty { get; set; }

        [Required]
        public int BranchId { get; set; }  // Relationship with Branch

        [Required]
        public string WorkingHours { get; set; }  // JSON

        public User User { get; set; }
        public Branch Branch { get; set; }
    }
}
