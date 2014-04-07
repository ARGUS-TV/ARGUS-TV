namespace ArgusTV.UI.Console.Panels
{
    partial class PluginServicesPanel
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
            this._servicesDataGridView = new System.Windows.Forms.DataGridView();
            this.isActiveDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serviceUrlDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priorityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._servicesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._deleteButton = new System.Windows.Forms.Button();
            this._createNewButton = new System.Windows.Forms.Button();
            this._pingButton = new System.Windows.Forms.Button();
            this._createMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._testSharesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._servicesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._servicesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _servicesDataGridView
            // 
            this._servicesDataGridView.AllowUserToAddRows = false;
            this._servicesDataGridView.AllowUserToDeleteRows = false;
            this._servicesDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._servicesDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._servicesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._servicesDataGridView.AutoGenerateColumns = false;
            this._servicesDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._servicesDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._servicesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._servicesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.isActiveDataGridViewCheckBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.serviceUrlDataGridViewTextBoxColumn,
            this.priorityDataGridViewTextBoxColumn});
            this._servicesDataGridView.DataSource = this._servicesBindingSource;
            this._servicesDataGridView.GridColor = System.Drawing.Color.White;
            this._servicesDataGridView.Location = new System.Drawing.Point(0, 0);
            this._servicesDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._servicesDataGridView.MultiSelect = false;
            this._servicesDataGridView.Name = "_servicesDataGridView";
            this._servicesDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._servicesDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._servicesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._servicesDataGridView.Size = new System.Drawing.Size(750, 795);
            this._servicesDataGridView.TabIndex = 0;
            this._servicesDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this._servicesDataGridView_CellValueChanged);
            this._servicesDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this._servicesDataGridView_DataError);
            this._servicesDataGridView.SelectionChanged += new System.EventHandler(this._servicesDataGridView_SelectionChanged);
            // 
            // isActiveDataGridViewCheckBoxColumn
            // 
            this.isActiveDataGridViewCheckBoxColumn.DataPropertyName = "IsActive";
            this.isActiveDataGridViewCheckBoxColumn.HeaderText = "Active";
            this.isActiveDataGridViewCheckBoxColumn.Name = "isActiveDataGridViewCheckBoxColumn";
            this.isActiveDataGridViewCheckBoxColumn.Width = 45;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // serviceUrlDataGridViewTextBoxColumn
            // 
            this.serviceUrlDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.serviceUrlDataGridViewTextBoxColumn.DataPropertyName = "ServiceUrl";
            this.serviceUrlDataGridViewTextBoxColumn.FillWeight = 200F;
            this.serviceUrlDataGridViewTextBoxColumn.HeaderText = "Service URL";
            this.serviceUrlDataGridViewTextBoxColumn.Name = "serviceUrlDataGridViewTextBoxColumn";
            this.serviceUrlDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // priorityDataGridViewTextBoxColumn
            // 
            this.priorityDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.priorityDataGridViewTextBoxColumn.DataPropertyName = "Priority";
            this.priorityDataGridViewTextBoxColumn.FillWeight = 50F;
            this.priorityDataGridViewTextBoxColumn.HeaderText = "Priority";
            this.priorityDataGridViewTextBoxColumn.Name = "priorityDataGridViewTextBoxColumn";
            this.priorityDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _servicesBindingSource
            // 
            this._servicesBindingSource.DataSource = typeof(ArgusTV.UI.Process.SortableBindingList<ArgusTV.DataContracts.PluginService>);
            this._servicesBindingSource.Sort = "";
            // 
            // _deleteButton
            // 
            this._deleteButton.Enabled = false;
            this._deleteButton.Location = new System.Drawing.Point(759, 223);
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
            this._createNewButton.Location = new System.Drawing.Point(759, 178);
            this._createNewButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._createNewButton.Name = "_createNewButton";
            this._createNewButton.Size = new System.Drawing.Size(135, 35);
            this._createNewButton.TabIndex = 20;
            this._createNewButton.Text = "Create New";
            this._createNewButton.UseVisualStyleBackColor = true;
            this._createNewButton.Click += new System.EventHandler(this._createNewButton_Click);
            // 
            // _pingButton
            // 
            this._pingButton.Enabled = false;
            this._pingButton.Location = new System.Drawing.Point(759, 37);
            this._pingButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._pingButton.Name = "_pingButton";
            this._pingButton.Size = new System.Drawing.Size(135, 35);
            this._pingButton.TabIndex = 10;
            this._pingButton.Text = "Ping";
            this._pingButton.UseVisualStyleBackColor = true;
            this._pingButton.Click += new System.EventHandler(this._pingButton_Click);
            // 
            // _createMenuStrip
            // 
            this._createMenuStrip.Name = "_createMenuStrip";
            this._createMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // _testSharesButton
            // 
            this._testSharesButton.Enabled = false;
            this._testSharesButton.Location = new System.Drawing.Point(759, 82);
            this._testSharesButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._testSharesButton.Name = "_testSharesButton";
            this._testSharesButton.Size = new System.Drawing.Size(135, 35);
            this._testSharesButton.TabIndex = 22;
            this._testSharesButton.Text = "Test Shares";
            this._testSharesButton.UseVisualStyleBackColor = true;
            this._testSharesButton.Click += new System.EventHandler(this._testSharesButton_Click);
            // 
            // PluginServicesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._testSharesButton);
            this.Controls.Add(this._pingButton);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._createNewButton);
            this.Controls.Add(this._servicesDataGridView);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(894, 0);
            this.Name = "PluginServicesPanel";
            this.Size = new System.Drawing.Size(894, 795);
            this.Load += new System.EventHandler(this.PluginServicesPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this._servicesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._servicesBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _servicesDataGridView;
        private System.Windows.Forms.BindingSource _servicesBindingSource;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.Button _createNewButton;
        private System.Windows.Forms.Button _pingButton;
        private System.Windows.Forms.ContextMenuStrip _createMenuStrip;
        private System.Windows.Forms.Button _testSharesButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isActiveDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn serviceUrlDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn priorityDataGridViewTextBoxColumn;
    }
}
