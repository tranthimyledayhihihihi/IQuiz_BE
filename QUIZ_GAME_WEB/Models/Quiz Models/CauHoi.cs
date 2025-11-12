using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models
{
    public class CauHoi
    {
        [Key]
        public int CauHoiID { get; set; }
        [Required]
        public int ChuDeID { get; set; }
        [Required]
        public int DoKhoID { get; set; }
        [Required]
        [MaxLength(500)]
        public string NoiDung { get; set; }
        [MaxLength(255)]
        // Sửa các dòng này
        public string? DapAnA { get; set; }
        public string? DapAnB { get; set; }
        public string? DapAnC { get; set; }
        public string? DapAnD { get; set; }
        public string? DapAnDung { get; set; }
        [ForeignKey("ChuDeID")]
        public virtual ChuDe ChuDe { get; set; }
        [ForeignKey("DoKhoID")]
        public virtual DoKho DoKho { get; set; }
    }
}