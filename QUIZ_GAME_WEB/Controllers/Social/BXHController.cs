using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.SocialRankingModels; // BXH
using QUIZ_GAME_WEB.Models.CoreEntities; // NguoiDung
using System.Linq;

namespace QUIZ_GAME_WEB.Controllers.Social
{
    [Route("api/social/[controller]")]
    [ApiController]
    public class BXHController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public BXHController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/social/BXH/Weekly
        // Chức năng: Lấy bảng xếp hạng tuần
        [HttpGet("Weekly")]
        public async Task<ActionResult<IEnumerable<BXH>>> GetWeeklyLeaderboard()
        {
            // Logic nghiệp vụ: Lấy top 100 theo DiemTuan
            return await _context.BXHs
                .OrderByDescending(b => b.DiemTuan)
                .Take(100)
                .Include(b => b.NguoiDung) // Include thông tin cơ bản của người dùng
                .ToListAsync();
        }

        // GET: api/social/BXH/Monthly
        // Chức năng: Lấy bảng xếp hạng tháng
        [HttpGet("Monthly")]
        public async Task<ActionResult<IEnumerable<BXH>>> GetMonthlyLeaderboard()
        {
            // Logic nghiệp vụ: Lấy top 100 theo DiemThang
            return await _context.BXHs
                .OrderByDescending(b => b.DiemThang)
                .Take(100)
                .Include(b => b.NguoiDung)
                .ToListAsync();
        }

        // GET: api/social/BXH/UserRank/{userId}
        // Chức năng: Lấy hạng của một người dùng cụ thể
        [HttpGet("UserRank/{userId}")]
        public async Task<ActionResult<BXH>> GetUserRank(int userId)
        {
            var bxh = await _context.BXHs
                .Include(b => b.NguoiDung)
                .FirstOrDefaultAsync(b => b.UserID == userId);

            if (bxh == null)
            {
                return NotFound("Người dùng chưa có trong bảng xếp hạng.");
            }

            return bxh;
        }
    }
}