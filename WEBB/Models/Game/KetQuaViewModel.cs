using System;

namespace WEBB.Models.Game
{
    // Dùng để hiển thị kết quả cuối cùng (nhận từ API /api/choi/submit)
    public class KetQuaViewModel
    {
        public int KetQuaID { get; set; }
        public int Diem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCauHoi { get; set; }
        public string TrangThai { get; set; }
        public string ThongBao { get; set; }
        public DateTime ThoiGian { get; set; }
        public int UserID { get; set; }
    }
}