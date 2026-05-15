using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DAL
{
    public class CourseDAL
    {
        public List<CourseDTO> GetAllCourses()
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    return db.Courses.AsNoTracking().ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi lấy danh sách khóa học: " + ex.Message);
                }
            }
        }
        public void UpdateStudentCount(int courseId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var course = db.Courses.Find(courseId);
                    if (course != null)
                    {
                        // Vì là kiểu int nên không cần ?? 0 nữa, cứ thế cộng trực tiếp
                        course.CurrentStudents = course.CurrentStudents + 1;
                        db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi cập nhật sĩ số khóa học: " + ex.Message);
                }
            }
        }
    }
}
