using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.Threading.Tasks;

// Namespace phải khớp với thư mục 'Admin'
namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QLChuDeController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public QLChuDeController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// (Admin) Lấy TẤT CẢ chủ đề (bao gồm cả chủ đề bị ẩn).
        /// </summary>
        // GET: api/QLChuDe
        [HttpGet]
        public async Task<IActionResult> GetTatCaChuDe()
        {
            var chuDes = await _context.ChuDes.ToListAsync();
            return Ok(chuDes);
        }

        /// <summary>
        /// (Admin) Lấy chi tiết 1 chủ đề.
        /// </summary>
        // GET: api/QLChuDe/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChuDe(int id)
        {
            var chuDe = await _context.ChuDes.FindAsync(id);

            if (chuDe == null)
            {
                return NotFound();
            }

            return Ok(chuDe);
        }

        /// <summary>
        /// (Admin) Tạo một chủ đề mới.
        /// </summary>
        // POST: api/QLChuDe
        [HttpPost]
        public async Task<IActionResult> CreateChuDe([FromBody] ChuDe chuDe)
        {
            // Bỏ qua ChuDeID vì nó là IDENTITY
            var newChuDe = new ChuDe
            {
                TenChuDe = chuDe.TenChuDe,
                MoTa = chuDe.MoTa,
                TrangThai = chuDe.TrangThai
            };

            _context.ChuDes.Add(newChuDe);
            await _context.SaveChangesAsync();

            // Trả về kết quả 201 Created với đối tượng vừa tạo
            return CreatedAtAction(nameof(GetChuDe), new { id = newChuDe.ChuDeID }, newChuDe);
        }

        /// <summary>
        /// (Admin) Cập nhật một chủ đề.
        /// </summary>
        // PUT: api/QLChuDe/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChuDe(int id, [FromBody] ChuDe chuDe)
        {
            if (id != chuDe.ChuDeID)
            {
                return BadRequest();
            }

            _context.Entry(chuDe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ChuDes.Any(e => e.ChuDeID == id))
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
        /// (Admin) Xóa một chủ đề.
        /// </summary>
        // DELETE: api/QLChuDe/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChuDe(int id)
        {
            var chuDe = await _context.ChuDes.FindAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }

            _context.ChuDes.Remove(chuDe);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - Xóa thành công
        }
    }
}