using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucProfile : UserControl
    {
        public ucProfile()
        {
            InitializeComponent();
        }

        private void ucProfile_Load(object sender, EventArgs e)
        {
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            // 1. Kiểm tra bảo mật Session để lấy dữ liệu thực
            if (SessionManager.CurrentUser != null)
            {
                var user = SessionManager.CurrentUser;

                // --- KHU VỰC NHẬP LIỆU (BÊN TRÁI) ---
                guna2TextBox1.Text = user.StudentCode;
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtPhone.Text = user.PhoneNumber;

                // Xử lý ngày sinh (Dùng thuộc tính Birthday trong DTO)
                if (user.Birthday.HasValue)
                {
                    dtpBirthday.Value = user.Birthday.Value;
                }

                // Xử lý giới tính (Dùng thuộc tính Gender trong DTO)
                if (user.Gender == "Nam") rbMale.Checked = true;
                else if (user.Gender == "Nữ") rbFemale.Checked = true;

                // Xử lý ngày đăng ký (Dùng thuộc tính CreatedAt trong DTO)
                if (user.CreatedAt.HasValue)
                {
                    txtRegistrationDate.Value = user.CreatedAt.Value;
                }

                // --- KHU VỰC PROFILE CARD (BÊN PHẢI) ---
                lblDisplayFullName.Text = user.FullName;
                lblDisplayStudentCode.Text = user.StudentCode;
                lblDisplayEmail.Text = user.Email;
                lblDisplayPhone.Text = user.PhoneNumber ?? "---";

                // Hiển thị trạng thái (Dùng thuộc tính Status trong DTO)
                lblAccountStatus.Text = user.Status ?? "Hoạt động";

                lblCreatedAt.Text = user.CreatedAt?.ToString("dd/MM/yyyy") ?? "---";

                // Hiển thị lần đăng nhập cuối
                lblLastLogin.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                // --- MỚI: NẠP ẢNH ĐẠI DIỆN (Tránh khóa file) ---
                string folderPath = Path.Combine(Application.StartupPath, "Avatars");
                string avatarPath = Path.Combine(folderPath, $"Avatar_{user.UserID}.jpg");

                if (File.Exists(avatarPath))
                {
                    // Giải phóng ảnh cũ nếu có để tránh tràn bộ nhớ
                    if (picAvatar.Image != null) picAvatar.Image.Dispose();

                    // Dùng FileStream để đọc ảnh, sau đó đóng stream ngay để không khóa file
                    using (FileStream fs = new FileStream(avatarPath, FileMode.Open, FileAccess.Read))
                    {
                        picAvatar.Image = Image.FromStream(fs);
                    }
                }
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadProfileData();
        }

        private void picAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 1. Chuẩn bị thư mục
                        string folderPath = Path.Combine(Application.StartupPath, "Avatars");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        string fileName = $"Avatar_{SessionManager.CurrentUser.UserID}.jpg";
                        string destPath = Path.Combine(folderPath, fileName);

                        // 2. QUAN TRỌNG: Giải phóng ảnh cũ đang hiển thị tại UC này để có thể ghi đè
                        if (picAvatar.Image != null)
                        {
                            picAvatar.Image.Dispose();
                            picAvatar.Image = null;
                        }

                        // 3. Copy ảnh mới đè lên ảnh cũ
                        File.Copy(ofd.FileName, destPath, true);

                        // 4. Nạp lại ảnh cho picAvatar tại UserControl này
                        LoadAvatarFromPath(destPath);

                        // 5. CẬP NHẬT FORM CHÍNH: Chỉ chạy khi các bước trên đã thành công
                        frmMain_Student mainForm = (frmMain_Student)this.FindForm();
                        if (mainForm != null)
                        {
                            mainForm.RefreshSmallAvatar();
                        }

                        MessageBox.Show("Đã lưu ảnh đại diện mới!", "Thông báo");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể lưu ảnh: " + ex.Message);
                    }
                }
            }
        }

        private void LoadAvatarFromPath(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    picAvatar.Image = Image.FromStream(fs);
                }
            }
        }
        private void SaveAvatar(string filePath)
        {
            // Ở đây bạn sẽ gọi tầng BUS để lưu vào Database
            // Sau khi lưu thành công, hãy cập nhật lại Session
            // SessionManager.CurrentUser.AvatarPath = newPath;

            MessageBox.Show("Cập nhật ảnh đại diện thành công!", "Thông báo");
        }

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            // 1. Thu thập dữ liệu từ giao diện
            var currentUser = SessionManager.CurrentUser;
            if (currentUser == null) return;

            currentUser.FullName = txtFullName.Text.Trim();
            currentUser.Email = txtEmail.Text.Trim();
            currentUser.PhoneNumber = txtPhone.Text.Trim();
            currentUser.Birthday = dtpBirthday.Value;
            currentUser.Gender = rbMale.Checked ? "Nam" : "Nữ";

            // 2. Gọi tầng BUS để lưu xuống Database (Giả sử bạn có UserBUS)
            // UserBUS userBus = new UserBUS();
            // bool result = userBus.UpdateUserInfo(currentUser);

            // 3. Thông báo và cập nhật lại hiển thị
            // if (result) {
            MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadProfileData(); // Nạp lại dữ liệu để các nhãn bên phải cập nhật theo
                               // }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            // --- BƯỚC 1: KHỞI TẠO FORM "ẢO" (DYNAMIC FORM) ---
            Form prompt = new Form()
            {
                Width = 400,
                Height = 350,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Hệ thống - Đổi mật khẩu",
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.White
            };

            // --- BƯỚC 2: TẠO CÁC CONTROL (DÙNG GUNA2 NẾU BRO ĐÃ CÀI, Ở ĐÂY TÔI DÙNG WINDOWS FORM CHUẨN CHO CHẮC CHẮN) ---
            Label lblTitle = new Label() { Left = 50, Top = 20, Text = "THAY ĐỔI MẬT KHẨU", Width = 300, Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#0F172A") };

            Label lblOld = new Label() { Left = 50, Top = 60, Text = "Mật khẩu hiện tại:", Width = 300, ForeColor = Color.DimGray };
            TextBox txtOld = new TextBox() { Left = 50, Top = 85, Width = 280, PasswordChar = '*', Font = new Font("Segoe UI", 10) };

            Label lblNew = new Label() { Left = 50, Top = 120, Text = "Mật khẩu mới:", Width = 300, ForeColor = Color.DimGray };
            TextBox txtNew = new TextBox() { Left = 50, Top = 145, Width = 280, PasswordChar = '*', Font = new Font("Segoe UI", 10) };

            Label lblConfirm = new Label() { Left = 50, Top = 180, Text = "Xác nhận mật khẩu mới:", Width = 300, ForeColor = Color.DimGray };
            TextBox txtConfirm = new TextBox() { Left = 50, Top = 205, Width = 280, PasswordChar = '*', Font = new Font("Segoe UI", 10) };

            // Nút Xác nhận (Màu xanh dương #2563EB)
            Button btnSubmit = new Button()
            {
                Text = "Cập nhật",
                Left = 50,
                Top = 250,
                Width = 130,
                Height = 35,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSubmit.FlatAppearance.BorderSize = 0;

            // Nút Hủy
            Button btnClose = new Button()
            {
                Text = "Hủy bỏ",
                Left = 200,
                Top = 250,
                Width = 130,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // --- BƯỚC 3: LOGIC XỬ LÝ KHI BẤM NÚT ---
            btnSubmit.Click += (s, args) =>
            {
                // 1. Kiểm tra trống
                if (string.IsNullOrWhiteSpace(txtOld.Text) || string.IsNullOrWhiteSpace(txtNew.Text) || string.IsNullOrWhiteSpace(txtConfirm.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ các trường mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Kiểm tra khớp mật khẩu mới
                if (txtNew.Text != txtConfirm.Text)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp với mật khẩu mới!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3. Kiểm tra độ dài mật khẩu (tùy chọn)
                if (txtNew.Text.Length < 6)
                {
                    MessageBox.Show("Mật khẩu mới phải có ít nhất 6 ký tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // GIẢ LẬP KẾT NỐI DATABASE (Bro sẽ thay bằng UserBUS sau này)
                    // bool isOldPassCorrect = userBus.CheckOldPassword(SessionManager.CurrentUser.UserID, txtOld.Text);
                    bool isOldPassCorrect = (txtOld.Text == "123"); // Đây là ví dụ, mặc định pass cũ là 123

                    if (isOldPassCorrect)
                    {
                        // Thực hiện update trong DB
                        // userBus.UpdatePassword(SessionManager.CurrentUser.UserID, txtNew.Text);

                        MessageBox.Show("Chúc mừng! Mật khẩu của bạn đã được thay đổi thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        prompt.Close();
                    }
                    else
                    {
                        MessageBox.Show("Mật khẩu hiện tại không chính xác. Vui lòng kiểm tra lại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                }
            };

            btnClose.Click += (s, args) => prompt.Close();

            // Thêm các thành phần vào bảng
            prompt.Controls.Add(lblTitle);
            prompt.Controls.Add(lblOld); prompt.Controls.Add(txtOld);
            prompt.Controls.Add(lblNew); prompt.Controls.Add(txtNew);
            prompt.Controls.Add(lblConfirm); prompt.Controls.Add(txtConfirm);
            prompt.Controls.Add(btnSubmit); prompt.Controls.Add(btnClose);

            // Hiển thị dạng hộp thoại
            prompt.ShowDialog();
        }
    }
}
