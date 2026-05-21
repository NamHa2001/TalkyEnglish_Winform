using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    public class AnnouncementsBUS
    {
        private readonly AnnouncementsDAL _announcementsDAL = new AnnouncementsDAL();

        // 1. Hàm Gửi Thông Báo (Nghiệp vụ mới quan trọng nhất)
        public bool SendNotification(AnnouncementsDTO announceDto)
        {
            try
            {
                // Bước A: Lưu nội dung thông báo gốc vào DB
                int newId = _announcementsDAL.InsertAnnouncement(announceDto);

                if (newId > 0)
                {
                    // Bước B: Xác định danh sách UserID cần nhận thông báo
                    List<int> receiverList = new List<int>();

                    using (var db = new TalkyDbContext())
                    {
                        if (announceDto.TargetType == "Individual")
                        {
                            // Nếu gửi đích danh 1 người
                            if (announceDto.ReceiverID.HasValue)
                                receiverList.Add(announceDto.ReceiverID.Value);
                        }
                        else
                        {
                            // Nếu gửi theo nhóm (Teacher/Student/All)
                            var query = db.Users.AsQueryable();

                            if (announceDto.TargetType == "Teacher")
                                query = query.Where(u => u.Role == "Teacher" || u.Role == "Instructor");
                            else if (announceDto.TargetType == "Student")
                                query = query.Where(u => u.Role == "Student");

                            // Lấy danh sách ID
                            receiverList = query.Select(u => u.UserID).ToList();
                        }
                    }

                    // Bước C: Tạo trạng thái "Chưa đọc" cho từng người trong danh sách
                    foreach (int uId in receiverList)
                    {
                        var status = new NotificationStatusDTO
                        {
                            AnnounceID = newId,
                            UserID = uId,
                            IsRead = false
                        };
                        _announcementsDAL.InsertNotificationStatus(status);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi BUS SendNotification: " + ex.Message);
            }
        }

        // 2. Các hàm lấy dữ liệu cũ (Giữ nguyên)
        public List<AnnouncementsDTO> GetRecentAnnouncements()
        {
            return _announcementsDAL.GetTopRecentAnnouncements();
        }

        public List<AnnouncementsDTO> GetAllAnnouncements()
        {
            return _announcementsDAL.GetAllAnnouncements();
        }

        public List<AnnouncementsDTO> GetByTargetType(string targetType)
        {
            return _announcementsDAL.GetAnnouncementsByTargetType(targetType);
        }

        public bool DeleteAnnouncement(int id)
        {
            if (id <= 0) return false;
            return _announcementsDAL.DeleteAnnouncement(id);
        }

        public List<AnnouncementsDTO> GetNotificationsForStudent(int studentId)
        {
            return _announcementsDAL.GetNotificationsForStudent(studentId);
        }

        public List<AnnouncementsDTO> GetNotificationsForInstructor(int instructorId)
        {
            return _announcementsDAL.GetNotificationsForInstructor(instructorId);
        }

        public bool MarkAsRead(int announceId, int userId)
        {
            if (announceId <= 0 || userId <= 0) return false;
            return _announcementsDAL.MarkAsRead(announceId, userId);
        }
    }
}