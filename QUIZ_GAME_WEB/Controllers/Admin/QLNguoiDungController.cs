using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// Namespace phải khớp với thư mục 'Admin'
namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QLNguoiDungController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public QLNguoiDungController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// (Admin) Lấy danh sách người dùng (có phân trang).
        /// </summary>
        // GET: api/QLNguoiDung
        [HttpGet]
        public async Task<IActionResult> GetNguoiDungs([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.NguoiDungs
                .OrderByDescending(u => u.NgayDangKy); // Người mới nhất lên đầu

            var totalItems = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new NguoiDungAdminViewModel // Sử dụng ViewModel
                {
                    UserID = u.UserID,
                    TenDangNhap = u.TenDangNhap,
                    Email = u.Email,
                    HoTen = u.HoTen,
                    NgayDangKy = u.NgayDangKy,
                    LanDangNhapCuoi = u.LanDangNhapCuoi,
                    TrangThai = u.TrangThai
                    // Không bao giờ trả về MatKhau!
                })
                .ToListAsync();

            return Ok(new
            {
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize,
                Data = users
            });
        }

        /// <summary>
        /// (Admin) Lấy chi tiết 1 người dùng (vẫn dùng ViewModel).
        /// </summary>
        // GET: api/QLNguoiDung/2
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNguoiDung(int id)
        {
            var user = await _context.NguoiDungs
                .Where(u => u.UserID == id)
                .Select(u => new NguoiDungAdminViewModel // Dùng ViewModel
                {
                    UserID = u.UserID,
                    TenDangNhap = u.TenDangNhap,
                    Email = u.Email,
                    HoTen = u.HoTen,
                    NgayDangKy = u.NgayDangKy,
                    LanDangNhapCuoi = u.LanDangNhapCuoi,
                    TrangThai = u.TrangThai
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        /// <summary>
        /// (Admin) Cập nhật trạng thái (Khóa/Mở khóa) tài khoản.
        /// </summary>
        // PUT: api/QLNguoiDung/2/SetTrangThai
        [HttpPut("{id}/SetTrangThai")]
        public async Task<IActionResult> SetTrangThaiNguoiDung(int id, [FromBody] bool trangThai)
        {
            var user = await _context.NguoiDungs.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            // Không cho phép admin tự khóa chính mình
            var adminIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user.UserID.ToString() == adminIdString)
            {
                return BadRequest(new { message = "Không thể tự khóa tài khoản Admin." });
            }

            user.TrangThai = trangThai; // trangThai = false (Khóa), true (Mở)
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content - Cập nhật thành công
        }
    }
}