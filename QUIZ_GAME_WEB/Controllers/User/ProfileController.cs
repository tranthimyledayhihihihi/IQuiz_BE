using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ProfileController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy thông tin hồ sơ của người dùng đang đăng nhập.
        /// </summary>
        // GET: api/profile/me
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdString!);

            // Lấy thông tin user, không bao giờ lấy MatKhau
            var userProfile = await _context.NguoiDungs
                .Where(u => u.UserID == userId)
                .Select(u => new
                {
                    u.UserID,
                    u.TenDangNhap,
                    u.Email,
                    u.HoTen,
                    u.AnhDaiDien,
                    u.NgayDangKy
                })
                .FirstOrDefaultAsync();

            if (userProfile == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            return Ok(userProfile);
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ (Họ Tên, Ảnh đại diện).
        /// </summary>
        // PUT: api/profile/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateModel model)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdString!);

            var user = await _context.NguoiDungs.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật các trường được phép
            user.HoTen = model.HoTen;
            user.AnhDaiDien = model.AnhDaiDien;
            // Không cho phép đổi Email/TenDangNhap ở đây

            _context.NguoiDungs.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật hồ sơ thành công." });
        }
    }
}

// === TẠO MODEL MỚI CHO VIỆC UPDATE ===
// Bạn có thể tạo tệp `ProfileUpdateModel.cs` riêng trong thư mục Models
// Hoặc để tạm ở đây cũng được
public class ProfileUpdateModel
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [MaxLength(100)]
    public string? HoTen { get; set; }

    [MaxLength(255)]
    public string? AnhDaiDien { get; set; } // URL to avatar
}