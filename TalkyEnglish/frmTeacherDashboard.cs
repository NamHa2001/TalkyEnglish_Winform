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
using System.Drawing;

namespace TalkyEnglish.GUI
{
    public partial class frmTeacherDashboard : Form
    {
        UserBUS _userBUS = new UserBUS();
        AnnouncementsBUS _announcementBUS = new AnnouncementsBUS();
        public frmTeacherDashboard()
        {
            InitializeComponent();
        }
        // Chú thích: Hàm nạp ảnh đại diện từ thư mục Avatars dựa trên ID giảng viên đang đăng nhập
        public void LoadAvatar()
        {
            if (SessionManager.CurrentUser != null)
            {
                string folderPath = System.IO.Path.Combine(Application.StartupPath, "Avatars");
                string avatarPath = System.IO.Path.Combine(folderPath, $"Avatar_{SessionManager.CurrentUser.UserID}.jpg");

                if (System.IO.File.Exists(avatarPath))
                {
                    // Sử dụng FileStream để đọc ảnh mà không khóa file, giúp có thể thay đổi ảnh sau đó
                    using (System.IO.FileStream fs = new System.IO.FileStream(avatarPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        // Lưu ý: Thay 'picSmallAvatar' bằng đúng tên PictureBox nhỏ trên thanh sidebar của bạn
                        picSmallAvatar.Image = Image.FromStream(fs);
                    }
                }
            }
        }

        // Chú thích: Hàm công khai để các UserControl (như Profile) có thể yêu cầu Form chính nạp lại ảnh
        public void RefreshSmallAvatar()
        {
            LoadAvatar();
        }
        private void pnlMainContent1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmTeacherDashboard_Load(object sender, EventArgs e)
        {
            LoadAnnouncementsToTarget(flpAnnouncements);
            dgvScheduleMini.AutoGenerateColumns = false;
            LoadTodaySchedule();
            // Kiểm tra Session trước khi nạp dữ liệu
            if (SessionManager.CurrentUser != null)
            {
                // Đổ dữ liệu từ Session vào Labels (Theo image_e97e42.png)
                lblFullName.Text = "Xin chào, " + SessionManager.CurrentUser.FullName;
                lblMaGV.Text = "Mã GV: " + SessionManager.CurrentUser.UserID.ToString();
                lblEmail.Text = "Email: " + SessionManager.CurrentUser.Email;
                LoadAvatar();

                // Nạp các con số cho 4 thẻ màu
                LoadDashboardStatistics();
            }
        }

        private void LoadDashboardStatistics()
        {
            try
            {
                // Lấy tổng số học viên từ hàm có sẵn trong DAL/BUS bro gửi
                int totalStudents = _userBUS.GetTotalStudents();


                // Giả sử bro muốn hiện tổng số giảng viên hoặc thông báo mới
                int totalInstructors = _userBUS.GetTotalInstructors();
                lblCountClass.Text = totalInstructors.ToString() + " đồng nghiệp"; // Ví dụ vậy kkk

                // Thẻ thông báo mới (Lấy số lượng tin chưa đọc hoặc tổng tin)
                var listNotify = _announcementBUS.GetAllAnnouncements();
                lblCountNotify.Text = (listNotify?.Count ?? 0).ToString() + " thông báo";
            }
            catch (Exception ex)
            {
                // Tránh app bị crash nếu DB có vấn đề
                Console.WriteLine("Lỗi load thống kê: " + ex.Message);
            }
        }

        private void AddControlToBody(UserControl uc)
        {
            pnlMainContent1.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlMainContent1.Controls.Add(uc);
            uc.BringToFront();
        }
        private void btnNotification_Click(object sender, EventArgs e)
        {
            ucTeacherNotification uc = new ucTeacherNotification();
            AddControlToBody(uc);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SessionManager.Clear(); // Xóa sạch dấu vết

                frmDangNhap loginForm = new frmDangNhap();
                loginForm.Show();

                this.Hide(); // Ẩn dashboard đi
            }
        }

        private void btnTrangchu_Click(object sender, EventArgs e)
        {
            pnlMainContent1.Controls.Clear();
            LoadDashboardStatistics();
        }

        private void btnlichoc_Click(object sender, EventArgs e)
        {
            ucTeacherSchedule uc = new ucTeacherSchedule();
            AddControlToBody(uc);
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            ucTeacherProfile uc = new ucTeacherProfile();
            AddControlToBody(uc);
        }

        private void btnRegisterCourse_Click(object sender, EventArgs e)
        {
            ucDSSV uc = new ucDSSV();
            AddControlToBody(uc);
        }


        private void LoadTodaySchedule()
        {
            ScheduleBUS _bus = new ScheduleBUS();
            int currentID = SessionManager.CurrentUser.UserID;

            // Lấy thứ hiện tại bằng tiếng Việt (tận dụng logic switch-case bro đã viết)
            string dayEng = DateTime.Today.DayOfWeek.ToString();
            string dayVie = GetVietnameseDay(dayEng); // Tách cái switch case của bro ra 1 hàm riêng cho gọn

            var allSchedules = _bus.GetSchedulesByTeacher(currentID);

            // LỌC CHỈ LẤY LỊCH HÔM NAY
            var todaySchedules = allSchedules.Where(s => s.DayOfWeek == dayVie).ToList();

            dgvScheduleMini.DataSource = todaySchedules;
        }

        // Hàm hỗ trợ chuyển đổi (copy từ switch case của bro)
        private string GetVietnameseDay(string dayEng)
        {
            switch (dayEng)
            {
                case "Monday": return "Thứ Hai";
                case "Tuesday": return "Thứ Ba";
                case "Wednesday": return "Thứ Tư";
                case "Thursday": return "Thứ Năm";
                case "Friday": return "Thứ Sáu";
                case "Saturday": return "Thứ Bảy";
                default: return "Chủ Nhật";
            }
        }


        private void LoadAnnouncementsToTarget(FlowLayoutPanel targetPanel)
        {
            try
            {
                // 1. Dọn dẹp vùng chứa mục tiêu
                targetPanel.Controls.Clear();

                // 2. Lấy dữ liệu mới nhất từ BUS
                AnnouncementsBUS bus = new AnnouncementsBUS();
                var list = bus.GetRecentAnnouncements();

                // 3. Đổ dữ liệu vào các thẻ UserControl
                foreach (var item in list)
                {
                    // Tạo mới cái thẻ "thông báo" bro đã làm riêng
                    ucAnnounceItem ucItem = new ucAnnounceItem();

                    // Nạp dữ liệu cho thẻ đó
                    ucItem.SetData(item);

                    // Thêm vào Panel mục tiêu (Ví dụ Panel ở Dashboard giảng viên)
                    targetPanel.Controls.Add(ucItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể nạp thông báo: " + ex.Message);
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void btnGrades_Click(object sender, EventArgs e)
        {
            ucInstructorGrading uc = new ucInstructorGrading();
            AddControlToBody(uc);
        }
    }
}
