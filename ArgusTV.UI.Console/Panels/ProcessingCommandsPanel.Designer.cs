namespace ArgusTV.UI.Console.Panels
{
    partial class ProcessingCommandsPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessingCommandsPanel));
            this._commandsDataGridView = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._commandsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._deleteButton = new System.Windows.Forms.Button();
            this._createNewButton = new System.Windows.Forms.Button();
            this._commandGroupBox = new System.Windows.Forms.GroupBox();
            this._isDefaultRadioCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this._isDefaultTelevisionCheckBox = new System.Windows.Forms.CheckBox();
            this._argsInfoLabel = new System.Windows.Forms.Label();
            this._runLabel = new System.Windows.Forms.Label();
            this._browseButton = new System.Windows.Forms.Button();
            this._runAtTimePicker = new System.Windows.Forms.DateTimePicker();
            this._runModeComboBox = new System.Windows.Forms.ComboBox();
            this._argumentsTextBox = new System.Windows.Forms.TextBox();
            this._argsLabel = new System.Windows.Forms.Label();
            this._commandTextBox = new System.Windows.Forms.TextBox();
            this._commandLabel = new System.Windows.Forms.Label();
            this._nameTextBox = new System.Windows.Forms.TextBox();
            this._nameLabel = new System.Windows.Forms.Label();
            this._openCommandDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this._commandsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._commandsBindingSource)).BeginInit();
            this._commandGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _commandsDataGridView
            // 
            this._commandsDataGridView.AllowUserToAddRows = false;
            this._commandsDataGridView.AllowUserToDeleteRows = false;
            this._commandsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._commandsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._commandsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._commandsDataGridView.AutoGenerateColumns = false;
            this._commandsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._commandsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._commandsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._commandsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn});
            this._commandsDataGridView.DataSource = this._commandsBindingSource;
            this._commandsDataGridView.GridColor = System.Drawing.Color.White;
            this._commandsDataGridView.Location = new System.Drawing.Point(0, 0);
            this._commandsDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._commandsDataGridView.MultiSelect = false;
            this._commandsDataGridView.Name = "_commandsDataGridView";
            this._commandsDataGridView.ReadOnly = true;
            this._commandsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._commandsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._commandsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._commandsDataGridView.Size = new System.Drawing.Size(1020, 394);
            this._commandsDataGridView.TabIndex = 0;
            this._commandsDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this._commandsDataGridView_DataError);
            this._commandsDataGridView.SelectionChanged += new System.EventHandler(this._commandsDataGridView_SelectionChanged);
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _commandsBindingSource
            // 
            this._commandsBindingSource.DataSource = typeof(ArgusTV.UI.Process.SortableBindingList<ArgusTV.DataContracts.ProcessingCommand>);
            this._commandsBindingSource.Sort = "";
            // 
            // _deleteButton
            // 
            this._deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._deleteButton.Enabled = false;
            this._deleteButton.Location = new System.Drawing.Point(1029, 82);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(135, 35);
            this._deleteButton.TabIndex = 21;
            this._deleteButton.Text = "Delete";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // _createNewButton
            // 
            this._createNewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._createNewButton.Location = new System.Drawing.Point(1029, 37);
            this._createNewButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._createNewButton.Name = "_createNewButton";
            this._createNewButton.Size = new System.Drawing.Size(135, 35);
            this._createNewButton.TabIndex = 20;
            this._createNewButton.Text = "Create New";
            this._createNewButton.UseVisualStyleBackColor = true;
            this._createNewButton.Click += new System.EventHandler(this._createNewButton_Click);
            // 
            // _commandGroupBox
            // 
            this._commandGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._commandGroupBox.Controls.Add(this._isDefaultRadioCheckBox);
            this._commandGroupBox.Controls.Add(this.label1);
            this._commandGroupBox.Controls.Add(this._isDefaultTelevisionCheckBox);
            this._commandGroupBox.Controls.Add(this._argsInfoLabel);
            this._commandGroupBox.Controls.Add(this._runLabel);
            this._commandGroupBox.Controls.Add(this._browseButton);
            this._commandGroupBox.Controls.Add(this._runAtTimePicker);
            this._commandGroupBox.Controls.Add(this._runModeComboBox);
            this._commandGroupBox.Controls.Add(this._argumentsTextBox);
            this._commandGroupBox.Controls.Add(this._argsLabel);
            this._commandGroupBox.Controls.Add(this._commandTextBox);
            this._commandGroupBox.Controls.Add(this._commandLabel);
            this._commandGroupBox.Controls.Add(this._nameTextBox);
            this._commandGroupBox.Controls.Add(this._nameLabel);
            this._commandGroupBox.Location = new System.Drawing.Point(0, 398);
            this._commandGroupBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._commandGroupBox.Name = "_commandGroupBox";
            this._commandGroupBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._commandGroupBox.Size = new System.Drawing.Size(1020, 358);
            this._commandGroupBox.TabIndex = 30;
            this._commandGroupBox.TabStop = false;
            this._commandGroupBox.Text = "Processing Command";
            // 
            // _isDefaultRadioCheckBox
            // 
            this._isDefaultRadioCheckBox.AutoSize = true;
            this._isDefaultRadioCheckBox.Location = new System.Drawing.Point(112, 315);
            this._isDefaultRadioCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._isDefaultRadioCheckBox.Name = "_isDefaultRadioCheckBox";
            this._isDefaultRadioCheckBox.Size = new System.Drawing.Size(364, 24);
            this._isDefaultRadioCheckBox.TabIndex = 14;
            this._isDefaultRadioCheckBox.Text = "Automatically assign to all new radio schedules";
            this._isDefaultRadioCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DimGray;
            this.label1.Location = new System.Drawing.Point(108, 108);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(706, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "Note: this path needs to be a local path on the machine where your core services " +
    "are running.";
            // 
            // _isDefaultTelevisionCheckBox
            // 
            this._isDefaultTelevisionCheckBox.AutoSize = true;
            this._isDefaultTelevisionCheckBox.Location = new System.Drawing.Point(112, 280);
            this._isDefaultTelevisionCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._isDefaultTelevisionCheckBox.Name = "_isDefaultTelevisionCheckBox";
            this._isDefaultTelevisionCheckBox.Size = new System.Drawing.Size(394, 24);
            this._isDefaultTelevisionCheckBox.TabIndex = 12;
            this._isDefaultTelevisionCheckBox.Text = "Automatically assign to all new television schedules";
            this._isDefaultTelevisionCheckBox.UseVisualStyleBackColor = true;
            // 
            // _argsInfoLabel
            // 
            this._argsInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._argsInfoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._argsInfoLabel.ForeColor = System.Drawing.Color.DimGray;
            this._argsInfoLabel.Location = new System.Drawing.Point(106, 178);
            this._argsInfoLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._argsInfoLabel.Name = "_argsInfoLabel";
            this._argsInfoLabel.Size = new System.Drawing.Size(904, 40);
            this._argsInfoLabel.TabIndex = 7;
            this._argsInfoLabel.Text = resources.GetString("_argsInfoLabel.Text");
            // 
            // _runLabel
            // 
            this._runLabel.AutoSize = true;
            this._runLabel.Location = new System.Drawing.Point(12, 237);
            this._runLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._runLabel.Name = "_runLabel";
            this._runLabel.Size = new System.Drawing.Size(55, 20);
            this._runLabel.TabIndex = 8;
            this._runLabel.Text = "When:";
            // 
            // _browseButton
            // 
            this._browseButton.Location = new System.Drawing.Point(621, 66);
            this._browseButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._browseButton.Name = "_browseButton";
            this._browseButton.Size = new System.Drawing.Size(112, 35);
            this._browseButton.TabIndex = 4;
            this._browseButton.Text = "Browse...";
            this._browseButton.UseVisualStyleBackColor = true;
            this._browseButton.Click += new System.EventHandler(this._browseButton_Click);
            // 
            // _runAtTimePicker
            // 
            this._runAtTimePicker.CustomFormat = "HH:mm";
            this._runAtTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this._runAtTimePicker.Location = new System.Drawing.Point(498, 234);
            this._runAtTimePicker.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._runAtTimePicker.Name = "_runAtTimePicker";
            this._runAtTimePicker.ShowUpDown = true;
            this._runAtTimePicker.Size = new System.Drawing.Size(112, 26);
            this._runAtTimePicker.TabIndex = 10;
            // 
            // _runModeComboBox
            // 
            this._runModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._runModeComboBox.FormattingEnabled = true;
            this._runModeComboBox.Items.AddRange(new object[] {
            "Run when the recording starts (live)",
            "Run when the recording ends",
            "Run at a fixed time after the recording ends",
            "Run before the recording is deleted"});
            this._runModeComboBox.Location = new System.Drawing.Point(112, 232);
            this._runModeComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._runModeComboBox.Name = "_runModeComboBox";
            this._runModeComboBox.Size = new System.Drawing.Size(376, 28);
            this._runModeComboBox.TabIndex = 9;
            this._runModeComboBox.SelectedIndexChanged += new System.EventHandler(this._runModeComboBox_SelectedIndexChanged);
            // 
            // _argumentsTextBox
            // 
            this._argumentsTextBox.Location = new System.Drawing.Point(111, 143);
            this._argumentsTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._argumentsTextBox.Name = "_argumentsTextBox";
            this._argumentsTextBox.Size = new System.Drawing.Size(499, 26);
            this._argumentsTextBox.TabIndex = 6;
            // 
            // _argsLabel
            // 
            this._argsLabel.AutoSize = true;
            this._argsLabel.Location = new System.Drawing.Point(12, 148);
            this._argsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._argsLabel.Name = "_argsLabel";
            this._argsLabel.Size = new System.Drawing.Size(91, 20);
            this._argsLabel.TabIndex = 5;
            this._argsLabel.Text = "Arguments:";
            // 
            // _commandTextBox
            // 
            this._commandTextBox.Location = new System.Drawing.Point(111, 69);
            this._commandTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._commandTextBox.Name = "_commandTextBox";
            this._commandTextBox.Size = new System.Drawing.Size(499, 26);
            this._commandTextBox.TabIndex = 3;
            // 
            // _commandLabel
            // 
            this._commandLabel.AutoSize = true;
            this._commandLabel.Location = new System.Drawing.Point(12, 74);
            this._commandLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._commandLabel.Name = "_commandLabel";
            this._commandLabel.Size = new System.Drawing.Size(46, 20);
            this._commandLabel.TabIndex = 2;
            this._commandLabel.Text = "Path:";
            // 
            // _nameTextBox
            // 
            this._nameTextBox.Location = new System.Drawing.Point(111, 29);
            this._nameTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new System.Drawing.Size(499, 26);
            this._nameTextBox.TabIndex = 1;
            // 
            // _nameLabel
            // 
            this._nameLabel.AutoSize = true;
            this._nameLabel.Location = new System.Drawing.Point(12, 34);
            this._nameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new System.Drawing.Size(55, 20);
            this._nameLabel.TabIndex = 0;
            this._nameLabel.Text = "Name:";
            // 
            // _openCommandDialog
            // 
            this._openCommandDialog.Filter = "Commands (*.exe;*.cmd;*.bat)|*.exe;*.cmd;*.bat";
            this._openCommandDialog.RestoreDirectory = true;
            // 
            // ProcessingCommandsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._commandsDataGridView);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._createNewButton);
            this.Controls.Add(this._commandGroupBox);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(894, 0);
            this.Name = "ProcessingCommandsPanel";
            this.Size = new System.Drawing.Size(1164, 762);
            this.Load += new System.EventHandler(this.ProcessingCommandsPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._commandsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._commandsBindingSource)).EndInit();
            this._commandGroupBox.ResumeLayout(false);
            this._commandGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _commandsDataGridView;
        private System.Windows.Forms.BindingSource _commandsBindingSource;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.Button _createNewButton;
        private System.Windows.Forms.GroupBox _commandGroupBox;
        private System.Windows.Forms.TextBox _nameTextBox;
        private System.Windows.Forms.Label _nameLabel;
        private System.Windows.Forms.TextBox _commandTextBox;
        private System.Windows.Forms.Label _commandLabel;
        private System.Windows.Forms.ComboBox _runModeComboBox;
        private System.Windows.Forms.TextBox _argumentsTextBox;
        private System.Windows.Forms.Label _argsLabel;
        private System.Windows.Forms.DateTimePicker _runAtTimePicker;
        private System.Windows.Forms.Label _runLabel;
        private System.Windows.Forms.Button _browseButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label _argsInfoLabel;
        private System.Windows.Forms.OpenFileDialog _openCommandDialog;
        private System.Windows.Forms.CheckBox _isDefaultTelevisionCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox _isDefaultRadioCheckBox;
    }
}
