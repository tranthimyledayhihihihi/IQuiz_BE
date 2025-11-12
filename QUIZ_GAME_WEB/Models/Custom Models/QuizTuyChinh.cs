using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models
{
    public class QuizTuyChinh
    {
        [Key]
        public int QuizTuyChinhID { get; set; }
        [Required]
        public int UserID { get; set; }
        [MaxLength(100)]
        public string TenQuiz { get; set; }
        [MaxLength(255)]
        public string MoTa { get; set; }
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}