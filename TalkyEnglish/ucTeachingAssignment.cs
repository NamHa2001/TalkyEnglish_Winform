using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;
using Excel = Microsoft.Office.Interop.Excel;
namespace TalkyEnglish.GUI
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class ucTeachingAssignment : UserControl
    {

        // Khai báo công cụ lấy dữ liệu
        private DAL.UserDAL _userDAL = new DAL.UserDAL();
        private DAL.CourseDAL _courseDAL = new CourseDAL();
        // Thêm dòng này vào
        private DAL.AssignmentDAL _assignmentDAL = new DAL.AssignmentDAL();

        public void LoadData()
        {
            dgvInstructors.AutoGenerateColumns = false;
            dgvCourses.AutoGenerateColumns = false;
            dgvAssignments.AutoGenerateColumns = false;
            dgvInstructors.DataSource = _userDAL.GetAllInstructors().ToList();
            dgvCourses.DataSource = _courseDAL.GetAllCourses().ToList();
        }
        public ucTeachingAssignment()
        {
            InitializeComponent();
            ConfigGridViews();
        }

        private void ucTeachingAssignment_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
            SetupGrids();      // 1. Xây khung cột trước
            LoadData();        // 2. Đổ dữ liệu Giảng viên/Khóa học
            LoadAssignments(); // 3. Đổ dữ liệu danh sách đã phân công
        }

        private void SetupGrids()
        {
            // Khóa chặt không cho tự sinh cột
            //dgvInstructors.AutoGenerateColumns = false;
            //dgvCourses.AutoGenerateColumns = false;
            //dgvAssignments.AutoGenerateColumns = false;

            // --- 1. Bảng Giảng viên (Khớp 100% thứ tự bạn đưa ra) ---
            if (dgvInstructors.Columns.Count >= 4)
            {
                dgvInstructors.Columns[0].DataPropertyName = "UserID";         // MaGV
                dgvInstructors.Columns[1].DataPropertyName = "FullName";       // Họ và tên
                dgvInstructors.Columns[2].DataPropertyName = "Specialization"; // Chuyên môn
                dgvInstructors.Columns[3].DataPropertyName = "Degree";         // Bằng cấp
            }

            // --- 2. Bảng Khóa học (Khớp 100% thứ tự bạn đưa ra) ---
            if (dgvCourses.Columns.Count >= 3)
            {
                dgvCourses.Columns[0].DataPropertyName = "CourseID";   // Mã KH
                dgvCourses.Columns[1].DataPropertyName = "CourseName"; // Tên KH
                dgvCourses.Columns[2].DataPropertyName = "Level";      // Trình độ
            }

            // --- 3. Bảng Phân công (Giữ nguyên logic 7 cột đã khớp) ---
            if (dgvAssignments.Columns.Count >= 7)
            {
                dgvAssignments.Columns[0].DataPropertyName = "AssignmentID";
                dgvAssignments.Columns[1].DataPropertyName = "InstructorID";
                dgvAssignments.Columns[2].DataPropertyName = "InstructorName";
                dgvAssignments.Columns[3].DataPropertyName = "CourseID";
                dgvAssignments.Columns[4].DataPropertyName = "CourseName";
                dgvAssignments.Columns[5].DataPropertyName = "AssignedDate";
                dgvAssignments.Columns[6].DataPropertyName = "Note";
            }

            // Ẩn các cột thừa nếu có
            for (int i = 4; i < dgvInstructors.Columns.Count; i++) dgvInstructors.Columns[i].Visible = false;
            for (int i = 3; i < dgvCourses.Columns.Count; i++) dgvCourses.Columns[i].Visible = false;
            for (int i = 7; i < dgvAssignments.Columns.Count; i++) dgvAssignments.Columns[i].Visible = false;
        }
        private void ConfigGridViews()
        {
            // 1. Cho phép cuộn ngang nếu nội dung quá dài
            dgvInstructors.ScrollBars = ScrollBars.Both;
            dgvCourses.ScrollBars = ScrollBars.Both;

            // 2. Làm đẹp tiêu đề (Font chữ đậm cho chuyên nghiệp)
            dgvInstructors.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvCourses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // 3. Chế độ chọn cả dòng để khi click vào là chọn luôn cả thầy cô/khóa học
            dgvInstructors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCourses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 4. Ngăn người dùng sửa trực tiếp trên bảng (quan trọng)
            dgvInstructors.ReadOnly = true;
            dgvCourses.ReadOnly = true;
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (dgvInstructors.CurrentRow == null || dgvCourses.CurrentRow == null) return;

            var instrCell  = dgvInstructors.CurrentRow.Cells["UserID"].Value;
            var courseCell = dgvCourses.CurrentRow.Cells["CourseID"].Value;
            if (instrCell == null || courseCell == null) return;
            if (!int.TryParse(instrCell.ToString(), out int instructorId)) return;
            if (!int.TryParse(courseCell.ToString(), out int courseId)) return;

            using (var db = new TalkyDbContext())
            {
                // --- BƯỚC KIỂM TRA TRÙNG (Logic mới) ---
                bool isExisted = db.TeachingAssignments.Any(a => a.InstructorID == instructorId && a.CourseID == courseId);

                if (isExisted)
                {
                    MessageBox.Show("Giảng viên này đã được phân công vào khóa học này rồi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Dừng lại, không cho lưu nữa
                }
                // ---------------------------------------

                var newAssignment = new TeachingAssignmentDTO
                {
                    InstructorID = instructorId,
                    CourseID = courseId,
                    Note = "Phân công mới",
                    AssignedDate = DateTime.Now
                };

                try
                {
                    if (_assignmentDAL.InsertAssignment(newAssignment))
                    {
                        MessageBox.Show("Phân công thành công!");
                        LoadAssignments(); // Cập nhật lại bảng bên dưới
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        private void LoadAssignments()
        {
            try
            {
                using (var db = new TalkyDbContext())
                {
                    var list = (from a in db.TeachingAssignments
                                join u in db.Users on a.InstructorID equals u.UserID
                                join c in db.Courses on a.CourseID equals c.CourseID
                                // Tìm trong hàm LoadAssignments, đoạn select new:
                                select new TeachingAssignmentDTO
                                {
                                    AssignmentID = a.AssignmentID,
                                    InstructorID = u.UserID,
                                    InstructorName = u.FullName,

                                    CourseID = c.CourseID,
                                    // SỬA DÒNG NÀY: Đảm bảo lấy CourseName từ bảng Courses (c)
                                    CourseName = c.CourseName,

                                    Note = a.Note,
                                    AssignedDate = a.AssignedDate
                                }).ToList();

                    dgvAssignments.DataSource = null;
                    dgvAssignments.DataSource = list;

                    // Đừng quên gọi lại SetupGrids() nếu bạn thấy cột bị xáo trộn
                    SetupGrids();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            UpdateTotalAssignments();
        }

        private void txtSearchInstructor_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearchInstructor.Text.ToLower();
            var fullList = _userDAL.GetAllInstructors(); // Lấy lại danh sách gốc

            var filteredList = fullList.Where(u =>
                (u.FullName?.ToLower().Contains(keyword) == true) ||
                u.UserID.ToString().Contains(keyword)
            ).ToList();

            dgvInstructors.DataSource = filteredList;
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearchCourse.Text.ToLower();
            var fullList = _courseDAL.GetAllCourses();

            var filteredList = fullList.Where(c =>
                (c.CourseName?.ToLower().Contains(keyword) == true) ||
                c.CourseID.ToString().Contains(keyword)
            ).ToList();

            dgvCourses.DataSource = filteredList;
        }

        private void UpdateTotalAssignments()
        {
            // Lấy số lượng dòng hiện có trong bảng phân công
            int count = dgvAssignments.Rows.Count;

            // Đổ con số này vào TextBox
            txtTotalAssignments.Text = count.ToString();

            // (Mẹo nhỏ) Để người dùng không tự ý sửa con số này tay:
            txtTotalAssignments.ReadOnly = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAssignments.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một dòng phân công để xóa!");
                return;
            }

            // Lấy đối tượng DTO đang được chọn ở dòng hiện tại
            var selectedAssignment = dgvAssignments.CurrentRow.DataBoundItem as TeachingAssignmentDTO;

            if (selectedAssignment != null)
            {
                int assignmentId = selectedAssignment.AssignmentID;
                string name = selectedAssignment.InstructorName;

                var confirm = MessageBox.Show($"Bạn có chắc muốn hủy phân công của {name} không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    if (_assignmentDAL.DeleteAssignment(assignmentId))
                    {
                        MessageBox.Show("Đã xóa thành công!");
                        LoadAssignments(); // Nạp lại bảng
                    }
                    else
                    {
                        MessageBox.Show("Xóa thất bại, vui lòng kiểm tra lại!");
                    }
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearchInstructor.Clear();
            txtSearchCourse.Clear();
            LoadData();
            LoadAssignments();
            MessageBox.Show("Dữ liệu đã được cập nhật mới nhất!");
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem bảng danh sách phân công có dữ liệu không
            if (dgvAssignments.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu phân công để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 2. Khởi tạo Excel
                Excel.Application excelApp = new Excel.Application();
                Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
                Excel._Worksheet worksheet = workbook.ActiveSheet;
                worksheet.Name = "BaoCaoPhanCong";

                // 3. Tạo tiêu đề lớn cho báo cáo (Merge cell)
                worksheet.Cells[1, 1] = "DANH SÁCH PHÂN CÔNG GIẢNG DẠY";
                Excel.Range titleRange = worksheet.get_Range("A1", "G1");
                titleRange.Merge();
                titleRange.Font.Size = 16;
                titleRange.Font.Bold = true;
                titleRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                // 4. Xuất tiêu đề cột từ DataGridView
                int colIndex = 1;
                for (int i = 0; i < dgvAssignments.Columns.Count; i++)
                {
                    if (dgvAssignments.Columns[i].Visible)
                    {
                        worksheet.Cells[3, colIndex] = dgvAssignments.Columns[i].HeaderText;
                        worksheet.Cells[3, colIndex].Font.Bold = true;
                        worksheet.Cells[3, colIndex].Interior.Color = ColorTranslator.ToOle(Color.LightGray);
                        worksheet.Cells[3, colIndex].Borders.Value = 1;
                        colIndex++;
                    }
                }

                // 5. Xuất dữ liệu từng dòng
                for (int i = 0; i < dgvAssignments.Rows.Count; i++)
                {
                    colIndex = 1;
                    for (int j = 0; j < dgvAssignments.Columns.Count; j++)
                    {
                        if (dgvAssignments.Columns[j].Visible)
                        {
                            var cellValue = dgvAssignments.Rows[i].Cells[j].Value;
                            worksheet.Cells[i + 4, colIndex] = cellValue != null ? cellValue.ToString() : "";
                            worksheet.Cells[i + 4, colIndex].Borders.Value = 1;
                            colIndex++;
                        }
                    }
                }

                // 6. Tự động căn chỉnh độ rộng cột
                worksheet.Columns.AutoFit();

                // 7. Hiển thị file Excel cho người dùng
                excelApp.Visible = true;

                MessageBox.Show("Xuất báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã chọn dòng nào ở bảng dưới chưa
            if (dgvAssignments.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một bản ghi phân công để sửa!");
                return;
            }

            // 2. Lấy đối tượng đang chọn
            var selected = dgvAssignments.CurrentRow.DataBoundItem as TeachingAssignmentDTO;

            if (selected != null)
            {
                // Ở đây mình ví dụ sửa Ghi chú nhanh bằng cách hiện InputBox 
                // (Nếu bạn có Form riêng hoặc TextBox thì thay vào chỗ này nhé)
                string currentNote = selected.Note;

                // Giả sử bạn muốn sửa nhanh cái Ghi chú
                // Bạn có thể dùng một Form nhỏ hoặc TextBox có sẵn trên giao diện của bạn
                // Mình sẽ tạm gọi một hàm giả định để lấy giá trị mới:
                string newNote = Microsoft.VisualBasic.Interaction.InputBox("Sửa ghi chú phân công:", "Cập nhật", currentNote);

                if (!string.IsNullOrEmpty(newNote))
                {
                    if (_assignmentDAL.UpdateAssignment(selected.AssignmentID, newNote, selected.AssignedDate))
                    {
                        MessageBox.Show("Cập nhật thành công!");
                        LoadAssignments(); // Nạp lại bảng để thấy thay đổi
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại!");
                    }
                }
            }
        }
    }
}
