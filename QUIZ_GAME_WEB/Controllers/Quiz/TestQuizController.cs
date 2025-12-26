using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;

namespace QUIZ_GAME_WEB.Controllers
{
    [ApiController]
    [Route("api")]
    public class QuizApiController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly ILogger<QuizApiController> _logger;

        public QuizApiController(QuizGameContext context, ILogger<QuizApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // =====================================================
        // TEST QUIZ - CATEGORIES
        // GET /api/testquiz/categories
        // =====================================================
        [HttpGet("testquiz/categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                _logger.LogInformation("🔍 Getting categories...");

                var categories = await _context.ChuDes
                    .Where(c => c.TrangThai == true)
                    .Select(c => new
                    {
                        id = c.ChuDeID,
                        name = c.TenChuDe,
                        description = c.MoTa ?? "",
                        icon = "quiz",
                        question_count = _context.CauHois.Count(q => q.ChuDeID == c.ChuDeID)
                    })
                    .ToListAsync();

                _logger.LogInformation("✅ Found {Count} categories", categories.Count);

                return Ok(new { success = true, data = categories });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting categories");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // TEST QUIZ - QUESTIONS BY CATEGORY
        // GET /api/testquiz/questions/{categoryId}
        // =====================================================
        [HttpGet("testquiz/questions/{categoryId:int}")]
        public async Task<IActionResult> GetQuestionsByCategory(int categoryId)
        {
            try
            {
                _logger.LogInformation("🔍 Getting questions for category {CategoryId}...", categoryId);

                var questions = await _context.CauHois
                    .Where(q => q.ChuDeID == categoryId)
                    .Select(q => new
                    {
                        id = q.CauHoiID,
                        question = q.NoiDung,
                        option_a = q.DapAnA,
                        option_b = q.DapAnB,
                        option_c = q.DapAnC,
                        option_d = q.DapAnD,
                        correct_answer = q.DapAnDung,
                        difficulty_id = q.DoKhoID,
                        category_id = q.ChuDeID
                    })
                    .ToListAsync();

                _logger.LogInformation("✅ Found {Count} questions for category {CategoryId}", questions.Count, categoryId);

                return Ok(new { success = true, data = questions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting questions for category {CategoryId}", categoryId);
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // FEATURED QUIZ
        // GET /api/quiz/featured
        // =====================================================
        [HttpGet("quiz/featured")]
        public async Task<IActionResult> GetFeaturedQuiz()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting featured quizzes");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
