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

        public frmDashboard_Admin()
        {
            InitializeComponent();

            // Kết nối Dataset với biểu đồ
            chartStatistics.Datasets.Add(gunaBarDataset1);
            gunaChart2.Datasets.Add(gunaPieDataset1);

            // Gọi hàm hiển thị biểu đồ
            HienThiDuLieuBieuDo();
        }

        // Hàm xử lý khi Form bắt đầu Load
        private void frmDashboard_Admin_Load(object sender, EventArgs e)
        {
            LoadThongKeConSo();
        }

        /// <summary>
        /// Hàm đổ dữ liệu thật từ SQL vào các TextBox thống kê
        /// </summary>
        private void LoadThongKeConSo()
        {
            try
            {
                // Lấy dữ liệu từ tầng BUS
                int totalStudents = _userBUS.GetTopInstructors().Count(); // Đây là ví dụ, BUS của bạn đã có hàm GetTopInstructors

                // Đổ vào TextBox (Dùng đúng tên chúng ta đã đặt ở bước trước)
                // Lưu ý: Thầy giả định bạn đã có các hàm tương ứng trong UserBUS

                txtTotalStudents.Text = _userBUS.GetTopInstructors().Count().ToString(); // Giả lập lấy số lượng
                txtTotalInstructors.Text = _userBUS.GetTopInstructors().Count().ToString();

                // Các phần chưa có bảng trong DB thì để mặc định hoặc số 0
                txtTotalCourses.Text = "0";
                txtRevenue.Text = "0 VNĐ";

                // Chỉnh ReadOnly để người dùng không sửa được con số
                txtTotalStudents.ReadOnly = true;
                txtTotalInstructors.ReadOnly = true;
                txtTotalCourses.ReadOnly = true;
                txtRevenue.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu thống kê: " + ex.Message);
            }
        }

        private void HienThiDuLieuBieuDo()
        {
            // Cấu hình dữ liệu cho biểu đồ Cột (Doanh thu)
            gunaBarDataset1.DataPoints.Clear();
            gunaBarDataset1.DataPoints.Add("Tháng 1", 100);
            gunaBarDataset1.DataPoints.Add("Tháng 2", 150);
            gunaBarDataset1.DataPoints.Add("Tháng 3", 120);
            gunaBarDataset1.FillColors.Add(Color.FromArgb(197, 160, 89)); // Màu Heritage Gold

            // Cấu hình dữ liệu cho biểu đồ Tròn
            gunaPieDataset1.DataPoints.Clear();
            gunaPieDataset1.DataPoints.Add("Học viên", 70);
            gunaPieDataset1.DataPoints.Add("Giảng viên", 30);

            gunaPieDataset1.FillColors.Add(Color.FromArgb(197, 160, 89));
            gunaPieDataset1.FillColors.Add(Color.FromArgb(45, 52, 54));

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
    }
}