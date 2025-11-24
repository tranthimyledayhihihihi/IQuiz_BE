// Models/Input Models/QuizCreateEditModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUIZ_GAME_WEB.Models.InputModels // Namespace đã được sửa
{
    public class QuizCreateEditModel
    {
        // Nullable: Nếu là chỉnh sửa (Edit) thì có QuizID, nếu là tạo mới (Create) thì không có.
        public int? QuizID { get; set; }

        [Required(ErrorMessage = "Tên Quiz là bắt buộc.")]
        [StringLength(100)]
        public string TenQuiz { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }

        // Bắt buộc phải có Chủ đề (nếu Quiz của bạn lưu trực tiếp tham số)
        [Required(ErrorMessage = "Phải chọn chủ đề.")]
        public int ChuDeID { get; set; }

        // Danh sách các ID câu hỏi được chọn
        [Required(ErrorMessage = "Phải chọn ít nhất một câu hỏi.")]
        public List<int> CauHoiIDs { get; set; } = new List<int>();

        // Thiết lập thời gian giới hạn cho Quiz (Ví dụ)
        [Required]
        [Range(1, 60, ErrorMessage = "Thời gian giới hạn phải từ 1 đến 60 phút.")]
        public int ThoiGianGioiHanPhut { get; set; } = 10;
    }
}