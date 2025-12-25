using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Data;
using QUIZ_GAME_WEB.Models.InputModels;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Controllers.User
{
    [Route("api/user/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProfileController : ControllerBase
    {
        private readonly QuizGameContext _context;
        private readonly IProfileService _profileService;

        public ProfileController(QuizGameContext context, IProfileService profileService)
        {
            _context = context;
            _profileService = profileService;
        }

        private int? GetUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var id)) return id;
            return null;
        }

        // ===============================================
        // 1. LẤY THÔNG TIN HỒ SƠ (GET: api/user/profile/me)
        // ===============================================
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token." });

                Console.WriteLine($"🔍 DEBUG: Getting profile for user {userId.Value}");

                // ✅ SỬA LỖI: THÊM .Include(u => u.VaiTro) để tải thông tin phân quyền
                var user = await _context.NguoiDungs
                    .Include(u => u.CaiDat)
                    .Include(u => u.VaiTro) // 🔑 Khắc phục lỗi VaiTro là null
                    .FirstOrDefaultAsync(u => u.UserID == userId.Value);

                if (user == null)
                    return NotFound(new { message = "Người dùng không tồn tại." });

                Console.WriteLine($"📊 DEBUG: Calculating stats for user {userId.Value}");

                // Lấy thống kê thật từ database
                var tongSoBaiQuiz = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value)
                    .CountAsync();

                Console.WriteLine($"📊 DEBUG: Found {tongSoBaiQuiz} quiz results");

                var diemTrungBinh = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value)
                    .AverageAsync(kq => (double?)kq.Diem) ?? 0.0;

                // Thống kê chi tiết hơn
                var tongSoCauDung = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value)
                    .SumAsync(kq => kq.SoCauDung);

                var tongSoCauHoi = await _context.KetQuas
                    .Where(kq => kq.UserID == userId.Value)
                    .SumAsync(kq => kq.TongCauHoi);

                Console.WriteLine($"📊 DEBUG: Stats - Quiz: {tongSoBaiQuiz}, Avg: {diemTrungBinh}, Correct: {tongSoCauDung}/{tongSoCauHoi}");

                // Ánh xạ sang Anonymous Type (bao gồm cả Vai trò và thống kê thật)
                var result = new
                {
                    user.UserID,
                    user.TenDangNhap,
                    user.Email,
                    user.HoTen,
                    user.AnhDaiDien,
                    user.NgayDangKy,
                    user.LanDangNhapCuoi,
                    VaiTro = user.VaiTro?.TenVaiTro, // ✅ Bây giờ VaiTro sẽ không null
                    CaiDat = user.CaiDat == null ? null : new
                    {
                        user.CaiDat.AmThanh,
                        user.CaiDat.NhacNen,
                        user.CaiDat.ThongBao,
                        user.CaiDat.NgonNgu
                    },
                    // ✅ THỐNG KÊ THẬT TỪ DATABASE
                    ThongKe = new
                    {
                        SoBaiQuizHoanThanh = tongSoBaiQuiz,
                        DiemTrungBinh = Math.Round(diemTrungBinh, 1),
                        TongSoCauDung = tongSoCauDung,
                        TongSoCauHoi = tongSoCauHoi,
                        TyLeDung = tongSoCauHoi > 0 ? Math.Round((double)tongSoCauDung / tongSoCauHoi * 100, 1) : 0.0
                    }
                };

                Console.WriteLine($"✅ DEBUG: Returning profile with stats: {tongSoBaiQuiz} quizzes, {diemTrungBinh:F1} avg");

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DEBUG: Error in GetMyProfile: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi truy vấn hồ sơ: " + ex.Message });
            }
        }

        // ===============================================
        // 2. CẬP NHẬT HỒ SƠ (PUT: api/user/profile/me)
        // ===============================================
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] ProfileUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token." });

                var success = await _profileService.UpdateProfileAsync(userId.Value, model);

                if (!success)
                    return BadRequest(new { message = "Cập nhật thất bại. Vui lòng kiểm tra email hoặc tên người dùng." });

                return Ok(new { message = "Cập nhật hồ sơ thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi cập nhật hồ sơ: " + ex.Message });
            }
        }

        // [Tái tạo Model lồng SettingUpdateModel]
        public class SettingUpdateModel
        {
            public bool AmThanh { get; set; } = true;
            public bool NhacNen { get; set; } = true;
            public bool ThongBao { get; set; } = true;
            [Required]
            public string NgonNgu { get; set; } = "vi";
        }

        // ===============================================
        // 3. CẬP NHẬT THỐNG KÊ SAU KHI HOÀN THÀNH QUIZ (POST: api/user/profile/update-stats)
        // ===============================================
        [HttpPost("update-stats")]
        public async Task<IActionResult> UpdateQuizStats([FromBody] QuizStatsUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token." });

                Console.WriteLine($"📊 DEBUG: Updating stats for user {userId.Value}");
                Console.WriteLine($"📊 DEBUG: Quiz result - {model.CorrectAnswers}/{model.TotalQuestions} = {model.Score}%");

                // 1. Lưu kết quả quiz vào bảng KetQua
                var ketQua = new KetQua
                {
                    UserID = userId.Value,
                    QuizAttemptID = 0, // Tạm thời set 0 vì không có QuizAttempt thật
                    Diem = (int)model.Score,
                    SoCauDung = model.CorrectAnswers,
                    TongCauHoi = model.TotalQuestions,
                    TrangThaiKetQua = "Hoàn thành",
                    ThoiGian = DateTime.Now
                };
                
                _context.KetQuas.Add(ketQua);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ DEBUG: Saved KetQua with ID {ketQua.KetQuaID}");

                // 2. Cập nhật hoặc tạo thống kê trong bảng ThongKeNguoiDung
                var today = DateTime.Today;
                var thongKe = await _context.ThongKeNguoiDungs
                    .FirstOrDefaultAsync(tk => tk.UserID == userId.Value && tk.Ngay == today);

                if (thongKe == null)
                {
                    // Tạo mới thống kê cho ngày hôm nay
                    thongKe = new ThongKeNguoiDung
                    {
                        UserID = userId.Value,
                        Ngay = today,
                        SoTran = 1,
                        SoCauDung = model.CorrectAnswers,
                        DiemTrungBinh = model.Score
                    };
                    _context.ThongKeNguoiDungs.Add(thongKe);
                    Console.WriteLine($"📊 DEBUG: Created new ThongKe for today");
                }
                else
                {
                    // Cập nhật thống kê hiện có
                    thongKe.SoTran++;
                    thongKe.SoCauDung += model.CorrectAnswers;
                    
                    // Tính điểm trung bình mới
                    var tongDiemCu = thongKe.DiemTrungBinh * (thongKe.SoTran - 1);
                    thongKe.DiemTrungBinh = (tongDiemCu + model.Score) / thongKe.SoTran;
                    
                    Console.WriteLine($"📊 DEBUG: Updated existing ThongKe - {thongKe.SoTran} games, {thongKe.DiemTrungBinh:F1} avg");
                }

                await _context.SaveChangesAsync();
                
                Console.WriteLine($"✅ DEBUG: Stats update completed successfully");
                
                return Ok(new { 
                    message = "Cập nhật thống kê thành công.",
                    userId = userId.Value,
                    correctAnswers = model.CorrectAnswers,
                    totalQuestions = model.TotalQuestions,
                    score = model.Score,
                    newStats = new {
                        soTranHomNay = thongKe.SoTran,
                        soCauDungHomNay = thongKe.SoCauDung,
                        diemTrungBinhHomNay = Math.Round(thongKe.DiemTrungBinh, 2)
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DEBUG: Error updating stats: {ex.Message}");
                return BadRequest(new { message = "Lỗi khi cập nhật thống kê: " + ex.Message });
            }
        }

        // Model cho việc cập nhật thống kê quiz
        public class QuizStatsUpdateModel
        {
            [Required]
            public int CorrectAnswers { get; set; }
            
            [Required]
            public int TotalQuestions { get; set; }
            
            [Required]
            public double Score { get; set; }
            
            public string Category { get; set; }
        }

        // ===============================================
        // 4. CẬP NHẬT CÀI ĐẶT (PUT: api/user/profile/settings)
        // ===============================================
        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] SettingUpdateModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized(new { message = "Không tìm thấy thông tin người dùng trong token." });

                var success = await _profileService.UpdateUserSettingAsync(
                    userId.Value,
                    model.AmThanh,
                    model.NhacNen,
                    model.ThongBao,
                    model.NgonNgu
                );

                if (!success)
                    return NotFound(new { message = "Người dùng không tồn tại hoặc cập nhật cài đặt thất bại." });

                return Ok(new { message = "Cập nhật cài đặt thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi khi cập nhật cài đặt: " + ex.Message });
            }
        }
    }
}