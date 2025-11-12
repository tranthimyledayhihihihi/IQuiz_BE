using System;
using System.Collections.Generic;

namespace QUIZ_GAME_WEB.Models
{
    // Model dùng để tạo và xuất báo cáo
    public class BaoCaoModel
    {
        public string TenBaoCao { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public List<string> CotDuLieu { get; set; }
        public List<List<string>> DongDuLieu { get; set; }
    }
}