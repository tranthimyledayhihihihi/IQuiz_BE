using System.ComponentModel.DataAnnotations;

namespace WEBB.Models.Admin
{
    // Dùng cho Form TẠO/SỬA Câu hỏi
    public class CauHoiCrudModel
    {
        public int CauHoiID { get; set; }

        [Required]
        [Display(Name = "Chủ đề")]
        public int ChuDeID { get; set; }

        [Required]
        [Display(Name = "Độ khó")]
        public int DoKhoID { get; set; }

        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        [Display(Name = "Nội dung câu hỏi")]
        public string NoiDung { get; set; }

        [Display(Name = "Đáp án A")]
        public string DapAnA { get; set; }
        [Display(Name = "Đáp án B")]
        public string DapAnB { get; set; }
        [Display(Name = "Đáp án C")]
        public string DapAnC { get; set; }
        [Display(Name = "Đáp án D")]
        public string DapAnD { get; set; }

        [Required(ErrorMessage = "Phải có đáp án đúng")]
        [Display(Name = "Đáp án đúng (A, B, C, hoặc D)")]
        public string DapAnDung { get; set; }
    }
}