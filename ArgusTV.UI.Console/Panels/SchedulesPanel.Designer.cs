namespace ArgusTV.UI.Console.Panels
{
    partial class SchedulesPanel
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
            this._createNewButton = new System.Windows.Forms.Button();
            this._schedulesDataGridView = new System.Windows.Forms.DataGridView();
            this.IsActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this._iconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.schedulePriorityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.preRecordSecondsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.postRecordSecondsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastModifiedTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._schedulesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._editButton = new System.Windows.Forms.Button();
            this._deleteButton = new System.Windows.Forms.Button();
            this._createManualScheduleButton = new System.Windows.Forms.Button();
            this._exportButton = new System.Windows.Forms.Button();
            this._importButton = new System.Windows.Forms.Button();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this._deleteObsoleteButton = new System.Windows.Forms.Button();
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._schedulesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._schedulesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _createNewButton
            // 
            this._createNewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._createNewButton.Location = new System.Drawing.Point(243, 638);
            this._createNewButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._createNewButton.Name = "_createNewButton";
            this._createNewButton.Size = new System.Drawing.Size(150, 35);
            this._createNewButton.TabIndex = 5;
            this._createNewButton.Text = "Create Schedule";
            this._createNewButton.UseVisualStyleBackColor = true;
            this._createNewButton.Click += new System.EventHandler(this._createNewButton_Click);
            // 
            // _schedulesDataGridView
            // 
            this._schedulesDataGridView.AllowUserToAddRows = false;
            this._schedulesDataGridView.AllowUserToDeleteRows = false;
            this._schedulesDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._schedulesDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._schedulesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._schedulesDataGridView.AutoGenerateColumns = false;
            this._schedulesDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._schedulesDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._schedulesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._schedulesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IsActive,
            this._iconColumn,
            this.nameDataGridViewTextBoxColumn,
            this.schedulePriorityDataGridViewTextBoxColumn,
            this.preRecordSecondsDataGridViewTextBoxColumn,
            this.postRecordSecondsDataGridViewTextBoxColumn,
            this.lastModifiedTimeDataGridViewTextBoxColumn});
            this._schedulesDataGridView.DataSource = this._schedulesBindingSource;
            this._schedulesDataGridView.Location = new System.Drawing.Point(0, 42);
            this._schedulesDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._schedulesDataGridView.Name = "_schedulesDataGridView";
            this._schedulesDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._schedulesDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._schedulesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._schedulesDataGridView.Size = new System.Drawing.Size(1041, 588);
            this._schedulesDataGridView.StandardTab = true;
            this._schedulesDataGridView.TabIndex = 2;
            this._schedulesDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this._schedulesDataGridView_CellFormatting);
            this._schedulesDataGridView.CurrentCellDirtyStateChanged += new System.EventHandler(this._schedulesDataGridView_CurrentCellDirtyStateChanged);
            this._schedulesDataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this._schedulesDataGridView_DataBindingComplete);
            this._schedulesDataGridView.SelectionChanged += new System.EventHandler(this._schedulesDataGridView_SelectionChanged);
            this._schedulesDataGridView.DoubleClick += new System.EventHandler(this._schedulesDataGridView_DoubleClick);
            // 
            // IsActive
            // 
            this.IsActive.DataPropertyName = "IsActive";
            this.IsActive.HeaderText = "Active";
            this.IsActive.Name = "IsActive";
            this.IsActive.Width = 60;
            // 
            // _iconColumn
            // 
            this._iconColumn.HeaderText = "";
            this._iconColumn.Name = "_iconColumn";
            this._iconColumn.ReadOnly = true;
            this._iconColumn.Width = 26;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // schedulePriorityDataGridViewTextBoxColumn
            // 
            this.schedulePriorityDataGridViewTextBoxColumn.DataPropertyName = "SchedulePriority";
            this.schedulePriorityDataGridViewTextBoxColumn.HeaderText = "Priority";
            this.schedulePriorityDataGridViewTextBoxColumn.Name = "schedulePriorityDataGridViewTextBoxColumn";
            this.schedulePriorityDataGridViewTextBoxColumn.ReadOnly = true;
            this.schedulePriorityDataGridViewTextBoxColumn.Width = 80;
            // 
            // preRecordSecondsDataGridViewTextBoxColumn
            // 
            this.preRecordSecondsDataGridViewTextBoxColumn.DataPropertyName = "PreRecordMinutes";
            this.preRecordSecondsDataGridViewTextBoxColumn.HeaderText = "Pre-record";
            this.preRecordSecondsDataGridViewTextBoxColumn.Name = "preRecordSecondsDataGridViewTextBoxColumn";
            this.preRecordSecondsDataGridViewTextBoxColumn.ReadOnly = true;
            this.preRecordSecondsDataGridViewTextBoxColumn.ToolTipText = "Pre-recording time (minutes)";
            this.preRecordSecondsDataGridViewTextBoxColumn.Width = 80;
            // 
            // postRecordSecondsDataGridViewTextBoxColumn
            // 
            this.postRecordSecondsDataGridViewTextBoxColumn.DataPropertyName = "PostRecordMinutes";
            this.postRecordSecondsDataGridViewTextBoxColumn.HeaderText = "Post-record";
            this.postRecordSecondsDataGridViewTextBoxColumn.Name = "postRecordSecondsDataGridViewTextBoxColumn";
            this.postRecordSecondsDataGridViewTextBoxColumn.ReadOnly = true;
            this.postRecordSecondsDataGridViewTextBoxColumn.ToolTipText = "Post-recording time (minutes)";
            this.postRecordSecondsDataGridViewTextBoxColumn.Width = 80;
            // 
            // lastModifiedTimeDataGridViewTextBoxColumn
            // 
            this.lastModifiedTimeDataGridViewTextBoxColumn.DataPropertyName = "LastModifiedTime";
            this.lastModifiedTimeDataGridViewTextBoxColumn.HeaderText = "Last Modified";
            this.lastModifiedTimeDataGridViewTextBoxColumn.Name = "lastModifiedTimeDataGridViewTextBoxColumn";
            this.lastModifiedTimeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // _schedulesBindingSource
            // 
            this._schedulesBindingSource.DataSource = typeof(ArgusTV.UI.Process.SortableBindingList<ArgusTV.DataContracts.ScheduleSummary>);
            // 
            // _editButton
            // 
            this._editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._editButton.Location = new System.Drawing.Point(0, 638);
            this._editButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._editButton.Name = "_editButton";
            this._editButton.Size = new System.Drawing.Size(112, 35);
            this._editButton.TabIndex = 3;
            this._editButton.Text = "Edit";
            this._editButton.UseVisualStyleBackColor = true;
            this._editButton.Click += new System.EventHandler(this._editButton_Click);
            // 
            // _deleteButton
            // 
            this._deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._deleteButton.Location = new System.Drawing.Point(122, 638);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(112, 35);
            this._deleteButton.TabIndex = 4;
            this._deleteButton.Text = "Delete";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // _createManualScheduleButton
            // 
            this._createManualScheduleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._createManualScheduleButton.Location = new System.Drawing.Point(402, 638);
            this._createManualScheduleButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._createManualScheduleButton.Name = "_createManualScheduleButton";
            this._createManualScheduleButton.Size = new System.Drawing.Size(150, 35);
            this._createManualScheduleButton.TabIndex = 6;
            this._createManualScheduleButton.Text = "Create Manual";
            this._createManualScheduleButton.UseVisualStyleBackColor = true;
            this._createManualScheduleButton.Click += new System.EventHandler(this._createManualScheduleButton_Click);
            // 
            // _exportButton
            // 
            this._exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._exportButton.Location = new System.Drawing.Point(928, 638);
            this._exportButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._exportButton.Name = "_exportButton";
            this._exportButton.Size = new System.Drawing.Size(112, 35);
            this._exportButton.TabIndex = 7;
            this._exportButton.Text = "Export...";
            this._exportButton.UseVisualStyleBackColor = true;
            this._exportButton.Click += new System.EventHandler(this._exportButton_Click);
            // 
            // _importButton
            // 
            this._importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._importButton.Location = new System.Drawing.Point(807, 638);
            this._importButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._importButton.Name = "_importButton";
            this._importButton.Size = new System.Drawing.Size(112, 35);
            this._importButton.TabIndex = 8;
            this._importButton.Text = "Import...";
            this._importButton.UseVisualStyleBackColor = true;
            this._importButton.Click += new System.EventHandler(this._importButton_Click);
            // 
            // _openFileDialog
            // 
            this._openFileDialog.DefaultExt = "xml";
            this._openFileDialog.Filter = "XML Files|*.xml|All Files|*.*";
            this._openFileDialog.Title = "Import Schedules From";
            // 
            // _saveFileDialog
            // 
            this._saveFileDialog.DefaultExt = "xml";
            this._saveFileDialog.Filter = "XML Files|*.xml|All Files|*.*";
            this._saveFileDialog.Title = "Export Schedules To";
            // 
            // _deleteObsoleteButton
            // 
            this._deleteObsoleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._deleteObsoleteButton.Location = new System.Drawing.Point(686, 638);
            this._deleteObsoleteButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._deleteObsoleteButton.Name = "_deleteObsoleteButton";
            this._deleteObsoleteButton.Size = new System.Drawing.Size(112, 35);
            this._deleteObsoleteButton.TabIndex = 9;
            this._deleteObsoleteButton.Text = "Cleanup Old";
            this._deleteObsoleteButton.UseVisualStyleBackColor = true;
            this._deleteObsoleteButton.Click += new System.EventHandler(this._deleteObsoleteButton_Click);
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television Schedules",
            "Radio Schedules"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(0, 0);
            this._channelTypeComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(373, 28);
            this._channelTypeComboBox.TabIndex = 1;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // SchedulesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._channelTypeComboBox);
            this.Controls.Add(this._deleteObsoleteButton);
            this.Controls.Add(this._importButton);
            this.Controls.Add(this._exportButton);
            this.Controls.Add(this._createManualScheduleButton);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._editButton);
            this.Controls.Add(this._schedulesDataGridView);
            this.Controls.Add(this._createNewButton);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SchedulesPanel";
            this.Size = new System.Drawing.Size(1041, 674);
            this.Load += new System.EventHandler(this.SchedulesPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._schedulesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._schedulesBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _createNewButton;
        private System.Windows.Forms.DataGridView _schedulesDataGridView;
        private System.Windows.Forms.BindingSource _schedulesBindingSource;
        private System.Windows.Forms.Button _editButton;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.Button _createManualScheduleButton;
        private System.Windows.Forms.Button _exportButton;
        private System.Windows.Forms.Button _importButton;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.SaveFileDialog _saveFileDialog;
        private System.Windows.Forms.Button _deleteObsoleteButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsActive;
        private System.Windows.Forms.DataGridViewImageColumn _iconColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn schedulePriorityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn preRecordSecondsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn postRecordSecondsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastModifiedTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.ComboBox _channelTypeComboBox;
    }
}
