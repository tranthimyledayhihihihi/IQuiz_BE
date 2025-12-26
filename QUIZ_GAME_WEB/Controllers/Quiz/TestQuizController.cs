using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [ApiController]
    [Route("api/quiz")]
    public class TestQuizController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public TestQuizController(QuizGameContext context)
        {
            _context = context;
        }

        // ================= QUIZ NỔI BẬT =================
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedQuiz()
        {
            var quizzes = await _context.QuizTuyChinhs
                .Where(q => q.IsNoiBat && q.TrangThai == "Approved")
                .OrderByDescending(q => q.LuotChoi)
                .Select(q => new
                {
                    quizID = q.QuizTuyChinhID,
                    tieuDe = q.TenQuiz,
                    moTa = q.MoTa,
                    anhBia = q.AnhBia,
                    luotChoi = q.LuotChoi
                })
                .ToListAsync();

            return Ok(quizzes);
        }

    }
}
