// Models/Core Entities/Admin.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities // Namespace đã được sửa
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; } // Khóa chính của bảng Admin

        [Required]
        [ForeignKey("User")] // Khóa ngoại 1:1 tới NguoiDung
        public int UserID { get; set; }

        [Required]
        [ForeignKey("VaiTro")] // Khóa ngoại N:1 tới Role (VaiTro)
        public int VaiTroID { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true; // 1: Active, 0: Suspended

        // Thuộc tính điều hướng (Navigation Properties)

        // Mối quan hệ 1:1 với NguoiDung
        public virtual NguoiDung User { get; set; }

        // Mối quan hệ N:1 với VaiTro
        public virtual Role VaiTro { get; set; }
    }
}