using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class GradesDTO
    {
        [DisplayName("ID Điểm")]
        public int GradeID { get; set; }

        [Browsable(false)]
        public int EnrolmentID { get; set; }

        // --- Các đầu điểm thành phần theo image_d53515.jpg ---

        [DisplayName("Bài tập / Thảo luận")]
        public double? AttendanceScore { get; set; } // Thay cho điểm chuyên cần

        [DisplayName("Điểm Giữa Kỳ")]
        public double? MidTerm { get; set; }

        [DisplayName("Điểm Cuối Kỳ")]
        public double? FinalTerm { get; set; }

        // --- Các cột kết quả tổng kết và xếp loại ---

        [DisplayName("Điểm Trung Bình")]
        public double? AverageScore { get; set; }

        [DisplayName("Điểm Chữ")]
        public string? GradeLetter { get; set; }

        [DisplayName("Xếp Loại")]
        public string? Ranking { get; set; }

        [DisplayName("Tỷ Lệ Chuyên Cần")]
        public double? AttendancePercentage { get; set; }

        [DisplayName("Số Buổi Học")]
        public string? TotalSessions { get; set; }

        // --- Phần nhận xét của giảng viên ---

        [DisplayName("Nhận Xét")]
        public string? Note { get; set; } // Giữ lại Note cũ để làm nhận xét chính

        [DisplayName("Ngày Nhận Xét")]
        public DateTime? CommentDate { get; set; }

        // --- Các thuộc tính bổ trợ hiển thị (Dùng cho bảng danh sách khóa học phía trên) ---

       
        public string? CourseName { get; set; }

        public string? TeacherName { get; set; }

    
        public string? Semester { get; set; } // Học kỳ (Ví dụ: HK1/2025)
    }
}