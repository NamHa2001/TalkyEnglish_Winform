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
    }
}
