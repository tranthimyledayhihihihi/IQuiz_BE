using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using System.Linq;
using System.Threading.Tasks;

// Đảm bảo namespace khớp với thư mục 'User'
namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    // Không cần [Authorize] - Bảng xếp hạng là công khai
    public class BXHController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public BXHController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy Bảng Xếp Hạng Tuần (Top 50).
        /// </summary>
        // GET: api/bxh/tuan
        [HttpGet("tuan")]
        public async Task<IActionResult> GetWeeklyLeaderboard()
        {
            var leaderboard = await _context.BXHs
                .OrderByDescending(b => b.DiemTuan) // Sắp xếp điểm từ cao đến thấp
                .Include(b => b.NguoiDung) // Lấy (Join) thông tin người dùng
                .Select(b => new // Chỉ chọn các trường an toàn để hiển thị
                {
                    b.UserID,
                    b.NguoiDung.TenDangNhap,
                    b.NguoiDung.HoTen,
                    b.NguoiDung.AnhDaiDien,
                    b.DiemTuan,
                    b.HangTuan
                })
                .Take(50) // Chỉ lấy 50 người đầu tiên
                .ToListAsync();

            return Ok(leaderboard);
        }

        /// <summary>
        /// Lấy Bảng Xếp Hạng Tháng (Top 50).
        /// </summary>
        // GET: api/bxh/thang
        [HttpGet("thang")]
        public async Task<IActionResult> GetMonthlyLeaderboard()
        {
            var leaderboard = await _context.BXHs
                .OrderByDescending(b => b.DiemThang)
                .Include(b => b.NguoiDung)
                .Select(b => new
                {
                    b.UserID,
                    b.NguoiDung.TenDangNhap,
                    b.NguoiDung.HoTen,
                    b.NguoiDung.AnhDaiDien,
                    b.DiemThang,
                    b.HangThang
                })
                .Take(50)
                .ToListAsync();

            return Ok(leaderboard);
        }
    }
}