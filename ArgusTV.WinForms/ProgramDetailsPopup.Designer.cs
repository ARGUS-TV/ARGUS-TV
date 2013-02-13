namespace ArgusTV.WinForms
{
    partial class ProgramDetailsPopup
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._detailsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._titleLabel = new System.Windows.Forms.Label();
            this._subTitleLabel = new System.Windows.Forms.Label();
            this._descriptionLabel = new System.Windows.Forms.Label();
            this._timeLabel = new System.Windows.Forms.Label();
            this._topPanel = new System.Windows.Forms.Panel();
            this._closeButton = new System.Windows.Forms.Button();
            this._topTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this._channelLabel = new System.Windows.Forms.Label();
            this._dateLabel = new System.Windows.Forms.Label();
            this._widescreenLabel = new System.Windows.Forms.Label();
            this._detailsPanel = new System.Windows.Forms.Panel();
            this._detailsTableLayoutPanel.SuspendLayout();
            this._topPanel.SuspendLayout();
            this._topTableLayoutPanel.SuspendLayout();
            this._detailsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _detailsTableLayoutPanel
            // 
            this._detailsTableLayoutPanel.BackColor = System.Drawing.Color.White;
            this._detailsTableLayoutPanel.ColumnCount = 2;
            this._detailsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this._detailsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._detailsTableLayoutPanel.Controls.Add(this._titleLabel, 1, 1);
            this._detailsTableLayoutPanel.Controls.Add(this._subTitleLabel, 1, 2);
            this._detailsTableLayoutPanel.Controls.Add(this._descriptionLabel, 1, 3);
            this._detailsTableLayoutPanel.Controls.Add(this._timeLabel, 0, 1);
            this._detailsTableLayoutPanel.Controls.Add(this._topPanel, 0, 0);
            this._detailsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detailsTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._detailsTableLayoutPanel.Name = "_detailsTableLayoutPanel";
            this._detailsTableLayoutPanel.RowCount = 4;
            this._detailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._detailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._detailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._detailsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._detailsTableLayoutPanel.Size = new System.Drawing.Size(498, 172);
            this._detailsTableLayoutPanel.TabIndex = 0;
            this._detailsTableLayoutPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _titleLabel
            // 
            this._titleLabel.AutoSize = true;
            this._titleLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this._titleLabel.Location = new System.Drawing.Point(103, 36);
            this._titleLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this._titleLabel.Name = "_titleLabel";
            this._titleLabel.Size = new System.Drawing.Size(59, 14);
            this._titleLabel.TabIndex = 0;
            this._titleLabel.Text = "Program";
            this._titleLabel.UseMnemonic = false;
            this._titleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _subTitleLabel
            // 
            this._subTitleLabel.AutoSize = true;
            this._subTitleLabel.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._subTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._subTitleLabel.Location = new System.Drawing.Point(103, 62);
            this._subTitleLabel.Margin = new System.Windows.Forms.Padding(3, 1, 3, 0);
            this._subTitleLabel.Name = "_subTitleLabel";
            this._subTitleLabel.Size = new System.Drawing.Size(43, 11);
            this._subTitleLabel.TabIndex = 1;
            this._subTitleLabel.Text = "Episode";
            this._subTitleLabel.UseMnemonic = false;
            this._subTitleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _descriptionLabel
            // 
            this._descriptionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._descriptionLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._descriptionLabel.Location = new System.Drawing.Point(103, 79);
            this._descriptionLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this._descriptionLabel.Name = "_descriptionLabel";
            this._descriptionLabel.Size = new System.Drawing.Size(392, 96);
            this._descriptionLabel.TabIndex = 2;
            this._descriptionLabel.Text = "Description";
            this._descriptionLabel.UseMnemonic = false;
            this._descriptionLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _timeLabel
            // 
            this._timeLabel.AutoSize = true;
            this._timeLabel.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._timeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._timeLabel.Location = new System.Drawing.Point(3, 39);
            this._timeLabel.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this._timeLabel.Name = "_timeLabel";
            this._timeLabel.Size = new System.Drawing.Size(94, 22);
            this._timeLabel.TabIndex = 4;
            this._timeLabel.Text = "00:00 AM-23:59 PM";
            this._timeLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _topPanel
            // 
            this._topPanel.AutoSize = true;
            this._detailsTableLayoutPanel.SetColumnSpan(this._topPanel, 2);
            this._topPanel.Controls.Add(this._closeButton);
            this._topPanel.Controls.Add(this._topTableLayoutPanel);
            this._topPanel.Location = new System.Drawing.Point(0, 3);
            this._topPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this._topPanel.Name = "_topPanel";
            this._topPanel.Size = new System.Drawing.Size(497, 27);
            this._topPanel.TabIndex = 5;
            this._topPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _closeButton
            // 
            this._closeButton.Location = new System.Drawing.Point(419, 1);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(75, 23);
            this._closeButton.TabIndex = 4;
            this._closeButton.Text = "Close";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
            // 
            // _topTableLayoutPanel
            // 
            this._topTableLayoutPanel.AutoSize = true;
            this._topTableLayoutPanel.ColumnCount = 3;
            this._topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._topTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._topTableLayoutPanel.Controls.Add(this._channelLabel, 0, 0);
            this._topTableLayoutPanel.Controls.Add(this._dateLabel, 1, 0);
            this._topTableLayoutPanel.Controls.Add(this._widescreenLabel, 2, 0);
            this._topTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._topTableLayoutPanel.Name = "_topTableLayoutPanel";
            this._topTableLayoutPanel.RowCount = 1;
            this._topTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._topTableLayoutPanel.Size = new System.Drawing.Size(210, 23);
            this._topTableLayoutPanel.TabIndex = 3;
            this._topTableLayoutPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _channelLabel
            // 
            this._channelLabel.AutoSize = true;
            this._channelLabel.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._channelLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this._channelLabel.Location = new System.Drawing.Point(3, 0);
            this._channelLabel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this._channelLabel.Name = "_channelLabel";
            this._channelLabel.Size = new System.Drawing.Size(104, 23);
            this._channelLabel.TabIndex = 0;
            this._channelLabel.Text = "CHANNEL";
            this._channelLabel.UseMnemonic = false;
            this._channelLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _dateLabel
            // 
            this._dateLabel.AutoSize = true;
            this._dateLabel.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._dateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._dateLabel.Location = new System.Drawing.Point(110, 10);
            this._dateLabel.Margin = new System.Windows.Forms.Padding(3, 10, 0, 0);
            this._dateLabel.Name = "_dateLabel";
            this._dateLabel.Size = new System.Drawing.Size(51, 11);
            this._dateLabel.TabIndex = 1;
            this._dateLabel.Text = "day, date";
            this._dateLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _widescreenLabel
            // 
            this._widescreenLabel.AutoSize = true;
            this._widescreenLabel.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._widescreenLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._widescreenLabel.Location = new System.Drawing.Point(164, 10);
            this._widescreenLabel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this._widescreenLabel.Name = "_widescreenLabel";
            this._widescreenLabel.Size = new System.Drawing.Size(29, 11);
            this._widescreenLabel.TabIndex = 2;
            this._widescreenLabel.Text = "[16:9]";
            this._widescreenLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // _detailsPanel
            // 
            this._detailsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._detailsPanel.Controls.Add(this._detailsTableLayoutPanel);
            this._detailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._detailsPanel.ForeColor = System.Drawing.Color.Black;
            this._detailsPanel.Location = new System.Drawing.Point(0, 0);
            this._detailsPanel.Name = "_detailsPanel";
            this._detailsPanel.Size = new System.Drawing.Size(500, 174);
            this._detailsPanel.TabIndex = 1;
            this._detailsPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            // 
            // ProgramDetailsPopup
            // 
            this.AcceptButton = this._closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._closeButton;
            this.ClientSize = new System.Drawing.Size(500, 174);
            this.ControlBox = false;
            this.Controls.Add(this._detailsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgramDetailsPopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.ProgramDetailsPopup_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WindowDragMouseDown);
            this._detailsTableLayoutPanel.ResumeLayout(false);
            this._detailsTableLayoutPanel.PerformLayout();
            this._topPanel.ResumeLayout(false);
            this._topPanel.PerformLayout();
            this._topTableLayoutPanel.ResumeLayout(false);
            this._topTableLayoutPanel.PerformLayout();
            this._detailsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _detailsTableLayoutPanel;
        private System.Windows.Forms.Label _titleLabel;
        private System.Windows.Forms.Label _subTitleLabel;
        private System.Windows.Forms.Panel _detailsPanel;
        private System.Windows.Forms.Label _descriptionLabel;
        private System.Windows.Forms.TableLayoutPanel _topTableLayoutPanel;
        private System.Windows.Forms.Label _channelLabel;
        private System.Windows.Forms.Label _dateLabel;
        private System.Windows.Forms.Label _widescreenLabel;
        private System.Windows.Forms.Label _timeLabel;
        private System.Windows.Forms.Panel _topPanel;
        private System.Windows.Forms.Button _closeButton;
    }
}