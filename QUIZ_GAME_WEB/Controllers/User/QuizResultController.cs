using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Security.Claims;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class QuizResultController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly ILogger<QuizResultController> _logger;

        public QuizResultController(QuizGameContext context, ILogger<QuizResultController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        /// <summary>
        /// Submit quiz result và tự động cập nhật thành tựu
        /// POST: api/user/quizresult/submit
        /// </summary>
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuizResult([FromBody] SubmitQuizResultRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                _logger.LogInformation($"🎯 User {userId.Value} submitting quiz result...");

                // Validate request
                if (request.TongCauHoi <= 0 || request.SoCauDung < 0 || request.SoCauDung > request.TongCauHoi)
                {
                    return BadRequest(new { message = "Dữ liệu kết quả không hợp lệ." });
                }

                // Tính điểm (0-100)
                int diem = (int)Math.Round((double)request.SoCauDung / request.TongCauHoi * 100);

                // Tạo QuizAttempt (giả lập)
                var quizAttempt = new QuizAttempt
                {
                    UserID = userId.Value,
                    QuizTuyChinhID = 1, // Default quiz
                    NgayBatDau = DateTime.Now.AddMinutes(-5),
                    NgayKetThuc = DateTime.Now,
                    SoCauHoiLam = request.TongCauHoi,
                    SoCauDung = request.SoCauDung,
                    Diem = diem,
                    TrangThai = "Hoàn thành"
                };

                _context.QuizAttempts.Add(quizAttempt);
                await _context.SaveChangesAsync();

                // Tạo KetQua
                var ketQua = new KetQua
                {
                    UserID = userId.Value,
                    QuizAttemptID = quizAttempt.QuizAttemptID,
                    Diem = diem,
                    SoCauDung = request.SoCauDung,
                    TongCauHoi = request.TongCauHoi,
                    TrangThaiKetQua = "Hoàn thành",
                    ThoiGian = DateTime.Now
                };

                _context.KetQuas.Add(ketQua);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Quiz result saved - Score: {diem}, Correct: {request.SoCauDung}/{request.TongCauHoi}");

                // Tự động kiểm tra và cập nhật thành tựu
                await CheckAndUpdateAchievements(userId.Value);

                // Cập nhật streak
                await UpdateUserStreak(userId.Value);

                return Ok(new
                {
                    success = true,
                    message = "Kết quả đã được lưu thành công!",
                    result = new
                    {
                        diem = diem,
                        soCauDung = request.SoCauDung,
                        tongCauHoi = request.TongCauHoi,
                        ketQuaId = ketQua.KetQuaID
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error submitting quiz result: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi lưu kết quả: " + ex.Message });
            }
        }

        /// <summary>
        /// Kiểm tra và cập nhật thành tựu tự động
        /// </summary>
        private async Task CheckAndUpdateAchievements(int userId)
        {
            try
            {
                _logger.LogInformation($"🏆 Checking achievements for user {userId}...");

                // Lấy thống kê hiện tại
                var totalQuizzes = await _context.KetQuas
                    .Where(kq => kq.UserID == userId)
                    .CountAsync();

                var avgScore = await _context.KetQuas
                    .Where(kq => kq.UserID == userId)
                    .AverageAsync(kq => (double?)kq.Diem) ?? 0.0;

                var perfectScores = await _context.KetQuas
                    .Where(kq => kq.UserID == userId && kq.Diem == 100)
                    .CountAsync();

                _logger.LogInformation($"📊 Stats - {totalQuizzes} quizzes, {avgScore:F1} avg, {perfectScores} perfect");

                // Lấy danh sách thành tựu đã có
                var existingAchievements = await _context.ThanhTuus
                    .Where(t => t.NguoiDungID == userId)
                    .Select(t => t.AchievementCode)
                    .ToListAsync();

                // Kiểm tra các thành tựu mới
                var newAchievements = new List<(string code, string name, string description)>();

                // Thành tựu quiz đầu tiên
                if (totalQuizzes >= 1 && !existingAchievements.Contains("FIRST_QUIZ_COMPLETED"))
                {
                    newAchievements.Add(("FIRST_QUIZ_COMPLETED", "🎯 Người mới bắt đầu", "Hoàn thành quiz đầu tiên"));
                }

                // Thành tựu 5 quiz
                if (totalQuizzes >= 5 && !existingAchievements.Contains("5_QUIZ_COMPLETED"))
                {
                    newAchievements.Add(("5_QUIZ_COMPLETED", "📚 Học sinh chăm chỉ", "Hoàn thành 5 quiz"));
                }

                // Thành tựu 10 quiz
                if (totalQuizzes >= 10 && !existingAchievements.Contains("10_QUIZ_COMPLETED"))
                {
                    newAchievements.Add(("10_QUIZ_COMPLETED", "🎓 Thạc sĩ tri thức", "Hoàn thành 10 quiz"));
                }

                // Thành tựu điểm cao
                if (avgScore >= 80 && !existingAchievements.Contains("HIGH_AVERAGE_80"))
                {
                    newAchievements.Add(("HIGH_AVERAGE_80", "🥇 Chuyên gia", "Đạt điểm trung bình trên 80"));
                }

                if (avgScore >= 90 && !existingAchievements.Contains("HIGH_AVERAGE_90"))
                {
                    newAchievements.Add(("HIGH_AVERAGE_90", "🏆 Bậc thầy", "Đạt điểm trung bình trên 90"));
                }

                // Thành tựu điểm tuyệt đối
                if (perfectScores >= 1 && !existingAchievements.Contains("FIRST_PERFECT_SCORE"))
                {
                    newAchievements.Add(("FIRST_PERFECT_SCORE", "💯 Hoàn hảo", "Đạt điểm tuyệt đối lần đầu"));
                }

                if (perfectScores >= 3 && !existingAchievements.Contains("THREE_PERFECT_SCORES"))
                {
                    newAchievements.Add(("THREE_PERFECT_SCORES", "⭐ Siêu sao", "Đạt điểm tuyệt đối 3 lần"));
                }

                // Thêm thành tựu mới vào database
                foreach (var (code, name, description) in newAchievements)
                {
                    var thanhTuu = new ThanhTuu
                    {
                        NguoiDungID = userId,
                        AchievementCode = code,
                        NgayDatDuoc = DateTime.Now
                    };

                    _context.ThanhTuus.Add(thanhTuu);
                    _logger.LogInformation($"🎉 New achievement unlocked: {name} for user {userId}");
                }

                if (newAchievements.Any())
                {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"✅ Added {newAchievements.Count} new achievements for user {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error checking achievements: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật streak của user
        /// </summary>
        private async Task UpdateUserStreak(int userId)
        {
            try
            {
                var today = DateTime.Today;
                var streak = await _context.ChuoiNgays.FirstOrDefaultAsync(c => c.UserID == userId);

                if (streak == null)
                {
                    // Tạo streak mới
                    streak = new ChuoiNgay
                    {
                        UserID = userId,
                        SoNgayLienTiep = 1,
                        NgayCapNhatCuoi = today
                    };
                    _context.ChuoiNgays.Add(streak);
                }
                else
                {
                    var lastUpdate = streak.NgayCapNhatCuoi.Date;
                    
                    if (lastUpdate == today)
                    {
                        // Đã chơi hôm nay rồi, không cần cập nhật
                        return;
                    }
                    else if (lastUpdate == today.AddDays(-1))
                    {
                        // Chơi liên tiếp, tăng streak
                        streak.SoNgayLienTiep++;
                        streak.NgayCapNhatCuoi = today;
                    }
                    else
                    {
                        // Bị gián đoạn, reset streak
                        streak.SoNgayLienTiep = 1;
                        streak.NgayCapNhatCuoi = today;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"🔥 Updated streak for user {userId}: {streak.SoNgayLienTiep} days");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error updating streak: {ex.Message}");
            }
        }

        /// <summary>
        /// Request model cho submit quiz result
        /// </summary>
        public class SubmitQuizResultRequest
        {
            public int TongCauHoi { get; set; }
            public int SoCauDung { get; set; }
            public int CategoryId { get; set; } = 1;
            public int DifficultyId { get; set; } = 1;
        }
    }
}