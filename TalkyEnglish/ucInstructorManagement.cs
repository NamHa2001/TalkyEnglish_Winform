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
    public partial class ucInstructorManagement : UserControl
    {
        UserBUS _userBUS = new UserBUS();
        private BindingSource instructorBindingSource = new BindingSource();
        List<UserDTO> _originalList = new List<UserDTO>();
        public ucInstructorManagement()
        {
            InitializeComponent();
        }

        public void LoadData()
        {
            try
            {
                // 1. KHÓA CHẶT tính năng tự sinh cột (Phải đặt TRƯỚC khi gán DataSource)
                dgvInstructors.AutoGenerateColumns = false;

                // 2. Lấy dữ liệu Giảng viên và đồng bộ Khóa học phụ trách bằng LINQ
                using (var db = new TalkyEnglish.DAL.TalkyDbContext())
                {
                    // Lấy danh sách gốc từ BUS
                    var instructors = _userBUS.GetAllInstructors();

                    // Lấy danh sách phân công (Join với bảng Courses để lấy tên khóa học)
                    var assignments = (from a in db.TeachingAssignments
                                       join c in db.Courses on a.CourseID equals c.CourseID
                                       select new { a.InstructorID, c.CourseName }).ToList();

                    // Khớp tên khóa học vào từng giảng viên (Gộp thành chuỗi: "Lớp A, Lớp B")
                    foreach (var ins in instructors)
                    {
                        var courseNames = assignments.Where(a => a.InstructorID == ins.UserID)
                                                     .Select(a => a.CourseName);

                        // Thuộc tính AssignedCourses này bạn cần thêm vào UserDTO.cs nhé
                        ins.AssignedCourses = string.Join(", ", courseNames);
                    }

                    _originalList = instructors;
                }

                // 3. Gán dữ liệu vào BindingSource
                instructorBindingSource.DataSource = _originalList;
                dgvInstructors.DataSource = instructorBindingSource;

                // 4. DIỆT TẬN GỐC: Xóa các cột thừa tự sinh (như StudentCode, PasswordHash, Role...)
                // Chỉ giữ lại những cột bạn đã định nghĩa thủ công trong Design
                for (int i = dgvInstructors.Columns.Count - 1; i >= 0; i--)
                {
                    // Nếu cột đó không có DataPropertyName (cột rác) hoặc nằm ngoài danh sách bạn vẽ
                    // Bạn có thể kiểm tra theo Name hoặc Index của các cột bạn đã tạo
                    if (string.IsNullOrEmpty(dgvInstructors.Columns[i].DataPropertyName))
                    {
                        // Nếu bạn không muốn xóa, có thể dùng .Visible = false;
                        dgvInstructors.Columns[i].Visible = false;
                    }

                    // Ẩn các cột nhạy cảm hoặc cột rác từ DTO
                    string colName = dgvInstructors.Columns[i].Name;
                    if (colName == "PasswordHash" || colName == "Role" || colName == "StudentCode")
                    {
                        dgvInstructors.Columns[i].Visible = false;
                    }
                }

                instructorBindingSource.ResetBindings(false);
                dgvInstructors.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách giảng viên: " + ex.Message);
            }
        }

        private void ucInstructorManagement_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
            LoadData();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem người dùng đã chọn dòng nào trên bảng chưa
            if (dgvInstructors.CurrentRow != null)
            {
                // 2. Lấy đối tượng UserDTO từ dòng đang chọn (Ép kiểu từ DataBoundItem)
                UserDTO selectedInstructor = (UserDTO)dgvInstructors.CurrentRow.DataBoundItem;

                // 3. Khởi tạo Form chi tiết và truyền đối tượng này vào Constructor
                // Đây chính là lúc Constructor có tham số (SỬA) mà ta viết nãy phát huy tác dụng
                frmInstructorDetail frm = new frmInstructorDetail(selectedInstructor);

                // 4. Hiện Form lên dưới dạng Dialog (khóa màn hình chính cho đến khi đóng Form)
                frm.ShowDialog();

                // 5. Sau khi đóng Form, load lại dữ liệu để cập nhật những gì vừa sửa lên bảng
                LoadData();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một giảng viên trong danh sách để sửa!", "Thông báo");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1. Khởi tạo Form chi tiết bằng Constructor KHÔNG tham số (Chế độ THÊM)
            frmInstructorDetail frm = new frmInstructorDetail();

            // 2. Hiện Form lên
            frm.ShowDialog();

            // 3. Sau khi đóng Form, load lại dữ liệu để bảng hiện thêm người mới (nếu có)
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim().ToLower();

            // Lọc danh sách gốc dựa trên từ khóa
            var filteredList = _originalList.Where(u =>
                (u.FullName != null && u.FullName.ToLower().Contains(keyword)) ||
                (u.Email != null && u.Email.ToLower().Contains(keyword)) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(keyword))
            ).ToList();

            // Cập nhật lại BindingSource với danh sách đã lọc
            instructorBindingSource.DataSource = filteredList;

            // Làm mới bảng
            dgvInstructors.Refresh();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã chọn giảng viên nào chưa
            if (dgvInstructors.CurrentRow != null)
            {
                // 2. Lấy đối tượng đang chọn
                UserDTO selectedInstructor = (UserDTO)dgvInstructors.CurrentRow.DataBoundItem;

                // 3. Hiển thị hộp thoại xác nhận (Cực kỳ quan trọng)
                DialogResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa giảng viên '{selectedInstructor.FullName}' không?\nDữ liệu đã xóa sẽ không thể khôi phục!",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        // 4. Gọi BUS để thực hiện xóa
                        bool isDeleted = _userBUS.DeleteInstructor(selectedInstructor.UserID);

                        if (isDeleted)
                        {
                            MessageBox.Show("Xóa giảng viên thành công!", "Thông báo");
                            // 5. Load lại dữ liệu để bảng cập nhật ngay lập tức
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Xóa thất bại. Giảng viên có thể không còn tồn tại.", "Lỗi");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message, "Lỗi hệ thống");
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn giảng viên cần xóa trong danh sách!", "Thông báo");
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem bảng có dữ liệu không
            if (dgvInstructors.Rows.Count == 0)
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
                worksheet.Name = "Danh sách Giảng viên";

                // 3. Xuất Tiêu đề cột từ DataGridView sang Excel
                // Chúng ta chỉ xuất những cột đang hiển thị (Visible)
                int excelCol = 1;
                for (int i = 0; i < dgvInstructors.Columns.Count; i++)
                {
                    if (dgvInstructors.Columns[i].Visible)
                    {
                        worksheet.Cells[1, excelCol] = dgvInstructors.Columns[i].HeaderText;
                        // Làm đậm tiêu đề
                        worksheet.Cells[1, excelCol].Font.Bold = true;
                        excelCol++;
                    }
                }

                // 4. Xuất Dữ liệu từng dòng
                for (int i = 0; i < dgvInstructors.Rows.Count; i++)
                {
                    excelCol = 1;
                    for (int j = 0; j < dgvInstructors.Columns.Count; j++)
                    {
                        if (dgvInstructors.Columns[j].Visible)
                        {
                            // Lấy giá trị từ ô
                            var cellValue = dgvInstructors.Rows[i].Cells[j].Value;
                            worksheet.Cells[i + 2, excelCol] = cellValue != null ? cellValue.ToString() : "";
                            excelCol++;
                        }
                    }
                }

                // 5. Căn chỉnh độ rộng cột tự động cho đẹp
                worksheet.Columns.AutoFit();

                // 6. Hiển thị Excel để người dùng xem và lưu
                excelApp.Visible = true;

                MessageBox.Show("Xuất báo cáo Excel thành công!", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất Excel: " + ex.Message, "Lỗi");
            }
        }
        private void ApplyFilter()
        {
            if (_originalList == null) return;

            string keyword = txtSearch.Text.Trim().ToLower();
            string spec = cboFilterSpecialization.Text;
            string degree = cboFilterDegree.Text;
            string status = cboFilterStatus.Text;
            DateTime filterDate = dtpFilterDate.Value.Date; // Lấy phần ngày, bỏ phần giờ

            var filtered = _originalList.Where(u =>
            {
                // 1. Lọc theo từ khóa
                bool matchesKeyword = string.IsNullOrEmpty(keyword) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(keyword)) ||
                    (u.Email != null && u.Email.ToLower().Contains(keyword));

                // 2. Lọc theo Chuyên môn (Bỏ qua nếu chọn "Tất cả")
                bool matchesSpec = spec == "Tất cả" || string.IsNullOrEmpty(spec) || u.Specialization == spec;

                // 3. Lọc theo Trình độ
                bool matchesDegree = degree == "Tất cả" || string.IsNullOrEmpty(degree) || u.Degree == degree;

                // 4. Lọc theo Trạng thái
                bool matchesStatus = status == "Tất cả" || string.IsNullOrEmpty(status) || u.Status == status;

                // 5. Lọc theo Ngày tham gia (Lấy những người tham gia >= ngày đã chọn)
                bool matchesDate = u.CreatedAt.HasValue && u.CreatedAt.Value.Date >= filterDate;

                return matchesKeyword && matchesSpec && matchesDegree && matchesStatus && matchesDate;
            }).ToList();

            instructorBindingSource.DataSource = filtered;
            dgvInstructors.Refresh();
        }
        private void cboFilterSpecialization_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void cboFilterDegree_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void cboFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void dtpFilterDate_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Đưa tất cả về mặc định
            txtSearch.Clear();
            cboFilterSpecialization.SelectedIndex = 0; // Giả sử mục 0 là "Tất cả"
            cboFilterDegree.SelectedIndex = 0;
            cboFilterStatus.SelectedIndex = 0;
            dtpFilterDate.Value = new DateTime(2020, 1, 1); // Đưa về một ngày xa trong quá khứ để hiện hết

            // Gọi lại hàm lọc để hiển thị lại toàn bộ
            ApplyFilter();
        }
    }

}
