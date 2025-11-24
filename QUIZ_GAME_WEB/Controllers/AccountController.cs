using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities; // NguoiDung
using System.Linq;
using System.Threading.Tasks;

// Namespace đã được đặt về thư mục gốc Controllers
namespace QUIZ_GAME_WEB.Controllers
{
    [Route("api/user/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public AccountController(QuizGameContext context)
        {
            _context = context;
        }

        // ===============================================
        // 🔑 API ĐỔI MẬT KHẨU (CHANGE PASSWORD)
        // ===============================================

        // POST: api/user/Account/ChangePassword
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model == null) return BadRequest("Dữ liệu không hợp lệ.");

            var user = await _context.NguoiDungs.FindAsync(model.UserID);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Logic nghiệp vụ: 1. Xác thực mật khẩu hiện tại
            if (!VerifyPassword(model.CurrentPassword, user.MatKhau))
            {
                return Unauthorized("Mật khẩu hiện tại không đúng.");
            }

            // Logic nghiệp vụ: 2. Cập nhật mật khẩu mới (đã hash)
            user.MatKhau = HashPassword(model.NewPassword);
            _context.NguoiDungs.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đổi mật khẩu thành công." });
        }

        // ===============================================
        // ✏️ API ĐỔI TÊN ĐĂNG NHẬP (CHANGE USERNAME)
        // ===============================================

        // POST: api/user/Account/ChangeUsername
        [HttpPost("ChangeUsername")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.NewUsername))
            {
                return BadRequest("Tên đăng nhập mới không được để trống.");
            }

            var user = await _context.NguoiDungs.FindAsync(model.UserID);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Logic nghiệp vụ: Kiểm tra tính duy nhất của tên đăng nhập mới
            if (await _context.NguoiDungs.AnyAsync(n => n.TenDangNhap == model.NewUsername))
            {
                return Conflict("Tên đăng nhập mới đã được sử dụng.");
            }

            user.TenDangNhap = model.NewUsername;
            _context.NguoiDungs.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Đổi tên đăng nhập thành công." });
        }


        // ===============================================
        // HÀM HỖ TRỢ (ĐƯỢC ĐẶT NỘI BỘ TRONG CONTROLLER)
        // ===============================================

        private string HashPassword(string password)
        {
            // CHÚ Ý: Thay bằng logic hashing thực tế (ví dụ: BCrypt)
            return $"hashed_{password}_password";
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            // CHÚ Ý: Thay bằng logic verification thực tế
            return hashedPassword == HashPassword(inputPassword);
        }
    }

    // DTO-LIKE MODELS

    public class ChangePasswordModel
    {
        public int UserID { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangeUsernameModel
    {
        public int UserID { get; set; }
        public string NewUsername { get; set; }
    }
}