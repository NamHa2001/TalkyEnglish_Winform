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
    public partial class ucAdminNotificationManager : UserControl
    {
        public ucAdminNotificationManager()
        {
            InitializeComponent();
        }

        // Hàm này để đổ dữ liệu lên danh sách bên phải giao diện Admin
       

        private void btnSend_Click(object sender, EventArgs e)
        {
            // 1. Thu thập dữ liệu
            string title = txtTitle.Text.Trim();
            string content = rtbContent.Text.Trim();
            string category = cboCategory.Text;
            string targetType = cboTargetType.Text;
            string priority = rdbPriorityUrgent.Checked ? "Urgent" : "Normal";

            // 2. Kiểm tra nhập liệu
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề và nội dung!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 3. Đóng gói vào DTO
                AnnouncementsDTO newAnnounce = new AnnouncementsDTO
                {
                    Title = title,
                    Content = content,
                    Category = category,
                    TargetType = targetType,
                    PriorityLevel = priority,
                    PublishDate = DateTime.Now,
                    SenderID = SessionManager.CurrentUser.UserID // Lấy ID Admin đang đăng nhập
                };

                // Nếu gửi cá nhân, ta cần lấy ID người nhận (tạm thời giả định bro lấy từ cbo hoặc biến chọn)
                if (targetType == "Individual")
                {
                    // Đoạn này bro thay bằng biến lưu ID người nhận mà bro chọn trên UI nhé
                    // newAnnounce.ReceiverID = selectedReceiverID; 
                }

                // 4. Triệu hồi BUS xử lý
                AnnouncementsBUS bus = new AnnouncementsBUS();
                bool isSuccess = bus.SendNotification(newAnnounce);

                if (isSuccess)
                {
                    MessageBox.Show("Gửi thông báo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 1. Làm sạch Form soạn thảo
                    txtTitle.Clear();
                    rtbContent.Clear();
                    rdbPriorityNormal.Checked = true;

                    // 2. CẬP NHẬT DANH SÁCH BÊN PHẢI NGAY LẬP TỨC
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Đặt hàm này trong class ucAdminNotificationManager
        public void LoadData()
        {
            try
            {
                // 1. Kiểm tra xem FlowLayoutPanel có tồn tại không
                // Giả sử cái khung chứa danh sách của bro tên là flpNotifications
                if (flpNotifications == null) return;

                // 2. Xóa các item cũ để không bị trùng lặp khi bấm nút nhiều lần
                flpNotifications.Controls.Clear();

                // 3. Gọi BUS để lấy danh sách từ Database
                AnnouncementsBUS bus = new AnnouncementsBUS();
                var list = bus.GetAllAnnouncements(); // Hàm này anh em mình đã viết ở file BUS rồi

                // 4. Đổ dữ liệu vào giao diện
                foreach (var item in list)
                {
                    // Tạo một bản sao của cái item nhỏ
                    ucNotificationItem ucItem = new ucNotificationItem();

                    // Đẩy dữ liệu vào (Hàm SetData anh em mình vừa sửa lỗi Font xong)
                    ucItem.SetData(item);

                    // Cấu hình cho item tràn hết chiều ngang của FlowLayoutPanel
                    ucItem.Width = flpNotifications.Width - 25;

                    // Thêm vào khung hiển thị
                    flpNotifications.Controls.Add(ucItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi LoadData: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ucAdminNotificationManager_Load(object sender, EventArgs e)
        {
            cboTargetType.SelectedIndex = 0; // Tự chọn "All"
            cboCategory.SelectedIndex = 0;   // Tự chọn "Hệ thống"
            LoadData();
        }
    }
}