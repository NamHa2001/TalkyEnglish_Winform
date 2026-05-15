using Microsoft.Reporting.WinForms;
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
    public partial class ucReportRevenue : UserControl
    {
        private ReportBUS _reportBUS = new ReportBUS();
        private ReportViewer rptDoanhThu;
        public ucReportRevenue()
        {
            InitializeComponent();
            SetupReportViewer();
        }

        private void UpdatePickerVisibility()
        {
            // Kiểm tra xem người dùng có đang chọn lọc tự do (không chọn 3 cái trên) hay không
            // Nếu chọn Ngày/Tuần/Tháng thì isRangeSelection = false
            bool isRangeSelection = !radTheoNgay.Checked && !radTheoTuan.Checked && !radTheoThang.Checked;

            // Thay vì dùng .Visible (ẩn), mình dùng .Enabled (vô hiệu hóa)
            // Khi chọn Theo ngày/tuần/tháng -> Ô 'Đến ngày' sẽ bị mờ đi, không cho chỉnh
            dtpDenNgay.Enabled = isRangeSelection;

            // Nếu bro có cái Label chữ "Đến" thì cũng cho nó mờ theo cho đồng bộ
            // lblDenNgay.Enabled = isRangeSelection; 
        }
        private void SetupReportViewer()
        {
            rptDoanhThu = new ReportViewer();
            rptDoanhThu.Dock = DockStyle.Fill;

            // Nhét nó vào cái Panel chứa báo cáo (ví dụ tên là pnlHienThi)
            // Nếu không có Panel thì dùng: this.Controls.Add(rptDoanhThu);
            pnlHienThi.Controls.Add(rptDoanhThu);

            rptDoanhThu.SendToBack();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Lấy khoảng thời gian đã được tính toán logic
                var range = GetReportRange();
                DateTime tuNgay = range.fromDate;
                DateTime denNgay = range.toDate;

                // 2. Gọi BUS lấy dữ liệu
                List<RevenueReportDTO> lstData = _reportBUS.GetRevenueByDate(tuNgay, denNgay);

                if (lstData == null || lstData.Count == 0)
                {
                    MessageBox.Show($"Không có doanh thu từ {tuNgay:dd/MM/yyyy} đến {denNgay:dd/MM/yyyy}",
                                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    rptDoanhThu.LocalReport.DataSources.Clear();
                    rptDoanhThu.RefreshReport();
                    return;
                }

                // 3. Cấu hình file báo cáo
                // Đảm bảo file rptDoanhThu.rdlc đã được đặt thuộc tính "Copy to Output Directory = Copy always"
                string reportPath = Path.Combine(Application.StartupPath, "rptDoanhThu.rdlc");
                rptDoanhThu.LocalReport.ReportPath = reportPath;

                ReportDataSource rds = new ReportDataSource("DataSetDoanhThu", lstData);
                rptDoanhThu.LocalReport.DataSources.Clear();
                rptDoanhThu.LocalReport.DataSources.Add(rds);

                // 4. Hiển thị
                rptDoanhThu.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private (DateTime fromDate, DateTime toDate) GetReportRange()
        {
            // Lấy giá trị từ Picker chính
            DateTime selectedDate = dtpTuNgay.Value.Date;

            if (radTheoNgay.Checked)
            {
                return (selectedDate, selectedDate);
            }
            else if (radTheoTuan.Checked)
            {
                // Tính Thứ 2 của tuần chứa ngày đang chọn
                int diff = (7 + (selectedDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                DateTime startOfWeek = selectedDate.AddDays(-1 * diff);
                return (startOfWeek, startOfWeek.AddDays(6));
            }
            else if (radTheoThang.Checked)
            {
                // Từ ngày 1 đến ngày cuối cùng của tháng đang chọn
                DateTime startOfMonth = new DateTime(selectedDate.Year, selectedDate.Month, 1);
                DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                return (startOfMonth, endOfMonth);
            }

            // Trường hợp lọc tự do theo cả 2 ô Picker (nếu không chọn 3 cái trên)
            return (dtpTuNgay.Value.Date, dtpDenNgay.Value.Date);
        }
        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radTheoNgay_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePickerVisibility();
        }

        private void radTheoTuan_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePickerVisibility();
        }

        private void radTheoThang_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePickerVisibility();
        }
    }

}
