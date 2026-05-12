using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DTO;
using Microsoft.EntityFrameworkCore;

namespace TalkyEnglish.DAL
{
    public class UserDAL
    {
        // XÓA BỎ biến private readonly TalkyDbContext _context cũ ở đây.
        // Tất cả các hàm bên dưới sẽ tự mở và đóng kết nối riêng để đảm bảo dữ liệu luôn tươi.

        public List<UserDTO> GetAllStudents()
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    // Lấy danh sách học viên và bao gồm cả các cột mới
                    return db.Users
                             .Where(u => u.Role == "Student")
                             .AsNoTracking()
                             .ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetAllStudents: " + ex.Message);
                }
            }
        }

        public List<UserDTO> GetAllInstructors()
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    // Lọc những người có Role là "Instructor"
                    return db.Users
                        .Where(u => u.Role == "Instructor")
                        .AsNoTracking()
                        .ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetAllInstructors: " + ex.Message);
                }
            }
        }

        public bool AddUser(UserDTO user)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    db.Users.Add(user);
                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL AddUser: " + ex.Message);
                }
            }
        }

        public bool UpdateUser(UserDTO user)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var existingUser = db.Users.Find(user.UserID);
                    if (existingUser != null)
                    {
                        // Cập nhật giá trị mới vào thực thể đang được theo dõi
                        db.Entry(existingUser).CurrentValues.SetValues(user);

                        // Đảm bảo trạng thái là Modified để EF sinh lệnh UPDATE
                        db.Entry(existingUser).State = EntityState.Modified;

                        return db.SaveChanges() > 0;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL UpdateUser: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }
        }

        public bool DeleteUser(int userId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var user = db.Users.Find(userId);
                    if (user != null)
                    {
                        db.Users.Remove(user);
                        return db.SaveChanges() > 0;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL DeleteUser: " + ex.Message);
                }
            }
        }

        public int GetTotalStudents()
        {
            using (var db = new TalkyDbContext())
            {
                return db.Users.Count(u => u.Role == "Student");
            }
        }

        public int GetTotalInstructors()
        {
            using (var db = new TalkyDbContext())
            {
                return db.Users.Count(u => u.Role == "Instructor");
            }
        }

        public List<UserDTO> SearchStudents(string keyword)
        {
            using (var db = new TalkyDbContext())
            {
                // Lọc danh sách: phải là Student VÀ (Tên chứa từ khóa HOẶC Email chứa từ khóa)
                return db.Users
                    .Where(u => u.Role == "Student" &&
                               (u.FullName.Contains(keyword) || u.Email.Contains(keyword)))
                    .AsNoTracking()
                    .ToList();
            }
        }

        // Hàm Thêm mới Giảng viên
        public bool InsertInstructor(UserDTO user)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    // Ép cứng Role là Instructor để không bị lẫn lộn
                    user.Role = "Instructor";
                    user.CreatedAt = DateTime.Now;

                    db.Users.Add(user);
                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL Insert: " + ex.Message);
                }
            }
        }

        // Hàm Cập nhật Giảng viên
        public bool UpdateInstructor(UserDTO user)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var existingUser = db.Users.Find(user.UserID);
                    if (existingUser != null)
                    {
                        existingUser.FullName = user.FullName;
                        existingUser.Email = user.Email;
                        existingUser.PhoneNumber = user.PhoneNumber;
                        existingUser.Birthday = user.Birthday;
                        existingUser.Gender = user.Gender;
                        existingUser.Specialization = user.Specialization;
                        existingUser.Degree = user.Degree;
                        existingUser.Status = user.Status;
                        // Không cập nhật Password và Role ở đây để bảo mật

                        return db.SaveChanges() > 0;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL Update: " + ex.Message);
                }
            }
        }

        public List<UserDTO> SearchStudentsAdvanced(string keyword, string course, string level, string status)
        {
            using (var db = new TalkyDbContext())
            {
                // Luôn bắt đầu lọc với Role là Student
                var query = db.Users.Where(u => u.Role == "Student");

                // 1. Lọc theo từ khóa (Tên hoặc Email hoặc Mã học viên)
                if (!string.IsNullOrEmpty(keyword))
                {
                    query = query.Where(u => u.FullName.Contains(keyword) ||
                                           u.Email.Contains(keyword) ||
                                           u.StudentCode.Contains(keyword));
                }

                // 2. Lọc theo Khóa học
                if (!string.IsNullOrEmpty(course) && course != "Tất cả")
                {
                    query = query.Where(u => u.CourseName == course);
                }

                // 3. Lọc theo Trình độ
                if (!string.IsNullOrEmpty(level) && level != "Tất cả")
                {
                    query = query.Where(u => u.Level == level);
                }

                // 4. Lọc theo Trạng thái
                if (!string.IsNullOrEmpty(status) && status != "Tất cả")
                {
                    query = query.Where(u => u.Status == status);
                }

                return query.AsNoTracking().ToList();
            }
        }

        public bool DeleteInstructor(int userId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var user = db.Users.Find(userId);
                    if (user != null)
                    {
                        db.Users.Remove(user);
                        return db.SaveChanges() > 0;
                    }
                    return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL Delete: " + ex.Message);
                }
            }
        }

        public bool InsertStudent(UserDTO user)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    user.Role = "Student"; // Đảm bảo luôn là Student
                    if (user.CreatedAt == null) user.CreatedAt = DateTime.Now;
                    db.Users.Add(user);
                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi chi tiết: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }
        }

        public List<TeachingAssignmentDTO> GetAllAssignments()
        {
            using (var db = new TalkyDbContext())
            {
                // Join 3 bảng để lấy đủ tên hiển thị
                var query = from a in db.TeachingAssignments
                            join u in db.Users on a.InstructorID equals u.UserID
                            join c in db.Courses on a.CourseID equals c.CourseID
                            select new TeachingAssignmentDTO
                            {
                                AssignmentID = a.AssignmentID,
                                InstructorID = a.InstructorID,
                                CourseID = a.CourseID,
                                InstructorName = u.FullName, // Bạn cần thêm property này vào DTO nếu chưa có
                                CourseName = c.CourseName,   // Tương tự
                                AssignedDate = a.AssignedDate
                            };
                return query.ToList();
            }
        }

        // Thêm hàm này để lấy toàn bộ User phục vụ đăng nhập
        public List<UserDTO> GetAllUsers()
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    return db.Users.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetAllUsers: " + ex.Message);
                }
            }
        }

        // Thêm hàm này vào UserDAL.cs
        public UserDTO GetUserForLogin(string email, string password)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    // Lấy duy nhất 1 User khớp Email và Password, lấy đủ tất cả các cột
                    var user = db.Users
                                 .AsNoTracking()
                                 .FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

                    return user; // Nếu không thấy sẽ trả về null
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetUserForLogin: " + ex.Message);
                }
            }
        }
    }
}