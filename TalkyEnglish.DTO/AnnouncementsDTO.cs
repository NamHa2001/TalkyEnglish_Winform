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
    }
}