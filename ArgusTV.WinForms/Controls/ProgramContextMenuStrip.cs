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
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

using ArgusTV.UI.Process.Guide;
using ArgusTV.DataContracts;
using ArgusTV.WinForms;
using ArgusTV.ServiceProxy;

namespace ArgusTV.WinForms.Controls
{
    public class ProgramContextMenuStrip : ContextMenuStrip
    {
        private const int _menuWidth = 193;

        private class PrePostRecordEntry
        {
            public PrePostRecordEntry(string text, int seconds, bool postRecordOnly)
            {
                this.Text = text;
                this.Seconds = seconds;
                this.PostRecordOnly = postRecordOnly;
            }

            public string Text { set; get; }
            public int Seconds { set; get; }
            public bool PostRecordOnly { set; get; }
        }

        private List<PrePostRecordEntry> _prePostRecordEntries = new List<PrePostRecordEntry>()
        {
            new PrePostRecordEntry("-2 mins", -120, true),
            new PrePostRecordEntry("-1 min", -60, true),
            new PrePostRecordEntry("-30 seconds", -30, true),
            new PrePostRecordEntry("0 mins", 0, false),
            new PrePostRecordEntry("1 min", 60, false),
            new PrePostRecordEntry("2 mins", 120, false),
            new PrePostRecordEntry("5 mins", 300, false),
            new PrePostRecordEntry("10 mins", 600, false),
            new PrePostRecordEntry("15 mins", 900, false),
            new PrePostRecordEntry("20 mins", 1200, false),
            new PrePostRecordEntry("30 mins", 1800, false)
        };

        private ToolStripMenuItem _editScheduleItem;
        private ToolStripMenuItem _priorityScheduleItem;
        private ToolStripMenuItem _resetToSchedulePriorityItem;
        private ToolStripMenuItem _preRecordScheduleItem;
        private ToolStripMenuItem _postRecordScheduleItem;
        private ToolStripMenuItem _resetToSchedulePreRecordItem;
        private ToolStripMenuItem _resetToSchedulePostRecordItem;
        private ToolStripMenuItem _deleteScheduleItem;
        private ToolStripMenuItem _cancelProgramItem;
        private ToolStripMenuItem _uncancelProgramItem;
        private ToolStripMenuItem _addToPreviouslyRecordedHistoryItem;
        private ToolStripMenuItem _removeFromPreviouslyRecordedHistoryItem;
        private ToolStripSeparator _separator1;
        private ToolStripSeparator _separator2;
        private ToolStripMenuItem _recordItem;
        private ToolStripMenuItem _recordDailyItem;
        private ToolStripMenuItem _recordWeeklyItem;
        private ToolStripMenuItem _recordAnyTimeItem;
        private ToolStripMenuItem _abortRecordingItem;
        private ToolStripMenuItem _playWithVlcItem;

        private Channel _channel;
        private Guid? _guideProgramId;
        private string _title;
        private string _subTitle;
        private string _episodeNumberDisplay;
        private DateTime _startTime;
        private ScheduleType? _scheduleType;
        private UpcomingProgram _upcomingProgram;
        private UpcomingGuideProgram _upcomingGuideProgram;
        private ActiveRecording _activeRecording;
        private bool _isTerminalServicesSession;

        private Guid? ScheduleId
        {
            get
            {
                return _upcomingProgram != null ? (Guid?)_upcomingProgram.ScheduleId :
                  (_upcomingGuideProgram != null ? (Guid?)_upcomingGuideProgram.ScheduleId : null);
            }
        }

        private bool IsPartOfSeries
        {
            get
            {
                return _upcomingProgram != null ? _upcomingProgram.IsPartOfSeries :
                  (_upcomingGuideProgram != null ? _upcomingGuideProgram.IsPartOfSeries : false);
            }
        }

        private bool IsCancelled
        {
            get
            {
                return _upcomingProgram != null ? _upcomingProgram.IsCancelled :
                  (_upcomingGuideProgram != null ? _upcomingGuideProgram.IsCancelled : false);
            }
        }

        public UpcomingCancellationReason CancellationReason
        {
            get
            {
                return _upcomingProgram != null ? _upcomingProgram.CancellationReason :
                  (_upcomingGuideProgram != null ? _upcomingGuideProgram.CancellationReason : UpcomingCancellationReason.None);
            }
        }

        private const int SM_REMOTESESSION = 0x1000;

        [DllImport("user32.dll", EntryPoint = ("GetSystemMetrics"))]
        private static extern bool GetSystemMetrics(int nIndex);

        #region Events

        public class CreateNewScheduleEventArgs : EventArgs
        {
            public CreateNewScheduleEventArgs(Schedule schedule)
            {
                _schedule = schedule;
            }

            private Schedule _schedule;

            public Schedule Schedule
            {
                get { return _schedule; }
            }
        }

        public event EventHandler<CreateNewScheduleEventArgs> CreateNewSchedule;

        public class EditScheduleEventArgs : EventArgs
        {
            public EditScheduleEventArgs(Guid scheduleId)
            {
                _scheduleId = scheduleId;
            }

            private Guid _scheduleId;

            public Guid ScheduleId
            {
                get { return _scheduleId; }
            }
        }

        public event EventHandler<EditScheduleEventArgs> EditSchedule;
        public event EventHandler<EditScheduleEventArgs> DeleteSchedule;

        public class CancelProgramEventArgs : EventArgs
        {
            public CancelProgramEventArgs(bool cancel, Guid scheduleId, Guid? guideProgramId, Guid channelId, DateTime startTime)
            {
                _cancel = cancel;
                _scheduleId = scheduleId;
                _guideProgramId = guideProgramId;
                _channelId = channelId;
                _startTime = startTime;
            }

            private bool _cancel;

            public bool Cancel
            {
                get { return _cancel; }
            }

            private Guid _scheduleId;

            public Guid ScheduleId
            {
                get { return _scheduleId; }
            }

            private Guid? _guideProgramId;

            public Guid? GuideProgramId
            {
                get { return _guideProgramId; }
            }

            private Guid _channelId;

            public Guid ChannelId
            {
                get { return _channelId; }
            }

            private DateTime _startTime;

            public DateTime StartTime
            {
                get { return _startTime; }
            }
        }

        public event EventHandler<CancelProgramEventArgs> CancelProgram;

        public class AddRemoveProgramHistoryEventArgs : EventArgs
        {
            public AddRemoveProgramHistoryEventArgs(bool addToHistory, UpcomingProgram upcomingProgram)
            {
                _addToHistory = addToHistory;
                _upcomingProgram = upcomingProgram;
            }

            private bool _addToHistory;

            public bool AddToHistory
            {
                get { return _addToHistory; }
            }

            private UpcomingProgram _upcomingProgram;

            public UpcomingProgram UpcomingProgram
            {
                get { return _upcomingProgram; }
            }
        }

        public event EventHandler<AddRemoveProgramHistoryEventArgs> AddRemoveProgramHistory;

        public class SetProgramPriorityEventArgs : EventArgs
        {
            public SetProgramPriorityEventArgs(Guid upcomingProgramId, DateTime startTime, UpcomingProgramPriority? priority)
            {
                _upcomingProgramId = upcomingProgramId;
                _startTime = startTime;
                _priority = priority;
            }

            private Guid _upcomingProgramId;

            public Guid UpcomingProgramId
            {
                get { return _upcomingProgramId; }
            }

            private DateTime _startTime;

            public DateTime StartTime
            {
                get { return _startTime; }
            }

            private UpcomingProgramPriority? _priority;

            public UpcomingProgramPriority? Priority
            {
                get { return _priority; }
            }
        }

        public event EventHandler<SetProgramPriorityEventArgs> SetProgramPriority;

        public class SetProgramPrePostRecordEventArgs : EventArgs
        {
            public SetProgramPrePostRecordEventArgs(Guid upcomingProgramId, DateTime startTime, bool isPreRecord, int? seconds)
            {
                _upcomingProgramId = upcomingProgramId;
                _startTime = startTime;
                _isPreRecord = isPreRecord;
                _seconds = seconds;
            }

            private Guid _upcomingProgramId;

            public Guid UpcomingProgramId
            {
                get { return _upcomingProgramId; }
            }

            private DateTime _startTime;

            public DateTime StartTime
            {
                get { return _startTime; }
            }

            private bool _isPreRecord;

            public bool IsPreRecord
            {
                get { return _isPreRecord; }
            }

            private int? _seconds;

            public int? Seconds
            {
                get { return _seconds; }
            }
        }

        public event EventHandler<PlayWithVlcEventArgs> PlayWithVlc;

        public class PlayWithVlcEventArgs : EventArgs
        {
            public PlayWithVlcEventArgs(string recordingFileName)
            {
                _recordingFileName = recordingFileName;
            }

            private string _recordingFileName;

            public string RecordingFileName
            {
                get { return _recordingFileName; }
            }
        }

        public event EventHandler<SetProgramPrePostRecordEventArgs> SetProgramPrePostRecord;

        #endregion

        public ProgramContextMenuStrip()
        {
            _isTerminalServicesSession = GetSystemMetrics(SM_REMOTESESSION);
            Icon recordIcon = ProgramIconUtility.GetIcon(ScheduleType.Recording, false);
            Icon recordSeriesIcon = ProgramIconUtility.GetIcon(ScheduleType.Recording, true);
            _recordItem = AddItem(this.Items, recordIcon, "Record");
            _recordDailyItem = AddItem(this.Items, recordSeriesIcon, "Record Daily");
            _recordWeeklyItem = AddItem(this.Items, recordSeriesIcon, "Record Weekly");
            _recordAnyTimeItem = AddItem(this.Items, recordSeriesIcon, "Record Any Time");
            _playWithVlcItem = AddItem(this.Items, "Play Recording With VLC");
            _abortRecordingItem = AddItem(this.Items, Properties.Resources.RecordCancelledIcon, "Abort Recording");

            _separator1 = AddSeparator(this.Items);
            _priorityScheduleItem = AddItem(this.Items, "Priority");
            AddItem(_priorityScheduleItem.DropDownItems, "Very Low", UpcomingProgramPriority.VeryLow);
            AddItem(_priorityScheduleItem.DropDownItems, "Low", UpcomingProgramPriority.Low);
            AddItem(_priorityScheduleItem.DropDownItems, "Normal", UpcomingProgramPriority.Normal);
            AddItem(_priorityScheduleItem.DropDownItems, "High", UpcomingProgramPriority.High);
            AddItem(_priorityScheduleItem.DropDownItems, "Very High", UpcomingProgramPriority.VeryHigh);
            AddItem(_priorityScheduleItem.DropDownItems, "Highest", UpcomingProgramPriority.Highest);
            AddSeparator(_priorityScheduleItem.DropDownItems);
            _resetToSchedulePriorityItem = AddItem(_priorityScheduleItem.DropDownItems, Properties.Resources.Reset, "Reset To Schedule Priority");
            _preRecordScheduleItem = AddItem(this.Items, "Pre-record");
            _postRecordScheduleItem = AddItem(this.Items, "Post-record");

            _separator2 = AddSeparator(this.Items);
            _editScheduleItem = AddItem(this.Items, Properties.Resources.EditProfile, "Edit Schedule");
            _deleteScheduleItem = AddItem(this.Items, Properties.Resources.RemoveProfile, "Delete Schedule");
            _cancelProgramItem = AddItem(this.Items, Properties.Resources.RecordSeriesCancelledIcon, "Cancel This Program");
            _uncancelProgramItem = AddItem(this.Items, Properties.Resources.RecordSeriesIcon, "Uncancel This Program");
            _addToPreviouslyRecordedHistoryItem = AddItem(this.Items, Properties.Resources.RecordSeriesCancelledHistoryIcon, "Add To Previously Recorded History");
            _removeFromPreviouslyRecordedHistoryItem = AddItem(this.Items, Properties.Resources.RecordSeriesIcon, "Remove From Previously Recorded History");
        }

        private void AddPrePostRecordEntries(ToolStripMenuItem item, int currentSeconds, bool isPostRecord)
        {
            bool itemChecked = false;
            item.DropDownItems.Clear();
            foreach (PrePostRecordEntry entry in _prePostRecordEntries)
            {
                if (!isPostRecord
                    && entry.PostRecordOnly)
                {
                    continue;
                }
                if (!itemChecked
                    && entry.Seconds > currentSeconds)
                {
                    TimeSpan span = TimeSpan.FromSeconds(currentSeconds);
                    string text = String.Format(CultureInfo.CurrentCulture, "{0} mins", span.TotalMinutes);
                    ToolStripMenuItem currentItem = AddItem(item.DropDownItems, text, currentSeconds);
                    currentItem.Checked = true;
                    itemChecked = true;
                }
                ToolStripMenuItem childItem = AddItem(item.DropDownItems, entry.Text, entry.Seconds);
                if (entry.Seconds == currentSeconds)
                {
                    childItem.Checked = true;
                    itemChecked = true;
                }
            }
        }

        public void SetTarget(ScheduleType scheduleType, UpcomingProgram upcomingProgram)
        {
            SetTarget(upcomingProgram.Channel, upcomingProgram.GuideProgramId, upcomingProgram.Title,
                upcomingProgram.SubTitle, upcomingProgram.EpisodeNumberDisplay,
                upcomingProgram.StartTime, scheduleType, upcomingProgram, null, null);
        }

        public void SetTarget(ActiveRecording activeRecording)
        {
            SetTarget(activeRecording.Program.Channel, activeRecording.Program.GuideProgramId, activeRecording.Program.Title,
                activeRecording.Program.SubTitle, activeRecording.Program.EpisodeNumberDisplay,
                activeRecording.Program.StartTime, ScheduleType.Recording, activeRecording.Program, null, activeRecording);
        }

        public void SetTarget(Channel channel, Guid? guideProgramId, string title, string subTitle, string episodeNumberDisplay, DateTime startTime,
            ScheduleType? scheduleType, UpcomingProgram upcomingProgram, UpcomingGuideProgram upcomingGuideProgram)
        {
            SetTarget(channel, guideProgramId, title, subTitle, episodeNumberDisplay, startTime,
                scheduleType, upcomingProgram, upcomingGuideProgram, null);
        }

        private void SetTarget(Channel channel, Guid? guideProgramId, string title, string subTitle, string episodeNumberDisplay, DateTime startTime,
            ScheduleType? scheduleType, UpcomingProgram upcomingProgram, UpcomingGuideProgram upcomingGuideProgram, ActiveRecording activeRecording)
        {
            _channel = channel;
            _guideProgramId = guideProgramId;
            _title = title;
            _subTitle = subTitle;
            _episodeNumberDisplay = episodeNumberDisplay;
            _startTime = startTime;
            _scheduleType = scheduleType;
            _upcomingProgram = upcomingProgram;
            _activeRecording = activeRecording;
            _upcomingGuideProgram = upcomingGuideProgram;
        }

        protected override void OnOpening(System.ComponentModel.CancelEventArgs e)
        {
            bool hasSchedule = _scheduleType.HasValue;
            bool showRecordItems = true;
            bool isActiveRecording = _activeRecording != null;

            _playWithVlcItem.Visible = isActiveRecording;
            _playWithVlcItem.Enabled = WinFormsUtility.IsVlcInstalled();
            _abortRecordingItem.Visible = isActiveRecording;

            _editScheduleItem.Visible = hasSchedule;
            if (hasSchedule)
            {
                bool isRecording = (_scheduleType == ScheduleType.Recording);
                if (isRecording)
                {
                    showRecordItems = false;
                }

                if (isRecording)
                {
                    _priorityScheduleItem.Visible = true;
                    _preRecordScheduleItem.Visible = true;
                    _resetToSchedulePreRecordItem = PopulatePrePostRecordItem(_preRecordScheduleItem,
                        _upcomingProgram.PreRecordSeconds, "Reset To Schedule Pre-record", false);
                    _postRecordScheduleItem.Visible = true;
                    _resetToSchedulePostRecordItem = PopulatePrePostRecordItem(_postRecordScheduleItem,
                        _upcomingProgram.PostRecordSeconds, "Reset To Schedule Post-record", true);
                    _separator2.Visible = true;
                }
                else
                {
                    _priorityScheduleItem.Visible = false;
                    _preRecordScheduleItem.Visible = false;
                    _postRecordScheduleItem.Visible = false;
                    _separator2.Visible = false;
                }

                _deleteScheduleItem.Visible = !this.IsCancelled && _scheduleType != ScheduleType.Suggestion;
                _cancelProgramItem.Visible = !isActiveRecording && !this.IsCancelled
                    && this.IsPartOfSeries && _scheduleType != ScheduleType.Suggestion;
                _uncancelProgramItem.Visible = !isActiveRecording && this.IsCancelled
                    && this.CancellationReason == UpcomingCancellationReason.Manual && _scheduleType != ScheduleType.Suggestion;
                _addToPreviouslyRecordedHistoryItem.Visible = !isActiveRecording && isRecording && !this.IsCancelled;
                _removeFromPreviouslyRecordedHistoryItem.Visible = !isActiveRecording && isRecording && this.IsCancelled
                    && this.CancellationReason == UpcomingCancellationReason.PreviouslyRecorded;

                if (isRecording
                    && _upcomingProgram != null)
                {
                    foreach (ToolStripItem item in _priorityScheduleItem.DropDownItems)
                    {
                        ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                        if (menuItem != null)
                        {
                            menuItem.Checked = (menuItem.Tag is UpcomingProgramPriority
                                && (UpcomingProgramPriority)menuItem.Tag == _upcomingProgram.Priority);
                        }
                    }
                }
            }
            else
            {
                _priorityScheduleItem.Visible = false;
                _preRecordScheduleItem.Visible = false;
                _postRecordScheduleItem.Visible = false;
                _separator2.Visible = false;
                _deleteScheduleItem.Visible = false;
                _cancelProgramItem.Visible = false;
                _uncancelProgramItem.Visible = false;
                _addToPreviouslyRecordedHistoryItem.Visible = false;
                _removeFromPreviouslyRecordedHistoryItem.Visible = false;
            }

            _recordItem.Visible = showRecordItems;
            _recordDailyItem.Visible = showRecordItems;
            _recordWeeklyItem.Visible = showRecordItems;
            _recordAnyTimeItem.Visible = showRecordItems;
            _separator1.Visible = hasSchedule && (showRecordItems || isActiveRecording);

            base.OnOpening(e);
        }

        private ToolStripMenuItem PopulatePrePostRecordItem(ToolStripMenuItem item, int seconds, string resetText, bool isPostRecord)
        {
            AddPrePostRecordEntries(item, seconds, isPostRecord);
            AddSeparator(item.DropDownItems);
            return AddItem(item.DropDownItems, Properties.Resources.Reset, resetText);
        }

        private void item_Click(object sender, EventArgs e)
        {
            Schedule schedule = null;
            if (sender == _editScheduleItem)
            {
                if (this.EditSchedule != null
                    && this.ScheduleId.HasValue)
                {
                    this.EditSchedule(this, new EditScheduleEventArgs(this.ScheduleId.Value));
                }
            }
            else if (sender == _deleteScheduleItem)
            {
                if (this.DeleteSchedule != null
                    && this.ScheduleId.HasValue)
                {
                    this.DeleteSchedule(this, new EditScheduleEventArgs(this.ScheduleId.Value));
                }
            }
            else if (sender == _cancelProgramItem
                || sender == _uncancelProgramItem
                || sender == _abortRecordingItem)
            {
                if (this.CancelProgram != null
                    && this.ScheduleId.HasValue)
                {
                    this.CancelProgram(this, new CancelProgramEventArgs(sender != _uncancelProgramItem, this.ScheduleId.Value,
                        _guideProgramId, _channel.ChannelId, _startTime));
                }
            }
            else if (sender == _addToPreviouslyRecordedHistoryItem
                || sender == _removeFromPreviouslyRecordedHistoryItem)
            {
                if (this.AddRemoveProgramHistory != null
                    && _upcomingProgram != null)
                {
                    this.AddRemoveProgramHistory(this,
                        new AddRemoveProgramHistoryEventArgs(sender == _addToPreviouslyRecordedHistoryItem, _upcomingProgram));
                }
            }
            else if (sender == _recordItem)
            {
                schedule = GuideController.CreateRecordOnceSchedule(_channel.ChannelType, _channel.ChannelId, _title, _subTitle, _episodeNumberDisplay, _startTime);
            }
            else if (sender == _recordDailyItem)
            {
                schedule = GuideController.CreateRecordRepeatingSchedule(GuideController.RepeatingType.Daily, _channel.ChannelType, _channel.ChannelId, _title, _startTime);
            }
            else if (sender == _recordWeeklyItem)
            {
                schedule = GuideController.CreateRecordRepeatingSchedule(GuideController.RepeatingType.Weekly, _channel.ChannelType, _channel.ChannelId, _title, _startTime);
            }
            else if (sender == _recordAnyTimeItem)
            {
                schedule = GuideController.CreateRecordRepeatingSchedule(GuideController.RepeatingType.AnyTime, _channel.ChannelType, _channel.ChannelId, _title, _startTime);
            }
            else if (sender == _playWithVlcItem)
            {
                if (this.PlayWithVlc != null
                    && _activeRecording != null)
                {
                    this.PlayWithVlc(this,
                        new PlayWithVlcEventArgs(_activeRecording.RecordingFileName));
                }
            }
            else if (((ToolStripMenuItem)sender).OwnerItem == _priorityScheduleItem)
            {
                UpcomingProgramPriority? newPriority = null;
                if (sender != _resetToSchedulePriorityItem)
                {
                    newPriority = (UpcomingProgramPriority)((ToolStripMenuItem)sender).Tag;
                }
                if (this.SetProgramPriority != null
                    && _upcomingProgram != null)
                {
                    this.SetProgramPriority(this,
                        new SetProgramPriorityEventArgs(_upcomingProgram.UpcomingProgramId, _startTime, newPriority));
                }
            }
            else if (((ToolStripMenuItem)sender).OwnerItem == _preRecordScheduleItem
                || ((ToolStripMenuItem)sender).OwnerItem == _postRecordScheduleItem)
            {
                int? seconds = null;
                if (sender != _resetToSchedulePreRecordItem
                    && sender != _resetToSchedulePostRecordItem)
                {
                    seconds = (int)((ToolStripMenuItem)sender).Tag;
                }
                if (this.SetProgramPrePostRecord != null
                    && _upcomingProgram != null)
                {
                    this.SetProgramPrePostRecord(this,
                        new SetProgramPrePostRecordEventArgs(_upcomingProgram.UpcomingProgramId, _startTime,
                            ((ToolStripMenuItem)sender).OwnerItem == _preRecordScheduleItem, seconds));
                }
            }
            if (schedule != null
                && this.CreateNewSchedule != null)
            {
                this.CreateNewSchedule(this, new CreateNewScheduleEventArgs(schedule));
            }
        }

        #region Menu Item Methods

        private ToolStripSeparator AddSeparator(ToolStripItemCollection items)
        {
            ToolStripSeparator separator = new ToolStripSeparator();
            separator.Size = new Size(_menuWidth - 3, 6);
            items.Add(separator);
            return separator;
        }

        private ToolStripMenuItem AddItem(ToolStripItemCollection items, string text)
        {
            return AddItem(items, null, text, null);
        }

        private ToolStripMenuItem AddItem(ToolStripItemCollection items, string text, object tag)
        {
            return AddItem(items, null, text, tag);
        }

        private ToolStripMenuItem AddItem(ToolStripItemCollection items, Icon icon, string text)
        {
            return AddItem(items, icon, text, null);
        }

        private ToolStripMenuItem AddItem(ToolStripItemCollection items, Icon icon, string text, object tag)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            if (icon != null
                && !_isTerminalServicesSession)
            {
                item.Image = icon.ToBitmap();
                item.ImageScaling = ToolStripItemImageScaling.None;
            }
            item.Size = new Size(_menuWidth, 22);
            item.Tag = tag;
            item.Click += new EventHandler(item_Click);
            items.Add(item);
            return item;
        }

        #endregion
    }
}
