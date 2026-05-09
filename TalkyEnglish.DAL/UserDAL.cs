using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DAL
{
    public class UserDAL
    {
        // Khởi tạo DbContext để kết nối Database
        private readonly TalkyDbContext _context = new TalkyDbContext();

        /// <summary>
        /// Lấy toàn bộ danh sách người dùng từ Database
        /// </summary>
        public List<UserDTO> GetAllUsers()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần thiết
                throw new Exception("Lỗi khi lấy danh sách người dùng: " + ex.Message);
            }
        }

        /// <summary>
        /// Thêm một người dùng mới vào Database (Dùng cho Đăng ký)
        /// </summary>
        /// <param name="user">Đối tượng UserDTO chứa thông tin cần lưu</param>
        /// <returns>True nếu thành công, False nếu thất bại</returns>
        public bool AddUser(UserDTO user)
        {
            try
            {
                // Thêm đối tượng vào tập hợp Users của DbContext
                _context.Users.Add(user);

                // Thực thi lệnh INSERT xuống SQL Server
                int result = _context.SaveChanges();

                // Nếu số dòng bị ảnh hưởng > 0 nghĩa là đã lưu thành công
                return result > 0;
            }
            catch (Exception)
            {
                // Có thể xảy ra lỗi nếu trùng Email (Unique) hoặc mất kết nối
                return false;
            }
        }

        /// <summary>
        /// Cập nhật thông tin người dùng (Dùng cho đổi mật khẩu hoặc sửa hồ sơ)
        /// </summary>
        public bool UpdateUser(UserDTO user)
        {
            try
            {
                var existingUser = _context.Users.Find(user.UserID);
                if (existingUser != null)
                {
                    _context.Entry(existingUser).CurrentValues.SetValues(user);
                    return _context.SaveChanges() > 0;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Đếm tổng số học viên
        public int GetTotalStudents()
        {
            using (var db = new TalkyDbContext())
            {
                return db.Users.Count(u => u.Role == "Student");
            }
        }

        // Đếm tổng số giảng viên
        public int GetTotalInstructors()
        {
            using (var db = new TalkyDbContext())
            {
                return db.Users.Count(u => u.Role == "Instructor");
            }
        }
    }
}