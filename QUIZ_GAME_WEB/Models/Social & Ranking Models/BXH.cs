using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models
{
    public class BXH
    {
        [Key]
        public int BXHID { get; set; }
        [Required]
        public int UserID { get; set; }
        public int DiemTuan { get; set; } = 0;
        public int DiemThang { get; set; } = 0;
        public int? HangTuan { get; set; }
        public int? HangThang { get; set; }

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}