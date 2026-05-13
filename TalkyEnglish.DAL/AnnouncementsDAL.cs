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
    }
}