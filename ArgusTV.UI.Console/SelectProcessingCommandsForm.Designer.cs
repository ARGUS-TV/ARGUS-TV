namespace ArgusTV.UI.Console
{
    partial class SelectProcessingCommandsForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._commandsDataGridView = new System.Windows.Forms.DataGridView();
            this._runCommandColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this._commandNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._commandsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._commandsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._commandsBindingSource)).BeginInit();
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
            this._runCommandColumn,
            this._commandNameColumn});
            this._commandsDataGridView.DataSource = this._commandsBindingSource;
            this._commandsDataGridView.GridColor = System.Drawing.Color.White;
            this._commandsDataGridView.Location = new System.Drawing.Point(18, 18);
            this._commandsDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._commandsDataGridView.MultiSelect = false;
            this._commandsDataGridView.Name = "_commandsDataGridView";
            this._commandsDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._commandsDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._commandsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._commandsDataGridView.Size = new System.Drawing.Size(555, 415);
            this._commandsDataGridView.TabIndex = 1;
            // 
            // _runCommandColumn
            // 
            this._runCommandColumn.DataPropertyName = "RunCommand";
            this._runCommandColumn.HeaderText = "Run";
            this._runCommandColumn.Name = "_runCommandColumn";
            this._runCommandColumn.Width = 50;
            // 
            // _commandNameColumn
            // 
            this._commandNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._commandNameColumn.DataPropertyName = "Name";
            this._commandNameColumn.HeaderText = "Name";
            this._commandNameColumn.Name = "_commandNameColumn";
            this._commandNameColumn.ReadOnly = true;
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(460, 448);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(112, 35);
            this._cancelButton.TabIndex = 13;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(339, 448);
            this._okButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(112, 35);
            this._okButton.TabIndex = 12;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // SelectProcessingCommandsForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(591, 498);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._commandsDataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectProcessingCommandsForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Schedule Processing Commands";
            this.Load += new System.EventHandler(this.SelectProcessingCommandsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._commandsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._commandsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _commandsDataGridView;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn _runCommandColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn _commandNameColumn;
        private System.Windows.Forms.BindingSource _commandsBindingSource;
    }
}