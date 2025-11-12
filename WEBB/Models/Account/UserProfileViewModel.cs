using System;

namespace WEBB.Models.Account
{
    // Dùng để hiển thị thông tin User (lấy từ API /api/profile/me)
    public class UserProfileViewModel
    {
        public int UserID { get; set; }
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public string AnhDaiDien { get; set; }
        public DateTime NgayDangKy { get; set; }
    }
}