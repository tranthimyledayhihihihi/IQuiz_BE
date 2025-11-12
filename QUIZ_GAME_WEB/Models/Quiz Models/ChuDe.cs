using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class ChuDe
    {
        [Key]
        public int ChuDeID { get; set; }

        [Required]
        [MaxLength(100)]
        public string TenChuDe { get; set; }

        [MaxLength(255)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true;

        public virtual ICollection<CauHoi> CauHois { get; set; }
    }
}