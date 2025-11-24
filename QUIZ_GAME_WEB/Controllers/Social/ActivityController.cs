using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.CoreEntities; // NguoiDung
using QUIZ_GAME_WEB.Models.SocialRankingModels; // NguoiDungOnline, ChuoiNgay
using System.Linq;

namespace QUIZ_GAME_WEB.Controllers.Social
{
    [Route("api/social/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ActivityController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/social/Activity/OnlineUsers
        // Chức năng: Lấy danh sách người dùng đang online (Logic nghiệp vụ: Cập nhật trong 5 phút gần nhất)
        [HttpGet("OnlineUsers")]
        public async Task<ActionResult<IEnumerable<NguoiDungOnline>>> GetOnlineUsers()
        {
            var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

            // Logic nghiệp vụ: Lấy người dùng online và include thông tin cơ bản của họ
            var onlineUsers = await _context.NguoiDungOnlines
                .Where(u => u.TrangThai == "Online" && u.ThoiGianCapNhat >= fiveMinutesAgo)
                .Include(u => u.NguoiDung)
                .ToListAsync();

            return Ok(onlineUsers);
        }

        // POST: api/social/Activity/UpdateStatus/{userId}
        // Chức năng: Cập nhật trạng thái và thời gian hoạt động của người dùng
        [HttpPost("UpdateStatus/{userId}")]
        public async Task<IActionResult> UpdateUserStatus(int userId, [FromQuery] string status = "Online")
        {
            var onlineRecord = await _context.NguoiDungOnlines
                .SingleOrDefaultAsync(o => o.UserID == userId);

            if (onlineRecord == null)
            {
                // Thêm mới nếu chưa có
                onlineRecord = new NguoiDungOnline
                {
                    UserID = userId,
                    TrangThai = status,
                    ThoiGianCapNhat = DateTime.Now
                };
                _context.NguoiDungOnlines.Add(onlineRecord);
            }
            else
            {
                // Cập nhật
                onlineRecord.TrangThai = status;
                onlineRecord.ThoiGianCapNhat = DateTime.Now;
                _context.NguoiDungOnlines.Update(onlineRecord);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/social/Activity/UpdateStreak/{userId}
        // Chức năng: Cập nhật chuỗi ngày chơi liên tiếp (Logic nghiệp vụ)
        [HttpPost("UpdateStreak/{userId}")]
        public async Task<IActionResult> UpdateUserStreak(int userId)
        {
            var chuoiNgay = await _context.ChuoiNgays.SingleOrDefaultAsync(c => c.UserID == userId);
            var today = DateTime.Today;

            if (chuoiNgay == null)
            {
                // Thêm mới nếu lần chơi đầu tiên
                _context.ChuoiNgays.Add(new ChuoiNgay { UserID = userId, SoNgayLienTiep = 1, NgayCapNhatCuoi = today });
            }
            else
            {
                var lastUpdateDay = chuoiNgay.NgayCapNhatCuoi.Date;

                if (lastUpdateDay == today)
                {
                    // Đã chơi hôm nay, không làm gì
                    return Ok(new { Message = "Streak đã được ghi nhận hôm nay." });
                }
                else if (lastUpdateDay == today.AddDays(-1))
                {
                    // Chơi liên tiếp
                    chuoiNgay.SoNgayLienTiep += 1;
                    chuoiNgay.NgayCapNhatCuoi = today;
                    _context.ChuoiNgays.Update(chuoiNgay);
                }
                else
                {
                    // Bị đứt chuỗi
                    chuoiNgay.SoNgayLienTiep = 1;
                    chuoiNgay.NgayCapNhatCuoi = today;
                    _context.ChuoiNgays.Update(chuoiNgay);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Chuỗi ngày chơi đã cập nhật." });
        }
    }
}