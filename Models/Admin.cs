using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        [Required]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
