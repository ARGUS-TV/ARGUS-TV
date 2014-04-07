namespace ArgusTV.UI.Console.Panels
{
    partial class UpcomingProgramsPanel
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
            this._upcomingProgramsControl = new ArgusTV.WinForms.UserControls.UpcomingOrActiveProgramsControl();
            this._upcomingTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this._filterLabel = new System.Windows.Forms.Label();
            this._schedulesComboBox = new System.Windows.Forms.ComboBox();
            this._showSkippedRecordings = new System.Windows.Forms.CheckBox();
            this._programContextMenuStrip = new ArgusTV.WinForms.Controls.ProgramContextMenuStrip();
            this._upcomingTableLayoutPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _upcomingProgramsControl
            // 
            this._upcomingProgramsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._upcomingProgramsControl.Location = new System.Drawing.Point(0, 25);
            this._upcomingProgramsControl.Margin = new System.Windows.Forms.Padding(0);
            this._upcomingProgramsControl.Name = "_upcomingProgramsControl";
            this._upcomingProgramsControl.ScheduleType = ArgusTV.DataContracts.ScheduleType.Recording;
            this._upcomingProgramsControl.ShowScheduleName = false;
            this._upcomingProgramsControl.Size = new System.Drawing.Size(831, 491);
            this._upcomingProgramsControl.Sortable = true;
            this._upcomingProgramsControl.TabIndex = 33;
            this._upcomingProgramsControl.UnfilteredUpcomingRecordings = null;
            this._upcomingProgramsControl.UpcomingPrograms = null;
            this._upcomingProgramsControl.GridMouseUp += new System.Windows.Forms.MouseEventHandler(this._upcomingProgramsControl_MouseUp);
            // 
            // _upcomingTableLayoutPanel
            // 
            this._upcomingTableLayoutPanel.ColumnCount = 1;
            this._upcomingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._upcomingTableLayoutPanel.Controls.Add(this._upcomingProgramsControl, 0, 1);
            this._upcomingTableLayoutPanel.Controls.Add(this.panel1, 0, 0);
            this._upcomingTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._upcomingTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this._upcomingTableLayoutPanel.Name = "_upcomingTableLayoutPanel";
            this._upcomingTableLayoutPanel.RowCount = 2;
            this._upcomingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._upcomingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._upcomingTableLayoutPanel.Size = new System.Drawing.Size(831, 516);
            this._upcomingTableLayoutPanel.TabIndex = 34;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._filterLabel);
            this.panel1.Controls.Add(this._schedulesComboBox);
            this.panel1.Controls.Add(this._showSkippedRecordings);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(831, 21);
            this.panel1.TabIndex = 34;
            // 
            // _filterLabel
            // 
            this._filterLabel.AutoSize = true;
            this._filterLabel.Location = new System.Drawing.Point(0, 4);
            this._filterLabel.Name = "_filterLabel";
            this._filterLabel.Size = new System.Drawing.Size(32, 13);
            this._filterLabel.TabIndex = 36;
            this._filterLabel.Text = "Filter:";
            // 
            // _schedulesComboBox
            // 
            this._schedulesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._schedulesComboBox.Location = new System.Drawing.Point(34, 0);
            this._schedulesComboBox.Name = "_schedulesComboBox";
            this._schedulesComboBox.Size = new System.Drawing.Size(250, 21);
            this._schedulesComboBox.TabIndex = 35;
            this._schedulesComboBox.SelectedIndexChanged += new System.EventHandler(this._schedulesComboBox_SelectedIndexChanged);
            // 
            // _showSkippedRecordings
            // 
            this._showSkippedRecordings.AutoSize = true;
            this._showSkippedRecordings.Location = new System.Drawing.Point(291, 3);
            this._showSkippedRecordings.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this._showSkippedRecordings.Name = "_showSkippedRecordings";
            this._showSkippedRecordings.Size = new System.Drawing.Size(145, 17);
            this._showSkippedRecordings.TabIndex = 34;
            this._showSkippedRecordings.Text = "Show skipped recordings";
            this._showSkippedRecordings.UseVisualStyleBackColor = true;
            this._showSkippedRecordings.CheckedChanged += new System.EventHandler(this._showSkippedRecordings_CheckedChanged);
            // 
            // _programContextMenuStrip
            // 
            this._programContextMenuStrip.Name = "programContextMenuStrip1";
            this._programContextMenuStrip.Size = new System.Drawing.Size(308, 158);
            this._programContextMenuStrip.CreateNewSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CreateNewScheduleEventArgs>(this._programContextMenuStrip_CreateNewSchedule);
            this._programContextMenuStrip.CancelProgram += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs>(this._programContextMenuStrip_CancelProgram);
            this._programContextMenuStrip.AddRemoveProgramHistory += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.AddRemoveProgramHistoryEventArgs>(this._programContextMenuStrip_AddRemoveProgramHistory);
            this._programContextMenuStrip.SetProgramPriority += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs>(this._programContextMenuStrip_SetProgramPriority);
            this._programContextMenuStrip.DeleteSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_DeleteSchedule);
            this._programContextMenuStrip.EditSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_EditSchedule);
            this._programContextMenuStrip.SetProgramPrePostRecord += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs>(this._programContextMenuStrip_SetProgramPrePostRecord);
            // 
            // UpcomingProgramsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._upcomingTableLayoutPanel);
            this.Name = "UpcomingProgramsPanel";
            this.Size = new System.Drawing.Size(831, 516);
            this.Load += new System.EventHandler(this.UpcomingProgramsPanel_Load);
            this._upcomingTableLayoutPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ArgusTV.WinForms.UserControls.UpcomingOrActiveProgramsControl _upcomingProgramsControl;
        private System.Windows.Forms.TableLayoutPanel _upcomingTableLayoutPanel;
        private System.Windows.Forms.CheckBox _showSkippedRecordings;
        private ArgusTV.WinForms.Controls.ProgramContextMenuStrip _programContextMenuStrip;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox _schedulesComboBox;
        private System.Windows.Forms.Label _filterLabel;
    }
}
