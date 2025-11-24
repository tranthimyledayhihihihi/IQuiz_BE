using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities; // NguoiDung, Admin
using QUIZ_GAME_WEB.Models; // VaiTro

namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class QLNguoiDungController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public QLNguoiDungController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/admin/QLNguoiDung (Lấy danh sách người dùng)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NguoiDung>>> GetNguoiDungs()
        {
            // Logic nghiệp vụ: Lấy chi tiết Vai Trò Admin
            return await _context.NguoiDungs
                .Include(n => n.Admin)
                .ThenInclude(a => a.VaiTro)
                .ToListAsync();
        }

        // POST: api/admin/QLNguoiDung/ToggleStatus/{id}
        // Chức năng: Khóa/Mở khóa tài khoản (chuyển đổi TrangThai)
        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.NguoiDungs.FindAsync(id);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Logic nghiệp vụ: Không được khóa Super Admin (UserID=1)
            if (user.UserID == 1)
            {
                return Forbid("Không thể khóa tài khoản Super Admin.");
            }

            user.TrangThai = !user.TrangThai; // Đảo ngược trạng thái
            _context.NguoiDungs.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Đã {(user.TrangThai ? "mở khóa" : "khóa")} tài khoản {user.TenDangNhap}" });
        }

        // POST: api/admin/QLNguoiDung/SetAdminRole/{userId}/{vaiTroId}
        // Chức năng: Cấp/Hạ quyền Admin (thao tác trên bảng Admin)
        [HttpPost("SetAdminRole/{userId}/{vaiTroId}")]
        public async Task<IActionResult> SetAdminRole(int userId, int vaiTroId)
        {
            var user = await _context.NguoiDungs.FindAsync(userId);
            if (user == null) return NotFound("Người dùng không tồn tại.");

            if (!await _context.VaiTros.AnyAsync(v => v.VaiTroID == vaiTroId))
            {
                return BadRequest("VaiTroID không hợp lệ.");
            }

            var adminRecord = await _context.Admins.SingleOrDefaultAsync(a => a.UserID == userId);

            if (adminRecord == null)
            {
                // Thêm bản ghi Admin mới nếu người dùng chưa có
                adminRecord = new Models.CoreEntities.Admin { UserID = userId, VaiTroID = vaiTroId, TrangThai = true, NgayTao = DateTime.Now };
                _context.Admins.Add(adminRecord);
            }
            else
            {
                // Cập nhật VaiTroID
                adminRecord.VaiTroID = vaiTroId;
                _context.Admins.Update(adminRecord);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"Đã cập nhật Vai Trò Admin cho người dùng ID {userId}." });
        }
    }
}