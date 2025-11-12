using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

// Namespace phải khớp với thư mục 'Admin'
namespace QUIZ_GAME_WEB.Controllers.Admin
{
     [Authorize(Roles = "Admin")]
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public AdminController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// (Admin) Lấy các số liệu thống kê tổng quan cho Dashboard.
        /// </summary>
        // GET: api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var today = DateTime.Today;

            // === SỬA LỖI: CHẠY CÁC TRUY VẤN TUẦN TỰ ===
            // DbContext không hỗ trợ chạy song song,
            // nên chúng ta 'await' từng cái một.
            int tongNguoiDung = await _context.NguoiDungs.CountAsync();
            int nguoiDungMoi = await _context.NguoiDungs.CountAsync(u => u.NgayDangKy.Date == today);
            int tongCauHoi = await _context.CauHois.CountAsync();
            int soTranHomNay = await _context.KetQuas.CountAsync(k => k.ThoiGian.Date == today);
            int tongChuDe = await _context.ChuDes.CountAsync();
            // ==========================================

            // Lấy kết quả
            var stats = new AdminDashboardModel
            {
                TongSoNguoiDung = tongNguoiDung,
                NguoiDungMoiHomNay = nguoiDungMoi,
                TongSoCauHoi = tongCauHoi,
                TongSoChuDe = tongChuDe,
                SoTranDauHomNay = soTranHomNay
            };

            return Ok(stats);
        }
    }
}