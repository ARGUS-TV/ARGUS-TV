namespace ArgusTV.UI.Console.UserControls
{
    partial class TvChannelProgramsControl
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
            this._programsDataGridView = new System.Windows.Forms.DataGridView();
            this._iconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this._channelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._startTimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._titleLinkColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this._categoryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._videoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._ratingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._starRatingDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._isRepeatDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this._isPremiereDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this._programsListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._programsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._programsListBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _programsDataGridView
            // 
            this._programsDataGridView.AllowUserToAddRows = false;
            this._programsDataGridView.AllowUserToDeleteRows = false;
            this._programsDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._programsDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._programsDataGridView.AutoGenerateColumns = false;
            this._programsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._programsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._programsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._programsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._iconColumn,
            this._channelColumn,
            this._startTimeDataGridViewTextBoxColumn,
            this._titleLinkColumn,
            this._categoryDataGridViewTextBoxColumn,
            this._videoDataGridViewTextBoxColumn,
            this._ratingDataGridViewTextBoxColumn,
            this._starRatingDataGridViewTextBoxColumn,
            this._isRepeatDataGridViewCheckBoxColumn,
            this._isPremiereDataGridViewCheckBoxColumn});
            this._programsDataGridView.DataSource = this._programsListBindingSource;
            this._programsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._programsDataGridView.GridColor = System.Drawing.Color.White;
            this._programsDataGridView.Location = new System.Drawing.Point(0, 0);
            this._programsDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._programsDataGridView.MultiSelect = false;
            this._programsDataGridView.Name = "_programsDataGridView";
            this._programsDataGridView.ReadOnly = true;
            this._programsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this._programsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this._programsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._programsDataGridView.Size = new System.Drawing.Size(1226, 603);
            this._programsDataGridView.StandardTab = true;
            this._programsDataGridView.TabIndex = 0;
            this._programsDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._programsDataGridView_CellContentClick);
            this._programsDataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this._programsDataGridView_DataBindingComplete);
            // 
            // _iconColumn
            // 
            this._iconColumn.HeaderText = "";
            this._iconColumn.Name = "_iconColumn";
            this._iconColumn.ReadOnly = true;
            this._iconColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._iconColumn.Width = 26;
            // 
            // _channelColumn
            // 
            this._channelColumn.DataPropertyName = "ChannelName";
            this._channelColumn.HeaderText = "Channel";
            this._channelColumn.Name = "_channelColumn";
            this._channelColumn.ReadOnly = true;
            this._channelColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _startTimeDataGridViewTextBoxColumn
            // 
            this._startTimeDataGridViewTextBoxColumn.DataPropertyName = "ProgramTimes";
            dataGridViewCellStyle2.Format = "g";
            dataGridViewCellStyle2.NullValue = null;
            this._startTimeDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this._startTimeDataGridViewTextBoxColumn.HeaderText = "Date/Time";
            this._startTimeDataGridViewTextBoxColumn.Name = "_startTimeDataGridViewTextBoxColumn";
            this._startTimeDataGridViewTextBoxColumn.ReadOnly = true;
            this._startTimeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._startTimeDataGridViewTextBoxColumn.Width = 200;
            // 
            // _titleLinkColumn
            // 
            this._titleLinkColumn.ActiveLinkColor = System.Drawing.Color.DarkRed;
            this._titleLinkColumn.DataPropertyName = "ProgramTitle";
            this._titleLinkColumn.HeaderText = "Title";
            this._titleLinkColumn.Name = "_titleLinkColumn";
            this._titleLinkColumn.ReadOnly = true;
            this._titleLinkColumn.Width = 270;
            // 
            // _categoryDataGridViewTextBoxColumn
            // 
            this._categoryDataGridViewTextBoxColumn.DataPropertyName = "Category";
            this._categoryDataGridViewTextBoxColumn.HeaderText = "Category";
            this._categoryDataGridViewTextBoxColumn.Name = "_categoryDataGridViewTextBoxColumn";
            this._categoryDataGridViewTextBoxColumn.ReadOnly = true;
            this._categoryDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _videoDataGridViewTextBoxColumn
            // 
            this._videoDataGridViewTextBoxColumn.DataPropertyName = "VideoFlags";
            this._videoDataGridViewTextBoxColumn.HeaderText = "Video";
            this._videoDataGridViewTextBoxColumn.Name = "_videoDataGridViewTextBoxColumn";
            this._videoDataGridViewTextBoxColumn.ReadOnly = true;
            this._videoDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._videoDataGridViewTextBoxColumn.Width = 50;
            // 
            // _ratingDataGridViewTextBoxColumn
            // 
            this._ratingDataGridViewTextBoxColumn.DataPropertyName = "Rating";
            this._ratingDataGridViewTextBoxColumn.HeaderText = "Rating";
            this._ratingDataGridViewTextBoxColumn.Name = "_ratingDataGridViewTextBoxColumn";
            this._ratingDataGridViewTextBoxColumn.ReadOnly = true;
            this._ratingDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._ratingDataGridViewTextBoxColumn.Width = 60;
            // 
            // _starRatingDataGridViewTextBoxColumn
            // 
            this._starRatingDataGridViewTextBoxColumn.DataPropertyName = "StarRating";
            this._starRatingDataGridViewTextBoxColumn.HeaderText = "Stars";
            this._starRatingDataGridViewTextBoxColumn.Name = "_starRatingDataGridViewTextBoxColumn";
            this._starRatingDataGridViewTextBoxColumn.ReadOnly = true;
            this._starRatingDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this._starRatingDataGridViewTextBoxColumn.Width = 60;
            // 
            // _isRepeatDataGridViewCheckBoxColumn
            // 
            this._isRepeatDataGridViewCheckBoxColumn.DataPropertyName = "IsRepeat";
            this._isRepeatDataGridViewCheckBoxColumn.HeaderText = "Repeat";
            this._isRepeatDataGridViewCheckBoxColumn.Name = "_isRepeatDataGridViewCheckBoxColumn";
            this._isRepeatDataGridViewCheckBoxColumn.ReadOnly = true;
            this._isRepeatDataGridViewCheckBoxColumn.Width = 50;
            // 
            // _isPremiereDataGridViewCheckBoxColumn
            // 
            this._isPremiereDataGridViewCheckBoxColumn.DataPropertyName = "IsPremiere";
            this._isPremiereDataGridViewCheckBoxColumn.HeaderText = "Premiere";
            this._isPremiereDataGridViewCheckBoxColumn.Name = "_isPremiereDataGridViewCheckBoxColumn";
            this._isPremiereDataGridViewCheckBoxColumn.ReadOnly = true;
            this._isPremiereDataGridViewCheckBoxColumn.Width = 50;
            // 
            // _programsListBindingSource
            // 
            this._programsListBindingSource.DataSource = typeof(ArgusTV.UI.Process.ChannelProgramsList);
            // 
            // TvChannelProgramsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._programsDataGridView);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "TvChannelProgramsControl";
            this.Size = new System.Drawing.Size(1226, 603);
            ((System.ComponentModel.ISupportInitialize)(this._programsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._programsListBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _programsDataGridView;
        private System.Windows.Forms.BindingSource _programsListBindingSource;
        private System.Windows.Forms.DataGridViewImageColumn _iconColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _channelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _startTimeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewLinkColumn _titleLinkColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _categoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _videoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _ratingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _starRatingDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn _isRepeatDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn _isPremiereDataGridViewCheckBoxColumn;
    }
}
