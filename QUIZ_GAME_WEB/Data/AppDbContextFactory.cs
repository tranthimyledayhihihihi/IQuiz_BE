using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QUIZ_GAME_WEB.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<QuizGameContext>
    {
        public QuizGameContext CreateDbContext(string[] args)
        {
            // Lấy Configuration từ appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Lấy chuỗi kết nối
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Tạo DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<QuizGameContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new QuizGameContext(optionsBuilder.Options);
        }
    }
}