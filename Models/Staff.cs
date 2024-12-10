using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class Staff
    {
        [Key] // Primary Key
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Specialty { get; set; }


        public virtual User User { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
