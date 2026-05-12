using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // SỬA: Thêm dấu ? để chấp nhận giá trị NULL từ SQL Server
        public string? Specialization { get; set; }

        // SỬA: Thêm dấu ? để chấp nhận giá trị NULL từ SQL Server
        public string? Degree { get; set; }

        // SỬA: Thêm dấu ? để chấp nhận giá trị NULL từ SQL Server
        public string? Status { get; set; }

        [Column("Birthday")]
        public DateTime? Birthday { get; set; }

        [Column("Gender")]
        public string? Gender { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? StudentCode { get; set; }

        public string? CourseName { get; set; }

        public string? Level { get; set; }

        [NotMapped]
        public string AssignedCourses { get; set; } = "";
    }
}