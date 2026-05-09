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
    public partial class frmDangKy : Form
    {
        UserBUS _userBUS = new UserBUS();
        public frmDangKy()
        {
            InitializeComponent();
        }

        private void txtMatKhau_IconRightClick(object sender, EventArgs e)
        {
            // Kiểm tra xem hiện tại có đang ẩn mật khẩu không
            if (txtMatKhau.UseSystemPasswordChar)
            {
                // Nếu đang ẩn -> Hiện mật khẩu và đổi icon sang mắt gạch chéo (nếu có)
                txtMatKhau.UseSystemPasswordChar = false;
                txtMatKhau.PasswordChar = '\0'; // Hiện ký tự bình thường
                                                // txtMatKhau.IconRight = Properties.Resources.eye_open; // Đổi icon nếu muốn
            }
            else
            {
                // Nếu đang hiện -> Ẩn mật khẩu
                txtMatKhau.UseSystemPasswordChar = true;
                // txtMatKhau.IconRight = Properties.Resources.eye_close; // Đổi icon nếu muốn
            }
        }

        private void txtxacnhanmk_IconRightClick(object sender, EventArgs e)
        {
            // Kiểm tra xem hiện tại có đang ẩn mật khẩu không
            if (txtxacnhanmk.UseSystemPasswordChar)
            {
                // Nếu đang ẩn -> Hiện mật khẩu và đổi icon sang mắt gạch chéo (nếu có)
                txtxacnhanmk.UseSystemPasswordChar = false;
                txtxacnhanmk.PasswordChar = '\0'; // Hiện ký tự bình thường
                                                  // txtMatKhau.IconRight = Properties.Resources.eye_open; // Đổi icon nếu muốn
            }
            else
            {
                // Nếu đang hiện -> Ẩn mật khẩu
                txtxacnhanmk.UseSystemPasswordChar = true;
                // txtMatKhau.IconRight = Properties.Resources.eye_close; // Đổi icon nếu muốn
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            // Tạo đối tượng DTO từ form
            UserDTO newUser = new UserDTO
            {
                FullName = txtHoTen.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PasswordHash = txtMatKhau.Text.Trim(),
                Role = "Student", // Mặc định khi đăng ký là học viên
                CreatedAt = DateTime.Now
            };

            string confirmPass = txtxacnhanmk.Text.Trim();
            bool isAccepted = chkAgree.Checked; // chkAgree là tên của Checkbox điều khoản

            // Gọi tầng BUS xử lý
            string result = _userBUS.Register(newUser, confirmPass, isAccepted);

            if (result == "SUCCESS")
            {
                MessageBox.Show("Đăng ký thành công! Hãy đăng nhập nhé.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Chuyển về trang đăng nhập
                frmDangNhap login = new frmDangNhap();
                login.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmDangKy_Load(object sender, EventArgs e)
        {

        }
    }
}
