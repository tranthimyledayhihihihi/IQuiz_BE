using Microsoft.EntityFrameworkCore;
using QUIZ_GAME_WEB.Models; // Đảm bảo namespace này đúng

namespace QUIZ_GAME_WEB.Data
{
    public class QuizGameContext : DbContext
    {
        public QuizGameContext(DbContextOptions<QuizGameContext> options) : base(options)
        {
        }

        // === PHẦN 1: KHAI BÁO TẤT CẢ DbSet (Sửa lỗi CS1061 trong SeedData) ===

        // 🔐 Account Models
        public DbSet<NguoiDung> NguoiDungs { get; set; } = null!;
        public DbSet<CaiDatNguoiDung> CaiDatNguoiDungs { get; set; } = null!;
        public DbSet<PhienDangNhap> PhienDangNhaps { get; set; } = null!;

        // ❓ Quiz Models
        public DbSet<ChuDe> ChuDes { get; set; } = null!;
        public DbSet<DoKho> DoKhos { get; set; } = null!;
        public DbSet<CauHoi> CauHois { get; set; } = null!;
        public DbSet<TroGiup> TroGiups { get; set; } = null!;

        // 🏆 Results Models
        public DbSet<KetQua> KetQuas { get; set; } = null!;
        public DbSet<ChuoiNgay> ChuoiNgays { get; set; } = null!;
        public DbSet<ThuongNgay> ThuongNgays { get; set; } = null!;
        public DbSet<ThanhTuu> ThanhTuus { get; set; } = null!;
        public DbSet<ThongKeNguoiDung> ThongKeNguoiDungs { get; set; } = null!;

        // 🌐 Social & Ranking Models
        public DbSet<BXH> BXHs { get; set; } = null!;
        public DbSet<NguoiDungOnline> NguoiDungOnlines { get; set; } = null!;

        // ⚡ Custom Models
        public DbSet<QuizNgay> QuizNgays { get; set; } = null!;
        public DbSet<CauSai> CauSais { get; set; } = null!;
        public DbSet<QuizTuyChinh> QuizTuyChinhs { get; set; } = null!;
        public DbSet<QuizChiaSe> QuizChiaSes { get; set; } = null!;


        // === PHẦN 2: CẤU HÌNH TÊN BẢNG (Sửa lỗi 500 "Invalid object name") ===
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Báo cho EFC biết Model 'NguoiDung' tương ứng với Bảng 'NguoiDung' (số ít)
            modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");

            // Làm tương tự cho TẤT CẢ các bảng khác
            modelBuilder.Entity<CaiDatNguoiDung>().ToTable("CaiDatNguoiDung");
            modelBuilder.Entity<PhienDangNhap>().ToTable("PhienDangNhap");
            modelBuilder.Entity<ChuDe>().ToTable("ChuDe");
            modelBuilder.Entity<DoKho>().ToTable("DoKho");
            modelBuilder.Entity<CauHoi>().ToTable("CauHoi");
            modelBuilder.Entity<TroGiup>().ToTable("TroGiup");
            modelBuilder.Entity<KetQua>().ToTable("KetQua");
            modelBuilder.Entity<ChuoiNgay>().ToTable("ChuoiNgay");
            modelBuilder.Entity<ThuongNgay>().ToTable("ThuongNgay");
            modelBuilder.Entity<ThanhTuu>().ToTable("ThanhTuu");
            modelBuilder.Entity<ThongKeNguoiDung>().ToTable("ThongKeNguoiDung");
            modelBuilder.Entity<BXH>().ToTable("BXH");
            modelBuilder.Entity<NguoiDungOnline>().ToTable("NguoiDungOnline");
            modelBuilder.Entity<QuizNgay>().ToTable("QuizNgay");
            modelBuilder.Entity<CauSai>().ToTable("CauSai");
            modelBuilder.Entity<QuizTuyChinh>().ToTable("QuizTuyChinh");
            modelBuilder.Entity<QuizChiaSe>().ToTable("QuizChiaSe");

            // Cấu hình các ràng buộc UNIQUE (nếu có)
            modelBuilder.Entity<NguoiDung>(entity =>
            {
                entity.HasIndex(e => e.TenDangNhap).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}