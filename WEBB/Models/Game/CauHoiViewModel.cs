namespace WEBB.Models.Game
{
    // Dùng để nhận 1 câu hỏi từ API (không chứa đáp án đúng)
    public class CauHoiViewModel
    {
        public int CauHoiID { get; set; }
        public int ChuDeID { get; set; }
        public int DoKhoID { get; set; }
        public string NoiDung { get; set; }
        public string DapAnA { get; set; }
        public string DapAnB { get; set; }
        public string DapAnC { get; set; }
        public string DapAnD { get; set; }
    }
}