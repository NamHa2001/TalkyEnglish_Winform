using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TalkyEnglish.DAL;
using TalkyEnglish.DTO;

namespace TalkyEnglish.GUI
{
    public partial class ucDSSV : UserControl
    {
        private DataTable _dtStudents;

        public ucDSSV()
        {
            InitializeComponent();
        }

        private void ucDSSV_Load(object sender, EventArgs e)
        {
            ButtonEffectHelper.RemoveGrayEffect(this);
            LoadStudentData();
        }

        private void LoadStudentData()
        {
            try
            {
                _dtStudents = new DataTable();
                _dtStudents.Columns.Add("STT");
                _dtStudents.Columns.Add("MaHocVien");
                _dtStudents.Columns.Add("HoTen");
                _dtStudents.Columns.Add("Email");
                _dtStudents.Columns.Add("SoDT");
                _dtStudents.Columns.Add("GioiTinh");
                _dtStudents.Columns.Add("NgaySinh");
                _dtStudents.Columns.Add("KhoaHoc");
                _dtStudents.Columns.Add("NgayDangKy");

                int instructorId = SessionManager.CurrentUser?.UserID ?? 0;

                using (var db = new TalkyDbContext())
                {
                    var query = from e in db.Enrolments
                                join u in db.Users    on e.StudentID equals u.UserID
                                join c in db.Courses  on e.CourseID  equals c.CourseID
                                join a in db.TeachingAssignments on c.CourseID equals a.CourseID
                                where a.InstructorID == instructorId && u.Role == "Student"
                                orderby e.EnrollmentDate descending
                                select new
                                {
                                    u.StudentCode,
                                    u.FullName,
                                    u.Email,
                                    u.PhoneNumber,
                                    u.Gender,
                                    u.Birthday,
                                    c.CourseName,
                                    e.EnrollmentDate
                                };

                    int stt = 1;
                    foreach (var item in query.ToList())
                    {
                        _dtStudents.Rows.Add(
                            stt++,
                            item.StudentCode ?? "",
                            item.FullName ?? "",
                            item.Email ?? "",
                            item.PhoneNumber ?? "",
                            item.Gender ?? "",
                            item.Birthday?.ToString("dd/MM/yyyy") ?? "",
                            item.CourseName ?? "",
                            item.EnrollmentDate?.ToString("dd/MM/yyyy") ?? ""
                        );
                    }
                }

                dgvStudents.DataSource = _dtStudents;
                UpdateTotalCount();
                ApplyGridStyle();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh sách học viên: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotalCount()
        {
            txtTotalStudents.Text = dgvStudents.Rows.Count.ToString();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_dtStudents == null) return;

            string keyword = txtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(keyword))
            {
                dgvStudents.DataSource = _dtStudents;
                UpdateTotalCount();
                return;
            }

            // Filter an toàn bằng LINQ thay vì RowFilter (tránh injection)
            var filtered = _dtStudents.AsEnumerable()
                .Where(r =>
                    r["HoTen"].ToString().ToLower().Contains(keyword) ||
                    r["MaHocVien"].ToString().ToLower().Contains(keyword) ||
                    r["SoDT"].ToString().Contains(keyword))
                .ToList();

            if (filtered.Count > 0)
            {
                var result = filtered.CopyToDataTable();
                // Cập nhật lại STT
                for (int i = 0; i < result.Rows.Count; i++)
                    result.Rows[i]["STT"] = (i + 1).ToString();
                dgvStudents.DataSource = result;
            }
            else
            {
                dgvStudents.DataSource = _dtStudents.Clone();
            }
            UpdateTotalCount();
        }

        private void ApplyGridStyle()
        {
            dgvStudents.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
            dgvStudents.ThemeStyle.HeaderStyle.BackColor = ColorTranslator.FromHtml("#2563EB");
            dgvStudents.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            dgvStudents.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvStudents.ThemeStyle.RowsStyle.Height = 35;
            dgvStudents.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.ColumnHeadersHeight = 40;
        }

        private void guna2Panel3_Paint(object sender, PaintEventArgs e) { }
        private void guna2HtmlLabel2_Click(object sender, EventArgs e) { }
    }
}
