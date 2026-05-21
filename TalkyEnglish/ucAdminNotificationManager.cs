using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;
using Guna.UI2.WinForms;

namespace TalkyEnglish.GUI
{
    public partial class ucAdminNotificationManager : UserControl
    {
        private string _currentFilter = "All";
        private Label _lblRecipient;
        private ComboBox _cboRecipient;
        private List<UserDTO> _allUsers = new List<UserDTO>();

        public ucAdminNotificationManager()
        {
            InitializeComponent();

            btnAddSchedule.Click += (s, e) => ApplyFilter("All", btnAddSchedule);
            guna2Button3.Click  += (s, e) => ApplyFilter("Teacher", guna2Button3);
            guna2Button5.Click  += (s, e) => ApplyFilter("Student", guna2Button5);
            guna2Button4.Click  += (s, e) => ApplyFilter("Individual", guna2Button4);

            btnReset.Click += btnReset_Click;
            cboTargetType.SelectedIndexChanged += cboTargetType_SelectedIndexChanged;

            InitRecipientPicker();
        }

        // ── Khởi tạo ô chọn người nhận (chỉ hiện khi chọn "Individual") ──
        private void InitRecipientPicker()
        {
            _lblRecipient = new Label
            {
                Text = "Người nhận:",
                Location = new Point(173, 108),
                Size = new Size(100, 22),
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(30, 30, 30),
                BackColor = Color.Transparent,
                Visible = false
            };

            _cboRecipient = new ComboBox
            {
                Location = new Point(173, 130),
                Size = new Size(235, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                Visible = false
            };

            guna2Panel3.Controls.Add(_lblRecipient);
            guna2Panel3.Controls.Add(_cboRecipient);
        }

        private void LoadRecipients()
        {
            _cboRecipient.Items.Clear();
            _allUsers = new UserBUS().GetAllUsersForPicker();
            _cboRecipient.Items.Add("-- Chọn người nhận --");
            foreach (var u in _allUsers)
                _cboRecipient.Items.Add($"{u.FullName}  ({u.Role})");
            _cboRecipient.SelectedIndex = 0;
        }

        private void cboTargetType_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isIndividual = cboTargetType.Text == "Individual";
            _lblRecipient.Visible = isIndividual;
            _cboRecipient.Visible = isIndividual;
            if (isIndividual && _cboRecipient.Items.Count == 0)
                LoadRecipients();
        }

        // ── Tab lọc bên phải ──
        private void ApplyFilter(string filter, Guna2Button activeBtn)
        {
            _currentFilter = filter;
            SetActiveFilterButton(activeBtn);
            LoadData(filter);
        }

        private void SetActiveFilterButton(Guna2Button activeBtn)
        {
            foreach (var btn in new[] { btnAddSchedule, guna2Button3, guna2Button5, guna2Button4 })
            {
                btn.FillColor = Color.White;
                btn.ForeColor = Color.Black;
                btn.HoverState.FillColor = Color.FromArgb(240, 245, 255);
            }
            activeBtn.FillColor = Color.FromArgb(37, 99, 235);
            activeBtn.ForeColor = Color.White;
            activeBtn.HoverState.FillColor = Color.FromArgb(37, 99, 235);
        }

        // ── Tải danh sách thông báo ──
        public void LoadData(string targetFilter = "All")
        {
            try
            {
                if (flpNotifications == null) return;
                flpNotifications.Controls.Clear();

                var bus = new AnnouncementsBUS();
                var list = targetFilter == "All"
                    ? bus.GetAllAnnouncements()
                    : bus.GetByTargetType(targetFilter);

                if (list == null || list.Count == 0)
                {
                    flpNotifications.Controls.Add(new Label
                    {
                        Text = "Không có thông báo nào.",
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10),
                        ForeColor = Color.Gray,
                        Margin = new Padding(10, 20, 0, 0)
                    });
                    return;
                }

                foreach (var item in list)
                {
                    var ucItem = new ucNotificationItem();
                    ucItem.SetData(item);
                    ucItem.Width = flpNotifications.Width - 25;
                    ucItem.DeleteRequested += (s, e) => LoadData(_currentFilter);
                    flpNotifications.Controls.Add(ucItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi LoadData: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Gửi thông báo ──
        private void btnSend_Click(object sender, EventArgs e)
        {
            string title      = txtTitle.Text.Trim();
            string content    = rtbContent.Text.Trim();
            string category   = cboCategory.Text;
            string targetType = cboTargetType.Text;
            string priority   = rdbPriorityUrgent.Checked ? "Urgent" : "Normal";

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
            {
                MessageBox.Show("Vui lòng nhập tiêu đề và nội dung!", "Thiếu thông tin",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (SessionManager.CurrentUser == null)
            {
                MessageBox.Show("Phiên đăng nhập đã hết hạn.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int? receiverID = null;
            if (targetType == "Individual")
            {
                if (_cboRecipient.SelectedIndex <= 0)
                {
                    MessageBox.Show("Vui lòng chọn người nhận!", "Thiếu thông tin",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                receiverID = _allUsers[_cboRecipient.SelectedIndex - 1].UserID;
            }

            try
            {
                var newAnnounce = new AnnouncementsDTO
                {
                    Title         = title,
                    Content       = content,
                    Category      = category,
                    TargetType    = targetType,
                    PriorityLevel = priority,
                    PublishDate   = DateTime.Now,
                    SenderID      = SessionManager.CurrentUser.UserID,
                    ReceiverID    = receiverID
                };

                bool ok = new AnnouncementsBUS().SendNotification(newAnnounce);
                if (ok)
                {
                    MessageBox.Show("Gửi thông báo thành công!", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearForm();
                    LoadData(_currentFilter);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Làm mới form soạn thảo ──
        private void btnReset_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtTitle.Clear();
            rtbContent.Clear();
            rdbPriorityNormal.Checked = true;
            cboTargetType.SelectedIndex = 0;
            cboCategory.SelectedIndex = 0;
        }

        private void ucAdminNotificationManager_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);

            // Làm sạch item rỗng trong cboTargetType
            cboTargetType.Items.Clear();
            cboTargetType.Items.AddRange(new object[] { "All", "Teacher", "Student", "Individual" });
            cboTargetType.SelectedIndex = 0;

            cboCategory.SelectedIndex = 0;

            SetActiveFilterButton(btnAddSchedule);
            LoadData("All");
        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
