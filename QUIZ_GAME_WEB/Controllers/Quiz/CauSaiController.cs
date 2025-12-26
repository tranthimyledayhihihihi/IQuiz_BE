using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.ResultsModels;
using System.Security.Claims;

namespace QUIZ_GAME_WEB.Controllers.Quiz
{
    [ApiController]
    [Route("api/quiz/causai")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CauSaiController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public CauSaiController(QuizGameContext context)
        {
            _context = context;
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("userId");

            if (int.TryParse(idStr, out var id))
                return id;

            return null;
        }

        // =========================================
        // GET: api/quiz/causai/recent?limit=10
        // =========================================
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentWrongAnswers([FromQuery] int limit = 10)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

            if (limit <= 0) limit = 10;

            var list = await _context.CauSais
    .Include(cs => cs.CauHoi)
        .ThenInclude(ch => ch.ChuDe)
    .Where(cs => cs.UserID == userId.Value)
    .OrderByDescending(cs => cs.NgaySai)
    .Take(limit)
    .Select(cs => new
    {
        cauSaiID = cs.CauSaiID,
        userID = cs.UserID,
        cauHoiID = cs.CauHoiID,
        quizAttemptID = cs.QuizAttemptID,
        ngaySai = cs.NgaySai,

        cauHoi = cs.CauHoi.NoiDung,
        dapAnA = cs.CauHoi.DapAnA,
        dapAnB = cs.CauHoi.DapAnB,
        dapAnC = cs.CauHoi.DapAnC,
        dapAnD = cs.CauHoi.DapAnD,
        dapAnChinhXac = cs.CauHoi.DapAnDung,

        tenChuDe = cs.CauHoi.ChuDe.TenChuDe
    })
    .ToListAsync();

            return Ok(new
            {
                success = true,
                data = list
            });


            return Ok(new
            {
                success = true,
                total = list.Count,
                data = list
            });
        }

        // =========================================
        // GET: api/quiz/causai/count/{attemptId}
        // =========================================
        [HttpGet("count/{attemptId:int}")]
        public async Task<IActionResult> CountWrongAnswers(int attemptId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

            var count = await _context.CauSais.CountAsync(cs =>
                cs.UserID == userId.Value &&
                cs.QuizAttemptID == attemptId
            );

            return Ok(new { attemptId, count });
        }
        /// <summary>
        /// Lấy danh sách câu sai theo chủ đề
        /// GET: api/quiz/causai/by-topic/{chuDeId}
        /// </summary>
        [HttpGet("by-topic/{chuDeId:int}")]
        public async Task<IActionResult> GetWrongAnswersByTopic(int chuDeId)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized(new { message = "Không tìm thấy UserID trong token." });

            var list = await _context.CauSais
                .Include(cs => cs.CauHoi)
                    .ThenInclude(ch => ch.ChuDe)
                .Where(cs =>
                    cs.UserID == userId.Value &&
                    cs.CauHoi.ChuDeID == chuDeId
                )
                .OrderByDescending(cs => cs.NgaySai)
                .Select(cs => new
                {
                    cauSaiID = cs.CauSaiID,
                    cauHoiID = cs.CauHoiID,
                    quizAttemptID = cs.QuizAttemptID,
                    ngaySai = cs.NgaySai,

                    cauHoi = cs.CauHoi.NoiDung,
                    dapAnA = cs.CauHoi.DapAnA,
                    dapAnB = cs.CauHoi.DapAnB,
                    dapAnC = cs.CauHoi.DapAnC,
                    dapAnD = cs.CauHoi.DapAnD,
                    dapAnChinhXac = cs.CauHoi.DapAnDung,

                    tenChuDe = cs.CauHoi.ChuDe.TenChuDe
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                data = list
            });
        }


    }
}
