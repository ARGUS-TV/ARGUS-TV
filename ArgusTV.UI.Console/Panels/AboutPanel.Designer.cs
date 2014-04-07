namespace ArgusTV.UI.Console.Panels
{
    partial class AboutPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutPanel));
            this._descriptionLabel = new System.Windows.Forms.Label();
            this._linkLabelDot_i = new System.Windows.Forms.LinkLabel();
            this._labelDot_i = new System.Windows.Forms.Label();
            this._groupBoxDot_i = new System.Windows.Forms.GroupBox();
            this._pictureBoxDot_i = new System.Windows.Forms.PictureBox();
            this._logoPictureBox = new System.Windows.Forms.PictureBox();
            this._grpDonations = new System.Windows.Forms.GroupBox();
            this._labelDonations = new System.Windows.Forms.Label();
            this._pictureBoxDonations = new System.Windows.Forms.PictureBox();
            this._linkLabelDonations = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._logoPanel = new System.Windows.Forms.Panel();
            this._groupBoxDot_i.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxDot_i)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._logoPictureBox)).BeginInit();
            this._grpDonations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxDonations)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this._logoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _descriptionLabel
            // 
            this._descriptionLabel.AutoSize = true;
            this._descriptionLabel.Location = new System.Drawing.Point(384, 43);
            this._descriptionLabel.Name = "_descriptionLabel";
            this._descriptionLabel.Size = new System.Drawing.Size(294, 20);
            this._descriptionLabel.TabIndex = 0;
            this._descriptionLabel.Text = "has been designed and implemented by:";
            // 
            // _linkLabelDot_i
            // 
            this._linkLabelDot_i.AutoSize = true;
            this._linkLabelDot_i.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._linkLabelDot_i.Location = new System.Drawing.Point(10, 85);
            this._linkLabelDot_i.Name = "_linkLabelDot_i";
            this._linkLabelDot_i.Size = new System.Drawing.Size(146, 18);
            this._linkLabelDot_i.TabIndex = 3;
            this._linkLabelDot_i.TabStop = true;
            this._linkLabelDot_i.Tag = "http://www.dot-i.com";
            this._linkLabelDot_i.Text = "http://www.dot-i.com";
            this._linkLabelDot_i.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._linkLabelDot_i_LinkClicked);
            // 
            // _labelDot_i
            // 
            this._labelDot_i.Font = new System.Drawing.Font("Arial", 8F);
            this._labelDot_i.Location = new System.Drawing.Point(174, 14);
            this._labelDot_i.Name = "_labelDot_i";
            this._labelDot_i.Size = new System.Drawing.Size(540, 133);
            this._labelDot_i.TabIndex = 5;
            this._labelDot_i.Text = resources.GetString("_labelDot_i.Text");
            // 
            // _groupBoxDot_i
            // 
            this._groupBoxDot_i.Controls.Add(this._labelDot_i);
            this._groupBoxDot_i.Controls.Add(this._pictureBoxDot_i);
            this._groupBoxDot_i.Controls.Add(this._linkLabelDot_i);
            this._groupBoxDot_i.Location = new System.Drawing.Point(20, 98);
            this._groupBoxDot_i.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this._groupBoxDot_i.Name = "_groupBoxDot_i";
            this._groupBoxDot_i.Size = new System.Drawing.Size(720, 150);
            this._groupBoxDot_i.TabIndex = 8;
            this._groupBoxDot_i.TabStop = false;
            // 
            // _pictureBoxDot_i
            // 
            this._pictureBoxDot_i.Cursor = System.Windows.Forms.Cursors.Hand;
            this._pictureBoxDot_i.Image = ((System.Drawing.Image)(resources.GetObject("_pictureBoxDot_i.Image")));
            this._pictureBoxDot_i.Location = new System.Drawing.Point(12, 16);
            this._pictureBoxDot_i.Name = "_pictureBoxDot_i";
            this._pictureBoxDot_i.Size = new System.Drawing.Size(120, 66);
            this._pictureBoxDot_i.TabIndex = 2;
            this._pictureBoxDot_i.TabStop = false;
            this._pictureBoxDot_i.Click += new System.EventHandler(this._pictureBoxDot_i_Click);
            // 
            // _logoPictureBox
            // 
            this._logoPictureBox.Image = global::ArgusTV.UI.Console.Properties.Resources.AboutLogo;
            this._logoPictureBox.Location = new System.Drawing.Point(10, 3);
            this._logoPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this._logoPictureBox.Name = "_logoPictureBox";
            this._logoPictureBox.Size = new System.Drawing.Size(377, 80);
            this._logoPictureBox.TabIndex = 9;
            this._logoPictureBox.TabStop = false;
            // 
            // _grpDonations
            // 
            this._grpDonations.Controls.Add(this._labelDonations);
            this._grpDonations.Controls.Add(this._pictureBoxDonations);
            this._grpDonations.Controls.Add(this._linkLabelDonations);
            this._grpDonations.Location = new System.Drawing.Point(20, 288);
            this._grpDonations.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this._grpDonations.Name = "_grpDonations";
            this._grpDonations.Size = new System.Drawing.Size(720, 150);
            this._grpDonations.TabIndex = 8;
            this._grpDonations.TabStop = false;
            // 
            // _labelDonations
            // 
            this._labelDonations.ForeColor = System.Drawing.Color.Maroon;
            this._labelDonations.Location = new System.Drawing.Point(174, 19);
            this._labelDonations.Name = "_labelDonations";
            this._labelDonations.Size = new System.Drawing.Size(540, 102);
            this._labelDonations.TabIndex = 6;
            this._labelDonations.Text = resources.GetString("_labelDonations.Text");
            // 
            // _pictureBoxDonations
            // 
            this._pictureBoxDonations.Cursor = System.Windows.Forms.Cursors.Hand;
            this._pictureBoxDonations.Image = ((System.Drawing.Image)(resources.GetObject("_pictureBoxDonations.Image")));
            this._pictureBoxDonations.Location = new System.Drawing.Point(12, 15);
            this._pictureBoxDonations.Name = "_pictureBoxDonations";
            this._pictureBoxDonations.Size = new System.Drawing.Size(120, 82);
            this._pictureBoxDonations.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pictureBoxDonations.TabIndex = 1;
            this._pictureBoxDonations.TabStop = false;
            this._pictureBoxDonations.Tag = "";
            this._pictureBoxDonations.Click += new System.EventHandler(this._pictureBoxDonations_Click);
            // 
            // _linkLabelDonations
            // 
            this._linkLabelDonations.AutoSize = true;
            this._linkLabelDonations.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._linkLabelDonations.Location = new System.Drawing.Point(10, 100);
            this._linkLabelDonations.Name = "_linkLabelDonations";
            this._linkLabelDonations.Size = new System.Drawing.Size(164, 18);
            this._linkLabelDonations.TabIndex = 4;
            this._linkLabelDonations.TabStop = true;
            this._linkLabelDonations.Tag = "http://www.argus-tv.com/";
            this._linkLabelDonations.Text = "Donate to ARGUS TV!";
            this._linkLabelDonations.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._linkLabelDonations_LinkClicked);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._logoPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._groupBoxDot_i, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._grpDonations, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(820, 540);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // _logoPanel
            // 
            this._logoPanel.Controls.Add(this._descriptionLabel);
            this._logoPanel.Controls.Add(this._logoPictureBox);
            this._logoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logoPanel.Location = new System.Drawing.Point(0, 0);
            this._logoPanel.Margin = new System.Windows.Forms.Padding(0);
            this._logoPanel.Name = "_logoPanel";
            this._logoPanel.Size = new System.Drawing.Size(820, 80);
            this._logoPanel.TabIndex = 0;
            // 
            // AboutPanel
            // 
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AboutPanel";
            this.Size = new System.Drawing.Size(820, 540);
            this.Load += new System.EventHandler(this.AboutPanel_Load);
            this._groupBoxDot_i.ResumeLayout(false);
            this._groupBoxDot_i.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxDot_i)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._logoPictureBox)).EndInit();
            this._grpDonations.ResumeLayout(false);
            this._grpDonations.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBoxDonations)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this._logoPanel.ResumeLayout(false);
            this._logoPanel.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Label _descriptionLabel;
        private System.Windows.Forms.PictureBox _pictureBoxDot_i;
        private System.Windows.Forms.LinkLabel _linkLabelDot_i;
        private System.Windows.Forms.Label _labelDot_i;
        private System.Windows.Forms.GroupBox _groupBoxDot_i;
        private System.Windows.Forms.PictureBox _logoPictureBox;
        private System.Windows.Forms.GroupBox _grpDonations;
        private System.Windows.Forms.Label _labelDonations;
        private System.Windows.Forms.PictureBox _pictureBoxDonations;
        private System.Windows.Forms.LinkLabel _linkLabelDonations;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel _logoPanel;

    }
}
