using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class KetQuaController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public KetQuaController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy lịch sử chơi của người dùng đang đăng nhập (phân trang).
        /// </summary>
        // GET: api/ketqua/history
        [HttpGet("history")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // 1. Lấy UserID từ token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            int userId = int.Parse(userIdString);

            // 2. Truy vấn kết quả, sắp xếp cái mới nhất lên đầu
            var query = _context.KetQuas
                .Where(kq => kq.UserID == userId)
                .OrderByDescending(kq => kq.ThoiGian);

            // 3. Lấy tổng số kết quả (để client biết có bao nhiêu trang)
            var totalItems = await query.CountAsync();

            // 4. Phân trang
            var history = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Data = history
            });
        }

        /// <summary>
        /// Lấy chi tiết một kết quả cụ thể.
        /// </summary>
        // GET: api/ketqua/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetResultDetails(int id)
        {
            // Lấy UserID từ token để xác thực
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdString!);

            var ketQua = await _context.KetQuas.FindAsync(id);

            if (ketQua == null)
            {
                return NotFound(new { message = "Không tìm thấy kết quả này." });
            }

            // Đảm bảo người dùng này CHỈ được xem kết quả của chính họ
            if (ketQua.UserID != userId)
            {
                return Forbid(); // 403 Forbidden - Bạn không có quyền xem
            }

            return Ok(ketQua);
        }
    }
}