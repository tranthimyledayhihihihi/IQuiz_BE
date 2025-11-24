using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.CoreEntities; // NguoiDung
using QUIZ_GAME_WEB.Models.ResultsModels; // ThanhTuu, ThuongNgay
using QUIZ_GAME_WEB.Models.SocialRankingModels; // ChuoiNgay

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/user/[controller]")]
    [ApiController]
    public class ThanhTuuController : ControllerBase
    {
        private readonly QuizGameContext _context;

        public ThanhTuuController(QuizGameContext context)
        {
            _context = context;
        }

        // GET: api/user/ThanhTuu/All
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<ThanhTuu>>> GetAllThanhTuus()
        {
            return await _context.ThanhTuus.ToListAsync();
        }

        // GET: api/user/ThanhTuu/User/{userId}
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<ThanhTuu>>> GetUserAchievedThanhTuus(int userId)
        {
            // Logic nghiệp vụ: Giả định chỉ trả về các thành tựu mẫu
            return await _context.ThanhTuus.Take(5).ToListAsync();
        }

        // GET: api/user/ThanhTuu/DailyReward/{userId}
        [HttpGet("DailyReward/{userId}")]
        public async Task<ActionResult<ThuongNgay>> GetDailyRewardStatus(int userId)
        {
            var today = DateTime.Today;
            var thuongNgay = await _context.ThuongNgays
                .FirstOrDefaultAsync(t => t.UserID == userId && t.NgayNhan == today);

            if (thuongNgay != null)
            {
                return Ok(thuongNgay);
            }

            // Logic nghiệp vụ: Nếu chưa nhận hôm nay, tạo đối tượng thưởng mới để gợi ý
            return Ok(new ThuongNgay
            {
                UserID = userId,
                NgayNhan = today,
                PhanThuong = "100 điểm thưởng",
                DiemThuong = 100,
                TrangThaiNhan = false
            });
        }

        // POST: api/user/ThanhTuu/ClaimDailyReward/{userId}
        [HttpPost("ClaimDailyReward/{userId}")]
        public async Task<IActionResult> ClaimDailyReward(int userId)
        {
            var today = DateTime.Today;
            var thuongNgay = await _context.ThuongNgays
                .FirstOrDefaultAsync(t => t.UserID == userId && t.NgayNhan == today);

            if (thuongNgay != null && thuongNgay.TrangThaiNhan == true)
            {
                return Conflict("Phần thưởng hôm nay đã được nhận.");
            }

            // Ghi nhận người dùng đã nhận thưởng
            if (thuongNgay == null)
            {
                thuongNgay = new ThuongNgay
                {
                    UserID = userId,
                    NgayNhan = today,
                    PhanThuong = "100 điểm thưởng",
                    DiemThuong = 100,
                    TrangThaiNhan = true
                };
                _context.ThuongNgays.Add(thuongNgay);
            }
            else
            {
                thuongNgay.TrangThaiNhan = true;
                _context.ThuongNgays.Update(thuongNgay);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Đã nhận thưởng ngày thành công." });
        }
    }
}