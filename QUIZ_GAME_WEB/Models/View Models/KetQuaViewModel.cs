using System;

namespace QUIZ_GAME_WEB.Models
{
    // Model này dùng để hiển thị kết quả sau khi chơi
    public class KetQuaViewModel
    {
        public int KetQuaID { get; set; }
        public int UserID { get; set; }
        public string TenNguoiDung { get; set; }
        public int Diem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCauHoi { get; set; }
        public DateTime ThoiGian { get; set; }
        public string ThongBao { get; set; } // "Chúc mừng bạn đã thắng!"
    }
}