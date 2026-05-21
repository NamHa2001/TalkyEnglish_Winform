using System;
using System.Drawing;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucNotificationItem : UserControl
    {
        private AnnouncementsDTO _data;
        public event EventHandler DeleteRequested;

        public ucNotificationItem()
        {
            InitializeComponent();
            btnDelete.Click += btnDelete_Click;
            btnDetails.Click += btnDetails_Click;
        }

        public void SetData(AnnouncementsDTO data)
        {
            if (data == null) return;
            _data = data;

            lblTitle.Text = data.Title;
            lblTargetType.Text = GetTargetLabel(data.TargetType);
            lblDate.Text = data.PublishDate?.ToString("dd/MM/yyyy HH:mm");

            if (data.PriorityLevel == "Urgent")
            {
                lblTitle.ForeColor = Color.FromArgb(220, 38, 38);
                lblTitle.Font = new System.Drawing.Font(lblTitle.Font, FontStyle.Bold);
            }
            else
            {
                lblTitle.ForeColor = Color.FromArgb(15, 23, 42);
                lblTitle.Font = new System.Drawing.Font(lblTitle.Font, FontStyle.Regular);
            }

            btnDelete.Enabled = true;
            btnDetails.Enabled = true;
        }

        private string GetTargetLabel(string targetType) => targetType switch
        {
            "All"        => "Tất cả",
            "Teacher"    => "Giảng viên",
            "Student"    => "Học viên",
            "Individual" => "Cá nhân",
            _            => targetType ?? ""
        };

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (_data == null) return;

            var confirm = MessageBox.Show(
                $"Xóa thông báo \"{_data.Title}\"?\nHành động này không thể hoàn tác.",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                bool ok = new AnnouncementsBUS().DeleteAnnouncement(_data.AnnounceID);
                if (ok)
                    DeleteRequested?.Invoke(this, EventArgs.Empty);
                else
                    MessageBox.Show("Không thể xóa thông báo này.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (_data == null) return;

            string priority = _data.PriorityLevel == "Urgent" ? "Khẩn cấp" : "Bình thường";

            MessageBox.Show(
                $"Tiêu đề:      {_data.Title}\n\n" +
                $"Nội dung:\n{_data.Content}\n\n" +
                $"Ngày gửi:     {_data.PublishDate?.ToString("dd/MM/yyyy HH:mm")}\n" +
                $"Đối tượng:    {GetTargetLabel(_data.TargetType)}\n" +
                $"Danh mục:     {_data.Category}\n" +
                $"Độ ưu tiên:   {priority}",
                "Chi tiết thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ucNotificationItem_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void ucNotificationItem_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
        }

        private void ucNotificationItem_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
        }
    }
}
