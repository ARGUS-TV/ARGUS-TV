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
using System.Windows.Forms;

using ArgusTV.UI.Console.Properties;
using ArgusTV.UI.Process;
using ArgusTV.DataContracts;
using ArgusTV.WinForms;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Panels
{
    public partial class ActiveRecordingsPanel : ContentPanel
    {
        public ActiveRecordingsPanel()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Active Recordings"; }
        }

        private UpcomingOrActiveProgramView GetSelectedUpcomingProgramView()
        {
            UpcomingOrActiveProgramView upcomingProgramView = null;
            if (_upcomingProgramsControl.SelectedRows.Count > 0)
            {
                upcomingProgramView = _upcomingProgramsControl.SelectedRows[0].DataBoundItem as UpcomingOrActiveProgramView;
            }
            return upcomingProgramView;
        }

        private void ActiveRecordingsPanel_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _upcomingProgramsControl.ScheduleType = ScheduleType.Recording;
                RefreshActiveRecordings();
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

        public override void OnClosed()
        {
            _upcomingProgramsControl.UpcomingPrograms = null;
            _upcomingProgramsControl.UnfilteredUpcomingRecordings = null;
        }

        private void RefreshActiveRecordings()
        {
            _upcomingProgramsControl.UnfilteredUpcomingRecordings =
                new UpcomingOrActiveProgramsList(Proxies.ControlService.GetAllUpcomingRecordings(UpcomingRecordingsFilter.All, true));
            _upcomingProgramsControl.UpcomingPrograms = new UpcomingOrActiveProgramsList(Proxies.ControlService.GetActiveRecordings());
        }

        private void _upcomingProgramsControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo htinfo = _upcomingProgramsControl.HitTest(e.X, e.Y);
                if (htinfo.Type == DataGridViewHitTestType.Cell)
                {
                    UpcomingOrActiveProgramView upcomingProgramView = _upcomingProgramsControl.Rows[htinfo.RowIndex].DataBoundItem as UpcomingOrActiveProgramView;
                    if (upcomingProgramView != null
                        && upcomingProgramView.ActiveRecording != null)
                    {
                        _upcomingProgramsControl.Rows[htinfo.RowIndex].Selected = true;
                        _programContextMenuStrip.SetTarget(upcomingProgramView.ActiveRecording);
                        _programContextMenuStrip.Show(_upcomingProgramsControl, e.Location);
                    }
                }
            }
        }

        private void _upcomingProgramsControl_GridDoubleClick(object sender, EventArgs e)
        {
            Point mousePosition = _upcomingProgramsControl.PointToClient(Control.MousePosition);
            DataGridView.HitTestInfo htinfo = _upcomingProgramsControl.HitTest(mousePosition.X, mousePosition.Y);
            if (htinfo.Type == DataGridViewHitTestType.Cell)
            {
                PlayRecording(GetSelectedUpcomingProgramView());
            }
        }

        private static void PlayRecording(UpcomingOrActiveProgramView upcomingProgramView)
        {
            if (upcomingProgramView != null
                && upcomingProgramView.ActiveRecording != null)
            {
                WinFormsUtility.RunVlc(upcomingProgramView.ActiveRecording.RecordingFileName);
            }
        }

        #region Program Context Menu

        private void _programContextMenuStrip_PlayWithVlc(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.PlayWithVlcEventArgs e)
        {
            WinFormsUtility.RunVlc(e.RecordingFileName);
        }

        private void _programContextMenuStrip_EditSchedule(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs e)
        {
            Utility.ContextEditSchedule(this, e);
        }

        private void _programContextMenuStrip_DeleteSchedule(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.EditScheduleEventArgs e)
        {
            if (Utility.ContextDeleteSchedule(this, e))
            {
                _upcomingProgramsControl.RemoveUpcomingProgramsForSchedule(e.ScheduleId);
            }
        }

        private void _programContextMenuStrip_CancelProgram(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs e)
        {
            if (Utility.ContextCancelProgram(this, e))
            {
                if (e.Cancel)
                {
                    _upcomingProgramsControl.RemoveUpcomingProgram(e.ScheduleId, e.GuideProgramId, e.ChannelId, e.StartTime);
                }
            }
        }

        private void _programContextMenuStrip_SetProgramPriority(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs e)
        {
            if (Utility.ContextSetProgramPriority(this, e))
            {
                RefreshActiveRecordings();
            }
        }

        private void _programContextMenuStrip_SetProgramPrePostRecord(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs e)
        {
            if (Utility.ContextSetProgramPrePostRecord(this, e))
            {
                RefreshActiveRecordings();
            }
        }

        #endregion
    }
}
