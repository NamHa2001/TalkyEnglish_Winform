using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class frmTeacherDashboard : Form
    {
        private readonly UserBUS _userBUS = new UserBUS();
        private readonly AnnouncementsBUS _announcementBUS = new AnnouncementsBUS();

        public frmTeacherDashboard()
        {
            InitializeComponent();
            // "Xem tất cả" trong panel thông báo → mở UC thông báo
            guna2Button2.Click += (s, e) => AddControlToBody(new ucTeacherNotification());
        }

        private void frmTeacherDashboard_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);

            if (SessionManager.CurrentUser == null) return;

            lblFullName.Text = "Xin chào, " + SessionManager.CurrentUser.FullName;
            lblMaGV.Text     = "Mã GV: "    + SessionManager.CurrentUser.UserID;
            lblEmail.Text    = "Email: "    + SessionManager.CurrentUser.Email;
            LoadAvatar();

            ShowHomeView();
        }

        // Khôi phục và tải lại toàn bộ nội dung trang chủ
        private void ShowHomeView()
        {
            pnlMainContent1.Controls.Clear();
            pnlMainContent1.Controls.Add(guna2Panel3);
            pnlMainContent1.Controls.Add(pnlMainContent);

            dgvScheduleMini.AutoGenerateColumns = false;
            lblCurrentDate.Text = DateTime.Today.ToString("dd/MM/yyyy");

            LoadDashboardStatistics();
            LoadTodaySchedule();
            LoadAnnouncementsToTarget(flpAnnouncements);
        }

        private void LoadDashboardStatistics()
        {
            if (SessionManager.CurrentUser == null) return;
            int instructorId = SessionManager.CurrentUser.UserID;

            try
            {
                // Card hồng: số lớp đang dạy
                int classCount = new CourseBUS().GetCoursesByInstructor(instructorId).Count;
                label8.Text = classCount.ToString();

                // Card xanh lá: tổng học viên
                label7.Text = _userBUS.GetTotalStudents().ToString();

                // Card xanh dương: thông báo chưa đọc của giảng viên này
                var notifications = _announcementBUS.GetNotificationsForInstructor(instructorId);
                label6.Text = notifications.Count(n => !n.IsRead).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load thống kê: " + ex.Message);
            }
        }

        private void LoadTodaySchedule()
        {
            if (SessionManager.CurrentUser == null) return;

            try
            {
                var bus          = new ScheduleBUS();
                string dayVie    = GetVietnameseDay(DateTime.Today.DayOfWeek.ToString());
                var allSchedules = bus.GetSchedulesByTeacher(SessionManager.CurrentUser.UserID);
                var today        = allSchedules.Where(s => s.DayOfWeek == dayVie).ToList();

                dgvScheduleMini.DataSource = today;

                // Card vàng: số tiết hôm nay
                label5.Text = today.Count.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load lịch dạy: " + ex.Message);
            }
        }

        private void LoadAnnouncementsToTarget(FlowLayoutPanel targetPanel)
        {
            try
            {
                targetPanel.Controls.Clear();
                var list = _announcementBUS.GetRecentAnnouncements();
                foreach (var item in list)
                {
                    var ucItem = new ucAnnounceItem();
                    ucItem.SetData(item);
                    targetPanel.Controls.Add(ucItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể nạp thông báo: " + ex.Message);
            }
        }

        // Hiển thị một UserControl bất kỳ ở vùng nội dung chính
        private void AddControlToBody(UserControl uc)
        {
            pnlMainContent1.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            pnlMainContent1.Controls.Add(uc);
            uc.BringToFront();
        }

        public void LoadAvatar()
        {
            if (SessionManager.CurrentUser == null) return;

            string folderPath = System.IO.Path.Combine(Application.StartupPath, "Avatars");
            string avatarPath = System.IO.Path.Combine(folderPath, $"Avatar_{SessionManager.CurrentUser.UserID}.jpg");

            if (!System.IO.File.Exists(avatarPath)) return;

            using (var fs = new System.IO.FileStream(avatarPath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                picSmallAvatar.Image = Image.FromStream(fs);
        }

        public void RefreshSmallAvatar() => LoadAvatar();

        private string GetVietnameseDay(string dayEng) => dayEng switch
        {
            "Monday"    => "Thứ Hai",
            "Tuesday"   => "Thứ Ba",
            "Wednesday" => "Thứ Tư",
            "Thursday"  => "Thứ Năm",
            "Friday"    => "Thứ Sáu",
            "Saturday"  => "Thứ Bảy",
            _           => "Chủ Nhật"
        };

        // ── Sidebar navigation ──
        private void btnTrangchu_Click(object sender, EventArgs e)       => ShowHomeView();
        private void btnlichoc_Click(object sender, EventArgs e)         => AddControlToBody(new ucTeacherSchedule());
        private void btnProfile_Click(object sender, EventArgs e)        => AddControlToBody(new ucTeacherProfile());
        private void btnRegisterCourse_Click(object sender, EventArgs e) => AddControlToBody(new ucDSSV());
        private void btnGrades_Click(object sender, EventArgs e)         => AddControlToBody(new ucInstructorGrading());
        private void btnNotification_Click(object sender, EventArgs e)   => AddControlToBody(new ucTeacherNotification());

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            SessionManager.Clear();
            new frmDangNhap().Show();
            this.Close();
        }

        private void pnlMainContent1_Paint(object sender, PaintEventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
    }
}
