namespace QUIZ_GAME_WEB.Models
{
    // Model này chỉ chứa thông tin an toàn để hiển thị
    public class CauHoiViewModel
    {
        public int CauHoiID { get; set; }
        public int ChuDeID { get; set; }
        public int DoKhoID { get; set; }
        public string? NoiDung { get; set; }
        public string? DapAnA { get; set; }
        public string? DapAnB { get; set; }
        public string? DapAnC { get; set; }
        public string? DapAnD { get; set; }
    }
}