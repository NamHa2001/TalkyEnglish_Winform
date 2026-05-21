using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DTO;
using Microsoft.EntityFrameworkCore;

namespace TalkyEnglish.DAL
{
    public class AnnouncementsDAL
    {
        // Lấy danh sách 5 thông báo mới nhất để hiển thị lên Dashboard
        public List<AnnouncementsDTO> GetTopRecentAnnouncements()
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    return db.Announcements
                             .OrderByDescending(a => a.PublishDate) // Sắp xếp mới nhất lên đầu
                             .Take(5) // Chỉ lấy 5 tin
                             .AsNoTracking()
                             .ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetTopRecentAnnouncements: " + ex.Message);
                }
            }
        }

        // Lấy toàn bộ thông báo (dành cho trang Xem tất cả)
        public List<AnnouncementsDTO> GetAllAnnouncements()
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    return db.Announcements
                             .OrderByDescending(a => a.PublishDate)
                             .AsNoTracking()
                             .ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetAllAnnouncements: " + ex.Message);
                }
            }
        }

        //MỚI
        // 1. Hàm lưu thông báo mới và trả về ID vừa tạo
        public int InsertAnnouncement(AnnouncementsDTO dto)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    db.Announcements.Add(dto);
                    db.SaveChanges();

                    // Sau khi SaveChanges, EF tự động nạp ID từ SQL về lại dto.AnnounceID
                    return dto.AnnounceID;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL InsertAnnouncement: " + ex.Message);
                }
            }
        }

        // 2. Hàm tạo trạng thái chưa đọc cho từng người nhận
        public bool InsertNotificationStatus(NotificationStatusDTO statusDto)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    db.NotificationStatuses.Add(statusDto);
                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL InsertNotificationStatus: " + ex.Message);
                }
            }
        }

        // 3. Lấy thông báo dành riêng cho một student, kèm trạng thái đã đọc
        public List<AnnouncementsDTO> GetNotificationsForStudent(int studentId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var query = from a in db.Announcements
                                join ns in db.NotificationStatuses
                                    on new { AID = a.AnnounceID, UID = studentId }
                                    equals new { AID = ns.AnnounceID, UID = ns.UserID }
                                    into statusGroup
                                from status in statusGroup.DefaultIfEmpty()
                                where a.TargetType == "All"
                                   || a.TargetType == "Student"
                                   || (a.TargetType == "Individual" && a.ReceiverID == studentId)
                                orderby a.PublishDate descending
                                select new AnnouncementsDTO
                                {
                                    AnnounceID    = a.AnnounceID,
                                    Title         = a.Title,
                                    Content       = a.Content,
                                    Category      = a.Category,
                                    TargetType    = a.TargetType,
                                    PriorityLevel = a.PriorityLevel,
                                    PublishDate   = a.PublishDate,
                                    SenderID      = a.SenderID,
                                    ReceiverID    = a.ReceiverID,
                                    IsRead        = status != null && status.IsRead
                                };

                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetNotificationsForStudent: " + ex.Message);
                }
            }
        }

        // 4. Đánh dấu đã đọc
        public bool MarkAsRead(int announceId, int userId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var status = db.NotificationStatuses
                        .FirstOrDefault(s => s.AnnounceID == announceId && s.UserID == userId);

                    if (status != null)
                    {
                        status.IsRead = true;
                        status.ReadAt = DateTime.Now;
                    }
                    else
                    {
                        db.NotificationStatuses.Add(new NotificationStatusDTO
                        {
                            AnnounceID = announceId,
                            UserID     = userId,
                            IsRead     = true,
                            ReadAt     = DateTime.Now
                        });
                    }
                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL MarkAsRead: " + ex.Message);
                }
            }
        }

        // 5. Lấy thông báo dành cho giảng viên, kèm trạng thái đã đọc
        public List<AnnouncementsDTO> GetNotificationsForInstructor(int instructorId)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var query = from a in db.Announcements
                                join ns in db.NotificationStatuses
                                    on new { AID = a.AnnounceID, UID = instructorId }
                                    equals new { AID = ns.AnnounceID, UID = ns.UserID }
                                    into statusGroup
                                from status in statusGroup.DefaultIfEmpty()
                                where a.TargetType == "All"
                                   || a.TargetType == "Teacher"
                                   || (a.TargetType == "Individual" && a.ReceiverID == instructorId)
                                orderby a.PublishDate descending
                                select new AnnouncementsDTO
                                {
                                    AnnounceID    = a.AnnounceID,
                                    Title         = a.Title,
                                    Content       = a.Content,
                                    Category      = a.Category,
                                    TargetType    = a.TargetType,
                                    PriorityLevel = a.PriorityLevel,
                                    PublishDate   = a.PublishDate,
                                    SenderID      = a.SenderID,
                                    ReceiverID    = a.ReceiverID,
                                    IsRead        = status != null && status.IsRead
                                };
                    return query.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetNotificationsForInstructor: " + ex.Message);
                }
            }
        }

        // 6. Lọc thông báo theo đối tượng nhận
        public List<AnnouncementsDTO> GetAnnouncementsByTargetType(string targetType)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    return db.Announcements
                             .Where(a => a.TargetType == targetType)
                             .OrderByDescending(a => a.PublishDate)
                             .AsNoTracking()
                             .ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL GetAnnouncementsByTargetType: " + ex.Message);
                }
            }
        }

        // 4. Xóa thông báo và các trạng thái đọc liên quan
        public bool DeleteAnnouncement(int id)
        {
            using (var db = new TalkyDbContext())
            {
                try
                {
                    var statuses = db.NotificationStatuses.Where(s => s.AnnounceID == id).ToList();
                    if (statuses.Any())
                        db.NotificationStatuses.RemoveRange(statuses);

                    var item = db.Announcements.Find(id);
                    if (item == null) return false;
                    db.Announcements.Remove(item);
                    return db.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Lỗi DAL DeleteAnnouncement: " + ex.Message);
                }
            }
        }
    }
}