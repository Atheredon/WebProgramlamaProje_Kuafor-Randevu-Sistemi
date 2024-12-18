using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class Staff : User
    {
        [Required]
        [StringLength(100)]
        public virtual Service Specialty { get; set; }

        [Required]
        public int? SpecialtyId { get; set; } // Foreign key
        public virtual ICollection<Service> Services { get; set; }
    }

}
