using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TalkyEnglish.BUS;
using TalkyEnglish.DTO;
using Excel = Microsoft.Office.Interop.Excel;

namespace TalkyEnglish.GUI
{
    public partial class ucTeacherSchedule : UserControl
    {
        private readonly ScheduleBUS _scheduleBus = new ScheduleBUS();
        private List<ScheduleDTO> _mySchedules = new List<ScheduleDTO>();

        public ucTeacherSchedule()
        {
            InitializeComponent();
        }

        private void ucTeacherSchedule_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);

            if (SessionManager.CurrentUser == null) return;

            dgvSchedule.AutoGenerateColumns = false;
            _mySchedules = _scheduleBus.GetSchedulesByTeacher(SessionManager.CurrentUser.UserID);
            dgvSchedule.DataSource = _mySchedules;
            UpdateTotalSessions();
        }

        private void btnFilterToday_Click(object sender, EventArgs e)
        {
            string dayVie = DayShort(DateTime.Today.DayOfWeek.ToString());
            var filtered = _mySchedules.Where(s => s.DayOfWeek == dayVie).ToList();
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = filtered;
            UpdateTotalSessions();
        }

        private void btnFilterAll_Click(object sender, EventArgs e)
        {
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = _mySchedules;
            UpdateTotalSessions();
        }

        private void btnFilterWeek_Click(object sender, EventArgs e)
        {
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = _mySchedules;
            UpdateTotalSessions();
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = _mySchedules;
            UpdateTotalSessions();
        }

        private void UpdateTotalSessions()
        {
            lblTotalSessions.Text = $"Tổng số ca dạy: {dgvSchedule.Rows.Count}";
        }

        private void btnPrintSchedule_Click(object sender, EventArgs e)
        {
            if (dgvSchedule.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu lịch dạy để xuất.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Excel.Application excelApp = null;
            try
            {
                excelApp = new Excel.Application();
                excelApp.Workbooks.Add();
                Excel._Worksheet workSheet = excelApp.ActiveSheet;

                for (int i = 0; i < dgvSchedule.Columns.Count; i++)
                {
                    if (dgvSchedule.Columns[i].Visible)
                    {
                        workSheet.Cells[1, i + 1] = dgvSchedule.Columns[i].HeaderText;
                        workSheet.Cells[1, i + 1].Font.Bold = true;
                    }
                }

                for (int i = 0; i < dgvSchedule.Rows.Count; i++)
                {
                    for (int j = 0; j < dgvSchedule.Columns.Count; j++)
                    {
                        if (dgvSchedule.Columns[j].Visible)
                            workSheet.Cells[i + 2, j + 1] = dgvSchedule.Rows[i].Cells[j].Value?.ToString();
                    }
                }

                workSheet.Columns.AutoFit();
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi xuất dữ liệu: " + ex.Message, "Lỗi hệ thống",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                excelApp?.Quit();
            }
        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e) { }

        private void dgvSchedule_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

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
