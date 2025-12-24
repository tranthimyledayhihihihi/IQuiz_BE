using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;

namespace QUIZ_GAME_WEB.Controllers
{
    /// <summary>
    /// Controller để quản lý chủ đề (Categories) - ASP.NET Core
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChuDeController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly ILogger<ChuDeController> _logger;

        public ChuDeController(QuizGameContext context, ILogger<ChuDeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách chủ đề với thống kê
        /// GET: api/chude/with-stats
        /// </summary>
        [HttpGet("with-stats")]
        public async Task<IActionResult> GetCategoriesWithStats()
        {
            try
            {
                _logger.LogInformation("🔍 Getting categories with stats from database...");

                var categories = await _context.ChuDes
                    .Where(c => c.TrangThai == true)
                    .Select(c => new
                    {
                        id = c.ChuDeID,
                        name = c.TenChuDe,
                        icon = GetCategoryIcon(c.TenChuDe),
                        quiz_count = _context.CauHois.Count(ch => ch.ChuDeID == c.ChuDeID),
                        progress_percent = 0
                    })
                    .OrderBy(c => c.id)
                    .ToListAsync();

                _logger.LogInformation($"✅ Found {categories.Count} categories in database");

                if (!categories.Any())
                {
                    _logger.LogWarning("⚠️ No categories found in database!");
                    return Ok(new List<object>()); // Return empty array instead of error
                }

                // Log each category for debugging
                foreach (var cat in categories)
                {
                    _logger.LogInformation($"📂 Category: {cat.name} (ID: {cat.id}, Questions: {cat.quiz_count})");
                }

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting categories: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi lấy thống kê chủ đề: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách tất cả chủ đề
        /// GET: api/chude
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("🔍 Getting all categories from database...");

                var categories = await _context.ChuDes
                    .Where(c => c.TrangThai == true)
                    .Select(c => new
                    {
                        ChuDeID = c.ChuDeID,
                        TenChuDe = c.TenChuDe,
                        MoTa = c.MoTa,
                        TrangThai = c.TrangThai,
                        SoCauHoi = _context.CauHois.Count(ch => ch.ChuDeID == c.ChuDeID),
                        Icon = GetCategoryIcon(c.TenChuDe)
                    })
                    .OrderBy(c => c.ChuDeID)
                    .ToListAsync();

                _logger.LogInformation($"✅ Found {categories.Count} categories");

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách chủ đề thành công",
                    data = categories,
                    total = categories.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting all categories: {Message}", ex.Message);
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy danh sách chủ đề: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết một chủ đề
        /// GET: api/chude/{id}
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                _logger.LogInformation($"🔍 Getting category by ID: {id}");

                var category = await _context.ChuDes
                    .Where(c => c.ChuDeID == id && c.TrangThai == true)
                    .Select(c => new
                    {
                        ChuDeID = c.ChuDeID,
                        TenChuDe = c.TenChuDe,
                        MoTa = c.MoTa,
                        TrangThai = c.TrangThai,
                        SoCauHoi = _context.CauHois.Count(ch => ch.ChuDeID == c.ChuDeID),
                        Icon = GetCategoryIcon(c.TenChuDe)
                    })
                    .FirstOrDefaultAsync();

                if (category == null)
                {
                    _logger.LogWarning($"⚠️ Category with ID {id} not found");
                    return NotFound();
                }

                _logger.LogInformation($"✅ Found category: {category.TenChuDe}");

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin chủ đề thành công",
                    data = category
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error getting category by ID {Id}: {Message}", id, ex.Message);
                return BadRequest(new
                {
                    success = false,
                    message = "Lỗi khi lấy thông tin chủ đề: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Test endpoint để kiểm tra database connection
        /// GET: api/chude/test
        /// </summary>
        [HttpGet("test")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                _logger.LogInformation("🧪 Testing database connection...");

                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    _logger.LogError("❌ Cannot connect to database!");
                    return StatusCode(500, new { message = "Cannot connect to database" });
                }

                // Count tables
                var categoryCount = await _context.ChuDes.CountAsync();
                var questionCount = await _context.CauHois.CountAsync();
                var difficultyCount = await _context.DoKhos.CountAsync();

                _logger.LogInformation($"✅ Database connected! Categories: {categoryCount}, Questions: {questionCount}, Difficulties: {difficultyCount}");

                return Ok(new
                {
                    success = true,
                    message = "Database connection successful",
                    data = new
                    {
                        categories = categoryCount,
                        questions = questionCount,
                        difficulties = difficultyCount,
                        canConnect = canConnect
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Database test failed: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database test failed: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Helper method để tạo icon cho category
        /// </summary>
        private static string GetCategoryIcon(string tenChuDe)
        {
            if (string.IsNullOrEmpty(tenChuDe))
                return "default";

            var lowerName = tenChuDe.ToLower();

            if (lowerName.Contains("toán") || lowerName.Contains("math"))
                return "math";
            else if (lowerName.Contains("sử") || lowerName.Contains("history") || lowerName.Contains("việt nam"))
                return "history";
            else if (lowerName.Contains("khoa học") || lowerName.Contains("science") || lowerName.Contains("tự nhiên"))
                return "science";
            else if (lowerName.Contains("địa") || lowerName.Contains("geography"))
                return "geography";
            else if (lowerName.Contains("văn") || lowerName.Contains("literature"))
                return "literature";
            else if (lowerName.Contains("anh") || lowerName.Contains("english"))
                return "language";
            else
                return "general";
        }
    }
}