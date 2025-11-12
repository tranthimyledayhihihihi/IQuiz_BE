using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using System.Threading.Tasks;

// Namespace phải khớp với thư mục 'Admin'
namespace QUIZ_GAME_WEB.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HeThongController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public HeThongController(QuizGameContext context)
        {
            _context = context;
        }

        /// <summary>
        /// (Admin) Lấy cài đặt hệ thống hiện tại (Mô phỏng).
        /// </summary>
        /// <remarks>
        /// Trong dự án thật, bạn sẽ đọc các cài đặt này từ
        /// một bảng 'CaiDatHeThong' trong DB hoặc từ appsettings.json.
        /// </remarks>
        // GET: api/hethong/settings
        [HttpGet("settings")]
        public IActionResult GetSystemSettings()
        {
            // Đây là dữ liệu mô phỏng (mock)
            var settings = new
            {
                ChoPhepDangKyMoi = true,
                CheDoBaoTri = false,
                PhienBanApi = "1.0.0"
            };
            return Ok(settings);
        }

        /// <summary>
        /// (Admin) Cập nhật cài đặt hệ thống (Mô phỏng).
        /// </summary>
        // POST: api/hethong/settings
        [HttpPost("settings")]
        public IActionResult UpdateSystemSettings([FromBody] object settings)
        {
            // Logic mô phỏng:
            // 1. Nhận 'settings'
            // 2. Lưu vào database hoặc appsettings.json

            // Trả về thành công
            return Ok(new { message = "Cài đặt hệ thống đã được cập nhật (mô phỏng)." });
        }


        /// <summary>
        /// (Admin) Kích hoạt một tác vụ sao lưu database (Mô phỏng).
        /// </summary>
        /// <remarks>
        /// API này chỉ mô phỏng việc sao lưu.
        /// Backup thật sự cần chạy lệnh SQL 'BACKUP DATABASE ...'
        /// </remarks>
        // POST: api/hethong/backup
        [HttpPost("backup")]
        public async Task<IActionResult> BackupDatabase()
        {
            try
            {
                // -- LOGIC MÔ PHỎNG --
                // (Chờ 2 giây để giả vờ đang backup)
                await Task.Delay(2000);

                // -- LOGIC THẬT SỰ (Ví dụ) --
                // string dbName = "quiz_game";
                // string backupPath = $"D:\\Backups\\quiz_game_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                // string sqlCommand = $"BACKUP DATABASE [{dbName}] TO DISK = '{backupPath}'";
                // await _context.Database.ExecuteSqlRawAsync(sqlCommand);
                // -------------------------

                return Ok(new { message = "Sao lưu database thành công (mô phỏng)." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi sao lưu database.", error = ex.Message });
            }
        }
    }
}