using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChoiController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ChoiController(QuizGameContext context)
        {
            _context = context;
        }

        // POST: api/choi/start
        // (Hàm này giữ nguyên code của bạn)
        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] GameStartRequest options)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var randomQuestions = await _context.CauHois
                    .Where(c => c.ChuDeID == options.ChuDeID && c.DoKhoID == options.DoKhoID)
                    .OrderBy(c => Guid.NewGuid()) // Giữ nguyên code của bạn
                    .Take(options.SoLuongCauHoi)
                    .Select(q => new QuestionResponse
                    {
                        CauHoiID = q.CauHoiID,
                        ChuDeID = q.ChuDeID,
                        DoKhoID = q.DoKhoID,
                        NoiDung = q.NoiDung,
                        DapAnA = q.DapAnA,
                        DapAnB = q.DapAnB,
                        DapAnC = q.DapAnC,
                        DapAnD = q.DapAnD
                    })
                    .ToListAsync();

                if (randomQuestions == null || !randomQuestions.Any())
                    return NotFound(new { message = "Không tìm thấy câu hỏi nào cho chủ đề hoặc độ khó này." });

                return Ok(new
                {
                    success = true,
                    questions = randomQuestions,
                    userId = userId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        // === NÂNG CẤP TOÀN BỘ HÀM SUBMIT ===
        // POST: api/choi/submit
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAnswers([FromBody] List<AnswerRequest> answers)
        {
            var userId_nullable = GetUserIdFromToken();
            if (userId_nullable == null)
                return Unauthorized(new { message = "Token không hợp lệ hoặc đã hết hạn." });

            int userId = userId_nullable.Value; // Đã có userId an toàn

            if (answers == null || !answers.Any())
                return BadRequest(new { message = "Không có câu trả lời nào được gửi lên." });

            // === BẮT ĐẦU TRANSACTION ===
            // Đảm bảo tất cả 5 bước (KetQua, CauSai, BXH...) CÙNG THÀNH CÔNG
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // (Giữ nguyên logic chấm điểm của bạn)
                var questionIds = answers.Select(a => a.CauHoiID).ToList();
                var correctAnswersList = await _context.CauHois
                    .Where(q => questionIds.Contains(q.CauHoiID))
                    .Select(q => new { CauHoiID = q.CauHoiID, DapAnDung = q.DapAnDung, DoKhoID = q.DoKhoID })
                    .ToListAsync();

                if (!correctAnswersList.Any())
                    return NotFound(new { message = "Không tìm thấy câu hỏi nào trong hệ thống." });

                var correctAnswers = correctAnswersList.ToDictionary(x => x.CauHoiID, x => new { x.DapAnDung, x.DoKhoID });

                int doKhoId = correctAnswers.Values.First().DoKhoID;
                var doKho = await _context.DoKhos.FindAsync(doKhoId);
                int diemMoiCau = doKho?.DiemThuong ?? 10;

                // === CHẤM ĐIỂM + LOGIC MỚI (LƯU CÂU SAI) ===
                int soCauDung = 0;
                var cauSaiList = new List<CauSai>(); // <-- LOGIC MỚI

                foreach (var answer in answers)
                {
                    if (correctAnswers.TryGetValue(answer.CauHoiID, out var correct))
                    {
                        if (!string.IsNullOrWhiteSpace(answer.DapAnChon) &&
                            !string.IsNullOrWhiteSpace(correct.DapAnDung) &&
                            string.Equals(answer.DapAnChon.Trim(), correct.DapAnDung.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            soCauDung++;
                        }
                        else
                        {
                            // <-- LOGIC MỚI 1: LƯU CÂU SAI
                            cauSaiList.Add(new CauSai { UserID = userId, CauHoiID = answer.CauHoiID, NgaySai = DateTime.Now });
                        }
                    }
                }
                int tongDiem = soCauDung * diemMoiCau;

                // (Giữ nguyên logic tạo KetQua của bạn)
                var ketQua = new KetQua
                {
                    UserID = userId,
                    Diem = tongDiem,
                    SoCauDung = soCauDung,
                    TongCauHoi = answers.Count,
                    TrangThai = (soCauDung >= (answers.Count + 1) / 2) ? "Thắng" : "Thua",
                    ThoiGian = DateTime.Now
                };
                _context.KetQuas.Add(ketQua);

                // === LOGIC MỚI 1 (Tiếp): THÊM CÂU SAI VÀO CONTEXT ===
                if (cauSaiList.Any())
                {
                    await _context.CauSais.AddRangeAsync(cauSaiList);
                }

                // === LOGIC MỚI 2: CẬP NHẬT THỐNG KÊ NGÀY ===
                var todayStats = await _context.ThongKeNguoiDungs
                    .FirstOrDefaultAsync(t => t.UserID == userId && t.Ngay == DateTime.Today);
                if (todayStats != null)
                {
                    todayStats.SoTran++;
                    todayStats.SoCauDung += soCauDung;
                }
                else
                {
                    _context.ThongKeNguoiDungs.Add(new ThongKeNguoiDung
                    {
                        UserID = userId,
                        Ngay = DateTime.Today,
                        SoTran = 1,
                        SoCauDung = soCauDung,
                        DiemTrungBinh = tongDiem
                    });
                }

                // === LOGIC MỚI 3: CẬP NHẬT BẢNG XẾP HẠNG ===
                var bxh = await _context.BXHs.FirstOrDefaultAsync(b => b.UserID == userId);
                if (bxh != null)
                {
                    bxh.DiemTuan += tongDiem;
                    bxh.DiemThang += tongDiem;
                }
                else
                {
                    _context.BXHs.Add(new BXH
                    {
                        UserID = userId,
                        DiemTuan = tongDiem,
                        DiemThang = tongDiem
                    });
                }

                // === LOGIC MỚI 4: CẬP NHẬT CHUỖI NGÀY (STREAK) ===
                var streak = await _context.ChuoiNgays.FirstOrDefaultAsync(c => c.UserID == userId);
                if (streak != null)
                {
                    if (streak.NgayCapNhatCuoi.Date == DateTime.Today) { /* Đã chơi hôm nay */ }
                    else if (streak.NgayCapNhatCuoi.Date == DateTime.Today.AddDays(-1))
                    {
                        streak.SoNgayLienTiep++;
                        streak.NgayCapNhatCuoi = DateTime.Today;
                    }
                    else
                    {
                        streak.SoNgayLienTiep = 1;
                        streak.NgayCapNhatCuoi = DateTime.Today;
                    }
                }
                else
                {
                    _context.ChuoiNgays.Add(new ChuoiNgay
                    {
                        UserID = userId,
                        SoNgayLienTiep = 1,
                        NgayCapNhatCuoi = DateTime.Today
                    });
                }

                // === LOGIC MỚI 5: LƯU TẤT CẢ THAY ĐỔI VÀO DB ===
                await _context.SaveChangesAsync();

                // === COMMIT TRANSACTION: CHỐT ĐƠN ===
                await transaction.CommitAsync();

                // (Giữ nguyên logic trả về của bạn)
                return Ok(new
                {
                    success = true,
                    ketQuaId = ketQua.KetQuaID,
                    diem = ketQua.Diem,
                    soCauDung = ketQua.SoCauDung,
                    tongCauHoi = ketQua.TongCauHoi,
                    trangThai = ketQua.TrangThai,
                    thongBao = $"Bạn đã trả lời đúng {soCauDung}/{answers.Count} câu và đạt {tongDiem} điểm!"
                });
            }
            catch (Exception ex)
            {
                // === ROLLBACK TRANSACTION: HỦY ĐƠN ===
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi server: " + ex.Message });
            }
        }

        // PHƯƠNG THỨC LẤY USER ID TỪ TOKEN
        private int? GetUserIdFromToken()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
                return null;

            if (int.TryParse(userIdString, out int userId))
                return userId;

            return null;
        }

        // Request/Response Models
        public class GameStartRequest
        {
            public int ChuDeID { get; set; }
            public int DoKhoID { get; set; }
            public int SoLuongCauHoi { get; set; }
        }

        public class QuestionResponse
        {
            public int CauHoiID { get; set; }
            public int ChuDeID { get; set; }
            public int DoKhoID { get; set; }
            public string NoiDung { get; set; }
            public string? DapAnA { get; set; }
            public string? DapAnB { get; set; }
            public string? DapAnC { get; set; }
            public string? DapAnD { get; set; }
        }

        public class AnswerRequest
        {
            public int CauHoiID { get; set; }
            public string DapAnChon { get; set; }
        }
    }
}