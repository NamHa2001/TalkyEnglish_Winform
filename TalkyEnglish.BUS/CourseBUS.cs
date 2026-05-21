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
        private readonly CourseDAL _courseDAL = new CourseDAL();
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
                        from u in instructorGroup.DefaultIfEmpty()
                        select new CourseDTO
                        {
                            CourseID = c.CourseID,
                            CourseCode = c.CourseCode,
                            CourseName = c.CourseName,
                            Price = c.Price,
                            Description = c.Description,
                            Duration = c.Duration,
                            Level = c.Level,
                            Status = c.Status,
                            InstructorID = c.InstructorID,
                            CreatedAt = c.CreatedAt,
                            // THÊM 2 DÒNG NÀY ĐỂ LẤY DỮ LIỆU SĨ SỐ
                            MaxStudents = c.MaxStudents,
                            CurrentStudents = c.CurrentStudents,
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

                existing.CourseName = course.CourseName;
                existing.Price = course.Price;
                existing.Description = course.Description;
                existing.Duration = course.Duration;
                existing.Level = course.Level;
                existing.Status = course.Status;
                existing.InstructorID = course.InstructorID;

                // GIỮ LẠI SĨ SỐ KHI CẬP NHẬT
                existing.MaxStudents = course.MaxStudents;
                existing.CurrentStudents = course.CurrentStudents;

                return _context.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public string GenerateCourseCode()
        {
            // Tìm khóa học có ID cao nhất
            var lastCourse = _context.Courses.OrderByDescending(c => c.CourseID).FirstOrDefault();

            if (lastCourse == null)
            {
                return "KH001"; // Nếu chưa có dữ liệu
            }

            // Lấy ID lớn nhất cộng thêm 1 và định dạng chuỗi
            int nextId = lastCourse.CourseID + 1;
            return "KH" + nextId.ToString("D3"); // D3 sẽ tạo ra 001, 002...
        }

        public bool DeleteCourse(int courseId)
        {
            try
            {
                var course = _context.Courses.Find(courseId);
                if (course != null)
                {
                    _context.Courses.Remove(course);
                    return _context.SaveChanges() > 0;
                }
                return false;
            }
            catch { return false; }
        }

    

        public List<CourseDTO> GetCoursesByInstructor(int instructorId)
        {
            if (instructorId <= 0) return GetAllCourses();

            var query = from c in _context.Courses
                        join u in _context.Users on c.InstructorID equals u.UserID into ig
                        from u in ig.DefaultIfEmpty()
                        where c.InstructorID == instructorId
                        select new CourseDTO
                        {
                            CourseID         = c.CourseID,
                            CourseCode       = c.CourseCode,
                            CourseName       = c.CourseName,
                            Price            = c.Price,
                            Description      = c.Description,
                            Duration         = c.Duration,
                            Level            = c.Level,
                            Status           = c.Status,
                            InstructorID     = c.InstructorID,
                            CreatedAt        = c.CreatedAt,
                            MaxStudents      = c.MaxStudents,
                            CurrentStudents  = c.CurrentStudents,
                            InstructorName   = u != null ? u.FullName : "Chưa phân công"
                        };
            return query.ToList();
        }

        public List<CourseDTO> FilterCourses(string keyword, int instructorId, string level, string status, DateTime? date)
        {
            // 1. Lấy toàn bộ danh sách gốc đã kèm tên Giảng viên
            var list = GetAllCourses();

            // 2. Lọc theo Từ khóa (Tên/Mã)
            if (!string.IsNullOrEmpty(keyword))
            {
                list = list.Where(c => c.CourseName.ToLower().Contains(keyword.ToLower()) ||
                                       c.CourseCode.ToLower().Contains(keyword.ToLower())).ToList();
            }

            // 3. Lọc theo Giảng viên (nếu chọn khác "Tất cả" - ID là -1)
            if (instructorId != -1)
            {
                list = list.Where(c => c.InstructorID == instructorId).ToList();
            }

            // 4. Lọc theo Trình độ
            if (level != "Tất cả" && !string.IsNullOrEmpty(level))
            {
                list = list.Where(c => c.Level == level).ToList();
            }

            // 5. Lọc theo Trạng thái
            if (status != "Tất cả" && !string.IsNullOrEmpty(status))
            {
                list = list.Where(c => c.Status == status).ToList();
            }

            // 6. Lọc theo Ngày (Chỉ so sánh ngày, bỏ qua giờ)
            if (date.HasValue)
            {
                //list = list.Where(c => c.CreatedAt.HasValue && c.CreatedAt.Value.Date == date.Value.Date).ToList();
            }

            return list;
        }

       
    }
}