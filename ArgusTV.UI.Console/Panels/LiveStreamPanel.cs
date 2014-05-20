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
using System.Runtime.InteropServices;

using ArgusTV.DataContracts;
using ArgusTV.UI.Console.Properties;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;
using ArgusTV.UI.Process.Guide;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Panels
{
    public partial class LiveTvPanel : ContentPanel
    {
        private static class ColumnIndex
        {
            public const int Channel = 0;
            public const int CurrentIcon = 1;
            public const int CurrentTitle = 2;
            public const int NextIcon = 3;
            public const int NextTitle = 4;
        }

        private UpcomingGuideProgramsDictionary _allUpcomingGuidePrograms = new UpcomingGuideProgramsDictionary();

        public LiveTvPanel()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_channelsGridView);
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_streamsDataGridView);
        }

        public override string Title
        {
            get { return "Live TV/Radio"; }
        }

        private bool _inLoad;

        private void ChannelsPanel_Load(object sender, EventArgs e)
        {
            try
            {
                _inLoad = true;
                Cursor.Current = Cursors.WaitCursor;
                _channelTypeComboBox.SelectedIndex = (int)ChannelType.Television;
                LoadAllGroups();
                LoadAllActiveStreams();
                EnableButtons();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                _inLoad = false;
            }
        }

        public override void OnClosed()
        {
            _channelsBindingSource.DataSource = null;
            _channelsBindingSource.ResetBindings(false);
            _streamsBindingSource.DataSource = null;
            _streamsBindingSource.ResetBindings(false);
        }

        public override void OnChildClosed(ContentPanel childPanel)
        {
            if (childPanel.DialogResult == DialogResult.OK)
            {
                RefreshSelectedGroupChannels();
            }
        }

        private void LoadAllGroups()
        {
            try
            {
                ChannelType channelType = (ChannelType)_channelTypeComboBox.SelectedIndex;
                List<ChannelGroup> channelGroups = new List<ChannelGroup>(Proxies.SchedulerService.GetAllChannelGroups(channelType, true));
                channelGroups.Add(new ChannelGroup()
                {
                    ChannelGroupId = channelType == ChannelType.Television ? ChannelGroup.AllTvChannelsGroupId : ChannelGroup.AllRadioChannelsGroupId,
                    ChannelType = channelType,
                    GroupName = "All Channels",
                    VisibleInGuide = true
                });
                _channelGroupsComboBox.DataSource = channelGroups;
                _channelGroupsComboBox.DisplayMember = "GroupName";
                _channelGroupsComboBox.ValueMember = "ChannelGroupId";
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAllActiveStreams()
        {
            try
            {
                _streamsBindingSource.DataSource = new LiveStreamsList(Proxies.ControlService.GetLiveStreams());
                RefreshSelectedGroupChannels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Channel> GetSelectedAvailableChannels()
        {
            List<Channel> channels = new List<Channel>();
            if (_streamsDataGridView.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow row in _streamsDataGridView.Rows)
                {
                    if (row.Selected)
                    {
                        channels.Add(row.DataBoundItem as Channel);
                    }
                }
            }
            return channels;
        }

        private void EnableButtons()
        {
            Channel selectedChannel = GetSelectedChannel();
            LiveStream selectedStream = GetSelectedStream();
            _startLiveTvButton.Enabled = (selectedChannel != null);
            _stopLiveTvButton.Enabled = (selectedStream != null);
            _openStreamButton.Enabled = (selectedStream != null);
            _tuneChannelButton.Enabled = (selectedChannel != null && selectedStream != null);
        }

        private Channel GetSelectedChannel()
        {
            if (_channelsGridView.SelectedRows.Count > 0)
            {
                CurrentAndNextProgramView currentAndNext = _channelsGridView.SelectedRows[0].DataBoundItem as CurrentAndNextProgramView;
                if (currentAndNext != null)
                {
                    return currentAndNext.CurrentAndNextProgram.Channel;
                }
            }
            return null;
        }

        private LiveStream GetSelectedStream()
        {
            if (_streamsDataGridView.SelectedRows.Count > 0)
            {
                LiveStreamView liveStreamView = _streamsDataGridView.SelectedRows[0].DataBoundItem as LiveStreamView;
                if (liveStreamView != null)
                {
                    return liveStreamView.LiveStream;
                }
            }
            return null;
        }

        private void RefreshSelectedGroupChannels()
        {
            try
            {
                RefreshAllUpcomingPrograms();
                ChannelGroup channelGroup = _channelGroupsComboBox.SelectedItem as ChannelGroup;
                _channelsBindingSource.DataSource = new CurrentAndNextProgramsList(
                    Proxies.SchedulerService.GetCurrentAndNextForGroup(channelGroup.ChannelGroupId, true, null));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _channelsBindingSource.DataSource = null;
            }
            _channelsGridView.ClearSelection();
            _channelsBindingSource.ResetBindings(true);
        }

        private void _streamsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void _startLiveTvButton_Click(object sender, EventArgs e)
        {
            StartLiveTv();
        }

        private void _tuneChannelButton_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                Channel channel = GetSelectedChannel();
                LiveStream liveStream = GetSelectedStream();
                if (liveStream != null
                    && channel != null)
                {
                    _channelsGridView.ClearSelection();
                    string lastRtspUrl = liveStream.RtspUrl;
                    LiveStreamResult result = Proxies.ControlService.TuneLiveStream(channel, ref liveStream);
                    if (result == LiveStreamResult.Succeeded)
                    {
                        LoadAllActiveStreams();
                        if (liveStream.RtspUrl != lastRtspUrl)
                        {
                            WinFormsUtility.RunStreamPlayer(liveStream.RtspUrl, true);
                        }
                    }
                    else
                    {
                        ShowLiveStreamResultMessageBox(result);
                    }
                }
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

        private void _stopLiveTvButton_Click(object sender, EventArgs e)
        {
            LiveStream stream = GetSelectedStream();
            if (stream != null)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Proxies.ControlService.StopLiveStream(stream);
                    LoadAllActiveStreams();
                    _streamsDataGridView.ClearSelection();
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
        }

        private void _refreshStreamsButton_Click(object sender, EventArgs e)
        {
            LoadAllActiveStreams();
        }

        private void _channelsGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            StartLiveTv();
        }

        private void _streamsDataGridView_DoubleClick(object sender, EventArgs e)
        {
            _openStreamButton_Click(sender, e);
        }

        private void _openStreamButton_Click(object sender, EventArgs e)
        {
            LiveStream stream = GetSelectedStream();
            if (stream != null)
            {
                if (DialogResult.Yes == MessageBox.Show(this, "WARNING: opening this stream will hijack it from any other clients." + Environment.NewLine + Environment.NewLine + "Are you sure you want to do this?", this.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    try
                    {
                        if (!WinFormsUtility.RunStreamPlayer(stream.RtspUrl, true))
                        {
                            MessageBox.Show(this, "VLC not found on this system.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void StartLiveTv()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                Channel channel = GetSelectedChannel();
                if (channel != null)
                {
                    _channelsGridView.ClearSelection();
                    LiveStream liveStream = null;
                    LiveStreamResult result = Proxies.ControlService.TuneLiveStream(channel, ref liveStream);
                    if (result == LiveStreamResult.Succeeded)
                    {
                        LoadAllActiveStreams();
                        WinFormsUtility.RunStreamPlayer(liveStream.RtspUrl, true);
                    }
                    else
                    {
                        ShowLiveStreamResultMessageBox(result);
                    }
                }
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

        private void ShowLiveStreamResultMessageBox(LiveStreamResult result)
        {
            if (result == LiveStreamResult.NotSupported)
            {
                MessageBox.Show(this, "No tuner was able to start the live stream.", "Information", MessageBoxButtons.OK);
            }
            else if (result == LiveStreamResult.NoFreeCardFound)
            {
                MessageBox.Show(this, "No free card was found to start the live stream.", "Information", MessageBoxButtons.OK);
            }
            else if (result == LiveStreamResult.ChannelTuneFailed)
            {
                MessageBox.Show(this, "Failed to tune to channel.", "Information", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(this, "Unable to start live streaming.", "Information", MessageBoxButtons.OK);
            }
        }

        private void RefreshAllUpcomingPrograms()
        {
            var upcomingRecordings = Proxies.ControlService.GetAllUpcomingRecordings(UpcomingRecordingsFilter.All, true);
            var upcomingAlerts = Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Alert, true);
            var upcomingSuggestions = Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Suggestion, true);
            _allUpcomingGuidePrograms = new UpcomingGuideProgramsDictionary(upcomingRecordings, upcomingAlerts, upcomingSuggestions);
        }

        public void RefreshIcons()
        {
            foreach (DataGridViewRow row in _channelsGridView.Rows)
            {
                Guid? currentUpcomingProgramId = null;
                Guid? nextUpcomingProgramId = null;

                CurrentAndNextProgramView programView = row.DataBoundItem as CurrentAndNextProgramView;
                if (programView != null)
                {
                    if (programView.CurrentAndNextProgram.Current != null)
                    {
                        currentUpcomingProgramId = programView.CurrentAndNextProgram.Current.GetUniqueUpcomingProgramId(
                            programView.CurrentAndNextProgram.Channel.ChannelId);
                    }
                    if (programView.CurrentAndNextProgram.Next != null)
                    {
                        nextUpcomingProgramId = programView.CurrentAndNextProgram.Next.GetUniqueUpcomingProgramId(
                            programView.CurrentAndNextProgram.Channel.ChannelId);
                    }
                }

                SetCellIcon(row, ColumnIndex.CurrentIcon, programView.CurrentAndNextProgram.Channel, currentUpcomingProgramId);
                SetCellIcon(row, ColumnIndex.NextIcon, programView.CurrentAndNextProgram.Channel, nextUpcomingProgramId);
            }
        }

        private void SetCellIcon(DataGridViewRow row, int columnIndex, Channel channel, Guid? upcomingProgramId)
        {
            Icon icon = Properties.Resources.TransparentIcon;
            string toolTip = null;
            if (upcomingProgramId.HasValue
                && _allUpcomingGuidePrograms.ContainsKey(upcomingProgramId.Value))
            {
                GuideUpcomingProgram programInfo = _allUpcomingGuidePrograms[upcomingProgramId.Value];

                ProgramIconUtility.GetIconAndToolTip(programInfo.Type, programInfo.CancellationReason, programInfo.IsPartOfSeries,
                    _allUpcomingGuidePrograms.UpcomingRecordings, programInfo.UpcomingRecording, out icon, out toolTip);
            }
            row.Cells[columnIndex].Value = icon;
            row.Cells[columnIndex].ToolTipText = toolTip;
        }

        private void _channelTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_inLoad)
            {
                LoadAllGroups();
            }
        }

        private void _channelGroupsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_inLoad)
            {
                RefreshSelectedGroupChannels();
            }
        }

        private void _refreshChannelsButton_Click(object sender, EventArgs e)
        {
            RefreshSelectedGroupChannels();
        }

        private void _channelsGridView_SelectionChanged(object sender, EventArgs e)
        {
            EnableButtons();
        }

        private void _channelsGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            RefreshIcons();
        }

        private void _channelsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0
                && e.RowIndex < _channelsBindingSource.Count)
            {
                CurrentAndNextProgramView programView = _channelsGridView.Rows[e.RowIndex].DataBoundItem as CurrentAndNextProgramView;
                if (programView != null
                    && (programView.CurrentAndNextProgram.LiveState == ChannelLiveState.Tunable || programView.CurrentAndNextProgram.LiveState == ChannelLiveState.Unknown))
                {
                    e.CellStyle.ForeColor = Color.Black;
                    e.CellStyle.SelectionForeColor = Color.Black;
                    _channelsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = null;
                }
                else
                {
                    e.CellStyle.ForeColor = Color.DarkGray;
                    e.CellStyle.SelectionForeColor = Color.Gray;

                    string tooltip = null;
                    if (programView != null)
                    {
                        switch (programView.CurrentAndNextProgram.LiveState)
                        {
                            case ChannelLiveState.NoFreeCard:
                                tooltip = "No free card available for channel";
                                break;

                            case ChannelLiveState.NotTunable:
                                tooltip = "Channel is not tunable";
                                break;
                        }
                    }
                    _channelsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = tooltip;
                }
                if (_channelsGridView.Columns[e.ColumnIndex] is DataGridViewLinkColumn)
                {
                    if (_channelsGridView.Rows[e.RowIndex].Selected)
                    {
                        ((DataGridViewLinkCell)_channelsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex]).LinkColor = e.CellStyle.SelectionForeColor;
                    }
                    else
                    {
                        ((DataGridViewLinkCell)_channelsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex]).LinkColor = e.CellStyle.ForeColor;
                    }
                }
            }
        }

        private void _channelsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == ColumnIndex.CurrentTitle || e.ColumnIndex == ColumnIndex.NextTitle)
                && e.RowIndex >= 0
                && e.RowIndex < _channelsGridView.Rows.Count)
            {
                CurrentAndNextProgramView programView = _channelsGridView.Rows[e.RowIndex].DataBoundItem as CurrentAndNextProgramView;
                GuideProgramSummary programSummary = (e.ColumnIndex == ColumnIndex.CurrentTitle)
                    ? programView.CurrentAndNextProgram.Current : programView.CurrentAndNextProgram.Next;
                if (programSummary != null)
                {
                    GuideProgram guideProgram = Proxies.GuideService.GetProgramById(programSummary.GuideProgramId);
                    using (ProgramDetailsPopup popup = new ProgramDetailsPopup())
                    {
                        popup.Channel = programView.CurrentAndNextProgram.Channel;
                        popup.GuideProgram = guideProgram;
                        Point location = Cursor.Position;
                        location.Offset(-250, -40);
                        popup.Location = location;
                        popup.ShowDialog(this);
                    }
                }
            }
        }

        private void _channelsGridView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                DataGridView.HitTestInfo htinfo = _channelsGridView.HitTest(e.X, e.Y);
                if (htinfo.Type == DataGridViewHitTestType.Cell)
                {
                    CurrentAndNextProgramView programView = _channelsGridView.Rows[htinfo.RowIndex].DataBoundItem as CurrentAndNextProgramView;
                    if (programView != null)
                    {
                        GuideProgramSummary guideProgram = null;
                        if (htinfo.ColumnIndex == ColumnIndex.CurrentIcon
                            || htinfo.ColumnIndex == ColumnIndex.CurrentTitle)
                        {
                            guideProgram = programView.CurrentAndNextProgram.Current;
                        }
                        else if (htinfo.ColumnIndex == ColumnIndex.NextIcon
                            || htinfo.ColumnIndex == ColumnIndex.NextTitle)
                        {
                            guideProgram = programView.CurrentAndNextProgram.Next;
                        }
                        if (guideProgram != null)
                        {
                            Guid upcomingProgramId = guideProgram.GetUniqueUpcomingProgramId(programView.CurrentAndNextProgram.Channel.ChannelId);

                            GuideUpcomingProgram upcomingProgramInfo = null;
                            _allUpcomingGuidePrograms.TryGetValue(upcomingProgramId, out upcomingProgramInfo);

                            _channelsGridView.Rows[htinfo.RowIndex].Selected = true;

                            ScheduleType? scheduleType = null;
                            UpcomingProgram upcomingProgram = null;
                            UpcomingGuideProgram upcomingGuideProgram = null;
                            if (upcomingProgramInfo != null)
                            {
                                scheduleType = upcomingProgramInfo.Type;
                                upcomingProgram = upcomingProgramInfo.UpcomingRecording != null ? upcomingProgramInfo.UpcomingRecording.Program : null;
                                upcomingGuideProgram = upcomingProgramInfo.UpcomingGuideProgram;
                            }

                            _programContextMenuStrip.SetTarget(programView.CurrentAndNextProgram.Channel, guideProgram.GuideProgramId,
                                guideProgram.Title, guideProgram.SubTitle, guideProgram.EpisodeNumberDisplay, guideProgram.StartTime,
                                scheduleType, upcomingProgram, upcomingGuideProgram);
                            _programContextMenuStrip.Show(_channelsGridView, e.Location);
                        }
                    }
                }
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
                RefreshSelectedGroupChannels();
            }
        }

        private void _programContextMenuStrip_CancelProgram(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.CancelProgramEventArgs e)
        {
            if (Utility.ContextCancelProgram(this, e))
            {
                RefreshSelectedGroupChannels();
            }
        }

        private void _programContextMenuStrip_AddRemoveProgramHistory(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.AddRemoveProgramHistoryEventArgs e)
        {
            if (Utility.ContextAddRemoveProgramHistory(this, e))
            {
                RefreshSelectedGroupChannels();
            }
        }

        private void _programContextMenuStrip_SetProgramPriority(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPriorityEventArgs e)
        {
            if (Utility.ContextSetProgramPriority(this, e))
            {
                RefreshSelectedGroupChannels();
            }
        }

        private void _programContextMenuStrip_SetProgramPrePostRecord(object sender, ArgusTV.WinForms.Controls.ProgramContextMenuStrip.SetProgramPrePostRecordEventArgs e)
        {
            if (Utility.ContextSetProgramPrePostRecord(this, e))
            {
                RefreshSelectedGroupChannels();
            }
        }

        #endregion

    }
}
