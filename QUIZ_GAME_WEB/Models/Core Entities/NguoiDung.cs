using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
using QUIZ_GAME_WEB.Models.QuizModels;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        public int UserID { get; set; }

        [Required, MaxLength(50)]
        public string TenDangNhap { get; set; }

        [Required, MaxLength(255)]
        public string MatKhau { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(100)]
        public string? HoTen { get; set; }

        [MaxLength(255)]
        public string? AnhDaiDien { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.Now;
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool TrangThai { get; set; } = true;

        // ===============================================
        // NAVIGATION PROPERTIES (Đã sửa lỗi và bổ sung)
        // ===============================================

        // 1. Mối quan hệ 1-1 (Admin, Cài đặt)
        // Fix cho QLNguoiDungController:
        public virtual Admin? Admin { get; set; }

        // Fix cho ProfileController:
        public virtual CaiDatNguoiDung? CaiDat { get; set; }

        // 2. Mối quan hệ 1-to-N (Các bảng có UserID là khóa ngoại)
        public virtual ICollection<KetQua> KetQuas { get; set; }
        public virtual ICollection<PhienDangNhap> PhienDangNhaps { get; set; }
        public virtual ICollection<ChuoiNgay> ChuoiNgays { get; set; }
        public virtual ICollection<ThongKeNguoiDung> ThongKeNguoiDungs { get; set; }
        public virtual ICollection<ThuongNgay> ThuongNgays { get; set; }
        public virtual ICollection<BXH> BXHs { get; set; }
        public virtual ICollection<NguoiDungOnline> NguoiDungOnlines { get; set; }
        public virtual ICollection<QuizTuyChinh> QuizTuyChinhs { get; set; }
        public virtual ICollection<CauSai> CauSais { get; set; }

        // 3. Mối quan hệ tự tham chiếu (QuizChiaSe: User gửi và User nhận)
        // Cần sử dụng [InverseProperty] để EF Core biết cách ánh xạ
        [InverseProperty(nameof(QuizChiaSe.UserGui))]
        public virtual ICollection<QuizChiaSe> QuizChiaSeGui { get; set; }

        [InverseProperty(nameof(QuizChiaSe.UserNhan))]
        public virtual ICollection<QuizChiaSe> QuizChiaSeNhan { get; set; }
    }
}