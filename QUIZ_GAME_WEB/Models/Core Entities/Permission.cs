// Models/Core Entities/Permission.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.CoreEntities // Namespace đã được sửa
{
    public class Permission
    {
        [Key]
        public int QuyenID { get; set; } // Khóa chính

        [Required]
        [MaxLength(100)]
        public string TenQuyen { get; set; } // Ví dụ: ql_cau_hoi, ql_nguoi_dung

        [MaxLength(255)]
        public string? MoTa { get; set; }

        // Thuộc tính điều hướng (Navigation Properties)

        // Mối quan hệ N-N với Role thông qua VaiTroQuyen
        public virtual ICollection<VaiTroQuyen> VaiTroQuyens { get; set; } = new HashSet<VaiTroQuyen>();
    }
}