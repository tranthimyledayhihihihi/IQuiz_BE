using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities; // VaiTro, Quyen, VaiTro_Quyen

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public AdminController(QuizGameContext context)
        {
            _context = context;
        }

        // ===============================================
        // A. QUẢN LÝ VAI TRÒ (VaiTro CRUD)
        // ===============================================


        // GET: api/admin/Admin/VaiTro
        [HttpGet("VaiTro")]
        public async Task<ActionResult<IEnumerable<VaiTro>>> GetVaiTros()
        {
            return await _context.VaiTros.ToListAsync();
        }


        // POST: api/admin/Admin/VaiTro
        [HttpPost("VaiTro")]
        public async Task<ActionResult<VaiTro>> PostVaiTro(VaiTro vaiTro)
        {
            // Logic nghiệp vụ: Kiểm tra tên vai trò đã tồn tại chưa
            if (_context.VaiTros.Any(v => v.TenVaiTro == vaiTro.TenVaiTro))
            {
                return Conflict("Tên Vai Trò đã tồn tại.");
            }

            _context.VaiTros.Add(vaiTro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVaiTros), new { id = vaiTro.VaiTroID }, vaiTro);
        }

        // ... (Các phương thức PUT/DELETE cho VaiTro)

        // ===============================================
        // B. QUẢN LÝ PHÂN QUYỀN (VaiTro_Quyen)
        // ===============================================

        // GET: api/admin/Admin/PhanQuyen/{vaiTroId}
        [HttpGet("PhanQuyen/{vaiTroId}")]
        public async Task<ActionResult<IEnumerable<Quyen>>> GetQuyenByVaiTro(int vaiTroId)
        {
            var quyenIds = await _context.VaiTroQuyens
                .Where(vq => vq.VaiTroID == vaiTroId)
                .Select(vq => vq.QuyenID)
                .ToListAsync();

            var quyens = await _context.Quyens
                .Where(q => quyenIds.Contains(q.QuyenID))
                .ToListAsync();

            return quyens;
        }


        // POST: api/admin/Admin/PhanQuyen/{vaiTroId}
        [HttpPost("PhanQuyen/{vaiTroId}")]
        public async Task<IActionResult> UpdatePhanQuyen(int vaiTroId, [FromBody] PhanQuyenUpdateModel model)
        {
            if (!_context.VaiTros.Any(v => v.VaiTroID == vaiTroId))
            {
                return NotFound("Vai Trò không tồn tại.");
            }

            // --- LOGIC NGHIỆP VỤ: XÓA VÀ TẠO LẠI ---

            // 1. Xóa tất cả quyền cũ
            var currentPermissions = await _context.VaiTroQuyens
                .Where(vq => vq.VaiTroID == vaiTroId)
                .ToListAsync();

            _context.VaiTroQuyens.RemoveRange(currentPermissions);

            // 2. Thêm quyền mới
            var newPermissions = model.QuyenIds
                .Select(quyenId => new VaiTroQuyen { VaiTroID = vaiTroId, QuyenID = quyenId })
                .ToList();

            _context.VaiTroQuyens.AddRange(newPermissions);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Đã cập nhật {newPermissions.Count} quyền cho Vai Trò ID: {vaiTroId}" });
        }
    }

    // DTO (Data Transfer Object) cho đầu vào cập nhật phân quyền
    public class PhanQuyenUpdateModel
    {
        public List<int> QuyenIds { get; set; } = new List<int>();
    }
}