using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    public class CourseBUS
    {
        private readonly TalkyDbContext _context;
        // Thêm hàm này vào class CourseBUS
        public List<UserDTO> GetInstructors()
        {
            // Giả sử những người có Role là 'Teacher' hoặc 'Instructor' là giảng viên
            // Nếu bạn chưa phân quyền, tạm thời lấy tất cả Users để chọn
            return _context.Users.ToList();
        }
        public CourseBUS()
        {
            _context = new TalkyDbContext();
        }

        // 1. Lấy danh sách khóa học kèm tên giảng viên (Đón đầu tương lai)
        public List<CourseDTO> GetAllCourses()
        {
            var query = from c in _context.Courses
                        join u in _context.Users on c.InstructorID equals u.UserID into instructorGroup
                        from u in instructorGroup.DefaultIfEmpty() // Left Join để nếu chưa có GV thì vẫn hiện khóa học
                        select new CourseDTO
                        {
                            CourseID = c.CourseID,
                            CourseCode = c.CourseCode,
                            CourseName = c.CourseName,
                            Price = c.Price,
                            Description = c.Description,
                            Level = c.Level,
                            Status = c.Status,
                            InstructorID = c.InstructorID,
                            CreatedAt = c.CreatedAt,
                            // Gán tên giảng viên từ bảng Users vào DTO để hiển thị lên Grid
                            InstructorName = u != null ? u.FullName : "Chưa phân công"
                        };

            return query.ToList();
        }

        // 2. Hàm tìm kiếm khóa học theo tên hoặc mã
        public List<CourseDTO> SearchCourses(string keyword)
        {
            var list = GetAllCourses();
            if (string.IsNullOrEmpty(keyword)) return list;

            return list.Where(c => c.CourseName.ToLower().Contains(keyword.ToLower()) ||
                                   c.CourseCode.ToLower().Contains(keyword.ToLower())).ToList();
        }

        // 3. Hàm đếm tổng số khóa học
        public int GetTotalCourses()
        {
            return _context.Courses.Count();
        }
        // Hàm Thêm mới khóa học
        public bool AddCourse(CourseDTO course)
        {
            try
            {
                _context.Courses.Add(course);
                return _context.SaveChanges() > 0;
            }
            catch { return false; }
        }

        // Hàm Cập nhật khóa học
        public bool UpdateCourse(CourseDTO course)
        {
            try
            {
                var existing = _context.Courses.Find(course.CourseID);
                if (existing == null) return false;

                // Cập nhật các trường cho phép sửa
                existing.CourseName = course.CourseName;
                existing.Price = course.Price;
                existing.Description = course.Description;
                existing.Level = course.Level;
                existing.Status = course.Status;
                existing.InstructorID = course.InstructorID;
                // Lưu ý: Không sửa CourseCode và CreatedAt vì đó là dữ liệu hệ thống

                return _context.SaveChanges() > 0;
            }
            catch { return false; }
        }

    }
}