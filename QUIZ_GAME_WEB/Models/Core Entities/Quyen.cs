
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("Quyen")]
    public class Quyen
    {
        [Key]
        public int QuyenID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenQuyen { get; set; }

        [MaxLength(255)]
        public string MoTa { get; set; }

        // Navigation properties
        public virtual ICollection<VaiTroQuyen> VaiTroQuyens { get; set; }
    }
}