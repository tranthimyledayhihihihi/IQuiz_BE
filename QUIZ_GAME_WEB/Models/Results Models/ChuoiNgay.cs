using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models
{
    public class ChuoiNgay
    {
        [Key]
        public int ChuoiID { get; set; }
        [Required]
        public int UserID { get; set; }
        public int SoNgayLienTiep { get; set; } = 0;
        public DateTime NgayCapNhatCuoi { get; set; } = DateTime.Now;

        [ForeignKey("UserID")]
        public virtual NguoiDung NguoiDung { get; set; }
    }
}