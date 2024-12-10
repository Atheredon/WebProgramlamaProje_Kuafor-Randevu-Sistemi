using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(5, 240)] //minutes
        public int Duration { get; set; }

        [Required]
        public int Price { get; set; }

        
        public virtual ICollection<Staff> Staff { get; set; }
    }
}
