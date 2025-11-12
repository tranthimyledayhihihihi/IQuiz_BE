using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class DoKho
    {
        [Key]
        public int DoKhoID { get; set; }
        [Required]
        [MaxLength(50)]
        public string TenDoKho { get; set; }
        public int DiemThuong { get; set; } = 0;

        public virtual ICollection<CauHoi> CauHois { get; set; }
    }
}