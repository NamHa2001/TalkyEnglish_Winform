using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DAL
{
   public class ScheduleDAL
    {
        private readonly TalkyDbContext _db = new TalkyDbContext();

        // Tạm thời để đây, lát mình sẽ viết hàm Add vào đây sau.

        public bool Add(ScheduleDTO schedule)
        {
            try
            {
                _db.Schedules.Add(schedule);
                return _db.SaveChanges() > 0;
            }
            catch { return false; }
        }

        public bool Delete(int scheduleId)
        {
            try
            {
                var item = _db.Schedules.Find(scheduleId);
                if (item != null)
                {
                    _db.Schedules.Remove(item);
                    return _db.SaveChanges() > 0;
                }
                return false;
            }
            catch { return false; }
        }

        public bool Update(ScheduleDTO schedule)
        {
            try
            {
                var existing = _db.Schedules.Find(schedule.ScheduleID);
                if (existing != null)
                {
                    // Cập nhật các trường dữ liệu
                    existing.DayOfWeek = schedule.DayOfWeek;
                    existing.StartTime = schedule.StartTime;
                    existing.EndTime = schedule.EndTime;
                    existing.RoomName = schedule.RoomName;

                    return _db.SaveChanges() > 0;
                }
                return false;
            }
            catch { return false; }
        }

        public List<ScheduleDTO> GetAll()
        {
            using (var db = new TalkyDbContext())
            {
                var query = from s in db.Schedules
                            join c in db.Courses on s.CourseID equals c.CourseID
                            // Đây là chỗ thay đổi: Join vào bảng Phân công thay vì lấy trực tiếp từ Courses
                            join a in db.TeachingAssignments on c.CourseID equals a.CourseID
                            join u in db.Users on a.InstructorID equals u.UserID
                            // Thêm điều kiện lọc để chắc chắn chỉ lấy Giảng viên
                            where u.Role == "Teacher" || u.Role == "Instructor"
                            select new ScheduleDTO
                            {
                                ScheduleID = s.ScheduleID,
                                CourseID = s.CourseID,
                                CourseCode = c.CourseCode,
                                CourseName = c.CourseName,
                                DayOfWeek = s.DayOfWeek,
                                StartTime = s.StartTime,
                                EndTime = s.EndTime,
                                RoomName = s.RoomName,
                                InstructorName = u.FullName // Lấy từ bảng Phân công ra thì chuẩn cơm mẹ nấu luôn!
                            };
                return query.ToList();
            }
        }

   

        public List<ScheduleDTO> GetByTeacher(int instructorId)
        {
            using (var db = new TalkyDbContext())
            {
                var query = from s in db.Schedules
                            join c in db.Courses on s.CourseID equals c.CourseID
                            join a in db.TeachingAssignments on c.CourseID equals a.CourseID
                            join u in db.Users on a.InstructorID equals u.UserID
                            where u.UserID == instructorId
                            select new ScheduleDTO
                            {
                                ScheduleID = s.ScheduleID,
                                CourseID = s.CourseID,
                                CourseCode = c.CourseCode,
                                CourseName = c.CourseName,
                                DayOfWeek = s.DayOfWeek,
                                StartTime = s.StartTime,
                                EndTime = s.EndTime,
                                RoomName = s.RoomName,
                                InstructorName = u.FullName
                            };
                return query.ToList();
            }
        }


        public List<object> GetStudentSchedule(int studentId)
        {
            // Sử dụng DbContext để kết nối Database
            using (var db = new TalkyDbContext())
            {
                var query = from e in db.Enrolments
                            join s in db.Schedules on e.CourseID equals s.CourseID
                            join c in db.Courses on e.CourseID equals c.CourseID
                            where e.StudentID == studentId
                            select new
                            {
                                TenKhoaHoc = c.CourseName,
                                Thu = s.DayOfWeek,
                                BatDau = s.StartTime,
                                KetThuc = s.EndTime,
                                Phong = s.RoomName
                            };
                return query.ToList<object>();
            }
        }
    }
}
