using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkyEnglish.DTO
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string? FullName { get; set; } // Thêm dấu ?
        public string? Email { get; set; }    // Thêm dấu ?
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
        public string? PhoneNumber { get; set; } // Cột này thường dễ bị Null nhất
        public DateTime? CreatedAt { get; set; }
    }
}