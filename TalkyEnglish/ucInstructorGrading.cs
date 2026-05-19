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

namespace TalkyEnglish.GUI
{
    public partial class ucInstructorGrading : UserControl
    {
        private readonly GradesBUS _gradesBUS = new GradesBUS();
        private int _selectedCourseId = 0;
        public ucInstructorGrading()
        {
            InitializeComponent();
            // Đăng ký các sự kiện tính điểm tự động
            txtChuyenCann.TextChanged += TinhDiemTrungBinh_RealTime;
            txtBaiTapp.TextChanged += TinhDiemTrungBinh_RealTime;
            txtDiemThii.TextChanged += TinhDiemTrungBinh_RealTime;

            // GIỮ LẠI đăng ký nút lưu và click chọn dòng
            guna2DataGridView1.SelectionChanged += guna2DataGridView1_SelectionChanged;
            btnLuu.Click += btnLuu_Click;
            guna2DataGridView1.RowPostPaint += guna2DataGridView1_RowPostPaint;
        }


        private void guna2DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // Lấy ra số thứ tự dựa trên chỉ số dòng đang vẽ (cộng thêm 1 để bắt đầu từ số 1)
            string stt = (e.RowIndex + 1).ToString();

            // Điền trực tiếp chữ vào ô ColSTT mà không sợ bị cơ chế gán Data xóa mất
            guna2DataGridView1.Rows[e.RowIndex].Cells["ColSTT"].Value = stt;
        }
        private void ucInstructorGrading_Load(object sender, EventArgs e)
        {
            // 1. Ánh xạ thuộc tính DTO vào đúng tên các cột trên DataGridView của bro
            // Xóa DataPropertyName của ColSTT để tí nữa vòng lặp tự vẽ số 1, 2, 3...
            ColSTT.DataPropertyName = "";
            ColMaHV.DataPropertyName = "GradeID";     // Hiện mã UserID của học viên lên cột "Mã Học Viên"
            ColTenHV.DataPropertyName = "TeacherName"; // Hiện họ tên thật của học viên lên cột "Tên Học Viên"
            ColTenKH.DataPropertyName = "CourseName";  // Hiện tên khóa học chuẩn vừa tối ưu dưới DAL
            ColTinhTrang.DataPropertyName = "Semester"; // Hiện chữ "Đã nhập điểm" / "Chưa nhập điểm"

            try
            {
                // 2. NẠP DỮ LIỆU VÀO COMBOBOX LỚP HỌC TRƯỚC
                // Đoạn này gọi từ CourseBUS gốc của nhóm bro để ComboBox ăn data xịn
                CourseBUS courseBUS = new CourseBUS();
                cboLopHoc.DataSource = courseBUS.GetAllCourses();
                cboLopHoc.ValueMember = "CourseID";
                cboLopHoc.DisplayMember = "CourseName";

                // 3. SAU KHI COMBOBOX ĐÃ CÓ DATA -> MỚI ĐĂNG KÝ SỰ KIỆN THAY ĐỔI LỚP HỌC
                // Làm vậy để tránh việc ComboBox bị kích hoạt ảo khi chưa có dữ liệu bro nhé
                cboLopHoc.SelectedIndexChanged += cboLopHoc_SelectedIndexChanged;

                // 4. CHỦ ĐỘNG GỌI LOAD DANH SÁCH LẦN ĐẦU TIÊN
                // Lúc này ComboBox đã có giá trị chọn mặc định đầu tiên nên sẽ load danh sách học viên chuẩn luôn
                LoadDanhSachHocVien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi tạo dữ liệu giao diện: " + ex.Message, "Lỗi GUI", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDanhSachHocVien()
        {
            if (cboLopHoc.SelectedValue != null && int.TryParse(cboLopHoc.SelectedValue.ToString(), out int courseId))
            {
                try
                {
                    _selectedCourseId = courseId;
                    List<GradesDTO> listGrades = _gradesBUS.GetStudentGradesByCourse(_selectedCourseId);

                    guna2DataGridView1.AutoGenerateColumns = false;
                    guna2DataGridView1.DataSource = listGrades; // Chỉ gán DataSource thô thôi, không chạy vòng lặp nữa bro
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cboLopHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDanhSachHocVien();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void TinhDiemTrungBinh_RealTime(object sender, EventArgs e)
        {
            CapNhatHienThiDTB();
        }
        private void CapNhatHienThiDTB()
        {
            double cc = double.TryParse(txtChuyenCann.Text, out double v1) ? v1 : 0;
            double bt = double.TryParse(txtBaiTapp.Text, out double v2) ? v2 : 0;
            double thi = double.TryParse(txtDiemThii.Text, out double v3) ? v3 : 0;

            double dtb = (cc * 0.1) + (bt * 0.3) + (thi * 0.6);
            txtdiemTB.Text = dtb.ToString("0.0");
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow == null) return;

            try
            {
                var selectedRow = guna2DataGridView1.CurrentRow.DataBoundItem as GradesDTO;
                if (selectedRow == null) return;

                // Đóng gói toàn bộ thông tin từ các ô TextBox của bro vào đối tượng DTO
                var gradeToSave = new GradesDTO
                {
                    EnrolmentID = selectedRow.EnrolmentID, // Giữ nguyên mã liên kết
                    AttendanceScore = double.TryParse(txtChuyenCann.Text, out double cc) ? cc : 0,
                    MidTerm = double.TryParse(txtBaiTapp.Text, out double bt) ? bt : 0,
                    FinalTerm = double.TryParse(txtDiemThii.Text, out double thi) ? thi : 0,
                    AverageScore = double.TryParse(txtdiemTB.Text, out double dtb) ? dtb : 0,
                    Note = txtNhanxet.Text,
                    CommentDate = DateTime.Now
                };

                // Gọi xuống lớp BUS để thực thi cập nhật cơ sở dữ liệu
                bool isSuccess = _gradesBUS.SaveOrUpdateGrade(gradeToSave);

                if (isSuccess)
                {
                    MessageBox.Show("Hệ thống đã lưu nhận xét và điểm số thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDanhSachHocVien(); // Tải lại bảng để đồng bộ dữ liệu mới nhất
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại. Xin vui lòng kiểm tra lại khoảng điểm hợp lệ (0 đến 10).", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tầng GUI: " + ex.Message, "Thông báo lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void guna2DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow != null)
            {
                var selectedGrade = guna2DataGridView1.CurrentRow.DataBoundItem as GradesDTO;

                if (selectedGrade != null)
                {
                    // Đổ dữ liệu thật từ SQL vào đúng các TextBox trong file design của bro
                    txtChuyenCann.Text = selectedGrade.AttendanceScore?.ToString() ?? "0";
                    txtBaiTapp.Text = selectedGrade.MidTerm?.ToString() ?? "0"; // Gán điểm Bài tập vào MidTerm như thỏa thuận
                    txtDiemThii.Text = selectedGrade.FinalTerm?.ToString() ?? "0";
                    txtNhanxet.Text = selectedGrade.Note ?? "";

                    CapNhatHienThiDTB();
                }
            }
        }

        private void guna2Panel7_Paint(object sender, PaintEventArgs e)
        {

        }
    }

}
