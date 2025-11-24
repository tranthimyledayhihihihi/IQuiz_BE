// Models/Quiz Models/Quiz.cs
using QUIZ_GAME_WEB.Models.CoreEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUIZ_GAME_WEB.Models.QuizModels // Namespace đã được sửa
{
    public class Quiz
    {
        [Key]
        public int QuizID { get; set; }

        [Required]
        [StringLength(150)]
        public string TenQuiz { get; set; }

        [StringLength(500)]
        public string? MoTa { get; set; }

        public int SoLuongCauHoi { get; set; } // Tổng số câu hỏi trong Quiz này

        public int ThoiGianGioiHanPhut { get; set; } // Thời gian giới hạn

        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Admin")]
        public int AdminID { get; set; } // ID Admin/Moderator tạo Quiz

        public bool IsActive { get; set; } = true; // Cho phép hiển thị

        // Thuộc tính điều hướng
        public virtual Admin Admin { get; set; }

        // Mối quan hệ N:M (Nếu Quiz có nhiều câu hỏi và câu hỏi thuộc nhiều Quiz)
        // public virtual ICollection<QuizCauHoi> QuizCauHois { get; set; }
    }
}