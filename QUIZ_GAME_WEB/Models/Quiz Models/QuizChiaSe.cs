using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class QuizChiaSe
    {
        [Key]
        public int QuizChiaSeID { get; set; }
        [Required]
        public int QuizTuyChinhID { get; set; }
        [Required]
        public int UserGuiID { get; set; }
        public int? UserNhanID { get; set; } // Có thể null nếu chia sẻ public
        public DateTime NgayChiaSe { get; set; } = DateTime.Now;

        [ForeignKey("QuizTuyChinhID")]
        public virtual QuizTuyChinh QuizTuyChinh { get; set; }
        [ForeignKey("UserGuiID")]
        public virtual NguoiDung UserGui { get; set; }
        [ForeignKey("UserNhanID")]
        public virtual NguoiDung UserNhan { get; set; }
    }
}