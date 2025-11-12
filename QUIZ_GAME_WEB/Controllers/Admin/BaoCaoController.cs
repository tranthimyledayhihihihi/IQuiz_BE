using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models; // Cần dùng BaoCaoRequestModel
using System.Linq;
using System.Threading.Tasks;

// Namespace phải khớp với thư mục 'Admin'
namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BaoCaoController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public BaoCaoController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// (Admin) Xuất báo cáo dựa trên loại yêu cầu.
        /// </summary>
        /// <remarks>
        /// Tạm thời trả về JSON. Trong dự án thật, bạn sẽ dùng
        /// thư viện như EPPlus/CsvHelper để trả về File().
        /// </remarks>
        // POST: api/baocao/export
        [HttpPost("export")]
        public async Task<IActionResult> ExportReport([FromBody] BaoCaoRequestModel request)
        {
            if (string.IsNullOrEmpty(request.LoaiBaoCao))
            {
                return BadRequest(new { message = "Loại báo cáo là bắt buộc." });
            }

            // Mặc định ngày bắt đầu/kết thúc nếu bị null
            var startDate = request.NgayBatDau ?? DateTime.MinValue;
            var endDate = request.NgayKetThuc ?? DateTime.MaxValue;

            object reportData; // Dùng 'object' để chứa nhiều loại báo cáo

            switch (request.LoaiBaoCao.ToLower())
            {
                // Báo cáo danh sách người dùng
                case "nguoidung":
                    reportData = await _context.NguoiDungs
                        .Where(u => u.NgayDangKy >= startDate && u.NgayDangKy <= endDate)
                        .Select(u => new NguoiDungAdminViewModel // Dùng lại ViewModel
                        {
                            UserID = u.UserID,
                            TenDangNhap = u.TenDangNhap,
                            Email = u.Email,
                            HoTen = u.HoTen,
                            NgayDangKy = u.NgayDangKy,
                            LanDangNhapCuoi = u.LanDangNhapCuoi,
                            TrangThai = u.TrangThai
                        })
                        .ToListAsync();
                    break;

                // Báo cáo kết quả chơi game
                case "ketqua":
                    reportData = await _context.KetQuas
                        .Where(k => k.ThoiGian >= startDate && k.ThoiGian <= endDate)
                        .Include(k => k.NguoiDung)
                        .Select(k => new
                        {
                            k.KetQuaID,
                            k.NguoiDung.TenDangNhap,
                            k.Diem,
                            k.SoCauDung,
                            k.TongCauHoi,
                            k.TrangThai,
                            k.ThoiGian
                        })
                        .OrderByDescending(k => k.ThoiGian)
                        .ToListAsync();
                    break;

                default:
                    return BadRequest(new { message = "Loại báo cáo không hợp lệ." });
            }

            // Thay vì trả về File(), chúng ta trả về JSON
            return Ok(reportData);
        }
    }
}