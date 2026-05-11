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
            SetupGrids();      // 1. Xây khung cột trước
            LoadData();        // 2. Đổ dữ liệu Giảng viên/Khóa học
            LoadAssignments(); // 3. Đổ dữ liệu danh sách đã phân công
        }

        private void SetupGrids()
        {
            // 1. KHÓA CHẶT: Không cho phép tự đẻ thêm cột mới
            dgvInstructors.AutoGenerateColumns = false;
            dgvCourses.AutoGenerateColumns = false;
            dgvAssignments.AutoGenerateColumns = false;

            // 2. KHỚP LỆNH: Nối dữ liệu vào đúng các cột bạn đã vẽ (theo Index)

            // --- Bảng Giảng Viên (Bạn vẽ 4 cột) ---
            if (dgvInstructors.Columns.Count >= 2)
            {
                dgvInstructors.Columns[0].DataPropertyName = "UserID";
                dgvInstructors.Columns[0].Name = "UserID"; // Để code tìm thấy khi click
                dgvInstructors.Columns[1].DataPropertyName = "FullName";
                dgvInstructors.Columns[1].Name = "FullName";

                if (dgvInstructors.Columns.Count > 2) dgvInstructors.Columns[2].DataPropertyName = "Specialization";
                if (dgvInstructors.Columns.Count > 3) dgvInstructors.Columns[3].DataPropertyName = "Degree";
            }

            // --- Bảng Khóa Học (Bạn vẽ 3 cột) ---
            if (dgvCourses.Columns.Count >= 2)
            {
                dgvCourses.Columns[0].DataPropertyName = "CourseID";
                dgvCourses.Columns[0].Name = "CourseID"; // Bắt buộc có để lấy ID khi lưu
                dgvCourses.Columns[1].DataPropertyName = "CourseName";
                if (dgvCourses.Columns.Count > 2) dgvCourses.Columns[2].DataPropertyName = "Level";
            }

            // --- Bảng Phân Công ---
            if (dgvAssignments.Columns.Count >= 3)
            {
                dgvAssignments.Columns[0].DataPropertyName = "AssignmentID";
                dgvAssignments.Columns[1].DataPropertyName = "InstructorName";
                dgvAssignments.Columns[2].DataPropertyName = "CourseName";
            }

            // 3. CHIÊU CUỐI: ẨN TOÀN BỘ CỘT THỪA (Chỉ thực hiện tại màn hình này)
            // Thuật toán: Nếu Index của cột lớn hơn số lượng cột bạn đã vẽ tay -> Ẩn nó đi.

            // Giả sử dgvInstructors bạn vẽ 4 cột (0,1,2,3)
            for (int i = 4; i < dgvInstructors.Columns.Count; i++) dgvInstructors.Columns[i].Visible = false;

            // Giả sử dgvCourses bạn vẽ 3 cột (0,1,2)
            for (int i = 3; i < dgvCourses.Columns.Count; i++) dgvCourses.Columns[i].Visible = false;

            // Giả sử dgvAssignments bạn vẽ 3 cột (0,1,2)
            for (int i = 3; i < dgvAssignments.Columns.Count; i++) dgvAssignments.Columns[i].Visible = false;
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

            // Lấy chính xác giá trị đang hiển thị trên ô
            int instructorId = Convert.ToInt32(dgvInstructors.CurrentRow.Cells["UserID"].Value);
            int courseId = Convert.ToInt32(dgvCourses.CurrentRow.Cells["CourseID"].Value);
            string instructorName = dgvInstructors.CurrentRow.Cells["FullName"].Value.ToString();

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
                    MessageBox.Show($"Thành công! Đã phân công cho {instructorName}");
                    LoadAssignments();
                }
            }
            catch (Exception ex)
            {
                // Nếu vẫn lỗi, nó sẽ hiện chi tiết "Inner Exception" mình đã thêm lúc nãy
                MessageBox.Show(ex.Message);
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
                                select new TeachingAssignmentDTO
                                {
                                    AssignmentID = a.AssignmentID,
                                    InstructorID = a.InstructorID,
                                    CourseID = a.CourseID,
                                    InstructorName = u.FullName,
                                    CourseName = c.CourseName,
                                    AssignedDate = a.AssignedDate,
                                    Note = a.Note
                                }).ToList();

                    dgvAssignments.DataSource = null; // Reset để nạp mới hoàn toàn
                    dgvAssignments.DataSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi load danh sách phân công: " + ex.Message);
            }
        }
    }
}
