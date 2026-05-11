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
    public partial class frmCourseDetail : Form
    {
        public readonly CourseBUS _courseBUS = new CourseBUS();
        // 1. Khai báo biến để chứa dữ liệu khóa học (nếu là trường hợp Sửa)
        private CourseDTO _currentCourse = null;
        public frmCourseDetail()
        {
            InitializeComponent();
            _currentCourse = null; // Xác nhận đây là thêm mới
            this.Text = "Thêm Khóa Học Mới";
            txtCourseCode.Text = "Hệ thống tự sinh";
        }
        public frmCourseDetail(CourseDTO course)
        {
            InitializeComponent();
            _currentCourse = course; // Lưu lại dữ liệu cũ
            this.Text = "Chỉnh Sửa Khóa Học";
        }

        private void LoadComboBoxData()
        {
            // 1. Đổ dữ liệu Trình độ (Cố định)
            cboLevel.Items.Clear();
            cboLevel.Items.AddRange(new string[] { "Cơ bản", "Trung cấp", "Nâng cao" });
            cboLevel.SelectedIndex = 0; // Mặc định chọn cái đầu tiên

            // 2. Đổ dữ liệu Trạng thái (Cố định)
            cboStatus.Items.Clear();
            cboStatus.Items.AddRange(new string[] { "Đang mở", "Tạm đóng", "Đã kết thúc" });
            cboStatus.SelectedIndex = 0;

            // 3. Đổ dữ liệu Giảng viên (Lấy từ BUS)
            try
            {
                var instructors = _courseBUS.GetInstructors();
                cboInstructor.DataSource = instructors;
                cboInstructor.DisplayMember = "FullName"; // Hiển thị tên người
                cboInstructor.ValueMember = "UserID";     // Giá trị ngầm là ID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách giảng viên: " + ex.Message);
            }
        }

        private void frmCourseDetail_Load(object sender, EventArgs e)
        {
            // Thêm dòng kiểm tra này để chắc chắn không bị Null
            if (cboStatus != null && cboLevel != null)
            {
                LoadComboBoxData();

                if (_currentCourse != null)
                {
                    FillData();
                }
            }
        }
        private void FillData()
        {
            if (_currentCourse != null)
            {
                txtCourseCode.Text = _currentCourse.CourseCode;
                txtCourseName.Text = _currentCourse.CourseName;
                txtPrice.Text = _currentCourse.Price?.ToString();
                txtDescription.Text = _currentCourse.Description;
                cboLevel.SelectedItem = _currentCourse.Level;
                cboStatus.SelectedItem = _currentCourse.Status;
                cboInstructor.SelectedValue = _currentCourse.InstructorID;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra dữ liệu đầu vào cơ bản (Validation)
            if (string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khóa học!");
                return;
            }

            // 2. Thu thập dữ liệu từ giao diện vào một đối tượng DTO
            CourseDTO course = _currentCourse ?? new CourseDTO();
            course.CourseName = txtCourseName.Text.Trim();
            course.Description = txtDescription.Text.Trim();
            course.Level = cboLevel.SelectedItem?.ToString();
            course.Status = cboStatus.SelectedItem?.ToString();
            course.InstructorID = (int?)cboInstructor.SelectedValue;

            // Xử lý chuyển đổi giá tiền (tránh lỗi nhập chữ)
            if (decimal.TryParse(txtPrice.Text, out decimal price))
                course.Price = price;

            // 3. Gọi BUS để lưu
            bool result;
            if (_currentCourse == null) // Trường hợp Thêm mới
            {
                result = _courseBUS.AddCourse(course);
            }
            else // Trường hợp Sửa
            {
                result = _courseBUS.UpdateCourse(course);
            }

            // 4. Thông báo kết quả
            if (result)
            {
                MessageBox.Show("Lưu thông tin thành công!");
                this.DialogResult = DialogResult.OK; // Đóng Form và báo cho trang danh sách biết để load lại
                this.Close();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi lưu dữ liệu.");
            }
        }
    }
}
