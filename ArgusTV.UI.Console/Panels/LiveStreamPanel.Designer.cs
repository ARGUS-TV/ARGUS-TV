namespace ArgusTV.UI.Console.Panels
{
    partial class LiveTvPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._streamsDataGridView = new System.Windows.Forms.DataGridView();
            this._streamsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._startLiveTvButton = new System.Windows.Forms.Button();
            this._channelsLabel = new System.Windows.Forms.Label();
            this._refreshStreamsButton = new System.Windows.Forms.Button();
            this._streamsLabel = new System.Windows.Forms.Label();
            this._stopLiveTvButton = new System.Windows.Forms.Button();
            this._openStreamButton = new System.Windows.Forms.Button();
            this._tuneChannelButton = new System.Windows.Forms.Button();
            this._channelsGridView = new System.Windows.Forms.DataGridView();
            this.tvChannelDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._currentIconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.currentProgramTitleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this._nextIconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.nextProgramTitleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this._channelsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._channelGroupsComboBox = new System.Windows.Forms.ComboBox();
            this._refreshChannelsButton = new System.Windows.Forms.Button();
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
            this._programContextMenuStrip = new ArgusTV.WinForms.Controls.ProgramContextMenuStrip();
            this.ChannelName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._rtspUrlDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecorderTuner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StreamStartedTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._streamsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._streamsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _streamsDataGridView
            // 
            this._streamsDataGridView.AllowUserToAddRows = false;
            this._streamsDataGridView.AllowUserToDeleteRows = false;
            this._streamsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._streamsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._streamsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._streamsDataGridView.AutoGenerateColumns = false;
            this._streamsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._streamsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._streamsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._streamsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChannelName,
            this._rtspUrlDataGridViewTextBoxColumn,
            this.RecorderTuner,
            this.StreamStartedTime});
            this._streamsDataGridView.DataSource = this._streamsBindingSource;
            this._streamsDataGridView.GridColor = System.Drawing.Color.White;
            this._streamsDataGridView.Location = new System.Drawing.Point(0, 463);
            this._streamsDataGridView.Margin = new System.Windows.Forms.Padding(0);
            this._streamsDataGridView.Name = "_streamsDataGridView";
            this._streamsDataGridView.ReadOnly = true;
            this._streamsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this._streamsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this._streamsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._streamsDataGridView.Size = new System.Drawing.Size(1070, 174);
            this._streamsDataGridView.StandardTab = true;
            this._streamsDataGridView.TabIndex = 21;
            this._streamsDataGridView.SelectionChanged += new System.EventHandler(this._streamsDataGridView_SelectionChanged);
            this._streamsDataGridView.DoubleClick += new System.EventHandler(this._streamsDataGridView_DoubleClick);
            // 
            // _streamsBindingSource
            // 
            this._streamsBindingSource.DataSource = typeof(ArgusTV.UI.Process.LiveStreamsList);
            // 
            // _startLiveTvButton
            // 
            this._startLiveTvButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._startLiveTvButton.Enabled = false;
            this._startLiveTvButton.Location = new System.Drawing.Point(0, 392);
            this._startLiveTvButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._startLiveTvButton.Name = "_startLiveTvButton";
            this._startLiveTvButton.Size = new System.Drawing.Size(150, 35);
            this._startLiveTvButton.TabIndex = 4;
            this._startLiveTvButton.Text = "Start Live Stream";
            this._startLiveTvButton.UseVisualStyleBackColor = true;
            this._startLiveTvButton.Click += new System.EventHandler(this._startLiveTvButton_Click);
            // 
            // _channelsLabel
            // 
            this._channelsLabel.AutoSize = true;
            this._channelsLabel.Location = new System.Drawing.Point(0, 6);
            this._channelsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._channelsLabel.Name = "_channelsLabel";
            this._channelsLabel.Size = new System.Drawing.Size(58, 20);
            this._channelsLabel.TabIndex = 0;
            this._channelsLabel.Text = "Group:";
            // 
            // _refreshStreamsButton
            // 
            this._refreshStreamsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._refreshStreamsButton.Location = new System.Drawing.Point(477, 642);
            this._refreshStreamsButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._refreshStreamsButton.Name = "_refreshStreamsButton";
            this._refreshStreamsButton.Size = new System.Drawing.Size(150, 35);
            this._refreshStreamsButton.TabIndex = 33;
            this._refreshStreamsButton.Text = "Refresh";
            this._refreshStreamsButton.UseVisualStyleBackColor = true;
            this._refreshStreamsButton.Click += new System.EventHandler(this._refreshStreamsButton_Click);
            // 
            // _streamsLabel
            // 
            this._streamsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._streamsLabel.AutoSize = true;
            this._streamsLabel.Location = new System.Drawing.Point(0, 438);
            this._streamsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._streamsLabel.Name = "_streamsLabel";
            this._streamsLabel.Size = new System.Drawing.Size(152, 20);
            this._streamsLabel.TabIndex = 20;
            this._streamsLabel.Text = "Active Live Streams:";
            // 
            // _stopLiveTvButton
            // 
            this._stopLiveTvButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._stopLiveTvButton.Enabled = false;
            this._stopLiveTvButton.Location = new System.Drawing.Point(159, 642);
            this._stopLiveTvButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._stopLiveTvButton.Name = "_stopLiveTvButton";
            this._stopLiveTvButton.Size = new System.Drawing.Size(150, 35);
            this._stopLiveTvButton.TabIndex = 31;
            this._stopLiveTvButton.Text = "Stop Live Stream";
            this._stopLiveTvButton.UseVisualStyleBackColor = true;
            this._stopLiveTvButton.Click += new System.EventHandler(this._stopLiveTvButton_Click);
            // 
            // _openStreamButton
            // 
            this._openStreamButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._openStreamButton.Enabled = false;
            this._openStreamButton.Location = new System.Drawing.Point(318, 642);
            this._openStreamButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._openStreamButton.Name = "_openStreamButton";
            this._openStreamButton.Size = new System.Drawing.Size(150, 35);
            this._openStreamButton.TabIndex = 32;
            this._openStreamButton.Text = "Open Stream";
            this._openStreamButton.UseVisualStyleBackColor = true;
            this._openStreamButton.Click += new System.EventHandler(this._openStreamButton_Click);
            // 
            // _tuneChannelButton
            // 
            this._tuneChannelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._tuneChannelButton.Location = new System.Drawing.Point(0, 642);
            this._tuneChannelButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._tuneChannelButton.Name = "_tuneChannelButton";
            this._tuneChannelButton.Size = new System.Drawing.Size(150, 35);
            this._tuneChannelButton.TabIndex = 30;
            this._tuneChannelButton.Text = "Tune Channel";
            this._tuneChannelButton.UseVisualStyleBackColor = true;
            this._tuneChannelButton.Click += new System.EventHandler(this._tuneChannelButton_Click);
            // 
            // _channelsGridView
            // 
            this._channelsGridView.AllowUserToAddRows = false;
            this._channelsGridView.AllowUserToDeleteRows = false;
            this._channelsGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this._channelsGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this._channelsGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._channelsGridView.AutoGenerateColumns = false;
            this._channelsGridView.BackgroundColor = System.Drawing.Color.White;
            this._channelsGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._channelsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._channelsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.tvChannelDataGridViewTextBoxColumn,
            this._currentIconColumn,
            this.currentProgramTitleDataGridViewTextBoxColumn,
            this._nextIconColumn,
            this.nextProgramTitleDataGridViewTextBoxColumn});
            this._channelsGridView.DataSource = this._channelsBindingSource;
            this._channelsGridView.GridColor = System.Drawing.Color.White;
            this._channelsGridView.Location = new System.Drawing.Point(0, 40);
            this._channelsGridView.Margin = new System.Windows.Forms.Padding(0);
            this._channelsGridView.MultiSelect = false;
            this._channelsGridView.Name = "_channelsGridView";
            this._channelsGridView.ReadOnly = true;
            this._channelsGridView.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this._channelsGridView.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this._channelsGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._channelsGridView.Size = new System.Drawing.Size(1070, 348);
            this._channelsGridView.StandardTab = true;
            this._channelsGridView.TabIndex = 3;
            this._channelsGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._channelsGridView_CellContentClick);
            this._channelsGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._channelsGridView_CellDoubleClick);
            this._channelsGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this._channelsGridView_CellFormatting);
            this._channelsGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this._channelsGridView_DataBindingComplete);
            this._channelsGridView.SelectionChanged += new System.EventHandler(this._channelsGridView_SelectionChanged);
            this._channelsGridView.MouseUp += new System.Windows.Forms.MouseEventHandler(this._channelsGridView_MouseUp);
            // 
            // tvChannelDataGridViewTextBoxColumn
            // 
            this.tvChannelDataGridViewTextBoxColumn.DataPropertyName = "ChannelName";
            this.tvChannelDataGridViewTextBoxColumn.HeaderText = "Channel";
            this.tvChannelDataGridViewTextBoxColumn.Name = "tvChannelDataGridViewTextBoxColumn";
            this.tvChannelDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _currentIconColumn
            // 
            this._currentIconColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._currentIconColumn.HeaderText = "";
            this._currentIconColumn.Name = "_currentIconColumn";
            this._currentIconColumn.ReadOnly = true;
            this._currentIconColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._currentIconColumn.Width = 26;
            // 
            // currentProgramTitleDataGridViewTextBoxColumn
            // 
            this.currentProgramTitleDataGridViewTextBoxColumn.ActiveLinkColor = System.Drawing.Color.DarkRed;
            this.currentProgramTitleDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.currentProgramTitleDataGridViewTextBoxColumn.DataPropertyName = "CurrentProgramTitle";
            this.currentProgramTitleDataGridViewTextBoxColumn.FillWeight = 50F;
            this.currentProgramTitleDataGridViewTextBoxColumn.HeaderText = "Current";
            this.currentProgramTitleDataGridViewTextBoxColumn.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.currentProgramTitleDataGridViewTextBoxColumn.LinkColor = System.Drawing.Color.Black;
            this.currentProgramTitleDataGridViewTextBoxColumn.Name = "currentProgramTitleDataGridViewTextBoxColumn";
            this.currentProgramTitleDataGridViewTextBoxColumn.ReadOnly = true;
            this.currentProgramTitleDataGridViewTextBoxColumn.TrackVisitedState = false;
            // 
            // _nextIconColumn
            // 
            this._nextIconColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._nextIconColumn.HeaderText = "";
            this._nextIconColumn.Name = "_nextIconColumn";
            this._nextIconColumn.ReadOnly = true;
            this._nextIconColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._nextIconColumn.Width = 26;
            // 
            // nextProgramTitleDataGridViewTextBoxColumn
            // 
            this.nextProgramTitleDataGridViewTextBoxColumn.ActiveLinkColor = System.Drawing.Color.DarkRed;
            this.nextProgramTitleDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nextProgramTitleDataGridViewTextBoxColumn.DataPropertyName = "NextProgramTitle";
            this.nextProgramTitleDataGridViewTextBoxColumn.FillWeight = 50F;
            this.nextProgramTitleDataGridViewTextBoxColumn.HeaderText = "Next";
            this.nextProgramTitleDataGridViewTextBoxColumn.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.nextProgramTitleDataGridViewTextBoxColumn.LinkColor = System.Drawing.Color.Black;
            this.nextProgramTitleDataGridViewTextBoxColumn.Name = "nextProgramTitleDataGridViewTextBoxColumn";
            this.nextProgramTitleDataGridViewTextBoxColumn.ReadOnly = true;
            this.nextProgramTitleDataGridViewTextBoxColumn.TrackVisitedState = false;
            // 
            // _channelsBindingSource
            // 
            this._channelsBindingSource.DataSource = typeof(ArgusTV.UI.Process.CurrentAndNextProgramsList);
            // 
            // _channelGroupsComboBox
            // 
            this._channelGroupsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._channelGroupsComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelGroupsComboBox.FormattingEnabled = true;
            this._channelGroupsComboBox.Location = new System.Drawing.Point(189, 0);
            this._channelGroupsComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._channelGroupsComboBox.Name = "_channelGroupsComboBox";
            this._channelGroupsComboBox.Size = new System.Drawing.Size(223, 28);
            this._channelGroupsComboBox.TabIndex = 2;
            this._channelGroupsComboBox.SelectedIndexChanged += new System.EventHandler(this._channelGroupsComboBox_SelectedIndexChanged);
            // 
            // _refreshChannelsButton
            // 
            this._refreshChannelsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._refreshChannelsButton.Location = new System.Drawing.Point(159, 392);
            this._refreshChannelsButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._refreshChannelsButton.Name = "_refreshChannelsButton";
            this._refreshChannelsButton.Size = new System.Drawing.Size(150, 35);
            this._refreshChannelsButton.TabIndex = 5;
            this._refreshChannelsButton.Text = "Refresh";
            this._refreshChannelsButton.UseVisualStyleBackColor = true;
            this._refreshChannelsButton.Click += new System.EventHandler(this._refreshChannelsButton_Click);
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television",
            "Radio"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(68, 0);
            this._channelTypeComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(110, 28);
            this._channelTypeComboBox.TabIndex = 1;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // _programContextMenuStrip
            // 
            this._programContextMenuStrip.Name = "programContextMenuStrip1";
            this._programContextMenuStrip.Size = new System.Drawing.Size(433, 466);
            this._programContextMenuStrip.CreateNewSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CreateNewScheduleEventArgs>(this._programContextMenuStrip_CreateNewSchedule);
            this._programContextMenuStrip.EditSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_EditSchedule);
            this._programContextMenuStrip.DeleteSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_DeleteSchedule);
            this._programContextMenuStrip.CancelProgram += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs>(this._programContextMenuStrip_CancelProgram);
            this._programContextMenuStrip.AddRemoveProgramHistory += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.AddRemoveProgramHistoryEventArgs>(this._programContextMenuStrip_AddRemoveProgramHistory);
            this._programContextMenuStrip.SetProgramPriority += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs>(this._programContextMenuStrip_SetProgramPriority);
            this._programContextMenuStrip.SetProgramPrePostRecord += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs>(this._programContextMenuStrip_SetProgramPrePostRecord);
            // 
            // ChannelName
            // 
            this.ChannelName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ChannelName.DataPropertyName = "ChannelName";
            this.ChannelName.HeaderText = "Channel";
            this.ChannelName.Name = "ChannelName";
            this.ChannelName.ReadOnly = true;
            // 
            // _rtspUrlDataGridViewTextBoxColumn
            // 
            this._rtspUrlDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._rtspUrlDataGridViewTextBoxColumn.DataPropertyName = "RtspUrl";
            this._rtspUrlDataGridViewTextBoxColumn.HeaderText = "URL";
            this._rtspUrlDataGridViewTextBoxColumn.Name = "_rtspUrlDataGridViewTextBoxColumn";
            this._rtspUrlDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // RecorderTuner
            // 
            this.RecorderTuner.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.RecorderTuner.DataPropertyName = "RecorderTuner";
            this.RecorderTuner.HeaderText = "Recorder";
            this.RecorderTuner.Name = "RecorderTuner";
            this.RecorderTuner.ReadOnly = true;
            // 
            // StreamStartedTime
            // 
            this.StreamStartedTime.DataPropertyName = "StreamStartedTime";
            dataGridViewCellStyle2.Format = "g";
            this.StreamStartedTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.StreamStartedTime.HeaderText = "Started";
            this.StreamStartedTime.Name = "StreamStartedTime";
            this.StreamStartedTime.ReadOnly = true;
            this.StreamStartedTime.Width = 185;
            // 
            // LiveTvPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._channelTypeComboBox);
            this.Controls.Add(this._refreshChannelsButton);
            this.Controls.Add(this._channelGroupsComboBox);
            this.Controls.Add(this._channelsGridView);
            this.Controls.Add(this._tuneChannelButton);
            this.Controls.Add(this._openStreamButton);
            this.Controls.Add(this._stopLiveTvButton);
            this.Controls.Add(this._streamsLabel);
            this.Controls.Add(this._refreshStreamsButton);
            this.Controls.Add(this._channelsLabel);
            this.Controls.Add(this._streamsDataGridView);
            this.Controls.Add(this._startLiveTvButton);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(894, 0);
            this.Name = "LiveTvPanel";
            this.Size = new System.Drawing.Size(1070, 677);
            this.Load += new System.EventHandler(this.ChannelsPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._streamsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._streamsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _streamsDataGridView;
        private System.Windows.Forms.BindingSource _streamsBindingSource;
        private System.Windows.Forms.Button _startLiveTvButton;
        private System.Windows.Forms.Label _channelsLabel;
        private System.Windows.Forms.Button _refreshStreamsButton;
        private System.Windows.Forms.Label _streamsLabel;
        private System.Windows.Forms.Button _stopLiveTvButton;
        private System.Windows.Forms.Button _openStreamButton;
        private System.Windows.Forms.Button _tuneChannelButton;
        private System.Windows.Forms.DataGridView _channelsGridView;
        private System.Windows.Forms.ComboBox _channelGroupsComboBox;
        private System.Windows.Forms.BindingSource _channelsBindingSource;
        private System.Windows.Forms.Button _refreshChannelsButton;
        private System.Windows.Forms.ComboBox _channelTypeComboBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn tvChannelDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn _currentIconColumn;
        private System.Windows.Forms.DataGridViewLinkColumn currentProgramTitleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewImageColumn _nextIconColumn;
        private System.Windows.Forms.DataGridViewLinkColumn nextProgramTitleDataGridViewTextBoxColumn;
        private ArgusTV.WinForms.Controls.ProgramContextMenuStrip _programContextMenuStrip;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChannelName;
        private System.Windows.Forms.DataGridViewTextBoxColumn _rtspUrlDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RecorderTuner;
        private System.Windows.Forms.DataGridViewTextBoxColumn StreamStartedTime;
    }
}
