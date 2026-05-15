using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkyEnglish.GUI
{
    public partial class ucDSSV : UserControl
    {
        DataTable dtStudents;
        private void InitMockData()
        {
            dtStudents = new DataTable();
            dtStudents.Columns.Add("STT");
            dtStudents.Columns.Add("MaHocVien");
            dtStudents.Columns.Add("HoTen");
            dtStudents.Columns.Add("Email");
            dtStudents.Columns.Add("SoDT");
            dtStudents.Columns.Add("GioiTinh");
            dtStudents.Columns.Add("NgaySinh");
            dtStudents.Columns.Add("KhoaHoc");
            dtStudents.Columns.Add("NgayDangKy");

            // Thêm dữ liệu mẫu
            dtStudents.Rows.Add("1", "HV001", "Nguyễn Minh Anh", "minhanh@gmail.com", "0912345678", "Nữ", "15/05/2005", "IELTS Foundation", "10/05/2026");
            dtStudents.Rows.Add("2", "HV002", "Lê Gia Bảo", "bao.legia@gmail.com", "0987654321", "Nam", "20/10/2004", "English Comm A2", "12/05/2026");
            dtStudents.Rows.Add("3", "HV003", "Trần Thu Thảo", "thao.tran@gmail.com", "0355123456", "Nữ", "05/12/2006", "Grammar Basic", "14/05/2026");
            dtStudents.Rows.Add("4", "HV004", "Phạm Hoàng Nam", "nam.pham@gmail.com", "0708999888", "Nam", "12/03/2003", "IELTS Foundation", "09/05/2026");
            dtStudents.Rows.Add("5", "HV005", "Vũ Phương Linh", "linh.vu@gmail.com", "0909123123", "Nữ", "25/08/2005", "English Comm A2", "13/05/2026");

            dgvStudents.DataSource = dtStudents;
            UpdateTotalCount(); // Cập nhật ô Tổng số
            ApplyGridStyle();   // Nhuộm màu xanh dương bro thích
        }

        private void UpdateTotalCount()
        {
            // Cập nhật con số vào ô txtTongSo ở góc dưới (ảnh image_693ed0.png)
            txtTotalStudents.Text = dgvStudents.Rows.Count.ToString();
        }
        public ucDSSV()
        {
            InitializeComponent();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            // Lọc ảo trên DataTable
            DataView dv = dtStudents.DefaultView;
            dv.RowFilter = string.Format("HoTen LIKE '%{0}%' OR MaHocVien LIKE '%{0}%' OR SoDT LIKE '%{0}%'", keyword);

            dgvStudents.DataSource = dv.ToTable();
            UpdateTotalCount();
        }



        private void ApplyGridStyle()
        {
            // 1. Chỉnh Theme về Default để mình có quyền ghi đè màu
            dgvStudents.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;

            // 2. Nhuộm màu Header (Màu xanh dương #2563EB)
            dgvStudents.ThemeStyle.HeaderStyle.BackColor = ColorTranslator.FromHtml("#2563EB");
            dgvStudents.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvStudents.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

            // 3. Nhuộm màu dòng được chọn (Màu xanh nhạt #EBF2FF)
          

            // 4. Các thiết lập bổ trợ cho xịn
            dgvStudents.ThemeStyle.RowsStyle.Height = 35; // Hàng cao ráo
            dgvStudents.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(248, 249, 250); // Dòng kẻ sọc nhạt

            // Dàn đều cột
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Bắt buộc gọi lệnh này để Guna vẽ lại theo màu mới
            dgvStudents.ColumnHeadersHeight = 40;
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void ucDSSV_Load(object sender, EventArgs e)
        {
            InitMockData();
        }
    }
}
