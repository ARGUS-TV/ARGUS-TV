namespace ArgusTV.UI.Console.Panels
{
    partial class GuidePanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GuidePanel));
            this._groupLabel = new System.Windows.Forms.Label();
            this._channelGroupsComboBox = new System.Windows.Forms.ComboBox();
            this._groupsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._epgControl = new ArgusTV.WinForms.Controls.EpgControl();
            this._loadingPanel = new System.Windows.Forms.Panel();
            this._busyPictureBox = new System.Windows.Forms.PictureBox();
            this._loadingLabel = new System.Windows.Forms.Label();
            this._programContextMenuStrip = new ArgusTV.WinForms.Controls.ProgramContextMenuStrip();
            this._guideDatePicker = new System.Windows.Forms.DateTimePicker();
            this._dateLabel = new System.Windows.Forms.Label();
            this._backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._nowButton = new System.Windows.Forms.Button();
            this._previousDayButton = new System.Windows.Forms.Button();
            this._nextDayButton = new System.Windows.Forms.Button();
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._groupsBindingSource)).BeginInit();
            this._loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._busyPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // _groupLabel
            // 
            this._groupLabel.AutoSize = true;
            this._groupLabel.Location = new System.Drawing.Point(0, 4);
            this._groupLabel.Name = "_groupLabel";
            this._groupLabel.Size = new System.Drawing.Size(39, 13);
            this._groupLabel.TabIndex = 0;
            this._groupLabel.Text = "Group:";
            // 
            // _channelGroupsComboBox
            // 
            this._channelGroupsComboBox.DataSource = this._groupsBindingSource;
            this._channelGroupsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelGroupsComboBox.FormattingEnabled = true;
            this._channelGroupsComboBox.Location = new System.Drawing.Point(122, 0);
            this._channelGroupsComboBox.Name = "_channelGroupsComboBox";
            this._channelGroupsComboBox.Size = new System.Drawing.Size(150, 21);
            this._channelGroupsComboBox.TabIndex = 2;
            this._channelGroupsComboBox.SelectedIndexChanged += new System.EventHandler(this._channelGroupsComboBox_SelectedIndexChanged);
            // 
            // _epgControl
            // 
            this._epgControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._epgControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this._epgControl.Location = new System.Drawing.Point(0, 31);
            this._epgControl.Name = "_epgControl";
            this._epgControl.ScrollPosition = new System.Drawing.Point(0, 0);
            this._epgControl.Size = new System.Drawing.Size(826, 493);
            this._epgControl.TabIndex = 10;
            this._epgControl.ProgramClicked += new System.EventHandler<ArgusTV.WinForms.Controls.EpgControl.ProgramEventArgs>(this._epgControl_ProgramClicked);
            this._epgControl.ProgramContextMenu += new System.EventHandler<ArgusTV.WinForms.Controls.EpgControl.ProgramEventArgs>(this._epgControl_ProgramContextMenu);
            // 
            // _loadingPanel
            // 
            this._loadingPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._loadingPanel.BackColor = System.Drawing.SystemColors.Window;
            this._loadingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._loadingPanel.Controls.Add(this._busyPictureBox);
            this._loadingPanel.Controls.Add(this._loadingLabel);
            this._loadingPanel.Location = new System.Drawing.Point(333, 221);
            this._loadingPanel.Name = "_loadingPanel";
            this._loadingPanel.Size = new System.Drawing.Size(160, 50);
            this._loadingPanel.TabIndex = 1000;
            this._loadingPanel.Visible = false;
            // 
            // _busyPictureBox
            // 
            this._busyPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._busyPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("_busyPictureBox.Image")));
            this._busyPictureBox.Location = new System.Drawing.Point(71, 25);
            this._busyPictureBox.Name = "_busyPictureBox";
            this._busyPictureBox.Size = new System.Drawing.Size(16, 16);
            this._busyPictureBox.TabIndex = 4;
            this._busyPictureBox.TabStop = false;
            // 
            // _loadingLabel
            // 
            this._loadingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._loadingLabel.AutoSize = true;
            this._loadingLabel.Location = new System.Drawing.Point(54, 7);
            this._loadingLabel.Name = "_loadingLabel";
            this._loadingLabel.Size = new System.Drawing.Size(54, 13);
            this._loadingLabel.TabIndex = 3;
            this._loadingLabel.Text = "Loading...";
            // 
            // _programContextMenuStrip
            // 
            this._programContextMenuStrip.Name = "_programContextMenuStrip";
            this._programContextMenuStrip.Size = new System.Drawing.Size(308, 302);
            this._programContextMenuStrip.CreateNewSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CreateNewScheduleEventArgs>(this._programContextMenuStrip_CreateNewSchedule);
            this._programContextMenuStrip.CancelProgram += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs>(this._programContextMenuStrip_CancelProgram);
            this._programContextMenuStrip.AddRemoveProgramHistory += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.AddRemoveProgramHistoryEventArgs>(this._programContextMenuStrip_AddRemoveProgramHistory);
            this._programContextMenuStrip.SetProgramPriority += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs>(this._programContextMenuStrip_SetProgramPriority);
            this._programContextMenuStrip.DeleteSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_DeleteSchedule);
            this._programContextMenuStrip.EditSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_EditSchedule);
            this._programContextMenuStrip.SetProgramPrePostRecord += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs>(this._programContextMenuStrip_SetProgramPrePostRecord);
            // 
            // _guideDatePicker
            // 
            this._guideDatePicker.Location = new System.Drawing.Point(315, 1);
            this._guideDatePicker.Name = "_guideDatePicker";
            this._guideDatePicker.Size = new System.Drawing.Size(200, 20);
            this._guideDatePicker.TabIndex = 4;
            this._guideDatePicker.ValueChanged += new System.EventHandler(this._manualDatePicker_ValueChanged);
            // 
            // _dateLabel
            // 
            this._dateLabel.AutoSize = true;
            this._dateLabel.Location = new System.Drawing.Point(282, 4);
            this._dateLabel.Name = "_dateLabel";
            this._dateLabel.Size = new System.Drawing.Size(33, 13);
            this._dateLabel.TabIndex = 3;
            this._dateLabel.Text = "Date:";
            // 
            // _backgroundWorker
            // 
            this._backgroundWorker.WorkerSupportsCancellation = true;
            this._backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._epgBackgroundWorker_DoWork);
            this._backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._epgBackgroundWorker_RunWorkerCompleted);
            // 
            // _nowButton
            // 
            this._nowButton.Location = new System.Drawing.Point(521, 0);
            this._nowButton.Name = "_nowButton";
            this._nowButton.Size = new System.Drawing.Size(45, 23);
            this._nowButton.TabIndex = 5;
            this._nowButton.Text = "Now";
            this._nowButton.UseVisualStyleBackColor = true;
            this._nowButton.Click += new System.EventHandler(this._nowButton_Click);
            // 
            // _previousDayButton
            // 
            this._previousDayButton.Location = new System.Drawing.Point(572, 0);
            this._previousDayButton.Name = "_previousDayButton";
            this._previousDayButton.Size = new System.Drawing.Size(95, 23);
            this._previousDayButton.TabIndex = 6;
            this._previousDayButton.Text = "< Previous Day";
            this._previousDayButton.UseVisualStyleBackColor = true;
            this._previousDayButton.Click += new System.EventHandler(this._previousDayButton_Click);
            // 
            // _nextDayButton
            // 
            this._nextDayButton.Location = new System.Drawing.Point(673, 0);
            this._nextDayButton.Name = "_nextDayButton";
            this._nextDayButton.Size = new System.Drawing.Size(95, 23);
            this._nextDayButton.TabIndex = 7;
            this._nextDayButton.Text = "Next Day >";
            this._nextDayButton.UseVisualStyleBackColor = true;
            this._nextDayButton.Click += new System.EventHandler(this._nextDayButton_Click);
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television",
            "Radio"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(41, 0);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(75, 21);
            this._channelTypeComboBox.TabIndex = 1;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // GuidePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._channelTypeComboBox);
            this.Controls.Add(this._nextDayButton);
            this.Controls.Add(this._previousDayButton);
            this.Controls.Add(this._nowButton);
            this.Controls.Add(this._loadingPanel);
            this.Controls.Add(this._dateLabel);
            this.Controls.Add(this._guideDatePicker);
            this.Controls.Add(this._groupLabel);
            this.Controls.Add(this._channelGroupsComboBox);
            this.Controls.Add(this._epgControl);
            this.Name = "GuidePanel";
            this.Size = new System.Drawing.Size(826, 524);
            this.Load += new System.EventHandler(this.GuidePanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._groupsBindingSource)).EndInit();
            this._loadingPanel.ResumeLayout(false);
            this._loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._busyPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _channelGroupsComboBox;
        private System.Windows.Forms.Label _groupLabel;
        private ArgusTV.WinForms.Controls.ProgramContextMenuStrip _programContextMenuStrip;
        private ArgusTV.WinForms.Controls.EpgControl _epgControl;
        private System.Windows.Forms.DateTimePicker _guideDatePicker;
        private System.Windows.Forms.Label _dateLabel;
        private System.ComponentModel.BackgroundWorker _backgroundWorker;
        private System.Windows.Forms.Panel _loadingPanel;
        private System.Windows.Forms.PictureBox _busyPictureBox;
        private System.Windows.Forms.Label _loadingLabel;
        private System.Windows.Forms.Button _nowButton;
        private System.Windows.Forms.Button _previousDayButton;
        private System.Windows.Forms.Button _nextDayButton;
        private System.Windows.Forms.ComboBox _channelTypeComboBox;
        private System.Windows.Forms.BindingSource _groupsBindingSource;

    }
}
