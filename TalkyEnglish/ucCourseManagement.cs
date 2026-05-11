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
using Excel = Microsoft.Office.Interop.Excel;

namespace TalkyEnglish.GUI
{
    public partial class ucCourseManagement : UserControl
    {
        private readonly CourseBUS _courseBUS = new CourseBUS();
        public ucCourseManagement()
        {
            InitializeComponent();
        }
        // Hàm này để nạp dữ liệu lên bảng và cập nhật số lượng
        public void LoadData()
        {
            try
            {
                // 1. Lấy danh sách từ BUS
                var courses = _courseBUS.GetAllCourses();

                // 2. Đổ vào DataGridView
                dgvCourses.DataSource = courses;

                // 3. Cập nhật nhãn tổng số khóa học (lblTotalCourses)
                lblTotalCourses.Text = $"Tổng số khóa học: {courses.Count}";

                // 4. Định dạng lại cột Học phí (Price) cho đẹp nếu cần
                dgvCourses.Columns["Price"].DefaultCellStyle.Format = "N0"; // Hiện 2.500.000 thay vì 2500000
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        // Sự kiện khi User Control vừa được load lên


        private void txtSearchCourse_TextChanged(object sender, EventArgs e)
        {
            dgvCourses.DataSource = _courseBUS.SearchCourses(txtSearchCourse.Text);
        }

        private void ucCourseManagement_Load_1(object sender, EventArgs e)
        {
            InitFilter();
            LoadData();
        }

        private void btnAddCourse_Click(object sender, EventArgs e)
        {
            // 1. Khởi tạo Form chi tiết (Dùng Constructor không tham số - Thêm mới)
            frmCourseDetail frm = new frmCourseDetail();

            // 2. Mở Form dưới dạng Dialog (buộc người dùng xử lý xong mới quay lại được)
            // Sau khi Form đóng, nó sẽ trả về kết quả trong dr
            DialogResult dr = frm.ShowDialog();

            // 3. Nếu người dùng nhấn "Lưu" thành công (DialogResult.OK)
            if (dr == DialogResult.OK)
            {
                // Gọi lại hàm LoadData để cập nhật bảng ngay lập tức
                LoadData();
            }
        }

        private CourseDTO GetSelectedCourse()
        {
            if (dgvCourses.SelectedRows.Count > 0)
            {
                // Lấy đối tượng CourseDTO gắn liền với dòng đó
                return (CourseDTO)dgvCourses.SelectedRows[0].DataBoundItem;
            }
            return null;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // 1. Lấy dữ liệu dòng đang chọn
            CourseDTO selected = GetSelectedCourse();

            if (selected != null)
            {
                // 2. Mở Form Detail và truyền đối tượng sang (Dùng Constructor có tham số)
                frmCourseDetail frm = new frmCourseDetail(selected);

                // 3. Hiển thị Form và chờ kết quả lưu
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    LoadData(); // Load lại bảng sau khi sửa thành công
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khóa học trong danh sách để sửa!");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. Lấy khóa học đang chọn
            CourseDTO selected = GetSelectedCourse();

            if (selected != null)
            {
                // 2. Hiện hộp thoại xác nhận (tránh bấm nhầm)
                DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa khóa học '{selected.CourseName}' không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes)
                {
                    // 3. Gọi BUS để xóa
                    if (_courseBUS.DeleteCourse(selected.CourseID))
                    {
                        MessageBox.Show("Xóa khóa học thành công!");
                        LoadData(); // Load lại bảng
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa khóa học này (có thể đã có học viên đăng ký).");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một khóa học để xóa!");
            }
        }
        private void InitFilter()
        {
            // 1. Trình độ
            cboFilterLevel.Items.Clear();
            cboFilterLevel.Items.AddRange(new string[] { "Tất cả", "Cơ bản", "Trung cấp", "Nâng cao" });
            cboFilterLevel.SelectedIndex = 0;

            // 2. Trạng thái
            cboFilterStatus.Items.Clear();
            cboFilterStatus.Items.AddRange(new string[] { "Tất cả", "Đang mở", "Tạm đóng", "Đã kết thúc" });
            cboFilterStatus.SelectedIndex = 0;

            // 3. Giảng viên (Đảm bảo thứ tự gán Member trước DataSource)
            var instructors = _courseBUS.GetInstructors();
            instructors.Insert(0, new UserDTO { UserID = -1, FullName = "Tất cả giảng viên" });

            cboFilterInstructor.ValueMember = "UserID";
            cboFilterInstructor.DisplayMember = "FullName";
            cboFilterInstructor.DataSource = instructors;
            cboFilterInstructor.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            // Chặn Null lúc đang khởi tạo
            if (cboFilterInstructor.SelectedItem == null ||
                cboFilterLevel.SelectedItem == null ||
                cboFilterStatus.SelectedItem == null)
            {
                return;
            }

            try
            {
                string keyword = txtSearchCourse.Text.Trim();

                // Lấy ID giảng viên an toàn tuyệt đối
                int instructorId = -1;
                if (cboFilterInstructor.SelectedItem is UserDTO selectedUser)
                {
                    instructorId = selectedUser.UserID;
                }

                string level = cboFilterLevel.SelectedItem.ToString();
                string status = cboFilterStatus.SelectedItem.ToString();

                // MẸO: Lấy ngày từ DateTimePicker
                DateTime date = dtpFilterDate.Value;

                // Gọi hàm BUS
                var result = _courseBUS.FilterCourses(keyword, instructorId, level, status, date);

                dgvCourses.DataSource = null; // Clear nhẹ để tránh lỗi Binding
                dgvCourses.DataSource = result;
                lblTotalCourses.Text = $"Tổng số khóa học: {result.Count}";
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần: Console.WriteLine(ex.Message);
            }
        }

        private void cboFilterInstructor_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cboFilterLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void cboFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void dtpFilterDate_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem bảng có dữ liệu không
            if (dgvCourses.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo");
                return;
            }

            try
            {
                // 2. Khởi tạo các đối tượng Excel
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
                Excel._Worksheet worksheet = null;

                worksheet = workbook.ActiveSheet;
                worksheet.Name = "Danh sách Khóa học"; // Đổi tên Sheet cho đúng module

                // 3. Xuất Tiêu đề cột từ DataGridView sang Excel
                // Duyệt qua các cột, chỉ lấy những cột đang hiển thị (Visible)
                int excelCol = 1;
                for (int i = 0; i < dgvCourses.Columns.Count; i++)
                {
                    if (dgvCourses.Columns[i].Visible)
                    {
                        worksheet.Cells[1, excelCol] = dgvCourses.Columns[i].HeaderText;
                        // Làm đậm tiêu đề
                        worksheet.Cells[1, excelCol].Font.Bold = true;

                        // Kẻ viền cho tiêu đề (Optional - giúp file đẹp hơn)
                        worksheet.Cells[1, excelCol].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                        excelCol++;
                    }
                }

                // 4. Xuất Dữ liệu từng dòng
                for (int i = 0; i < dgvCourses.Rows.Count; i++)
                {
                    excelCol = 1;
                    for (int j = 0; j < dgvCourses.Columns.Count; j++)
                    {
                        if (dgvCourses.Columns[j].Visible)
                        {
                            // Lấy giá trị từ ô
                            var cellValue = dgvCourses.Rows[i].Cells[j].Value;

                            // Xử lý định dạng đặc biệt cho Học phí (Price) để tránh hiện khoa học 1E+06
                            if (dgvCourses.Columns[j].Name == "Price")
                            {
                                worksheet.Cells[i + 2, excelCol].NumberFormat = "#,##0";
                            }

                            worksheet.Cells[i + 2, excelCol] = cellValue != null ? cellValue.ToString() : "";
                            excelCol++;
                        }
                    }
                }

                // 5. Căn chỉnh độ rộng cột tự động cho đẹp
                worksheet.Columns.AutoFit();

                // 6. Hiển thị Excel để người dùng xem và lưu
                excelApp.Visible = true;

                MessageBox.Show("Xuất báo cáo danh sách khóa học thành công!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi");
            }
        }

        private void ResetFilters()
        {
            // 1. Xóa trắng ô tìm kiếm
            txtSearchCourse.Clear();

            // 2. Đưa các ComboBox về lựa chọn đầu tiên ("Tất cả")
            if (cboFilterInstructor.Items.Count > 0) cboFilterInstructor.SelectedIndex = 0;
            if (cboFilterLevel.Items.Count > 0) cboFilterLevel.SelectedIndex = 0;
            if (cboFilterStatus.Items.Count > 0) cboFilterStatus.SelectedIndex = 0;

            // 3. Đưa ngày về ngày hiện tại
            dtpFilterDate.Value = DateTime.Now;

            // 4. Nếu bạn có CheckBox lọc ngày, hãy bỏ tích nó
            // chkEnableDateFilter.Checked = false;

            // 5. Sau khi đưa về mặc định, gọi nạp lại toàn bộ dữ liệu
            LoadData();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetFilters();
        }
    }
}

