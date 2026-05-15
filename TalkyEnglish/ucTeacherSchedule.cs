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
using Excel = Microsoft.Office.Interop.Excel;
namespace TalkyEnglish.GUI
{
    public partial class ucTeacherSchedule : UserControl
    {
        private ScheduleBUS _scheduleBus = new ScheduleBUS();
        private List<ScheduleDTO> _mySchedules = new List<ScheduleDTO>();
        public ucTeacherSchedule()
        {
            InitializeComponent();
        }

        private void ucTeacherSchedule_Load(object sender, EventArgs e)
        {
            dgvSchedule.AutoGenerateColumns = false;
            // 1. Lấy ID từ SessionManager
            int currentID = SessionManager.CurrentUser.UserID;

            // 2. Lấy dữ liệu và gán vào biến dùng chung _mySchedules
            _mySchedules = _scheduleBus.GetSchedulesByTeacher(currentID);

            // 3. Đổ dữ liệu vào Grid
            dgvSchedule.DataSource = _mySchedules;
        }

        private void btnFilterToday_Click(object sender, EventArgs e)
        {
            // 1. Lấy thứ hiện tại của hệ thống (Monday, Tuesday...)
            string dayEng = DateTime.Today.DayOfWeek.ToString();
            string dayVie = "";

            // 2. Chuyển sang tiếng Việt cho khớp với Database
            switch (dayEng)
            {
                case "Monday": dayVie = "Thứ Hai"; break;
                case "Tuesday": dayVie = "Thứ Ba"; break;
                case "Wednesday": dayVie = "Thứ Tư"; break;
                case "Thursday": dayVie = "Thứ Năm"; break;
                case "Friday": dayVie = "Thứ Sáu"; break;
                case "Saturday": dayVie = "Thứ Bảy"; break;
                case "Sunday": dayVie = "Chủ Nhật"; break;
            }

            // 3. Lọc dữ liệu đang có trên Grid (hoặc gọi lại BUS)
            // Giả sử bro đã có List<ScheduleDTO> _mySchedules được load từ lúc đầu
            var filtered = _mySchedules.Where(s => s.DayOfWeek == dayVie).ToList();

            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = filtered;
        }

        private void btnFilterAll_Click(object sender, EventArgs e)
        {
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = _mySchedules;
        }

        private void btnFilterWeek_Click(object sender, EventArgs e)
        {
            // Hiển thị toàn bộ lịch dạy lặp lại trong 1 tuần
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = _mySchedules;
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            // Tương tự, lịch cố định tháng này vẫn là các ca dạy đó
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = _mySchedules;
        }

        private void UpdateTotalSessions()
        {
            // Đếm số dòng đang hiển thị trên DataGridView
            int count = dgvSchedule.Rows.Count;

            // Gán vào Label của bro (Giả sử tên là lblTotalSessions)
            lblTotalSessions.Text = $"Tổng số ca dạy: {count}";
        }

        private void btnPrintSchedule_Click(object sender, EventArgs e)
        {
            if (dgvSchedule.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu lịch dạy để thực hiện thao tác in.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Excel.Application excelApp = new Excel.Application();
            try
            {
                excelApp.Workbooks.Add();
                Excel._Worksheet workSheet = excelApp.ActiveSheet;

                // 1. Tạo tiêu đề các cột trong Excel dựa trên DataGridView
                for (int i = 0; i < dgvSchedule.Columns.Count; i++)
                {
                    if (dgvSchedule.Columns[i].Visible)
                    {
                        workSheet.Cells[1, i + 1] = dgvSchedule.Columns[i].HeaderText;
                        workSheet.Cells[1, i + 1].Font.Bold = true;
                    }
                }

                // 2. Đổ dữ liệu từ DataGridView vào các dòng tương ứng
                for (int i = 0; i < dgvSchedule.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvSchedule.Columns.Count; j++)
                    {
                        if (dgvSchedule.Columns[j].Visible)
                        {
                            workSheet.Cells[i + 2, j + 1] = dgvSchedule.Rows[i].Cells[j].Value?.ToString();
                        }
                    }
                }

                // 3. Tự động căn chỉnh độ rộng cột và hiển thị ứng dụng Excel
                workSheet.Columns.AutoFit();
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi trong quá trình xuất dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                excelApp.Quit();
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void dgvSchedule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
