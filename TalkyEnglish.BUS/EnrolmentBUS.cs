using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    /// <summary>
    /// Lớp xử lý nghiệp vụ đăng ký khóa học và tiến độ học tập
    /// </summary>
    public class EnrolmentBUS
    {
        private readonly EnrolmentDAL _enrolmentDAL = new EnrolmentDAL();

        /// <summary>
        /// Lấy danh sách tiến độ các khóa học của một học viên
        /// </summary>
        public List<EnrolmentDTO> GetStudentProgress(int studentId)
        {
            if (studentId <= 0) return new List<EnrolmentDTO>();

            try
            {
                return _enrolmentDAL.GetStudentProgress(studentId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi BUS GetStudentProgress: " + ex.Message);
            }
        }
        public bool RegisterCourse(int studentId, int courseId)
        {
            if (studentId <= 0 || courseId <= 0) return false;

            try
            {
                // Chỉ gọi hàm đã viết sẵn trong EnrolmentDAL
                return _enrolmentDAL.AddEnrollment(studentId, courseId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi BUS RegisterCourse: " + ex.Message);
            }
        }
    }


}