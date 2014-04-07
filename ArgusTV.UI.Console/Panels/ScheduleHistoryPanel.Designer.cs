namespace ArgusTV.UI.Console.Panels
{
    partial class ScheduleHistoryPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._schedulesDataGridView = new System.Windows.Forms.DataGridView();
            this._titleBoundColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._episodeBoundColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._recordedOnBoundColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._historyBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._deleteButton = new System.Windows.Forms.Button();
            this._clearHistoryButton = new System.Windows.Forms.Button();
            this._nameTextBox = new System.Windows.Forms.TextBox();
            this._nameLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._schedulesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._historyBindingSource)).BeginInit();
            this.SuspendLayout();
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
            this._titleBoundColumn,
            this._episodeBoundColumn,
            this._recordedOnBoundColumn});
            this._schedulesDataGridView.DataSource = this._historyBindingSource;
            this._schedulesDataGridView.Location = new System.Drawing.Point(0, 42);
            this._schedulesDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._schedulesDataGridView.MultiSelect = false;
            this._schedulesDataGridView.Name = "_schedulesDataGridView";
            this._schedulesDataGridView.ReadOnly = true;
            this._schedulesDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this._schedulesDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this._schedulesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._schedulesDataGridView.Size = new System.Drawing.Size(1041, 588);
            this._schedulesDataGridView.StandardTab = true;
            this._schedulesDataGridView.TabIndex = 2;
            // 
            // _titleBoundColumn
            // 
            this._titleBoundColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._titleBoundColumn.DataPropertyName = "Title";
            this._titleBoundColumn.HeaderText = "Title";
            this._titleBoundColumn.Name = "_titleBoundColumn";
            this._titleBoundColumn.ReadOnly = true;
            // 
            // _episodeBoundColumn
            // 
            this._episodeBoundColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._episodeBoundColumn.DataPropertyName = "Episode";
            this._episodeBoundColumn.HeaderText = "Episode";
            this._episodeBoundColumn.Name = "_episodeBoundColumn";
            this._episodeBoundColumn.ReadOnly = true;
            // 
            // _recordedOnBoundColumn
            // 
            this._recordedOnBoundColumn.DataPropertyName = "RecordedOn";
            dataGridViewCellStyle2.Format = "g";
            this._recordedOnBoundColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this._recordedOnBoundColumn.HeaderText = "Recorded On";
            this._recordedOnBoundColumn.Name = "_recordedOnBoundColumn";
            this._recordedOnBoundColumn.ReadOnly = true;
            // 
            // _historyBindingSource
            // 
            this._historyBindingSource.DataSource = typeof(ArgusTV.UI.Process.SortableBindingList<ArgusTV.DataContracts.ScheduleRecordedProgram>);
            // 
            // _deleteButton
            // 
            this._deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._deleteButton.Location = new System.Drawing.Point(0, 638);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(135, 35);
            this._deleteButton.TabIndex = 4;
            this._deleteButton.Text = "Delete";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // _clearHistoryButton
            // 
            this._clearHistoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._clearHistoryButton.Location = new System.Drawing.Point(144, 638);
            this._clearHistoryButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._clearHistoryButton.Name = "_clearHistoryButton";
            this._clearHistoryButton.Size = new System.Drawing.Size(135, 35);
            this._clearHistoryButton.TabIndex = 6;
            this._clearHistoryButton.Text = "Clear History";
            this._clearHistoryButton.UseVisualStyleBackColor = true;
            this._clearHistoryButton.Click += new System.EventHandler(this._clearHistoryButton_Click);
            // 
            // _nameTextBox
            // 
            this._nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._nameTextBox.Location = new System.Drawing.Point(88, 2);
            this._nameTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.ReadOnly = true;
            this._nameTextBox.Size = new System.Drawing.Size(950, 26);
            this._nameTextBox.TabIndex = 8;
            // 
            // _nameLabel
            // 
            this._nameLabel.AutoSize = true;
            this._nameLabel.Location = new System.Drawing.Point(-3, 6);
            this._nameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new System.Drawing.Size(80, 20);
            this._nameLabel.TabIndex = 7;
            this._nameLabel.Text = "Schedule:";
            // 
            // ScheduleHistoryPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._nameTextBox);
            this.Controls.Add(this._nameLabel);
            this.Controls.Add(this._clearHistoryButton);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._schedulesDataGridView);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ScheduleHistoryPanel";
            this.Size = new System.Drawing.Size(1041, 674);
            this.Load += new System.EventHandler(this.ScheduleHistoryPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._schedulesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._historyBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _schedulesDataGridView;
        private System.Windows.Forms.BindingSource _historyBindingSource;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.Button _clearHistoryButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn _titleBoundColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _episodeBoundColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _recordedOnBoundColumn;
        private System.Windows.Forms.TextBox _nameTextBox;
        private System.Windows.Forms.Label _nameLabel;
    }
}
