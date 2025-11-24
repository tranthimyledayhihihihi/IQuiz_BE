using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models;
using QUIZ_GAME_WEB.Models.CoreEntities;
using QUIZ_GAME_WEB.Models.QuizModels;
using QUIZ_GAME_WEB.Models.ResultsModels;
using QUIZ_GAME_WEB.Models.SocialRankingModels;
namespace QUIZ_GAME_WEB.Data
{
    public class QuizGameContext : DbContext
    {
        public QuizGameContext(DbContextOptions<QuizGameContext> options) : base(options) { }

        // Định nghĩa các DbSet
        // Dựa trên các file Models đã cung cấp:
        public DbSet<Admin> Admins { get; set; }
        public DbSet<BXH> BXHs { get; set; }
        public DbSet<CaiDatNguoiDung> CaiDatNguoiDungs { get; set; }
        public DbSet<CauHoi> CauHois { get; set; }
        public DbSet<CauSai> CauSais { get; set; }
        public DbSet<ChuDe> ChuDes { get; set; }
        public DbSet<ChuoiNgay> ChuoiNgays { get; set; }
        public DbSet<DoKho> DoKhos { get; set; }
        public DbSet<KetQua> KetQuas { get; set; }
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<NguoiDungOnline> NguoiDungOnlines { get; set; }
        public DbSet<PhienDangNhap> PhienDangNhaps { get; set; }
        public DbSet<Quyen> Quyens { get; set; }
        public DbSet<QuizChiaSe> QuizChiaSes { get; set; }
        public DbSet<QuizNgay> QuizNgays { get; set; }
        public DbSet<QuizTuyChinh> QuizTuyChinhs { get; set; }
        public DbSet<ThanhTuu> ThanhTuus { get; set; }
        public DbSet<ThongKeNguoiDung> ThongKeNguoiDungs { get; set; }
        public DbSet<ThuongNgay> ThuongNgays { get; set; }
        public DbSet<TroGiup> TroGiups { get; set; }
        public DbSet<VaiTro> VaiTros { get; set; }
        public DbSet<VaiTroQuyen> VaiTroQuyens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình khóa chính kép cho VaiTro_Quyen
            modelBuilder.Entity<VaiTroQuyen>()
                .HasKey(vq => new { vq.VaiTroID, vq.QuyenID });

            // Cấu hình Unique Index cho NguoiDung
            modelBuilder.Entity<NguoiDung>()
                .HasIndex(n => n.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<NguoiDung>()
                .HasIndex(n => n.Email)
                .IsUnique();

            // Gọi phương thức SeedData để chèn dữ liệu mẫu
            modelBuilder.Seed();
        }
    }
}