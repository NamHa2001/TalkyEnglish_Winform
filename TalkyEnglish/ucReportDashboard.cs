using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TalkyEnglish.GUI
{
    public partial class ucReportDashboard : UserControl
    {
        public ucReportDashboard()
        {
            InitializeComponent();
        }

        private void guna2RadioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void LoadSubReport(UserControl subControl)
        {
            pnlReportContainer.Controls.Clear(); // Dọn dẹp sân khấu
            subControl.Dock = DockStyle.Fill;    // Phóng to full màn hình
            pnlReportContainer.Controls.Add(subControl);
        }
        private void ucReportDashboard_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
            LoadSubReport(new ucReportRevenue());
        }
        public void LoadData()
        {
            LoadSubReport(new ucReportRevenue());

            // 2. Tương lai: Nếu bro có làm mấy cái Thẻ KPI (Tổng tiền, Tổng học viên) 
            // trên cái trang gốc này, thì bro sẽ gọi Tầng BUS ra để đếm số lượng 
            // và gán vào Label ở ngay đây nhé! 
            // Ví dụ: lblTongHocVien.Text = _thongKeBUS.DemTongHocVien().ToString();
        }
        private void btnMenuDoanhThu_Click(object sender, EventArgs e)
        {
            LoadSubReport(new ucReportRevenue());
        }
    }  
}
