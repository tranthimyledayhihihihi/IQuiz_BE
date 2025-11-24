using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Text.Json.Serialization;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/quiz/[controller]")]
    [ApiController]
    public class ChoiController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ChoiController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/quiz/Choi/Start?count=5&chuDeIds=1,2&doKhoIds=1,2
        // SỬA LỖI: Đặt tham số tùy chọn ở cuối cùng hoặc làm cho các tham số sau nó cũng là tùy chọn.
        [HttpGet("Start")]
        public async Task<ActionResult<IEnumerable<CauHoi>>> StartQuiz(
            [FromQuery] string? chuDeIds = null,  // <-- Đã làm tùy chọn
            [FromQuery] string? doKhoIds = null,  // <-- Đã làm tùy chọn
            [FromQuery] int count = 5)            // <-- Đặt tham số tùy chọn cuối cùng
        {
            // Logic Nghiệp vụ: Phân tích điều kiện lọc
            var cIds = chuDeIds?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => int.Parse(id)).ToList() ?? new List<int>();
            var dIds = doKhoIds?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id => int.Parse(id)).ToList() ?? new List<int>();

            // Logic kiểm tra điều kiện
            if (count <= 0 || !cIds.Any() || !dIds.Any())
            {
                // Logic nghiệp vụ: Nếu thiếu ID, coi như là thiếu điều kiện chơi hợp lệ
                return BadRequest("Vui lòng cung cấp ChuDeIds và DoKhoIds hợp lệ.");
            }

            // Truy vấn DAL (Lấy ngẫu nhiên)
            var quiz = await _context.CauHois
                .Where(ch => cIds.Contains(ch.ChuDeID) && dIds.Contains(ch.DoKhoID))
                .OrderBy(ch => Guid.NewGuid())
                .Take(count)
                .ToListAsync();

            // Logic bảo mật: ẨN đáp án đúng
            quiz.ForEach(q => q.DapAnDung = null);

            return Ok(quiz);
        }

        // POST: api/quiz/Choi/Submit
        [HttpPost("Submit")]
        public async Task<ActionResult<KetQua>> SubmitAnswer([FromBody] SubmitAnswerDto model)
        {
            if (model == null || model.UserID <= 0 || model.CauHoiID <= 0 || string.IsNullOrEmpty(model.UserDapAn))
            {
                return BadRequest("Dữ liệu gửi lên không hợp lệ.");
            }

            var cauHoi = await _context.CauHois.FindAsync(model.CauHoiID);
            if (cauHoi == null) return NotFound("Câu hỏi không tồn tại.");

            // Logic Nghiệp vụ: Kiểm tra và tính điểm
            bool isCorrect = string.Equals(cauHoi.DapAnDung, model.UserDapAn, StringComparison.OrdinalIgnoreCase);
            int diemThuong = 0;

            if (isCorrect)
            {
                var doKho = await _context.DoKhos.FindAsync(cauHoi.DoKhoID);
                diemThuong = doKho?.DiemThuong ?? 0;
            }

            // Ghi nhận kết quả
            var ketQua = new KetQua
            {
                UserID = model.UserID,
                Diem = diemThuong,
                SoCauDung = isCorrect ? 1 : 0,
                TongCauHoi = 1,
                TrangThai = isCorrect ? "Đúng" : "Sai",
                ThoiGian = DateTime.Now
            };
            _context.KetQuas.Add(ketQua);

            // Cập nhật Thống kê người dùng (Logic nghiệp vụ BUS)
            var today = DateTime.Today;
            var thongKe = await _context.ThongKeNguoiDungs.FirstOrDefaultAsync(t => t.UserID == model.UserID && t.Ngay == today);

            if (thongKe == null)
            {
                thongKe = new ThongKeNguoiDung { UserID = model.UserID, Ngay = today, SoTran = 1, SoCauDung = ketQua.SoCauDung, DiemTrungBinh = diemThuong };
                _context.ThongKeNguoiDungs.Add(thongKe);
            }
            else
            {
                thongKe.SoTran += 1;
                thongKe.SoCauDung += ketQua.SoCauDung;
                thongKe.DiemTrungBinh = ((thongKe.DiemTrungBinh * (thongKe.SoTran - 1)) + diemThuong) / thongKe.SoTran;
            }

            await _context.SaveChangesAsync();
            return Ok(ketQua);
        }
    }

    // DTO cho đầu vào SubmitAnswer
    public class SubmitAnswerDto
    {
        public int UserID { get; set; }
        public int CauHoiID { get; set; }
        public string UserDapAn { get; set; } // A, B, C, D
    }
}