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
        private readonly AnnouncementsBUS _announcementBUS = new AnnouncementsBUS();
        public ucTeacherNotification()
        {
            InitializeComponent();
        }

        public void LoadData(string filterType = "All", string searchText = "")
        {
            flpNotifications.Controls.Clear();

            if (SessionManager.CurrentUser == null) return;

            // Lấy thông báo đúng đối tượng kèm trạng thái đã đọc của giảng viên hiện tại
            List<AnnouncementsDTO> list = _announcementBUS
                .GetNotificationsForInstructor(SessionManager.CurrentUser.UserID);

            if (list == null) return;

            int currentUserId = SessionManager.CurrentUser.UserID;
            string kw = searchText?.Trim().ToLower() ?? "";

            foreach (var item in list)
            {
                // Lọc đối tượng: All, Teacher, hoặc gửi cá nhân đúng người
                bool isTarget = item.TargetType == "All"
                             || item.TargetType == "Teacher"
                             || (item.TargetType == "Individual" && item.ReceiverID == currentUserId);

                // Lọc tìm kiếm (null-safe)
                bool matchesSearch = string.IsNullOrEmpty(kw)
                    || (item.Title?.ToLower().Contains(kw) == true)
                    || (item.Content?.ToLower().Contains(kw) == true);

                // Lọc tab
                bool matchesTab = filterType switch
                {
                    "Urgent" => item.PriorityLevel == "Urgent",
                    "Unread" => !item.IsRead,
                    _        => true
                };

                if (!isTarget || !matchesSearch || !matchesTab) continue;

                var ucItem = new ucStudentNotificationItem();
                ucItem.SetData(item);
                ucItem.Width = flpNotifications.Width - 25;
                // Khi user đánh dấu đã đọc → reload để cập nhật badge số thông báo
                ucItem.ReadStateChanged += (s, e) => LoadData(filterType, searchText);
                flpNotifications.Controls.Add(ucItem);
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
            LoadData("Unread", txtSearch.Text);
        }

        private void btnFilterAll_Click(object sender, EventArgs e)
        {
            LoadData("All", txtSearch.Text);
        }
    }
}
