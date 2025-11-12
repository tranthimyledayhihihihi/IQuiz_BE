using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class AnswerSubmitModel
    {
        [Required]
        public int CauHoiID { get; set; }

        [Required]
        public string? DapAnChon { get; set; } // "A", "B", "C", "D"
    }
}