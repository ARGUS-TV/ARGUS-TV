namespace ArgusTV.UI.Console.Panels
{
    partial class ActiveRecordingsPanel
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
            this._programContextMenuStrip = new ArgusTV.WinForms.Controls.ProgramContextMenuStrip();
            this.SuspendLayout();
            // 
            // _upcomingProgramsControl
            // 
            this._upcomingProgramsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._upcomingProgramsControl.Location = new System.Drawing.Point(0, 0);
            this._upcomingProgramsControl.Name = "_upcomingProgramsControl";
            this._upcomingProgramsControl.ScheduleType = ArgusTV.DataContracts.ScheduleType.Recording;
            this._upcomingProgramsControl.ShowScheduleName = false;
            this._upcomingProgramsControl.Size = new System.Drawing.Size(831, 513);
            this._upcomingProgramsControl.Sortable = true;
            this._upcomingProgramsControl.TabIndex = 33;
            this._upcomingProgramsControl.UnfilteredUpcomingRecordings = null;
            this._upcomingProgramsControl.UpcomingPrograms = null;
            this._upcomingProgramsControl.GridMouseUp += new System.Windows.Forms.MouseEventHandler(this._upcomingProgramsControl_MouseUp);
            this._upcomingProgramsControl.GridDoubleClick += new System.EventHandler(this._upcomingProgramsControl_GridDoubleClick);
            // 
            // _programContextMenuStrip
            // 
            this._programContextMenuStrip.Name = "_programContextMenuStrip";
            this._programContextMenuStrip.Size = new System.Drawing.Size(308, 114);
            this._programContextMenuStrip.PlayWithVlc += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.PlayWithVlcEventArgs>(this._programContextMenuStrip_PlayWithVlc);
            this._programContextMenuStrip.CancelProgram += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs>(this._programContextMenuStrip_CancelProgram);
            this._programContextMenuStrip.SetProgramPriority += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs>(this._programContextMenuStrip_SetProgramPriority);
            this._programContextMenuStrip.DeleteSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_DeleteSchedule);
            this._programContextMenuStrip.EditSchedule += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs>(this._programContextMenuStrip_EditSchedule);
            this._programContextMenuStrip.SetProgramPrePostRecord += new System.EventHandler<ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs>(this._programContextMenuStrip_SetProgramPrePostRecord);
            // 
            // ActiveRecordingsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._upcomingProgramsControl);
            this.Name = "ActiveRecordingsPanel";
            this.Size = new System.Drawing.Size(831, 516);
            this.Load += new System.EventHandler(this.ActiveRecordingsPanel_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private ArgusTV.WinForms.UserControls.UpcomingOrActiveProgramsControl _upcomingProgramsControl;
        private ArgusTV.WinForms.Controls.ProgramContextMenuStrip _programContextMenuStrip;
    }
}
