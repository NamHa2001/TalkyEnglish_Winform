using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucTeacherNotification : UserControl
    {
        AnnouncementsBUS _announcementBUS = new AnnouncementsBUS();
        public ucTeacherNotification()
        {
            InitializeComponent();
        }

        public void LoadData(string filterType = "All", string searchText = "")
        {
            // 1. Dọn dẹp danh sách cũ trong FlowLayoutPanel
            flpNotifications.Controls.Clear();

            // 2. Lấy danh sách toàn bộ thông báo từ Database
            List<AnnouncementsDTO> list = _announcementBUS.GetAllAnnouncements();

            if (list == null) return;

            foreach (var item in list)
            {
                // 3. LOGIC QUAN TRỌNG: Chỉ hiện thông báo cho Giảng viên hoặc Tất cả
                bool isTarget = (item.TargetType == "All" || item.TargetType == "Teacher");

                // 4. Kiểm tra Tìm kiếm (Tiêu đề hoặc Nội dung)
                bool matchesSearch = string.IsNullOrEmpty(searchText) ||
                                     item.Title.ToLower().Contains(searchText.ToLower()) ||
                                     item.Content.ToLower().Contains(searchText.ToLower());

                // 5. Kiểm tra Bộ lọc Tab (Tất cả / Khẩn cấp)
                bool matchesTab = false;
                if (filterType == "All")
                {
                    matchesTab = true;
                }
                else if (filterType == "Urgent")
                {
                    matchesTab = (item.PriorityLevel == "Urgent");
                }

                // 6. Đổ vào giao diện nếu thỏa mãn các điều kiện
                if (isTarget && matchesSearch && matchesTab)
                {
                    // Tận dụng lại UserControl thẻ thông báo đã thiết kế
                    ucStudentNotificationItem ucItem = new ucStudentNotificationItem();
                    ucItem.SetData(item);

                    // Căn chỉnh chiều rộng tự động theo khung chứa
                    ucItem.Width = flpNotifications.Width - 25;

                    flpNotifications.Controls.Add(ucItem);
                }
            }
        }

        private void ucTeacherNotification_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData("All", txtSearch.Text);
        }

        private void btnFilterUrgent_Click(object sender, EventArgs e)
        {
            LoadData("Urgent", txtSearch.Text);
        }

        private void btnunread_Click(object sender, EventArgs e)
        {

        }

        private void btnFilterAll_Click(object sender, EventArgs e)
        {
            LoadData("All", txtSearch.Text);
        }
    }
}
