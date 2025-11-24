using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data; // <== SỬA TẠI ĐÂY
using QUIZ_GAME_WEB.Models.QuizModels; // CauHoi, ChuDe, DoKho

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class QLCauHoiController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public QLCauHoiController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/admin/QLCauHoi 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CauHoi>>> GetCauHois()
        {
            return await _context.CauHois
                .Include(ch => ch.ChuDe)
                .Include(ch => ch.DoKho)
                .ToListAsync();
        }

        // GET: api/admin/QLCauHoi/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CauHoi>> GetCauHoi(int id)
        {
            var cauHoi = await _context.CauHois
                .Include(ch => ch.ChuDe)
                .Include(ch => ch.DoKho)
                .FirstOrDefaultAsync(ch => ch.CauHoiID == id);

            if (cauHoi == null)
            {
                return NotFound();
            }
            return cauHoi;
        }

        // POST: api/admin/QLCauHoi
        [HttpPost]
        public async Task<ActionResult<CauHoi>> PostCauHoi(CauHoi cauHoi)
        {
            // Logic nghiệp vụ: Kiểm tra ChuDeID và DoKhoID có hợp lệ không
            if (!await _context.ChuDes.AnyAsync(c => c.ChuDeID == cauHoi.ChuDeID) ||
                !await _context.DoKhos.AnyAsync(d => d.DoKhoID == cauHoi.DoKhoID))
            {
                return BadRequest("ChuDeID hoặc DoKhoID không hợp lệ.");
            }

            _context.CauHois.Add(cauHoi);
            await _context.SaveChangesAsync();

            // Lấy lại đối tượng đầy đủ để trả về client
            var createdCauHoi = await _context.CauHois
                .Include(ch => ch.ChuDe)
                .Include(ch => ch.DoKho)
                .FirstOrDefaultAsync(ch => ch.CauHoiID == cauHoi.CauHoiID);

            return CreatedAtAction(nameof(GetCauHoi), new { id = cauHoi.CauHoiID }, createdCauHoi);
        }

        // PUT: api/admin/QLCauHoi/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCauHoi(int id, CauHoi cauHoi)
        {
            if (id != cauHoi.CauHoiID)
            {
                return BadRequest("ID không khớp.");
            }

            // Logic nghiệp vụ: Kiểm tra khóa ngoại
            if (!await _context.ChuDes.AnyAsync(c => c.ChuDeID == cauHoi.ChuDeID) ||
                !await _context.DoKhos.AnyAsync(d => d.DoKhoID == cauHoi.DoKhoID))
            {
                return BadRequest("ChuDeID hoặc DoKhoID không hợp lệ.");
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
                throw;
            }

            return NoContent();
        }

        // DELETE: api/admin/QLCauHoi/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCauHoi(int id)
        {
            var cauHoi = await _context.CauHois.FindAsync(id);
            if (cauHoi == null)
            {
                return NotFound();
            }

            // Logic nghiệp vụ: Ngăn xóa nếu câu hỏi đang được tham chiếu
            if (await _context.CauSais.AnyAsync(cs => cs.CauHoiID == id) ||
                await _context.QuizNgays.AnyAsync(qn => qn.CauHoiID == id))
            {
                return Conflict("Không thể xóa. Câu hỏi này đang được sử dụng trong dữ liệu kết quả hoặc quiz hàng ngày.");
            }

            _context.CauHois.Remove(cauHoi);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}