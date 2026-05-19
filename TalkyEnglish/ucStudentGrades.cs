    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using TalkyEnglish.DTO;
using TalkyEnglish.BUS; // Chèn dòng này vào trên cùng file ucStudentGrades.cs nha bro
namespace TalkyEnglish.GUI
    {
    public partial class ucStudentGrades : UserControl
    {

        private readonly GradesBUS _gradesBUS = new GradesBUS();
        private List<GradesDTO> _courseList = new List<GradesDTO>();

        // 🌟 BIẾN SESSION ĐĂNG NHẬP: Bro thay thế số 2 này bằng biến Static User 
        // chứa ID tài khoản đang đăng nhập của dự án nhóm bro nha!

        // Ví dụ tên class Session nhóm bro là "GlobalUser" hoặc "Session"
        // ĐỔI THÀNH DÒNG NÀY NHA BRO
        private int _currentStudentID = SessionManager.CurrentUser.UserID;
        public ucStudentGrades()
        {
            InitializeComponent();
            dgvCourseList.RowPostPaint += dgvCourseList_RowPostPaint;
            dgvCourseList.SelectionChanged += dgvCourseList_SelectionChanged;

            // Đăng ký sự kiện Click cho nút "In kết quả" (btnPrint)
            btnPrint.Click += btnPrint_Click;
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }





        private void dgvCourseList_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvCourseList.SelectedRows.Count > 0)
            {
                var selectedGrade = dgvCourseList.SelectedRows[0].DataBoundItem as GradesDTO;
                if (selectedGrade != null)
                {
                    HienThiChiTietDiem(selectedGrade);
                }
            }
        }

        private void ucStudentGrades_Load(object sender, EventArgs e)
        {
            guna2Panel1.Visible = false;

            // Đẩy bảng danh sách khóa học (guna2Panel6) dịch lên trên một chút cho đẹp
            guna2Panel6.Location = new System.Drawing.Point(guna2Panel6.Location.X, guna2Panel7.Location.Y + guna2Panel7.Height + 10);

            // 2. MAPPING DỮ LIỆU VÀO CÁC CỘT TRÊN DATAGRIDVIEW CỦA BRO
            dgvCourseList.AutoGenerateColumns = false;
            if (dgvCourseList.Columns["colCourseName"] != null) dgvCourseList.Columns["colCourseName"].DataPropertyName = "CourseName";
            if (dgvCourseList.Columns["colTeacherName"] != null) dgvCourseList.Columns["colTeacherName"].DataPropertyName = "TeacherName";
            if (dgvCourseList.Columns["colSemester"] != null) dgvCourseList.Columns["colSemester"].DataPropertyName = "Semester";
            if (dgvCourseList.Columns["colAvgScore"] != null) dgvCourseList.Columns["colAvgScore"].DataPropertyName = "AverageScore";

            // 3. TẢI DỮ LIỆU THỰC TẾ TỪ SQL SERVER
            LoadDiemHocVienThucTe();
        }

        private void LoadDiemHocVienThucTe()
        {
            try
            {
                // Gọi xuống tầng BUS để lấy danh sách kết quả học tập theo ID học viên
                _courseList = _gradesBUS.GetGradesByStudentID(_currentStudentID);

                dgvCourseList.DataSource = null;
                dgvCourseList.DataSource = _courseList;

                // Tự động chọn dòng đầu tiên để hiển thị chi tiết khi vừa load bảng xong
                if (dgvCourseList.Rows.Count > 0)
                {
                    dgvCourseList.Rows[0].Selected = true;
                    HienThiChiTietDiem((GradesDTO)dgvCourseList.Rows[0].DataBoundItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu điểm học viên: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiChiTietDiem(GradesDTO grade)
        {
            // 1. ĐỔ DỮ LIỆU THÀNH PHẦN VÀO KHUNG "TỔNG KẾT"
            txtChuyenCann.Text = grade.AttendanceScore?.ToString("0.0") ?? "0.0";
            txtBaiTapp.Text = grade.MidTerm?.ToString("0.0") ?? "0.0"; // Giữ nguyên điểm bài tập
            txtDiemThii.Text = grade.FinalTerm?.ToString("0.0") ?? "0.0";
            txtdiemTB.Text = grade.AverageScore?.ToString("0.0") ?? "0.0";

            // 🛠️ ĐÃ FIX TẠI ĐÂY: Đổi từ txtBaiTapp sang đúng ô hiển thị xếp loại của bro
            if (txtRanking != null)
            {
                txtRanking.Text = grade.Ranking;
            }

            // 2. ĐỔ DỮ LIỆU VÀO KHUNG "NHẬN XÉT CỦA GIÁO VIÊN"
            txtTeacherComment.Text = grade.Note;

            if (lblTeacherName != null) lblTeacherName.Text = "Giáo viên nhận xét: " + grade.TeacherName;
            if (lblCommentDate != null) lblCommentDate.Text = "Ngày nhận xét: " + (grade.CommentDate?.ToString("dd/MM/yyyy") ?? "Chưa ghi nhận");
        }

        private void dgvCourseList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Tự động vẽ số thứ tự tăng dần 1, 2, 3... cho cột colSTT trên GridView
            if (dgvCourseList.Columns["colSTT"] != null)
            {
                dgvCourseList.Rows[e.RowIndex].Cells["colSTT"].Value = (e.RowIndex + 1).ToString();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (dgvCourseList.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu điểm nào để in ấn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            saveFileDialog.FileName = "BangDiem_HocVien_" + _currentStudentID + ".xlsx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Kết xuất dữ liệu theo luồng UTF-8 Tab-Separated mượt mà
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                    {
                        // Viết tiêu đề các cột trong file Excel
                        sw.WriteLine("STT\tTên Khóa Học / Lớp\tGiảng Viên Phụ Trách\tHọc Kỳ\tĐiểm Tổng Kết Tổng Quát");

                        for (int i = 0; i < dgvCourseList.Rows.Count; i++)
                        {
                            string stt = (i + 1).ToString();
                            string course = dgvCourseList.Rows[i].Cells["colCourseName"].Value?.ToString() ?? "";
                            string teacher = dgvCourseList.Rows[i].Cells["colTeacherName"].Value?.ToString() ?? "";
                            string semester = dgvCourseList.Rows[i].Cells["colSemester"].Value?.ToString() ?? "";
                            string score = dgvCourseList.Rows[i].Cells["colAvgScore"].Value?.ToString() ?? "";

                            sw.WriteLine($"{stt}\t{course}\t{teacher}\t{semester}\t{score}");
                        }
                    }

                    MessageBox.Show("Xuất dữ liệu thành công! Hệ thống sẽ mở bảng điểm để bro tiến hành lệnh in.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Mở file Excel vừa xuất để học viên bấm Ctrl + P in trực tiếp luôn
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(saveFileDialog.FileName) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra khi trích xuất tệp in ấn: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
