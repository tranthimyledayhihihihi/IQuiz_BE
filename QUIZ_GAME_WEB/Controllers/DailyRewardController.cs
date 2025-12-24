using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using System.Security.Claims;

namespace QUIZ_GAME_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class DailyRewardController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly ILogger<DailyRewardController> _logger;

        public DailyRewardController(QuizGameContext context, ILogger<DailyRewardController> logger)
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

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDailyRewards(int userId)
        {
            try
            {
                var currentUserId = GetUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                _logger.LogInformation($"🎁 Getting daily rewards for user {currentUserId.Value}");

                // Tạo mock data vì chưa có bảng ThuongNgay
                var rewards = new List<object>
                {
                    new
                    {
                        ThuongID = 1,
                        UserID = currentUserId.Value,
                        NgayNhan = DateTime.Today.AddDays(-2),
                        LoaiThuong = "Coins",
                        GiaTri = 100,
                        MoTa = "Daily login reward"
                    },
                    new
                    {
                        ThuongID = 2,
                        UserID = currentUserId.Value,
                        NgayNhan = DateTime.Today.AddDays(-1),
                        LoaiThuong = "Coins",
                        GiaTri = 100,
                        MoTa = "Daily login reward"
                    }
                };

                return Ok(new { success = true, data = rewards });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error getting daily rewards: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("user/{userId}/today")]
        public async Task<IActionResult> CheckTodayReward(int userId)
        {
            try
            {
                var currentUserId = GetUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                _logger.LogInformation($"🎁 Checking today's reward for user {currentUserId.Value}");

                // Giả lập: chưa nhận thưởng hôm nay
                return Ok(new { 
                    success = true, 
                    claimed = false,
                    message = "Available to claim",
                    reward = new { type = "Coins", value = 100 }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error checking today's reward: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("claim")]
        public async Task<IActionResult> ClaimDailyReward([FromBody] ClaimRewardRequest request)
        {
            try
            {
                var currentUserId = GetUserId();
                if (currentUserId == null)
                    return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

                _logger.LogInformation($"🎁 Claiming daily reward for user {currentUserId.Value}");

                // Giả lập thành công
                return Ok(new { 
                    success = true, 
                    rewardId = 1,
                    giaTri = 100,
                    message = "🎉 Daily reward claimed successfully! +100 coins"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error claiming daily reward: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class ClaimRewardRequest
    {
        public int UserID { get; set; }
        public string LoaiThuong { get; set; } = "Coins";
        public int GiaTri { get; set; } = 100;
        public string MoTa { get; set; } = "Daily login reward";
    }
}