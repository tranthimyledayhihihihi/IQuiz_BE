using System.ComponentModel.DataAnnotations;

namespace WEBB.Models.Account
{
    // Dùng cho Form cập nhật Profile
    public class ProfileUpdateModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ và Tên")]
        public string HoTen { get; set; }

        [Display(Name = "Ảnh đại diện (URL)")]
        public string AnhDaiDien { get; set; }
    }
}