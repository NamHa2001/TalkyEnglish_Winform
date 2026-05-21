using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing; // Thư viện để điều khiển máy in
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;
namespace TalkyEnglish.GUI
{
    
    public partial class ucStudentSchedule : UserControl
    {
        private readonly ScheduleBUS _scheduleBus = new ScheduleBUS();
        // Đối tượng quản lý bản in
        private PrintDocument printDoc = new PrintDocument();
        private PrintPreviewDialog printPreview = new PrintPreviewDialog();
        // Đây là nơi khai báo biến lưu trữ dữ liệu gốc cho toàn bộ UC
        private List<ScheduleDTO> _originalList = new List<ScheduleDTO>();
        public ucStudentSchedule()
        {
            InitializeComponent();
            ButtonEffectHelper.RemoveGrayEffect(this);
            LoadScheduleData();
        }

        private void LoadScheduleData()
        {
            try
            {
                // 1. Kiểm tra xem có ai đang đăng nhập không
                if (SessionManager.CurrentUser != null)
                {
                    int studentId = SessionManager.CurrentUser.UserID;

                    // 2. Lấy dữ liệu thật từ BUS và đổ vào List gốc
                    // Vì hàm GetStudentSchedule trả về List<object>, ta gán vào DataSource
                    var data = _scheduleBus.GetStudentSchedule(studentId);

                    dgvMySchedule.DataSource = data;
                    // 1. Định dạng cột Giờ Bắt Đầu
                    if (dgvMySchedule.Columns["BatDau"] != null)
                    {
                        dgvMySchedule.Columns["BatDau"].HeaderText = "Giờ Bắt Đầu";
                        // Định dạng hh:mm để mất đống số 0 phía sau
                        dgvMySchedule.Columns["BatDau"].DefaultCellStyle.Format = @"hh\:mm";
                    }

                    // 2. Định dạng cột Giờ Kết Thúc
                    if (dgvMySchedule.Columns["KetThuc"] != null)
                    {
                        dgvMySchedule.Columns["KetThuc"].HeaderText = "Giờ Kết Thúc";
                        dgvMySchedule.Columns["KetThuc"].DefaultCellStyle.Format = @"hh\:mm";
                    }
                    // 3. Cập nhật nhãn tổng số buổi dựa trên số dòng trong Grid
                    lblTotalSessions.Text = $"Tổng số: {dgvMySchedule.Rows.Count} buổi học";

                    // 4. Định dạng lại tiêu đề cột cho khớp với dữ liệu thật từ DAL
                    if (dgvMySchedule.Columns["TenKhoaHoc"] != null) dgvMySchedule.Columns["TenKhoaHoc"].HeaderText = "Tên khóa học";
                    if (dgvMySchedule.Columns["Thu"] != null) dgvMySchedule.Columns["Thu"].HeaderText = "Thứ";
                    if (dgvMySchedule.Columns["BatDau"] != null) dgvMySchedule.Columns["BatDau"].HeaderText = "Bắt đầu";
                    if (dgvMySchedule.Columns["KetThuc"] != null) dgvMySchedule.Columns["KetThuc"].HeaderText = "Kết thúc";
                    if (dgvMySchedule.Columns["Phong"] != null) dgvMySchedule.Columns["Phong"].HeaderText = "Phòng";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp lịch học: " + ex.Message);
            }
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

            // 3. Duyệt qua DataGridView để vẽ từng dòng dữ liệu thật
            foreach (DataGridViewRow row in dgvMySchedule.Rows)
            {
                if (row.IsNewRow) continue;

                // SỬA TẠI ĐÂY: Dùng đúng tên cột mà DAL đã trả về
                string thu = row.Cells["Thu"]?.Value?.ToString() ?? "";
                string course = row.Cells["TenKhoaHoc"]?.Value?.ToString() ?? "";
                string room = row.Cells["Phong"]?.Value?.ToString() ?? "";
                string time = $"{row.Cells["BatDau"]?.Value} - {row.Cells["KetThuc"]?.Value}";

                g.DrawString(thu, fontBody, Brushes.Black, 50, y + 5);
                g.DrawString(course, fontBody, Brushes.Black, 150, y + 5);
                g.DrawString(time, fontBody, Brushes.Black, 450, y + 5); // Thay teacher bằng giờ học cho học viên dễ nhìn
                g.DrawString(room, fontBody, Brushes.Black, 650, y + 5);

                y += 25;
                g.DrawLine(Pens.LightGray, 50, y, 750, y);
            }
        }

        private int GetVisibleRowCount()
        {
            int count = 0;
            foreach (DataGridViewRow r in dgvMySchedule.Rows) if (r.Visible) count++;
            return count;
        }

        private string GetVietnameseDay(DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Monday: return "Thứ Hai"; // Sửa lại cho đúng chữ trong SQL của bro
                case DayOfWeek.Tuesday: return "Thứ Ba";
                case DayOfWeek.Wednesday: return "Thứ Tư";
                case DayOfWeek.Thursday: return "Thứ Năm";
                case DayOfWeek.Friday: return "Thứ Sáu";
                case DayOfWeek.Saturday: return "Thứ Bảy";
                case DayOfWeek.Sunday: return "Chủ Nhật";
                default: return "";
            }
        }
        private void FilterSchedule()
        {
            try
            {
                DateTime fromDate = dtpFromDate.Value.Date;
                DateTime toDate = dtpToDate.Value.Date;

                // 1. Xác định danh sách các "Thứ" xuất hiện trong khoảng ngày học viên chọn
                List<string> thuInRange = new List<string>();
                for (DateTime date = fromDate; date <= toDate; date = date.AddDays(1))
                {
                    string vnDay = GetVietnameseDay(date.DayOfWeek);
                    if (!thuInRange.Contains(vnDay)) thuInRange.Add(vnDay);
                }

                // 2. Lọc trực tiếp trên DataGridView
                // Chúng ta tạm ngưng kết nối để ẩn dòng không khớp
                CurrencyManager cm = (CurrencyManager)BindingContext[dgvMySchedule.DataSource];
                cm.SuspendBinding();

                foreach (DataGridViewRow row in dgvMySchedule.Rows)
                {
                    if (row.IsNewRow) continue;

                    // Lấy giá trị cột "Thứ" trong bảng của bro
                    string cellValue = row.Cells["Thu"]?.Value?.ToString();

                    // Nếu "Thứ" đó nằm trong khoảng ngày đã chọn thì hiện, ngược lại thì ẩn
                    row.Visible = thuInRange.Contains(cellValue);
                }

                cm.ResumeBinding();
                lblTotalSessions.Text = $"Tổng số: {GetVisibleRowCount()} buổi học";
            }
            catch { /* Tránh lỗi khi bảng trống */ }
        }

        private void btnThisWeek_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            // Tính toán ngày Thứ Hai của tuần này
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-1 * diff).Date;

            // Thứ Hai đến Chủ Nhật (cộng thêm 6 ngày)
            dtpFromDate.Value = startOfWeek;
            dtpToDate.Value = startOfWeek.AddDays(6);

            FilterSchedule();
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
            DateTime today = DateTime.Today;
            // Ngày đầu tháng
            dtpFromDate.Value = new DateTime(today.Year, today.Month, 1);
            // Ngày cuối tháng
            dtpToDate.Value = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

            FilterSchedule();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // 1. Reset lại 2 ô chọn ngày về ngày hiện tại
            dtpFromDate.Value = DateTime.Today;
            dtpToDate.Value = DateTime.Today;

            // 2. Nạp lại dữ liệu thật từ SQL
            LoadScheduleData();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra nếu không có dữ liệu thì không in
            if (dgvMySchedule.Rows.Count == 0)
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
