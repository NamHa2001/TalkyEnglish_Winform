using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class CourseDTO
    {
        public int CourseID { get; set; }

        // 1. Mã khóa học tự sinh (KH001, KH002...)
        public string? CourseCode { get; set; }

        public string? CourseName { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }

        // 2. Trình độ mới thêm (Cơ bản, Trung cấp, Nâng cao)
        public string? Level { get; set; }

        public int? InstructorID { get; set; }
        public int? CategoryID { get; set; }
        public string? Status { get; set; }

        // 3. Ngày tạo khóa học mới thêm
        public DateTime? CreatedAt { get; set; }

        // Thuộc tính bổ trợ để hiển thị tên Giảng viên trên giao diện
        // (Không cần có trong bảng Courses của SQL)
        [NotMapped]
        public string? InstructorName { get; set; }
    }
}