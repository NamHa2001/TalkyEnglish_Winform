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

namespace TalkyEnglish.GUI
{
    public partial class ucCourseManagement : UserControl
    {
        private readonly CourseBUS _courseBUS = new CourseBUS();
        public ucCourseManagement()
        {
            InitializeComponent();
        }
        // Hàm này để nạp dữ liệu lên bảng và cập nhật số lượng
        public void LoadData()
        {
            try
            {
                // 1. Lấy danh sách từ BUS
                var courses = _courseBUS.GetAllCourses();

                // 2. Đổ vào DataGridView
                dgvCourses.DataSource = courses;

                // 3. Cập nhật nhãn tổng số khóa học (lblTotalCourses)
                lblTotalCourses.Text = $"Tổng số khóa học: {courses.Count}";

                // 4. Định dạng lại cột Học phí (Price) cho đẹp nếu cần
                dgvCourses.Columns["Price"].DefaultCellStyle.Format = "N0"; // Hiện 2.500.000 thay vì 2500000
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        // Sự kiện khi User Control vừa được load lên


        private void txtSearchCourse_TextChanged(object sender, EventArgs e)
        {
            dgvCourses.DataSource = _courseBUS.SearchCourses(txtSearchCourse.Text);
        }

        private void ucCourseManagement_Load_1(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAddCourse_Click(object sender, EventArgs e)
        {
            // 1. Khởi tạo Form chi tiết (Dùng Constructor không tham số - Thêm mới)
            frmCourseDetail frm = new frmCourseDetail();

            // 2. Mở Form dưới dạng Dialog (buộc người dùng xử lý xong mới quay lại được)
            // Sau khi Form đóng, nó sẽ trả về kết quả trong dr
            DialogResult dr = frm.ShowDialog();

            // 3. Nếu người dùng nhấn "Lưu" thành công (DialogResult.OK)
            if (dr == DialogResult.OK)
            {
                // Gọi lại hàm LoadData để cập nhật bảng ngay lập tức
                LoadData();
            }
        }
    }
}

