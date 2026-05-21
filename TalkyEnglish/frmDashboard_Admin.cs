using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TalkyEnglish.BUS; // Khai báo để dùng tầng BUS
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class frmDashboard_Admin : Form
    {
        private readonly UserBUS _userBUS = new UserBUS();
        private readonly ReportBUS _reportBUS = new ReportBUS();
        private readonly CourseBUS _courseBUS = new CourseBUS();

        ucInstructorManagement _ucInstructor;
        ucCourseManagement _ucCourse;
        ucTeachingAssignment _ucTeaching;
        ucScheduleManagement _ucSchedule;
        ucAdminNotificationManager _ucAdminNotification;
        ucTuitionManagement _ucTuition;
        ucReportDashboard _ucReport;

        public frmDashboard_Admin()
        {
            InitializeComponent();

            //// Kết nối Dataset với biểu đồ
            //chartStatistics.Datasets.Add(gunaBarDataset1);
            //gunaChart2.Datasets.Add(gunaPieDataset1);

            //// Gọi hàm hiển thị biểu đồ
            //HienThiDuLieuBieuDo();
        }

        private void frmDashboard_Admin_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
            if (SessionManager.CurrentUser != null)
            {
                txtFullName.Text = SessionManager.CurrentUser.FullName;
                lblAdminCode.Text = " ID: " + (SessionManager.CurrentUser.UserID.ToString());
                lblEmail.Text = "Email: " + SessionManager.CurrentUser.Email;
                txtFullName.ForeColor = ColorTranslator.FromHtml("#0F172A");
                lblAdminCode.ForeColor = ColorTranslator.FromHtml("#2563EB");
            }
            LoadThongKeConSo();
            HienThiDuLieuBieuDo();
        }

        // Hàm dùng để nạp các UserControl vào vùng trống chính của Dashboard
        private void addUserControl(UserControl userControl)
        {
            // 1. Dọn dẹp: Xóa bỏ các Control đang hiển thị ở vùng chính (ví dụ cái biểu đồ cũ)
            pnlMainContent1.Controls.Clear();

            // 2. Thiết lập: Để trang mới này tràn đầy và vừa khít với Panel chứa nó
            userControl.Dock = DockStyle.Fill;

            // 3. Hiển thị: Đưa trang mới vào và đẩy lên trên cùng
            pnlMainContent1.Controls.Add(userControl);
            userControl.BringToFront();
        }

        private void ShowUserControl(UserControl uc)
        {
            // Giả sử cái Panel lớn ở giữa của bạn tên là 'pnlMain'
            pnlMainContent1.Controls.Clear(); // Xóa cái đang hiện (Học viên hoặc Dashboard cũ)
            uc.Dock = DockStyle.Fill; // Để UC tràn đầy cái Panel
            pnlMainContent1.Controls.Add(uc); // Thêm UC mới vào
        }

        private void LoadThongKeConSo()
        {
            try
            {
                int studentsCount = _userBUS.GetTotalStudents();
                int instructorsCount = _userBUS.GetTotalInstructors();
                int coursesCount = _courseBUS.GetTotalCourses();

                DateTime firstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                decimal monthlyRevenue = _reportBUS.GetRevenueByDate(firstDay, lastDay)
                                                    .Sum(r => r.SoTien);

                txtTotalStudents.Text = studentsCount.ToString();
                txtTotalCourses.Text = coursesCount.ToString();
                label8.Text = instructorsCount.ToString();
                txtRevenue.Text = string.Format("{0:N0} VNĐ", monthlyRevenue);

                txtTotalStudents.ForeColor = ColorTranslator.FromHtml("#2563EB");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi load thống kê: " + ex.Message);
            }
        }

        private void HienThiDuLieuBieuDo()
        {
            try
            {
                // --- BIỂU ĐỒ CỘT: Doanh thu 6 tháng gần nhất ---
                gunaBarDataset1.DataPoints.Clear();
                gunaBarDataset1.FillColors.Clear();

                for (int i = 5; i >= 0; i--)
                {
                    DateTime month = DateTime.Now.AddMonths(-i);
                    DateTime from = new DateTime(month.Year, month.Month, 1);
                    DateTime to = from.AddMonths(1).AddDays(-1);

                    decimal revenue = _reportBUS.GetRevenueByDate(from, to).Sum(r => r.SoTien);
                    gunaBarDataset1.DataPoints.Add("T" + month.Month, (double)revenue);
                }

                gunaBarDataset1.FillColors.Add(ColorTranslator.FromHtml("#2563EB"));
                gunaBarDataset1.Label = "Doanh thu (VNĐ)";
                chartStatistics.Datasets.Add(gunaBarDataset1);

                // --- BIỂU ĐỒ TRÒN: Phân bổ trình độ khóa học thực tế ---
                gunaPieDataset1.DataPoints.Clear();
                gunaPieDataset1.FillColors.Clear();

                var courses = _courseBUS.GetAllCourses();
                var levelGroups = courses
                    .GroupBy(c => string.IsNullOrWhiteSpace(c.Level) ? "Khác" : c.Level)
                    .Select(g => new { Level = g.Key, Count = g.Count() })
                    .OrderByDescending(g => g.Count)
                    .ToList();

                var levelColors = new[]
                {
                    ColorTranslator.FromHtml("#2563EB"),
                    ColorTranslator.FromHtml("#FBBF24"),
                    ColorTranslator.FromHtml("#8B5CF6"),
                    ColorTranslator.FromHtml("#10B981"),
                };

                for (int i = 0; i < levelGroups.Count; i++)
                {
                    gunaPieDataset1.DataPoints.Add(levelGroups[i].Level, levelGroups[i].Count);
                    gunaPieDataset1.FillColors.Add(levelColors[i % levelColors.Length]);
                }

                gunaPieDataset1.Label = "Phân loại khóa học";
                gunaChart2.Datasets.Add(gunaPieDataset1);

                chartStatistics.Update();
                gunaChart2.Update();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi hiển thị biểu đồ: " + ex.Message);
            }
        }

        // Sự kiện nút Đăng xuất
        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                frmDangNhap login = new frmDangNhap();
                login.Show();
                this.Close();
            }
        }

        private void btnMenuStudents_Click(object sender, EventArgs e)
        {
            ucStudentManagement ucStudent = new ucStudentManagement();

            // 2. Gọi hàm nạp để nó "nhảy" ra màn hình chính
            addUserControl(ucStudent);
        }

        private void btnMenuInstructors_Click(object sender, EventArgs e)
        {
            // Nếu UC chưa được tạo thì mới tạo mới
            if (_ucInstructor == null)
            {
                _ucInstructor = new ucInstructorManagement();
            }

            // Gọi hàm LoadData để chắc chắn dữ liệu luôn mới khi bấm vào
            _ucInstructor.LoadData();

            // Hiển thị nó lên vùng chính
            ShowUserControl(_ucInstructor);
        }

        private void btnMenuCourses_Click(object sender, EventArgs e)
        {
            // Nếu UC chưa được tạo thì mới tạo mới
            if (_ucCourse == null)
            {
                _ucCourse = new ucCourseManagement();
            }

            // Gọi hàm LoadData để chắc chắn dữ liệu luôn mới khi bấm vào
            _ucCourse.LoadData();

            // Hiển thị nó lên vùng chính
            ShowUserControl(_ucCourse);
            btnMenuCourses.BackColor = Color.FromArgb(197, 160, 89); // Màu Heritage Gold
        }

        private void btnMenuAssignment_Click(object sender, EventArgs e)
        {
            if (_ucTeaching == null)
            {
                _ucTeaching = new ucTeachingAssignment();
            }

            // Gọi hàm LoadData để chắc chắn dữ liệu luôn mới khi bấm vào
            _ucTeaching.LoadData();

            // Hiển thị nó lên vùng chính
            ShowUserControl(_ucTeaching);
            btnMenuCourses.BackColor = Color.FromArgb(197, 160, 89); // Màu Heritage Gold
        }

        private void btnSchedual_Click(object sender, EventArgs e)
        {
            if (_ucSchedule == null)
            {
                _ucSchedule = new ucScheduleManagement();
            }

            // Gọi hàm LoadData để chắc chắn dữ liệu luôn mới khi bấm vào
            _ucSchedule.LoadData();

            // Hiển thị nó lên vùng chính
            ShowUserControl(_ucSchedule);
            btnMenuCourses.BackColor = Color.FromArgb(197, 160, 89); // Màu Heritage Gold
        }

        private void btnNotification_Click(object sender, EventArgs e)
        {
            if (_ucAdminNotification == null)
            {
                _ucAdminNotification = new ucAdminNotificationManager();
            }

            // Gọi hàm LoadData để chắc chắn dữ liệu luôn mới khi bấm vào
            _ucAdminNotification.LoadData();

            // Hiển thị nó lên vùng chính
            ShowUserControl(_ucAdminNotification);
            btnMenuCourses.BackColor = Color.FromArgb(197, 160, 89);
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (_ucTuition == null)
            {
                _ucTuition = new ucTuitionManagement();
            }

            // Gọi hàm LoadData để chắc chắn dữ liệu luôn mới khi bấm vào
            _ucTuition.LoadData();

            // Hiển thị nó lên vùng chính
            ShowUserControl(_ucTuition);
            btnMenuCourses.BackColor = Color.FromArgb(197, 160, 89);
        }

        private void btnMenuReports_Click(object sender, EventArgs e)
        {
            if (_ucReport == null)
            {
                _ucReport = new ucReportDashboard();
            }

            // Gọi hàm LoadData để chắc chắn dữ liệu luôn mới khi bấm vào
            _ucReport.LoadData();

            // Hiển thị nó lên vùng chính
            ShowUserControl(_ucReport);
            btnMenuCourses.BackColor = Color.FromArgb(197, 160, 89);
        }

        private void btnMenuDashboard_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SessionManager.Clear(); // Xóa sạch dấu vết

                frmDangNhap loginForm = new frmDangNhap();
                loginForm.Show();

                this.Hide(); // Ẩn dashboard đi
            }
        }

        private void txtTotalStudents_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
