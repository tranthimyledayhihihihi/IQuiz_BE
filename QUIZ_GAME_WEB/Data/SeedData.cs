// ------------------------------------------------------------------
// FILE: Data/SeedData.cs (HOÀN CHỈNH VÀ ĐÃ BỔ SUNG Initialize)
// ------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QUIZ_GAME_WEB.Models;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;

namespace QUIZ_GAME_WEB.Data
{
    public static class SeedData
    {
        // 📢 PHƯƠNG THỨC Initialize (ĐƯỢC GỌI TỪ Program.cs)
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<QuizGameContext>();

                // Áp dụng tất cả các Migration và tạo/cập nhật Database. 
                // Điều này cũng tự động chèn dữ liệu HasData.
                context.Database.Migrate();

                context.SaveChanges();
            }
        }

        // 📢 PHƯƠNG THỨC EXTENSION: Seed (ĐƯỢC GỌI TRONG DbContext.OnModelCreating)
        public static void Seed(this ModelBuilder modelBuilder)
        {
            // Sử dụng ngày cố định để đảm bảo tính nhất quán của seed data
            var seedTime = new DateTime(2023, 11, 21, 10, 0, 0);
            var yesterday = seedTime.AddDays(-1).Date;

            // =============================================
            // 1️⃣ HỆ THỐNG PHÂN QUYỀN
            // =============================================

            modelBuilder.Entity<VaiTro>().HasData(
                new VaiTro { VaiTroID = 1, TenVaiTro = "SuperAdmin", MoTa = "Quản trị viên cấp cao, toàn quyền hệ thống." },
                new VaiTro { VaiTroID = 2, TenVaiTro = "Moderator", MoTa = "Kiểm duyệt viên, quản lý câu hỏi và người dùng." },
                new VaiTro { VaiTroID = 3, TenVaiTro = "Player", MoTa = "Người dùng/Người chơi thông thường." }
            );

            modelBuilder.Entity<Quyen>().HasData(
                new Quyen { QuyenID = 1, TenQuyen = "ql_nguoi_dung", MoTa = "Quản lý (Khóa/Mở khóa) tài khoản người dùng." },
                new Quyen { QuyenID = 2, TenQuyen = "ql_cau_hoi", MoTa = "Thêm, sửa, xóa, duyệt câu hỏi." },
                new Quyen { QuyenID = 3, TenQuyen = "ql_baocao", MoTa = "Truy cập và tạo báo cáo hệ thống." },
                new Quyen { QuyenID = 4, TenQuyen = "ql_vai_tro", MoTa = "Quản lý vai trò và quyền hạn (Chỉ SuperAdmin)." }
            );

            modelBuilder.Entity<VaiTroQuyen>().HasData(
                new VaiTroQuyen { VaiTroID = 1, QuyenID = 1 }, new VaiTroQuyen { VaiTroID = 1, QuyenID = 2 }, new VaiTroQuyen { VaiTroID = 1, QuyenID = 3 }, new VaiTroQuyen { VaiTroID = 1, QuyenID = 4 },
                new VaiTroQuyen { VaiTroID = 2, QuyenID = 1 }, new VaiTroQuyen { VaiTroID = 2, QuyenID = 2 }, new VaiTroQuyen { VaiTroID = 2, QuyenID = 3 }
            );

            // =============================================
            // 2️⃣ TÀI KHOẢN & LIÊN QUAN
            // =============================================

            // NguoiDung
            modelBuilder.Entity<NguoiDung>().HasData(
                new NguoiDung { UserID = 1, TenDangNhap = "admin_sa", MatKhau = "hashed_sa_password", Email = "superadmin@quiz.com", HoTen = "Nguyễn Super Admin", NgayDangKy = seedTime.AddDays(-10), TrangThai = true, LanDangNhapCuoi = seedTime, AnhDaiDien = null },
                new NguoiDung { UserID = 2, TenDangNhap = "player01", MatKhau = "hashed_p1_password", Email = "player01@quiz.com", HoTen = "Trần Văn A", NgayDangKy = seedTime.AddDays(-8), TrangThai = true, LanDangNhapCuoi = seedTime.AddHours(-1), AnhDaiDien = null },
                new NguoiDung { UserID = 3, TenDangNhap = "player02", MatKhau = "hashed_p2_password", Email = "player02@quiz.com", HoTen = "Lê Thị B", NgayDangKy = seedTime.AddDays(-6), TrangThai = true, LanDangNhapCuoi = seedTime.AddMinutes(-30), AnhDaiDien = null }
            );

            // Admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdminID = 1, UserID = 1, VaiTroID = 1, TrangThai = true, NgayTao = seedTime.AddDays(-10) }
            );

            // CaiDatNguoiDung
            modelBuilder.Entity<CaiDatNguoiDung>().HasData(
                new CaiDatNguoiDung { SettingID = 1, UserID = 2, AmThanh = true, NhacNen = false, ThongBao = true, NgonNgu = "vi" },
                new CaiDatNguoiDung { SettingID = 2, UserID = 3, AmThanh = true, NhacNen = true, ThongBao = true, NgonNgu = "vi" }
            );

            // =============================================
            // 3️⃣ QUIZ & THAM SỐ
            // =============================================

            // ChuDe
            modelBuilder.Entity<ChuDe>().HasData(
                new ChuDe { ChuDeID = 1, TenChuDe = "Lịch Sử Việt Nam", MoTa = "Các sự kiện và nhân vật lịch sử quan trọng.", TrangThai = true },
                new ChuDe { ChuDeID = 2, TenChuDe = "Toán Học Phổ Thông", MoTa = "Các bài toán đại số và hình học cơ bản.", TrangThai = true },
                new ChuDe { ChuDeID = 3, TenChuDe = "Khoa Học Tự Nhiên", MoTa = "Kiến thức vật lý, hóa học, sinh học.", TrangThai = true }
            );

            // DoKho
            modelBuilder.Entity<DoKho>().HasData(
                new DoKho { DoKhoID = 1, TenDoKho = "Dễ", DiemThuong = 10 },
                new DoKho { DoKhoID = 2, TenDoKho = "Trung bình", DiemThuong = 25 },
                new DoKho { DoKhoID = 3, TenDoKho = "Khó", DiemThuong = 50 }
            );

            // TroGiup
            modelBuilder.Entity<TroGiup>().HasData(
                new TroGiup { TroGiupID = 1, TenTroGiup = "50/50", MoTa = "Loại bỏ hai đáp án sai." },
                new TroGiup { TroGiupID = 2, TenTroGiup = "Hỏi khán giả", MoTa = "Tham khảo ý kiến cộng đồng." }
            );

            // CauHoi
            modelBuilder.Entity<CauHoi>().HasData(
                new CauHoi { CauHoiID = 1, ChuDeID = 1, DoKhoID = 1, NoiDung = "Ai là người phất cờ khởi nghĩa đầu tiên chống Pháp?", DapAnA = "Phan Đình Phùng", DapAnB = "Trần Văn Thời", DapAnC = "Trương Định", DapAnD = "Nguyễn Trung Trực", DapAnDung = "C" },
                new CauHoi { CauHoiID = 2, ChuDeID = 1, DoKhoID = 2, NoiDung = "Chiến dịch Điện Biên Phủ diễn ra năm nào?", DapAnA = "1953", DapAnB = "1954", DapAnC = "1975", DapAnD = "1950", DapAnDung = "B" },
                new CauHoi { CauHoiID = 3, ChuDeID = 2, DoKhoID = 1, NoiDung = "Căn bậc hai của 9 là bao nhiêu?", DapAnA = "3", DapAnB = "9", DapAnC = "3 và -3", DapAnD = "Không có", DapAnDung = "C" },
                new CauHoi { CauHoiID = 4, ChuDeID = 3, DoKhoID = 2, NoiDung = "Chất nào sau đây không dẫn điện?", DapAnA = "Đồng", DapAnB = "Vàng", DapAnC = "Nhựa", DapAnD = "Bạc", DapAnDung = "C" }
            );

            // =============================================
            // 4️⃣ KẾT QUẢ & XẾP HẠNG
            // =============================================

            // KetQua
            modelBuilder.Entity<KetQua>().HasData(
                new KetQua { KetQuaID = 1, UserID = 2, Diem = 50, SoCauDung = 2, TongCauHoi = 2, TrangThai = "Hoàn thành", ThoiGian = seedTime.AddHours(-5) },
                new KetQua { KetQuaID = 2, UserID = 2, Diem = 75, SoCauDung = 3, TongCauHoi = 4, TrangThai = "Hoàn thành", ThoiGian = seedTime.AddHours(-2) },
                new KetQua { KetQuaID = 3, UserID = 3, Diem = 25, SoCauDung = 1, TongCauHoi = 2, TrangThai = "Hoàn thành", ThoiGian = seedTime.AddHours(-1) }
            );

            // BXH
            modelBuilder.Entity<BXH>().HasData(
                new BXH { BXHID = 1, UserID = 2, DiemTuan = 125, DiemThang = 125, HangTuan = 1, HangThang = 1 },
                new BXH { BXHID = 2, UserID = 3, DiemTuan = 25, DiemThang = 25, HangTuan = 2, HangThang = 2 }
            );

            // ChuoiNgay
            modelBuilder.Entity<ChuoiNgay>().HasData(
                new ChuoiNgay { ChuoiID = 1, UserID = 2, SoNgayLienTiep = 5, NgayCapNhatCuoi = yesterday },
                new ChuoiNgay { ChuoiID = 2, UserID = 3, SoNgayLienTiep = 2, NgayCapNhatCuoi = yesterday }
            );

            // =============================================
            // 5️⃣ CHẾ ĐỘ ĐẶC BIỆT & TÙY CHỈNH
            // =============================================

            // QuizTuyChinh
            modelBuilder.Entity<QuizTuyChinh>().HasData(
                new QuizTuyChinh { QuizTuyChinhID = 1, UserID = 2, TenQuiz = "Quiz Của Tôi", MoTa = "Các câu hỏi tôi thích nhất.", NgayTao = seedTime.AddDays(-1) }
            );

            // QuizNgay
            modelBuilder.Entity<QuizNgay>().HasData(
                new QuizNgay { QuizNgayID = 1, Ngay = yesterday, CauHoiID = 1 }
            );

            // CauSai
            modelBuilder.Entity<CauSai>().HasData(
                new CauSai { CauSaiID = 1, UserID = 3, CauHoiID = 2, NgaySai = yesterday }
            );

            // QuizChiaSe
            modelBuilder.Entity<QuizChiaSe>().HasData(
                new QuizChiaSe { QuizChiaSeID = 1, QuizTuyChinhID = 1, UserGuiID = 2, UserNhanID = 3, NgayChiaSe = seedTime }
            );

            // ThanhTuu (Không có dữ liệu INSERT trong SQL, chỉ tạo bản ghi mẫu)
            modelBuilder.Entity<ThanhTuu>().HasData(
                new ThanhTuu { ThanhTuuID = 1, TenThanhTuu = "Người Mới", MoTa = "Hoàn thành 10 Quiz.", DieuKien = "KetQua > 10" },
                new ThanhTuu { ThanhTuuID = 2, TenThanhTuu = "Chuyên Gia", MoTa = "Đạt 1000 điểm.", DieuKien = "TongDiem > 1000" }
            );

            // ThuongNgay (Không có dữ liệu INSERT trong SQL, chỉ tạo bản ghi mẫu)
            modelBuilder.Entity<ThuongNgay>().HasData(
                new ThuongNgay { ThuongID = 1, UserID = 2, NgayNhan = yesterday, PhanThuong = "50 điểm", DiemThuong = 50, TrangThaiNhan = true },
                new ThuongNgay { ThuongID = 2, UserID = 3, NgayNhan = yesterday.AddDays(-1), PhanThuong = "50 điểm", DiemThuong = 50, TrangThaiNhan = true }
            );
        }
    }
}