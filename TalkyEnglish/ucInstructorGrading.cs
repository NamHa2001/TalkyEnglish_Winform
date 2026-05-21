using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucInstructorGrading : UserControl
    {
        private readonly GradesBUS _gradesBUS = new GradesBUS();
        private int _selectedCourseId = 0;
        private Label _lblStudentName;

        public ucInstructorGrading()
        {
            InitializeComponent();
            // Chỉ đăng ký các sự kiện CHƯA có trong Designer.cs
            // (btnLuu.Click, SelectionChanged, cboLopHoc.SelectedIndexChanged đã được Designer đăng ký)
            txtChuyenCann.TextChanged += TinhDiemTrungBinh_RealTime;
            txtBaiTapp.TextChanged    += TinhDiemTrungBinh_RealTime;
            txtDiemThii.TextChanged   += TinhDiemTrungBinh_RealTime;
            guna2DataGridView1.RowPostPaint += guna2DataGridView1_RowPostPaint;
        }

        private void guna2DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            guna2DataGridView1.Rows[e.RowIndex].Cells["ColSTT"].Value = (e.RowIndex + 1).ToString();
        }

        private void ucInstructorGrading_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);

            // Nhãn tên học viên đang được chọn (thêm vào guna2Panel2 bên phải label3)
            _lblStudentName = new Label
            {
                Text      = "(Chọn học viên từ danh sách)",
                Location  = new Point(160, 7),
                Size      = new Size(205, 18),
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(37, 99, 235),
                TextAlign = ContentAlignment.MiddleLeft
            };
            guna2Panel2.Controls.Add(_lblStudentName);

            // Điểm trung bình chỉ để hiển thị, không cho sửa tay
            txtdiemTB.ReadOnly  = true;
            txtdiemTB.FillColor = Color.FromArgb(245, 245, 245);

            // Ánh xạ cột DataGridView
            ColSTT.DataPropertyName      = "";
            ColMaHV.DataPropertyName     = "GradeID";
            ColTenHV.DataPropertyName    = "TeacherName";
            ColTenKH.DataPropertyName    = "CourseName";
            ColTinhTrang.DataPropertyName = "Semester";

            // Học kỳ (dùng để lọc theo kỳ trong tương lai)
            CboHocki.Items.Clear();
            CboHocki.Items.Add("Tất cả");
            CboHocki.Items.Add("Học kỳ 1");
            CboHocki.Items.Add("Học kỳ 2");
            CboHocki.SelectedIndex = 0;

            try
            {
                int instructorId = SessionManager.CurrentUser?.UserID ?? 0;
                CourseBUS courseBUS = new CourseBUS();
                var courses = courseBUS.GetCoursesByInstructor(instructorId);

                cboLopHoc.DataSource    = courses;
                cboLopHoc.ValueMember   = "CourseID";
                cboLopHoc.DisplayMember = "CourseName";

                LoadDanhSachHocVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo dữ liệu: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDanhSachHocVien()
        {
            if (cboLopHoc.SelectedValue == null) return;
            if (!int.TryParse(cboLopHoc.SelectedValue.ToString(), out int courseId)) return;

            try
            {
                _selectedCourseId = courseId;
                var listGrades = _gradesBUS.GetStudentGradesByCourse(_selectedCourseId);
                guna2DataGridView1.AutoGenerateColumns = false;
                guna2DataGridView1.DataSource = listGrades;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboLopHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDanhSachHocVien();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void TinhDiemTrungBinh_RealTime(object sender, EventArgs e)
        {
            CapNhatHienThiDTB();
        }

        private void CapNhatHienThiDTB()
        {
            double cc  = double.TryParse(txtChuyenCann.Text, out double v1) ? v1 : 0;
            double bt  = double.TryParse(txtBaiTapp.Text,   out double v2) ? v2 : 0;
            double thi = double.TryParse(txtDiemThii.Text,  out double v3) ? v3 : 0;
            txtdiemTB.Text = ((cc * 0.1) + (bt * 0.3) + (thi * 0.6)).ToString("0.0");
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow == null) return;

            var selectedRow = guna2DataGridView1.CurrentRow.DataBoundItem as GradesDTO;
            if (selectedRow == null) return;

            try
            {
                bool ccOk  = double.TryParse(txtChuyenCann.Text, out double cc);
                bool btOk  = double.TryParse(txtBaiTapp.Text,    out double bt);
                bool thiOk = double.TryParse(txtDiemThii.Text,   out double thi);

                if (!ccOk || !btOk || !thiOk || cc < 0 || cc > 10 || bt < 0 || bt > 10 || thi < 0 || thi > 10)
                {
                    MessageBox.Show(
                        "Điểm phải là số trong khoảng 0 – 10.\n\n" +
                        $"• Chuyên cần: {txtChuyenCann.Text}\n" +
                        $"• Bài tập:    {txtBaiTapp.Text}\n" +
                        $"• Thi:        {txtDiemThii.Text}",
                        "Điểm không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                double avg = (cc * 0.1) + (bt * 0.3) + (thi * 0.6);
                string studentName = selectedRow.TeacherName ?? "học viên";

                var confirm = MessageBox.Show(
                    $"Xác nhận lưu điểm cho \"{studentName}\"?\n\n" +
                    $"Chuyên cần: {cc:0.0}   Bài tập: {bt:0.0}   Thi: {thi:0.0}\n" +
                    $"Điểm trung bình: {avg:0.0}",
                    "Xác nhận lưu điểm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                var gradeToSave = new GradesDTO
                {
                    EnrolmentID     = selectedRow.EnrolmentID,
                    AttendanceScore = cc,
                    MidTerm         = bt,
                    FinalTerm       = thi,
                    AverageScore    = avg,
                    Note            = txtNhanxet.Text,
                    CommentDate     = DateTime.Now
                };

                bool isSuccess = _gradesBUS.SaveOrUpdateGrade(gradeToSave);
                if (isSuccess)
                {
                    MessageBox.Show("Đã lưu điểm thành công!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachHocVien();
                }
                else
                {
                    MessageBox.Show("Lưu điểm thất bại. Vui lòng kiểm tra lại.", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Thông báo lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow == null) return;

            var selectedGrade = guna2DataGridView1.CurrentRow.DataBoundItem as GradesDTO;
            if (selectedGrade == null) return;

            txtChuyenCann.Text = selectedGrade.AttendanceScore?.ToString() ?? "0";
            txtBaiTapp.Text    = selectedGrade.MidTerm?.ToString()         ?? "0";
            txtDiemThii.Text   = selectedGrade.FinalTerm?.ToString()       ?? "0";
            txtNhanxet.Text    = selectedGrade.Note ?? "";
            CapNhatHienThiDTB();

            if (_lblStudentName != null)
                _lblStudentName.Text = "HV: " + (selectedGrade.TeacherName ?? "");
        }

        private void guna2Panel7_Paint(object sender, PaintEventArgs e) { }
    }
}
