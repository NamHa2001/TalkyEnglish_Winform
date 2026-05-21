using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucStudentGrades : UserControl
    {
        private readonly GradesBUS _gradesBUS = new GradesBUS();
        private List<GradesDTO> _courseList = new List<GradesDTO>();

        // Dùng property thay field để tránh NullReferenceException khi field-initializer chạy trước Session
        private int CurrentStudentID => SessionManager.CurrentUser?.UserID ?? 0;

        public ucStudentGrades()
        {
            InitializeComponent();
            // Chỉ đăng ký sự kiện CHƯA có trong Designer.cs
            // (dgvCourseList.SelectionChanged và btnPrint.Click đã được Designer đăng ký)
            dgvCourseList.RowPostPaint += dgvCourseList_RowPostPaint;
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) { }

        private void ucStudentGrades_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);

            // Ẩn bộ lọc, đẩy bảng danh sách khóa học lên sát header
            guna2Panel1.Visible = false;
            guna2Panel6.Location = new Point(
                guna2Panel6.Location.X,
                guna2Panel7.Location.Y + guna2Panel7.Height + 10);

            // Thêm cột STT ở vị trí đầu nếu chưa có (Designer không khai báo cột này)
            if (dgvCourseList.Columns["colSTT"] == null)
            {
                dgvCourseList.Columns.Insert(0, new DataGridViewTextBoxColumn
                {
                    Name     = "colSTT",
                    HeaderText = "STT",
                    Width    = 45,
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
            }

            dgvCourseList.AutoGenerateColumns = false;
            LoadDiemHocVienThucTe();
        }

        private void LoadDiemHocVienThucTe()
        {
            try
            {
                _courseList = _gradesBUS.GetGradesByStudentID(CurrentStudentID);

                dgvCourseList.DataSource = null;
                dgvCourseList.DataSource = _courseList;

                if (_courseList.Count > 0)
                {
                    dgvCourseList.Rows[0].Selected = true;
                    var firstGrade = dgvCourseList.Rows[0].DataBoundItem as GradesDTO;
                    if (firstGrade != null)
                        HienThiChiTietDiem(firstGrade);
                }
                else
                {
                    ClearChiTiet();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu điểm: " + ex.Message, "Thông báo lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCourseList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCourseList.SelectedRows.Count == 0) return;
            var grade = dgvCourseList.SelectedRows[0].DataBoundItem as GradesDTO;
            if (grade != null)
                HienThiChiTietDiem(grade);
        }

        private void HienThiChiTietDiem(GradesDTO grade)
        {
            txtChuyenCann.Text = grade.AttendanceScore?.ToString("0.0") ?? "0.0";
            txtBaiTapp.Text    = grade.MidTerm?.ToString("0.0")         ?? "0.0";
            txtDiemThii.Text   = grade.FinalTerm?.ToString("0.0")       ?? "0.0";
            txtdiemTB.Text     = grade.AverageScore?.ToString("0.0")    ?? "0.0";

            // Xếp loại + màu theo mức điểm
            string ranking = grade.Ranking ?? "—";
            txtRanking.Text      = ranking;
            txtRanking.ForeColor = ranking switch
            {
                "Giỏi"       => Color.FromArgb(22,  163,  74),   // xanh lá
                "Khá"        => Color.FromArgb(37,   99, 235),   // xanh dương
                "Trung Bình" => Color.FromArgb(234,  88,  12),   // cam
                "Yếu"        => Color.FromArgb(220,  38,  38),   // đỏ
                _            => Color.Gray
            };

            txtTeacherComment.Text = grade.Note ?? "Chưa có nhận xét từ giảng viên.";
            if (lblTeacherName  != null) lblTeacherName.Text  = "Giáo viên: " + (grade.TeacherName ?? "—");
            if (lblCommentDate  != null) lblCommentDate.Text  = "Ngày nhận xét: " + (grade.CommentDate?.ToString("dd/MM/yyyy") ?? "Chưa ghi nhận");
        }

        private void ClearChiTiet()
        {
            txtChuyenCann.Text   = "—";
            txtBaiTapp.Text      = "—";
            txtDiemThii.Text     = "—";
            txtdiemTB.Text       = "—";
            txtRanking.Text      = "—";
            txtRanking.ForeColor = Color.Gray;
            txtTeacherComment.Text = "Chưa có dữ liệu điểm.";
            if (lblTeacherName  != null) lblTeacherName.Text  = "Giáo viên nhận xét:";
            if (lblCommentDate  != null) lblCommentDate.Text  = "Ngày nhận xét:";
        }

        private void dgvCourseList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (dgvCourseList.Columns["colSTT"] != null)
                dgvCourseList.Rows[e.RowIndex].Cells["colSTT"].Value = (e.RowIndex + 1).ToString();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_courseList.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu điểm nào để xuất!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var saveDialog = new SaveFileDialog
            {
                Filter   = "Excel/TSV (*.xlsx)|*.xlsx|Tab-separated (*.tsv)|*.tsv",
                FileName = $"BangDiem_{CurrentStudentID}_{DateTime.Now:yyyyMMdd}"
            };

            if (saveDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                using var sw = new System.IO.StreamWriter(saveDialog.FileName, false, Encoding.UTF8);
                sw.WriteLine("STT\tTên Khóa Học\tGiáo Viên\tHọc Kỳ\tĐiểm TB\tXếp Loại");

                for (int i = 0; i < _courseList.Count; i++)
                {
                    var g = _courseList[i];
                    sw.WriteLine($"{i + 1}\t{g.CourseName}\t{g.TeacherName}\t{g.Semester}\t{g.AverageScore:0.0}\t{g.Ranking}");
                }

                MessageBox.Show("Xuất bảng điểm thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo(saveDialog.FileName) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi hệ thống",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
