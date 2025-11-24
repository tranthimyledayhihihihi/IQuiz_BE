using System;
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.ResultsModels
{
    public class KetQua
    {
        [Key]
        public int KetQuaID { get; set; }
        [Required]
        public int UserID { get; set; }
        public int Diem { get; set; }
        public int SoCauDung { get; set; }
        public int TongCauHoi { get; set; }

        // === SỬA LỖI Ở ĐÂY ===
        // Đã đổi tên từ "KetQua" thành "TrangThai" để tránh trùng tên class
        [MaxLength(10)]
        public string TrangThai { get; set; } // Ví dụ: "Thắng", "Thua"

        public DateTime ThoiGian { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}