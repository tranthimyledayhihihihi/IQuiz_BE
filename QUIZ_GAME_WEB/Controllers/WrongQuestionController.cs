using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace QUIZ_GAME_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WrongQuestionController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=LAPTOP-JGDHOLEA;Initial Catalog=QUIZ_GAME_WEB_DB;Persist Security Info=True;User ID=sa;Password=123456789;Encrypt=True;Trust Server Certificate=True";

        [HttpGet("user/{userId}")]
        public IActionResult GetWrongQuestions(int userId)
        {
            try
            {
                var wrongQuestions = new List<object>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT cs.CauSaiID, cs.UserID, cs.CauHoiID, cs.DapAnSai, cs.DapAnDung, cs.NgayTao,
                               ch.NoiDung as CauHoi, ch.DapAnA, ch.DapAnB, ch.DapAnC, ch.DapAnD, ch.DapAnDung as DapAnChinhXac,
                               cd.TenChuDe
                        FROM CauSai cs
                        INNER JOIN CauHoi ch ON cs.CauHoiID = ch.CauHoiID
                        INNER JOIN ChuDe cd ON ch.ChuDeID = cd.ChuDeID
                        WHERE cs.UserID = @userId
                        ORDER BY cs.NgayTao DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                wrongQuestions.Add(new
                                {
                                    CauSaiID = reader.GetInt32("CauSaiID"),
                                    UserID = reader.GetInt32("UserID"),
                                    CauHoiID = reader.GetInt32("CauHoiID"),
                                    DapAnSai = reader.GetString("DapAnSai"),
                                    DapAnDung = reader.GetString("DapAnDung"),
                                    NgayTao = reader.GetDateTime("NgayTao"),
                                    CauHoi = reader.GetString("CauHoi"),
                                    DapAnA = reader.GetString("DapAnA"),
                                    DapAnB = reader.GetString("DapAnB"),
                                    DapAnC = reader.GetString("DapAnC"),
                                    DapAnD = reader.GetString("DapAnD"),
                                    DapAnChinhXac = reader.GetString("DapAnChinhXac"),
                                    TenChuDe = reader.GetString("TenChuDe")
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

        [HttpPost("add")]
        public IActionResult AddWrongQuestion([FromBody] AddWrongQuestionRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        INSERT INTO CauSai (UserID, CauHoiID, DapAnSai, DapAnDung, NgayTao)
                        VALUES (@userId, @cauHoiId, @dapAnSai, @dapAnDung, @ngayTao)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", request.UserID);
                        command.Parameters.AddWithValue("@cauHoiId", request.CauHoiID);
                        command.Parameters.AddWithValue("@dapAnSai", request.DapAnSai);
                        command.Parameters.AddWithValue("@dapAnDung", request.DapAnDung);
                        command.Parameters.AddWithValue("@ngayTao", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok(new { success = true, message = "Wrong question added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class AddWrongQuestionRequest
    {
        public int UserID { get; set; }
        public int CauHoiID { get; set; }
        public string DapAnSai { get; set; }
        public string DapAnDung { get; set; }
    }
}