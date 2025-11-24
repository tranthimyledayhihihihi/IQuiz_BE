using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities; // NguoiDung, CaiDatNguoiDung
using System.Linq;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/user/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ProfileController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/user/Profile/{userId}
        // Chức năng: Lấy toàn bộ hồ sơ (NguoiDung + CaiDat)
        [HttpGet("{userId}")]
        public async Task<ActionResult<NguoiDung>> GetUserProfile(int userId)
        {
            var user = await _context.NguoiDungs
                // ❌ DÒNG LỖI CŨ: .Include(n => n.CaiDatNguoiDungs) 
                // ✅ DÒNG ĐÃ SỬA:
                .Include(n => n.CaiDat)
                .FirstOrDefaultAsync(n => n.UserID == userId);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Logic bảo mật: Ẩn mật khẩu trước khi trả về
            user.MatKhau = null;

            return user;
        }

        // ... (Các phương thức khác giữ nguyên) ...

        // PUT: api/user/Profile/{userId}/Update
        [HttpPut("{userId}/Update")]
        public async Task<IActionResult> UpdateUserProfile(int userId, [FromBody] NguoiDung updatedUser)
        {
            // ... code cập nhật NguoiDung ...
            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            bool hasChanges = false;

            if (updatedUser.HoTen != null)
            {
                user.HoTen = updatedUser.HoTen;
                hasChanges = true;
            }

            if (updatedUser.AnhDaiDien != null)
            {
                user.AnhDaiDien = updatedUser.AnhDaiDien;
                hasChanges = true;
            }

            if (updatedUser.Email != null && user.Email != updatedUser.Email)
            {
                if (await _context.NguoiDungs.AnyAsync(n => n.Email == updatedUser.Email && n.UserID != userId))
                {
                    return Conflict("Email đã được sử dụng bởi tài khoản khác.");
                }
                user.Email = updatedUser.Email;
                hasChanges = true;
            }

            if (hasChanges)
            {
                _context.NguoiDungs.Update(user);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        // PUT: api/user/Profile/{userId}/Settings
        [HttpPut("{userId}/Settings")]
        public async Task<IActionResult> UpdateUserSettings(int userId, [FromBody] CaiDatNguoiDung updatedSettings)
        {
            var setting = await _context.CaiDatNguoiDungs.FirstOrDefaultAsync(c => c.UserID == userId);

            if (setting == null)
            {
                setting = new CaiDatNguoiDung { UserID = userId };
                _context.CaiDatNguoiDungs.Add(setting);
            }

            setting.AmThanh = updatedSettings.AmThanh;
            setting.NhacNen = updatedSettings.NhacNen;
            setting.ThongBao = updatedSettings.ThongBao;

            if (updatedSettings.NgonNgu != null)
            {
                setting.NgonNgu = updatedSettings.NgonNgu;
            }

            _context.CaiDatNguoiDungs.Update(setting);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}