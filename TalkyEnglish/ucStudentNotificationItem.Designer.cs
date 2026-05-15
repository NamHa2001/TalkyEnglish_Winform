namespace TalkyEnglish.GUI
{
    partial class ucStudentNotificationItem
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            btnDetails = new Guna.UI2.WinForms.Guna2Button();
            btnMarkAsRead = new Guna.UI2.WinForms.Guna2Button();
            lblDate = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblSender = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlPriority = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            lblContent = new Guna.UI2.WinForms.Guna2HtmlLabel();
            ((System.ComponentModel.ISupportInitialize)pnlPriority).BeginInit();
            SuspendLayout();
            // 
            // btnDetails
            // 
            btnDetails.BorderColor = Color.FromArgb(251, 191, 36);
            btnDetails.BorderRadius = 5;
            btnDetails.BorderThickness = 1;
            btnDetails.CustomizableEdges = customizableEdges1;
            btnDetails.DisabledState.BorderColor = Color.DarkGray;
            btnDetails.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDetails.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDetails.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDetails.FillColor = Color.White;
            btnDetails.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnDetails.ForeColor = Color.FromArgb(251, 191, 36);
            btnDetails.Location = new Point(932, 49);
            btnDetails.Name = "btnDetails";
            btnDetails.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnDetails.Size = new Size(78, 34);
            btnDetails.TabIndex = 15;
            btnDetails.Text = "Chi tiết";
            // 
            // btnMarkAsRead
            // 
            btnMarkAsRead.BorderRadius = 5;
            btnMarkAsRead.BorderThickness = 1;
            btnMarkAsRead.CustomizableEdges = customizableEdges3;
            btnMarkAsRead.DisabledState.BorderColor = Color.DarkGray;
            btnMarkAsRead.DisabledState.CustomBorderColor = Color.DarkGray;
            btnMarkAsRead.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnMarkAsRead.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnMarkAsRead.FillColor = Color.FromArgb(34, 197, 94);
            btnMarkAsRead.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnMarkAsRead.ForeColor = Color.White;
            btnMarkAsRead.Location = new Point(932, 9);
            btnMarkAsRead.Name = "btnMarkAsRead";
            btnMarkAsRead.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnMarkAsRead.Size = new Size(78, 34);
            btnMarkAsRead.TabIndex = 14;
            btnMarkAsRead.Text = "Đã đọc";
            btnMarkAsRead.Click += btnMarkAsRead_Click;
            // 
            // lblDate
            // 
            lblDate.BackColor = Color.Transparent;
            lblDate.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblDate.Location = new Point(802, 16);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(78, 22);
            lblDate.TabIndex = 13;
            lblDate.Text = "Ngày tháng";
            // 
            // lblSender
            // 
            lblSender.BackColor = Color.Transparent;
            lblSender.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSender.Location = new Point(670, 16);
            lblSender.Name = "lblSender";
            lblSender.Size = new Size(71, 22);
            lblSender.TabIndex = 12;
            lblSender.Text = "Người gửi";
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(93, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(54, 22);
            lblTitle.TabIndex = 11;
            lblTitle.Text = "Tiêu đề";
            // 
            // pnlPriority
            // 
            pnlPriority.Image = Properties.Resources.megaphone;
            pnlPriority.ImageRotate = 0F;
            pnlPriority.Location = new Point(22, 18);
            pnlPriority.Name = "pnlPriority";
            pnlPriority.ShadowDecoration.CustomizableEdges = customizableEdges5;
            pnlPriority.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            pnlPriority.Size = new Size(50, 50);
            pnlPriority.SizeMode = PictureBoxSizeMode.StretchImage;
            pnlPriority.TabIndex = 10;
            pnlPriority.TabStop = false;
            // 
            // lblContent
            // 
            lblContent.BackColor = Color.Transparent;
            lblContent.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblContent.Location = new Point(93, 49);
            lblContent.Name = "lblContent";
            lblContent.Size = new Size(68, 22);
            lblContent.TabIndex = 16;
            lblContent.Text = "Nội dung:";
            // 
            // ucStudentNotificationItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(lblContent);
            Controls.Add(btnDetails);
            Controls.Add(btnMarkAsRead);
            Controls.Add(lblDate);
            Controls.Add(lblSender);
            Controls.Add(lblTitle);
            Controls.Add(pnlPriority);
            Name = "ucStudentNotificationItem";
            Size = new Size(1034, 95);
            ((System.ComponentModel.ISupportInitialize)pnlPriority).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btnDetails;
        private Guna.UI2.WinForms.Guna2Button btnMarkAsRead;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblDate;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblSender;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2CirclePictureBox pnlPriority;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblContent;
    }
}
