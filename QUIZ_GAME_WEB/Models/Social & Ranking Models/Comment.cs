// Models/Social & Ranking Models/Comment.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QUIZ_GAME_WEB.Models.CoreEntities;

namespace QUIZ_GAME_WEB.Models.Social_RankingModels // Namespace đã được sửa
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        [ForeignKey("NguoiDung")]
        public int UserID { get; set; }

        // ID của đối tượng được bình luận (QuizID, QuestionID, v.v.)
        [Required]
        public int RelatedEntityID { get; set; }

        [Required]
        [MaxLength(50)]
        public string EntityType { get; set; } // Ví dụ: "Quiz", "Question", "User"

        [Required]
        [MaxLength(500)]
        public string NoiDung { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;

        // Thuộc tính điều hướng
        public virtual NguoiDung User { get; set; }
    }
}