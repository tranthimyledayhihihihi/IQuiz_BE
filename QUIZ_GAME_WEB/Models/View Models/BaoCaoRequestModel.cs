using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    // Model để nhận yêu cầu tạo báo cáo
    public class BaoCaoRequestModel
    {
        // Ví dụ: "NguoiDung", "KetQua"
        [Required]
        public string? LoaiBaoCao { get; set; }

        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }
}