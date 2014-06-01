/*
 *	Copyright (C) 2007-2014 ARGUS TV
 *	http://www.argus-tv.com
 *
 *  This Program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *
 *  This Program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with GNU Make; see the file COPYING.  If not, write to
 *  the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *  http://www.gnu.org/copyleft/gpl.html
 *
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using ArgusTV.UI.Console.Properties;
using ArgusTV.UI.Process;
using ArgusTV.DataContracts;
using ArgusTV.WinForms;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Panels
{
    public partial class UpcomingProgramsPanel : ContentPanel
    {
        public UpcomingProgramsPanel()
        {
            InitializeComponent();
        }

        public UpcomingProgramsPanel(ScheduleType scheduleType)
            : this()
        {
            _scheduleType = scheduleType;
        }

        public override string Title
        {
            get { return "Upcoming " + _scheduleType.ToString() + "s"; }
        }

        private ScheduleType _scheduleType = ScheduleType.Recording;

        public ScheduleType ScheduleType
        {
            get { return _scheduleType; }
            set { _scheduleType = value; }
        }

        private UpcomingProgram GetSelectedUpcomingProgram()
        {
            UpcomingOrActiveProgramView upcomingProgramView = null;
            if (_upcomingProgramsControl.SelectedRows.Count > 0)
            {
                upcomingProgramView = _upcomingProgramsControl.SelectedRows[0].DataBoundItem as UpcomingOrActiveProgramView;
            }
            return upcomingProgramView == null ? null : upcomingProgramView.UpcomingProgram;
        }

        private void UpcomingProgramsPanel_Load(object sender, EventArgs e)
        {
            LoadSchedulesComboBox();
            _showSkippedRecordings.Visible = (_scheduleType == ScheduleType.Recording);
            _showSkippedRecordings.Checked = Properties.Settings.Default.ShowSkippedRecordings;
            _upcomingProgramsControl.ScheduleType = _scheduleType;
            _upcomingProgramsControl.Sortable = true;
            _upcomingProgramsControl.ShowScheduleName = true;
            RefreshUpcomingPrograms();
            StartListenerTask(EventGroup.RecordingEvents|EventGroup.ScheduleEvents);
        }

        public override void OnChildClosed(ContentPanel childPanel)
        {
            if (childPanel.DialogResult == DialogResult.OK)
            {
                RefreshUpcomingPrograms();
            }
        }

        public override void OnClosed()
        {
            _upcomingProgramsControl.UpcomingPrograms = null;
            _upcomingProgramsControl.UnfilteredUpcomingRecordings = null;
        }

        private void LoadSchedulesComboBox()
        {
            try
            {
                List<ScheduleSummary> schedules = new List<ScheduleSummary>();
                var allSchedules = Proxies.SchedulerService.GetAllSchedules(ChannelType.Television, _scheduleType, false);
                foreach (ScheduleSummary schedule in allSchedules)
                {
                    if (schedule.IsActive)
                    {
                        schedules.Add(schedule);
                    }
                }
                schedules.Sort(
                    delegate(ScheduleSummary s1, ScheduleSummary s2) { return s1.Name.CompareTo(s2.Name); });
                ScheduleSummary allSchedulesEntry = new ScheduleSummary();
                allSchedulesEntry.ScheduleId = Guid.Empty;
                allSchedulesEntry.Name = String.Empty;
                schedules.Insert(0, allSchedulesEntry);

                _schedulesComboBox.DataSource = schedules;
                _schedulesComboBox.DisplayMember = "Name";
            }
            catch (Exception ex)
            {
                _schedulesComboBox.DataSource = null;
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshUpcomingPrograms()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                UpcomingOrActiveProgramsList upcomingPrograms;
                if (_scheduleType == ScheduleType.Recording)
                {
                    UpcomingRecordingsFilter filter = _showSkippedRecordings.Checked ?
                        UpcomingRecordingsFilter.All : UpcomingRecordingsFilter.Recordings|UpcomingRecordingsFilter.CancelledByUser;
                    var allUpcomingRecordings = Proxies.ControlService.GetAllUpcomingRecordings(filter, true);
                    _upcomingProgramsControl.UnfilteredUpcomingRecordings = new UpcomingOrActiveProgramsList(allUpcomingRecordings);
                    upcomingPrograms = new UpcomingOrActiveProgramsList(allUpcomingRecordings);
                    upcomingPrograms.RemoveActiveRecordings(Proxies.ControlService.GetActiveRecordings());
                }
                else
                {
                    _upcomingProgramsControl.UnfilteredUpcomingRecordings = null;
                    upcomingPrograms = new UpcomingOrActiveProgramsList(Proxies.SchedulerService.GetAllUpcomingPrograms(_scheduleType, true));
                }

                ScheduleSummary schedule = _schedulesComboBox.SelectedItem as ScheduleSummary;
                if (schedule != null)
                {
                    upcomingPrograms.ApplyScheduleFilter(schedule.ScheduleId);
                }

                _upcomingProgramsControl.UpcomingPrograms = upcomingPrograms;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void _upcomingProgramsControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo htinfo = _upcomingProgramsControl.HitTest(e.X, e.Y);
                if (htinfo.Type == DataGridViewHitTestType.Cell)
                {
                    UpcomingOrActiveProgramView upcomingProgramView = _upcomingProgramsControl.Rows[htinfo.RowIndex].DataBoundItem as UpcomingOrActiveProgramView;
                    if (upcomingProgramView != null)
                    {
                        UpcomingProgram upcomingProgram = upcomingProgramView.UpcomingProgram;
                        _upcomingProgramsControl.Rows[htinfo.RowIndex].Selected = true;
                        _programContextMenuStrip.SetTarget(_scheduleType, upcomingProgram);
                        _programContextMenuStrip.Show(_upcomingProgramsControl, e.Location);
                    }
                }
            }
        }

        private void _showSkippedRecordings_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowSkippedRecordings = _showSkippedRecordings.Checked;
            Properties.Settings.Default.Save();
            RefreshUpcomingPrograms();
        }

        private void _schedulesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUpcomingPrograms();
        }

        protected override void OnArgusTVEvent(SynchronizationContext uiSyncContext, ServiceEvent @event)
        {
            if (@event.Name == ServiceEventNames.UpcomingRecordingsChanged
                || @event.Name == ServiceEventNames.UpcomingAlertsChanged
                || @event.Name == ServiceEventNames.UpcomingSuggestionsChanged)
            {
                uiSyncContext.Post(s => RefreshUpcomingPrograms(), null);
            }
        }

        #region Program Context Menu

        private void _programContextMenuStrip_CreateNewSchedule(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CreateNewScheduleEventArgs e)
        {
            Utility.ContextCreateNewSchedule(this, e);
        }

        private void _programContextMenuStrip_EditSchedule(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs e)
        {
            Utility.ContextEditSchedule(this, e);
        }

        private void _programContextMenuStrip_DeleteSchedule(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs e)
        {
            if (Utility.ContextDeleteSchedule(this, e))
            {
                RefreshUpcomingPrograms();
            }
        }

        private void _programContextMenuStrip_CancelProgram(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs e)
        {
            if (Utility.ContextCancelProgram(this, e))
            {
                RefreshUpcomingPrograms();
            }
        }

        private void _programContextMenuStrip_AddRemoveProgramHistory(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.AddRemoveProgramHistoryEventArgs e)
        {
            if (Utility.ContextAddRemoveProgramHistory(this, e))
            {
                RefreshUpcomingPrograms();
            }
        }

        private void _programContextMenuStrip_SetProgramPriority(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs e)
        {
            if (Utility.ContextSetProgramPriority(this, e))
            {
                RefreshUpcomingPrograms();
            }
        }

        private void _programContextMenuStrip_SetProgramPrePostRecord(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs e)
        {
            if (Utility.ContextSetProgramPrePostRecord(this, e))
            {
                RefreshUpcomingPrograms();
            }
        }

        #endregion
    }
}
