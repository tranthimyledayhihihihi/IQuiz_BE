using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// Đảm bảo namespace khớp với thư mục 'User'
namespace QUIZ_GAME_WEB.Controllers.User
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhTuuController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ThanhTuuController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách TẤT CẢ các thành tựu/huy hiệu có trong game.
        /// </summary>
        // GET: api/thanhtuu/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAchievements()
        {
            var allAchievements = await _context.ThanhTuus.ToListAsync();
            return Ok(allAchievements);
        }

        /// <summary>
        /// Lấy danh sách các thành tựu mà người dùng ĐÃ ĐẠT ĐƯỢC.
        /// </summary>
        /// <remarks>
        /// LƯU Ý: Đây là logic mô phỏng. Trong dự án thật, bạn cần một
        /// bảng 'NguoiDung_ThanhTuu' để lưu huy hiệu đã mở khóa,
        /// hoặc kiểm tra logic phức tạp dựa trên 'DieuKien'.
        /// </remarks>
        // GET: api/thanhtuu/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyAchievements()
        {
            // 1. Lấy UserID từ token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdString!);

            // 2. Lấy tất cả thành tựu có thể có
            var allAchievements = await _context.ThanhTuus.ToListAsync();
            var myUnlockedAchievements = new List<ThanhTuu>();

            // --- LOGIC KIỂM TRA (MÔ PHỎNG) ---
            // Chúng ta sẽ kiểm tra điều kiện của từng huy hiệu

            // 3. Lấy dữ liệu thống kê của người dùng
            var userGameCount = await _context.KetQuas.CountAsync(kq => kq.UserID == userId);

            // 4. Duyệt qua từng huy hiệu và kiểm tra
            foreach (var achievement in allAchievements)
            {
                // Kiểm tra huy hiệu "Lính mới" (từ SeedData)
                if (achievement.DieuKien == "played_1_game" && userGameCount > 0)
                {
                    myUnlockedAchievements.Add(achievement);
                }

                // (Bạn có thể thêm các logic 'else if' khác ở đây)
                // else if (achievement.DieuKien == "correct_50_history" && ...)
                // {
                //     myUnlockedAchievements.Add(achievement);
                // }
            }

            return Ok(myUnlockedAchievements);
        }
    }
}   