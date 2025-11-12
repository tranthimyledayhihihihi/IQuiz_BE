using System.ComponentModel.DataAnnotations;

namespace WEBB.Models.Admin
{
    // Dùng cho Form TẠO/SỬA Chủ đề
    public class ChuDeCrudModel
    {
        public int ChuDeID { get; set; }

        [Required(ErrorMessage = "Tên chủ đề là bắt buộc")]
        [Display(Name = "Tên chủ đề")]
        public string TenChuDe { get; set; }

        [Display(Name = "Mô tả")]
        public string MoTa { get; set; }

        [Display(Name = "Trạng thái (Hiện/Ẩn)")]
        public bool TrangThai { get; set; } = true;
    }
}