using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TalkyEnglish.GUI
{
    public partial class ucCourseProgress : UserControl
    {
        public ucCourseProgress()
        {
            InitializeComponent();
            ButtonEffectHelper.RemoveGrayEffect(this);
        }
        // Hàm này giúp bạn thay đổi nội dung dòng khóa học dễ dàng
        public void SetData(string courseName, string teacherName, int progress, Color iconColor)
        {
            lblCourseName.Text = courseName;
            lblTeacher.Text = "Giáo viên: " + teacherName;
            progressBar.Value = progress;
            lblPercent.Text = progress + "%";
            panelIcon.FillColor = iconColor; // Đổi màu icon cho sinh động
        }
    }
}
