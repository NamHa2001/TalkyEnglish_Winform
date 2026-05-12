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
    }
}