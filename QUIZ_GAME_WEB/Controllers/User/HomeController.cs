using Microsoft.AspNetCore.Mvc;
using QUIZ_GAME_WEB.Data;

namespace QUIZ_GAME_WEB.Controllers.User
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

        // GET: api/home/dashboard
        // [HttpGet("dashboard")]
        // public IActionResult GetDashboardData()
        // {
        //     // Code logic để lấy dữ liệu dashboard (chủ đề, điểm số,...)
        //     return Ok();
        // }
    }
}