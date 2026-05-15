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
    public partial class ucRegisterCourse : UserControl
    {
        // Thêm khai báo này ở đầu class ucRegisterCourse
        private EnrolmentBUS _enrolmentBUS = new EnrolmentBUS();
        CourseBUS _courseBUS = new CourseBUS();

        public ucRegisterCourse()
        {
            InitializeComponent();
        }

        private void ucRegisterCourse_Load(object sender, EventArgs e)
        {
            try
            {
                // Chỉ hiện các cột bro đã thiết kế thủ công trong Designer
                dgvCourses.AutoGenerateColumns = false;

                // Nạp danh sách khóa học từ Database
                dgvCourses.DataSource = _courseBUS.GetAllCourses();

                // Căn chỉnh các cột cho lấp đầy bảng
                dgvCourses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Định dạng hiển thị tiền tệ cho cột Học phí
                if (dgvCourses.Columns["Price"] != null)
                {
                    dgvCourses.Columns["Price"].DefaultCellStyle.Format = "N0";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách khóa học: " + ex.Message, "Thông báo");
            }
        }

        private void dgvCourses_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count > 0)
            {
                // Ép kiểu dữ liệu dòng đang chọn về CourseDTO
                var course = (CourseDTO)dgvCourses.SelectedRows[0].DataBoundItem;

                // Đổ dữ liệu ra các Label và TextBox bên cạnh
                lblCourseCode.Text = course.CourseCode;
                lblCourseName.Text = course.CourseName;
                lblLevel.Text = course.Level;
                lblDuration.Text = course.Duration;
                lblPrice.Text = string.Format("{0:N0} VNĐ", course.Price);
                txtDescription.Text = course.Description;

                // Tự động Khóa nút Đăng ký nếu khóa học đã hết chỗ (AvailableSlots = 0)
                btnRegister.Enabled = (course.AvailableSlots > 0);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCourses.SelectedRows.Count > 0)
                {
                    var course = (CourseDTO)dgvCourses.SelectedRows[0].DataBoundItem;

                    // Kiểm tra đăng nhập (Session)
                    if (SessionManager.CurrentUser == null)
                    {
                        MessageBox.Show("Vui lòng đăng nhập để thực hiện chức năng này!", "Thông báo");
                        return;
                    }

                    // Kiểm tra lại số lượng chỗ trống thực tế
                    if (course.AvailableSlots <= 0)
                    {
                        MessageBox.Show("Khóa học đã đủ học viên, vui lòng chọn khóa khác!", "Thông báo");
                        return;
                    }

                    int studentId = SessionManager.CurrentUser.UserID;

                    // Gọi tầng BUS để lưu đăng ký vào Database
                    if (_enrolmentBUS.RegisterCourse(studentId, course.CourseID))
                    {
                        MessageBox.Show("Đăng ký thành công khóa học: " + course.CourseName, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Cập nhật lại bảng để trừ bớt số lượng "Chỗ trống" ngay lập tức
                        dgvCourses.DataSource = _courseBUS.GetAllCourses();
                    }
                    else
                    {
                        MessageBox.Show("Đăng ký thất bại! Có thể bạn đã đăng ký khóa học này trước đó.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một khóa học trong danh sách!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi chi tiết từ SQL
                Exception realException = ex;
                while (realException.InnerException != null) realException = realException.InnerException;
                MessageBox.Show("Lỗi hệ thống: " + realException.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
