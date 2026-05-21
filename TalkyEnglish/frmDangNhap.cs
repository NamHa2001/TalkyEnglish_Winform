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
    public partial class frmDangNhap : Form
    {
        UserBUS _userBUS = new UserBUS();
        public frmDangNhap()
        {
            InitializeComponent();
        }

        private void frmDangNhap_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Email và Mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Gọi tầng BUS để xác thực
                UserDTO user = _userBUS.Login(email, password);

                // TRƯỜNG HỢP 1: Sai tài khoản hoặc mật khẩu (User trả về null)
                if (user == null)
                {
                    MessageBox.Show("Email hoặc mật khẩu không chính xác. Vui lòng kiểm tra lại!", "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // TRƯỜNG HỢP 2: Đăng nhập thành công
                SessionManager.CurrentUser = user;
                MessageBox.Show($"Chào mừng {user.FullName} ({user.Role}) đã quay trở lại!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Điều hướng dựa trên quyền (Role)
                string userRole = user.Role.Trim().ToUpper();

                if (userRole == "ADMIN")
                {
                    try
                    {
                        frmDashboard_Admin adminForm = new frmDashboard_Admin();
                        adminForm.Show();
                        this.Hide();
                    }
                    catch (Exception exAdmin)
                    {
                        // Log lỗi chi tiết nếu Form Admin bị treo khi khởi tạo
                        MessageBox.Show("Đã xảy ra lỗi khi khởi tạo giao diện Admin:\n" + exAdmin.Message, "Lỗi khởi tạo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                else if (userRole == "STUDENT")
                {
                    frmMain_Student studentForm = new frmMain_Student();
                    studentForm.Show();
                    this.Hide();
                }
                else if (userRole == "INSTRUCTOR")
                {
                    // frmMain_Teacher là tên cái Form bro vừa thiết kế Dashboard
                    frmTeacherDashboard teacherForm = new frmTeacherDashboard();
                    teacherForm.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                // TRƯỜNG HỢP 3: Lỗi kết nối Database hoặc lỗi hệ thống khác
                MessageBox.Show("Lỗi kết nối hệ thống: " + ex.Message, "Lỗi kỹ thuật", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPassword_IconRightClick(object sender, EventArgs e)
        {
            // Kiểm tra xem hiện tại có đang ẩn mật khẩu không
            if (txtPassword.UseSystemPasswordChar)
            {
                // Nếu đang ẩn -> Hiện mật khẩu và đổi icon sang mắt gạch chéo (nếu có)
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.PasswordChar = '\0'; // Hiện ký tự bình thường
                                                 // txtMatKhau.IconRight = Properties.Resources.eye_open; // Đổi icon nếu muốn
            }
            else
            {
                // Nếu đang hiện -> Ẩn mật khẩu
                txtPassword.UseSystemPasswordChar = true;
                // txtMatKhau.IconRight = Properties.Resources.eye_close; // Đổi icon nếu muốn
            }
        }

        private void lblRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 1. Khởi tạo Form Đăng ký
            frmDangKy registerForm = new frmDangKy();

            // 2. Hiển thị Form Đăng ký
            registerForm.Show();

            // 3. Ẩn Form Đăng nhập hiện tại (hoặc Close nếu bạn muốn giải phóng bộ nhớ)
            this.Hide();
        }
    }
}
