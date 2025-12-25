using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace QUIZ_GAME_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomQuizController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=LAPTOP-JGDHOLEA;Initial Catalog=QUIZ_GAME_WEB_DB;Persist Security Info=True;User ID=sa;Password=123456789;Encrypt=True;Trust Server Certificate=True";

        [HttpGet("user/{userId}")]
        public IActionResult GetUserCustomQuizzes(int userId)
        {
            try
            {
                var customQuizzes = new List<object>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT QuizTuyChinhID, UserID, TenQuiz, MoTa, NgayTao, SoLuongCauHoi, ThoiGianGioiHan
                        FROM QuizTuyChinh
                        WHERE UserID = @userId
                        ORDER BY NgayTao DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                customQuizzes.Add(new
                                {
                                    QuizTuyChinhID = reader.GetInt32("QuizTuyChinhID"),
                                    UserID = reader.GetInt32("UserID"),
                                    TenQuiz = reader.GetString("TenQuiz"),
                                    MoTa = reader.IsDBNull("MoTa") ? "" : reader.GetString("MoTa"),
                                    NgayTao = reader.GetDateTime("NgayTao"),
                                    SoLuongCauHoi = reader.GetInt32("SoLuongCauHoi"),
                                    ThoiGianGioiHan = reader.GetInt32("ThoiGianGioiHan")
                                });
                            }
                        }
                    }
                }

                return Ok(new { success = true, data = customQuizzes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllCustomQuizzes()
        {
            try
            {
                var customQuizzes = new List<object>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT qc.QuizTuyChinhID, qc.UserID, qc.TenQuiz, qc.MoTa, qc.NgayTao, 
                               qc.SoLuongCauHoi, qc.ThoiGianGioiHan, nd.TenNguoiDung
                        FROM QuizTuyChinh qc
                        INNER JOIN NguoiDung nd ON qc.UserID = nd.UserID
                        ORDER BY qc.NgayTao DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                customQuizzes.Add(new
                                {
                                    QuizTuyChinhID = reader.GetInt32("QuizTuyChinhID"),
                                    UserID = reader.GetInt32("UserID"),
                                    TenQuiz = reader.GetString("TenQuiz"),
                                    MoTa = reader.IsDBNull("MoTa") ? "" : reader.GetString("MoTa"),
                                    NgayTao = reader.GetDateTime("NgayTao"),
                                    SoLuongCauHoi = reader.GetInt32("SoLuongCauHoi"),
                                    ThoiGianGioiHan = reader.GetInt32("ThoiGianGioiHan"),
                                    TenNguoiDung = reader.GetString("TenNguoiDung")
                                });
                            }
                        }
                    }
                }

                return Ok(new { success = true, data = customQuizzes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("create")]
        public IActionResult CreateCustomQuiz([FromBody] CreateCustomQuizRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        INSERT INTO QuizTuyChinh (UserID, TenQuiz, MoTa, NgayTao, SoLuongCauHoi, ThoiGianGioiHan)
                        VALUES (@userId, @tenQuiz, @moTa, @ngayTao, @soLuongCauHoi, @thoiGianGioiHan);
                        SELECT SCOPE_IDENTITY();";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", request.UserID);
                        command.Parameters.AddWithValue("@tenQuiz", request.TenQuiz);
                        command.Parameters.AddWithValue("@moTa", request.MoTa ?? "");
                        command.Parameters.AddWithValue("@ngayTao", DateTime.Now);
                        command.Parameters.AddWithValue("@soLuongCauHoi", request.SoLuongCauHoi);
                        command.Parameters.AddWithValue("@thoiGianGioiHan", request.ThoiGianGioiHan);

                        var newId = command.ExecuteScalar();
                        return Ok(new { success = true, quizId = newId, message = "Custom quiz created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CreateCustomQuizRequest
    {
        public int UserID { get; set; }
        public string TenQuiz { get; set; }
        public string MoTa { get; set; }
        public int SoLuongCauHoi { get; set; }
        public int ThoiGianGioiHan { get; set; }
    }
}