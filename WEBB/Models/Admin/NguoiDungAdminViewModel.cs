using System;

namespace WEBB.Models.Admin
{
    // Dùng để hiển thị danh sách người dùng (nhận từ API /api/QLNguoiDung)
    public class NguoiDungAdminViewModel
    {
        public int UserID { get; set; }
        public string TenDangNhap { get; set; }
        public string Email { get; set; }
        public string HoTen { get; set; }
        public DateTime NgayDangKy { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool TrangThai { get; set; }
    }
}