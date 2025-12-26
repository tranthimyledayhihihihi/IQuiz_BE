using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace QUIZ_GAME_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class WrongQuestionController : ControllerBase
    {
        private readonly string _connectionString =
            "Data Source=LAPTOP-JGDHOLEA;Initial Catalog=QUIZ_GAME_WEB_DB;Persist Security Info=True;User ID=sa;Password=123456789;Encrypt=True;Trust Server Certificate=True";

        // =====================================================
        // LẤY USERID TỪ JWT
        // =====================================================
        private int? GetUserIdFromToken()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idStr, out var id))
                return id;
            return null;
        }

        // =====================================================
        // GET: api/WrongQuestion/me
        // Lấy lịch sử câu hỏi làm sai (danh sách)
        // =====================================================
        [HttpGet("me")]
        public IActionResult GetMyWrongQuestions()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { success = false, message = "Không tìm thấy UserID" });

                var wrongQuestions = new List<object>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                        SELECT 
                            cs.CauSaiID,
                            cs.UserID,
                            cs.CauHoiID,
                            cs.QuizAttemptID,
                            cs.NgaySai,

                            ch.NoiDung AS CauHoi,
                            ch.DapAnA,
                            ch.DapAnB,
                            ch.DapAnC,
                            ch.DapAnD,
                            ch.DapAnDung AS DapAnChinhXac,

                            cd.ChuDeID,
                            cd.TenChuDe
                        FROM CauSai cs
                        INNER JOIN CauHoi ch ON cs.CauHoiID = ch.CauHoiID
                        INNER JOIN ChuDe cd ON ch.ChuDeID = cd.ChuDeID
                        WHERE cs.UserID = @userId
                        ORDER BY cs.NgaySai DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                wrongQuestions.Add(new
                                {
                                    CauSaiID = reader.GetInt32(0),
                                    UserID = reader.GetInt32(1),
                                    CauHoiID = reader.GetInt32(2),
                                    QuizAttemptID = reader.GetInt32(3),
                                    NgaySai = reader.GetDateTime(4),

                                    CauHoi = reader.GetString(5),
                                    DapAnA = reader.GetString(6),
                                    DapAnB = reader.GetString(7),
                                    DapAnC = reader.GetString(8),
                                    DapAnD = reader.GetString(9),
                                    DapAnChinhXac = reader.GetString(10),

                                    ChuDeID = reader.GetInt32(11),
                                    TenChuDe = reader.GetString(12)
                                });
                            }
                        }
                    }
                }

                return Ok(new { success = true, data = wrongQuestions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // GET: api/WrongQuestion/quiz
        // Trả về danh sách câu hỏi sai để chơi/ôn tập
        // (ĐÃ FIX LỖI ORDER BY + DISTINCT)
        // =====================================================
        [HttpGet("quiz")]
        public IActionResult GetWrongQuestionsAsQuiz()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { success = false, message = "Không tìm thấy UserID" });

                var questions = new List<object>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // ⚠️ KHÔNG DÙNG DISTINCT → tránh lỗi ORDER BY
                    var query = @"
                        SELECT TOP (50)
                            ch.CauHoiID,
                            ch.NoiDung,
                            ch.DapAnA,
                            ch.DapAnB,
                            ch.DapAnC,
                            ch.DapAnD,
                            ch.DapAnDung,
                            ch.ChuDeID
                        FROM CauSai cs
                        INNER JOIN CauHoi ch ON cs.CauHoiID = ch.CauHoiID
                        WHERE cs.UserID = @userId
                        ORDER BY cs.NgaySai DESC";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId.Value);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                questions.Add(new
                                {
                                    CauHoiID = reader.GetInt32(0),
                                    NoiDung = reader.GetString(1),
                                    DapAnA = reader.GetString(2),
                                    DapAnB = reader.GetString(3),
                                    DapAnC = reader.GetString(4),
                                    DapAnD = reader.GetString(5),
                                    DapAnDung = reader.GetString(6),
                                    ChuDeID = reader.GetInt32(7)
                                });
                            }
                        }
                    }
                }

                return Ok(new
                {
                    success = true,
                    questions = questions
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // =====================================================
        // POST: api/WrongQuestion/add
        // Lưu câu hỏi làm sai
        // =====================================================
        [HttpPost("add")]
        public IActionResult AddWrongQuestion([FromBody] AddWrongQuestionRequest request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                    return Unauthorized(new { success = false, message = "Không tìm thấy UserID" });

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                        INSERT INTO CauSai (UserID, CauHoiID, QuizAttemptID, NgaySai)
                        VALUES (@userId, @cauHoiId, @quizAttemptId, GETDATE())";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId.Value);
                        cmd.Parameters.AddWithValue("@cauHoiId", request.CauHoiID);
                        cmd.Parameters.AddWithValue("@quizAttemptId", request.QuizAttemptID);

                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = "Đã lưu câu hỏi làm sai"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // =====================================================
    // DTO
    // =====================================================
    public class AddWrongQuestionRequest
    {
        public int CauHoiID { get; set; }
        public int QuizAttemptID { get; set; }
    }
}
