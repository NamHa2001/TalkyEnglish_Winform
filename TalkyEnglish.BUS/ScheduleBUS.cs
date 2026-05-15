using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkyEnglish.DTO;
using TalkyEnglish.DAL; // Thêm dòng này để BUS thấy DAL
namespace TalkyEnglish.BUS
{
    public class ScheduleBUS
    {
        private readonly ScheduleDAL _scheduleDal = new ScheduleDAL();

       

        public List<object> GetStudentSchedule(int studentId)
        {
            // Kiểm tra logic nếu cần, sau đó gọi xuống DAL
            if (studentId <= 0) return new List<object>();
            return _scheduleDal.GetStudentSchedule(studentId);
        }
        public string SaveSchedule(ScheduleDTO newSche)
        {
            // 1. Lấy toàn bộ lịch hiện có để so sánh
            var allSchedules = _scheduleDal.GetAll();

            // 2. Logic Check trùng: Cùng Thứ, Cùng Phòng và Giao thoa thời gian
            // Công thức: (Bắt đầu mới < Kết thúc cũ) VÀ (Kết thúc mới > Bắt đầu cũ)
            var conflict = allSchedules.Any(s =>
                s.DayOfWeek == newSche.DayOfWeek &&
                s.RoomName == newSche.RoomName &&
                newSche.StartTime < s.EndTime &&
                newSche.EndTime > s.StartTime
            );

            if (conflict)
            {
                return "Phòng này đã có lớp trong khung giờ này rồi bro ơi!";
            }

            // 3. Nếu không trùng, tiến hành lưu vào DB qua DAL
            bool isSuccess = _scheduleDal.Add(newSche);

            return isSuccess ? "OK" : "Lỗi hệ thống, không thể lưu lịch học.";
        }

        public string UpdateSchedule(ScheduleDTO upSche)
        {
            var allSchedules = _scheduleDal.GetAll();

            // Check trùng nhưng loại trừ chính cái ID đang sửa ra
            var conflict = allSchedules.Any(s =>
                s.ScheduleID != upSche.ScheduleID &&
                s.DayOfWeek == upSche.DayOfWeek &&
                s.RoomName == upSche.RoomName &&
                upSche.StartTime < s.EndTime &&
                upSche.EndTime > s.StartTime
            );

            if (conflict) return "Lịch sửa bị trùng với lớp khác rồi bro!";

            return _scheduleDal.Update(upSche) ? "OK" : "Lỗi khi cập nhật dữ liệu.";
        }

        public List<ScheduleDTO> GetAllSchedules()
        {
            return _scheduleDal.GetAll();
        }

        public bool DeleteSchedule(int scheduleId)
        {
            return _scheduleDal.Delete(scheduleId);
        }



        public List<ScheduleDTO> GetSchedulesByTeacher(int teacherId)
        {
            // Gọi trực tiếp hàm lọc theo ID ở DAL cho nó nhanh và chính xác 100%
            return _scheduleDal.GetByTeacher(teacherId);
        }
    }
}
