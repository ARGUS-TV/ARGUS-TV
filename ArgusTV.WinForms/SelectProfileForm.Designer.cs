namespace ArgusTV.WinForms
{
    partial class SelectProfileForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectProfileForm));
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._profilesDataGridView = new System.Windows.Forms.DataGridView();
            this._nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._editProfileColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this._removeProfileColumn = new System.Windows.Forms.DataGridViewLinkColumn();
            this._profilesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._connectionsLabel = new System.Windows.Forms.Label();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serverSettingsDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.savePasswordDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._profilesDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._profilesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._okButton.Location = new System.Drawing.Point(222, 288);
            this._okButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(112, 35);
            this._okButton.TabIndex = 100;
            this._okButton.Text = "Connect";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelButton.Location = new System.Drawing.Point(344, 288);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(112, 35);
            this._cancelButton.TabIndex = 101;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            // 
            // _profilesDataGridView
            // 
            this._profilesDataGridView.AllowUserToAddRows = false;
            this._profilesDataGridView.AllowUserToDeleteRows = false;
            this._profilesDataGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._profilesDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._profilesDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._profilesDataGridView.AutoGenerateColumns = false;
            this._profilesDataGridView.BackgroundColor = System.Drawing.Color.White;
            this._profilesDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._profilesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._profilesDataGridView.ColumnHeadersVisible = false;
            this._profilesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this._nameColumn,
            this._editProfileColumn,
            this._removeProfileColumn,
            this.nameDataGridViewTextBoxColumn,
            this.serverSettingsDataGridViewTextBoxColumn,
            this.savePasswordDataGridViewCheckBoxColumn});
            this._profilesDataGridView.DataSource = this._profilesBindingSource;
            this._profilesDataGridView.Location = new System.Drawing.Point(18, 38);
            this._profilesDataGridView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._profilesDataGridView.MultiSelect = false;
            this._profilesDataGridView.Name = "_profilesDataGridView";
            this._profilesDataGridView.ReadOnly = true;
            this._profilesDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._profilesDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._profilesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._profilesDataGridView.Size = new System.Drawing.Size(438, 240);
            this._profilesDataGridView.StandardTab = true;
            this._profilesDataGridView.TabIndex = 1;
            this._profilesDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this._profilesDataGridView_CellContentClick);
            this._profilesDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this._profilesDataGridView_CellFormatting);
            this._profilesDataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this._profilesDataGridView_CellPainting);
            this._profilesDataGridView.DoubleClick += new System.EventHandler(this._profilesDataGridView_DoubleClick);
            this._profilesDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this._profilesDataGridView_KeyDown);
            // 
            // _nameColumn
            // 
            this._nameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this._nameColumn.DataPropertyName = "Name";
            this._nameColumn.HeaderText = "Connection";
            this._nameColumn.Name = "_nameColumn";
            this._nameColumn.ReadOnly = true;
            this._nameColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // _editProfileColumn
            // 
            this._editProfileColumn.ActiveLinkColor = System.Drawing.Color.DarkRed;
            this._editProfileColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._editProfileColumn.HeaderText = "";
            this._editProfileColumn.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._editProfileColumn.LinkColor = System.Drawing.Color.DarkSlateBlue;
            this._editProfileColumn.Name = "_editProfileColumn";
            this._editProfileColumn.ReadOnly = true;
            this._editProfileColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._editProfileColumn.TrackVisitedState = false;
            this._editProfileColumn.Width = 22;
            // 
            // _removeProfileColumn
            // 
            this._removeProfileColumn.ActiveLinkColor = System.Drawing.Color.DarkRed;
            this._removeProfileColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this._removeProfileColumn.HeaderText = "";
            this._removeProfileColumn.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this._removeProfileColumn.LinkColor = System.Drawing.Color.DarkSlateBlue;
            this._removeProfileColumn.Name = "_removeProfileColumn";
            this._removeProfileColumn.ReadOnly = true;
            this._removeProfileColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this._removeProfileColumn.Text = "";
            this._removeProfileColumn.TrackVisitedState = false;
            this._removeProfileColumn.Width = 22;
            // 
            // _profilesBindingSource
            // 
            this._profilesBindingSource.DataSource = typeof(System.Collections.Generic.IList<ArgusTV.Client.Common.ConnectionProfile>);
            // 
            // _connectionsLabel
            // 
            this._connectionsLabel.AutoSize = true;
            this._connectionsLabel.Location = new System.Drawing.Point(18, 14);
            this._connectionsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._connectionsLabel.Name = "_connectionsLabel";
            this._connectionsLabel.Size = new System.Drawing.Size(102, 20);
            this._connectionsLabel.TabIndex = 0;
            this._connectionsLabel.Text = "Connections:";
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // serverSettingsDataGridViewTextBoxColumn
            // 
            this.serverSettingsDataGridViewTextBoxColumn.DataPropertyName = "ServerSettings";
            this.serverSettingsDataGridViewTextBoxColumn.HeaderText = "ServerSettings";
            this.serverSettingsDataGridViewTextBoxColumn.Name = "serverSettingsDataGridViewTextBoxColumn";
            this.serverSettingsDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // savePasswordDataGridViewCheckBoxColumn
            // 
            this.savePasswordDataGridViewCheckBoxColumn.DataPropertyName = "SavePassword";
            this.savePasswordDataGridViewCheckBoxColumn.HeaderText = "SavePassword";
            this.savePasswordDataGridViewCheckBoxColumn.Name = "savePasswordDataGridViewCheckBoxColumn";
            this.savePasswordDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // SelectProfileForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.ClientSize = new System.Drawing.Size(474, 338);
            this.Controls.Add(this._connectionsLabel);
            this.Controls.Add(this._profilesDataGridView);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectProfileForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connect to ARGUS TV";
            this.Load += new System.EventHandler(this.SelectProfileForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this._profilesDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._profilesBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.DataGridView _profilesDataGridView;
        private System.Windows.Forms.BindingSource _profilesBindingSource;
        private System.Windows.Forms.Label _connectionsLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn _nameColumn;
        private System.Windows.Forms.DataGridViewLinkColumn _editProfileColumn;
        private System.Windows.Forms.DataGridViewLinkColumn _removeProfileColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn serverSettingsDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn savePasswordDataGridViewCheckBoxColumn;
    }
}