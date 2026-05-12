using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    /// <summary>
    /// Lớp xử lý nghiệp vụ liên quan đến người dùng
    /// Đảm bảo các quy tắc kinh doanh trước khi giao tiếp với tầng dữ liệu (DAL)
    /// </summary>
    public class UserBUS
    {
        // Khai báo readonly để đảm bảo tính an toàn và chỉ khởi tạo một lần
        private readonly UserDAL _userDAL = new UserDAL();

        /// <summary>
        /// Xác thực thông tin đăng nhập người dùng
        /// </summary>
        /// <param name="email">Email đăng nhập</param>
        /// <param name="password">Mật khẩu (Text thuần)</param>
        /// <returns>Đối tượng UserDTO nếu hợp lệ, ngược lại trả về null</returns>
        public UserDTO Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return null;

            // Sử dụng hàm truy vấn trực tiếp để đảm bảo lấy đủ các cột tính toán (StudentCode)
            return _userDAL.GetUserForLogin(email.Trim(), password.Trim());
        
        }

        /// <summary>
        /// Nghiệp vụ đăng ký tài khoản mới cho học viên
        /// </summary>
        /// <param name="newUser">Dữ liệu người dùng mới</param>
        /// <param name="confirmPassword">Mật khẩu nhập lại</param>
        /// <param name="isTermAccepted">Trạng thái xác nhận điều khoản</param>
        /// <returns>Chuỗi "SUCCESS" nếu thành công, hoặc chuỗi thông báo lỗi cụ thể</returns>
        public string Register(UserDTO newUser, string confirmPassword, bool isTermAccepted)
        {
            // Bước 1: Kiểm tra quy tắc nghiệp vụ (Business Rule) - Checkbox
            if (!isTermAccepted)
            {
                return "Bạn phải đồng ý với điều khoản sử dụng của Talky English!";
            }

            // Bước 2: Kiểm tra tính đầy đủ của dữ liệu (Validation)
            if (string.IsNullOrWhiteSpace(newUser.FullName) ||
                string.IsNullOrWhiteSpace(newUser.Email) ||
                string.IsNullOrWhiteSpace(newUser.PasswordHash))
            {
                return "Vui lòng điền đầy đủ các thông tin bắt buộc (*)!";
            }

            // Bước 3: Kiểm tra tính khớp của mật khẩu
            if (newUser.PasswordHash != confirmPassword)
            {
                return "Mật khẩu xác nhận không khớp!";
            }

            // Bước 4: Kiểm tra định dạng Email sơ bộ
            if (!newUser.Email.Contains("@") || !newUser.Email.Contains("."))
            {
                return "Địa chỉ Email không đúng định dạng!";
            }

            // Bước 5: Kiểm tra Email đã tồn tại trong hệ thống chưa
            // Dùng Any() để tối ưu hiệu năng thay vì lấy toàn bộ danh sách
            bool isEmailTaken = _userDAL.GetAllStudents().Any(u =>
                string.Equals(u.Email, newUser.Email, StringComparison.OrdinalIgnoreCase));

            if (isEmailTaken)
            {
                return "Email này đã được sử dụng. Vui lòng chọn Email khác!";
            }

            // Bước 6: Thiết lập các giá trị mặc định cho User mới
            newUser.Role = "Student"; // Mặc định là học viên
            newUser.CreatedAt = DateTime.Now;

            // Bước 7: Thực hiện lưu xuống Database qua tầng DAL
            bool isSaved = _userDAL.AddUser(newUser);

            return isSaved ? "SUCCESS" : "Đã có lỗi xảy ra trong quá trình lưu dữ liệu. Vui lòng thử lại sau!";
        }

        /// <summary>
        /// Lấy danh sách giảng viên để hiển thị trên Dashboard/Home
        /// </summary>
        public List<UserDTO> GetTopInstructors()
        {
            return _userDAL.GetAllStudents()
                           .Where(u => string.Equals(u.Role, "Instructor", StringComparison.OrdinalIgnoreCase))
                           .ToList();
        }

        public int GetStudentCount() => _userDAL.GetTotalStudents();
        public int GetInstructorCount() => _userDAL.GetTotalInstructors();
        public List<UserDTO> GetAllStudents()
        {
            // Lọc danh sách User, chỉ lấy những người có Role là "Student"
            return _userDAL.GetAllStudents().Where(u => u.Role == "Student").ToList();
        }


        //Kiểm tra email đã có trong db chưa
        public bool IsEmailExist(string email)
        {
            // Lấy toàn bộ user và kiểm tra xem có ai trùng email không
            return _userDAL.GetAllStudents().Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public bool DeleteUser(int userId)
        {
            return _userDAL.DeleteUser(userId);
        }
        public bool UpdateUser(UserDTO user)
        {
            // 1. Kiểm tra logic nghiệp vụ nếu cần (ví dụ: FullName không được rỗng)
            if (string.IsNullOrEmpty(user.FullName)) return false;

            // 2. Gọi xuống tầng DAL để lưu vào Database
            return _userDAL.UpdateUser(user);
        }
        public List<UserDTO> SearchStudents(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return GetAllStudents();
            return _userDAL.SearchStudents(keyword.Trim());
        }

        public List<UserDTO> GetAllInstructors()
        {
            // Gọi xuống hàm ở DAL mà bạn vừa thêm lúc nãy
            return _userDAL.GetAllInstructors();
        }

        public bool InsertInstructor(UserDTO user)
        {
            return _userDAL.InsertInstructor(user);
        }

        public bool UpdateInstructor(UserDTO user)
        {
            return _userDAL.UpdateInstructor(user);
        }

        public bool DeleteInstructor(int userId)
        {
            return _userDAL.DeleteInstructor(userId);
        }

        public bool AddStudent(UserDTO student)
        {
            if (string.IsNullOrWhiteSpace(student.FullName)) return false;
            return _userDAL.InsertStudent(student);
        }

        public bool UpdateStudent(UserDTO student)
        {
            if (student.UserID <= 0) return false;
            return _userDAL.UpdateUser(student); // Dùng chung hàm UpdateUser vì logic giống nhau
        }
    }
}