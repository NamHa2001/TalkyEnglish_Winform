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
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;
using Excel = Microsoft.Office.Interop.Excel;
namespace TalkyEnglish.GUI
{
    public partial class ucStudentManagement : UserControl
    {
        private readonly UserDAL _userDAL = new UserDAL();
        // 1. Hàm nạp dữ liệu lên bảng
        private void LoadStudentData()
        {
            List<UserDTO> students = _userDAL.GetAllStudents();
            dgvStudents.AutoGenerateColumns = false;
            dgvStudents.DataSource = students;

            // --- THÊM ĐOẠN ẨN CỘT NÀY VÀO ---
            string[] columnsToHide = { "Specialization", "Degree", "PasswordHash", "Role" };
            foreach (string colName in columnsToHide)
            {
                if (dgvStudents.Columns.Contains(colName))
                    dgvStudents.Columns[colName].Visible = false;
            }
            UpdateStudentCount();
        }
        public ucStudentManagement()
        {
            InitializeComponent();

        }

        private void dgvStudents_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void ucStudentManagement_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
            InitFilters();
            LoadStudentData();
        }

        private void InitFilters()
        {
            // 1. Nạp dữ liệu Trạng thái (Fix cứng theo nghiệp vụ)
            cboStatus.Items.Clear();
            cboStatus.Items.Add("Tất cả");
            cboStatus.Items.Add("Đang học");
            cboStatus.Items.Add("Bảo lưu");
            cboStatus.Items.Add("Đã nghỉ");
            cboStatus.SelectedIndex = 0;

            // 2. Nạp dữ liệu Trình độ (Fix cứng hoặc lấy từ DB tùy bạn, ở đây tôi ví dụ fix cứng)
            cboLevel.Items.Clear();
            cboLevel.Items.Add("Tất cả");
            cboLevel.Items.Add("Cơ bản");
            cboLevel.Items.Add("Trung cấp");
            cboLevel.Items.Add("Nâng cao");
            cboLevel.SelectedIndex = 0;

            // 3. Nạp danh sách Khóa học từ Database
            CourseDAL courseDAL = new CourseDAL();
            var courses = courseDAL.GetAllCourses();
            cboCourseStudent.DataSource = null;

            // Tạo một danh sách mới có mục "Tất cả" ở đầu
            var comboItems = new List<string> { "Tất cả" };
            comboItems.AddRange(courses.Select(c => c.CourseName).ToList());

            cboCourseStudent.DataSource = comboItems;
            cboCourseStudent.SelectedIndex = 0;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // 1. Xóa nội dung ô tìm kiếm
            txtSearch.Clear();

            // 2. Đưa các bộ lọc về trạng thái "Tất cả" (SelectedIndex = 0)
            // Sử dụng tên mới cboCourseFilter để tránh lỗi
            if (cboCourseStudent.Items.Count > 0) cboCourseStudent.SelectedIndex = 0;
            if (cboLevel.Items.Count > 0) cboLevel.SelectedIndex = 0;
            if (cboStatus.Items.Count > 0) cboStatus.SelectedIndex = 0;

            // 3. Đặt lại khoảng thời gian (ví dụ: mặc định xem từ 1 tháng trước đến hiện tại)
            dtpFromDate.Value = DateTime.Now.AddMonths(-1);

            // 4. Gọi lại hàm load dữ liệu để làm mới bảng
            LoadStudentData();
        }
        private void PerformSearch()
        {
            string keyword = txtSearch.Text.Trim();
            string course = cboCourseStudent.Text;
            string level = cboLevel.Text;
            string status = cboStatus.Text;

            // Gọi DAL lấy dữ liệu đã lọc
            var filteredData = _userDAL.SearchStudentsAdvanced(keyword, course, level, status);
            dgvStudents.DataSource = filteredData;

            // --- PHẢI ẨN LẠI CỘT Ở ĐÂY NỮA ---
            string[] columnsToHide = { "Specialization", "Degree", "PasswordHash", "Role" };
            foreach (string colName in columnsToHide)
            {
                if (dgvStudents.Columns.Contains(colName))
                    dgvStudents.Columns[colName].Visible = false;
            }
        }

        private void cboCourseStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void cboLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void cboStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void dtpFromDate_ValueChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Mở Form chi tiết ở chế độ Thêm mới
            frmStudentDetail frm = new frmStudentDetail();

            // Nếu người dùng bấm Lưu thành công (DialogResult.OK)
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadStudentData(); // Load lại bảng cho nó cập nhật
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem có đang chọn dòng nào không
            if (dgvStudents.CurrentRow != null)
            {
                // 2. Lấy đối tượng học viên từ dòng đang chọn
                UserDTO selectedStudent = (UserDTO)dgvStudents.CurrentRow.DataBoundItem;

                // 3. Mở Form và truyền học viên đó sang
                frmStudentDetail frm = new frmStudentDetail(selectedStudent);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadStudentData(); // Cập nhật lại bảng sau khi sửa
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một học viên để sửa!", "Thông báo");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã chọn học viên nào trên bảng chưa
            if (dgvStudents.CurrentRow != null)
            {
                // 2. Lấy đối tượng học viên đang chọn
                UserDTO selectedStudent = (UserDTO)dgvStudents.CurrentRow.DataBoundItem;

                // 3. Hiện hộp thoại xác nhận (để tránh xóa nhầm)
                DialogResult confirm = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa học viên: {selectedStudent.FullName} không?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        // 4. Gọi DAL hoặc BUS để thực hiện xóa
                        // Ở đây mình dùng luôn _userDAL như bạn đang dùng ở các hàm trên cho nhanh
                        bool result = _userDAL.DeleteUser(selectedStudent.UserID);

                        if (result)
                        {
                            MessageBox.Show("Xóa học viên thành công!");
                            LoadStudentData(); // Load lại bảng để cập nhật danh sách mới
                        }
                        else
                        {
                            MessageBox.Show("Xóa thất bại. Vui lòng thử lại!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một học viên trong danh sách để xóa!");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvStudents.Rows.Count > 0)
            {
                // 1. Khởi tạo ứng dụng Excel
                Excel.Application excelApp = new Excel.Application();
                excelApp.Application.Workbooks.Add(Type.Missing);

                // 2. Tạo tiêu đề cột (Lấy các cột đang hiển thị trên DataGridView)
                int excelColumnIndex = 1;
                for (int i = 1; i < dgvStudents.Columns.Count + 1; i++)
                {
                    if (dgvStudents.Columns[i - 1].Visible)
                    {
                        excelApp.Cells[1, excelColumnIndex] = dgvStudents.Columns[i - 1].HeaderText;
                        excelApp.Cells[1, excelColumnIndex].Font.Bold = true; // In đậm tiêu đề
                        excelApp.Cells[1, excelColumnIndex].Interior.Color = Color.LightGray; // Tô nền
                        excelColumnIndex++;
                    }
                }

                // 3. Đổ dữ liệu từ bảng vào Excel
                for (int i = 0; i < dgvStudents.Rows.Count; i++)
                {
                    excelColumnIndex = 1;
                    for (int j = 0; j < dgvStudents.Columns.Count; j++)
                    {
                        if (dgvStudents.Columns[j].Visible)
                        {
                            // Lấy giá trị và gán vào cell
                            var cellValue = dgvStudents.Rows[i].Cells[j].Value;
                            excelApp.Cells[i + 2, excelColumnIndex] = cellValue != null ? cellValue.ToString() : "";
                            excelColumnIndex++;
                        }
                    }
                }

                // 4. Căn chỉnh tự động các cột cho đẹp
                excelApp.Columns.AutoFit();

                // 5. Hiển thị Excel lên để người dùng tự lưu (hoặc dùng SaveFileDialog như các phần trước)
                excelApp.Visible = true;
            }
            else
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void UpdateStudentCount()
        {
            // Giả sử tên TextBox/Label của bạn là txtTotalStudents hoặc lblTotalStudents
            // Ở đây mình dùng dgvStudents.Rows.Count để đếm
            int count = dgvStudents.Rows.Count;
            txtTotalStudents.Text = count.ToString();
        }
    }
}
