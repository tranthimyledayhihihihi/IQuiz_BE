using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;

namespace QUIZ_GAME_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestQuizController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly ILogger<TestQuizController> _logger;

        public TestQuizController(QuizGameContext context, ILogger<TestQuizController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("questions/{categoryId}")]
        public async Task<IActionResult> GetQuestionsByCategory(int categoryId)
        {
            try
            {
                _logger.LogInformation($"🔍 Getting questions for category {categoryId}...");

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

                _logger.LogInformation($"✅ Found {questions.Count} questions for category {categoryId}");

                return Ok(new { success = true, data = questions });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error getting questions: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("categories")]
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

                _logger.LogInformation($"✅ Found {categories.Count} categories");

                return Ok(new { success = true, data = categories });
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error getting categories: {ex.Message}");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}