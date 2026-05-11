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

        public List<UserDTO> GetAllUsers()
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    return db.Users.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetAllUsers: " + ex.Message);
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
    }
}