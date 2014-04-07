namespace ArgusTV.UI.Console.Panels
{
    partial class GuideChannelsPanel
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
            this._nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._xmlTvIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._channelsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._deleteButton = new System.Windows.Forms.Button();
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
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
            this._channelsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._channelsDataGridView.AutoGenerateColumns = false;
            this._channelsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._channelsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._channelsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._channelsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._nameDataGridViewTextBoxColumn,
            this._xmlTvIdDataGridViewTextBoxColumn});
            this._channelsDataGridView.DataSource = this._channelsBindingSource;
            this._channelsDataGridView.GridColor = System.Drawing.Color.White;
            this._channelsDataGridView.Location = new System.Drawing.Point(0, 42);
            this._channelsDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._channelsDataGridView.Name = "_channelsDataGridView";
            this._channelsDataGridView.ReadOnly = true;
            this._channelsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._channelsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._channelsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._channelsDataGridView.Size = new System.Drawing.Size(750, 754);
            this._channelsDataGridView.TabIndex = 2;
            this._channelsDataGridView.SelectionChanged += new System.EventHandler(this._channelsDataGridView_SelectionChanged);
            // 
            // _nameDataGridViewTextBoxColumn
            // 
            this._nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this._nameDataGridViewTextBoxColumn.FillWeight = 60F;
            this._nameDataGridViewTextBoxColumn.HeaderText = "Guide Channel  Name";
            this._nameDataGridViewTextBoxColumn.Name = "_nameDataGridViewTextBoxColumn";
            this._nameDataGridViewTextBoxColumn.ReadOnly = true;
            this._nameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _xmlTvIdDataGridViewTextBoxColumn
            // 
            this._xmlTvIdDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._xmlTvIdDataGridViewTextBoxColumn.DataPropertyName = "XmlTvId";
            this._xmlTvIdDataGridViewTextBoxColumn.FillWeight = 40F;
            this._xmlTvIdDataGridViewTextBoxColumn.HeaderText = "External ID";
            this._xmlTvIdDataGridViewTextBoxColumn.Name = "_xmlTvIdDataGridViewTextBoxColumn";
            this._xmlTvIdDataGridViewTextBoxColumn.ReadOnly = true;
            this._xmlTvIdDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _channelsBindingSource
            // 
            this._channelsBindingSource.DataSource = typeof(ArgusTV.UI.Process.SortableBindingList<ArgusTV.DataContracts.GuideChannel>);
            this._channelsBindingSource.Sort = "";
            // 
            // _deleteButton
            // 
            this._deleteButton.Enabled = false;
            this._deleteButton.Location = new System.Drawing.Point(759, 78);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(135, 35);
            this._deleteButton.TabIndex = 99;
            this._deleteButton.Text = "Delete";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television Guide Channels",
            "Radio Guide Channels"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(0, 0);
            this._channelTypeComboBox.Margin = new System.Windows.Forms.Padding(0);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(268, 28);
            this._channelTypeComboBox.TabIndex = 105;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // GuideChannelsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._channelTypeComboBox);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._channelsDataGridView);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(894, 0);
            this.Name = "GuideChannelsPanel";
            this.Size = new System.Drawing.Size(894, 795);
            this.Load += new System.EventHandler(this.ChannelsPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._channelsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._channelsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _channelsDataGridView;
        private System.Windows.Forms.BindingSource _channelsBindingSource;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn _nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _xmlTvIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.ComboBox _channelTypeComboBox;
    }
}
