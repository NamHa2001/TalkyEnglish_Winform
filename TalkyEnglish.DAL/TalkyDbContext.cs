using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DAL
{
    public class TalkyDbContext : DbContext
    {
        // Khai báo các bảng dữ liệu dưới dạng DbSet
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<CourseDTO> Courses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Server=. (Dấu chấm đại diện cho Local)
                // User Id=sa; Password=Namha2001@
                // TrustServerCertificate=True để tránh lỗi SSL khi kết nối cục bộ
                string connectionString = @"Server=.;Database=DB_TalkyEnglish;User Id=sa;Password=Namha2001@;TrustServerCertificate=True;";

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Định nghĩa khóa chính cho các bảng (vì chúng ta dùng DTO làm Entity)
            modelBuilder.Entity<UserDTO>().HasKey(u => u.UserID);
            modelBuilder.Entity<CourseDTO>().HasKey(c => c.CourseID);

            // Ánh xạ tên bảng chính xác trong SQL (nếu tên Class khác tên bảng)
            modelBuilder.Entity<UserDTO>().ToTable("Users");
            modelBuilder.Entity<CourseDTO>().ToTable("Courses");
        }
    }
}