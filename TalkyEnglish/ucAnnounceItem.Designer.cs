namespace TalkyEnglish.GUI
{
    partial class ucAnnounceItem
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblContent = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblDate = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox1).BeginInit();
            SuspendLayout();
            // 
            // guna2CirclePictureBox1
            // 
            guna2CirclePictureBox1.ImageRotate = 0F;
            guna2CirclePictureBox1.Location = new Point(6, 5);
            guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            guna2CirclePictureBox1.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CirclePictureBox1.Size = new Size(69, 62);
            guna2CirclePictureBox1.TabIndex = 0;
            guna2CirclePictureBox1.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(81, 5);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(54, 22);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Tiêu đề";
            // 
            // lblContent
            // 
            lblContent.BackColor = Color.Transparent;
            lblContent.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblContent.Location = new Point(81, 33);
            lblContent.Name = "lblContent";
            lblContent.Size = new Size(65, 22);
            lblContent.TabIndex = 2;
            lblContent.Text = "Nội dung";
            // 
            // lblDate
            // 
            lblDate.BackColor = Color.Transparent;
            lblDate.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblDate.Location = new Point(269, 5);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(78, 22);
            lblDate.TabIndex = 3;
            lblDate.Text = "Ngày tháng";
            // 
            // ucAnnounceItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblDate);
            Controls.Add(lblContent);
            Controls.Add(lblTitle);
            Controls.Add(guna2CirclePictureBox1);
            Name = "ucAnnounceItem";
            Size = new Size(426, 70);
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblContent;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDate;
    }
}
