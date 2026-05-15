using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class TuitionDTO
    {
        // Khóa chính để ngầm định thao tác cập nhật (Nhớ map đúng tên này vào DataPropertyName của cột Ẩn)
        public int EnrollmentID { get; set; }

        // Các thông tin hiển thị lên Grid và TextBox
        public string StudentCode { get; set; }
        public string FullName { get; set; }
        public string CourseName { get; set; }
        public decimal Price { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public string PaymentStatus { get; set; }
    }
}