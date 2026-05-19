using System;
using System.Collections.Generic;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    public class GradesBUS
    {
        public List<GradesDTO> GetGradesByStudentID(int studentId)
        {
            return _gradesDAL.GetGradesByStudentID(studentId);
        }
        private readonly GradesDAL _gradesDAL = new GradesDAL();

        /// <summary>
        /// Lấy danh sách điểm số và học viên theo khóa học
        /// </summary>
        public List<GradesDTO> GetStudentGradesByCourse(int courseId)
        {
            if (courseId <= 0) return new List<GradesDTO>();
            try
            {
                return _gradesDAL.GetStudentGradesByCourse(courseId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi BUS GetStudentGradesByCourse: " + ex.Message);
            }
        }

        /// <summary>
        /// Xử lý cập nhật hoặc thêm mới điểm học viên
        /// </summary>
        public bool SaveOrUpdateGrade(GradesDTO grade)
        {
            if (grade.EnrolmentID <= 0) return false;

            // Validate điểm số nằm trong khoảng hợp lệ từ 0 đến 10
            if (grade.AttendanceScore < 0 || grade.AttendanceScore > 10 ||
                grade.MidTerm < 0 || grade.MidTerm > 10 ||
                grade.FinalTerm < 0 || grade.FinalTerm > 10)
            {
                return false;
            }

            try
            {
                return _gradesDAL.SaveOrUpdateGrade(grade);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi BUS SaveOrUpdateGrade: " + ex.Message);
            }
        }
    }
}