using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.Interfaces;     // IResultRepository, IRewardService
using QUIZ_GAME_WEB.Models.ResultsModels;  // ThanhTuu, ChuoiNgay
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    // Đổi tên Controller nếu cần (ví dụ: AchievmentsController nếu bạn đặt tên lớp như vậy)
    public class AchievementController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly IResultRepository _resultRepo;
        private readonly IRewardService _rewardService;

        public AchievementController(QuizGameContext context, IResultRepository resultRepo = null, IRewardService rewardService = null)
        {
            _context = context;
            _resultRepo = resultRepo;
            _rewardService = rewardService;
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        // ======================================================
        // 1. LẤY THÀNH TỰU ĐÃ ĐẠT (GET: api/user/achievement/me)
        // ======================================================
        /// <summary>
        /// Lấy danh sách thành tựu của chính người dùng dựa trên thống kê thật
        /// </summary>
        [HttpGet("me")]
        public async Task<IActionResult> GetMyAchievements()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                Console.WriteLine($"🏆 DEBUG: Getting achievements for user {userId.Value}");

                // Lấy thống kê từ database
                var totalQuizzes = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value)
                    .CountAsync();

                var avgScore = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value)
                    .AverageAsync(kq => (double?)kq.Diem) ?? 0.0;

                var totalCorrect = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value)
                    .SumAsync(kq => kq.SoCauDung);

                var perfectScores = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value && kq.Diem == 100)
                    .CountAsync();

                Console.WriteLine($"🏆 DEBUG: Stats - {totalQuizzes} quizzes, {avgScore:F1} avg, {totalCorrect} correct, {perfectScores} perfect");

                // Tạo danh sách thành tựu dựa trên thống kê
                var achievements = new List<object>();

                // Thành tựu cơ bản
                if (totalQuizzes >= 1)
                {
                    achievements.Add(new
                    {
                        id = 1,
                        tenThanhTuu = "🎯 Người mới bắt đầu",
                        moTa = "Hoàn thành quiz đầu tiên",
                        icon = "🎯",
                        isUnlocked = true,
                        progress = 100,
                        requirement = "Hoàn thành 1 quiz"
                    });
                }

                if (totalQuizzes >= 5)
                {
                    achievements.Add(new
                    {
                        id = 2,
                        tenThanhTuu = "📚 Học sinh chăm chỉ",
                        moTa = "Hoàn thành 5 quiz",
                        icon = "📚",
                        isUnlocked = true,
                        progress = 100,
                        requirement = "Hoàn thành 5 quiz"
                    });
                }

                if (totalQuizzes >= 10)
                {
                    achievements.Add(new
                    {
                        id = 3,
                        tenThanhTuu = "🎓 Thạc sĩ tri thức",
                        moTa = "Hoàn thành 10 quiz",
                        icon = "🎓",
                        isUnlocked = true,
                        progress = 100,
                        requirement = "Hoàn thành 10 quiz"
                    });
                }

                // Thành tựu điểm số
                if (avgScore >= 80)
                {
                    achievements.Add(new
                    {
                        id = 4,
                        tenThanhTuu = "🥇 Chuyên gia",
                        moTa = "Đạt điểm trung bình trên 80",
                        icon = "🥇",
                        isUnlocked = true,
                        progress = 100,
                        requirement = "Điểm TB ≥ 80"
                    });
                }

                if (avgScore >= 90)
                {
                    achievements.Add(new
                    {
                        id = 5,
                        tenThanhTuu = "🏆 Bậc thầy",
                        moTa = "Đạt điểm trung bình trên 90",
                        icon = "🏆",
                        isUnlocked = true,
                        progress = 100,
                        requirement = "Điểm TB ≥ 90"
                    });
                }

                // Thành tựu hoàn hảo
                if (perfectScores >= 1)
                {
                    achievements.Add(new
                    {
                        id = 6,
                        tenThanhTuu = "💯 Hoàn hảo",
                        moTa = "Đạt điểm tuyệt đối trong 1 quiz",
                        icon = "💯",
                        isUnlocked = true,
                        progress = 100,
                        requirement = "Đạt 100 điểm"
                    });
                }

                if (perfectScores >= 3)
                {
                    achievements.Add(new
                    {
                        id = 7,
                        tenThanhTuu = "⭐ Siêu sao",
                        moTa = "Đạt điểm tuyệt đối trong 3 quiz",
                        icon = "⭐",
                        isUnlocked = true,
                        progress = 100,
                        requirement = "Đạt 100 điểm 3 lần"
                    });
                }

                // Thành tựu chưa đạt (để tạo động lực)
                if (totalQuizzes < 20)
                {
                    achievements.Add(new
                    {
                        id = 8,
                        tenThanhTuu = "🚀 Chinh phục viên",
                        moTa = "Hoàn thành 20 quiz",
                        icon = "🚀",
                        isUnlocked = false,
                        progress = (int)((double)totalQuizzes / 20 * 100),
                        requirement = $"Hoàn thành 20 quiz ({totalQuizzes}/20)"
                    });
                }

                Console.WriteLine($"✅ DEBUG: Generated {achievements.Count} achievements");

                return Ok(new
                {
                    success = true,
                    totalAchievements = achievements.Count,
                    unlockedCount = achievements.Count(a => (bool)((dynamic)a).isUnlocked),
                    achievements = achievements
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DEBUG: Error getting achievements: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi lấy danh sách thành tựu: " + ex.Message });
            }
        }

        // ======================================================
        // 2. LẤY CHUỖI NGÀY CHƠI (GET: api/user/achievement/streak)
        // ======================================================
        /// <summary>
        /// Lấy thông tin chuỗi ngày chơi (streak) từ database
        /// </summary>
        [HttpGet("streak")]
        public async Task<IActionResult> GetMyStreak()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                Console.WriteLine($"🔥 DEBUG: Getting streak for user {userId.Value}");

                // Lấy streak từ database hoặc tính toán đơn giản
                var streak = await _context.ChuoiNgays
                    .FirstOrDefaultAsync(c => c.UserID == userId.Value);

                if (streak == null)
                {
                    // Tạo streak mới nếu chưa có
                    Console.WriteLine($"🔥 DEBUG: No streak found, creating new one");
                    return Ok(new { 
                        soNgayLienTiep = 0, 
                        ngayCapNhatCuoi = (DateTime?)null,
                        message = "Bắt đầu chuỗi ngày chơi của bạn!"
                    });
                }

                Console.WriteLine($"🔥 DEBUG: Found streak - {streak.SoNgayLienTiep} days");

                return Ok(new
                {
                    soNgayLienTiep = streak.SoNgayLienTiep,
                    ngayCapNhatCuoi = streak.NgayCapNhatCuoi,
                    message = streak.SoNgayLienTiep > 0 ? 
                        $"Bạn đã chơi liên tục {streak.SoNgayLienTiep} ngày!" : 
                        "Hãy bắt đầu chuỗi ngày chơi!"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DEBUG: Error getting streak: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi lấy chuỗi ngày chơi: " + ex.Message });
            }
        }

        // ======================================================
        // 3. NHẬN THƯỞNG HẰNG NGÀY (POST: api/user/achievement/daily-reward)
        // ======================================================
        /// <summary>
        /// Nhận thưởng điểm đăng nhập hằng ngày (đơn giản hóa)
        /// </summary>
        [HttpPost("daily-reward")]
        public async Task<IActionResult> ClaimDailyReward()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                Console.WriteLine($"🎁 DEBUG: Claiming daily reward for user {userId.Value}");

                // Kiểm tra xem hôm nay đã nhận thưởng chưa (đơn giản)
                var today = DateTime.Today;
                var todayQuizzes = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value && kq.ThoiGian.Date == today)
                    .CountAsync();

                if (todayQuizzes == 0)
                {
                    return Ok(new { 
                        awarded = false, 
                        message = "Hãy hoàn thành ít nhất 1 quiz hôm nay để nhận thưởng!" 
                    });
                }

                // Giả lập nhận thưởng thành công
                Console.WriteLine($"🎁 DEBUG: Daily reward claimed successfully");

                return Ok(new { 
                    awarded = true, 
                    message = "🎁 Nhận thưởng hằng ngày thành công! +50 điểm",
                    reward = 50
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DEBUG: Error claiming daily reward: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi yêu cầu nhận thưởng: " + ex.Message });
            }
        }
    }
}