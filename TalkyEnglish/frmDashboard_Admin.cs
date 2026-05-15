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
        // 1. KHAI BÁO BIẾN TẦNG BUS (Rất quan trọng)
        private readonly UserBUS _userBUS = new UserBUS();
        // Khai báo biến toàn cục trong Form chính
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

        // Hàm xử lý khi Form bắt đầu Load
        private void frmDashboard_Admin_Load(object sender, EventArgs e)
        {

            // 1. Hiển thị thông tin Admin từ Session (Giống bên trang Student)
            if (SessionManager.CurrentUser != null)
            {
                // Giả sử bro có các Label tên là lblAdminName, lblAdminCode, lblAdminEmail
                txtFullName.Text = SessionManager.CurrentUser.FullName; // Ô hiển thị "Xin chào..."
                lblAdminCode.Text = " ID: " + (SessionManager.CurrentUser.UserID.ToString()); // Hoặc StudentCode tùy DB
                lblEmail.Text = "Email: " + SessionManager.CurrentUser.Email;

                // [MÀU SẮC]: Áp dụng hệ màu Navy đậm #0F172A cho tiêu đề
                txtFullName.ForeColor = ColorTranslator.FromHtml("#0F172A");
                lblAdminCode.ForeColor = ColorTranslator.FromHtml("#2563EB"); // Xanh dương cho mã số
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

        /// <summary>
        /// Hàm đổ dữ liệu thật từ SQL vào các TextBox thống kê
        /// </summary>
        private void LoadThongKeConSo()
        {
            try
            {
                // Lấy số lượng thực tế (Thay tên hàm tương ứng với BUS của bro)
                int studentsCount = _userBUS.GetAllStudents().Count(); // Cần hàm GetAllStudents
                int instructorsCount = _userBUS.GetTopInstructors().Count();
                int coursesCount = new CourseBUS().GetAllCourses().Count(); // Gọi BUS khóa học

                // Đổ vào giao diện
                txtTotalStudents.Text = studentsCount.ToString();

                txtTotalCourses.Text = coursesCount.ToString();

                // Doanh thu: Giả sử lấy từ bảng Invoices hoặc một hàm BUS khác
                // txtRevenue.Text = string.Format("{0:N0} VNĐ", totalRevenue);

                // Chỉnh ReadOnly và Màu sắc cho con số
                txtTotalStudents.ForeColor = ColorTranslator.FromHtml("#2563EB");

            }
            catch (Exception ex)
            {
                // Xử lý lỗi im lặng để không hiện bảng thông báo liên tục khi load
                Console.WriteLine("Lỗi load thống kê: " + ex.Message);
            }
        }

        private void HienThiDuLieuBieuDo()
        {
            // --- BIỂU ĐỒ CỘT (DOANH THU THÁNG) ---
            gunaBarDataset1.DataPoints.Clear();
            // Số liệu dưới 5 triệu cho mỗi tháng
            gunaBarDataset1.DataPoints.Add("Tháng 3", 2100000);
            gunaBarDataset1.DataPoints.Add("Tháng 4", 3800000);
            gunaBarDataset1.DataPoints.Add("Tháng 5", 4250000);

            // Màu xanh dương chính (#2563EB)
            gunaBarDataset1.FillColors.Add(ColorTranslator.FromHtml("#2563EB"));
            gunaBarDataset1.Label = "Doanh thu (VNĐ)";

            // --- BIỂU ĐỒ TRÒN (TỶ LỆ TRÌNH ĐỘ KHÓA HỌC) ---
            chartStatistics.Datasets.Add(gunaBarDataset1);
            gunaPieDataset1.DataPoints.Clear();
            gunaPieDataset1.DataPoints.Add("Cơ bản", 50);
            gunaPieDataset1.DataPoints.Add("Trung cấp", 35);
            gunaPieDataset1.DataPoints.Add("Nâng cao", 15);

            // Phối màu theo hệ màu bro đã gửi (Xanh - Vàng - Tím)
            gunaPieDataset1.FillColors.Add(ColorTranslator.FromHtml("#2563EB")); // Xanh
            gunaPieDataset1.FillColors.Add(ColorTranslator.FromHtml("#FBBF24")); // Vàng
            gunaPieDataset1.FillColors.Add(ColorTranslator.FromHtml("#8B5CF6")); // Tím
            gunaPieDataset1.Label = "Phân loại khóa học";
            gunaChart2.Datasets.Add(gunaPieDataset1);

            // Cập nhật lên giao diện
            chartStatistics.Update();
            gunaChart2.Update();
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