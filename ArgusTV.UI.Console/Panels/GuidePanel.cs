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
using System.Globalization;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;
using ArgusTV.UI.Process.Guide;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Panels
{
    public partial class GuidePanel : ContentPanel
    {
        private static class SessionKey
        {
            public const string GuideDate = "WebAccess.Guide.GuideDate";
            public const string ChannelType = "WebAccess.Guide.ChannelType";
            public const string ChannelGroupId = "WebAccess.Guide.ChannelGroupId";
            public const string ScrollPosition = "WebAccess.Guide.ScrollPosition";
        }

        private GuideModel _model;
        private GuideController _controller;
        private bool _gotoNowTime;

        public GuidePanel()
        {
            InitializeComponent();
        }

        public override string Title
        {
            get { return "Electronic Program Guide (EPG)"; }
        }

        private bool _inLoad;

        private void GuidePanel_Load(object sender, EventArgs e)
        {
            try
            {
                _inLoad = true;

                Cursor.Current = Cursors.WaitCursor;

                _model = new GuideModel();
                _controller = new GuideController(_model);

                ChannelType channelType = ChannelType.Television;
                if (MainForm.Session.ContainsKey(SessionKey.ChannelType))
                {
                    channelType = (ChannelType)MainForm.Session[SessionKey.ChannelType];
                }
                _controller.Initialize(channelType, 24, ArgusTV.WinForms.Controls.EpgControl.EpgHoursOffset, "All Channels");

                _channelTypeComboBox.SelectedIndex = (int)_model.ChannelType;

                _groupsBindingSource.DataSource = _model.ChannelGroups;
                _channelGroupsComboBox.DisplayMember = "GroupName";
                _channelGroupsComboBox.ValueMember = "ChannelGroupId";
                if (MainForm.Session.ContainsKey(SessionKey.ChannelGroupId))
                {
                    _channelGroupsComboBox.SelectedValue = (Guid)MainForm.Session[SessionKey.ChannelGroupId];
                }

                DateTime guideDate = GetCurrentGuideDate();

                if (MainForm.Session.ContainsKey(SessionKey.GuideDate))
                {
                    guideDate = (DateTime)MainForm.Session[SessionKey.GuideDate];
                }
                _gotoNowTime = true;
                _guideDatePicker.Value = guideDate;

                RefreshEpg(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                _inLoad = false;
            }
        }

        private DateTime GetCurrentGuideDate()
        {
            DateTime guideDate = DateTime.Now;
            if (guideDate.TimeOfDay.TotalHours < _model.EpgHoursOffset)
            {
                guideDate = guideDate.AddDays(-1);
            }
            return guideDate.Date;
        }

        public override void OnClosed()
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
            }
            MainForm.Session[SessionKey.ScrollPosition] = _epgControl.ScrollPosition;
        }

        private void RefreshEpg(bool reloadEpg)
        {
            if (this.Visible
                && this.MainForm != null
                && !this.MainForm.Disposing)
            {
                if (_backgroundWorker.IsBusy)
                {
                    _backgroundWorker.CancelAsync();
                    while (_backgroundWorker.IsBusy)
                    {
                        System.Threading.Thread.Sleep(100);
                        Application.DoEvents();
                    }
                }
                RefreshEpgArgs args = new RefreshEpgArgs();
                args.GuideDateTime = _guideDatePicker.Value.Date.AddHours(_model.EpgHoursOffset);
                args.ChannelGroupId = (_model.ChannelType == ChannelType.Television)
                    ? ChannelGroup.AllTvChannelsGroupId : ChannelGroup.AllRadioChannelsGroupId;
                if (_channelGroupsComboBox.SelectedItem != null)
                {
                    ChannelGroup channelGroup = _channelGroupsComboBox.SelectedItem as ChannelGroup;
                    args.ChannelGroupId = channelGroup.ChannelGroupId;
                }
                args.ReloadEpg = reloadEpg;
                MainForm.Session[SessionKey.GuideDate] = args.GuideDateTime;
                MainForm.Session[SessionKey.ChannelType] = _model.ChannelType;
                MainForm.Session[SessionKey.ChannelGroupId] = args.ChannelGroupId;
                _loadingPanel.Visible = true;
                _backgroundWorker.RunWorkerAsync(args);
            }
        }

        private void OpenEditSchedulePanel(Schedule schedule)
        {
            EditSchedulePanel editPanel = new EditSchedulePanel();
            editPanel.Schedule = schedule;
            editPanel.OpenPanel(this);
        }

        public override void OnChildClosed(ContentPanel childPanel)
        {
            if (childPanel.DialogResult == DialogResult.OK)
            {
                _controller.RefreshUpcomingPrograms();
                RefreshEpg(false);
            }
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_inLoad)
            {
                _controller.ChangeChannelType((ChannelType)_channelTypeComboBox.SelectedIndex);
                _groupsBindingSource.DataSource = _model.ChannelGroups;
                _groupsBindingSource.ResetBindings(false);
                if (_channelGroupsComboBox.SelectedIndex == 0)
                {
                    RefreshEpg(true);
                }
                else
                {
                    _channelGroupsComboBox.SelectedIndex = 0;
                }
            }
        }

        private void _channelGroupsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_inLoad
                && _channelTypeComboBox.SelectedIndex >= 0)
            {
                RefreshEpg(true);
            }
        }

        private void _manualDatePicker_ValueChanged(object sender, EventArgs e)
        {
            if (!_inLoad
                && _channelTypeComboBox.SelectedIndex >= 0)
            {
                RefreshEpg(true);
            }
        }

        private void _nowButton_Click(object sender, EventArgs e)
        {
            DateTime currentGuideDate = GetCurrentGuideDate();
            if (_guideDatePicker.Value.Date == currentGuideDate)
            {
                _epgControl.GotoNowTime();
            }
            else
            {
                _gotoNowTime = true;
                _guideDatePicker.Value = currentGuideDate;
            }
        }

        private void _previousDayButton_Click(object sender, EventArgs e)
        {
            _guideDatePicker.Value = _guideDatePicker.Value.AddDays(-1);
        }

        private void _nextDayButton_Click(object sender, EventArgs e)
        {
            _guideDatePicker.Value = _guideDatePicker.Value.AddDays(1);
        }

        private void _epgControl_ProgramClicked(object sender, ArgusTV.WinForms.Controls.EpgControl.ProgramEventArgs e)
        {
            try
            {
                GuideProgram guideProgram = Proxies.GuideService.GetProgramById(e.GuideProgram.GuideProgramId);
                using (ProgramDetailsPopup popup = new ProgramDetailsPopup())
                {
                    popup.Channel = e.Channel;
                    popup.GuideProgram = guideProgram;
                    Point location = _epgControl.PointToScreen(e.Location);
                    location.Offset(-250, -40);
                    popup.Location = location;
                    popup.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void _epgControl_ProgramContextMenu(object sender, ArgusTV.WinForms.Controls.EpgControl.ProgramEventArgs e)
        {
            ScheduleType? scheduleType = null;
            UpcomingProgram upcomingProgram = null;
            UpcomingGuideProgram upcomingGuideProgram = null;

            GuideUpcomingProgram upcomingProgramInfo;
            if (_model.UpcomingRecordingsById.TryGetValue(e.UpcomingProgramId, out upcomingProgramInfo)
                || _model.UpcomingAlertsById.TryGetValue(e.UpcomingProgramId, out upcomingProgramInfo)
                || _model.UpcomingSuggestionsById.TryGetValue(e.UpcomingProgramId, out upcomingProgramInfo))
            {
                scheduleType = upcomingProgramInfo.Type;
                upcomingProgram = upcomingProgramInfo.UpcomingRecording != null ? upcomingProgramInfo.UpcomingRecording.Program : null;
                upcomingGuideProgram = upcomingProgramInfo.UpcomingGuideProgram;
            }

            _programContextMenuStrip.SetTarget(e.Channel, e.GuideProgram.GuideProgramId,
                e.GuideProgram.Title, e.GuideProgram.SubTitle,
                e.GuideProgram.EpisodeNumberDisplay, e.GuideProgram.StartTime,
                scheduleType, upcomingProgram, upcomingGuideProgram);
            _programContextMenuStrip.Show(_epgControl, e.Location);
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
                _controller.RefreshUpcomingPrograms();
                RefreshEpg(false);
            }
        }

        private void _programContextMenuStrip_CancelProgram(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs e)
        {
            if (Utility.ContextCancelProgram(this, e))
            {
                _controller.RefreshUpcomingPrograms();
                RefreshEpg(false);
            }
        }

        private void _programContextMenuStrip_AddRemoveProgramHistory(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.AddRemoveProgramHistoryEventArgs e)
        {
            if (Utility.ContextAddRemoveProgramHistory(this, e))
            {
                _controller.RefreshUpcomingPrograms();
                RefreshEpg(false);
            }
        }

        private void _programContextMenuStrip_SetProgramPriority(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs e)
        {
            if (Utility.ContextSetProgramPriority(this, e))
            {
                _controller.RefreshUpcomingPrograms();
                RefreshEpg(false);
            }
        }

        private void _programContextMenuStrip_SetProgramPrePostRecord(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs e)
        {
            if (Utility.ContextSetProgramPrePostRecord(this, e))
            {
                _controller.RefreshUpcomingPrograms();
                RefreshEpg(false);
            }
        }

        #endregion

        #region Background Worker

        private class RefreshEpgArgs
        {
            public bool ReloadEpg { set; get; }
            public Guid ChannelGroupId { set; get; }
            public DateTime GuideDateTime { set; get; }
        }

        private bool EpgCancellationPending()
        {
            return _backgroundWorker.CancellationPending;
        }

        private void _epgBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            RefreshEpgArgs args = e.Argument as RefreshEpgArgs;
            _controller.RefreshEpgData(args.ReloadEpg, args.ChannelGroupId, args.GuideDateTime, this.EpgCancellationPending);
            e.Cancel = _backgroundWorker.CancellationPending;
        }

        private void _epgBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.Visible)
            {
                _loadingPanel.Visible = false;
                if (e.Error != null)
                {
                    MessageBox.Show(this, e.Error.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!e.Cancelled)
                {
                    _epgControl.RefreshEpg(_model);
                    if (_gotoNowTime)
                    {
                        _epgControl.GotoNowTime();
                        _gotoNowTime = false;
                    }
                    if (MainForm.Session.ContainsKey(SessionKey.ScrollPosition))
                    {
                        _epgControl.ScrollPosition = (Point)MainForm.Session[SessionKey.ScrollPosition];
                        MainForm.Session.Remove(SessionKey.ScrollPosition);
                    }
                }
            }
        }

        #endregion
    }
}
