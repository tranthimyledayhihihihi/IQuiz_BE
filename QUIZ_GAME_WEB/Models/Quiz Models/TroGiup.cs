using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.QuizModels
{
    public class TroGiup
    {
        [Key]
        public int TroGiupID { get; set; }
        [MaxLength(50)]
        public string TenTroGiup { get; set; } // Ví dụ: "50/50", "Hỏi khán giả"
        [MaxLength(255)]
        public string MoTa { get; set; }
    }
}