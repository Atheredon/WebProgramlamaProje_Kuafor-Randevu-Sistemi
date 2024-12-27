using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace KuaförRandevuSistemi.Models
{
    public class Staff : User
    {
        public int? SpecialtyId { get; set; }
        public virtual Service? Specialty { get; set; }
        public bool Available { get; set; } = true;

        public virtual ICollection<Service>? Services { get; set; }
    }


}
