using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class UserDTO
    {
        [Key]
        public int UserID { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        [Browsable(false)]
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string Specialization { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;       // Trạng thái (Đang dạy, Nghỉ phép...)

        // --- THÊM 2 TRƯỜNG NÀY VÀO ---
        [Column("Birthday")]
        public DateTime? Birthday { get; set; } // Ngày sinh

        [Column("Gender")]
        public string? Gender { get; set; }     // Giới tính
                                                // ----------------------------

        public DateTime? CreatedAt { get; set; }
        public string? StudentCode { get; set; }

        public string? CourseName { get; set; }
        public string? Level { get; set; }
    }
}