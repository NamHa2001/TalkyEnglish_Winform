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

namespace TalkyEnglish.GUI
{
    public partial class ucStudentGrades : UserControl
    {

        // Lưu danh sách các khóa học để hiển thị lên bảng trên
        private List<GradesDTO> _courseList = new List<GradesDTO>();
        public ucStudentGrades()
        {
            InitializeComponent();
            LoadMockGrades();
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e)
        {

        }


        private void LoadMockGrades()
        {
            // Chú thích báo cáo: Khởi tạo dữ liệu mẫu dựa trên kiến trúc GradesDTO
            _courseList = new List<GradesDTO>
            {
                new GradesDTO {
                    EnrolmentID = 1,
                    CourseName = "English Communication A2",
                    TeacherName = "Nguyễn Thị Thu Hà",
                    Semester = "HK1/2025",
                    AttendanceScore = 8.0, MidTerm = 7.5, FinalTerm = 8.0,
                    AverageScore = 7.8, Ranking = "Khá", GradeLetter = "B",
                    TotalSessions = "24/24", AttendancePercentage = 96.0,
                    Note = "An có tiến bộ tốt trong kỹ năng giao tiếp. Phát âm rõ ràng hơn...",
                    CommentDate = new DateTime(2025, 5, 15)
                },
                new GradesDTO {
                    EnrolmentID = 2,
                    CourseName = "IELTS Foundation",
                    TeacherName = "Trần Minh Hoàng",
                    Semester = "HK1/2025",
                    AttendanceScore = 7.0, MidTerm = 6.0, FinalTerm = 6.5,
                    AverageScore = 6.5, Ranking = "Trung Bình Khá", GradeLetter = "C+",
                    TotalSessions = "20/24", AttendancePercentage = 83.3,
                    Note = "Cần tập trung hơn vào phần Writing task 2.",
                    CommentDate = new DateTime(2025, 4, 20)
                }
            };

            // Cơ chế đồng bộ hóa: Tắt tự động tạo cột để khớp với thiết kế thủ công
            dgvCourseList.AutoGenerateColumns = false;
            dgvCourseList.DataSource = null;
            dgvCourseList.DataSource = _courseList;

            // Chú thích báo cáo: Tối ưu hiển thị cho bảng danh sách tổng quát
            if (dgvCourseList.Rows.Count > 0)
            {
                dgvCourseList.Rows[0].Selected = true; // Tự động chọn dòng đầu tiên
            }
        }
        private void UpdateGradeDetailTable(GradesDTO grade)
        {
            // Chú thích báo cáo: Chuyển đổi các thuộc tính DTO thành danh sách hàng (Rows) cho bảng chi tiết
            // Logic tính toán: Điểm quy đổi = (Điểm * Tỷ trọng) / 100

            var detailList = new List<object>
    {
        new { STT = 1, HangMuc = "Điểm giữa kỳ", Diem = grade.MidTerm, TyTrong = 40, QuyDoi = (grade.MidTerm * 40) / 100 },
        new { STT = 2, HangMuc = "Điểm cuối kỳ", Diem = grade.FinalTerm, TyTrong = 50, QuyDoi = (grade.FinalTerm * 50) / 100 },
        new { STT = 3, HangMuc = "Bài tập / Thảo luận", Diem = grade.AttendanceScore, TyTrong = 10, QuyDoi = (grade.AttendanceScore * 10) / 100 }
    };

            // Đổ dữ liệu vào Guna2DataGridView chi tiết (Cột trái dưới)
            dgvGradeDetail.DataSource = detailList.ToList();

            // Tối ưu tiêu đề cột cho chuyên nghiệp
            if (dgvGradeDetail.Columns["HangMuc"] != null) dgvGradeDetail.Columns["HangMuc"].HeaderText = "Hạng mục";
            if (dgvGradeDetail.Columns["Diem"] != null) dgvGradeDetail.Columns["Diem"].HeaderText = "Điểm";
            if (dgvGradeDetail.Columns["TyTrong"] != null) dgvGradeDetail.Columns["TyTrong"].HeaderText = "Tỷ trọng (%)";
            if (dgvGradeDetail.Columns["QuyDoi"] != null) dgvGradeDetail.Columns["QuyDoi"].HeaderText = "Điểm quy đổi";
        }

        private void dgvCourseList_SelectionChanged(object sender, EventArgs e)
        {
            // Thêm kiểm tra null cho selectedGrade
            if (dgvCourseList.SelectedRows.Count > 0)
            {
                var item = dgvCourseList.SelectedRows[0].DataBoundItem;
                if (item != null)
                {
                    var selectedGrade = (GradesDTO)item;

                    // Đổ dữ liệu vào TextBox (Dùng toán tử ?. để an toàn)
                    txtAvgScore.Text = selectedGrade.AverageScore?.ToString("N1");
                    txtRanking.Text = selectedGrade.Ranking;
                    txtGradeLetter.Text = selectedGrade.GradeLetter;
                    txtTotalSessions.Text = selectedGrade.TotalSessions;
                    txtAttendancePercentage.Text = selectedGrade.AttendancePercentage?.ToString("N1") + "%";

                    txtTeacherComment.Text = selectedGrade.Note;

                    // Kiểm tra các Label có tồn tại không trước khi gán
                    if (lblTeacherName != null) lblTeacherName.Text = selectedGrade.TeacherName;
                    if (lblCommentDate != null) lblCommentDate.Text = selectedGrade.CommentDate?.ToString("dd/MM/yyyy");

                    UpdateGradeDetailTable(selectedGrade);
                }
            }
        }
    }
}
