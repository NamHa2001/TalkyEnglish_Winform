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
    }

}
