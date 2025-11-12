namespace WEBB.Models.Admin
{
    // Dùng để hiển thị danh sách câu hỏi (nhận từ API /api/QLCauHoi)
    public class CauHoiAdminViewModel
    {
        public int CauHoiID { get; set; }
        public string NoiDung { get; set; }
        public string TenChuDe { get; set; }
        public string TenDoKho { get; set; }
        public string DapAnDung { get; set; }
    }
}