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
    public partial class ucStudentManagement : UserControl
    {
        UserBUS _userBUS = new UserBUS();

        // Khai báo một BindingSource làm trung gian để giữ cột Design không bị mất
        private BindingSource studentBindingSource = new BindingSource();

        public ucStudentManagement()
        {
            InitializeComponent();
            // CHỐT HẠ: Tắt chế độ tự sinh cột để Grid luôn giữ đúng các cột tiếng Việt bạn đã thiết kế
            dgvStudents.AutoGenerateColumns = false;
        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {
        }

        private void ucStudentManagement_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void BindingData()
        {
            // Kiểm tra CurrentRow để chắc chắn có dòng đang được chọn
            if (dgvStudents.CurrentRow != null)
            {
                try
                {
                    // Lấy trực tiếp đối tượng dữ liệu gắn với dòng đó (Cách này an toàn hơn dùng Cells)
                    var student = (UserDTO)dgvStudents.CurrentRow.DataBoundItem;

                    if (student != null)
                    {
                        txtID.Text = student.UserID.ToString();
                        txtFullName.Text = student.FullName;
                        txtEmail.Text = student.Email;
                        txtPhone.Text = student.PhoneNumber;
                        dtpBirthDay.Value = student.Birthday ?? DateTime.Now;
                        cboGender.Text = student.Gender;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi Binding: " + ex.Message);
                }
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {
        }

        public void LoadData()
        {
            try
            {
                // 1. Lấy danh sách học viên tươi từ SQL qua tầng BUS
                List<UserDTO> listStudents = _userBUS.GetAllStudents();

                // 2. Sử dụng BindingSource để nạp dữ liệu. 
                // Cách này giúp Grid không bị "giật" hoặc tự reset cột
                studentBindingSource.DataSource = listStudents;
                dgvStudents.DataSource = studentBindingSource;

                // 3. Ép Grid vẽ lại dữ liệu mới nhất
                studentBindingSource.ResetBindings(false);
                dgvStudents.Refresh();

                // Ẩn cột mật khẩu (Nếu trong Design bạn lỡ để hiển thị)
                if (dgvStudents.Columns["PasswordHash"] != null)
                    dgvStudents.Columns["PasswordHash"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách học viên: " + ex.Message);
            }
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            BindingData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtID.Clear();
            txtFullName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            dtpBirthDay.Value = DateTime.Now;
            cboGender.SelectedIndex = -1;
            txtFullName.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string fullName = txtFullName.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string gender = cboGender.Text;
                DateTime dob = dtpBirthDay.Value;
                string password = "123";

                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email))
                {
                    MessageBox.Show("Họ tên và Email là bắt buộc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!email.Contains("@") || !email.Contains("."))
                {
                    MessageBox.Show("Email không đúng định dạng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (DateTime.Now.Year - dob.Year < 5)
                {
                    MessageBox.Show("Ngày sinh không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_userBUS.IsEmailExist(email))
                {
                    MessageBox.Show("Email này đã được sử dụng bởi một tài khoản khác!", "Lỗi trùng dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                UserDTO newStudent = new UserDTO
                {
                    FullName = fullName,
                    Email = email,
                    PhoneNumber = phone,
                    PasswordHash = password,
                    Birthday = dob,
                    Gender = gender,
                    Role = "Student",
                    CreatedAt = DateTime.Now
                };

                string result = _userBUS.Register(newStudent, password, true);

                if (result == "SUCCESS")
                {
                    MessageBox.Show("Thêm học viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    btnRefresh_Click(null, null);
                }
                else
                {
                    MessageBox.Show("Lỗi: " + result, "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống khi thêm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvStudents.CurrentRow == null || string.IsNullOrEmpty(txtID.Text))
                {
                    MessageBox.Show("Vui lòng chọn học viên cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy đối tượng dữ liệu hiện tại từ dòng đang chọn để giữ mật khẩu và ngày tạo
                var currentStudent = (UserDTO)dgvStudents.CurrentRow.DataBoundItem;

                UserDTO updatedStudent = new UserDTO
                {
                    UserID = int.Parse(txtID.Text),
                    FullName = txtFullName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    PhoneNumber = txtPhone.Text.Trim(),
                    Birthday = dtpBirthDay.Value,
                    Gender = cboGender.Text,
                    Role = "Student",
                    PasswordHash = currentStudent.PasswordHash, // Giữ mật khẩu cũ
                    CreatedAt = currentStudent.CreatedAt       // Giữ ngày tạo cũ
                };

                if (_userBUS.UpdateUser(updatedStudent))
                {
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtID.Text))
                {
                    MessageBox.Show("Vui lòng chọn học viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int userId = int.Parse(txtID.Text);
                string userName = txtFullName.Text;

                DialogResult confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa học viên '{userName}' không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    if (_userBUS.DeleteUser(userId))
                    {
                        MessageBox.Show("Đã xóa học viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        btnRefresh_Click(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvStudents_SelectionChanged(object sender, EventArgs e)
        {
            BindingData();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();

            // Gọi BUS để lấy danh sách đã lọc
            List<UserDTO> result = _userBUS.SearchStudents(keyword);

            // Nạp vào BindingSource để Grid hiển thị (giữ nguyên cột Design)
            studentBindingSource.DataSource = result;
            studentBindingSource.ResetBindings(false);

            if (result.Count == 0)
            {
                // Nếu không thấy ai thì thông báo nhẹ một cái
                Console.WriteLine("Không tìm thấy học viên nào khớp với từ khóa.");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            guna2Button3_Click(null, null);
        }
    }
}