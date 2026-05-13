using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class EnrolmentDTO
    {
        [DisplayName("ID Đăng Ký")]
        public int EnrollmentID { get; set; }

        [Browsable(false)]
        public int StudentID { get; set; }

        [Browsable(false)]
        public int CourseID { get; set; }

        // SỬA TÊN: Khớp với cột EnrollmentDate trong SQL
        [DisplayName("Ngày Đăng Ký")]
        public DateTime? EnrollmentDate { get; set; }

        [DisplayName("Trạng Thái Thanh Toán")]
        public string? PaymentStatus { get; set; }

        // --- Các trường bổ trợ (Dùng Fluent API .Ignore trong DbContext để không báo lỗi) ---
        [DisplayName("Tên Học Viên")]
        public string? StudentName { get; set; }

        [DisplayName("Tên Khóa Học")]
        public string? CourseName { get; set; }

        [DisplayName("Học Phí")]
        public decimal? Price { get; set; }

        [DisplayName("Tiến Độ")]
        public int ProgressValue { get; set; }

        [DisplayName("Giảng Viên")]
        public string? InstructorName { get; set; }
    }
}
    
