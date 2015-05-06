namespace ArgusTV.UI.Console.Panels
{
    partial class ChannelsPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._channelsDataGridView = new System.Windows.Forms.DataGridView();
            this._visibleColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this._lcnColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._channelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._guideChannelColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this._preRecColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._postRecColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._startColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._stopColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._channelsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._moveUpButton = new System.Windows.Forms.Button();
            this._moveDownButton = new System.Windows.Forms.Button();
            this._moveBottomButton = new System.Windows.Forms.Button();
            this._moveTopButton = new System.Windows.Forms.Button();
            this._deleteButton = new System.Windows.Forms.Button();
            this._createNewButton = new System.Windows.Forms.Button();
            this._sortByLcnButton = new System.Windows.Forms.Button();
            this._visibleOffButton = new System.Windows.Forms.Button();
            this._visibleOnButton = new System.Windows.Forms.Button();
            this._channelGroupControl = new ArgusTV.WinForms.UserControls.ChannelGroupControl();
            this._editSelectedRowsButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._channelsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).BeginInit();
            this.SuspendLayout();
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
            this._channelsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._channelsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._visibleColumn,
            this._lcnColumn,
            this._channelColumn,
            this._guideChannelColumn,
            this._preRecColumn,
            this._postRecColumn,
            this._startColumn,
            this._stopColumn});
            this._channelsDataGridView.DataSource = this._channelsBindingSource;
            this._channelsDataGridView.GridColor = System.Drawing.Color.White;
            this._channelsDataGridView.Location = new System.Drawing.Point(0, 27);
            this._channelsDataGridView.Name = "_channelsDataGridView";
            this._channelsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._channelsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._channelsDataGridView.RowTemplate.ReadOnly = true;
            this._channelsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._channelsDataGridView.Size = new System.Drawing.Size(702, 490);
            this._channelsDataGridView.TabIndex = 2;
            this._channelsDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._channelsDataGridView_CellDoubleClick);
            this._channelsDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this._channelsDataGridView_CellValueChanged);
            this._channelsDataGridView.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this._channelsDataGridView_RowLeave);
            this._channelsDataGridView.SelectionChanged += new System.EventHandler(this._channelsDataGridView_SelectionChanged);
            // 
            // _visibleColumn
            // 
            this._visibleColumn.DataPropertyName = "VisibleInGuide";
            this._visibleColumn.HeaderText = "Visible";
            this._visibleColumn.Name = "_visibleColumn";
            this._visibleColumn.Width = 50;
            // 
            // _lcnColumn
            // 
            this._lcnColumn.DataPropertyName = "LogicalChannelNumber";
            this._lcnColumn.HeaderText = "LCN";
            this._lcnColumn.Name = "_lcnColumn";
            this._lcnColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._lcnColumn.ToolTipText = "Logical Channel Number";
            this._lcnColumn.Width = 60;
            // 
            // _channelColumn
            // 
            this._channelColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._channelColumn.DataPropertyName = "DisplayName";
            this._channelColumn.HeaderText = "Channel";
            this._channelColumn.Name = "_channelColumn";
            this._channelColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _guideChannelColumn
            // 
            this._guideChannelColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._guideChannelColumn.HeaderText = "Guide Channel";
            this._guideChannelColumn.Name = "_guideChannelColumn";
            // 
            // _preRecColumn
            // 
            this._preRecColumn.DataPropertyName = "DefaultPreRecordSeconds";
            this._preRecColumn.HeaderText = "Pre-rec";
            this._preRecColumn.Name = "_preRecColumn";
            this._preRecColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._preRecColumn.ToolTipText = "Default Pre-recording (sec)";
            this._preRecColumn.Width = 60;
            // 
            // _postRecColumn
            // 
            this._postRecColumn.DataPropertyName = "DefaultPostRecordSeconds";
            this._postRecColumn.HeaderText = "Post-rec";
            this._postRecColumn.Name = "_postRecColumn";
            this._postRecColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._postRecColumn.ToolTipText = "Default Post-recording (sec)";
            this._postRecColumn.Width = 60;
            // 
            // _startColumn
            // 
            this._startColumn.DataPropertyName = "BroadcastStart";
            this._startColumn.HeaderText = "Start";
            this._startColumn.Name = "_startColumn";
            this._startColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._startColumn.ToolTipText = "If this channel is shared, the time this channel starts broadcasting";
            this._startColumn.Width = 60;
            // 
            // _stopColumn
            // 
            this._stopColumn.DataPropertyName = "BroadcastStop";
            this._stopColumn.HeaderText = "Stop";
            this._stopColumn.Name = "_stopColumn";
            this._stopColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._stopColumn.ToolTipText = "If this channel is shared, the time this channel stops broadcasting";
            this._stopColumn.Width = 60;
            // 
            // _channelsBindingSource
            // 
            this._channelsBindingSource.DataSource = typeof(ArgusTV.UI.Process.SortableBindingList<ArgusTV.DataContracts.Channel>);
            this._channelsBindingSource.Sort = "";
            // 
            // _moveUpButton
            // 
            this._moveUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._moveUpButton.Enabled = false;
            this._moveUpButton.Location = new System.Drawing.Point(708, 80);
            this._moveUpButton.Name = "_moveUpButton";
            this._moveUpButton.Size = new System.Drawing.Size(90, 23);
            this._moveUpButton.TabIndex = 94;
            this._moveUpButton.Text = "Move Up";
            this._moveUpButton.UseVisualStyleBackColor = true;
            this._moveUpButton.Click += new System.EventHandler(this._moveUpButton_Click);
            // 
            // _moveDownButton
            // 
            this._moveDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._moveDownButton.Enabled = false;
            this._moveDownButton.Location = new System.Drawing.Point(708, 109);
            this._moveDownButton.Name = "_moveDownButton";
            this._moveDownButton.Size = new System.Drawing.Size(90, 23);
            this._moveDownButton.TabIndex = 95;
            this._moveDownButton.Text = "Move Down";
            this._moveDownButton.UseVisualStyleBackColor = true;
            this._moveDownButton.Click += new System.EventHandler(this._moveDownButton_Click);
            // 
            // _moveBottomButton
            // 
            this._moveBottomButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._moveBottomButton.Enabled = false;
            this._moveBottomButton.Location = new System.Drawing.Point(708, 138);
            this._moveBottomButton.Name = "_moveBottomButton";
            this._moveBottomButton.Size = new System.Drawing.Size(90, 23);
            this._moveBottomButton.TabIndex = 96;
            this._moveBottomButton.Text = "To Bottom";
            this._moveBottomButton.UseVisualStyleBackColor = true;
            this._moveBottomButton.Click += new System.EventHandler(this._moveBottomButton_Click);
            // 
            // _moveTopButton
            // 
            this._moveTopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._moveTopButton.Enabled = false;
            this._moveTopButton.Location = new System.Drawing.Point(708, 51);
            this._moveTopButton.Name = "_moveTopButton";
            this._moveTopButton.Size = new System.Drawing.Size(90, 23);
            this._moveTopButton.TabIndex = 97;
            this._moveTopButton.Text = "To Top";
            this._moveTopButton.UseVisualStyleBackColor = true;
            this._moveTopButton.Click += new System.EventHandler(this._moveTopButton_Click);
            // 
            // _deleteButton
            // 
            this._deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._deleteButton.Enabled = false;
            this._deleteButton.Location = new System.Drawing.Point(708, 312);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(90, 23);
            this._deleteButton.TabIndex = 101;
            this._deleteButton.Text = "Delete";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // _createNewButton
            // 
            this._createNewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._createNewButton.Location = new System.Drawing.Point(708, 196);
            this._createNewButton.Name = "_createNewButton";
            this._createNewButton.Size = new System.Drawing.Size(90, 23);
            this._createNewButton.TabIndex = 98;
            this._createNewButton.Text = "Create New";
            this._createNewButton.UseVisualStyleBackColor = true;
            this._createNewButton.Click += new System.EventHandler(this._createNewButton_Click);
            // 
            // _sortByLcnButton
            // 
            this._sortByLcnButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._sortByLcnButton.Location = new System.Drawing.Point(708, 369);
            this._sortByLcnButton.Name = "_sortByLcnButton";
            this._sortByLcnButton.Size = new System.Drawing.Size(90, 23);
            this._sortByLcnButton.TabIndex = 102;
            this._sortByLcnButton.Text = "Sort By LCN";
            this._sortByLcnButton.UseVisualStyleBackColor = true;
            this._sortByLcnButton.Click += new System.EventHandler(this._sortByLcnButton_Click);
            // 
            // _visibleOffButton
            // 
            this._visibleOffButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._visibleOffButton.Location = new System.Drawing.Point(708, 254);
            this._visibleOffButton.Name = "_visibleOffButton";
            this._visibleOffButton.Size = new System.Drawing.Size(90, 23);
            this._visibleOffButton.TabIndex = 100;
            this._visibleOffButton.Text = "Visible Off";
            this._visibleOffButton.UseVisualStyleBackColor = true;
            this._visibleOffButton.Click += new System.EventHandler(this._visibleOffButton_Click);
            // 
            // _visibleOnButton
            // 
            this._visibleOnButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._visibleOnButton.Location = new System.Drawing.Point(708, 225);
            this._visibleOnButton.Name = "_visibleOnButton";
            this._visibleOnButton.Size = new System.Drawing.Size(90, 23);
            this._visibleOnButton.TabIndex = 99;
            this._visibleOnButton.Text = "Visible On";
            this._visibleOnButton.UseVisualStyleBackColor = true;
            this._visibleOnButton.Click += new System.EventHandler(this._visibleOnButton_Click);
            // 
            // _channelGroupControl
            // 
            this._channelGroupControl.Location = new System.Drawing.Point(0, 0);
            this._channelGroupControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._channelGroupControl.Name = "_channelGroupControl";
            this._channelGroupControl.ShowAllChannelsOnTop = false;
            this._channelGroupControl.Size = new System.Drawing.Size(276, 21);
            this._channelGroupControl.TabIndex = 104;
            this._channelGroupControl.SelectedGroupChanged += new System.EventHandler(this._channelGroupControl_SelectedGroupChanged);
            // 
            // _editSelectedRowsButton
            // 
            this._editSelectedRowsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._editSelectedRowsButton.Location = new System.Drawing.Point(708, 283);
            this._editSelectedRowsButton.Name = "_editSelectedRowsButton";
            this._editSelectedRowsButton.Size = new System.Drawing.Size(87, 23);
            this._editSelectedRowsButton.TabIndex = 105;
            this._editSelectedRowsButton.Text = "Edit";
            this._editSelectedRowsButton.UseVisualStyleBackColor = true;
            this._editSelectedRowsButton.Click += new System.EventHandler(this.editSelectedRowsButton_Click);
            // 
            // ChannelsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._editSelectedRowsButton);
            this.Controls.Add(this._channelGroupControl);
            this.Controls.Add(this._visibleOnButton);
            this.Controls.Add(this._visibleOffButton);
            this.Controls.Add(this._sortByLcnButton);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._createNewButton);
            this.Controls.Add(this._moveTopButton);
            this.Controls.Add(this._moveBottomButton);
            this.Controls.Add(this._moveDownButton);
            this.Controls.Add(this._moveUpButton);
            this.Controls.Add(this._channelsDataGridView);
            this.MinimumSize = new System.Drawing.Size(596, 0);
            this.Name = "ChannelsPanel";
            this.Size = new System.Drawing.Size(798, 517);
            this.Load += new System.EventHandler(this.ChannelsPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._channelsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _channelsDataGridView;
        private System.Windows.Forms.BindingSource _channelsBindingSource;
        private System.Windows.Forms.Button _moveUpButton;
        private System.Windows.Forms.Button _moveDownButton;
        private System.Windows.Forms.Button _moveBottomButton;
        private System.Windows.Forms.Button _moveTopButton;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.Button _createNewButton;
        private System.Windows.Forms.Button _sortByLcnButton;
        private System.Windows.Forms.Button _visibleOffButton;
        private System.Windows.Forms.Button _visibleOnButton;
        private ArgusTV.WinForms.UserControls.ChannelGroupControl _channelGroupControl;
        private System.Windows.Forms.DataGridViewCheckBoxColumn _visibleColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _lcnColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _channelColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn _guideChannelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _preRecColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _postRecColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _startColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _stopColumn;
        private System.Windows.Forms.Button _editSelectedRowsButton;
    }
}
