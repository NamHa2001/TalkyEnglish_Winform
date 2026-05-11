using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class CourseDTO
    {
        // Sử dụng [Browsable(false)] cho những cột bạn muốn có dữ liệu nhưng KHÔNG hiện lên bảng
        [DisplayName(" ID")]
        public int CourseID { get; set; }

        [DisplayName("Mã Khóa Học")]
        public string? CourseCode { get; set; }

        [DisplayName("Tên Khóa Học")]
        public string? CourseName { get; set; }

        [DisplayName("Học Phí")]
        public decimal? Price { get; set; }

        [DisplayName("Trình Độ")]
        public string? Level { get; set; }

        [DisplayName("Thời Lượng")]
        public string? Duration { get; set; }

        [DisplayName("Trạng Thái")]
        public string? Status { get; set; }

        [DisplayName("Ngày Tạo")]
        public DateTime? CreatedAt { get; set; }

        [DisplayName("Giảng Viên")]
        [NotMapped]
        public string? InstructorName { get; set; }

        // Những cột ID phụ hoặc mô tả dài thường sẽ ẩn đi để bảng sạch hơn
        [Browsable(false)]
        public string? Description { get; set; }

        [Browsable(false)]
        public int? InstructorID { get; set; }

        [Browsable(false)]
        public int? CategoryID { get; set; }
    }
}
