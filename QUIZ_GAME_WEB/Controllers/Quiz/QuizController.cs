using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.QuizModels;
using System.Linq;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [Route("api/quiz/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public QuizController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/quiz/Quiz/ChuDe
        // Chức năng: Lấy danh sách Chủ đề đang hoạt động
        [HttpGet("ChuDe")]
        public async Task<ActionResult<IEnumerable<ChuDe>>> GetChuDes()
        {
            return await _context.ChuDes.Where(c => c.TrangThai == true).ToListAsync();
        }

        // GET: api/quiz/Quiz/DoKho
        // Chức năng: Lấy danh sách Độ khó
        [HttpGet("DoKho")]
        public async Task<ActionResult<IEnumerable<DoKho>>> GetDoKhos()
        {
            return await _context.DoKhos.ToListAsync();
        }

        // GET: api/quiz/Quiz/TroGiup
        // Chức năng: Lấy danh sách các loại Trợ giúp
        [HttpGet("TroGiup")]
        public async Task<ActionResult<IEnumerable<TroGiup>>> GetTroGiups()
        {
            return await _context.TroGiups.ToListAsync();
        }

        // GET: api/quiz/Quiz/QuizNgay
        // Chức năng: Lấy Câu hỏi của Quiz Ngày
        [HttpGet("QuizNgay")]
        public async Task<ActionResult<CauHoi>> GetDailyQuiz()
        {
            var today = DateTime.Today;

            // Logic Nghiệp vụ: Tìm QuizNgay cho hôm nay
            var quizNgay = await _context.QuizNgays
                .Include(qn => qn.CauHoi)
                .FirstOrDefaultAsync(qn => qn.Ngay == today);

            if (quizNgay?.CauHoi == null)
            {
                return NotFound("Quiz Ngày hôm nay chưa được thiết lập.");
            }

            // Logic bảo mật: ẨN đáp án đúng
            quizNgay.CauHoi.DapAnDung = null;

            return quizNgay.CauHoi;
        }
    }
}