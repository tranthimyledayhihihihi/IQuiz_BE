using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // <-- Đã có
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

// Namespace của bạn (có thể là .User)
namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly IConfiguration _config;

        public AccountController(QuizGameContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ... (Hàm Register của bạn giữ nguyên) ...
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DangKyModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (await _context.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap))
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });
            if (await _context.NguoiDungs.AnyAsync(u => u.Email == model.Email))
                return BadRequest(new { message = "Email đã tồn tại" });

            var newUser = new NguoiDung
            {
                TenDangNhap = model.TenDangNhap,
                Email = model.Email,
                MatKhau = model.MatKhau, // ⚠️
                HoTen = model.HoTen,
                NgayDangKy = DateTime.Now,
                TrangThai = true
            };
            _context.NguoiDungs.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đăng ký thành công" });
        }

        // ... (Hàm Login của bạn giữ nguyên) ...
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DangNhapModel model)
        {
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap);

            if (user == null || user.MatKhau != model.MatKhau)
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng" });
            }

            user.LanDangNhapCuoi = DateTime.Now;
            _context.NguoiDungs.Update(user);
            await _context.SaveChangesAsync();

            var tokenString = CreateToken(user);

            return Ok(new
            {
                token = tokenString,
                userId = user.UserID,
                tenNguoiDung = user.HoTen
            });
        }

        // === SỬA LẠI HÀM CreateToken ===
        private string CreateToken(NguoiDung user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tạo danh sách Claims
            var claims = new List<Claim> // <-- Đổi thành List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.TenDangNhap),
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
            };

            // === THÊM LOGIC PHÂN QUYỀN TẠI ĐÂY ===
            if (user.TenDangNhap.ToLower() == "admin")
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }
            // ======================================

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims, // <-- Sử dụng danh sách claims đã có quyền
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}