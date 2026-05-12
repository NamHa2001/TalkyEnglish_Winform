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
        public int EnrolmentID { get; set; }

        [Browsable(false)]
        public int StudentID { get; set; }

        [Browsable(false)]
        public int CourseID { get; set; }

        [DisplayName("Ngày Đăng Ký")]
        public DateTime? EnrolDate { get; set; }

        [DisplayName("Trạng Thái Thanh Toán")]
        public string? PaymentStatus { get; set; } // Ví dụ: Đã đóng, Còn nợ

        // --- Các trường bổ trợ để hiển thị lên bảng (Grid) ---
        [DisplayName("Tên Học Viên")]
        public string? StudentName { get; set; }

        [DisplayName("Tên Khóa Học")]
        public string? CourseName { get; set; }

        [DisplayName("Học Phí")]
        public decimal? Price { get; set; }

        // --- BỔ SUNG CÁC TRƯỜNG DÀNH CHO DASHBOARD ---

        [DisplayName("Tiến Độ")]
        public int ProgressValue { get; set; } // Chứa giá trị % để đổ vào ProgressBar

        [DisplayName("Giảng Viên")]
        public string? InstructorName { get; set; } // Hiển thị tên GV ở phần Tiến độ học tập
    }
}
    
