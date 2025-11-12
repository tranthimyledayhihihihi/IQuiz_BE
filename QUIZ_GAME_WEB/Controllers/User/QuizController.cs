using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.User
{
    // Tất cả API trong đây đều yêu cầu đăng nhập
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly QuizGameContext _context;

        // Bắt buộc phải có constructor để inject DbContext
        public QuizController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách TẤT CẢ các chủ đề (chỉ lấy chủ đề đang Bật).
        /// </summary>
        // GET: api/quiz/chude
        [HttpGet("chude")]
        public async Task<IActionResult> GetChuDe()
        {
            // Chỉ lấy các chủ đề có TrangThai = true (đang hoạt động)
            var list = await _context.ChuDes
                .Where(c => c.TrangThai == true)
                .Select(c => new { c.ChuDeID, c.TenChuDe, c.MoTa }) // Chỉ trả về thông tin cần thiết
                .ToListAsync();

            return Ok(list);
        }

        /// <summary>
        /// Lấy danh sách TẤT CẢ các độ khó.
        /// </summary>
        // GET: api/quiz/dokho
        [HttpGet("dokho")]
        public async Task<IActionResult> GetDoKho()
        {
            var list = await _context.DoKhos.ToListAsync();
            return Ok(list);
        }
    }
}