namespace WEBB.Models.Game
{
    // Dùng để gửi 1 câu trả lời (gửi tới API /api/choi/submit)
    public class AnswerSubmitModel
    {
        public int CauHoiID { get; set; }
        public string DapAnChon { get; set; }
    }
}