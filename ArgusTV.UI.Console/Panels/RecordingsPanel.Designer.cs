namespace ArgusTV.UI.Console.Panels
{
    partial class RecordingsPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordingsPanel));
            this._recordingsTreeView = new ArgusTV.WinForms.Controls.MultiSelectTreeView();
            this._detailsGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this._applyKeepButton = new System.Windows.Forms.Button();
            this._keepUntilControl = new ArgusTV.UI.Console.UserControls.KeepUntilControl();
            this._recStartLabel = new System.Windows.Forms.Label();
            this._isPartialCheckBox = new System.Windows.Forms.CheckBox();
            this._recStartTextBox = new System.Windows.Forms.TextBox();
            this._keepLabel = new System.Windows.Forms.Label();
            this._scheduleNameTextBox = new System.Windows.Forms.TextBox();
            this._recStopLabel = new System.Windows.Forms.Label();
            this._recStopTextBox = new System.Windows.Forms.TextBox();
            this._scheduleLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this._keepUntilTextBox = new System.Windows.Forms.TextBox();
            this._lastWatchedLabel = new System.Windows.Forms.Label();
            this._lastWatchedTextBox = new System.Windows.Forms.TextBox();
            this._descriptionTextBox = new System.Windows.Forms.TextBox();
            this._descLabel = new System.Windows.Forms.Label();
            this._keepUntilLabel = new System.Windows.Forms.Label();
            this._includeNonExistingCheckBox = new System.Windows.Forms.CheckBox();
            this._showByLabel = new System.Windows.Forms.Label();
            this._showByComboBox = new System.Windows.Forms.ComboBox();
            this._syncRecordingButton = new System.Windows.Forms.Button();
            this._loadingPanel = new System.Windows.Forms.Panel();
            this._busyPictureBox = new System.Windows.Forms.PictureBox();
            this._loadingLabel = new System.Windows.Forms.Label();
            this._backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this._exportButton = new System.Windows.Forms.Button();
            this._recordingContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._openContainingFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._showRecordingThumbnailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._manuallyRunPostProcessingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._resetWatchedStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._deleteRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._diskSpaceProgressBar = new System.Windows.Forms.ProgressBar();
            this._diskLabel = new System.Windows.Forms.Label();
            this._toolTip = new System.Windows.Forms.ToolTip(this.components);
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
            this._recordedOnLabel = new System.Windows.Forms.Label();
            this._detailsGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this._loadingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._busyPictureBox)).BeginInit();
            this._recordingContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _recordingsTreeView
            // 
            this._recordingsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._recordingsTreeView.HideSelection = false;
            this._recordingsTreeView.ItemHeight = 18;
            this._recordingsTreeView.Location = new System.Drawing.Point(0, 47);
            this._recordingsTreeView.Name = "_recordingsTreeView";
            this._recordingsTreeView.Size = new System.Drawing.Size(708, 221);
            this._recordingsTreeView.TabIndex = 20;
            this._recordingsTreeView.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this._recordingsTreeView_AfterCollapse);
            this._recordingsTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this._recordingsTreeView_BeforeExpand);
            this._recordingsTreeView.DoubleClick += new System.EventHandler(this._recordingsTreeView_DoubleClick);
            this._recordingsTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this._recordingsTreeView_KeyUp);
            this._recordingsTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._recordingsTreeView_NodeMouseClick);
            this._recordingsTreeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this._recordingsTreeView_AfterExpand);
            // 
            // _detailsGroupBox
            // 
            this._detailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._detailsGroupBox.Controls.Add(this.tableLayoutPanel1);
            this._detailsGroupBox.Location = new System.Drawing.Point(0, 272);
            this._detailsGroupBox.Name = "_detailsGroupBox";
            this._detailsGroupBox.Size = new System.Drawing.Size(708, 124);
            this._detailsGroupBox.TabIndex = 30;
            this._detailsGroupBox.TabStop = false;
            this._detailsGroupBox.Text = "Details";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(702, 105);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._applyKeepButton);
            this.panel1.Controls.Add(this._keepUntilControl);
            this.panel1.Controls.Add(this._recStartLabel);
            this.panel1.Controls.Add(this._isPartialCheckBox);
            this.panel1.Controls.Add(this._recStartTextBox);
            this.panel1.Controls.Add(this._keepLabel);
            this.panel1.Controls.Add(this._scheduleNameTextBox);
            this.panel1.Controls.Add(this._recStopLabel);
            this.panel1.Controls.Add(this._recStopTextBox);
            this.panel1.Controls.Add(this._scheduleLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(345, 99);
            this.panel1.TabIndex = 0;
            // 
            // _applyKeepButton
            // 
            this._applyKeepButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._applyKeepButton.Location = new System.Drawing.Point(290, 77);
            this._applyKeepButton.Name = "_applyKeepButton";
            this._applyKeepButton.Size = new System.Drawing.Size(55, 23);
            this._applyKeepButton.TabIndex = 9;
            this._applyKeepButton.Text = "Apply";
            this._applyKeepButton.UseVisualStyleBackColor = true;
            this._applyKeepButton.Click += new System.EventHandler(this._applyKeepButton_Click);
            // 
            // _keepUntilControl
            // 
            this._keepUntilControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._keepUntilControl.Enabled = false;
            this._keepUntilControl.Location = new System.Drawing.Point(88, 78);
            this._keepUntilControl.MaximumSize = new System.Drawing.Size(9999, 21);
            this._keepUntilControl.MinimumSize = new System.Drawing.Size(150, 21);
            this._keepUntilControl.Name = "_keepUntilControl";
            this._keepUntilControl.Size = new System.Drawing.Size(196, 21);
            this._keepUntilControl.TabIndex = 8;
            this._keepUntilControl.KeepUntilChanged += new System.EventHandler(this._keepUntilControl_KeepUntilChanged);
            // 
            // _recStartLabel
            // 
            this._recStartLabel.AutoSize = true;
            this._recStartLabel.Location = new System.Drawing.Point(0, 29);
            this._recStartLabel.Name = "_recStartLabel";
            this._recStartLabel.Size = new System.Drawing.Size(82, 13);
            this._recStartLabel.TabIndex = 2;
            this._recStartLabel.Text = "Recording start:";
            // 
            // _isPartialCheckBox
            // 
            this._isPartialCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._isPartialCheckBox.AutoSize = true;
            this._isPartialCheckBox.Enabled = false;
            this._isPartialCheckBox.Location = new System.Drawing.Point(290, 54);
            this._isPartialCheckBox.Name = "_isPartialCheckBox";
            this._isPartialCheckBox.Size = new System.Drawing.Size(55, 17);
            this._isPartialCheckBox.TabIndex = 6;
            this._isPartialCheckBox.Text = "Partial";
            this._isPartialCheckBox.UseVisualStyleBackColor = true;
            // 
            // _recStartTextBox
            // 
            this._recStartTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._recStartTextBox.Location = new System.Drawing.Point(88, 26);
            this._recStartTextBox.Name = "_recStartTextBox";
            this._recStartTextBox.ReadOnly = true;
            this._recStartTextBox.Size = new System.Drawing.Size(257, 20);
            this._recStartTextBox.TabIndex = 3;
            // 
            // _keepLabel
            // 
            this._keepLabel.AutoSize = true;
            this._keepLabel.Location = new System.Drawing.Point(0, 81);
            this._keepLabel.Name = "_keepLabel";
            this._keepLabel.Size = new System.Drawing.Size(35, 13);
            this._keepLabel.TabIndex = 7;
            this._keepLabel.Text = "Keep:";
            // 
            // _scheduleNameTextBox
            // 
            this._scheduleNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._scheduleNameTextBox.Location = new System.Drawing.Point(88, 0);
            this._scheduleNameTextBox.Name = "_scheduleNameTextBox";
            this._scheduleNameTextBox.ReadOnly = true;
            this._scheduleNameTextBox.Size = new System.Drawing.Size(257, 20);
            this._scheduleNameTextBox.TabIndex = 1;
            // 
            // _recStopLabel
            // 
            this._recStopLabel.AutoSize = true;
            this._recStopLabel.Location = new System.Drawing.Point(0, 55);
            this._recStopLabel.Name = "_recStopLabel";
            this._recStopLabel.Size = new System.Drawing.Size(82, 13);
            this._recStopLabel.TabIndex = 4;
            this._recStopLabel.Text = "Recording stop:";
            // 
            // _recStopTextBox
            // 
            this._recStopTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._recStopTextBox.Location = new System.Drawing.Point(88, 52);
            this._recStopTextBox.Name = "_recStopTextBox";
            this._recStopTextBox.ReadOnly = true;
            this._recStopTextBox.Size = new System.Drawing.Size(196, 20);
            this._recStopTextBox.TabIndex = 5;
            // 
            // _scheduleLabel
            // 
            this._scheduleLabel.AutoSize = true;
            this._scheduleLabel.Location = new System.Drawing.Point(0, 3);
            this._scheduleLabel.Name = "_scheduleLabel";
            this._scheduleLabel.Size = new System.Drawing.Size(84, 13);
            this._scheduleLabel.TabIndex = 0;
            this._scheduleLabel.Text = "Schedule name:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this._keepUntilTextBox);
            this.panel2.Controls.Add(this._lastWatchedLabel);
            this.panel2.Controls.Add(this._lastWatchedTextBox);
            this.panel2.Controls.Add(this._descriptionTextBox);
            this.panel2.Controls.Add(this._descLabel);
            this.panel2.Controls.Add(this._keepUntilLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(354, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(345, 99);
            this.panel2.TabIndex = 1;
            // 
            // _keepUntilTextBox
            // 
            this._keepUntilTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._keepUntilTextBox.Location = new System.Drawing.Point(80, 78);
            this._keepUntilTextBox.Name = "_keepUntilTextBox";
            this._keepUntilTextBox.ReadOnly = true;
            this._keepUntilTextBox.Size = new System.Drawing.Size(265, 20);
            this._keepUntilTextBox.TabIndex = 14;
            // 
            // _lastWatchedLabel
            // 
            this._lastWatchedLabel.AutoSize = true;
            this._lastWatchedLabel.Location = new System.Drawing.Point(0, 55);
            this._lastWatchedLabel.Name = "_lastWatchedLabel";
            this._lastWatchedLabel.Size = new System.Drawing.Size(74, 13);
            this._lastWatchedLabel.TabIndex = 11;
            this._lastWatchedLabel.Text = "Last watched:";
            // 
            // _lastWatchedTextBox
            // 
            this._lastWatchedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lastWatchedTextBox.Location = new System.Drawing.Point(80, 52);
            this._lastWatchedTextBox.Name = "_lastWatchedTextBox";
            this._lastWatchedTextBox.ReadOnly = true;
            this._lastWatchedTextBox.Size = new System.Drawing.Size(265, 20);
            this._lastWatchedTextBox.TabIndex = 12;
            // 
            // _descriptionTextBox
            // 
            this._descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._descriptionTextBox.Location = new System.Drawing.Point(80, 0);
            this._descriptionTextBox.Multiline = true;
            this._descriptionTextBox.Name = "_descriptionTextBox";
            this._descriptionTextBox.ReadOnly = true;
            this._descriptionTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._descriptionTextBox.Size = new System.Drawing.Size(265, 45);
            this._descriptionTextBox.TabIndex = 10;
            // 
            // _descLabel
            // 
            this._descLabel.AutoSize = true;
            this._descLabel.Location = new System.Drawing.Point(0, 3);
            this._descLabel.Name = "_descLabel";
            this._descLabel.Size = new System.Drawing.Size(63, 13);
            this._descLabel.TabIndex = 9;
            this._descLabel.Text = "Description:";
            // 
            // _keepUntilLabel
            // 
            this._keepUntilLabel.AutoSize = true;
            this._keepUntilLabel.Location = new System.Drawing.Point(0, 81);
            this._keepUntilLabel.Name = "_keepUntilLabel";
            this._keepUntilLabel.Size = new System.Drawing.Size(57, 13);
            this._keepUntilLabel.TabIndex = 13;
            this._keepUntilLabel.Text = "Keep until:";
            // 
            // _includeNonExistingCheckBox
            // 
            this._includeNonExistingCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._includeNonExistingCheckBox.AutoSize = true;
            this._includeNonExistingCheckBox.Checked = true;
            this._includeNonExistingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._includeNonExistingCheckBox.Location = new System.Drawing.Point(558, 25);
            this._includeNonExistingCheckBox.Name = "_includeNonExistingCheckBox";
            this._includeNonExistingCheckBox.Size = new System.Drawing.Size(150, 17);
            this._includeNonExistingCheckBox.TabIndex = 14;
            this._includeNonExistingCheckBox.Text = "Include missing recordings";
            this._includeNonExistingCheckBox.UseVisualStyleBackColor = true;
            this._includeNonExistingCheckBox.CheckedChanged += new System.EventHandler(this._includeNonExistingCheckBox_CheckedChanged);
            // 
            // _showByLabel
            // 
            this._showByLabel.AutoSize = true;
            this._showByLabel.Location = new System.Drawing.Point(1, 26);
            this._showByLabel.Name = "_showByLabel";
            this._showByLabel.Size = new System.Drawing.Size(53, 13);
            this._showByLabel.TabIndex = 10;
            this._showByLabel.Text = "Group by:";
            // 
            // _showByComboBox
            // 
            this._showByComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._showByComboBox.FormattingEnabled = true;
            this._showByComboBox.ItemHeight = 13;
            this._showByComboBox.Items.AddRange(new object[] {
            "Title",
            "Schedule",
            "Category",
            "Channel"});
            this._showByComboBox.Location = new System.Drawing.Point(60, 22);
            this._showByComboBox.Name = "_showByComboBox";
            this._showByComboBox.Size = new System.Drawing.Size(150, 21);
            this._showByComboBox.TabIndex = 11;
            this._showByComboBox.SelectedIndexChanged += new System.EventHandler(this._showByComboBox_SelectedIndexChanged);
            // 
            // _syncRecordingButton
            // 
            this._syncRecordingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._syncRecordingButton.Location = new System.Drawing.Point(0, 400);
            this._syncRecordingButton.Name = "_syncRecordingButton";
            this._syncRecordingButton.Size = new System.Drawing.Size(150, 23);
            this._syncRecordingButton.TabIndex = 50;
            this._syncRecordingButton.Text = "Import/Synchronize...";
            this._syncRecordingButton.UseVisualStyleBackColor = true;
            this._syncRecordingButton.Click += new System.EventHandler(this._syncRecordingButton_Click);
            // 
            // _loadingPanel
            // 
            this._loadingPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._loadingPanel.BackColor = System.Drawing.SystemColors.Window;
            this._loadingPanel.Controls.Add(this._busyPictureBox);
            this._loadingPanel.Controls.Add(this._loadingLabel);
            this._loadingPanel.Location = new System.Drawing.Point(274, 125);
            this._loadingPanel.Name = "_loadingPanel";
            this._loadingPanel.Size = new System.Drawing.Size(160, 50);
            this._loadingPanel.TabIndex = 999;
            this._loadingPanel.Visible = false;
            // 
            // _busyPictureBox
            // 
            this._busyPictureBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._busyPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("_busyPictureBox.Image")));
            this._busyPictureBox.Location = new System.Drawing.Point(72, 26);
            this._busyPictureBox.Name = "_busyPictureBox";
            this._busyPictureBox.Size = new System.Drawing.Size(16, 16);
            this._busyPictureBox.TabIndex = 4;
            this._busyPictureBox.TabStop = false;
            // 
            // _loadingLabel
            // 
            this._loadingLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._loadingLabel.AutoSize = true;
            this._loadingLabel.Location = new System.Drawing.Point(55, 8);
            this._loadingLabel.Name = "_loadingLabel";
            this._loadingLabel.Size = new System.Drawing.Size(54, 13);
            this._loadingLabel.TabIndex = 3;
            this._loadingLabel.Text = "Loading...";
            // 
            // _backgroundWorker
            // 
            this._backgroundWorker.WorkerReportsProgress = true;
            this._backgroundWorker.WorkerSupportsCancellation = true;
            this._backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._backgroundWorker_DoWork);
            this._backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._backgroundWorker_RunWorkerCompleted);
            // 
            // _exportButton
            // 
            this._exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._exportButton.Location = new System.Drawing.Point(156, 400);
            this._exportButton.Name = "_exportButton";
            this._exportButton.Size = new System.Drawing.Size(75, 23);
            this._exportButton.TabIndex = 51;
            this._exportButton.Text = "Export...";
            this._exportButton.UseVisualStyleBackColor = true;
            this._exportButton.Click += new System.EventHandler(this._exportButton_Click);
            // 
            // _recordingContextMenuStrip
            // 
            this._recordingContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._openToolStripMenuItem,
            this._openContainingFolderToolStripMenuItem,
            this._showRecordingThumbnailToolStripMenuItem,
            this._manuallyRunPostProcessingToolStripMenuItem,
            this._resetWatchedStatusToolStripMenuItem,
            this._deleteRecordingToolStripMenuItem});
            this._recordingContextMenuStrip.Name = "_recordingContextMenuStrip";
            this._recordingContextMenuStrip.Size = new System.Drawing.Size(234, 158);
            // 
            // _openToolStripMenuItem
            // 
            this._openToolStripMenuItem.Name = "_openToolStripMenuItem";
            this._openToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this._openToolStripMenuItem.Text = "&Open";
            this._openToolStripMenuItem.Click += new System.EventHandler(this._openToolStripMenuItem_Click);
            // 
            // _openContainingFolderToolStripMenuItem
            // 
            this._openContainingFolderToolStripMenuItem.Name = "_openContainingFolderToolStripMenuItem";
            this._openContainingFolderToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this._openContainingFolderToolStripMenuItem.Text = "Open Containing &Folder";
            this._openContainingFolderToolStripMenuItem.Click += new System.EventHandler(this._openContainingFolderToolStripMenuItem_Click);
            // 
            // _showRecordingThumbnailToolStripMenuItem
            // 
            this._showRecordingThumbnailToolStripMenuItem.Name = "_showRecordingThumbnailToolStripMenuItem";
            this._showRecordingThumbnailToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this._showRecordingThumbnailToolStripMenuItem.Text = "Show Thumbnail";
            this._showRecordingThumbnailToolStripMenuItem.Click += new System.EventHandler(this._showRecordingThumbnailToolStripMenuItem_Click);
            // 
            // _manuallyRunPostProcessingToolStripMenuItem
            // 
            this._manuallyRunPostProcessingToolStripMenuItem.Name = "_manuallyRunPostProcessingToolStripMenuItem";
            this._manuallyRunPostProcessingToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this._manuallyRunPostProcessingToolStripMenuItem.Text = "Manually Run Post Processing";
            // 
            // _resetWatchedStatusToolStripMenuItem
            // 
            this._resetWatchedStatusToolStripMenuItem.Name = "_resetWatchedStatusToolStripMenuItem";
            this._resetWatchedStatusToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this._resetWatchedStatusToolStripMenuItem.Text = "&Reset Watched Status";
            this._resetWatchedStatusToolStripMenuItem.Click += new System.EventHandler(this._resetWatchedStatusToolStripMenuItem_Click);
            // 
            // _deleteRecordingToolStripMenuItem
            // 
            this._deleteRecordingToolStripMenuItem.Name = "_deleteRecordingToolStripMenuItem";
            this._deleteRecordingToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this._deleteRecordingToolStripMenuItem.Text = "&Delete Recording";
            this._deleteRecordingToolStripMenuItem.Click += new System.EventHandler(this._deleteRecordingToolStripMenuItem_Click);
            // 
            // _diskSpaceProgressBar
            // 
            this._diskSpaceProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._diskSpaceProgressBar.Location = new System.Drawing.Point(60, 0);
            this._diskSpaceProgressBar.Name = "_diskSpaceProgressBar";
            this._diskSpaceProgressBar.Size = new System.Drawing.Size(648, 18);
            this._diskSpaceProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._diskSpaceProgressBar.TabIndex = 0;
            // 
            // _diskLabel
            // 
            this._diskLabel.AutoSize = true;
            this._diskLabel.Location = new System.Drawing.Point(0, 2);
            this._diskLabel.Name = "_diskLabel";
            this._diskLabel.Size = new System.Drawing.Size(57, 13);
            this._diskLabel.TabIndex = 1001;
            this._diskLabel.Text = "Disk used:";
            // 
            // _toolTip
            // 
            this._toolTip.AutoPopDelay = 5000;
            this._toolTip.InitialDelay = 500;
            this._toolTip.IsBalloon = true;
            this._toolTip.ReshowDelay = 100;
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television",
            "Radio"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(298, 22);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(150, 21);
            this._channelTypeComboBox.TabIndex = 13;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // _recordedOnLabel
            // 
            this._recordedOnLabel.AutoSize = true;
            this._recordedOnLabel.Location = new System.Drawing.Point(220, 26);
            this._recordedOnLabel.Name = "_recordedOnLabel";
            this._recordedOnLabel.Size = new System.Drawing.Size(72, 13);
            this._recordedOnLabel.TabIndex = 12;
            this._recordedOnLabel.Text = "Recorded on:";
            // 
            // RecordingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._recordedOnLabel);
            this.Controls.Add(this._channelTypeComboBox);
            this.Controls.Add(this._diskSpaceProgressBar);
            this.Controls.Add(this._diskLabel);
            this.Controls.Add(this._loadingPanel);
            this.Controls.Add(this._exportButton);
            this.Controls.Add(this._syncRecordingButton);
            this.Controls.Add(this._showByComboBox);
            this.Controls.Add(this._showByLabel);
            this.Controls.Add(this._includeNonExistingCheckBox);
            this.Controls.Add(this._detailsGroupBox);
            this.Controls.Add(this._recordingsTreeView);
            this.Name = "RecordingsPanel";
            this.Size = new System.Drawing.Size(708, 423);
            this.Load += new System.EventHandler(this.RecordingsPanel_Load);
            this._detailsGroupBox.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this._loadingPanel.ResumeLayout(false);
            this._loadingPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._busyPictureBox)).EndInit();
            this._recordingContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ArgusTV.WinForms.Controls.MultiSelectTreeView _recordingsTreeView;
        private System.Windows.Forms.GroupBox _detailsGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label _scheduleLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox _recStopTextBox;
        private System.Windows.Forms.Label _recStopLabel;
        private System.Windows.Forms.Label _keepLabel;
        private System.Windows.Forms.Label _recStartLabel;
        private System.Windows.Forms.TextBox _recStartTextBox;
        private System.Windows.Forms.TextBox _scheduleNameTextBox;
        private System.Windows.Forms.TextBox _descriptionTextBox;
        private System.Windows.Forms.Label _descLabel;
        private System.Windows.Forms.CheckBox _isPartialCheckBox;
        private System.Windows.Forms.Label _keepUntilLabel;
        private System.Windows.Forms.Label _lastWatchedLabel;
        private System.Windows.Forms.TextBox _lastWatchedTextBox;
        private System.Windows.Forms.CheckBox _includeNonExistingCheckBox;
        private ArgusTV.UI.Console.UserControls.KeepUntilControl _keepUntilControl;
        private System.Windows.Forms.TextBox _keepUntilTextBox;
        private System.Windows.Forms.Button _applyKeepButton;
        private System.Windows.Forms.Label _showByLabel;
        private System.Windows.Forms.ComboBox _showByComboBox;
        private System.Windows.Forms.Button _syncRecordingButton;
        private System.Windows.Forms.Panel _loadingPanel;
        private System.Windows.Forms.PictureBox _busyPictureBox;
        private System.Windows.Forms.Label _loadingLabel;
        private System.ComponentModel.BackgroundWorker _backgroundWorker;
        private System.Windows.Forms.Button _exportButton;
        private System.Windows.Forms.ContextMenuStrip _recordingContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _openContainingFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _resetWatchedStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _deleteRecordingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _manuallyRunPostProcessingToolStripMenuItem;
        private System.Windows.Forms.ProgressBar _diskSpaceProgressBar;
        private System.Windows.Forms.Label _diskLabel;
        private System.Windows.Forms.ToolTip _toolTip;
        private System.Windows.Forms.ComboBox _channelTypeComboBox;
        private System.Windows.Forms.Label _recordedOnLabel;
        private System.Windows.Forms.ToolStripMenuItem _showRecordingThumbnailToolStripMenuItem;
    }
}
