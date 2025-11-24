// Models/Core Entities/VaiTroQuyen.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.CoreEntities // Namespace đã được sửa
{
    public class VaiTroQuyen
    {
        // Khóa Chính Phức Hợp (Composite Key)
        // EF Core sẽ tự động nhận diện đây là khóa chính khi không có [Key] riêng

        [Required]
        [ForeignKey("VaiTro")]
        public int VaiTroID { get; set; }

        [Required]
        [ForeignKey("Quyen")]
        public int QuyenID { get; set; }

        // Thuộc tính điều hướng (Navigation Properties)

        // Mối quan hệ với VaiTro (Role.cs)
        public virtual Role VaiTro { get; set; }

        // Mối quan hệ với Quyen (Permission.cs)
        public virtual Permission Quyen { get; set; }
    }
}