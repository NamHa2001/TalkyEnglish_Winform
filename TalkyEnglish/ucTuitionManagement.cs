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
    public partial class ucTuitionManagement : UserControl
    {

        private readonly TuitionBUS _tuitionBUS = new TuitionBUS();
        private List<TuitionDTO> _allTuitions = new List<TuitionDTO>(); // Chứa danh sách gốc để lọc
        private int _selectedEnrollmentID = 0; // Biến lưu tạm ID để xử lý thu tiền
        private decimal _selectedPrice = 0; // Biến lưu tạm số tiền
        public ucTuitionManagement()
        {
            InitializeComponent();
            dgvTuition.AutoGenerateColumns = false;

            // Mang phần Setup ComboBox vào thẳng đây để chắc chắn 100% nó được tạo
            cboStatus.Items.Add("Tất cả");
            cboStatus.Items.Add("Đã đóng");
            cboStatus.Items.Add("Chưa đóng");

            // Tạm thời tắt bắt sự kiện để lúc gán Index = 0 nó không bị lỗi chạy FilterData sớm
            cboStatus.SelectedIndexChanged -= cboStatus_SelectedIndexChanged;
            cboStatus.SelectedIndex = 0;
            cboStatus.SelectedIndexChanged += cboStatus_SelectedIndexChanged;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            FilterData();
        }

        private void ucTuitionManagement_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);

            LoadData();
  

            // 3. Tải dữ liệu lần đầu
        
        }

        public void LoadData()
        {
            // Lấy dữ liệu từ BUS
            _allTuitions = _tuitionBUS.GetAllTuitions();

            // Đổ lên DataGridView
            dgvTuition.DataSource = null; // Xóa kết nối cũ
            dgvTuition.DataSource = _allTuitions;

            // Tính tổng số người chưa đóng tiền
            int unpaidCount = _allTuitions.Count(t => t.PaymentStatus != "Paid" && t.PaymentStatus != "Đã đóng");
            txtTotalUnpaid.Text = unpaidCount.ToString() + " học viên";

            // Reset các ô nhập liệu bên phải
            ResetDetailPanel();
        }
        private void ResetDetailPanel()
        {
            txtFullName.Text = "";
            txtStudentCode.Text = "";
            txtCourseName.Text = "";
            txtEnrollmentDate.Text = "";
            txtPrice.Text = "";
            txtStatus.Text = "";
            btnMarkAsPaid.Enabled = false; // Khóa nút cập nhật lại
            _selectedEnrollmentID = 0;
        }

        private void dgvTuition_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvTuition.Rows[e.RowIndex];

            var idCell    = row.Cells["colEnrollmentID"].Value;
            var priceCell = row.Cells["colPrice"].Value;
            if (idCell == null || priceCell == null) return;

            if (!int.TryParse(idCell.ToString(), out int enrollId)) return;
            if (!decimal.TryParse(priceCell.ToString(), out decimal price)) return;

            _selectedEnrollmentID = enrollId;
            _selectedPrice        = price;

            txtStudentCode.Text   = row.Cells["colStudentCode"].Value?.ToString() ?? "";
            txtFullName.Text      = row.Cells["colFullName"].Value?.ToString()    ?? "";
            txtCourseName.Text    = row.Cells["colCourseName"].Value?.ToString()  ?? "";

            DateTime? date = row.Cells["colEnrollmentDate"].Value as DateTime?;
            txtEnrollmentDate.Text = date?.ToString("dd/MM/yyyy") ?? "";

            txtPrice.Text = _selectedPrice.ToString("N0") + " VNĐ";

            string status = row.Cells["colStatus"].Value?.ToString() ?? "";
            bool isPaid   = status == "Paid" || status == "Đã đóng";
            txtStatus.Text       = isPaid ? "Đã đóng" : "Chưa đóng";
            btnMarkAsPaid.Enabled = !isPaid;
        }

        private void btnMarkAsPaid_Click(object sender, EventArgs e)
        {
            if (_selectedEnrollmentID == 0)
            {
                MessageBox.Show("Vui lòng chọn một học viên từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Hỏi lại lần cuối cho chắc cốp
            DialogResult dialog = MessageBox.Show($"Xác nhận thu học phí {_selectedPrice.ToString("N0")} VNĐ của học viên {txtFullName.Text}?",
                                                  "Xác nhận thanh toán",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                // Gọi BUS xử lý
                string result = _tuitionBUS.ProcessPayment(_selectedEnrollmentID, _selectedPrice);

                if (result == "OK")
                {
                    MessageBox.Show("Cập nhật thanh toán thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); // Load lại bảng để dòng đó chuyển sang màu xanh / trạng thái Đã đóng
                }
                else
                {
                    MessageBox.Show(result, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void FilterData()
        {
            string keyword = txtSearch.Text.Trim().ToLower();
            string statusFilter = cboStatus.SelectedItem?.ToString();

            // Dùng LINQ để lọc danh sách gốc
            var filteredList = _allTuitions.Where(t =>
     (string.IsNullOrEmpty(keyword) ||
      (t.FullName != null && t.FullName.ToLower().Contains(keyword)) ||
      (t.StudentCode != null && t.StudentCode.ToLower().Contains(keyword)))
     &&
     (statusFilter == "Tất cả" ||
      (statusFilter == "Đã đóng" && (t.PaymentStatus == "Paid" || t.PaymentStatus == "Đã đóng")) ||
      (statusFilter == "Chưa đóng" && t.PaymentStatus != "Paid" && t.PaymentStatus != "Đã đóng"))
 ).ToList();

            // Gán lại vào DataGridView
            dgvTuition.DataSource = null;
            dgvTuition.DataSource = filteredList;
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterData();
        }

     

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ResetDetailPanel();
        }
    }
}
