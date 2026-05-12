using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    /// <summary>
    /// Lớp xử lý nghiệp vụ liên quan đến phân công và lịch học
    /// </summary>
    public class AssignmentBUS
    {
        private readonly AssignmentDAL _assignmentDAL = new AssignmentDAL();

        /// <summary>
        /// Lấy lịch học trong ngày hôm nay của một học viên cụ thể
        /// </summary>
        /// <param name="studentId">ID của học viên đang đăng nhập</param>
        /// <returns>Danh sách lịch học hôm nay</returns>
        public List<TeachingAssignmentDTO> GetTodaySchedule(int studentId)
        {
            // Kiểm tra đầu vào cơ bản
            if (studentId <= 0) return new List<TeachingAssignmentDTO>();

            try
            {
                // Gọi xuống DAL để lấy dữ liệu lịch học đã được Join giữa các bảng
                return _assignmentDAL.GetTodayScheduleForStudent(studentId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi BUS GetTodaySchedule: " + ex.Message);
            }
        }

        // Bạn có thể bổ sung các hàm Insert/Update/Delete Assignment vào đây sau 
        // để đồng bộ với AssignmentDAL đã có
    }
}