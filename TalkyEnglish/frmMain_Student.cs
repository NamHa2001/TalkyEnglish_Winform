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
    public partial class frmMain_Student : Form
    {
        private readonly AnnouncementsBUS _announcementsBUS = new AnnouncementsBUS();
        public frmMain_Student()
        {
            InitializeComponent();
            SetCurrentDate();
            ApplyModernInterface();
        }


        private void LoadMyCourses()
        {
            flpCourses.Controls.Clear();

            // 1. Khóa học thứ nhất - Màu xanh dương #2563EB
            ucCourseProgress c1 = new ucCourseProgress();
            c1.SetData("English Communication A2", "Nguyễn Thị Thu Hà", 70, ColorTranslator.FromHtml("#2563EB"));
            flpCourses.Controls.Add(c1);

            // 2. Khóa học thứ hai - Màu vàng chính #FBBF24
            ucCourseProgress c2 = new ucCourseProgress();
            c2.SetData("IELTS Foundation", "Trần Minh Hoàng", 45, ColorTranslator.FromHtml("#FBBF24"));
            flpCourses.Controls.Add(c2);

            // 3. Khóa học thứ ba - Màu tím #8B5CF6
            ucCourseProgress c3 = new ucCourseProgress();
            c3.SetData("Grammar for Beginners", "Lê Quang Huy", 20, ColorTranslator.FromHtml("#8B5CF6"));
            flpCourses.Controls.Add(c3);
        }
        private void addUserControl(UserControl uc)
        {
            // Chú thích báo cáo: Hàm điều hướng giúp thay đổi nội dung vùng làm việc chính (pnlMainContent1)
            uc.Dock = DockStyle.Fill; // Ép UC tràn đầy Panel
            pnlMainContent1.Controls.Clear(); // Xóa trang cũ (ví dụ đang ở Trang chủ)
            pnlMainContent1.Controls.Add(uc); // Thêm trang mới (ví dụ trang Lịch học)
            uc.BringToFront();
        }

        // Hàm này đặt tại frmMain_Student.cs
        public void RefreshSmallAvatar()
        {
            if (SessionManager.CurrentUser != null)
            {
                string folderPath = Path.Combine(Application.StartupPath, "Avatars");
                string avatarPath = Path.Combine(folderPath, $"Avatar_{SessionManager.CurrentUser.UserID}.jpg");

                if (File.Exists(avatarPath))
                {
                    // Giải phóng ảnh cũ để không bị khóa file
                    if (picSmallAvatar.Image != null) picSmallAvatar.Image.Dispose();

                    using (FileStream fs = new FileStream(avatarPath, FileMode.Open, FileAccess.Read))
                    {
                        picSmallAvatar.Image = Image.FromStream(fs);
                    }
                }
            }
        }
        private void frmMain_Student_Load(object sender, EventArgs e)
        {
            // 1. Hiển thị thông tin học viên từ Session
            if (SessionManager.CurrentUser != null)
            {
                // Cập nhật Họ tên học viên
                txtFullName.Text = SessionManager.CurrentUser.FullName;

                // Tối ưu hiển thị Mã học viên
                lblStudentCode.Text = "Mã HV: " + (SessionManager.CurrentUser.StudentCode ?? "N/A");
                lblStudentCode.ForeColor = ColorTranslator.FromHtml("#2563EB");
                lblStudentCode.Font = new Font("Segoe UI", 9F, FontStyle.Bold); // Chỉnh font chữ đậm cho chuyên nghiệp
                lblStudentCode.BackColor = Color.Transparent; // Trả về nền trong suốt cho đẹp
                lblStudentCode.BringToFront();

                // Tối ưu hiển thị Email
                lblEmail.Text = "Email: " + (SessionManager.CurrentUser.Email ?? "Chưa cập nhật");
                lblEmail.ForeColor = ColorTranslator.FromHtml("#64748B"); // Màu xám nhẹ nhàng, tinh tế
                lblEmail.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
                lblEmail.BackColor = Color.Transparent;
                lblEmail.BringToFront();
            }

            // 2. Load các thành phần khác trên Dashboard
            LoadAnnouncements();
            LoadLichHocHomNay();
            LoadMyCourses();
        }

        private void LoadLichHocHomNay()
        {
            // 1. Tạo cấu trúc bảng
            DataTable dt = new DataTable();
            dt.Columns.Add("StartTime");
            dt.Columns.Add("EndTime");
            dt.Columns.Add("Subject");
            dt.Columns.Add("Teacher");
            dt.Columns.Add("Room");

            // 2. Thêm dữ liệu giả (Dựa theo ảnh image_ee393e.png)
            dt.Rows.Add("18:30", "20:00", "English Communication A2", "Nguyễn Thị Thu Hà", "301");
            dt.Rows.Add("20:15", "21:45", "IELTS Foundation", "Trần Minh Hoàng", "302");

            // 3. Đổ vào DataGridView
            dgvSchedule.DataSource = dt;

            // 4. Định dạng hiển thị (Tùy chỉnh để giống ảnh nhất)
            dgvSchedule.Columns["StartTime"].HeaderText = "Giờ bắt đầu";
            dgvSchedule.Columns["EndTime"].HeaderText = "Giờ kết thúc";
            dgvSchedule.Columns["Subject"].HeaderText = "Môn học";
            dgvSchedule.Columns["Teacher"].HeaderText = "Giáo viên";
            dgvSchedule.Columns["Room"].HeaderText = "Phòng học";

            // Căn giữa văn bản cho các cột
            dgvSchedule.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSchedule.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void SetCurrentDate()
        {
            DateTime now = DateTime.Now;
            // Định dạng: Thứ..., ngày/tháng/năm
            lblCurrentDate.Text = string.Format("{0}, {1}",
                GetVietnameseDayOfWeek(now.DayOfWeek),
                now.ToString("dd/MM/yyyy"));
        }

        private string GetVietnameseDayOfWeek(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return "Thứ 2";
                case DayOfWeek.Tuesday: return "Thứ 3";
                case DayOfWeek.Wednesday: return "Thứ 4";
                case DayOfWeek.Thursday: return "Thứ 5";
                case DayOfWeek.Friday: return "Thứ 6";
                case DayOfWeek.Saturday: return "Thứ 7";
                default: return "Chủ Nhật";
            }
        }

        private void LoadAnnouncements()
        {
            try
            {
                // Xóa các thông báo mẫu (nếu có) trong FlowLayoutPanel
                flpAnnouncements.Controls.Clear();

                // Lấy danh sách 5 thông báo mới nhất từ BUS
                var list = _announcementsBUS.GetRecentAnnouncements();

                foreach (var item in list)
                {
                    // Khởi tạo khuôn mẫu cho mỗi thông báo
                    ucAnnounceItem ucItem = new ucAnnounceItem();

                    // Nạp dữ liệu thực từ Database vào khuôn
                    ucItem.SetData(item);

                    // Thêm khuôn đã có dữ liệu vào vùng chứa trên giao diện
                    flpAnnouncements.Controls.Add(ucItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp thông báo: " + ex.Message);
            }
        }

        private void btnlichoc_Click(object sender, EventArgs e)
        {
            ucStudentSchedule uc = new ucStudentSchedule();
            addUserControl(uc);
        }

        private void btnGrades_Click(object sender, EventArgs e)
        {
            ucStudentGrades uc = new ucStudentGrades();
            addUserControl(uc);
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            ucProfile uc = new ucProfile();

            // Sử dụng hàm điều hướng chung để hiển thị lên vùng pnlMainContent1
            addUserControl(uc);
        }

        private void btnRegisterCourse_Click(object sender, EventArgs e)
        {
            ucRegisterCourse uc = new ucRegisterCourse();

            // 2. Sử dụng hàm điều hướng có sẵn của bạn để hiển thị lên vùng trung tâm
            // Chú thích: Hàm này sẽ tự động Clear trang cũ và Fill trang Đăng ký vào Panel
            addUserControl(uc);
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            ucStudentNotification uc = new ucStudentNotification();

            // 2. Sử dụng hàm điều hướng có sẵn của bạn để hiển thị lên vùng trung tâm
            // Chú thích: Hàm này sẽ tự động Clear trang cũ và Fill trang Đăng ký vào Panel
            addUserControl(uc);
        }
        private void ApplyModernInterface()
        {
            // --- 1. Thiết lập màu nền tổng thể cho Form (Màu #F1F5F9) ---
            this.BackColor = ColorTranslator.FromHtml("#F1F5F9");

            // --- 2. Màu chữ tiêu đề chào mừng (Màu Navy đậm #0F172A) ---
            label1.ForeColor = ColorTranslator.FromHtml("#0F172A"); // Giả sử label1 là chữ "Xin chào"
            txtFullName.ForeColor = ColorTranslator.FromHtml("#2563EB"); // Tên học viên màu xanh dương chính

            // --- 3. Tối ưu DataGridView Lịch học (Giống image_79365b.jpg) ---
            dgvSchedule.BackgroundColor = Color.White;
            dgvSchedule.BorderStyle = BorderStyle.None;

            // Màu Header của bảng (Xanh dương chính #2563EB)
            dgvSchedule.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#2563EB");
            dgvSchedule.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSchedule.EnableHeadersVisualStyles = false;

            // Màu các dòng dữ liệu
            dgvSchedule.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#EBF2FF"); // Xanh nhạt khi chọn
            dgvSchedule.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#2563EB");

            // --- 4. Thiết lập màu cho Sidebar/Menu (Nếu bro dùng Panel làm Menu bên trái) ---
            // Giả sử pnlSidebar là panel chứa các nút Trang chủ, Lịch học...

        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SessionManager.Clear(); // Xóa sạch dấu vết

                frmDangNhap loginForm = new frmDangNhap();
                loginForm.Show();

                this.Hide(); // Ẩn dashboard đi
            }
        }
    }
}
