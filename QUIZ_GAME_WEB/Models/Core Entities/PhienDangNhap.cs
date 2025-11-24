using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities
{
    public class PhienDangNhap
    {
        [Key]
        public int SessionID { get; set; }
        [Required]
        public int UserID { get; set; }
        [MaxLength(500)]
        public string Token { get; set; } // Có thể là JWT Token
        public DateTime ThoiGianDangNhap { get; set; } = DateTime.Now;
        public DateTime? ThoiGianHetHan { get; set; }
        public bool TrangThai { get; set; } = true;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}