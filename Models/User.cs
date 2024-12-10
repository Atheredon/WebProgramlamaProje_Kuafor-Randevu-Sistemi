using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(10)] // Role can be "Admin", "Staff", or "Customer"
        public string Role { get; set; }
    }
}
