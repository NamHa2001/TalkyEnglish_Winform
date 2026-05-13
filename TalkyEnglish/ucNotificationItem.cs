using Microsoft.Office.Interop.Excel;
using System;
using System.Drawing;
using System.Windows.Forms;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucNotificationItem : UserControl
    {
        public ucNotificationItem()
        {
            InitializeComponent();
        }

        // Hàm "thần thánh" để nạp dữ liệu từ DTO vào các Label
        public void SetData(AnnouncementsDTO data)
        {
            if (data == null) return;

            // 1. Đổ dữ liệu chữ
            lblTitle.Text = data.Title;
            lblTargetType.Text = data.TargetType;

            // Format ngày tháng cho đẹp: ví dụ 14/05/2026 00:15
            lblDate.Text = data.PublishDate?.ToString("dd/MM/yyyy HH:mm");

            // 2. Xử lý màu sắc theo độ ưu tiên (PriorityLevel)
            // Màu Heritage Gold của bro: #C5A059
            Color heritageGold = ColorTranslator.FromHtml("#C5A059");

            if (data.PriorityLevel == "Urgent")
            {
     
                lblTitle.ForeColor = Color.Red;
                // Chỉ định rõ là System.Drawing.Font để hết bị tranh chấp
                lblTitle.Font = new System.Drawing.Font(lblTitle.Font, FontStyle.Bold);
            }
            else
            {
           
                lblTitle.ForeColor = Color.Black;
                // Tương tự cho dòng này luôn bro nha
                lblTitle.Font = new System.Drawing.Font(lblTitle.Font, FontStyle.Regular);
            }
        }

        // Thêm hiệu ứng hover cho chuyên nghiệp nè bro
        private void ucNotificationItem_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(245, 245, 245); // Xám nhẹ khi di chuột vào
        }

        private void ucNotificationItem_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = Color.White; // Trở lại trắng khi rời đi
        }
    }
}