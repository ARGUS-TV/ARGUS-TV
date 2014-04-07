namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    partial class SelectFoldersPage
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
            this._foldersDataGridView = new System.Windows.Forms.DataGridView();
            this._folderPathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._foldersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._deleteButton = new System.Windows.Forms.Button();
            this._addFolderButton = new System.Windows.Forms.Button();
            this._extensionsTextBox = new System.Windows.Forms.TextBox();
            this._extLabel = new System.Windows.Forms.Label();
            this._openButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._foldersDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._foldersBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _foldersDataGridView
            // 
            this._foldersDataGridView.AllowUserToAddRows = false;
            this._foldersDataGridView.AllowUserToDeleteRows = false;
            this._foldersDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._foldersDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._foldersDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._foldersDataGridView.AutoGenerateColumns = false;
            this._foldersDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._foldersDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._foldersDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._foldersDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._folderPathColumn});
            this._foldersDataGridView.DataSource = this._foldersBindingSource;
            this._foldersDataGridView.GridColor = System.Drawing.Color.White;
            this._foldersDataGridView.Location = new System.Drawing.Point(0, 0);
            this._foldersDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._foldersDataGridView.Name = "_foldersDataGridView";
            this._foldersDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._foldersDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._foldersDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._foldersDataGridView.Size = new System.Drawing.Size(752, 442);
            this._foldersDataGridView.TabIndex = 0;
            this._foldersDataGridView.SelectionChanged += new System.EventHandler(this._foldersDataGridView_SelectionChanged);
            // 
            // _folderPathColumn
            // 
            this._folderPathColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._folderPathColumn.DataPropertyName = "Folder";
            this._folderPathColumn.HeaderText = "Folder";
            this._folderPathColumn.Name = "_folderPathColumn";
            this._folderPathColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // _deleteButton
            // 
            this._deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._deleteButton.Enabled = false;
            this._deleteButton.Location = new System.Drawing.Point(760, 126);
            this._deleteButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(135, 35);
            this._deleteButton.TabIndex = 12;
            this._deleteButton.Text = "Remove";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // _addFolderButton
            // 
            this._addFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._addFolderButton.Location = new System.Drawing.Point(760, 37);
            this._addFolderButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._addFolderButton.Name = "_addFolderButton";
            this._addFolderButton.Size = new System.Drawing.Size(135, 35);
            this._addFolderButton.TabIndex = 10;
            this._addFolderButton.Text = "Add";
            this._addFolderButton.UseVisualStyleBackColor = true;
            this._addFolderButton.Click += new System.EventHandler(this._addFolderButton_Click);
            // 
            // _extensionsTextBox
            // 
            this._extensionsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._extensionsTextBox.Location = new System.Drawing.Point(100, 451);
            this._extensionsTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._extensionsTextBox.Name = "_extensionsTextBox";
            this._extensionsTextBox.Size = new System.Drawing.Size(649, 26);
            this._extensionsTextBox.TabIndex = 21;
            // 
            // _extLabel
            // 
            this._extLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._extLabel.AutoSize = true;
            this._extLabel.Location = new System.Drawing.Point(0, 455);
            this._extLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._extLabel.Name = "_extLabel";
            this._extLabel.Size = new System.Drawing.Size(91, 20);
            this._extLabel.TabIndex = 20;
            this._extLabel.Text = "Extensions:";
            // 
            // _openButton
            // 
            this._openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._openButton.Enabled = false;
            this._openButton.Location = new System.Drawing.Point(760, 82);
            this._openButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._openButton.Name = "_openButton";
            this._openButton.Size = new System.Drawing.Size(135, 35);
            this._openButton.TabIndex = 11;
            this._openButton.Text = "Open";
            this._openButton.UseVisualStyleBackColor = true;
            this._openButton.Click += new System.EventHandler(this._openButton_Click);
            // 
            // SelectFoldersPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._openButton);
            this.Controls.Add(this._extensionsTextBox);
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._extLabel);
            this.Controls.Add(this._addFolderButton);
            this.Controls.Add(this._foldersDataGridView);
            this.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.Name = "SelectFoldersPage";
            this.Size = new System.Drawing.Size(896, 482);
            ((System.ComponentModel.ISupportInitialize)(this._foldersDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._foldersBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView _foldersDataGridView;
        private System.Windows.Forms.BindingSource _foldersBindingSource;
        private System.Windows.Forms.Button _deleteButton;
        private System.Windows.Forms.Button _addFolderButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn _folderPathColumn;
        private System.Windows.Forms.TextBox _extensionsTextBox;
        private System.Windows.Forms.Label _extLabel;
        private System.Windows.Forms.Button _openButton;
    }
}
