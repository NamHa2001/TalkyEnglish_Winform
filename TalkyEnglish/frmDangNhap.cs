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

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // 1. Lấy thông tin từ giao diện
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 2. Kiểm tra nhanh (Validation) trước khi gọi xuống Database
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Email và Mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 3. Gọi tầng BUS để xác thực
                UserDTO user = _userBUS.Login(email, password);

                if (user != null)
                {
                    // Đăng nhập thành công
                    MessageBox.Show($"Chào mừng {user.FullName} ({user.Role}) đã quay trở lại!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tạm thời để đây, sau này ta sẽ mở trang Dashboard chính tại đây
                    // frmMain main = new frmMain(user);
                    // main.Show();
                    // this.Hide();
                }
                else
                {
                    // Thất bại
                    MessageBox.Show("Email hoặc mật khẩu không chính xác. Vui lòng thử lại!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Xử lý nếu có lỗi kết nối Database
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
