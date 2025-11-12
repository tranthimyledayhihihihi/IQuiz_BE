using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Data;

namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    // [Authorize]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ThongKeController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/thongke/users
        // [HttpGet("users")]
        // public async Task<IActionResult> GetUserStats()
        // {
        //     // Code logic thống kê chi tiết về người dùng
        //     return Ok();
        // }

        // GET: api/thongke/quiz
        // [HttpGet("quiz")]
        // public async Task<IActionResult> GetQuizStats()
        // {
        //     // Code logic thống kê về câu hỏi (ví dụ: câu nào sai nhiều nhất)
        //     return Ok();
        // }
    }
}