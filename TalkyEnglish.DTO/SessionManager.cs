using System;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DTO
{
    /// <summary>
    /// Lớp tĩnh lưu trữ thông tin người dùng đang đăng nhập trong hệ thống
    /// </summary>
    public static class SessionManager
    {
        // Lưu toàn bộ đối tượng User để có thể lấy FullName, Role, UserID ở bất cứ đâu
        public static UserDTO CurrentUser { get; set; }

        /// <summary>
        /// Hàm xóa thông tin khi người dùng đăng xuất
        /// </summary>
        public static void Clear()
        {
            CurrentUser = null;
        }
    }
}