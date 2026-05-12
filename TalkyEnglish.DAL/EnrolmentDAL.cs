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
                                // Join thêm bảng Users để lấy tên Giảng viên quản lý khóa học
                                join u in db.Users on c.InstructorID equals u.UserID
                                where en.StudentID == studentId
                                select new EnrolmentDTO
                                {
                                    EnrolmentID = en.EnrolmentID,
                                    CourseID = en.CourseID,
                                    CourseName = c.CourseName,
                                    StudentID = en.StudentID,
                                    InstructorName = u.FullName, // Lấy tên GV đổ vào DTO
                                    // Giả định lộ trình 20 buổi để tính %
                                    ProgressValue = db.Attendances.Count(at => at.EnrolmentID == en.EnrolmentID && at.IsPresent) * 100 / 20
                                };
                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetStudentProgress: " + ex.Message);
                }
            }
        }
    }
}