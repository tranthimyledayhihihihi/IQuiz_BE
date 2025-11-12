using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class ProfileUpdateModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100)]
        public string? HoTen { get; set; }

        [MaxLength(255)]
        public string? AnhDaiDien { get; set; } // URL to avatar
    }
}