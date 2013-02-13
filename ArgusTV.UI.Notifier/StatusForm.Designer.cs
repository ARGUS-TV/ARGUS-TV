namespace ArgusTV.UI.Notifier
{
    partial class StatusForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusForm));
            this._iconContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._openManagementConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._wakeupServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this._upcomingGroupBox = new System.Windows.Forms.GroupBox();
            this._upcomingProgramsControl = new ArgusTV.WinForms.UserControls.UpcomingOrActiveProgramsControl();
            this._recordingsGroupBox = new System.Windows.Forms.GroupBox();
            this._activeRecordingsControl = new ArgusTV.WinForms.UserControls.UpcomingOrActiveProgramsControl();
            this._closeButton = new System.Windows.Forms.Button();
            this._refreshButton = new System.Windows.Forms.Button();
            this._refreshTimer = new System.Windows.Forms.Timer(this.components);
            this._connectionBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._programContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._playRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._refreshStatusBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._refreshProgramsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._toolTipTimer = new System.Windows.Forms.Timer(this.components);
            this._iconContextMenuStrip.SuspendLayout();
            this._upcomingGroupBox.SuspendLayout();
            this._recordingsGroupBox.SuspendLayout();
            this._programContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _iconContextMenuStrip
            // 
            this._iconContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._openToolStripMenuItem,
            this._openManagementConsoleToolStripMenuItem,
            this._wakeupServerToolStripMenuItem,
            this.toolStripSeparator1,
            this._optionsToolStripMenuItem,
            this._exitToolStripMenuItem});
            this._iconContextMenuStrip.Name = "_iconContextMenuStrip";
            this._iconContextMenuStrip.Size = new System.Drawing.Size(205, 142);
            this._iconContextMenuStrip.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this._iconContextMenuStrip_Closed);
            this._iconContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this._iconContextMenuStrip_Opening);
            // 
            // _openToolStripMenuItem
            // 
            this._openToolStripMenuItem.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this._openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("_openToolStripMenuItem.Image")));
            this._openToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._openToolStripMenuItem.Name = "_openToolStripMenuItem";
            this._openToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this._openToolStripMenuItem.Text = "&Open";
            this._openToolStripMenuItem.Click += new System.EventHandler(this._openToolStripMenuItem_Click);
            // 
            // _openManagementConsoleToolStripMenuItem
            // 
            this._openManagementConsoleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("_openManagementConsoleToolStripMenuItem.Image")));
            this._openManagementConsoleToolStripMenuItem.Name = "_openManagementConsoleToolStripMenuItem";
            this._openManagementConsoleToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this._openManagementConsoleToolStripMenuItem.Text = "Open Scheduler Console";
            this._openManagementConsoleToolStripMenuItem.Click += new System.EventHandler(this._openManagementConsoleToolStripMenuItem_Click);
            // 
            // _wakeupServerToolStripMenuItem
            // 
            this._wakeupServerToolStripMenuItem.Name = "_wakeupServerToolStripMenuItem";
            this._wakeupServerToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this._wakeupServerToolStripMenuItem.Text = "&Wake Up Server";
            this._wakeupServerToolStripMenuItem.Click += new System.EventHandler(this._wakeupServerToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
            // 
            // _optionsToolStripMenuItem
            // 
            this._optionsToolStripMenuItem.Name = "_optionsToolStripMenuItem";
            this._optionsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this._optionsToolStripMenuItem.Text = "O&ptions...";
            this._optionsToolStripMenuItem.Click += new System.EventHandler(this._optionsToolStripMenuItem_Click);
            // 
            // _exitToolStripMenuItem
            // 
            this._exitToolStripMenuItem.Name = "_exitToolStripMenuItem";
            this._exitToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this._exitToolStripMenuItem.Text = "E&xit";
            this._exitToolStripMenuItem.Click += new System.EventHandler(this._exitToolStripMenuItem_Click);
            // 
            // _notifyIcon
            // 
            this._notifyIcon.ContextMenuStrip = this._iconContextMenuStrip;
            this._notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("_notifyIcon.Icon")));
            this._notifyIcon.Text = "Server Status (ARGUS TV)";
            this._notifyIcon.Visible = true;
            this._notifyIcon.BalloonTipClicked += new System.EventHandler(this._notifyIcon_BalloonTipClicked);
            this._notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._notifyIcon_MouseDoubleClick);
            // 
            // _upcomingGroupBox
            // 
            this._upcomingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._upcomingGroupBox.Controls.Add(this._upcomingProgramsControl);
            this._upcomingGroupBox.Location = new System.Drawing.Point(12, 150);
            this._upcomingGroupBox.Name = "_upcomingGroupBox";
            this._upcomingGroupBox.Size = new System.Drawing.Size(687, 255);
            this._upcomingGroupBox.TabIndex = 4;
            this._upcomingGroupBox.TabStop = false;
            this._upcomingGroupBox.Text = "Upcoming Recordings";
            // 
            // _upcomingProgramsControl
            // 
            this._upcomingProgramsControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._upcomingProgramsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._upcomingProgramsControl.Location = new System.Drawing.Point(3, 16);
            this._upcomingProgramsControl.Name = "_upcomingProgramsControl";
            this._upcomingProgramsControl.Padding = new System.Windows.Forms.Padding(4, 0, 4, 4);
            this._upcomingProgramsControl.ScheduleType = ArgusTV.DataContracts.ScheduleType.Recording;
            this._upcomingProgramsControl.ShowScheduleName = false;
            this._upcomingProgramsControl.Size = new System.Drawing.Size(681, 236);
            this._upcomingProgramsControl.Sortable = true;
            this._upcomingProgramsControl.TabIndex = 0;
            this._upcomingProgramsControl.UnfilteredUpcomingRecordings = null;
            this._upcomingProgramsControl.UpcomingPrograms = null;
            // 
            // _recordingsGroupBox
            // 
            this._recordingsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recordingsGroupBox.Controls.Add(this._activeRecordingsControl);
            this._recordingsGroupBox.Location = new System.Drawing.Point(12, 12);
            this._recordingsGroupBox.Name = "_recordingsGroupBox";
            this._recordingsGroupBox.Size = new System.Drawing.Size(690, 132);
            this._recordingsGroupBox.TabIndex = 3;
            this._recordingsGroupBox.TabStop = false;
            this._recordingsGroupBox.Text = "Active Recordings";
            // 
            // _activeRecordingsControl
            // 
            this._activeRecordingsControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this._activeRecordingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._activeRecordingsControl.Location = new System.Drawing.Point(3, 16);
            this._activeRecordingsControl.Name = "_activeRecordingsControl";
            this._activeRecordingsControl.Padding = new System.Windows.Forms.Padding(4, 0, 4, 4);
            this._activeRecordingsControl.ScheduleType = ArgusTV.DataContracts.ScheduleType.Recording;
            this._activeRecordingsControl.ShowScheduleName = false;
            this._activeRecordingsControl.Size = new System.Drawing.Size(684, 113);
            this._activeRecordingsControl.Sortable = true;
            this._activeRecordingsControl.TabIndex = 0;
            this._activeRecordingsControl.UnfilteredUpcomingRecordings = null;
            this._activeRecordingsControl.UpcomingPrograms = null;
            this._activeRecordingsControl.GridMouseUp += new System.Windows.Forms.MouseEventHandler(this._activeRecordingsControl_GridMouseUp);
            this._activeRecordingsControl.GridDoubleClick += new System.EventHandler(this._activeRecordingsControl_GridDoubleClick);
            // 
            // _closeButton
            // 
            this._closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._closeButton.Location = new System.Drawing.Point(624, 411);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(75, 23);
            this._closeButton.TabIndex = 201;
            this._closeButton.Text = "Close";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
            // 
            // _refreshButton
            // 
            this._refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._refreshButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._refreshButton.Location = new System.Drawing.Point(543, 411);
            this._refreshButton.Name = "_refreshButton";
            this._refreshButton.Size = new System.Drawing.Size(75, 23);
            this._refreshButton.TabIndex = 200;
            this._refreshButton.Text = "Refresh";
            this._refreshButton.UseVisualStyleBackColor = true;
            this._refreshButton.Click += new System.EventHandler(this._refreshButton_Click);
            // 
            // _refreshTimer
            // 
            this._refreshTimer.Tick += new System.EventHandler(this._refreshTimer_Tick);
            // 
            // _connectionBackgroundWorker
            // 
            this._connectionBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._connectionBackgroundWorker_DoWork);
            this._connectionBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._connectionBackgroundWorker_RunWorkerCompleted);
            // 
            // _programContextMenuStrip
            // 
            this._programContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._playRecordingToolStripMenuItem});
            this._programContextMenuStrip.Name = "_programContextMenuStrip";
            this._programContextMenuStrip.Size = new System.Drawing.Size(206, 26);
            this._programContextMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._programContextMenuStrip_ItemClicked);
            // 
            // _playRecordingToolStripMenuItem
            // 
            this._playRecordingToolStripMenuItem.Name = "_playRecordingToolStripMenuItem";
            this._playRecordingToolStripMenuItem.Size = new System.Drawing.Size(205, 22);
            this._playRecordingToolStripMenuItem.Text = "Play Recording With VLC";
            // 
            // _refreshStatusBackgroundWorker
            // 
            this._refreshStatusBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._refreshStatusBackgroundWorker_DoWork);
            this._refreshStatusBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._refreshStatusBackgroundWorker_RunWorkerCompleted);
            // 
            // _refreshProgramsBackgroundWorker
            // 
            this._refreshProgramsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._refreshProgramsBackgroundWorker_DoWork);
            this._refreshProgramsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._refreshProgramsBackgroundWorker_RunWorkerCompleted);
            // 
            // _toolTipTimer
            // 
            this._toolTipTimer.Interval = 250;
            this._toolTipTimer.Tick += new System.EventHandler(this._toolTipTimer_Tick);
            // 
            // StatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._closeButton;
            this.ClientSize = new System.Drawing.Size(714, 444);
            this.Controls.Add(this._upcomingGroupBox);
            this.Controls.Add(this._refreshButton);
            this.Controls.Add(this._recordingsGroupBox);
            this.Controls.Add(this._closeButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(670, 480);
            this.Name = "StatusForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ARGUS TV Notifier";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this._iconContextMenuStrip.ResumeLayout(false);
            this._upcomingGroupBox.ResumeLayout(false);
            this._recordingsGroupBox.ResumeLayout(false);
            this._programContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip _iconContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _openToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private System.Windows.Forms.GroupBox _recordingsGroupBox;
        private System.Windows.Forms.Button _closeButton;
        private System.Windows.Forms.ToolStripMenuItem _exitToolStripMenuItem;
        private System.Windows.Forms.GroupBox _upcomingGroupBox;
        private ArgusTV.WinForms.UserControls.UpcomingOrActiveProgramsControl _upcomingProgramsControl;
        private ArgusTV.WinForms.UserControls.UpcomingOrActiveProgramsControl _activeRecordingsControl;
        private System.Windows.Forms.Button _refreshButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _optionsToolStripMenuItem;
        private System.Windows.Forms.Timer _refreshTimer;
        private System.ComponentModel.BackgroundWorker _connectionBackgroundWorker;
        private System.Windows.Forms.ToolStripMenuItem _openManagementConsoleToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip _programContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _playRecordingToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker _refreshStatusBackgroundWorker;
        private System.ComponentModel.BackgroundWorker _refreshProgramsBackgroundWorker;
        private System.Windows.Forms.Timer _toolTipTimer;
        private System.Windows.Forms.ToolStripMenuItem _wakeupServerToolStripMenuItem;
    }
}