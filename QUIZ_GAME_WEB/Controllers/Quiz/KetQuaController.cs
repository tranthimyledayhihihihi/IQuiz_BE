using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/quiz/[controller]")]
    [ApiController]
    public class KetQuaController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public KetQuaController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/quiz/KetQua/User/{userId}
        // Chức năng: Lấy lịch sử kết quả chơi game của một người dùng
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<KetQua>>> GetResultsByUserId(int userId)
        {
            // Logic Nghiệp vụ: Lấy lịch sử và sắp xếp theo thời gian
            var results = await _context.KetQuas
                .Where(kq => kq.UserID == userId)
                .OrderByDescending(kq => kq.ThoiGian)
                .Take(50) // Giới hạn 50 kết quả gần nhất
                .ToListAsync();

            if (!results.Any())
            {
                return NotFound("Không tìm thấy kết quả nào cho người dùng này.");
            }

            return Ok(results);
        }

        // GET: api/quiz/KetQua/{id}
        // Chức năng: Lấy chi tiết một kết quả cụ thể
        [HttpGet("{id}")]
        public async Task<ActionResult<KetQua>> GetKetQuaDetail(int id)
        {
            var ketQua = await _context.KetQuas
                .FirstOrDefaultAsync(kq => kq.KetQuaID == id);

            if (ketQua == null)
            {
                return NotFound("Không tìm thấy chi tiết kết quả.");
            }

            return ketQua;
        }
    }
}