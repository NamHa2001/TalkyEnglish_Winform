namespace TalkyEnglish.GUI
{
    partial class ucCourseProgress
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            progressBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            panelIcon = new Guna.UI2.WinForms.Guna2Chip();
            lblPercent = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTeacher = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblCourseName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox1).BeginInit();
            SuspendLayout();
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.FromArgb(235, 242, 255);
            guna2Panel1.BorderColor = Color.FromArgb(224, 224, 224);
            guna2Panel1.Controls.Add(progressBar);
            guna2Panel1.Controls.Add(panelIcon);
            guna2Panel1.Controls.Add(lblPercent);
            guna2Panel1.Controls.Add(lblTeacher);
            guna2Panel1.Controls.Add(lblCourseName);
            guna2Panel1.Controls.Add(guna2CirclePictureBox1);
            guna2Panel1.CustomizableEdges = customizableEdges6;
            guna2Panel1.Location = new Point(3, 0);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges7;
            guna2Panel1.Size = new Size(1040, 89);
            guna2Panel1.TabIndex = 0;
            // 
            // progressBar
            // 
            progressBar.BorderRadius = 5;
            progressBar.CustomizableEdges = customizableEdges1;
            progressBar.FillColor = Color.FromArgb(94, 148, 255);
            progressBar.Location = new Point(387, 42);
            progressBar.Name = "progressBar";
            progressBar.ProgressColor = Color.FromArgb(18, 110, 226);
            progressBar.ProgressColor2 = Color.FromArgb(241, 243, 245);
            progressBar.ShadowDecoration.CustomizableEdges = customizableEdges2;
            progressBar.Size = new Size(351, 10);
            progressBar.TabIndex = 11;
            progressBar.Text = "guna2ProgressBar1";
            progressBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // panelIcon
            // 
            panelIcon.BorderColor = Color.Silver;
            panelIcon.CustomizableEdges = customizableEdges3;
            panelIcon.FillColor = Color.FromArgb(192, 255, 192);
            panelIcon.Font = new Font("Segoe UI", 9.5F);
            panelIcon.ForeColor = Color.DarkGreen;
            panelIcon.IsClosable = false;
            panelIcon.Location = new Point(886, 25);
            panelIcon.Name = "panelIcon";
            panelIcon.ShadowDecoration.CustomizableEdges = customizableEdges4;
            panelIcon.Size = new Size(120, 41);
            panelIcon.TabIndex = 10;
            panelIcon.Text = "Đang học";
            // 
            // lblPercent
            // 
            lblPercent.BackColor = Color.Transparent;
            lblPercent.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPercent.Location = new Point(766, 26);
            lblPercent.Name = "lblPercent";
            lblPercent.Size = new Size(20, 30);
            lblPercent.TabIndex = 9;
            lblPercent.Text = "%";
            // 
            // lblTeacher
            // 
            lblTeacher.BackColor = Color.Transparent;
            lblTeacher.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTeacher.Location = new Point(115, 48);
            lblTeacher.Name = "lblTeacher";
            lblTeacher.Size = new Size(89, 22);
            lblTeacher.TabIndex = 8;
            lblTeacher.Text = "tên giáo viên";
            // 
            // lblCourseName
            // 
            lblCourseName.BackColor = Color.Transparent;
            lblCourseName.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCourseName.ForeColor = SystemColors.ActiveCaptionText;
            lblCourseName.Location = new Point(115, 17);
            lblCourseName.Name = "lblCourseName";
            lblCourseName.Size = new Size(106, 25);
            lblCourseName.TabIndex = 7;
            lblCourseName.Text = "tên khóa học";
            // 
            // guna2CirclePictureBox1
            // 
            guna2CirclePictureBox1.Image = Properties.Resources.toeic;
            guna2CirclePictureBox1.ImageRotate = 0F;
            guna2CirclePictureBox1.Location = new Point(35, 13);
            guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            guna2CirclePictureBox1.ShadowDecoration.CustomizableEdges = customizableEdges5;
            guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CirclePictureBox1.Size = new Size(64, 62);
            guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            guna2CirclePictureBox1.TabIndex = 6;
            guna2CirclePictureBox1.TabStop = false;
            // 
            // ucCourseProgress
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(guna2Panel1);
            Name = "ucCourseProgress";
            Size = new Size(1041, 90);
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2ProgressBar progressBar;
        private Guna.UI2.WinForms.Guna2Chip panelIcon;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPercent;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTeacher;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCourseName;
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
    }
}
