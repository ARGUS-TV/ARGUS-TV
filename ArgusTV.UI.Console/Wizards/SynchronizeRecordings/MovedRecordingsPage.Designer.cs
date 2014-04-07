namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    partial class MovedRecordingsPage
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
            this._recordingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._recordingsDataGridView = new System.Windows.Forms.DataGridView();
            this._channelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._titleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._episodeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._recordingFileColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._recordingsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._recordingsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _recordingsDataGridView
            // 
            this._recordingsDataGridView.AllowUserToAddRows = false;
            this._recordingsDataGridView.AllowUserToDeleteRows = false;
            this._recordingsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._recordingsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._recordingsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._recordingsDataGridView.AutoGenerateColumns = false;
            this._recordingsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._recordingsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._recordingsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._recordingsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._channelColumn,
            this._titleColumn,
            this._episodeColumn,
            this._recordingFileColumn});
            this._recordingsDataGridView.DataSource = this._recordingsBindingSource;
            this._recordingsDataGridView.GridColor = System.Drawing.Color.White;
            this._recordingsDataGridView.Location = new System.Drawing.Point(0, 0);
            this._recordingsDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._recordingsDataGridView.Name = "_recordingsDataGridView";
            this._recordingsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._recordingsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._recordingsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._recordingsDataGridView.Size = new System.Drawing.Size(896, 482);
            this._recordingsDataGridView.TabIndex = 13;
            // 
            // _channelColumn
            // 
            this._channelColumn.DataPropertyName = "ChannelDisplayName";
            this._channelColumn.HeaderText = "Channel";
            this._channelColumn.Name = "_channelColumn";
            this._channelColumn.ReadOnly = true;
            // 
            // _titleColumn
            // 
            this._titleColumn.DataPropertyName = "Title";
            this._titleColumn.HeaderText = "Title";
            this._titleColumn.Name = "_titleColumn";
            this._titleColumn.ReadOnly = true;
            this._titleColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._titleColumn.Width = 200;
            // 
            // _episodeColumn
            // 
            this._episodeColumn.DataPropertyName = "SubTitle";
            this._episodeColumn.HeaderText = "Episode";
            this._episodeColumn.Name = "_episodeColumn";
            this._episodeColumn.ReadOnly = true;
            this._episodeColumn.Width = 200;
            // 
            // _recordingFileColumn
            // 
            this._recordingFileColumn.DataPropertyName = "RecordingFileName";
            this._recordingFileColumn.HeaderText = "File";
            this._recordingFileColumn.Name = "_recordingFileColumn";
            this._recordingFileColumn.ReadOnly = true;
            this._recordingFileColumn.Width = 300;
            // 
            // MovedRecordingsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._recordingsDataGridView);
            this.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.Name = "MovedRecordingsPage";
            this.Size = new System.Drawing.Size(896, 482);
            ((System.ComponentModel.ISupportInitialize)(this._recordingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._recordingsDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.BindingSource _recordingsBindingSource;
        private System.Windows.Forms.DataGridView _recordingsDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn _channelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _titleColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _episodeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _recordingFileColumn;
    }
}
