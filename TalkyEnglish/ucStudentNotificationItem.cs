using System;
using System.Drawing;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucStudentNotificationItem : UserControl
    {
        private AnnouncementsDTO _data;

        // Parent lắng nghe để biết khi nào cần reload (ví dụ: filter "Chưa đọc")
        public event EventHandler ReadStateChanged;

        public ucStudentNotificationItem()
        {
            InitializeComponent();
            ButtonEffectHelper.RemoveGrayEffect(this);
            btnDetails.Click += btnDetails_Click;
        }

        public void SetData(AnnouncementsDTO data)
        {
            if (data == null) return;
            _data = data;

            lblTitle.Text = data.Title;
            lblContent.Text = data.Content != null && data.Content.Length > 120
                ? data.Content.Substring(0, 120) + "..."
                : data.Content;
            lblSender.Text = "Người gửi: Admin";
            lblDate.Text = data.PublishDate?.ToString("dd/MM/yyyy HH:mm");

            if (data.PriorityLevel == "Urgent")
            {
                pnlPriority.BackColor = Color.FromArgb(239, 68, 68);
                lblTitle.ForeColor = Color.FromArgb(220, 38, 38);
                lblTitle.Font = new Font(lblTitle.Font, FontStyle.Bold);
            }
            else
            {
                pnlPriority.BackColor = ColorTranslator.FromHtml("#C5A059");
                lblTitle.ForeColor = Color.FromArgb(15, 23, 42);
                lblTitle.Font = new Font(lblTitle.Font, FontStyle.Regular);
            }

            ApplyReadState(data.IsRead);
        }

        private void ApplyReadState(bool isRead)
        {
            if (isRead)
            {
                this.BackColor = Color.FromArgb(248, 248, 248);
                lblTitle.ForeColor = Color.Gray;
                lblTitle.Font = new Font(lblTitle.Font, FontStyle.Regular);
                lblContent.ForeColor = Color.Silver;
                btnMarkAsRead.Visible = false;
            }
            else
            {
                this.BackColor = Color.White;
                btnMarkAsRead.Visible = true;
            }
        }

        private void btnMarkAsRead_Click(object sender, EventArgs e)
        {
            if (_data == null || _data.IsRead) return;

            if (SessionManager.CurrentUser == null) return;

            try
            {
                bool ok = new AnnouncementsBUS().MarkAsRead(_data.AnnounceID, SessionManager.CurrentUser.UserID);
                if (ok)
                {
                    _data.IsRead = true;
                    ApplyReadState(true);
                    ReadStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật trạng thái: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (_data == null) return;

            string priority = _data.PriorityLevel == "Urgent" ? "Khẩn cấp" : "Bình thường";
            string status   = _data.IsRead ? "Đã đọc" : "Chưa đọc";
            string target   = _data.TargetType switch
            {
                "All"        => "Tất cả",
                "Student"    => "Học viên",
                "Individual" => "Cá nhân",
                _            => _data.TargetType ?? ""
            };

            MessageBox.Show(
                $"Tiêu đề:      {_data.Title}\n\n" +
                $"Nội dung:\n{_data.Content}\n\n" +
                $"Ngày gửi:     {_data.PublishDate?.ToString("dd/MM/yyyy HH:mm")}\n" +
                $"Danh mục:     {_data.Category}\n" +
                $"Đối tượng:    {target}\n" +
                $"Độ ưu tiên:   {priority}\n" +
                $"Trạng thái:   {status}",
                "Chi tiết thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ucStudentNotificationItem_MouseEnter(object sender, EventArgs e)
        {
            if (_data == null || !_data.IsRead)
                this.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void ucStudentNotificationItem_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = (_data != null && _data.IsRead)
                ? Color.FromArgb(248, 248, 248)
                : Color.White;
        }
    }
}
