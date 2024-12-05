using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class Branch
    {
        public int BranchId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Address { get; set; }

        [Required]
        public string WorkingHours { get; set; }  // JSON
    }
}
