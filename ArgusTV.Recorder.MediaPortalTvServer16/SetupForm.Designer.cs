namespace ArgusTV.Recorder.MediaPortalTvServer
{

    partial class SetupForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this._tvSchedulerGroupBox = new System.Windows.Forms.GroupBox();
            this._connectButton = new System.Windows.Forms.Button();
            this._portNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._tcpPortLabel = new System.Windows.Forms.Label();
            this._restartTcpLabel = new System.Windows.Forms.Label();
            this._serverTextBox = new System.Windows.Forms.TextBox();
            this._pluginTcpNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this._serverLabel = new System.Windows.Forms.Label();
            this._pluginTcpLabel = new System.Windows.Forms.Label();
            this._channelsPanel = new System.Windows.Forms.Panel();
            this._importChannelsButton = new System.Windows.Forms.Button();
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
            this._linkChannelButton = new System.Windows.Forms.Button();
            this._refreshChannelsButton = new System.Windows.Forms.Button();
            this._channelsDataGridView = new System.Windows.Forms.DataGridView();
            this._tvSchedulerChannelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._mediaPortalChannelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._channelItemsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._notConnectedPanel = new System.Windows.Forms.Panel();
            this._notConnectedLabel = new System.Windows.Forms.Label();
            this._uncRecordingGroupBox = new System.Windows.Forms.GroupBox();
            this._createUncShareButton = new System.Windows.Forms.Button();
            this._uncPathsDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._uncPathsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._refreshUncButton = new System.Windows.Forms.Button();
            this._syncTve3EpgcheckBox = new System.Windows.Forms.CheckBox();
            this._epgGroupBox = new System.Windows.Forms.GroupBox();
            this._epgAutoCreateChannelsWithGroupRadioButton = new System.Windows.Forms.RadioButton();
            this._epgAutoCreateChannelsDvbRadioButton = new System.Windows.Forms.RadioButton();
            this._epgOnlyLinkedChannelsRadioButton = new System.Windows.Forms.RadioButton();
            this._tabControl = new System.Windows.Forms.TabControl();
            this._configTabPage = new System.Windows.Forms.TabPage();
            this._powerGroupBox = new System.Windows.Forms.GroupBox();
            this._restartOnResumeCheckBox = new System.Windows.Forms.CheckBox();
            this._uncPathsTabPage = new System.Windows.Forms.TabPage();
            this._uncTimeshiftGroupBox = new System.Windows.Forms.GroupBox();
            this._createTimeshiftingShareButton = new System.Windows.Forms.Button();
            this._uncTimeshiftingDataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._uncTimeshiftingBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._channelsTabPage = new System.Windows.Forms.TabPage();
            this._tvSchedulerGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._portNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._pluginTcpNumericUpDown)).BeginInit();
            this._channelsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._channelsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelItemsBindingSource)).BeginInit();
            this._notConnectedPanel.SuspendLayout();
            this._uncRecordingGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._uncPathsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._uncPathsBindingSource)).BeginInit();
            this._epgGroupBox.SuspendLayout();
            this._tabControl.SuspendLayout();
            this._configTabPage.SuspendLayout();
            this._powerGroupBox.SuspendLayout();
            this._uncPathsTabPage.SuspendLayout();
            this._uncTimeshiftGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._uncTimeshiftingDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._uncTimeshiftingBindingSource)).BeginInit();
            this._channelsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tvSchedulerGroupBox
            // 
            this._tvSchedulerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tvSchedulerGroupBox.Controls.Add(this._connectButton);
            this._tvSchedulerGroupBox.Controls.Add(this._portNumericUpDown);
            this._tvSchedulerGroupBox.Controls.Add(this._tcpPortLabel);
            this._tvSchedulerGroupBox.Controls.Add(this._restartTcpLabel);
            this._tvSchedulerGroupBox.Controls.Add(this._serverTextBox);
            this._tvSchedulerGroupBox.Controls.Add(this._pluginTcpNumericUpDown);
            this._tvSchedulerGroupBox.Controls.Add(this._serverLabel);
            this._tvSchedulerGroupBox.Controls.Add(this._pluginTcpLabel);
            this._tvSchedulerGroupBox.Location = new System.Drawing.Point(6, 6);
            this._tvSchedulerGroupBox.Name = "_tvSchedulerGroupBox";
            this._tvSchedulerGroupBox.Size = new System.Drawing.Size(580, 76);
            this._tvSchedulerGroupBox.TabIndex = 0;
            this._tvSchedulerGroupBox.TabStop = false;
            this._tvSchedulerGroupBox.Text = "ARGUS TV Scheduler service";
            // 
            // _connectButton
            // 
            this._connectButton.Location = new System.Drawing.Point(333, 16);
            this._connectButton.Name = "_connectButton";
            this._connectButton.Size = new System.Drawing.Size(110, 23);
            this._connectButton.TabIndex = 14;
            this._connectButton.Text = "(Re)Connect Now";
            this._connectButton.UseVisualStyleBackColor = true;
            this._connectButton.Click += new System.EventHandler(this._connectButton_Click);
            // 
            // _portNumericUpDown
            // 
            this._portNumericUpDown.Location = new System.Drawing.Point(267, 18);
            this._portNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._portNumericUpDown.Name = "_portNumericUpDown";
            this._portNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this._portNumericUpDown.TabIndex = 13;
            // 
            // _tcpPortLabel
            // 
            this._tcpPortLabel.AutoSize = true;
            this._tcpPortLabel.Location = new System.Drawing.Point(209, 21);
            this._tcpPortLabel.Name = "_tcpPortLabel";
            this._tcpPortLabel.Size = new System.Drawing.Size(52, 13);
            this._tcpPortLabel.TabIndex = 12;
            this._tcpPortLabel.Text = "TCP port:";
            // 
            // _restartTcpLabel
            // 
            this._restartTcpLabel.AutoSize = true;
            this._restartTcpLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._restartTcpLabel.ForeColor = System.Drawing.Color.DimGray;
            this._restartTcpLabel.Location = new System.Drawing.Point(210, 48);
            this._restartTcpLabel.Name = "_restartTcpLabel";
            this._restartTcpLabel.Size = new System.Drawing.Size(240, 13);
            this._restartTcpLabel.TabIndex = 17;
            this._restartTcpLabel.Text = "(restart MediaPortal TVService if you change this)";
            // 
            // _serverTextBox
            // 
            this._serverTextBox.Location = new System.Drawing.Point(53, 18);
            this._serverTextBox.Name = "_serverTextBox";
            this._serverTextBox.Size = new System.Drawing.Size(150, 20);
            this._serverTextBox.TabIndex = 11;
            // 
            // _pluginTcpNumericUpDown
            // 
            this._pluginTcpNumericUpDown.Location = new System.Drawing.Point(147, 45);
            this._pluginTcpNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this._pluginTcpNumericUpDown.Name = "_pluginTcpNumericUpDown";
            this._pluginTcpNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this._pluginTcpNumericUpDown.TabIndex = 16;
            // 
            // _serverLabel
            // 
            this._serverLabel.AutoSize = true;
            this._serverLabel.Location = new System.Drawing.Point(8, 21);
            this._serverLabel.Name = "_serverLabel";
            this._serverLabel.Size = new System.Drawing.Size(41, 13);
            this._serverLabel.TabIndex = 10;
            this._serverLabel.Text = "Server:";
            // 
            // _pluginTcpLabel
            // 
            this._pluginTcpLabel.AutoSize = true;
            this._pluginTcpLabel.Location = new System.Drawing.Point(8, 48);
            this._pluginTcpLabel.Name = "_pluginTcpLabel";
            this._pluginTcpLabel.Size = new System.Drawing.Size(130, 13);
            this._pluginTcpLabel.TabIndex = 15;
            this._pluginTcpLabel.Text = "Recorder plugin TCP port:";
            // 
            // _channelsPanel
            // 
            this._channelsPanel.Controls.Add(this._importChannelsButton);
            this._channelsPanel.Controls.Add(this._channelTypeComboBox);
            this._channelsPanel.Controls.Add(this._linkChannelButton);
            this._channelsPanel.Controls.Add(this._refreshChannelsButton);
            this._channelsPanel.Controls.Add(this._channelsDataGridView);
            this._channelsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._channelsPanel.Location = new System.Drawing.Point(3, 3);
            this._channelsPanel.Name = "_channelsPanel";
            this._channelsPanel.Size = new System.Drawing.Size(586, 368);
            this._channelsPanel.TabIndex = 0;
            // 
            // _importChannelsButton
            // 
            this._importChannelsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._importChannelsButton.Location = new System.Drawing.Point(355, 345);
            this._importChannelsButton.Name = "_importChannelsButton";
            this._importChannelsButton.Size = new System.Drawing.Size(145, 23);
            this._importChannelsButton.TabIndex = 105;
            this._importChannelsButton.Text = "Create/Import Channels...";
            this._importChannelsButton.UseVisualStyleBackColor = true;
            this._importChannelsButton.Click += new System.EventHandler(this._importChannelsButton_Click);
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television Channels",
            "Radio Channels"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(6, 3);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(250, 21);
            this._channelTypeComboBox.TabIndex = 104;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // _linkChannelButton
            // 
            this._linkChannelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._linkChannelButton.Location = new System.Drawing.Point(6, 345);
            this._linkChannelButton.Name = "_linkChannelButton";
            this._linkChannelButton.Size = new System.Drawing.Size(110, 23);
            this._linkChannelButton.TabIndex = 2;
            this._linkChannelButton.Text = "Edit Channel Link";
            this._linkChannelButton.UseVisualStyleBackColor = true;
            this._linkChannelButton.Click += new System.EventHandler(this._linkChannelButton_Click);
            // 
            // _refreshChannelsButton
            // 
            this._refreshChannelsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._refreshChannelsButton.Location = new System.Drawing.Point(506, 345);
            this._refreshChannelsButton.Name = "_refreshChannelsButton";
            this._refreshChannelsButton.Size = new System.Drawing.Size(75, 23);
            this._refreshChannelsButton.TabIndex = 3;
            this._refreshChannelsButton.Text = "Refresh";
            this._refreshChannelsButton.UseVisualStyleBackColor = true;
            this._refreshChannelsButton.Click += new System.EventHandler(this._refreshButton_Click);
            // 
            // _channelsDataGridView
            // 
            this._channelsDataGridView.AllowUserToAddRows = false;
            this._channelsDataGridView.AllowUserToDeleteRows = false;
            this._channelsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._channelsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._channelsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._channelsDataGridView.AutoGenerateColumns = false;
            this._channelsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._channelsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._channelsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._channelsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._tvSchedulerChannelColumn,
            this._mediaPortalChannelColumn});
            this._channelsDataGridView.DataSource = this._channelItemsBindingSource;
            this._channelsDataGridView.GridColor = System.Drawing.Color.White;
            this._channelsDataGridView.Location = new System.Drawing.Point(6, 30);
            this._channelsDataGridView.MultiSelect = false;
            this._channelsDataGridView.Name = "_channelsDataGridView";
            this._channelsDataGridView.ReadOnly = true;
            this._channelsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._channelsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._channelsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._channelsDataGridView.Size = new System.Drawing.Size(575, 311);
            this._channelsDataGridView.StandardTab = true;
            this._channelsDataGridView.TabIndex = 1;
            this._channelsDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this._channelsDataGridView_CellFormatting);
            this._channelsDataGridView.SelectionChanged += new System.EventHandler(this._channelsDataGridView_SelectionChanged);
            this._channelsDataGridView.DoubleClick += new System.EventHandler(this._channelsDataGridView_DoubleClick);
            // 
            // _tvSchedulerChannelColumn
            // 
            this._tvSchedulerChannelColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._tvSchedulerChannelColumn.DataPropertyName = "ChannelName";
            this._tvSchedulerChannelColumn.FillWeight = 30F;
            this._tvSchedulerChannelColumn.HeaderText = "Name";
            this._tvSchedulerChannelColumn.Name = "_tvSchedulerChannelColumn";
            this._tvSchedulerChannelColumn.ReadOnly = true;
            // 
            // _mediaPortalChannelColumn
            // 
            this._mediaPortalChannelColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._mediaPortalChannelColumn.DataPropertyName = "Message";
            this._mediaPortalChannelColumn.FillWeight = 70F;
            this._mediaPortalChannelColumn.HeaderText = "Status";
            this._mediaPortalChannelColumn.Name = "_mediaPortalChannelColumn";
            this._mediaPortalChannelColumn.ReadOnly = true;
            // 
            // _notConnectedPanel
            // 
            this._notConnectedPanel.Controls.Add(this._notConnectedLabel);
            this._notConnectedPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._notConnectedPanel.Location = new System.Drawing.Point(3, 3);
            this._notConnectedPanel.Name = "_notConnectedPanel";
            this._notConnectedPanel.Size = new System.Drawing.Size(586, 368);
            this._notConnectedPanel.TabIndex = 1;
            // 
            // _notConnectedLabel
            // 
            this._notConnectedLabel.AutoSize = true;
            this._notConnectedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._notConnectedLabel.ForeColor = System.Drawing.Color.Red;
            this._notConnectedLabel.Location = new System.Drawing.Point(3, 3);
            this._notConnectedLabel.Name = "_notConnectedLabel";
            this._notConnectedLabel.Size = new System.Drawing.Size(334, 13);
            this._notConnectedLabel.TabIndex = 0;
            this._notConnectedLabel.Text = "Not connected to ARGUS TV, check server settings in Configuration.";
            // 
            // _uncRecordingGroupBox
            // 
            this._uncRecordingGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._uncRecordingGroupBox.Controls.Add(this._createUncShareButton);
            this._uncRecordingGroupBox.Controls.Add(this._uncPathsDataGridView);
            this._uncRecordingGroupBox.Location = new System.Drawing.Point(6, 6);
            this._uncRecordingGroupBox.Name = "_uncRecordingGroupBox";
            this._uncRecordingGroupBox.Size = new System.Drawing.Size(580, 152);
            this._uncRecordingGroupBox.TabIndex = 1;
            this._uncRecordingGroupBox.TabStop = false;
            this._uncRecordingGroupBox.Text = "UNC Recording Paths";
            // 
            // _createUncShareButton
            // 
            this._createUncShareButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._createUncShareButton.Location = new System.Drawing.Point(6, 123);
            this._createUncShareButton.Name = "_createUncShareButton";
            this._createUncShareButton.Size = new System.Drawing.Size(100, 23);
            this._createUncShareButton.TabIndex = 11;
            this._createUncShareButton.Text = "Create Share";
            this._createUncShareButton.UseVisualStyleBackColor = true;
            this._createUncShareButton.Click += new System.EventHandler(this._createUncShareButton_Click);
            // 
            // _uncPathsDataGridView
            // 
            this._uncPathsDataGridView.AllowUserToAddRows = false;
            this._uncPathsDataGridView.AllowUserToDeleteRows = false;
            this._uncPathsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this._uncPathsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this._uncPathsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._uncPathsDataGridView.AutoGenerateColumns = false;
            this._uncPathsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._uncPathsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._uncPathsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._uncPathsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this._uncPathsDataGridView.DataSource = this._uncPathsBindingSource;
            this._uncPathsDataGridView.GridColor = System.Drawing.Color.White;
            this._uncPathsDataGridView.Location = new System.Drawing.Point(6, 19);
            this._uncPathsDataGridView.MultiSelect = false;
            this._uncPathsDataGridView.Name = "_uncPathsDataGridView";
            this._uncPathsDataGridView.ReadOnly = true;
            this._uncPathsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this._uncPathsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this._uncPathsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._uncPathsDataGridView.Size = new System.Drawing.Size(568, 100);
            this._uncPathsDataGridView.StandardTab = true;
            this._uncPathsDataGridView.TabIndex = 10;
            this._uncPathsDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this._uncPathsDataGridView_CellFormatting);
            this._uncPathsDataGridView.SelectionChanged += new System.EventHandler(this._uncPathsDataGridView_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "CardName";
            this.dataGridViewTextBoxColumn1.FillWeight = 30F;
            this.dataGridViewTextBoxColumn1.HeaderText = "Card";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Message";
            this.dataGridViewTextBoxColumn2.FillWeight = 70F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Path";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // _refreshUncButton
            // 
            this._refreshUncButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._refreshUncButton.Location = new System.Drawing.Point(511, 322);
            this._refreshUncButton.Name = "_refreshUncButton";
            this._refreshUncButton.Size = new System.Drawing.Size(75, 23);
            this._refreshUncButton.TabIndex = 12;
            this._refreshUncButton.Text = "Refresh";
            this._refreshUncButton.UseVisualStyleBackColor = true;
            this._refreshUncButton.Click += new System.EventHandler(this._refreshUncButton_Click);
            // 
            // _syncTve3EpgcheckBox
            // 
            this._syncTve3EpgcheckBox.AutoSize = true;
            this._syncTve3EpgcheckBox.Location = new System.Drawing.Point(11, 18);
            this._syncTve3EpgcheckBox.Name = "_syncTve3EpgcheckBox";
            this._syncTve3EpgcheckBox.Size = new System.Drawing.Size(421, 17);
            this._syncTve3EpgcheckBox.TabIndex = 11;
            this._syncTve3EpgcheckBox.Text = "Send EPG data to ARGUS TV (don\'t forget to turn on grabbing on the channels too)";
            this._syncTve3EpgcheckBox.UseVisualStyleBackColor = true;
            this._syncTve3EpgcheckBox.CheckedChanged += new System.EventHandler(this._syncTve3EpgcheckBox_CheckedChanged);
            // 
            // _epgGroupBox
            // 
            this._epgGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._epgGroupBox.Controls.Add(this._epgAutoCreateChannelsWithGroupRadioButton);
            this._epgGroupBox.Controls.Add(this._epgAutoCreateChannelsDvbRadioButton);
            this._epgGroupBox.Controls.Add(this._epgOnlyLinkedChannelsRadioButton);
            this._epgGroupBox.Controls.Add(this._syncTve3EpgcheckBox);
            this._epgGroupBox.Location = new System.Drawing.Point(6, 138);
            this._epgGroupBox.Name = "_epgGroupBox";
            this._epgGroupBox.Size = new System.Drawing.Size(580, 116);
            this._epgGroupBox.TabIndex = 2;
            this._epgGroupBox.TabStop = false;
            this._epgGroupBox.Text = "DVB-EPG";
            // 
            // _epgAutoCreateChannelsWithGroupRadioButton
            // 
            this._epgAutoCreateChannelsWithGroupRadioButton.AutoSize = true;
            this._epgAutoCreateChannelsWithGroupRadioButton.Location = new System.Drawing.Point(26, 87);
            this._epgAutoCreateChannelsWithGroupRadioButton.Name = "_epgAutoCreateChannelsWithGroupRadioButton";
            this._epgAutoCreateChannelsWithGroupRadioButton.Size = new System.Drawing.Size(416, 17);
            this._epgAutoCreateChannelsWithGroupRadioButton.TabIndex = 14;
            this._epgAutoCreateChannelsWithGroupRadioButton.TabStop = true;
            this._epgAutoCreateChannelsWithGroupRadioButton.Text = "Automatically create and link channels in ARGUS TV (duplicate MediaPortal group)";
            this._epgAutoCreateChannelsWithGroupRadioButton.UseVisualStyleBackColor = true;
            // 
            // _epgAutoCreateChannelsDvbRadioButton
            // 
            this._epgAutoCreateChannelsDvbRadioButton.AutoSize = true;
            this._epgAutoCreateChannelsDvbRadioButton.Location = new System.Drawing.Point(26, 64);
            this._epgAutoCreateChannelsDvbRadioButton.Name = "_epgAutoCreateChannelsDvbRadioButton";
            this._epgAutoCreateChannelsDvbRadioButton.Size = new System.Drawing.Size(371, 17);
            this._epgAutoCreateChannelsDvbRadioButton.TabIndex = 13;
            this._epgAutoCreateChannelsDvbRadioButton.TabStop = true;
            this._epgAutoCreateChannelsDvbRadioButton.Text = "Automatically create and link channels in ARGUS TV (\"DVB-EPG\" group)";
            this._epgAutoCreateChannelsDvbRadioButton.UseVisualStyleBackColor = true;
            // 
            // _epgOnlyLinkedChannelsRadioButton
            // 
            this._epgOnlyLinkedChannelsRadioButton.AutoSize = true;
            this._epgOnlyLinkedChannelsRadioButton.Location = new System.Drawing.Point(26, 41);
            this._epgOnlyLinkedChannelsRadioButton.Name = "_epgOnlyLinkedChannelsRadioButton";
            this._epgOnlyLinkedChannelsRadioButton.Size = new System.Drawing.Size(227, 17);
            this._epgOnlyLinkedChannelsRadioButton.TabIndex = 12;
            this._epgOnlyLinkedChannelsRadioButton.TabStop = true;
            this._epgOnlyLinkedChannelsRadioButton.Text = "Only process EPG data for linked channels";
            this._epgOnlyLinkedChannelsRadioButton.UseVisualStyleBackColor = true;
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this._configTabPage);
            this._tabControl.Controls.Add(this._uncPathsTabPage);
            this._tabControl.Controls.Add(this._channelsTabPage);
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.Location = new System.Drawing.Point(0, 0);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(600, 400);
            this._tabControl.TabIndex = 15;
            this._tabControl.SelectedIndexChanged += new System.EventHandler(this._tabControl_SelectedIndexChanged);
            // 
            // _configTabPage
            // 
            this._configTabPage.Controls.Add(this._powerGroupBox);
            this._configTabPage.Controls.Add(this._tvSchedulerGroupBox);
            this._configTabPage.Controls.Add(this._epgGroupBox);
            this._configTabPage.Location = new System.Drawing.Point(4, 22);
            this._configTabPage.Name = "_configTabPage";
            this._configTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._configTabPage.Size = new System.Drawing.Size(592, 374);
            this._configTabPage.TabIndex = 0;
            this._configTabPage.Text = "Configuration";
            this._configTabPage.UseVisualStyleBackColor = true;
            // 
            // _powerGroupBox
            // 
            this._powerGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._powerGroupBox.Controls.Add(this._restartOnResumeCheckBox);
            this._powerGroupBox.Location = new System.Drawing.Point(6, 88);
            this._powerGroupBox.Name = "_powerGroupBox";
            this._powerGroupBox.Size = new System.Drawing.Size(580, 44);
            this._powerGroupBox.TabIndex = 3;
            this._powerGroupBox.TabStop = false;
            this._powerGroupBox.Text = "Power Management";
            // 
            // _restartOnResumeCheckBox
            // 
            this._restartOnResumeCheckBox.AutoSize = true;
            this._restartOnResumeCheckBox.Location = new System.Drawing.Point(11, 18);
            this._restartOnResumeCheckBox.Name = "_restartOnResumeCheckBox";
            this._restartOnResumeCheckBox.Size = new System.Drawing.Size(317, 17);
            this._restartOnResumeCheckBox.TabIndex = 16;
            this._restartOnResumeCheckBox.Text = "Re-initialize TV Server when the system resumes from standby";
            this._restartOnResumeCheckBox.UseVisualStyleBackColor = true;
            // 
            // _uncPathsTabPage
            // 
            this._uncPathsTabPage.Controls.Add(this._uncTimeshiftGroupBox);
            this._uncPathsTabPage.Controls.Add(this._refreshUncButton);
            this._uncPathsTabPage.Controls.Add(this._uncRecordingGroupBox);
            this._uncPathsTabPage.Location = new System.Drawing.Point(4, 22);
            this._uncPathsTabPage.Name = "_uncPathsTabPage";
            this._uncPathsTabPage.Size = new System.Drawing.Size(592, 374);
            this._uncPathsTabPage.TabIndex = 2;
            this._uncPathsTabPage.Text = "UNC Paths";
            this._uncPathsTabPage.UseVisualStyleBackColor = true;
            // 
            // _uncTimeshiftGroupBox
            // 
            this._uncTimeshiftGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._uncTimeshiftGroupBox.Controls.Add(this._createTimeshiftingShareButton);
            this._uncTimeshiftGroupBox.Controls.Add(this._uncTimeshiftingDataGridView);
            this._uncTimeshiftGroupBox.Location = new System.Drawing.Point(6, 164);
            this._uncTimeshiftGroupBox.Name = "_uncTimeshiftGroupBox";
            this._uncTimeshiftGroupBox.Size = new System.Drawing.Size(580, 152);
            this._uncTimeshiftGroupBox.TabIndex = 13;
            this._uncTimeshiftGroupBox.TabStop = false;
            this._uncTimeshiftGroupBox.Text = "UNC Timeshift Paths";
            // 
            // _createTimeshiftingShareButton
            // 
            this._createTimeshiftingShareButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._createTimeshiftingShareButton.Location = new System.Drawing.Point(6, 123);
            this._createTimeshiftingShareButton.Name = "_createTimeshiftingShareButton";
            this._createTimeshiftingShareButton.Size = new System.Drawing.Size(100, 23);
            this._createTimeshiftingShareButton.TabIndex = 11;
            this._createTimeshiftingShareButton.Text = "Create Share";
            this._createTimeshiftingShareButton.UseVisualStyleBackColor = true;
            this._createTimeshiftingShareButton.Click += new System.EventHandler(this._createTimeshiftingShareButton_Click);
            // 
            // _uncTimeshiftingDataGridView
            // 
            this._uncTimeshiftingDataGridView.AllowUserToAddRows = false;
            this._uncTimeshiftingDataGridView.AllowUserToDeleteRows = false;
            this._uncTimeshiftingDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this._uncTimeshiftingDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this._uncTimeshiftingDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._uncTimeshiftingDataGridView.AutoGenerateColumns = false;
            this._uncTimeshiftingDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._uncTimeshiftingDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._uncTimeshiftingDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._uncTimeshiftingDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4});
            this._uncTimeshiftingDataGridView.DataSource = this._uncTimeshiftingBindingSource;
            this._uncTimeshiftingDataGridView.GridColor = System.Drawing.Color.White;
            this._uncTimeshiftingDataGridView.Location = new System.Drawing.Point(6, 19);
            this._uncTimeshiftingDataGridView.MultiSelect = false;
            this._uncTimeshiftingDataGridView.Name = "_uncTimeshiftingDataGridView";
            this._uncTimeshiftingDataGridView.ReadOnly = true;
            this._uncTimeshiftingDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            this._uncTimeshiftingDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this._uncTimeshiftingDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._uncTimeshiftingDataGridView.Size = new System.Drawing.Size(568, 100);
            this._uncTimeshiftingDataGridView.StandardTab = true;
            this._uncTimeshiftingDataGridView.TabIndex = 10;
            this._uncTimeshiftingDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this._uncPathsDataGridView_CellFormatting);
            this._uncTimeshiftingDataGridView.SelectionChanged += new System.EventHandler(this._uncTimeshiftingDataGridView_SelectionChanged);
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "CardName";
            this.dataGridViewTextBoxColumn3.FillWeight = 30F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Card";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Message";
            this.dataGridViewTextBoxColumn4.FillWeight = 70F;
            this.dataGridViewTextBoxColumn4.HeaderText = "Path";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // _channelsTabPage
            // 
            this._channelsTabPage.Controls.Add(this._channelsPanel);
            this._channelsTabPage.Controls.Add(this._notConnectedPanel);
            this._channelsTabPage.Location = new System.Drawing.Point(4, 22);
            this._channelsTabPage.Name = "_channelsTabPage";
            this._channelsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._channelsTabPage.Size = new System.Drawing.Size(592, 374);
            this._channelsTabPage.TabIndex = 1;
            this._channelsTabPage.Text = "Channels";
            this._channelsTabPage.UseVisualStyleBackColor = true;
            // 
            // SetupForm
            // 
            this.Controls.Add(this._tabControl);
            this.Name = "SetupForm";
            this.Size = new System.Drawing.Size(600, 400);
            this._tvSchedulerGroupBox.ResumeLayout(false);
            this._tvSchedulerGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._portNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._pluginTcpNumericUpDown)).EndInit();
            this._channelsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._channelsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelItemsBindingSource)).EndInit();
            this._notConnectedPanel.ResumeLayout(false);
            this._notConnectedPanel.PerformLayout();
            this._uncRecordingGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._uncPathsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._uncPathsBindingSource)).EndInit();
            this._epgGroupBox.ResumeLayout(false);
            this._epgGroupBox.PerformLayout();
            this._tabControl.ResumeLayout(false);
            this._configTabPage.ResumeLayout(false);
            this._powerGroupBox.ResumeLayout(false);
            this._powerGroupBox.PerformLayout();
            this._uncPathsTabPage.ResumeLayout(false);
            this._uncTimeshiftGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._uncTimeshiftingDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._uncTimeshiftingBindingSource)).EndInit();
            this._channelsTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _tvSchedulerGroupBox;
        private System.Windows.Forms.NumericUpDown _portNumericUpDown;
        private System.Windows.Forms.Label _tcpPortLabel;
        private System.Windows.Forms.TextBox _serverTextBox;
        private System.Windows.Forms.Label _serverLabel;
        private System.Windows.Forms.Button _connectButton;
        private System.Windows.Forms.DataGridView _channelsDataGridView;
        private System.Windows.Forms.Panel _notConnectedPanel;
        private System.Windows.Forms.Panel _channelsPanel;
        private System.Windows.Forms.Label _notConnectedLabel;
        private System.Windows.Forms.BindingSource _channelItemsBindingSource;
        private System.Windows.Forms.GroupBox _uncRecordingGroupBox;
        private System.Windows.Forms.CheckBox _syncTve3EpgcheckBox;
        private System.Windows.Forms.Button _refreshChannelsButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn _tvSchedulerChannelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _mediaPortalChannelColumn;
        private System.Windows.Forms.NumericUpDown _pluginTcpNumericUpDown;
        private System.Windows.Forms.Label _pluginTcpLabel;
        private System.Windows.Forms.Label _restartTcpLabel;
        private System.Windows.Forms.GroupBox _epgGroupBox;
        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.TabPage _configTabPage;
        private System.Windows.Forms.TabPage _channelsTabPage;
        private System.Windows.Forms.DataGridView _uncPathsDataGridView;
        private System.Windows.Forms.Button _refreshUncButton;
        private System.Windows.Forms.BindingSource _uncPathsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Button _createUncShareButton;
        private System.Windows.Forms.GroupBox _powerGroupBox;
        private System.Windows.Forms.CheckBox _restartOnResumeCheckBox;
        private System.Windows.Forms.Button _linkChannelButton;
        private System.Windows.Forms.TabPage _uncPathsTabPage;
        private System.Windows.Forms.GroupBox _uncTimeshiftGroupBox;
        private System.Windows.Forms.Button _createTimeshiftingShareButton;
        private System.Windows.Forms.DataGridView _uncTimeshiftingDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.BindingSource _uncTimeshiftingBindingSource;
        private System.Windows.Forms.ComboBox _channelTypeComboBox;
        private System.Windows.Forms.Button _importChannelsButton;
        private System.Windows.Forms.RadioButton _epgAutoCreateChannelsWithGroupRadioButton;
        private System.Windows.Forms.RadioButton _epgAutoCreateChannelsDvbRadioButton;
        private System.Windows.Forms.RadioButton _epgOnlyLinkedChannelsRadioButton;
      }
}
