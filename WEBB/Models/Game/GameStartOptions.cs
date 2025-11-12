namespace WEBB.Models.Game
{
    // Dùng để gửi yêu cầu bắt đầu game (gửi tới API /api/choi/start)
    public class GameStartOptions
    {
        public int ChuDeID { get; set; }
        public int DoKhoID { get; set; }
        public int SoLuongCauHoi { get; set; }
    }
}