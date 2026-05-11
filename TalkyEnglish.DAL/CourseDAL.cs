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
        }
}
