using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DTO;
using Microsoft.EntityFrameworkCore;

namespace TalkyEnglish.DAL
{
    public class EnrolmentDAL
    {
        // Lấy danh sách khóa học học viên đang học và tính % tiến độ
        public List<EnrolmentDTO> GetStudentProgress(int studentId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var query = from en in db.Enrolments
                                join c in db.Courses on en.CourseID equals c.CourseID
                                join u in db.Users on c.InstructorID equals u.UserID
                                where en.StudentID == studentId
                                select new EnrolmentDTO
                                {
                                    EnrollmentID = en.EnrollmentID,
                                    CourseID = en.CourseID,
                                    CourseName = c.CourseName,
                                    StudentID = en.StudentID,
                                    EnrollmentDate = en.EnrollmentDate, // Đã đổi tên ở đây
                                    InstructorName = u.FullName,
                                    ProgressValue = db.Attendances.Count(at => at.EnrolmentID == en.EnrollmentID && at.IsPresent) * 100 / 20
                                };
                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetStudentProgress: " + ex.Message);
                }
            }
        }
        // Thêm hàm này vào trong class EnrolmentDAL
        public bool AddEnrollment(int studentId, int courseId)
        {
            using (var db = new TalkyDbContext())
            {
                // 1. Kiểm tra xem học viên này đã đăng ký khóa này chưa
                bool isExisted = db.Enrolments.Any(e => e.StudentID == studentId && e.CourseID == courseId);
                if (isExisted) return false;

                // 2. Tạo đối tượng mới để lưu
                var newEntry = new EnrolmentDTO
                {
                    StudentID = studentId,
                    CourseID = courseId,
                    EnrollmentDate = DateTime.Now, // Lưu ý: Tên thuộc tính mới EnrollmentDate
                    //PaymentStatus = "Chưa đóng"
                };

                db.Enrolments.Add(newEntry);
                return db.SaveChanges() > 0;
            }
        }
    }
}