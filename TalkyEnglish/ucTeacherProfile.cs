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
    public partial class ucTeacherProfile : UserControl
    {
        public ucTeacherProfile()
        {
            InitializeComponent();
        }

        private void LoadProfileData()
        {
            // 1. Kiểm tra phiên đăng nhập của Giảng viên
            if (SessionManager.CurrentUser != null)
            {
                var user = SessionManager.CurrentUser;

                // --- KHU VỰC NHẬP LIỆU (BÊN TRÁI) ---
                // Lưu ý: Giảng viên dùng mã định danh tương ứng (thường là UserName hoặc ID)
                guna2TextBox1.Text = user.UserID.ToString();
                txtFullName.Text = user.FullName;
                txtEmail.Text = user.Email;
                txtPhone.Text = user.PhoneNumber;

                // Xử lý ngày sinh
                if (user.Birthday.HasValue)
                {
                    dtpBirthday.Value = user.Birthday.Value;
                }

                // Xử lý giới tính
                if (user.Gender == "Nam") rbMale.Checked = true;
                else if (user.Gender == "Nữ") rbFemale.Checked = true;

                // Xử lý ngày tham gia công tác (CreatedAt)


                // --- KHU VỰC PROFILE CARD (BÊN PHẢI) ---
                lblDisplayFullName.Text = user.FullName;
                // Giả sử bạn dùng UserID để hiển thị mã giảng viên
                guna2TextBox1.Text = user.UserID.ToString();
                lblDisplayStudentCode.Text = user.UserID.ToString();// Hiển thị mã giảng viên
                lblDisplayEmail.Text = user.Email;
                lblDisplayPhone.Text = user.PhoneNumber ?? "---";

                // Hiển thị trạng thái tài khoản
                lblAccountStatus.Text = user.Status ?? "Đang hoạt động";
                txtRegistrationDate.Text = user.CreatedAt?.ToString("dd/MM/yyyy") ?? "---";

                // Hiển thị lần đăng nhập hiện tại
                lblLastLogin.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                // --- NẠP ẢNH ĐẠI DIỆN ---
                LoadAvatarFromServer(user.UserID);
            }
        }
        private void LoadAvatarFromServer(int userId)
        {
            string folderPath = Path.Combine(Application.StartupPath, "Avatars");
            string avatarPath = Path.Combine(folderPath, $"Avatar_{userId}.jpg");

            if (File.Exists(avatarPath))
            {
                if (picAvatar.Image != null) picAvatar.Image.Dispose();

                using (FileStream fs = new FileStream(avatarPath, FileMode.Open, FileAccess.Read))
                {
                    picAvatar.Image = Image.FromStream(fs);
                }
            }
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
                        string folderPath = Path.Combine(Application.StartupPath, "Avatars");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        string fileName = $"Avatar_{SessionManager.CurrentUser.UserID}.jpg";
                        string destPath = Path.Combine(folderPath, fileName);

                        // Giải phóng luồng ảnh cũ để ghi đè file
                        if (picAvatar.Image != null)
                        {
                            picAvatar.Image.Dispose();
                            picAvatar.Image = null;
                        }

                        File.Copy(ofd.FileName, destPath, true);

                        // Nạp lại ảnh mới
                        LoadAvatarFromPath(destPath);

                        // CẬP NHẬT FORM CHÍNH CỦA GIẢNG VIÊN (frmMain_Teacher)
                        // Giả sử form chính của giảng viên tên là frmMain_Teacher
                        var mainForm = this.FindForm() as frmTeacherDashboard;
                        if (mainForm != null)
                        {
                            mainForm.RefreshSmallAvatar();
                        }

                        MessageBox.Show("Ảnh đại diện đã được cập nhật thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể cập nhật ảnh đại diện: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void UpdateMainFormAvatar()
        {
            // Tìm đến Form chính và yêu cầu nạp lại ảnh đại diện nhỏ
            var mainForm = this.FindForm() as frmTeacherDashboard;
            if (mainForm != null)
            {
                // Bước này sẽ gọi hàm RefreshSmallAvatar mà anh em mình sẽ thêm vào Form chính sau
                mainForm.RefreshSmallAvatar();
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

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            var currentUser = SessionManager.CurrentUser;
            if (currentUser == null) return;

            // 1. Kiểm tra dữ liệu đầu vào cơ bản
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Họ tên không được để trống.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Thu thập dữ liệu cập nhật
            currentUser.FullName = txtFullName.Text.Trim();
            currentUser.Email = txtEmail.Text.Trim();
            currentUser.PhoneNumber = txtPhone.Text.Trim();
            currentUser.Birthday = dtpBirthday.Value;
            currentUser.Gender = rbMale.Checked ? "Nam" : "Nữ";

            // 3. Gọi tầng BUS để thực thi lưu trữ
            UserBUS userBus = new UserBUS();
            bool result = userBus.UpdateUserInfo(currentUser);

            if (result)
            {
                MessageBox.Show("Thông tin cá nhân đã được cập nhật thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SessionManager.CurrentUser = currentUser; // Cập nhật lại Session
                LoadProfileData(); // Cập nhật lại giao diện
            }
            else
            {
                MessageBox.Show("Cập nhật thông tin thất bại. Vui lòng kiểm tra lại kết nối.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            LoadProfileData();
        }

        private void ucTeacherProfile_Load(object sender, EventArgs e)
        {
            LoadProfileData();
        }
    }
}
