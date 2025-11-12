namespace QUIZ_GAME_WEB.Models
{
    // ViewModel để hiển thị người dùng trong Bảng điều khiển Admin
    public class NguoiDungAdminViewModel
    {
        public int UserID { get; set; }
        public string? TenDangNhap { get; set; }
        public string? Email { get; set; }
        public string? HoTen { get; set; }
        public DateTime NgayDangKy { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool TrangThai { get; set; } // 1 = Hoạt động, 0 = Bị khóa
    }
}