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
        public DbSet<EnrolmentDTO> Enrolments { get; set; }
        public DbSet<AnnouncementsDTO> Announcements { get; set; }
        public DbSet<AttendanceDTO> Attendances { get; set; }
        public DbSet<GradesDTO> Grades { get; set; }
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

                // SỬA LẠI ĐOẠN NÀY: Thêm .ValueGeneratedOnAddOrUpdate()
                entity.Property(e => e.StudentCode)
                      .HasComputedColumnSql("('HV' + RIGHT('000' + CAST([UserID] AS [varchar](3)), 3))")
                      .ValueGeneratedOnAddOrUpdate(); // Ép EF luôn phải đọc giá trị này sau khi truy vấn

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

            // 3. Cấu hình cho bảng Enrolments (Đăng ký khóa học)
            modelBuilder.Entity<EnrolmentDTO>(entity =>
            {
                entity.ToTable("Enrolments");
                entity.HasKey(e => e.EnrolmentID);

                // Cấu hình Ngày đăng ký mặc định
                entity.Property(e => e.EnrolDate)
                      .HasDefaultValueSql("GETDATE()");

                // Thiết lập mối quan hệ với bảng Users (Học viên)
                entity.HasOne<UserDTO>()
                      .WithMany()
                      .HasForeignKey(e => e.StudentID)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa user thì xóa luôn đăng ký

                // Thiết lập mối quan hệ với bảng Courses (Khóa học)
                entity.HasOne<CourseDTO>()
                      .WithMany()
                      .HasForeignKey(e => e.CourseID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 4. Cấu hình cho bảng Announcements (Thông báo)
            modelBuilder.Entity<AnnouncementsDTO>(entity =>
            {
                entity.ToTable("Announcements");
                entity.HasKey(a => a.AnnounceID);
                entity.Property(a => a.PublishDate).HasDefaultValueSql("GETDATE()");
            });

            // 5. Cấu hình cho bảng Attendance (Điểm danh)
            modelBuilder.Entity<AttendanceDTO>(entity =>
            {
                entity.ToTable("Attendance");
                entity.HasKey(at => at.AttendanceID);
                entity.Property(at => at.AttendanceDate).HasDefaultValueSql("GETDATE()");
                entity.Property(at => at.IsPresent).HasDefaultValue(true);

                // Quan hệ: Một lượt đăng ký có nhiều ngày điểm danh
                entity.HasOne<EnrolmentDTO>()
                      .WithMany()
                      .HasForeignKey(at => at.EnrolmentID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            // 6. Cấu hình cho bảng Grades (Điểm số)
            modelBuilder.Entity<GradesDTO>(entity =>
            {
                entity.ToTable("Grades");
                entity.HasKey(g => g.GradeID);

                // Mặc định điểm là 0.0 (kiểu số thực) và chỉ định rõ kiểu dữ liệu trong SQL
                entity.Property(g => g.MidTerm)
                      .HasColumnType("float")
                      .HasDefaultValue(0.0);

                entity.Property(g => g.FinalTerm)
                              .HasColumnType("float")
                              .HasDefaultValue(0.0);

                // Quan hệ: Một lượt đăng ký sẽ có một bảng điểm
                entity.HasOne<EnrolmentDTO>()
                      .WithMany()
                      .HasForeignKey(g => g.EnrolmentID)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}