
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("VaiTro")]
    public class VaiTro
    {
        [Key]
        public int VaiTroID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenVaiTro { get; set; }

        [MaxLength(255)]
        public string MoTa { get; set; }

        // Navigation properties
        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<VaiTroQuyen> VaiTroQuyens { get; set; }
    }
}
