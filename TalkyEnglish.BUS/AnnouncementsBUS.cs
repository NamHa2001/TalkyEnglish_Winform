using System;
using System.Collections.Generic;
using System.Linq;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.BUS
{
    /// <summary>
    /// Lớp xử lý nghiệp vụ liên quan đến thông báo hệ thống
    /// </summary>
    public class AnnouncementsBUS
    {
        private readonly AnnouncementsDAL _announcementsDAL = new AnnouncementsDAL();

        /// <summary>
        /// Lấy 5 thông báo mới nhất cho Dashboard học viên
        /// </summary>
        /// <returns>Danh sách DTO thông báo</returns>
        public List<AnnouncementsDTO> GetRecentAnnouncements()
        {
            try
            {
                // Gọi xuống DAL để lấy dữ liệu
                return _announcementsDAL.GetTopRecentAnnouncements();
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý thêm nếu cần
                throw new Exception("Lỗi BUS GetRecentAnnouncements: " + ex.Message);
            }
        }

        /// <summary>
        /// Lấy toàn bộ thông báo hệ thống
        /// </summary>
        public List<AnnouncementsDTO> GetAllAnnouncements()
        {
            return _announcementsDAL.GetAllAnnouncements();
        }
    }
}