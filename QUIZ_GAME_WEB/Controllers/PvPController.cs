using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace QUIZ_GAME_WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PvPController : ControllerBase
    {
        private readonly string _connectionString = "Server=LAPTOP-RQMKJHVS\\SQLEXPRESS;Database=QUIZ;Trusted_Connection=true;TrustServerCertificate=true;";

        [HttpGet("battles/user/{userId}")]
        public IActionResult GetUserBattles(int userId)
        {
            try
            {
                var battles = new List<object>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT td.TranDauID, td.NguoiChoi1ID, td.NguoiChoi2ID, td.NgayBatDau, td.NgayKetThuc,
                               td.TrangThai, td.NguoiThangID, td.DiemNguoiChoi1, td.DiemNguoiChoi2,
                               nd1.TenNguoiDung as TenNguoiChoi1, nd2.TenNguoiDung as TenNguoiChoi2
                        FROM TranDauTrucTiep td
                        LEFT JOIN NguoiDung nd1 ON td.NguoiChoi1ID = nd1.UserID
                        LEFT JOIN NguoiDung nd2 ON td.NguoiChoi2ID = nd2.UserID
                        WHERE td.NguoiChoi1ID = @userId OR td.NguoiChoi2ID = @userId
                        ORDER BY td.NgayBatDau DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                battles.Add(new
                                {
                                    TranDauID = reader.GetInt32("TranDauID"),
                                    NguoiChoi1ID = reader.GetInt32("NguoiChoi1ID"),
                                    NguoiChoi2ID = reader.IsDBNull("NguoiChoi2ID") ? (int?)null : reader.GetInt32("NguoiChoi2ID"),
                                    NgayBatDau = reader.GetDateTime("NgayBatDau"),
                                    NgayKetThuc = reader.IsDBNull("NgayKetThuc") ? (DateTime?)null : reader.GetDateTime("NgayKetThuc"),
                                    TrangThai = reader.GetString("TrangThai"),
                                    NguoiThangID = reader.IsDBNull("NguoiThangID") ? (int?)null : reader.GetInt32("NguoiThangID"),
                                    DiemNguoiChoi1 = reader.IsDBNull("DiemNguoiChoi1") ? (int?)null : reader.GetInt32("DiemNguoiChoi1"),
                                    DiemNguoiChoi2 = reader.IsDBNull("DiemNguoiChoi2") ? (int?)null : reader.GetInt32("DiemNguoiChoi2"),
                                    TenNguoiChoi1 = reader.IsDBNull("TenNguoiChoi1") ? "" : reader.GetString("TenNguoiChoi1"),
                                    TenNguoiChoi2 = reader.IsDBNull("TenNguoiChoi2") ? "" : reader.GetString("TenNguoiChoi2")
                                });
                            }
                        }
                    }
                }

                return Ok(new { success = true, data = battles });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("battle/{battleId}/answers")]
        public IActionResult GetBattleAnswers(int battleId)
        {
            try
            {
                var answers = new List<object>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        SELECT tl.TraLoiID, tl.TranDauID, tl.UserID, tl.CauHoiID, tl.DapAnChon, 
                               tl.ThoiGianTraLoi, tl.LaDapAnDung, nd.TenNguoiDung,
                               ch.NoiDung as CauHoi, ch.DapAnA, ch.DapAnB, ch.DapAnC, ch.DapAnD, ch.DapAnDung
                        FROM TraLoiTrucTiep tl
                        INNER JOIN NguoiDung nd ON tl.UserID = nd.UserID
                        INNER JOIN CauHoi ch ON tl.CauHoiID = ch.CauHoiID
                        WHERE tl.TranDauID = @battleId
                        ORDER BY tl.CauHoiID, tl.ThoiGianTraLoi";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@battleId", battleId);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                answers.Add(new
                                {
                                    TraLoiID = reader.GetInt32("TraLoiID"),
                                    TranDauID = reader.GetInt32("TranDauID"),
                                    UserID = reader.GetInt32("UserID"),
                                    CauHoiID = reader.GetInt32("CauHoiID"),
                                    DapAnChon = reader.GetString("DapAnChon"),
                                    ThoiGianTraLoi = reader.GetInt32("ThoiGianTraLoi"),
                                    LaDapAnDung = reader.GetBoolean("LaDapAnDung"),
                                    TenNguoiDung = reader.GetString("TenNguoiDung"),
                                    CauHoi = reader.GetString("CauHoi"),
                                    DapAnA = reader.GetString("DapAnA"),
                                    DapAnB = reader.GetString("DapAnB"),
                                    DapAnC = reader.GetString("DapAnC"),
                                    DapAnD = reader.GetString("DapAnD"),
                                    DapAnDung = reader.GetString("DapAnDung")
                                });
                            }
                        }
                    }
                }

                return Ok(new { success = true, data = answers });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("battle/create")]
        public IActionResult CreateBattle([FromBody] CreateBattleRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        INSERT INTO TranDauTrucTiep (NguoiChoi1ID, NgayBatDau, TrangThai)
                        VALUES (@nguoiChoi1Id, @ngayBatDau, @trangThai);
                        SELECT SCOPE_IDENTITY();";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nguoiChoi1Id", request.NguoiChoi1ID);
                        command.Parameters.AddWithValue("@ngayBatDau", DateTime.Now);
                        command.Parameters.AddWithValue("@trangThai", "Waiting");

                        var newBattleId = command.ExecuteScalar();
                        return Ok(new { success = true, battleId = newBattleId, message = "Battle created successfully" });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("battle/{battleId}/join")]
        public IActionResult JoinBattle(int battleId, [FromBody] JoinBattleRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var query = @"
                        UPDATE TranDauTrucTiep 
                        SET NguoiChoi2ID = @nguoiChoi2Id, TrangThai = 'Playing'
                        WHERE TranDauID = @battleId AND TrangThai = 'Waiting'";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@battleId", battleId);
                        command.Parameters.AddWithValue("@nguoiChoi2Id", request.NguoiChoi2ID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok(new { success = true, message = "Joined battle successfully" });
                        }
                        else
                        {
                            return BadRequest(new { success = false, message = "Battle not available" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CreateBattleRequest
    {
        public int NguoiChoi1ID { get; set; }
    }

    public class JoinBattleRequest
    {
        public int NguoiChoi2ID { get; set; }
    }
}