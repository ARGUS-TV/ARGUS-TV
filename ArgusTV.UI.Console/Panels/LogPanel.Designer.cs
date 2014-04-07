namespace ArgusTV.UI.Console.Panels
{
    partial class LogPanel
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
            this._logListControl = new ArgusTV.UI.Console.UserControls.LogListControl();
            this._searchCriteriaGroupBox = new System.Windows.Forms.GroupBox();
            this._severityComboBox = new System.Windows.Forms.ComboBox();
            this._moduleComboBox = new System.Windows.Forms.ComboBox();
            this._severityLabel = new System.Windows.Forms.Label();
            this._moduleLabel = new System.Windows.Forms.Label();
            this._endDateLabel = new System.Windows.Forms.Label();
            this._endDatePicker = new System.Windows.Forms.DateTimePicker();
            this._startDateLabel = new System.Windows.Forms.Label();
            this._startDatePicker = new System.Windows.Forms.DateTimePicker();
            this._searchButton = new System.Windows.Forms.Button();
            this._clearButton = new System.Windows.Forms.Button();
            this._maxEntriesReachedLabel = new System.Windows.Forms.Label();
            this._openLogsButton = new System.Windows.Forms.Button();
            this._searchCriteriaGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _logListControl
            // 
            this._logListControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._logListControl.Location = new System.Drawing.Point(0, 105);
            this._logListControl.LogEntries = null;
            this._logListControl.Name = "_logListControl";
            this._logListControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._logListControl.Size = new System.Drawing.Size(831, 411);
            this._logListControl.TabIndex = 5;
            // 
            // _searchCriteriaGroupBox
            // 
            this._searchCriteriaGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._searchCriteriaGroupBox.Controls.Add(this._severityComboBox);
            this._searchCriteriaGroupBox.Controls.Add(this._moduleComboBox);
            this._searchCriteriaGroupBox.Controls.Add(this._severityLabel);
            this._searchCriteriaGroupBox.Controls.Add(this._moduleLabel);
            this._searchCriteriaGroupBox.Controls.Add(this._endDateLabel);
            this._searchCriteriaGroupBox.Controls.Add(this._endDatePicker);
            this._searchCriteriaGroupBox.Controls.Add(this._startDateLabel);
            this._searchCriteriaGroupBox.Controls.Add(this._startDatePicker);
            this._searchCriteriaGroupBox.Location = new System.Drawing.Point(0, 0);
            this._searchCriteriaGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this._searchCriteriaGroupBox.Name = "_searchCriteriaGroupBox";
            this._searchCriteriaGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this._searchCriteriaGroupBox.Size = new System.Drawing.Size(831, 72);
            this._searchCriteriaGroupBox.TabIndex = 0;
            this._searchCriteriaGroupBox.TabStop = false;
            this._searchCriteriaGroupBox.Text = "Criteria";
            // 
            // _severityComboBox
            // 
            this._severityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._severityComboBox.FormattingEnabled = true;
            this._severityComboBox.Location = new System.Drawing.Point(330, 41);
            this._severityComboBox.Margin = new System.Windows.Forms.Padding(2);
            this._severityComboBox.Name = "_severityComboBox";
            this._severityComboBox.Size = new System.Drawing.Size(186, 21);
            this._severityComboBox.TabIndex = 7;
            // 
            // _moduleComboBox
            // 
            this._moduleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._moduleComboBox.FormattingEnabled = true;
            this._moduleComboBox.Location = new System.Drawing.Point(64, 41);
            this._moduleComboBox.Margin = new System.Windows.Forms.Padding(2);
            this._moduleComboBox.Name = "_moduleComboBox";
            this._moduleComboBox.Size = new System.Drawing.Size(186, 21);
            this._moduleComboBox.TabIndex = 5;
            // 
            // _severityLabel
            // 
            this._severityLabel.AutoSize = true;
            this._severityLabel.Location = new System.Drawing.Point(270, 44);
            this._severityLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._severityLabel.Name = "_severityLabel";
            this._severityLabel.Size = new System.Drawing.Size(48, 13);
            this._severityLabel.TabIndex = 6;
            this._severityLabel.Text = "Severity:";
            // 
            // _moduleLabel
            // 
            this._moduleLabel.AutoSize = true;
            this._moduleLabel.Location = new System.Drawing.Point(4, 44);
            this._moduleLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._moduleLabel.Name = "_moduleLabel";
            this._moduleLabel.Size = new System.Drawing.Size(45, 13);
            this._moduleLabel.TabIndex = 4;
            this._moduleLabel.Text = "Module:";
            // 
            // _endDateLabel
            // 
            this._endDateLabel.AutoSize = true;
            this._endDateLabel.Location = new System.Drawing.Point(270, 20);
            this._endDateLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._endDateLabel.Name = "_endDateLabel";
            this._endDateLabel.Size = new System.Drawing.Size(53, 13);
            this._endDateLabel.TabIndex = 2;
            this._endDateLabel.Text = "End date:";
            // 
            // _endDatePicker
            // 
            this._endDatePicker.Location = new System.Drawing.Point(330, 17);
            this._endDatePicker.Margin = new System.Windows.Forms.Padding(2);
            this._endDatePicker.Name = "_endDatePicker";
            this._endDatePicker.Size = new System.Drawing.Size(186, 20);
            this._endDatePicker.TabIndex = 3;
            // 
            // _startDateLabel
            // 
            this._startDateLabel.AutoSize = true;
            this._startDateLabel.Location = new System.Drawing.Point(4, 20);
            this._startDateLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._startDateLabel.Name = "_startDateLabel";
            this._startDateLabel.Size = new System.Drawing.Size(56, 13);
            this._startDateLabel.TabIndex = 0;
            this._startDateLabel.Text = "Start date:";
            // 
            // _startDatePicker
            // 
            this._startDatePicker.Location = new System.Drawing.Point(64, 17);
            this._startDatePicker.Margin = new System.Windows.Forms.Padding(2);
            this._startDatePicker.Name = "_startDatePicker";
            this._startDatePicker.Size = new System.Drawing.Size(186, 20);
            this._startDatePicker.TabIndex = 1;
            // 
            // _searchButton
            // 
            this._searchButton.Location = new System.Drawing.Point(0, 76);
            this._searchButton.Margin = new System.Windows.Forms.Padding(2);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(90, 23);
            this._searchButton.TabIndex = 1;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // _clearButton
            // 
            this._clearButton.Location = new System.Drawing.Point(94, 76);
            this._clearButton.Margin = new System.Windows.Forms.Padding(2);
            this._clearButton.Name = "_clearButton";
            this._clearButton.Size = new System.Drawing.Size(90, 23);
            this._clearButton.TabIndex = 2;
            this._clearButton.Text = "Clear Criteria";
            this._clearButton.UseVisualStyleBackColor = true;
            this._clearButton.Click += new System.EventHandler(this._clearButton_Click);
            // 
            // _maxEntriesReachedLabel
            // 
            this._maxEntriesReachedLabel.AutoSize = true;
            this._maxEntriesReachedLabel.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this._maxEntriesReachedLabel.Location = new System.Drawing.Point(193, 81);
            this._maxEntriesReachedLabel.Name = "_maxEntriesReachedLabel";
            this._maxEntriesReachedLabel.Size = new System.Drawing.Size(0, 13);
            this._maxEntriesReachedLabel.TabIndex = 3;
            // 
            // _openLogsButton
            // 
            this._openLogsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._openLogsButton.Location = new System.Drawing.Point(721, 76);
            this._openLogsButton.Name = "_openLogsButton";
            this._openLogsButton.Size = new System.Drawing.Size(110, 23);
            this._openLogsButton.TabIndex = 4;
            this._openLogsButton.Text = "Open Local Logs";
            this._openLogsButton.UseVisualStyleBackColor = true;
            this._openLogsButton.Click += new System.EventHandler(this._openLogsButton_Click);
            // 
            // LogPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._openLogsButton);
            this.Controls.Add(this._maxEntriesReachedLabel);
            this.Controls.Add(this._clearButton);
            this.Controls.Add(this._searchButton);
            this.Controls.Add(this._searchCriteriaGroupBox);
            this.Controls.Add(this._logListControl);
            this.Name = "LogPanel";
            this.Size = new System.Drawing.Size(831, 516);
            this.Load += new System.EventHandler(this.LogPanel_Load);
            this._searchCriteriaGroupBox.ResumeLayout(false);
            this._searchCriteriaGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ArgusTV.UI.Console.UserControls.LogListControl _logListControl;
        private System.Windows.Forms.GroupBox _searchCriteriaGroupBox;
        private System.Windows.Forms.Label _startDateLabel;
        private System.Windows.Forms.Label _endDateLabel;
        private System.Windows.Forms.DateTimePicker _startDatePicker;
        private System.Windows.Forms.ComboBox _severityComboBox;
        private System.Windows.Forms.ComboBox _moduleComboBox;
        private System.Windows.Forms.Label _severityLabel;
        private System.Windows.Forms.Label _moduleLabel;
        private System.Windows.Forms.DateTimePicker _endDatePicker;
        private System.Windows.Forms.Button _searchButton;
        private System.Windows.Forms.Button _clearButton;
        private System.Windows.Forms.Label _maxEntriesReachedLabel;
        private System.Windows.Forms.Button _openLogsButton;
    }
}
