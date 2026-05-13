
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucStudentNotificationItem : UserControl
    {
        public ucStudentNotificationItem()
        {
            InitializeComponent();
        }

        public void SetData(AnnouncementsDTO data)
        {
            if (data == null) return;

            // 1. Đổ dữ liệu chữ
            lblTitle.Text = data.Title;
            if (data.Content != null && data.Content.Length > 100)
                lblContent.Text = data.Content.Substring(0, 100) + "...";
            else
                lblContent.Text = data.Content;
            lblSender.Text = "Người gửi: Admin"; // Sau này có thể lấy tên cụ thể từ data.SenderID
            lblDate.Text = data.PublishDate?.ToString("dd/MM/yyyy HH:mm");

            // 2. Xử lý màu sắc và định dạng theo độ ưu tiên
            Color heritageGold = ColorTranslator.FromHtml("#C5A059");

            if (data.PriorityLevel == "Urgent")
            {
                pnlPriority.BackColor = Color.Red;
                lblTitle.ForeColor = Color.Red;
                lblTitle.Font = new Font(lblTitle.Font, FontStyle.Bold);
            }
            else
            {
                pnlPriority.BackColor = heritageGold;
                lblTitle.ForeColor = Color.Black;
                lblTitle.Font = new Font(lblTitle.Font, FontStyle.Regular);
            }

            // 3. Logic mờ đi nếu tin đã đọc (giả sử bro truyền thêm trạng thái đã đọc vào DTO)
            // if (isRead) { this.BackColor = Color.FromArgb(240, 240, 240); }
        }

        // Hiệu ứng Hover cho sang xịn mịn
        private void ucStudentNotificationItem_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void ucStudentNotificationItem_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White;
        }

        private void btnMarkAsRead_Click(object sender, EventArgs e)
        {
            // 1. Đổi màu nền sang xám nhạt
            this.BackColor = Color.FromArgb(235, 235, 235);

            // 2. Ẩn nút "Đã đọc" đi vì đã đọc rồi
            btnMarkAsRead.Visible = false;

            // 3. Đổi màu chữ tiêu đề về bình thường (hết đỏ/đậm)
            lblTitle.ForeColor = Color.Gray;
            lblTitle.Font = new Font(lblTitle.Font, FontStyle.Regular);

            // (Optional) Gọi BUS để Update Database cột IsRead = 1 ở đây nhé bro
        }
    }
}

