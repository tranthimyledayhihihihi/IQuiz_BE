// Models/Core Entities/Role.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.CoreEntities // Namespace đã được sửa
{
    public class Role
    {
        [Key]
        public int VaiTroID { get; set; } // Khóa chính

        [Required]
        [MaxLength(50)]
        public string TenVaiTro { get; set; } // Ví dụ: SuperAdmin, Moderator, Player

        [MaxLength(255)]
        public string? MoTa { get; set; }

        // Thuộc tính điều hướng (Navigation Properties)

        // Mối quan hệ N:N (Vai trò có nhiều quyền) thông qua VaiTroQuyen
        public virtual ICollection<VaiTroQuyen> VaiTroQuyens { get; set; } = new HashSet<VaiTroQuyen>();

        // Mối quan hệ 1:N (Một vai trò có thể có nhiều Admin)
        public virtual ICollection<Admin> Admins { get; set; } = new HashSet<Admin>();
    }
}