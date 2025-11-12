using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models
{
    public class CauSai
    {
        [Key]
        public int CauSaiID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int CauHoiID { get; set; }
        public DateTime NgaySai { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; }
        [ForeignKey("CauHoiID")]
        public virtual CauHoi CauHoi { get; set; }
    }
}