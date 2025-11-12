using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models
{
    public class NguoiDung
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        [MaxLength(50)]
        public string TenDangNhap { get; set; }
        [Required]
        [MaxLength(255)]
        public string MatKhau { get; set; } // Sẽ lưu dạng hash
        [MaxLength(100)]
        // Sửa các dòng này
        public string? Email { get; set; }
        public string? HoTen { get; set; }
        public string? AnhDaiDien { get; set; }
        public DateTime NgayDangKy { get; set; } = DateTime.Now;
        public DateTime? LanDangNhapCuoi { get; set; }
        public bool TrangThai { get; set; } = true;

        // Navigation properties
        public virtual CaiDatNguoiDung CaiDat { get; set; }
        public virtual ICollection<KetQua> KetQuas { get; set; }
        public virtual ICollection<PhienDangNhap> PhienDangNhaps { get; set; }
    }
}