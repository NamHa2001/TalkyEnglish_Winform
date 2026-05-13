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

using System.Drawing.Printing; // Thư viện để điều khiển máy in
namespace TalkyEnglish.GUI
{
    public partial class ucStudentSchedule : UserControl
    {

        // Đối tượng quản lý bản in
        private PrintDocument printDoc = new PrintDocument();
        private PrintPreviewDialog printPreview = new PrintPreviewDialog();
        // Đây là nơi khai báo biến lưu trữ dữ liệu gốc cho toàn bộ UC
        private List<ScheduleDTO> _originalList = new List<ScheduleDTO>();
        public ucStudentSchedule()
        {
            InitializeComponent();
            LoadScheduleData();
        }

        private void LoadScheduleData()
        {
            // SỬA TẠI ĐÂY: Gán trực tiếp vào biến toàn cục _originalList thay vì tạo biến cục bộ 'list'
            _originalList = new List<ScheduleDTO>
    {
       
    };

            // Đổ dữ liệu từ kho gốc vào DataGridView
            dgvSchedule.DataSource = _originalList;

            // Các thiết lập tiêu đề cột (Giữ nguyên của bạn)
            if (dgvSchedule.Columns["Date"] != null) dgvSchedule.Columns["Date"].HeaderText = "Ngày học";
            if (dgvSchedule.Columns["DayOfWeek"] != null) dgvSchedule.Columns["DayOfWeek"].HeaderText = "Thứ";
            if (dgvSchedule.Columns["CourseName"] != null) dgvSchedule.Columns["CourseName"].HeaderText = "Tên khóa học / Lớp";

            dgvSchedule.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);

            // Cập nhật nhãn tổng số buổi ngay khi nạp lần đầu
            lblTotalSessions.Text = $"Tổng số: {_originalList.Count} buổi học";
        }
        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Chú thích báo cáo: Sử dụng Graphics để vẽ tiêu đề và nội dung bảng lên trang giấy A4
            Graphics g = e.Graphics;
            Font fontTitle = new Font("Segoe UI", 18, FontStyle.Bold);
            Font fontHeader = new Font("Segoe UI", 10, FontStyle.Bold);
            Font fontBody = new Font("Segoe UI", 10, FontStyle.Regular);

            // 1. Vẽ tiêu đề
            g.DrawString("THỜI KHÓA BIỂU CÁ NHÂN", fontTitle, Brushes.Blue, 200, 50);
            string studentName = SessionManager.CurrentUser != null ? SessionManager.CurrentUser.FullName : "Học viên";
            g.DrawString($"Học viên: {studentName}", fontBody, Brushes.Black, 50, 100);
            g.DrawString($"Ngày in: {DateTime.Now:dd/MM/yyyy}", fontBody, Brushes.Black, 50, 120);

            // 2. Vẽ đường kẻ ngang (Header của bảng)
            int y = 160;
            g.DrawLine(Pens.Black, 50, y, 750, y);
            g.DrawString("Ngày", fontHeader, Brushes.Black, 50, y + 5);
            g.DrawString("Môn học", fontHeader, Brushes.Black, 150, y + 5);
            g.DrawString("Giáo viên", fontHeader, Brushes.Black, 450, y + 5);
            g.DrawString("Phòng", fontHeader, Brushes.Black, 650, y + 5);
            y += 30;
            g.DrawLine(Pens.Black, 50, y, 750, y);

            // 3. Duyệt qua DataGridView để vẽ từng dòng dữ liệu ảo
            foreach (DataGridViewRow row in dgvSchedule.Rows)
            {
                if (row.IsNewRow) continue;

                string date = Convert.ToDateTime(row.Cells["Date"].Value).ToString("dd/MM/yyyy");
                string course = row.Cells["CourseName"].Value.ToString();
                string teacher = row.Cells["TeacherName"].Value.ToString();
                string room = row.Cells["RoomName"].Value.ToString();

                g.DrawString(date, fontBody, Brushes.Black, 50, y + 5);
                g.DrawString(course, fontBody, Brushes.Black, 150, y + 5);
                g.DrawString(teacher, fontBody, Brushes.Black, 450, y + 5);
                g.DrawString(room, fontBody, Brushes.Black, 650, y + 5);

                y += 25;
                g.DrawLine(Pens.LightGray, 50, y, 750, y); // Kẻ dòng mờ ngăn cách
            }
        }
        private void FilterSchedule()
        {
            //// Chú thích báo cáo: Ép kiểu dữ liệu về .Date để loại bỏ sai số giờ phút giây khi so sánh
            //DateTime fromDate = dtpFromDate.Value.Date;
            //DateTime toDate = dtpToDate.Value.Date;

            //// Sử dụng .Date trên biến chạy 's' để đảm bảo so sánh chính xác tuyệt đối
            //var filteredData = _originalList
            //    .Where(s => s.Date.Date >= fromDate && s.Date.Date <= toDate)
            //    .ToList();

            //// Cập nhật giao diện
            //dgvSchedule.DataSource = null;
            //dgvSchedule.DataSource = filteredData;

            //// Cập nhật nhãn tổng số (đảm bảo tên nhãn đúng với Designer của bạn)
            //lblTotalSessions.Text = $"Tổng số: {filteredData.Count} buổi học";
        }

        private void btnThisWeek_Click(object sender, EventArgs e)
        {
            // Chú thích báo cáo: Thuật toán xác định phạm vi tuần hiện tại để tối ưu trải nghiệm người dùng
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff);
            DateTime endOfWeek = startOfWeek.AddDays(6);

            dtpFromDate.Value = startOfWeek;
            dtpToDate.Value = endOfWeek;

            FilterSchedule(); // Gọi hàm lọc
        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpFromDate.Value = DateTime.Today;
            dtpToDate.Value = DateTime.Today;

            // BẮT BUỘC phải gọi hàm này để thực hiện logic lọc
            FilterSchedule();
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            // Chú thích báo cáo: Thuật toán tự động xác định ngày bắt đầu và kết thúc của tháng hiện hành
            DateTime today = DateTime.Today;
            dtpFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpToDate.Value = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            FilterSchedule();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Chú thích báo cáo: Khôi phục trạng thái hiển thị ban đầu, nạp lại toàn bộ danh sách từ bộ nhớ đệm (_originalList)

            // 1. Reset lại 2 ô chọn ngày về ngày hiện tại (cho đồng bộ giao diện)
            dtpFromDate.Value = DateTime.Today;
            dtpToDate.Value = DateTime.Today;

            // 2. Hiển thị lại toàn bộ danh sách gốc
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = _originalList;

            // 3. Cập nhật lại nhãn tổng số buổi
            lblTotalSessions.Text = $"Tổng số: {_originalList.Count} buổi học";
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nếu không có dữ liệu thì không in
            if (dgvSchedule.Rows.Count == 0)
            {
                MessageBox.Show("Không có lịch học để in!");
                return;
            }

            // 2. Gán sự kiện vẽ trang vào đối tượng in
            printDoc.PrintPage -= printDoc_PrintPage; // Reset để tránh trùng lặp
            printDoc.PrintPage += printDoc_PrintPage;

            // 3. Cấu hình Preview và hiển thị
            printPreview.Document = printDoc;
            printPreview.WindowState = FormWindowState.Maximized; // Cho to toàn màn hình cho đẹp
            printPreview.ShowDialog();
        }
    }
}
