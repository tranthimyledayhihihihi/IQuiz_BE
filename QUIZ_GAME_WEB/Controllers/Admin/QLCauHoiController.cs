using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.Linq;
using System.Threading.Tasks;

// Namespace phải khớp với thư mục 'Admin'
namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QLCauHoiController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public QLCauHoiController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// (Admin) Lấy danh sách câu hỏi (có phân trang và join).
        /// </summary>
        // GET: api/QLCauHoi
        [HttpGet]
        public async Task<IActionResult> GetCauHois([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.CauHois
                .Include(c => c.ChuDe) // Join với bảng Chủ Đề
                .Include(c => c.DoKho) // Join với bảng Độ Khó
                .OrderBy(c => c.CauHoiID);

            var totalItems = await query.CountAsync();

            var cauHois = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new CauHoiAdminViewModel // Sử dụng ViewModel
                {
                    CauHoiID = q.CauHoiID,
                    NoiDung = q.NoiDung,
                    TenChuDe = q.ChuDe.TenChuDe, // Lấy tên chủ đề
                    TenDoKho = q.DoKho.TenDoKho, // Lấy tên độ khó
                    DapAnDung = q.DapAnDung
                })
                .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Data = cauHois
            });
        }

        /// <summary>
        /// (Admin) Lấy chi tiết 1 câu hỏi (dùng Model gốc).
        /// </summary>
        // GET: api/QLCauHoi/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCauHoi(int id)
        {
            var cauHoi = await _context.CauHois.FindAsync(id);

            if (cauHoi == null)
            {
                return NotFound();
            }

            return Ok(cauHoi);
        }

        /// <summary>
        /// (Admin) Tạo một câu hỏi mới.
        /// </summary>
        // POST: api/QLCauHoi
        [HttpPost]
        public async Task<IActionResult> CreateCauHoi([FromBody] CauHoi cauHoi)
        {
            // Chúng ta dùng trực tiếp Model 'CauHoi' để tạo
            _context.CauHois.Add(cauHoi);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCauHoi), new { id = cauHoi.CauHoiID }, cauHoi);
        }

        /// <summary>
        /// (Admin) Cập nhật một câu hỏi.
        /// </summary>
        // PUT: api/QLCauHoi/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCauHoi(int id, [FromBody] CauHoi cauHoi)
        {
            if (id != cauHoi.CauHoiID)
            {
                return BadRequest();
            }

            _context.Entry(cauHoi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.CauHois.Any(e => e.CauHoiID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content - Cập nhật thành công
        }

        /// <summary>
        /// (Admin) Xóa một câu hỏi.
        /// </summary>
        // DELETE: api/QLCauHoi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCauHoi(int id)
        {
            var cauHoi = await _context.CauHois.FindAsync(id);
            if (cauHoi == null)
            {
                return NotFound();
            }

            _context.CauHois.Remove(cauHoi);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - Xóa thành công
        }
    }
}