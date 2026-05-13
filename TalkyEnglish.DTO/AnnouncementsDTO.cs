using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class AnnouncementsDTO
    {
      
        [DisplayName("ID Thông Báo")]
        public int AnnounceID { get; set; }

        [DisplayName("Tiêu Đề")]
        public string? Title { get; set; }

        [DisplayName("Nội Dung")]
        public string? Content { get; set; }

        [DisplayName("Ngày Đăng")]
        public DateTime? PublishDate { get; set; }

        [DisplayName("Danh Mục")]
        public string? Category { get; set; } // Ví dụ: Lịch nghỉ, Khai giảng, Ưu đãi

        [DisplayName("Đối Tượng Nhận")]
        public string? TargetType { get; set; } // Lưu: All, Teacher, Student, Individual

        [DisplayName("ID Người Nhận")]
        public int? ReceiverID { get; set; } // Nullable vì có khi gửi cho cả nhóm

        [DisplayName("Độ Ưu Tiên")]
        public string? PriorityLevel { get; set; } // Normal hoặc Urgent

        [DisplayName("ID Người Gửi")]
        public int SenderID { get; set; }
    }
}