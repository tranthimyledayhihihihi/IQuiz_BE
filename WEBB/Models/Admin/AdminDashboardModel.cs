namespace WEBB.Models.Admin
{
    // Dùng để hiển thị Dashboard (nhận từ API /api/admin/dashboard)
    public class AdminDashboardModel
    {
        public int TongSoNguoiDung { get; set; }
        public int NguoiDungMoiHomNay { get; set; }
        public int TongSoCauHoi { get; set; }
        public int TongSoChuDe { get; set; }
        public int SoTranDauHomNay { get; set; }
    }
}