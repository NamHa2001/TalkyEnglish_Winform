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
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class frmStudentDetail : Form
    {

        private UserDTO _student;
        private bool _isUpdate = false;
        private UserDAL _userDAL = new UserDAL();
        public frmStudentDetail()
        {
            InitializeComponent();
            _student = new UserDTO();
            _isUpdate = false;
            lblTitle.Text = "Thêm mới học viên";
        }

        public frmStudentDetail(UserDTO student)
        {
            InitializeComponent();
            _student = student;
            _isUpdate = true;
            lblTitle.Text = "Cập nhật thông tin học viên";
            FillDataToControls(); // Hàm này để đổ dữ liệu cũ vào các ô nhập
        }
        private void FillDataToControls()
        {
            if (_student != null)
            {
                txtFullName.Text = _student.FullName;
                txtEmail.Text = _student.Email;
                txtPhoneNumber.Text = _student.PhoneNumber;
                txtID.Text = _student.StudentCode;
                dtpCreatedAt.Value = _student.CreatedAt ?? DateTime.Now;
                if (_student.Birthday.HasValue)
                    dtpBirthday.Value = _student.Birthday.Value;
                else
                    dtpBirthday.Value = DateTime.Now;

                cboGender.Text = _student.Gender;
                cboCourse.Text = _student.CourseName;
                cboLevel.Text = _student.Level;
                cboStatus.Text = _student.Status;
            }
        }

        private void InitData()
        {
            // 1. Nạp Giới tính
            cboGender.Items.Clear();
            cboGender.Items.AddRange(new string[] { "Nam", "Nữ", "Khác" });
            if (cboGender.Items.Count > 0) cboGender.SelectedIndex = 0;

            // 2. Nạp Trình độ
            cboLevel.Items.Clear();
            cboLevel.Items.AddRange(new string[] { "Cơ bản", "Trung cấp", "Nâng cao" });
            if (cboLevel.Items.Count > 0) cboLevel.SelectedIndex = 0;

            // 3. Nạp Trạng thái
            cboStatus.Items.Clear();
            cboStatus.Items.AddRange(new string[] { "Đang học", "Bảo lưu", "Đã nghỉ" });
            if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;

            // 4. Nạp danh sách Khóa học từ Database (CourseDAL)
            try
            {
                CourseDAL courseDAL = new CourseDAL();
                var courses = courseDAL.GetAllCourses();
                cboCourse.DataSource = courses;
                cboCourse.DisplayMember = "CourseName"; // Hiển thị tên khóa học
                cboCourse.ValueMember = "CourseName";   // Giá trị lấy ra là tên khóa học để lưu vào UserDTO
            }
            catch { /* Xử lý lỗi nếu cần */ }
        }

        private void frmStudentDetail_Load(object sender, EventArgs e)
        {
            InitData(); // Nạp dữ liệu vào các ComboBox trước

            // Nếu là chế độ Cập nhật thì mới điền dữ liệu của học viên vào
            if (_isUpdate)
            {
                FillDataToControls();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Thu thập dữ liệu từ các Control bạn đã đặt tên
            _student.FullName = txtFullName.Text.Trim();
            _student.Email = txtEmail.Text.Trim();
            _student.PhoneNumber = txtPhoneNumber.Text.Trim();
            _student.Birthday = dtpBirthday.Value;
            _student.Gender = cboGender.Text;
            _student.CourseName = cboCourse.Text; // Lấy từ ComboBox Khóa học mới thêm
            _student.Level = cboLevel.Text;
            _student.Status = cboStatus.Text;
            _student.CreatedAt = dtpCreatedAt.Value;
            if (!_isUpdate) // Chỉ gán mật khẩu mặc định khi thêm mới
            {
                _student.PasswordHash = "123456";
            }

            // 2. Gọi tầng BUS để xử lý
            UserBUS userBUS = new UserBUS();
            bool result;

            if (_isUpdate)
                result = userBUS.UpdateStudent(_student);
            else
                result = userBUS.AddStudent(_student);

            // 3. Thông báo kết quả
            if (result)
            {
                MessageBox.Show("Lưu thông tin học viên thành công!", "Thông báo");
                this.DialogResult = DialogResult.OK; // Để trang chính biết mà load lại bảng
                this.Close();
            }
            else
            {
                MessageBox.Show("Lưu thất bại, vui lòng kiểm tra lại dữ liệu.", "Lỗi");
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // 1. Xóa trắng các TextBox
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhoneNumber.Clear();

            // Nếu là chế độ Thêm mới thì mới xóa mã học viên (nếu bạn cho nhập tay)
            if (!_isUpdate)
            {
                txtID.Clear();
            }

            // 2. Đưa các DateTimePicker về ngày hiện tại
            dtpBirthday.Value = DateTime.Now;
            dtpCreatedAt.Value = DateTime.Now;

            // 3. Đưa các ComboBox về lựa chọn đầu tiên
            if (cboGender.Items.Count > 0) cboGender.SelectedIndex = 0;
            if (cboCourse.Items.Count > 0) cboCourse.SelectedIndex = 0;
            if (cboLevel.Items.Count > 0) cboLevel.SelectedIndex = 0;
            if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;

            txtFullName.Focus(); // Đưa con trỏ chuột về ô Họ tên để nhập lại cho nhanh
        }
    }
}
