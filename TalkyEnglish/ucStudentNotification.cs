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
    public partial class ucStudentNotification : UserControl
    {

        AnnouncementsBUS _announcementBUS = new AnnouncementsBUS();
        public ucStudentNotification()
        {
            InitializeComponent();
        }

        public void LoadData(string filterType = "All", string searchText = "")
        {
            // 1. Dọn dẹp danh sách cũ
            flpNotifications.Controls.Clear();

            // 2. Lấy dữ liệu từ BUS
            List<AnnouncementsDTO> list = _announcementBUS.GetAllAnnouncements();

            if (list == null) return;

            foreach (var item in list)
            {
                // 3. Kiểm tra Đối tượng (Chỉ hiện cho All hoặc Student)
                bool isTarget = (item.TargetType == "All" || item.TargetType == "Student");

                // 4. Kiểm tra Tìm kiếm (Tiêu đề hoặc Nội dung)
                bool matchesSearch = string.IsNullOrEmpty(searchText) ||
                                     item.Title.ToLower().Contains(searchText.ToLower()) ||
                                     item.Content.ToLower().Contains(searchText.ToLower());

                // 5. Kiểm tra Bộ lọc Tab (Tất cả / Chưa đọc / Khẩn cấp)
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
                    ucStudentNotificationItem ucItem = new ucStudentNotificationItem();
                    ucItem.SetData(item);

                    // Căn chỉnh chiều rộng cho vừa khung
                    ucItem.Width = flpNotifications.Width - 25;

                    flpNotifications.Controls.Add(ucItem);
                }
            }
        }

        private void ucStudentNotification_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            LoadData("All", txtSearch.Text);
        }

        private void btnFilterUrgent_Click(object sender, EventArgs e)
        {
            LoadData("Urgent");
        }

        private void btnFilterAll_Click(object sender, EventArgs e)
        {
            LoadData("All");
        }

        private void btnunread_Click(object sender, EventArgs e)
        {
            LoadData("Unread");
        }
    }
}
