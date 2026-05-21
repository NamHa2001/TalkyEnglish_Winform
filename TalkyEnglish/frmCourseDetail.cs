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
            txtCourseCode.Text = _courseBUS.GenerateCourseCode();
            ApplyModernStyle();
        }
        public frmCourseDetail(CourseDTO course)
        {
            InitializeComponent();
            _currentCourse = course; // Lưu lại dữ liệu cũ
            lblTitle.Text = "Chỉnh Sửa Thông Tin Khóa Học";
            this.Text = "Chỉnh Sửa Khóa Học";
            ApplyModernStyle();
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
            ButtonEffectHelper.RemoveGrayEffect(this);
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
                txtDuration.Text = _currentCourse.Duration;
                txtCourseCode.Text = _currentCourse.CourseCode;
                txtCourseName.Text = _currentCourse.CourseName;
                txtPrice.Text = _currentCourse.Price?.ToString("N0");
                txtDescription.Text = _currentCourse.Description;
                cboLevel.SelectedItem = _currentCourse.Level;
                cboStatus.SelectedItem = _currentCourse.Status;
                txtPrice.ForeColor = ColorTranslator.FromHtml("#0F172A");
                cboInstructor.SelectedValue = _currentCourse.InstructorID;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Validation: Tên khóa học không được để trống
            if (string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên khóa học!");
                txtCourseName.Focus();
                return;
            }

            // 2. Thu thập dữ liệu vào DTO
            // Nếu _currentCourse null thì tạo mới, nếu không thì dùng lại cái cũ để Update
            CourseDTO course = _currentCourse ?? new CourseDTO();

            course.CourseCode = txtCourseCode.Text.Trim();
            course.CourseName = txtCourseName.Text.Trim();
            course.Description = txtDescription.Text.Trim();
            course.Duration = txtDuration.Text.Trim(); // Thu thập thời lượng
            course.Level = cboLevel.SelectedItem?.ToString();
            course.Status = cboStatus.SelectedItem?.ToString();
            course.InstructorID = (int?)cboInstructor.SelectedValue;

            // Xử lý giá tiền
            if (decimal.TryParse(txtPrice.Text, out decimal price))
            {
                course.Price = price;
            }
            else
            {
                course.Price = 0;
            }

            // 3. Thực hiện lưu thông qua tầng BUS
            bool result;
            if (_currentCourse == null)
            {
                // Thêm mới thì gán ngày tạo
                course.CreatedAt = DateTime.Now;
                result = _courseBUS.AddCourse(course);
            }
            else
            {
                result = _courseBUS.UpdateCourse(course);
            }

            // 4. Phản hồi người dùng
            if (result)
            {
                MessageBox.Show("Lưu thông tin khóa học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra trong quá trình lưu dữ liệu vào cơ sở dữ liệu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close(); // Nút Hủy bỏ
        }

        private void ApplyModernStyle()
        {
            // --- 1. MÀU NỀN TỔNG THỂ ---
            // Thêm màu nền xám xanh rất nhạt (#F1F5F9) để form trông dịu mắt và hiện đại
            this.BackColor = ColorTranslator.FromHtml("#F1F5F9");

            // --- 2. ĐỊNH DẠNG PANEL CHÍNH ---
            // Chỉnh Panel chứa các ô nhập liệu thành màu trắng tinh khôi (#FFFFFF)
            if (guna2Panel1 != null)
            {
                guna2Panel1.FillColor = Color.White;
                // Thêm bo góc cho Panel (Nếu Guna hỗ trợ thuộc tính Radius)
                guna2Panel1.BorderRadius = 15;
            }

            // --- 3. TIÊU ĐỀ (HEADER) ---
            // Chỉnh tiêu đề thành màu Navy đậm (#0F172A) và Font Segoe UI đậm
            lblTitle.ForeColor = ColorTranslator.FromHtml("#0F172A");
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);

            // --- 4. CÁC NHÃN (LABELS) ---
            // Duyệt qua tất cả Label để chỉnh sang màu xám phụ (#64748B) cho tinh tế
            foreach (Control c in this.Controls)
            {
                if (c is Label && c != lblTitle)
                {
                    c.ForeColor = ColorTranslator.FromHtml("#64748B");
                    c.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                }
            }

            // --- 5. NÚT LƯU THÔNG TIN ---
            // Chỉnh màu nút Lưu thành Xanh dương chính (#2563EB), chữ trắng
            btnSave.FillColor = ColorTranslator.FromHtml("#2563EB");
            btnSave.ForeColor = Color.White;
            btnSave.BorderRadius = 10;
            // Hiệu ứng di chuột: Đậm hơn một chút khi hover
            btnSave.HoverState.FillColor = Color.FromArgb(30, 80, 220);

            // --- 6. NÚT HỦY BỎ ---
            // Chỉnh màu nút Hủy thành nền trắng, viền xám (#64748B) để phân cấp với nút Lưu
            btnCancel.FillColor = Color.White;
            btnCancel.ForeColor = ColorTranslator.FromHtml("#64748B");
            btnCancel.BorderColor = ColorTranslator.FromHtml("#64748B");
            btnCancel.BorderThickness = 1;
            btnCancel.BorderRadius = 10;
            // Hiệu ứng di chuột: Chuyển sang màu nền nhạt
            btnCancel.HoverState.FillColor = ColorTranslator.FromHtml("#F1F5F9");

            // --- 7. ĐỊNH DẠNG TEXTBOX & COMBOBOX ---
            // Duyệt qua các ô nhập liệu bên trong panel để đồng bộ màu viền và bo góc
            if (guna2Panel1 != null)
            {
                foreach (Control c in this.guna2Panel1.Controls)
                {
                    if (c is Guna.UI2.WinForms.Guna2TextBox || c is Guna.UI2.WinForms.Guna2ComboBox)
                    {
                        var gControl = (dynamic)c;
                        gControl.BorderRadius = 8;
                        // Khi click vào ô nhập liệu, viền sẽ đổi sang màu Xanh dương chính (#2563EB)
                        gControl.FocusedState.BorderColor = ColorTranslator.FromHtml("#2563EB");
                    }
                }
            }
        }
    }
}

