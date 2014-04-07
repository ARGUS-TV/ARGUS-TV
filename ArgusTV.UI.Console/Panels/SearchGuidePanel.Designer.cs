namespace ArgusTV.UI.Console.Panels
{
    partial class SearchGuidePanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this._wordsLabel = new System.Windows.Forms.Label();
            this._searchTextBox = new System.Windows.Forms.TextBox();
            this._searchButton = new System.Windows.Forms.Button();
            this._tvChannelProgramsControl = new ArgusTV.UI.Console.UserControls.TvChannelProgramsControl();
            this._programContextMenuStrip = new ArgusTV.WinForms.Controls.ProgramContextMenuStrip();
            this._programsPanel = new System.Windows.Forms.Panel();
            this._backButton = new System.Windows.Forms.Button();
            this._titlesPanel = new System.Windows.Forms.Panel();
            this._titlesGridView = new System.Windows.Forms.DataGridView();
            this._showProgramsButton = new System.Windows.Forms.Button();
            this._channelTypeComboBox = new System.Windows.Forms.ComboBox();
            this._inLabel = new System.Windows.Forms.Label();
            this._programsPanel.SuspendLayout();
            this._titlesPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._titlesGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _wordsLabel
            // 
            this._wordsLabel.AutoSize = true;
            this._wordsLabel.Location = new System.Drawing.Point(0, 8);
            this._wordsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._wordsLabel.Name = "_wordsLabel";
            this._wordsLabel.Size = new System.Drawing.Size(193, 20);
            this._wordsLabel.TabIndex = 1;
            this._wordsLabel.Text = "Words in title starting with:";
            // 
            // _searchTextBox
            // 
            this._searchTextBox.Location = new System.Drawing.Point(204, 3);
            this._searchTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._searchTextBox.Name = "_searchTextBox";
            this._searchTextBox.Size = new System.Drawing.Size(298, 26);
            this._searchTextBox.TabIndex = 2;
            this._searchTextBox.Enter += new System.EventHandler(this._searchTextBox_Enter);
            this._searchTextBox.Leave += new System.EventHandler(this._searchTextBox_Leave);
            // 
            // _searchButton
            // 
            this._searchButton.Location = new System.Drawing.Point(666, 0);
            this._searchButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(112, 35);
            this._searchButton.TabIndex = 5;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // _tvChannelProgramsControl
            // 
            this._tvChannelProgramsControl.AllUpcomingPrograms = null;
            this._tvChannelProgramsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tvChannelProgramsControl.Location = new System.Drawing.Point(0, 0);
            this._tvChannelProgramsControl.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this._tvChannelProgramsControl.Name = "_tvChannelProgramsControl";
            this._tvChannelProgramsControl.Size = new System.Drawing.Size(1246, 709);
            this._tvChannelProgramsControl.Sortable = true;
            this._tvChannelProgramsControl.TabIndex = 10;
            this._tvChannelProgramsControl.TvChannelPrograms = null;
            this._tvChannelProgramsControl.GridMouseUp += new System.Windows.Forms.MouseEventHandler(this._tvChannelProgramsControl_MouseUp);
            // 
            // _programContextMenuStrip
            // 
            this._programContextMenuStrip.Name = "programContextMenuStrip1";
            this._programContextMenuStrip.Size = new System.Drawing.Size(433, 466);
            this._programContextMenuStrip.CreateNewSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CreateNewScheduleEventArgs>(this._programContextMenuStrip_CreateNewSchedule);
            this._programContextMenuStrip.EditSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_EditSchedule);
            this._programContextMenuStrip.DeleteSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_DeleteSchedule);
            this._programContextMenuStrip.CancelProgram += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs>(this._programContextMenuStrip_CancelProgram);
            this._programContextMenuStrip.AddRemoveProgramHistory += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.AddRemoveProgramHistoryEventArgs>(this._programContextMenuStrip_AddRemoveProgramHistory);
            this._programContextMenuStrip.SetProgramPriority += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs>(this._programContextMenuStrip_SetProgramPriority);
            this._programContextMenuStrip.SetProgramPrePostRecord += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs>(this._programContextMenuStrip_SetProgramPrePostRecord);
            // 
            // _programsPanel
            // 
            this._programsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._programsPanel.Controls.Add(this._backButton);
            this._programsPanel.Controls.Add(this._tvChannelProgramsControl);
            this._programsPanel.Location = new System.Drawing.Point(0, 43);
            this._programsPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._programsPanel.Name = "_programsPanel";
            this._programsPanel.Size = new System.Drawing.Size(1246, 751);
            this._programsPanel.TabIndex = 6;
            // 
            // _backButton
            // 
            this._backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._backButton.Location = new System.Drawing.Point(0, 715);
            this._backButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._backButton.Name = "_backButton";
            this._backButton.Size = new System.Drawing.Size(180, 35);
            this._backButton.TabIndex = 11;
            this._backButton.Text = "Back";
            this._backButton.UseVisualStyleBackColor = true;
            this._backButton.Click += new System.EventHandler(this._backButton_Click);
            // 
            // _titlesPanel
            // 
            this._titlesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._titlesPanel.Controls.Add(this._titlesGridView);
            this._titlesPanel.Controls.Add(this._showProgramsButton);
            this._titlesPanel.Location = new System.Drawing.Point(0, 43);
            this._titlesPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._titlesPanel.Name = "_titlesPanel";
            this._titlesPanel.Size = new System.Drawing.Size(1246, 751);
            this._titlesPanel.TabIndex = 7;
            // 
            // _titlesGridView
            // 
            this._titlesGridView.AllowUserToAddRows = false;
            this._titlesGridView.AllowUserToDeleteRows = false;
            this._titlesGridView.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this._titlesGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this._titlesGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._titlesGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._titlesGridView.BackgroundColor = System.Drawing.Color.White;
            this._titlesGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this._titlesGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._titlesGridView.Location = new System.Drawing.Point(0, 0);
            this._titlesGridView.MultiSelect = false;
            this._titlesGridView.Name = "_titlesGridView";
            this._titlesGridView.ReadOnly = true;
            this._titlesGridView.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._titlesGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            this._titlesGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this._titlesGridView.RowTemplate.Height = 24;
            this._titlesGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._titlesGridView.Size = new System.Drawing.Size(1246, 711);
            this._titlesGridView.StandardTab = true;
            this._titlesGridView.TabIndex = 1;
            this._titlesGridView.SelectionChanged += new System.EventHandler(this._titlesGridView_SelectionChanged);
            this._titlesGridView.DoubleClick += new System.EventHandler(this._titlesGridView_DoubleClick);
            // 
            // _showProgramsButton
            // 
            this._showProgramsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._showProgramsButton.Location = new System.Drawing.Point(0, 715);
            this._showProgramsButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._showProgramsButton.Name = "_showProgramsButton";
            this._showProgramsButton.Size = new System.Drawing.Size(180, 35);
            this._showProgramsButton.TabIndex = 2;
            this._showProgramsButton.Text = "Show Programs";
            this._showProgramsButton.UseVisualStyleBackColor = true;
            this._showProgramsButton.Click += new System.EventHandler(this._showProgramsButton_Click);
            // 
            // _channelTypeComboBox
            // 
            this._channelTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._channelTypeComboBox.FormattingEnabled = true;
            this._channelTypeComboBox.Items.AddRange(new object[] {
            "Television",
            "Radio"});
            this._channelTypeComboBox.Location = new System.Drawing.Point(544, 2);
            this._channelTypeComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._channelTypeComboBox.Name = "_channelTypeComboBox";
            this._channelTypeComboBox.Size = new System.Drawing.Size(110, 28);
            this._channelTypeComboBox.TabIndex = 4;
            this._channelTypeComboBox.SelectedIndexChanged += new System.EventHandler(this._channelTypeComboBox_SelectedIndexChanged);
            // 
            // _inLabel
            // 
            this._inLabel.AutoSize = true;
            this._inLabel.Location = new System.Drawing.Point(513, 8);
            this._inLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this._inLabel.Name = "_inLabel";
            this._inLabel.Size = new System.Drawing.Size(21, 20);
            this._inLabel.TabIndex = 3;
            this._inLabel.Text = "in";
            // 
            // SearchGuidePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._inLabel);
            this.Controls.Add(this._channelTypeComboBox);
            this.Controls.Add(this._wordsLabel);
            this.Controls.Add(this._searchButton);
            this.Controls.Add(this._searchTextBox);
            this.Controls.Add(this._titlesPanel);
            this.Controls.Add(this._programsPanel);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SearchGuidePanel";
            this.Size = new System.Drawing.Size(1246, 794);
            this.Load += new System.EventHandler(this.UpcomingProgramsPanel_Load);
            this._programsPanel.ResumeLayout(false);
            this._titlesPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._titlesGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _wordsLabel;
        private System.Windows.Forms.TextBox _searchTextBox;
        private System.Windows.Forms.Button _searchButton;
        private ArgusTV.UI.Console.UserControls.TvChannelProgramsControl _tvChannelProgramsControl;
        private ArgusTV.WinForms.Controls.ProgramContextMenuStrip _programContextMenuStrip;
        private System.Windows.Forms.Panel _programsPanel;
        private System.Windows.Forms.Button _backButton;
        private System.Windows.Forms.Panel _titlesPanel;
        private System.Windows.Forms.Button _showProgramsButton;
        private System.Windows.Forms.DataGridView _titlesGridView;
        private System.Windows.Forms.ComboBox _channelTypeComboBox;
        private System.Windows.Forms.Label _inLabel;
    }
}
