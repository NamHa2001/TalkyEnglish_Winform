using System;
using Microsoft.EntityFrameworkCore;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DAL
{
    public class TalkyDbContext : DbContext
    {
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<CourseDTO> Courses { get; set; }
        public virtual DbSet<TeachingAssignmentDTO> TeachingAssignments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = @"Server=.;Database=DB_TalkyEnglish;User Id=sa;Password=Namha2001@;TrustServerCertificate=True;";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Cấu hình cho bảng Users
            modelBuilder.Entity<UserDTO>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.Gender).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

                // Cấu hình cột tính toán cho Mã học viên
                entity.Property(e => e.StudentCode)
                      .HasComputedColumnSql("('HV' + RIGHT('000' + CAST([UserID] AS [varchar](3)), 3))");

                // Cấu hình 2 cột mới thêm để lưu thông tin khóa học
                entity.Property(e => e.CourseName).HasMaxLength(255);
                entity.Property(e => e.Level).HasMaxLength(100);
            });


            // 2. Cấu hình cho bảng Courses (Nâng cấp để quản lý lâu dài)
            modelBuilder.Entity<CourseDTO>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(c => c.CourseID);

                // Cấu hình CourseCode là cột tính toán (Computed) từ SQL
                // Bạn không bao giờ phải nhập tay trường này trong code C#
                entity.Property(c => c.CourseCode)
                      .HasComputedColumnSql("('KH' + RIGHT('000' + CAST([CourseID] AS [varchar](3)), 3))");

                // Cấu hình Ngày tạo mặc định
                entity.Property(c => c.CreatedAt)
                      .HasDefaultValueSql("GETDATE()");

                // Đảm bảo kiểu dữ liệu tiền tệ chính xác
                entity.Property(c => c.Price)
                      .HasColumnType("decimal(18, 2)");

                // --- QUAN TRỌNG: THIẾT LẬP MỐI QUAN HỆ GIẢNG VIÊN ---
                // Khai báo rằng CourseDTO có InstructorID liên kết đến UserID của UserDTO
                // Điều này giúp sau này bạn dùng lệnh .Include(c => c.Instructor) cực kỳ dễ dàng
                entity.HasOne<UserDTO>()
                      .WithMany()
                      .HasForeignKey(c => c.InstructorID)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh xóa nhầm Giảng viên khi còn Khóa học
            });
        }
    }
}