using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using QUIZ_GAME_WEB.Models; // Đảm bảo namespace này đúng

namespace QUIZ_GAME_WEB.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new QuizGameContext(
                serviceProvider.GetRequiredService<DbContextOptions<QuizGameContext>>()))
            {
                // Kiểm tra xem dữ liệu đã tồn tại chưa
                if (context.ChuDes.Any())
                {
                    return; // DB đã được seed
                }

                // Thêm dữ liệu Độ Khó
                context.DoKhos.AddRange(
                    new DoKho { TenDoKho = "Dễ", DiemThuong = 10 },
                    new DoKho { TenDoKho = "Trung bình", DiemThuong = 20 },
                    new DoKho { TenDoKho = "Khó", DiemThuong = 30 }
                );

                // Thêm dữ liệu Chủ Đề
                context.ChuDes.AddRange(
                    new ChuDe { TenChuDe = "Lịch sử", MoTa = "Câu hỏi về lịch sử Việt Nam và thế giới", TrangThai = true },
                    new ChuDe { TenChuDe = "Địa lý", MoTa = "Câu hỏi về địa lý, bản đồ", TrangThai = true },
                    new ChuDe { TenChuDe = "Khoa học", MoTa = "Câu hỏi về khoa học tự nhiên", TrangThai = true }
                );

                // (Bạn có thể thêm code để tạo tài khoản Admin ở đây, nhớ hash mật khẩu)

                context.SaveChanges();
            }
        }
    }
}