using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkyEnglish.DTO;

namespace TalkyEnglish.DAL
{
    public class AssignmentDAL
    {
        public bool InsertAssignment(TeachingAssignmentDTO assignment)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    // 1. Tạo một đối tượng thực thể mới, KHÔNG COPY cái ID
                    var entity = new TeachingAssignmentDTO
                    {
                        // Tuyệt đối không gán AssignmentID ở đây
                        InstructorID = assignment.InstructorID,
                        CourseID = assignment.CourseID,
                        AssignedDate = DateTime.Now,
                        Note = assignment.Note ?? "Phân công mới"
                    };

                    db.TeachingAssignments.Add(entity);
                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    // Đoạn này để bạn đọc được lỗi thật sự là gì nếu vẫn hỏng
                    var innerMsg = ex.InnerException != null ? ex.InnerException.Message : "";
                    var deeperMsg = ex.InnerException?.InnerException != null ? ex.InnerException.InnerException.Message : "";
                    throw new Exception($"Lỗi thực sự: {ex.Message} | Chi tiết: {innerMsg} | {deeperMsg}");
                }
            }
        }
        public bool DeleteAssignment(int assignmentId)
        {
            try
            {
                using (var db = new TalkyDbContext())
                {
                    // 1. Tìm bản ghi cần xóa dựa trên ID
                    var assignment = db.TeachingAssignments.Find(assignmentId);

                    if (assignment != null)
                    {
                        // 2. Lệnh xóa khỏi tập hợp
                        db.TeachingAssignments.Remove(assignment);

                        // 3. Lưu thay đổi xuống Database
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Thêm dấu ? vào sau DateTime
        public bool UpdateAssignment(int assignmentId, string newNote, DateTime? newDate)
        {
            try
            {
                using (var db = new TalkyDbContext())
                {
                    var assignment = db.TeachingAssignments.Find(assignmentId);
                    if (assignment != null)
                    {
                        assignment.Note = newNote;
                        assignment.AssignedDate = newDate; // Bây giờ cả 2 đều là DateTime? nên sẽ hết lỗi
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception) { return false; }
        }

        // Lấy lịch học hôm nay cho một học viên cụ thể
        public List<TeachingAssignmentDTO> GetTodayScheduleForStudent(int studentId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    DateTime today = DateTime.Today;

                    var query = from en in db.Enrolments
                                join ta in db.TeachingAssignments on en.CourseID equals ta.CourseID
                                join u in db.Users on ta.InstructorID equals u.UserID
                                join c in db.Courses on ta.CourseID equals c.CourseID
                                where en.StudentID == studentId &&
                                      ta.AssignedDate.HasValue &&
                                      ta.AssignedDate.Value.Date == today
                                select new TeachingAssignmentDTO
                                {
                                    AssignmentID = ta.AssignmentID,
                                    CourseName = c.CourseName,
                                    InstructorName = u.FullName,
                                    AssignedDate = ta.AssignedDate,
                                    Note = ta.Note
                                };
                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetTodayScheduleForStudent: " + ex.Message);
                }
            }
        }
    }

}
