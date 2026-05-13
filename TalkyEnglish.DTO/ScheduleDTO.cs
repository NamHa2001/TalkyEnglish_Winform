using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TalkyEnglish.DTO
{
    public class ScheduleDTO
    {
        // Các thuộc tính khớp 100% với bảng Schedules trong Database
        public int ScheduleID { get; set; }
        public int CourseID { get; set; }
        public string DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; } // Dùng TimeSpan cho kiểu time trong SQL
        public TimeSpan EndTime { get; set; }
        public string RoomName { get; set; }

        // Các thuộc tính hỗ trợ hiển thị (không lưu xuống bảng Schedules)
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string InstructorName { get; set; } // Khớp với tên cột InstructorName trên Grid dgvSchedules
    }
}