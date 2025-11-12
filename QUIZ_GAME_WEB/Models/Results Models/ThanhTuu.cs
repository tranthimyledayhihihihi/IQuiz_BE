using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class ThanhTuu
    {
        [Key]
        public int ThanhTuuID { get; set; }
        [MaxLength(100)]
        // Sửa các dòng này
        public string? TenThanhTuu { get; set; }
        public string? MoTa { get; set; }
        public string? BieuTuong { get; set; }
        public string? DieuKien { get; set; }
    }
}