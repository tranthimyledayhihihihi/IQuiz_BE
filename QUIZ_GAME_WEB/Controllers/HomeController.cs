using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using System.Threading.Tasks;

// Namespace đã được đặt về QUIZ_GAME_WEB.Controllers
namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public HomeController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/Home
        // Chức năng: Kiểm tra trạng thái hoạt động của API
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                Status = "Running",
                Service = "QuizGame API",
                Version = "1.0"
            });
        }

        // GET: api/Home/HealthCheck
        // Chức năng: Kiểm tra kết nối Database (Health Check quan trọng)
        [HttpGet("HealthCheck")]
        public async Task<IActionResult> DatabaseHealthCheck()
        {
            try
            {
                // Logic nghiệp vụ: Cố gắng kết nối và thực hiện một truy vấn đơn giản
                await _context.Database.OpenConnectionAsync();
                await _context.Database.CloseConnectionAsync();

                return Ok(new
                {
                    DatabaseStatus = "OK",
                    Message = "API và Database đều hoạt động."
                });
            }
            catch (Exception ex)
            {
                // Trả về lỗi 500 nếu kết nối database thất bại
                return StatusCode(500, new
                {
                    DatabaseStatus = "Error",
                    Message = "Không thể kết nối đến database.",
                    Detail = ex.Message
                });
            }
        }

        // GET: /
        // Endpoint mặc định cho Root path (thường được cấu hình trong Program.cs)
        [HttpGet("/")]
        public IActionResult GetRoot()
        {
            return RedirectToAction(nameof(GetStatus));
        }
    }
}