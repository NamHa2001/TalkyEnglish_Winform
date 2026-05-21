using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucScheduleManagement : UserControl
    {
        private readonly CourseBUS   _courseBus   = new CourseBUS();
        private readonly ScheduleBUS _scheduleBus = new ScheduleBUS();

        private int _selectedCourseId = -1;
        private List<ScheduleDTO> _allSchedules = new List<ScheduleDTO>();
        private List<CourseDTO>   _allCourses   = new List<CourseDTO>();

        public ucScheduleManagement()
        {
            InitializeComponent();
        }

        private void ucScheduleManagement_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);

            // Đăng ký sự kiện chưa có trong Designer.cs
            txtSearchCourse.TextChanged += TxtSearchCourse_TextChanged;
            btnFilterAll.Click          += (s, ev) => ApplyDayFilter(null);
            btnFilterToday.Click        += (s, ev) => ApplyDayFilter(DayShort(DateTime.Today.DayOfWeek.ToString()));
            btnFilterWeek.Click         += (s, ev) => ApplyDayFilter(null);
            btnFilterMonth.Click        += (s, ev) => ApplyDayFilter(null);

            LoadData();
        }

        // ── Tải dữ liệu ──────────────────────────────────────────────────────────

        public void LoadData()
        {
            dgvCourses.AutoGenerateColumns   = false;
            dgvSchedules.AutoGenerateColumns = false;
            LoadCoursesToGrid();
            LoadSchedulesToGrid();
        }

        private void LoadCoursesToGrid()
        {
            try
            {
                _allCourses = _courseBus.GetAllCourses();
                dgvCourses.DataSource = null;
                dgvCourses.DataSource = _allCourses;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách khóa học: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSchedulesToGrid()
        {
            try
            {
                _allSchedules = _scheduleBus.GetAllSchedules();
                FilterSchedulesByCourse();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách lịch học: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Lọc dgvSchedules theo khóa học đang chọn (hoặc tất cả nếu chưa chọn)
        private void FilterSchedulesByCourse()
        {
            dgvSchedules.DataSource = _selectedCourseId == -1
                ? _allSchedules
                : _allSchedules.Where(s => s.CourseID == _selectedCourseId).ToList();
        }

        // Lọc dgvSchedules theo thứ trong tuần (null = hiện tất cả)
        private void ApplyDayFilter(string dayFilter)
        {
            var source = _selectedCourseId == -1
                ? _allSchedules
                : _allSchedules.Where(s => s.CourseID == _selectedCourseId).ToList();

            if (!string.IsNullOrEmpty(dayFilter))
                source = source.Where(s => s.DayOfWeek == dayFilter).ToList();

            dgvSchedules.DataSource = source;
        }

        // ── Tìm kiếm khóa học ────────────────────────────────────────────────────

        private void TxtSearchCourse_TextChanged(object sender, EventArgs e)
        {
            string kw = txtSearchCourse.Text.Trim().ToLower();
            dgvCourses.DataSource = string.IsNullOrEmpty(kw)
                ? _allCourses
                : _allCourses
                    .Where(c => (c.CourseName?.ToLower().Contains(kw) == true)
                             || (c.CourseCode?.ToLower().Contains(kw) == true))
                    .ToList();
        }

        // ── Sự kiện Grid ─────────────────────────────────────────────────────────

        private void dgvCourses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvCourses.Rows[e.RowIndex];
            if (row.Cells["colCourseID"].Value == null) return;

            _selectedCourseId = Convert.ToInt32(row.Cells["colCourseID"].Value);
            string courseName  = row.Cells["colCourseName"].Value?.ToString() ?? "";
            label3.Text = $"Lịch học của khóa: {courseName}";

            FilterSchedulesByCourse();
        }

        private void dgvSchedules_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvSchedules.Rows[e.RowIndex];

            if (row.Cells["colDayOfWeek"].Value != null)
                cbDayOfWeek.Text = row.Cells["colDayOfWeek"].Value.ToString();

            if (row.Cells["colRoomName"].Value != null)
                cbRoom.Text = row.Cells["colRoomName"].Value.ToString();

            if (row.Cells["colStartTime"].Value is TimeSpan start)
                dtpStartTime.Value = DateTime.Today.Add(start);

            if (row.Cells["colEndTime"].Value is TimeSpan end)
                dtpEndTime.Value = DateTime.Today.Add(end);
        }

        // ── Thêm lịch ────────────────────────────────────────────────────────────

        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            if (_selectedCourseId == -1)
            {
                MessageBox.Show("Vui lòng chọn một khóa học bên trái trước khi thêm lịch!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cbDayOfWeek.SelectedIndex < 0 || cbRoom.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Thứ và Phòng học!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TimeSpan start = dtpStartTime.Value.TimeOfDay;
            TimeSpan end   = dtpEndTime.Value.TimeOfDay;
            if (start >= end)
            {
                MessageBox.Show("Giờ bắt đầu phải nhỏ hơn giờ kết thúc!",
                    "Lỗi thời gian", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newSchedule = new ScheduleDTO
            {
                CourseID  = _selectedCourseId,
                DayOfWeek = cbDayOfWeek.Text,
                StartTime = start,
                EndTime   = end,
                RoomName  = cbRoom.Text
            };

            string result = _scheduleBus.SaveSchedule(newSchedule);
            if (result == "OK")
            {
                MessageBox.Show("Thêm lịch học thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSchedulesToGrid();
                cbDayOfWeek.SelectedIndex = -1;
                cbRoom.SelectedIndex      = -1;
            }
            else
            {
                MessageBox.Show(result, "Cảnh báo trùng lịch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── Cập nhật lịch ────────────────────────────────────────────────────────

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvSchedules.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một lịch học cần cập nhật!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var idCell = dgvSchedules.CurrentRow.Cells["colScheduleID"].Value;
            if (idCell == null || !int.TryParse(idCell.ToString(), out int id))
            {
                MessageBox.Show("Không thể xác định ID lịch học, vui lòng thử lại!",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cbDayOfWeek.SelectedIndex < 0 || cbRoom.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn đầy đủ Thứ và Phòng học!",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            TimeSpan start = dtpStartTime.Value.TimeOfDay;
            TimeSpan end   = dtpEndTime.Value.TimeOfDay;
            if (start >= end)
            {
                MessageBox.Show("Giờ bắt đầu phải nhỏ hơn giờ kết thúc!",
                    "Lỗi thời gian", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var confirm = MessageBox.Show(
                $"Xác nhận cập nhật lịch học?\n\n" +
                $"Thứ: {cbDayOfWeek.Text}   |   Phòng: {cbRoom.Text}\n" +
                $"Giờ: {dtpStartTime.Value:HH:mm} – {dtpEndTime.Value:HH:mm}",
                "Xác nhận cập nhật", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            var upSchedule = new ScheduleDTO
            {
                ScheduleID = id,
                DayOfWeek  = cbDayOfWeek.Text,
                StartTime  = start,
                EndTime    = end,
                RoomName   = cbRoom.Text
            };

            string result = _scheduleBus.UpdateSchedule(upSchedule);
            if (result == "OK")
            {
                MessageBox.Show("Cập nhật lịch học thành công!", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSchedulesToGrid();
            }
            else
            {
                MessageBox.Show(result, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── Xóa lịch ─────────────────────────────────────────────────────────────

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvSchedules.CurrentRow == null) return;

            var idCell = dgvSchedules.CurrentRow.Cells["colScheduleID"].Value;
            if (idCell == null || !int.TryParse(idCell.ToString(), out int id)) return;

            var dr = MessageBox.Show("Bạn chắc chắn muốn xóa lịch học này?",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes) return;

            if (_scheduleBus.DeleteSchedule(id))
            {
                MessageBox.Show("Đã xóa lịch học thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSchedulesToGrid();
            }
            else
            {
                MessageBox.Show("Xóa lịch học thất bại, vui lòng thử lại.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── Làm mới form ─────────────────────────────────────────────────────────

        private void btnClearForm_Click(object sender, EventArgs e)
        {
            _selectedCourseId         = -1;
            label3.Text               = "Nhập thông tin lịch cho khóa học đã chọn";
            cbDayOfWeek.SelectedIndex = -1;
            cbRoom.SelectedIndex      = -1;
            dtpStartTime.Value        = DateTime.Today.AddHours(8);
            dtpEndTime.Value          = DateTime.Today.AddHours(10);
            txtSearchCourse.Text      = "";
            dgvCourses.ClearSelection();
            dgvSchedules.ClearSelection();
            dgvSchedules.DataSource = _allSchedules;
        }

        // ── Helper ───────────────────────────────────────────────────────────────

        private static string DayShort(string dayEng) => dayEng switch
        {
            "Monday"    => "Thứ 2",
            "Tuesday"   => "Thứ 3",
            "Wednesday" => "Thứ 4",
            "Thursday"  => "Thứ 5",
            "Friday"    => "Thứ 6",
            "Saturday"  => "Thứ 7",
            _           => "Chủ Nhật"
        };
    }
}
