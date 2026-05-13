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
    public partial class ucScheduleManagement : UserControl
    {

        // Khai báo BUS
        CourseBUS courseBus = new CourseBUS();
        private int selectedCourseId = -1;
        ScheduleBUS scheduleBus = new ScheduleBUS();
        // Hàm nạp danh sách khóa học vào Grid bên trái (dgvCourses)
        private void LoadCoursesToGrid()
        {
            var list = courseBus.GetAllCourses();
            dgvCourses.DataSource = list;

            // Nếu bro đã set DataPropertyName cho các cột colCourseCode, colCourseName 
            // thì dữ liệu sẽ tự nhảy vào đúng chỗ.
        }

        private void LoadSchedulesToGrid()
        {
            dgvSchedules.AutoGenerateColumns = false; // Chặn cột thừa như cái Grid trên
            dgvSchedules.DataSource = scheduleBus.GetAllSchedules();
        }

        public void LoadData()
        {
            dgvCourses.AutoGenerateColumns = false;
            dgvSchedules.AutoGenerateColumns = false;
            LoadCoursesToGrid(); // Hàm đổ danh sách khóa học vào dgvCourses
                                 // Sau này có thêm hàm LoadSchedules() thì cũng quăng vào đây luôn
            LoadSchedulesToGrid();
        }
        public ucScheduleManagement()
        {
            InitializeComponent();
        }



        private void ucScheduleManagement_Load(object sender, EventArgs e)
        {
            LoadCoursesToGrid();
        }

        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã chọn khóa học chưa
            if (selectedCourseId == -1)
            {
                MessageBox.Show("Vui lòng chọn một khóa học bên trái trước khi thêm lịch!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra xem đã chọn Thứ và Phòng chưa (Tránh để trống dữ liệu)
            if (string.IsNullOrEmpty(cbDayOfWeek.Text) || string.IsNullOrEmpty(cbRoom.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Thứ và Phòng học bro ơi!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 3. Đóng gói dữ liệu
            TimeSpan start = dtpStartTime.Value.TimeOfDay;
            TimeSpan end = dtpEndTime.Value.TimeOfDay;

            // Kiểm tra logic thời gian cơ bản (Giờ bắt đầu phải trước giờ kết thúc)
            if (start >= end)
            {
                MessageBox.Show("Giờ bắt đầu phải nhỏ hơn giờ kết thúc chứ bro kkk!", "Lỗi thời gian", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ScheduleDTO newSchedule = new ScheduleDTO
            {
                CourseID = selectedCourseId,
                DayOfWeek = cbDayOfWeek.Text,
                StartTime = start,
                EndTime = end,
                RoomName = cbRoom.Text
            };

            // 4. Gọi hàm Save (Bây giờ nó trả về string: "OK" hoặc thông báo lỗi trùng)
            string result = scheduleBus.SaveSchedule(newSchedule);

            if (result == "OK")
            {
                MessageBox.Show("Thêm lịch thành công rực rỡ!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 5. Làm mới danh sách dưới Grid
                LoadSchedulesToGrid();

                // 6. Reset nhẹ các ô nhập liệu để sẵn sàng cho lần sau
                cbDayOfWeek.SelectedIndex = -1;
                cbRoom.SelectedIndex = -1;
            }
            else
            {
                // Nếu trùng lịch, nó sẽ hiện thông báo: "Phòng này đã có lớp..." mà mình viết ở BUS
                MessageBox.Show(result, "Cảnh báo trùng lịch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dgvCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvCourses.Rows[e.RowIndex];
                selectedCourseId = Convert.ToInt32(row.Cells["colCourseID"].Value);

                // Hiện tên khóa học lên một Label nào đó cho Admin yên tâm

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSchedules.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvSchedules.CurrentRow.Cells["colScheduleID"].Value);

            DialogResult dr = MessageBox.Show("Bro chắc chắn muốn xóa lịch này chứ?", "Xác nhận", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
            {
                if (scheduleBus.DeleteSchedule(id))
                {
                    MessageBox.Show("Xóa xong rồi bro!");
                    LoadSchedulesToGrid(); // F5 lại Grid cho sạch
                }
            }
        }

        private void btnClearForm_Click(object sender, EventArgs e)
        {
            selectedCourseId = -1;
            cbDayOfWeek.SelectedIndex = -1;
            cbRoom.SelectedIndex = -1;
            dtpStartTime.Value = DateTime.Now; // Hoặc set về một giờ mặc định nào đó
            dtpEndTime.Value = DateTime.Now;

            // Nếu muốn xóa sạch lựa chọn trên Grid
            dgvCourses.ClearSelection();
            dgvSchedules.ClearSelection();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvSchedules.CurrentRow == null)
            {
                MessageBox.Show("Chọn cái lịch cần sửa ở bảng dưới đã bro!");
                return;
            }

            // Lấy ID từ dòng đang chọn
            int id = Convert.ToInt32(dgvSchedules.CurrentRow.Cells["colScheduleID"].Value);

            ScheduleDTO upSchedule = new ScheduleDTO
            {
                ScheduleID = id,
                DayOfWeek = cbDayOfWeek.Text,
                StartTime = dtpStartTime.Value.TimeOfDay,
                EndTime = dtpEndTime.Value.TimeOfDay,
                RoomName = cbRoom.Text
            };

            string result = scheduleBus.UpdateSchedule(upSchedule);
            if (result == "OK")
            {
                MessageBox.Show("Cập nhật lịch thành công!");
                LoadSchedulesToGrid();
            }
            else
            {
                MessageBox.Show(result);
            }
        }

        private void dgvSchedules_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvSchedules.Rows[e.RowIndex];
                cbDayOfWeek.Text = row.Cells["colDayOfWeek"].Value.ToString();
                cbRoom.Text = row.Cells["colRoomName"].Value.ToString();

                // Gán giờ vào DateTimePicker
                TimeSpan start = (TimeSpan)row.Cells["colStartTime"].Value;
                dtpStartTime.Value = DateTime.Today.Add(start);

                TimeSpan end = (TimeSpan)row.Cells["colEndTime"].Value;
                dtpEndTime.Value = DateTime.Today.Add(end);
            }
        }
    }
}
