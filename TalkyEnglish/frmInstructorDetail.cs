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
    public partial class frmInstructorDetail : Form
    {
        UserBUS _userBUS = new UserBUS();
        private bool _isEditMode = false; // Mặc định là Thêm
        private UserDTO _currentInstructor; // Biến chứa dữ liệu khi Sửa
        public frmInstructorDetail()
        {
            InitializeComponent();
            _isEditMode = false;
            lblTitle.Text = "THÊM GIẢNG VIÊN MỚI"; // Giả sử bạn có cái Label tiêu đề ở trên cùng
        }

        private void guna2GroupBox1_Click(object sender, EventArgs e)
        {

        }

        public frmInstructorDetail(UserDTO instructor)
        {
            InitializeComponent();
            _isEditMode = true;
            _currentInstructor = instructor;
            lblTitle.Text = "CẬP NHẬT THÔNG TIN GIẢNG VIÊN";

            // Sau khi nạp xong giao diện, ta sẽ đổ dữ liệu của 'instructor' lên các ô
            MappingDataToFields();
        }

        private void MappingDataToFields()
        {
            // Kiểm tra nếu đối tượng dữ liệu không rỗng mới làm
            if (_currentInstructor != null)
            {
                // 1. Thông tin cá nhân
                txtID.Text = _currentInstructor.UserID.ToString();
                txtFullName.Text = _currentInstructor.FullName;
                txtEmail.Text = _currentInstructor.Email;
                txtPhone.Text = _currentInstructor.PhoneNumber;

                // Xử lý ngày sinh (Nếu null thì lấy ngày hiện tại)
                dtpBirthday.Value = _currentInstructor.Birthday ?? DateTime.Now;

                // Xử lý ComboBox Giới tính
                cboGender.Text = _currentInstructor.Gender;

                // 2. Thông tin công việc
                txtSpecialization.Text = _currentInstructor.Specialization;
                cboDegree.Text = _currentInstructor.Degree;
                cboStatus.Text = _currentInstructor.Status;

                // Xử lý ngày tham gia
                dtpCreatedAt.Value = _currentInstructor.CreatedAt ?? DateTime.Now;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Kiểm tra dữ liệu đầu vào (Validation) cơ bản
                if (string.IsNullOrWhiteSpace(txtFullName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ Họ tên và Email!", "Thông báo");
                    return;
                }

                // 2. Thu thập dữ liệu từ các ô nhập liệu vào một đối tượng DTO
                UserDTO instructor = new UserDTO();

                // Nếu là Sửa, ta phải giữ lại cái ID cũ để Database biết đường mà Update
                if (_isEditMode && _currentInstructor != null)
                {
                    instructor.UserID = _currentInstructor.UserID;
                }

                instructor.FullName = txtFullName.Text.Trim();
                instructor.Email = txtEmail.Text.Trim();
                instructor.PhoneNumber = txtPhone.Text.Trim();
                instructor.Birthday = dtpBirthday.Value;
                instructor.Gender = cboGender.Text;
                instructor.Specialization = txtSpecialization.Text.Trim();
                instructor.Degree = cboDegree.Text;
                instructor.Status = cboStatus.Text;

                // Mặc định mật khẩu cho giảng viên mới nếu bạn muốn (Ví dụ: 123)
                instructor.PasswordHash = "123";

                // 3. Thực hiện hành động tùy theo chế độ (Thêm hoặc Sửa)
                bool result = false;
                if (_isEditMode)
                {
                    result = _userBUS.UpdateInstructor(instructor);
                    if (result) MessageBox.Show("Cập nhật giảng viên thành công!", "Thông báo");
                }
                else
                {
                    result = _userBUS.InsertInstructor(instructor);
                    if (result) MessageBox.Show("Thêm giảng viên mới thành công!", "Thông báo");
                }

                // 4. Nếu thành công thì đóng Form để quay về bảng chính
                if (result)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
