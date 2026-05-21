using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;
using Guna.UI2.WinForms;

namespace TalkyEnglish.GUI
{
    public partial class ucStudentNotification : UserControl
    {
        private readonly AnnouncementsBUS _bus = new AnnouncementsBUS();
        private string _currentFilter = "All";
        private List<AnnouncementsDTO> _allItems = new List<AnnouncementsDTO>();

        public ucStudentNotification()
        {
            InitializeComponent();
        }

        // ── Tải dữ liệu từ DB và render theo filter + search ──
        public void LoadData(string filterType = "All", string searchText = "")
        {
            _currentFilter = filterType;

            try
            {
                // Chỉ tải lại từ DB khi search rỗng và filter thay đổi
                // (hoặc lần đầu tiên)
                if (_allItems.Count == 0 || string.IsNullOrEmpty(searchText))
                    RefreshFromDB();

                RenderItems(filterType, searchText);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải thông báo: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshFromDB()
        {
            if (SessionManager.CurrentUser == null)
            {
                _allItems = new List<AnnouncementsDTO>();
                return;
            }
            _allItems = _bus.GetNotificationsForStudent(SessionManager.CurrentUser.UserID);
        }

        private void RenderItems(string filterType, string searchText)
        {
            flpNotifications.Controls.Clear();

            var filtered = _allItems.Where(item =>
            {
                // Lọc theo tab
                bool matchesTab = filterType switch
                {
                    "Urgent" => item.PriorityLevel == "Urgent",
                    "Unread" => !item.IsRead,
                    _        => true   // "All"
                };

                // Lọc theo ô tìm kiếm
                bool matchesSearch = string.IsNullOrEmpty(searchText)
                    || (item.Title?.ToLower().Contains(searchText.ToLower()) == true)
                    || (item.Content?.ToLower().Contains(searchText.ToLower()) == true);

                return matchesTab && matchesSearch;
            }).ToList();

            if (filtered.Count == 0)
            {
                flpNotifications.Controls.Add(new Label
                {
                    Text = "Không có thông báo nào.",
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10),
                    ForeColor = Color.Gray,
                    Margin = new Padding(10, 20, 0, 0)
                });
                return;
            }

            foreach (var item in filtered)
            {
                var ucItem = new ucStudentNotificationItem();
                ucItem.SetData(item);
                ucItem.Width = flpNotifications.Width - 25;

                // Khi học viên bấm "Đã đọc", cần reload (ảnh hưởng bộ đếm "Chưa đọc")
                ucItem.ReadStateChanged += (s, e) =>
                {
                    RefreshFromDB();
                    RenderItems(_currentFilter, txtSearch.Text);
                };

                flpNotifications.Controls.Add(ucItem);
            }

            UpdateUnreadBadge();
        }

        private void UpdateUnreadBadge()
        {
            int unreadCount = _allItems.Count(x => !x.IsRead);
            btnunread.Text = unreadCount > 0
                ? $"Chưa đọc ({unreadCount})"
                : "Chưa đọc";
        }

        // ── Highlight tab đang active ──
        private void SetActiveButton(Guna2Button active)
        {
            foreach (var btn in new[] { btnFilterAll, btnunread, btnFilterUrgent })
            {
                btn.FillColor = Color.White;
                btn.ForeColor = Color.Black;
                btn.HoverState.FillColor = Color.FromArgb(240, 245, 255);
            }
            active.FillColor = Color.FromArgb(37, 99, 235);
            active.ForeColor = Color.White;
            active.HoverState.FillColor = Color.FromArgb(37, 99, 235);
        }

        // ── Event handlers ──
        private void ucStudentNotification_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
            SetActiveButton(btnFilterAll);
            LoadData("All");
        }

        private void btnFilterAll_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnFilterAll);
            LoadData("All", txtSearch.Text);
        }

        private void btnunread_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnunread);
            LoadData("Unread", txtSearch.Text);
        }

        private void btnFilterUrgent_Click(object sender, EventArgs e)
        {
            SetActiveButton(btnFilterUrgent);
            LoadData("Urgent", txtSearch.Text);
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {
            RenderItems(_currentFilter, txtSearch.Text);
        }
    }
}
