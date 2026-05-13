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
            dgvCourses.DataSource = _courseBUS.GetAllCourses();

            if (dgvCourses.Columns["CourseID"] != null) dgvCourses.Columns["CourseID"].Visible = false;
            if (dgvCourses.Columns["InstructorID"] != null) dgvCourses.Columns["InstructorID"].Visible = false;
            if (dgvCourses.Columns["CategoryID"] != null) dgvCourses.Columns["CategoryID"].Visible = false;
            if (dgvCourses.Columns["Description"] != null) dgvCourses.Columns["Description"].Visible = false;
        }

        private void dgvCourses_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCourses.SelectedRows.Count > 0)
            {
                var course = (CourseDTO)dgvCourses.SelectedRows[0].DataBoundItem;

                // Gán đúng tên thuộc tính từ file CourseDTO bạn gửi
                lblCourseCode.Text = course.CourseCode;
                lblCourseName.Text = course.CourseName;
                lblLevel.Text = course.Level;
                lblDuration.Text = course.Duration;
                lblPrice.Text = string.Format("{0:N0} VNĐ", course.Price);
                txtDescription.Text = course.Description;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCourses.SelectedRows.Count > 0)
                {
                    var course = (CourseDTO)dgvCourses.SelectedRows[0].DataBoundItem;

                    if (SessionManager.CurrentUser == null)
                    {
                        MessageBox.Show("Lỗi: Phiên đăng nhập hết hạn!");
                        return;
                    }

                    int studentId = SessionManager.CurrentUser.UserID;

                    // SỬA TẠI ĐÂY: Gọi từ _enrolmentBUS thay vì _courseBUS
                    if (_enrolmentBUS.RegisterCourse(studentId, course.CourseID))
                    {
                        MessageBox.Show("Đăng ký thành công!", "Thông báo");
                    }
                    else
                    {
                        MessageBox.Show("Bạn đã đăng ký khóa học này rồi hoặc có lỗi xảy ra.", "Lưu ý");
                    }
                }
            }
            catch (Exception ex)
            {
                // Đào sâu vào tận cùng của lỗi
                Exception realException = ex;
                while (realException.InnerException != null)
                {
                    realException = realException.InnerException;
                }

                // Bây giờ nó sẽ hiện đúng cái lỗi SQL: ví dụ "Tên cột sai" hoặc "Vi phạm khóa"
                MessageBox.Show("LỖI THỰC TẾ: " + realException.Message);
            }
        }
    }
}
