namespace TalkyEnglish.GUI
{
    partial class ucNotificationItem
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
            lblDate = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTargetType = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2CirclePictureBox1 = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            btnDelete = new Guna.UI2.WinForms.Guna2Button();
            btnDetail = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox1).BeginInit();
            SuspendLayout();
            // 
            // lblDate
            // 
            lblDate.BackColor = Color.Transparent;
            lblDate.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 0);
            lblDate.Location = new Point(77, 54);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(78, 22);
            lblDate.TabIndex = 7;
            lblDate.Text = "Ngày tháng";
            // 
            // lblTargetType
            // 
            lblTargetType.BackColor = Color.Transparent;
            lblTargetType.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTargetType.Location = new Point(317, 14);
            lblTargetType.Name = "lblTargetType";
            lblTargetType.Size = new Size(9, 22);
            lblTargetType.TabIndex = 6;
            lblTargetType.Text = "s";
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(77, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(54, 22);
            lblTitle.TabIndex = 5;
            lblTitle.Text = "Tiêu đề";
            // 
            // guna2CirclePictureBox1
            // 
            guna2CirclePictureBox1.ImageRotate = 0F;
            guna2CirclePictureBox1.Location = new Point(2, 14);
            guna2CirclePictureBox1.Name = "guna2CirclePictureBox1";
            guna2CirclePictureBox1.ShadowDecoration.CustomizableEdges = customizableEdges1;
            guna2CirclePictureBox1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CirclePictureBox1.Size = new Size(69, 62);
            guna2CirclePictureBox1.TabIndex = 4;
            guna2CirclePictureBox1.TabStop = false;
            // 
            // btnDelete
            // 
            btnDelete.BorderRadius = 5;
            btnDelete.BorderThickness = 1;
            btnDelete.CustomizableEdges = customizableEdges2;
            btnDelete.DisabledState.BorderColor = Color.DarkGray;
            btnDelete.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDelete.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDelete.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDelete.FillColor = Color.White;
            btnDelete.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnDelete.ForeColor = Color.Black;
            btnDelete.Location = new Point(511, 9);
            btnDelete.Name = "btnDelete";
            btnDelete.ShadowDecoration.CustomizableEdges = customizableEdges3;
            btnDelete.Size = new Size(78, 34);
            btnDelete.TabIndex = 8;
            btnDelete.Text = "Xóa";
            // 
            // btnDetail
            // 
            btnDetail.BorderRadius = 5;
            btnDetail.BorderThickness = 1;
            btnDetail.CustomizableEdges = customizableEdges4;
            btnDetail.DisabledState.BorderColor = Color.DarkGray;
            btnDetail.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDetail.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDetail.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDetail.FillColor = Color.White;
            btnDetail.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnDetail.ForeColor = Color.Black;
            btnDetail.Location = new Point(511, 49);
            btnDetail.Name = "btnDetail";
            btnDetail.ShadowDecoration.CustomizableEdges = customizableEdges5;
            btnDetail.Size = new Size(78, 34);
            btnDetail.TabIndex = 9;
            btnDetail.Text = "Chi tiết";
            // 
            // ucNotificationItem
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnDetail);
            Controls.Add(btnDelete);
            Controls.Add(lblDate);
            Controls.Add(lblTargetType);
            Controls.Add(lblTitle);
            Controls.Add(guna2CirclePictureBox1);
            Name = "ucNotificationItem";
            Size = new Size(616, 93);
            ((System.ComponentModel.ISupportInitialize)guna2CirclePictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Guna.UI2.WinForms.Guna2CirclePictureBox guna2CirclePictureBox1;
        private Guna.UI2.WinForms.Guna2Button btnDelete;
        private Guna.UI2.WinForms.Guna2Button btnDetail;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblDate;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblTargetType;
        public Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
    }
}
