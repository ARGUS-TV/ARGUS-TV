namespace ArgusTV.UI.Console.UserControls
{
    partial class LogListControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._logDataGridView = new System.Windows.Forms.DataGridView();
            this._iconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.LogTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Module = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._logDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _logDataGridView
            // 
            this._logDataGridView.AllowUserToAddRows = false;
            this._logDataGridView.AllowUserToDeleteRows = false;
            this._logDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._logDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._logDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._logDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._logDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._logDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._iconColumn,
            this.LogTime,
            this.Module,
            this.Message});
            this._logDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logDataGridView.Location = new System.Drawing.Point(0, 0);
            this._logDataGridView.MultiSelect = false;
            this._logDataGridView.Name = "_logDataGridView";
            this._logDataGridView.ReadOnly = true;
            this._logDataGridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._logDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            this._logDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this._logDataGridView.RowTemplate.Height = 24;
            this._logDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._logDataGridView.Size = new System.Drawing.Size(1226, 603);
            this._logDataGridView.StandardTab = true;
            this._logDataGridView.TabIndex = 0;
            this._logDataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this._logDataGridView_DataBindingComplete);
            // 
            // _iconColumn
            // 
            this._iconColumn.HeaderText = "";
            this._iconColumn.Name = "_iconColumn";
            this._iconColumn.ReadOnly = true;
            this._iconColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._iconColumn.Width = 20;
            // 
            // LogTime
            // 
            this.LogTime.DataPropertyName = "LogTime";
            dataGridViewCellStyle2.Format = "G";
            dataGridViewCellStyle2.NullValue = null;
            this.LogTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.LogTime.HeaderText = "Date/Time";
            this.LogTime.Name = "LogTime";
            this.LogTime.ReadOnly = true;
            this.LogTime.Width = 140;
            // 
            // Module
            // 
            this.Module.DataPropertyName = "Module";
            this.Module.HeaderText = "Module";
            this.Module.Name = "Module";
            this.Module.ReadOnly = true;
            // 
            // Message
            // 
            this.Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Message.DataPropertyName = "Message";
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            // 
            // LogListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._logDataGridView);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "LogListControl";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(1226, 603);
            ((System.ComponentModel.ISupportInitialize)(this._logDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView _logDataGridView;
        private System.Windows.Forms.DataGridViewImageColumn _iconColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LogTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Module;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
    }
}
