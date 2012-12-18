#region Copyright (C) 2007-2012 ARGUS TV
/*
 *	Copyright (C) 2007-2012 ARGUS TV
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
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;
using ArgusTV.UI.Process;

namespace ArgusTV.UI.MediaPortal
{
    public class ManualSchedule: GUIWindow
    {
        [SkinControlAttribute(3)] protected GUIToggleButtonControl _mondayButton;
        [SkinControlAttribute(4)] protected GUIToggleButtonControl _tuesdayButton;
        [SkinControlAttribute(5)] protected GUIToggleButtonControl _wednesdayButton;
        [SkinControlAttribute(6)] protected GUIToggleButtonControl _thursdayButton;
        [SkinControlAttribute(7)] protected GUIToggleButtonControl _fridayButton;
        [SkinControlAttribute(8)] protected GUIToggleButtonControl _saturdayButton;
        [SkinControlAttribute(9)] protected GUIToggleButtonControl _sundayButton;
        [SkinControlAttribute(10)] protected GUIButtonControl _PriorityButton;
        [SkinControlAttribute(11)] protected GUIButtonControl _RecordButton;
        [SkinControlAttribute(12)] protected GUISpinControl _spinGroup; 
        [SkinControlAttribute(13)] protected GUISpinControl _spinChannel;
        [SkinControlAttribute(14)] protected GUISpinControl _spinStartMinute;
        [SkinControlAttribute(15)] protected GUISpinControl _spinStartHour;
        [SkinControlAttribute(16)] protected GUISpinControl _spinStartDay;
        [SkinControlAttribute(17)] protected GUISpinControl _spinStartMonth;
        [SkinControlAttribute(18)] protected GUISpinControl _spinStartYear;
        [SkinControlAttribute(19)] protected GUISpinControl _spinMinutesDuration;
        [SkinControlAttribute(20)] protected GUISpinControl _spinHoursDuration;
        [SkinControlAttribute(21)] protected GUIButtonControl _KeepButton;
        [SkinControlAttribute(22)] protected GUIButtonControl _AlertMeButton;
        [SkinControlAttribute(23)] protected GUILabelControl _StartTimeLabel;
        [SkinControlAttribute(24)] protected GUILabelControl _SelectDaysLabel;
        [SkinControlAttribute(25)] protected GUIButtonControl _ChangeNameButton;
        [SkinControlAttribute(26)] protected GUIListControl _upcomingEpsiodesList;
        [SkinControlAttribute(27)] protected GUILabelControl _upcomingEpisodesLabel;
        [SkinControlAttribute(28)] protected GUILabelControl _programTimeLabel;
        [SkinControlAttribute(29)] protected GUIFadeLabel _programTitleFadeLabel;

        private bool _recordOnce = true;

        static private UpcomingProgram _upcomingProgram = null;
        static public UpcomingProgram upcomingProgram
        {
            get { return _upcomingProgram; }
            set { _upcomingProgram = value; }
        }

        static private ChannelType _channelType = ChannelType.Television;
        static public ChannelType channelType
        {
            get { return _channelType; }
            set { _channelType = value; }
        }

        public ManualSchedule()
        {
            GetID = WindowId.ManualShedule;
        }

        #region Service Agents

        private SchedulerServiceAgent _tvSchedulerAgent;
        public ISchedulerService SchedulerAgent
        {
            get
            {
                if (_tvSchedulerAgent == null)
                {
                    _tvSchedulerAgent = new SchedulerServiceAgent();
                }
                return _tvSchedulerAgent;
            }
        }

        private ControlServiceAgent _tvControlAgent;
        public IControlService ControlAgent
        {
            get
            {
                if (_tvControlAgent == null)
                {
                    _tvControlAgent = new ControlServiceAgent();
                }
                return _tvControlAgent;
            }
        }

        #endregion

        #region overrides

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _upcomingEpisodesLabel.Label = Utility.GetLocalizedText(TextId.UpcomingRecordings);
            _ChangeNameButton.Label = Utility.GetLocalizedText(TextId.RenameShedule);
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\ARGUS_ManualSchedule.xml");
        }

        protected override void OnPageLoad()
        {
            PluginMain.EnsureConnection();
            base.OnPageLoad();

            if (PluginMain.IsConnected())
            {
                UpdateChannels();
                UpdateDateTime();
                UpdateButtonStates();
                UpdateDaysinMonth(Int32.Parse(_spinStartMonth.GetLabel()), Int32.Parse(_spinStartYear.GetLabel()));
                GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100748));
            }
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            if (_tvSchedulerAgent != null)
            {
                _tvSchedulerAgent.Dispose();
            }
            if (_tvControlAgent != null)
            {
                _tvControlAgent.Dispose();
            }
            base.OnPageDestroy(new_windowId);
        }

        protected override void OnClicked(int controlId, GUIControl control, global::MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (control == _spinGroup)
            {
                UpdateChannels();
            }
            else if (control == _spinStartMonth || control == _spinStartYear)
            {
                UpdateDaysinMonth(Int32.Parse(_spinStartMonth.GetLabel()), Int32.Parse(_spinStartYear.GetLabel()));
            }
            else if (control == _RecordButton)
            {
                Onrecord(ScheduleType.Recording);
                UpdateButtonStates();
            }
            else if (control == _AlertMeButton)
            {
                Onrecord(ScheduleType.Alert);
                UpdateButtonStates();
            }
            else if (control == _KeepButton)
            {
                OnKeep();
            }
            else if (control == _PriorityButton)
            {
                OnPriority();
            }
            else if (control == _mondayButton || control == _tuesdayButton || control == _wednesdayButton 
                || control == _thursdayButton || control == _fridayButton || control == _saturdayButton 
                || control == _sundayButton)
            {
                UpdateButtonStates();
            }
            else if (control == _ChangeNameButton)
            {
                OnChangeName();
            }
            base.OnClicked(controlId, control, actionType);
        }

        #endregion

        #region screen loading

        private void UpdateButtonStates()
        {
            Schedule sched = null;
            if (_upcomingProgram != null)
            {
                sched = SchedulerAgent.GetScheduleById(_upcomingProgram.ScheduleId);
            }

            if (_upcomingProgram != null
                && sched != null
                && sched.ScheduleType == ScheduleType.Recording
                && !_upcomingProgram.IsCancelled)
            {
                _RecordButton.Label = Utility.GetLocalizedText(TextId.DontRecord);
                _AlertMeButton.IsEnabled = false;
            }
            else
            {
                _RecordButton.Label = Utility.GetLocalizedText(TextId.Record);
                _AlertMeButton.IsEnabled = true;
            }

            if (_upcomingProgram != null
                && sched != null
                && sched.ScheduleType == ScheduleType.Alert
                && !_upcomingProgram.IsCancelled)
            {
                _AlertMeButton.Label = Utility.GetLocalizedText(TextId.CancelReminder);
            }
            else
            {
                _AlertMeButton.Label = Utility.GetLocalizedText(TextId.SetReminder);
            }

            _StartTimeLabel.Label = Utility.GetLocalizedText(TextId.OnDate);
            _SelectDaysLabel.Label = Utility.GetLocalizedText(TextId.SelectDays);

            if (_upcomingProgram != null && sched != null)
            {
                try
                {
                    ScheduleRule daysofweekrule = sched.Rules.FindRuleByType(ScheduleRuleType.DaysOfWeek);
                    if (daysofweekrule != null)
                    {
                        string daysofweek = daysofweekrule.Arguments[0].ToString();

                        if (daysofweek.Contains(ScheduleDaysOfWeek.Mondays.ToString()))
                        {
                            _mondayButton.Selected = true;
                        }
                        if (daysofweek.Contains(ScheduleDaysOfWeek.Tuesdays.ToString()))
                        {
                            _tuesdayButton.Selected = true;
                        }
                        if (daysofweek.Contains(ScheduleDaysOfWeek.Wednesdays.ToString()))
                        {
                            _wednesdayButton.Selected = true;
                        }
                        if (daysofweek.Contains(ScheduleDaysOfWeek.Thursdays.ToString()))
                        {
                            _thursdayButton.Selected = true;
                        }
                        if (daysofweek.Contains(ScheduleDaysOfWeek.Fridays.ToString()))
                        {
                            _fridayButton.Selected = true;
                        }
                        if (daysofweek.Contains(ScheduleDaysOfWeek.Saturdays.ToString()))
                        {
                            _saturdayButton.Selected = true;
                        }
                        if (daysofweek.Contains(ScheduleDaysOfWeek.Sundays.ToString()))
                        {
                            _sundayButton.Selected = true;
                        }
                    }
                }
                catch
                {
                    Log.Error("ManualSchedule: error updating select buttons");
                }

                _spinChannel.IsEnabled = false;
                _spinGroup.IsEnabled = false;
                _spinHoursDuration.IsEnabled = false;
                _spinMinutesDuration.IsEnabled = false;
                _spinStartDay.IsEnabled = false;
                _spinStartHour.IsEnabled = false;
                _spinStartMinute.IsEnabled = false;
                _spinStartMonth.IsEnabled = false;
                _spinStartYear.IsEnabled = false;

                _mondayButton.IsEnabled = false;
                _tuesdayButton.IsEnabled = false;
                _wednesdayButton.IsEnabled = false;
                _thursdayButton.IsEnabled = false;
                _fridayButton.IsEnabled = false;
                _saturdayButton.IsEnabled = false;
                _sundayButton.IsEnabled = false;

                if (sched.ScheduleType == ScheduleType.Recording
                    && !_upcomingProgram.IsCancelled)
                {
                    _KeepButton.IsEnabled = true;
                    _PriorityButton.IsEnabled = true;
                }
                else
                {
                    _KeepButton.IsEnabled = false;
                    _PriorityButton.IsEnabled = false;
                }

                if (_upcomingProgram.IsCancelled)
                {
                    _ChangeNameButton.IsEnabled = false;
                }
                else
                {
                    _ChangeNameButton.IsEnabled = true;
                }
            }
            else
            {
                _spinChannel.IsEnabled = true;
                _spinGroup.IsEnabled = true;
                _spinHoursDuration.IsEnabled = true;
                _spinMinutesDuration.IsEnabled = true;
                _spinStartDay.IsEnabled = true;
                _spinStartHour.IsEnabled = true;
                _spinStartMinute.IsEnabled = true;
                _spinStartMonth.IsEnabled = true;
                _spinStartYear.IsEnabled = true;

                _mondayButton.IsEnabled = true;
                _tuesdayButton.IsEnabled = true;
                _wednesdayButton.IsEnabled = true;
                _thursdayButton.IsEnabled = true;
                _fridayButton.IsEnabled = true;
                _saturdayButton.IsEnabled = true;
                _sundayButton.IsEnabled = true;

                _KeepButton.IsEnabled = false;
                _PriorityButton.IsEnabled = false;
                _ChangeNameButton.IsEnabled = false;
            }

            if (_mondayButton.Selected || _tuesdayButton.Selected || _wednesdayButton.Selected
                || _thursdayButton.Selected || _fridayButton.Selected || _saturdayButton.Selected
                || _sundayButton.Selected)
            {
                _StartTimeLabel.Label = Utility.GetLocalizedText(TextId.FromDate);
                _recordOnce = false;
            }
            else
            {
                _StartTimeLabel.Label = Utility.GetLocalizedText(TextId.OnDate);
                _recordOnce = true;
            }

            Update(sched);
        }

        private void Update(Schedule _schedule)
        {
            _upcomingEpsiodesList.Clear();
            if (_programTimeLabel != null && _programTitleFadeLabel != null)
            {
                if (_upcomingProgram != null)
                {
                    string strTime = String.Format("{0} {1} - {2}",
                        Utility.GetShortDayDateString(_upcomingProgram.StartTime),
                        _upcomingProgram.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                        _upcomingProgram.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                    _programTimeLabel.Label = strTime;
                    _programTitleFadeLabel.Label = _upcomingProgram.Title;
                }
                else
                {
                    _programTimeLabel.Label = string.Empty;
                    _programTitleFadeLabel.Label = string.Empty;
                }
            }

            if (_schedule != null)
            {
                if (_schedule.ScheduleType == ScheduleType.Recording)
                {
                    UpcomingRecording[] recordings = ControlAgent.GetUpcomingRecordings(_schedule.ScheduleId, true);
                    foreach (UpcomingRecording recording in recordings)
                    {
                        GUIListItem item = new GUIListItem();
                        string title = recording.Title;
                        item.Label = title;
                        string logoImagePath = Utility.GetLogoImage(recording.Program.Channel, SchedulerAgent);
                        if (logoImagePath == null
                            || !System.IO.File.Exists(logoImagePath))
                        {
                            item.Label = String.Format("[{0}] {1}", recording.Program.Channel.DisplayName, title);
                            logoImagePath = "defaultVideoBig.png";
                        }

                        item.PinImage = Utility.GetIconImageFileName(ScheduleType.Recording, recording.Program.IsPartOfSeries,
                        recording.Program.CancellationReason, recording);

                        item.TVTag = recording;
                        item.ThumbnailImage = logoImagePath;
                        item.IconImageBig = logoImagePath;
                        item.IconImage = logoImagePath;
                        item.Label2 = String.Format("{0} {1} - {2}",
                                                  Utility.GetShortDayDateString(recording.StartTime),
                                                  recording.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                                  recording.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                        _upcomingEpsiodesList.Add(item);
                    }
                }
                else
                {
                    UpcomingProgram[] _progs = SchedulerAgent.GetUpcomingPrograms(_schedule, true);
                    foreach (UpcomingProgram program in _progs)
                    {
                        GUIListItem item = new GUIListItem();
                        string title = program.Title;
                        item.Label = title;
                        //item.OnItemSelected += new global::MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
                        string logoImagePath = Utility.GetLogoImage(program.Channel, SchedulerAgent);
                        if (logoImagePath == null
                            || !System.IO.File.Exists(logoImagePath))
                        {
                            item.Label = String.Format("[{0}] {1}", program.Channel.DisplayName, title);
                            logoImagePath = "defaultVideoBig.png";
                        }

                        if (_schedule.ScheduleType == ScheduleType.Alert)
                        {
                            item.PinImage = item.PinImage = Utility.GetIconImageFileName(ScheduleType.Alert, program.IsPartOfSeries,
                                program.CancellationReason, null);
                        }
                        item.TVTag = program;
                        item.ThumbnailImage = logoImagePath;
                        item.IconImageBig = logoImagePath;
                        item.IconImage = logoImagePath;
                        item.Label2 = String.Format("{0} {1} - {2}",
                                                  Utility.GetShortDayDateString(program.StartTime),
                                                  program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                                  program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                        _upcomingEpsiodesList.Add(item);
                    }
                }
            }
            _upcomingEpisodesLabel.IsVisible = (_upcomingEpsiodesList != null && _upcomingEpsiodesList.Count > 0);
        }

        private void UpdateChannels()
        {
            Schedule schedule = null;
            bool channelFound = false;
            int channelIndex = 0;
            int groupIndex = 0;

            if (_upcomingProgram != null)
            {
                schedule = SchedulerAgent.GetScheduleById(_upcomingProgram.ScheduleId);
                if (schedule != null)
                {
                    _channelType = _upcomingProgram.Channel.ChannelType;
                }
            }

            List<ChannelGroup> groups = new List<ChannelGroup>(PluginMain.Navigator.GetGroups(_channelType));
            if (_spinGroup.GetLabel() == "" || _spinGroup.GetLabel() == GUILocalizeStrings.Get(2014))
            {
                _spinGroup.Reset();
                foreach (ChannelGroup group in groups)
                {
                    _spinGroup.AddLabel(group.GroupName, 0);
                }
            }

            //based on name but I don't know anything better
            string groupName = _spinGroup.GetLabel();
            _spinChannel.Reset();

            foreach (ChannelGroup group in groups)
            {
                if (group.GroupName == groupName || (channelFound == false && schedule != null))
                {
                    foreach (Channel chan in SchedulerAgent.GetChannelsInGroup(group.ChannelGroupId, true))
                    {
                        _spinChannel.AddLabel(chan.DisplayName, 0);
                        if (!channelFound && schedule != null)
                        {
                            if (_upcomingProgram.Channel.ChannelId == chan.ChannelId)
                            {
                                channelFound = true;
                            }
                            channelIndex++;
                        }
                    }
                }
                if (!channelFound)
                {
                    groupIndex++;
                }
            }

            if (!channelFound && schedule != null)
            {
                // channel not found in groups, create a "unknown group" for this channel
                _spinGroup.AddLabel(GUILocalizeStrings.Get(2014), 0);
                _spinGroup.Value = groupIndex;
                _spinChannel.Reset();
                _spinChannel.AddLabel(_upcomingProgram.Channel.DisplayName, 0);
            }

            if (channelFound)
            {
                _spinChannel.Value = channelIndex - 1;
                _spinGroup.Value = groupIndex;
            }
        }

        private void UpdateDateTime()
        {
            DateTime time = DateTime.Now;
            Schedule sched = null;
            ScheduleRule _manualRule = null;
            int valueYear = 0;
            bool yearFound = true;

            if (_upcomingProgram != null)
            {
                sched = SchedulerAgent.GetScheduleById(_upcomingProgram.ScheduleId);
                if (sched != null)
                {
                    yearFound = false;
                    _manualRule = sched.Rules.FindRuleByType(ScheduleRuleType.ManualSchedule);
                    if (_manualRule != null)
                    {
                        time = Convert.ToDateTime(_manualRule.Arguments[0]);
                    }
                }
            }

            for (int i = DateTime.Now.Year; i <= DateTime.Now.Year + 2; i++)
            {
                _spinStartYear.AddLabel(i.ToString(), 0);
                if (_upcomingProgram != null && sched != null && yearFound == false)
                {
                    if (i.ToString() ==  time.Year.ToString())
                    {
                        yearFound = true;
                    }
                    valueYear++;
                }
            }

            if (yearFound == false)
            {
                // year not found in list, add year in list
                _spinStartYear.AddLabel(time.ToString(), 0);
                yearFound = true;
                valueYear++;
            }

            for (int i = 1; i <= 12; i++)
            {
                if (i < 10)
                    _spinStartMonth.AddLabel("0" + i.ToString(), 0);
                else
                    _spinStartMonth.AddLabel(i.ToString(), 0);
            }

            UpdateDaysinMonth(Int32.Parse(_spinStartMonth.GetLabel()), Int32.Parse(_spinStartYear.GetLabel()));

            for (int i = 0; i <= 23; i++)
            {
                if (i < 10)
                {
                    _spinStartHour.AddLabel("0" + i.ToString(), 0);
                    _spinHoursDuration.AddLabel("0" + i.ToString(), 0);
                }
                else
                {
                    _spinStartHour.AddLabel(i.ToString(), 0);
                    _spinHoursDuration.AddLabel(i.ToString(), 0);
                }
            }

            for (int i = 0; i <= 59; i++)
            {
                if (i < 10)
                {
                    _spinStartMinute.AddLabel("0" + i.ToString(), 0);
                    _spinMinutesDuration.AddLabel("0" + i.ToString(), 0);
                }
                else
                {
                    _spinStartMinute.AddLabel(i.ToString(), 0);
                    _spinMinutesDuration.AddLabel(i.ToString(), 0);
                }
            }

            _spinStartMonth.Value = (time.Month - 1);
            _spinStartDay.Value = (time.Day - 1);
            _spinStartHour.Value = time.Hour;
            _spinStartMinute.Value = time.Minute;

            if (_upcomingProgram != null && sched != null && _manualRule != null)
            {
                if (yearFound)
                {
                    _spinStartYear.Value = (valueYear - 1);
                }
                _spinHoursDuration.Value = _upcomingProgram.Duration.Hours;
                _spinMinutesDuration.Value = _upcomingProgram.Duration.Minutes;
            }
            else
            {
                //default duration = 1 hour
                _spinHoursDuration.Value = 1;
                _spinMinutesDuration.Value = 0;
            }
        }

        private void UpdateDaysinMonth(int month, int year)
        {
            int selected = 0;
            if (_spinStartDay.GetLabel() != "")
            {
                selected = Int32.Parse(_spinStartDay.GetLabel());
            }
            _spinStartDay.Reset();
            for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
            {
                if (i < 10)
                {
                    _spinStartDay.AddLabel("0" + i.ToString(), 0);
                }
                else
                {
                    _spinStartDay.AddLabel(i.ToString(), 0);
                }
                
                if (i == selected)
                {
                    _spinStartDay.Value = (i - 1);
                }
            }
        }

        #endregion

        #region create/edit schedule

        private void Onrecord(ScheduleType scheduleType)
        {
            Schedule schedule = null;
            if (_upcomingProgram != null)
            {
                schedule = SchedulerAgent.GetScheduleById(_upcomingProgram.ScheduleId);
            }
            if (_upcomingProgram != null && schedule != null
                && schedule.ScheduleType == scheduleType)//delete schedule
            {
                if (_upcomingProgram.IsCancelled)
                {
                    SchedulerAgent.UncancelUpcomingProgram(_upcomingProgram.ScheduleId, _upcomingProgram.GuideProgramId, _upcomingProgram.Channel.ChannelId, _upcomingProgram.StartTime);
                    try
                    {
                        //refresh _upcomingProgram
                        _upcomingProgram = SchedulerAgent.GetUpcomingPrograms(schedule, true)[0];
                    }
                    catch { }
                }
                else
                {
                    if (_upcomingProgram.IsPartOfSeries)
                    {
                        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        if (dlgYesNo != null)
                        {
                            dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.DeleteEntireSchedule));
                            dlgYesNo.SetLine(1, "\"" + schedule.Name + "\"");
                            dlgYesNo.SetLine(2, Utility.GetLocalizedText(TextId.AreYouSure));
                            dlgYesNo.SetLine(3, String.Empty);
                            dlgYesNo.SetDefaultToYes(false);
                            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                            if (dlgYesNo.IsConfirmed)
                            {
                                SchedulerAgent.DeleteSchedule(schedule.ScheduleId);
                                _upcomingProgram = null;
                            }
                        }
                    }
                    else
                    {
                        SchedulerAgent.DeleteSchedule(_upcomingProgram.ScheduleId);
                        _upcomingProgram = null;
                    }
                }
            }
            else//create new schedule
            {
                TimeSpan duration = new TimeSpan(Int32.Parse(_spinHoursDuration.GetLabel()), Int32.Parse(_spinMinutesDuration.GetLabel()), 0);
                DateTime startTime = new DateTime(Int32.Parse(_spinStartYear.GetLabel()), Int32.Parse(_spinStartMonth.GetLabel()), Int32.Parse(_spinStartDay.GetLabel()), Int32.Parse(_spinStartHour.GetLabel()), Int32.Parse(_spinStartMinute.GetLabel()), 0);
                ScheduleDaysOfWeek daysOfWeek = new ScheduleDaysOfWeek();

                //TODO: What if we have multiple channels with the same name
                Channel channel = SchedulerAgent.GetChannelByDisplayName(_channelType, _spinChannel.GetLabel());

                Schedule newSchedule = null;
                newSchedule = SchedulerAgent.CreateNewSchedule(_channelType, scheduleType);
                newSchedule.Rules.Add(ScheduleRuleType.Channels, channel.ChannelId);
                newSchedule.Rules.Add(ScheduleRuleType.ManualSchedule, startTime, new ScheduleTime(duration));
                if (!_recordOnce)
                {
                    string days = " ";
                    if (_mondayButton.Selected)
                    {
                        daysOfWeek = daysOfWeek | ScheduleDaysOfWeek.Mondays;
                        days += Utility.GetLocalizedText(TextId.Mon) + ",";
                    }
                    if (_tuesdayButton.Selected)
                    {
                        daysOfWeek = daysOfWeek | ScheduleDaysOfWeek.Tuesdays;
                        days += Utility.GetLocalizedText(TextId.Tue) + ",";
                    }
                    if (_wednesdayButton.Selected)
                    {
                        daysOfWeek = daysOfWeek | ScheduleDaysOfWeek.Wednesdays;
                        days += Utility.GetLocalizedText(TextId.Wed) + ",";
                    }
                    if (_thursdayButton.Selected)
                    {
                        daysOfWeek = daysOfWeek | ScheduleDaysOfWeek.Thursdays;
                        days += Utility.GetLocalizedText(TextId.Thu) + ",";
                    }
                    if (_fridayButton.Selected)
                    {
                        daysOfWeek = daysOfWeek | ScheduleDaysOfWeek.Fridays;
                        days += Utility.GetLocalizedText(TextId.Fri) + ",";
                    }
                    if (_saturdayButton.Selected)
                    {
                        daysOfWeek = daysOfWeek | ScheduleDaysOfWeek.Saturdays;
                        days += Utility.GetLocalizedText(TextId.Sat) + ",";
                    }
                    if (_sundayButton.Selected)
                    {
                        daysOfWeek = daysOfWeek | ScheduleDaysOfWeek.Sundays;
                        days += Utility.GetLocalizedText(TextId.Sun) + ",";
                    }
                    days = days.Remove(days.Length - 1);
                    newSchedule.Rules.Add(ScheduleRuleType.DaysOfWeek, daysOfWeek);
                    newSchedule.Name = String.Format(CultureInfo.CurrentCulture, "{0} {1} {2:t}-{3:t}", channel.DisplayName, days, startTime, startTime.Add(duration));
                }
                else
                {
                    newSchedule.Name = String.Format(CultureInfo.CurrentCulture, "{0} {1:g}-{2:t}", channel.DisplayName, startTime, startTime.Add(duration));
                }

                //TODO: try to prevent dublicate manual schedules
                //and find a better way to get the newly created "_schedule" and  "_upcomingProgram"
                if (newSchedule != null)
                {
                    newSchedule.ScheduleType = scheduleType;
                    SchedulerAgent.SaveSchedule(newSchedule);

                    bool found = false;
                    UpcomingProgram[] _programs = SchedulerAgent.GetAllUpcomingPrograms(scheduleType, true);
                    foreach (UpcomingProgram _prog in _programs)
                    {
                        if (_prog.Channel.ChannelId == channel.ChannelId
                        && _prog.Duration == duration
                        && !found)
                        {
                            Schedule _schedule = SchedulerAgent.GetScheduleById(_prog.ScheduleId);
                            if (_schedule.Rules.FindRuleByType(ScheduleRuleType.ManualSchedule) != null)
                            {
                                if (_schedule.Name == newSchedule.Name)
                                {
                                    if (_recordOnce
                                        && _prog.StartTime == startTime)
                                    {
                                        _upcomingProgram = _prog;
                                        found = true;
                                    }
                                    else if (!_recordOnce && _schedule.Rules.FindRuleByType(ScheduleRuleType.DaysOfWeek) != null
                                        && _schedule.Rules.FindRuleByType(ScheduleRuleType.DaysOfWeek).Arguments[0].ToString() == daysOfWeek.ToString()
                                        && Convert.ToDateTime(_schedule.Rules.FindRuleByType(ScheduleRuleType.ManualSchedule).Arguments[0]) == startTime)
                                    {
                                        _upcomingProgram = _prog;
                                        found = true;
                                    }
                                    Update(_schedule);
                                    break;
                                }
                            }
                        }
                    }

                    if (!found)
                    {
                        GUIWindowManager.ShowPreviousWindow();
                    }
                }
            }
        }

        private void OnKeep()
        {
            if (_upcomingProgram != null)
            {
                Schedule schedule = SchedulerAgent.GetScheduleById(_upcomingProgram.ScheduleId);
                if (schedule == null) return;

                GUIDialogMenu dialog = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dialog == null) return;
                dialog.Reset();

                dialog.SetHeading(1042); // Keep until
                dialog.Add(Utility.GetLocalizedText(TextId.UntilSpaceNeeded));
                dialog.Add(Utility.GetLocalizedText(TextId.NumberOfDays));
                dialog.Add(Utility.GetLocalizedText(TextId.NumberOfEpisodes));
                dialog.Add(Utility.GetLocalizedText(TextId.NumberOfWatchedEpisodes));
                dialog.Add(Utility.GetLocalizedText(TextId.Forever)); 

                switch (schedule.KeepUntilMode)
                {
                    case KeepUntilMode.UntilSpaceIsNeeded:
                        dialog.SelectedLabel = 0;
                        break;
                    case KeepUntilMode.NumberOfDays:
                        dialog.SelectedLabel = 1;
                        break;
                    case KeepUntilMode.NumberOfEpisodes:
                        dialog.SelectedLabel = 2;
                        break;
                    case KeepUntilMode.NumberOfWatchedEpisodes:
                        dialog.SelectedLabel = 3;
                        break;
                    case KeepUntilMode.Forever:
                        dialog.SelectedLabel = 4;
                        break;
                }

                dialog.DoModal(GetID);
                if (dialog.SelectedId == -1) return;

                switch (dialog.SelectedLabel)
                {
                    case 0:
                        schedule.KeepUntilMode = KeepUntilMode.UntilSpaceIsNeeded;
                        break;

                    case 1:
                        {
                            int? value = GetKeepValue(1045, KeepUntilMode.NumberOfDays, 3014, 3015,
                                schedule.KeepUntilMode == KeepUntilMode.NumberOfDays ? schedule.KeepUntilValue : 7);
                            if (value.HasValue)
                            {
                                schedule.KeepUntilMode = KeepUntilMode.NumberOfDays;
                                schedule.KeepUntilValue = value;
                            }
                        }
                        break;

                    case 2:
                        {
                            int? value = GetKeepValue(887, KeepUntilMode.NumberOfEpisodes, 682, 914,
                                schedule.KeepUntilMode == KeepUntilMode.NumberOfEpisodes ? schedule.KeepUntilValue : 3);
                            if (value.HasValue)
                            {
                                schedule.KeepUntilMode = KeepUntilMode.NumberOfEpisodes;
                                schedule.KeepUntilValue = value;
                            }
                        }
                        break;

                    case 3:
                        {
                            int? value = GetKeepValue(887, KeepUntilMode.NumberOfWatchedEpisodes, 682, 914,
                                schedule.KeepUntilMode == KeepUntilMode.NumberOfWatchedEpisodes ? schedule.KeepUntilValue : 3);
                            if (value.HasValue)
                            {
                                schedule.KeepUntilMode = KeepUntilMode.NumberOfWatchedEpisodes;
                                schedule.KeepUntilValue = value;
                            }
                        }
                        break;

                    case 4:
                        schedule.KeepUntilMode = KeepUntilMode.Forever;
                        break;
                }

                if (schedule != null)
                {
                    SchedulerAgent.SaveSchedule(schedule);
                }
            }
        }

        private int? GetKeepValue(int heading, KeepUntilMode mode, int singularName, int pluralName, int? currentValue)
        {
            DataTable valueTable = KeepUntilControlUtility.CreateValueTable(mode, currentValue);
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.ShowQuickNumbers = false;
                dlg.SetHeading(heading);

                string singularValue = "1 " + GUILocalizeStrings.Get(singularName);
                string pluralSuffix = " " + GUILocalizeStrings.Get(pluralName);

                foreach (DataRow value in valueTable.Rows)
                {
                    int val = (int)value[KeepUntilControlUtility.ValueColumnName];
                    if (val == 1)
                    {
                        dlg.Add(singularValue);
                    }
                    else
                    {
                        dlg.Add(value[KeepUntilControlUtility.TextColumnName] + pluralSuffix);
                    }
                }

                dlg.SelectedLabel = KeepUntilControlUtility.GetIndexToSelect(valueTable, currentValue);
                dlg.DoModal(GetID);
                if (dlg.SelectedLabel >= 0)
                {
                    return (int)valueTable.Rows[dlg.SelectedLabel][KeepUntilControlUtility.ValueColumnName];
                }
            }
            return null;
        }

        private void OnPriority()
        {
            if (_upcomingProgram != null)
            {
                Schedule schedule = SchedulerAgent.GetScheduleById(_upcomingProgram.ScheduleId);
                if (schedule == null) return;
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg != null)
                {
                    dlg.Reset();
                    dlg.SetHeading(Utility.GetLocalizedText(TextId.RecordingsListItemsSuffix));
                    dlg.Add(Utility.GetLocalizedText(TextId.VeryLow));
                    dlg.Add(Utility.GetLocalizedText(TextId.Low));
                    dlg.Add(Utility.GetLocalizedText(TextId.Normal));
                    dlg.Add(Utility.GetLocalizedText(TextId.High));
                    dlg.Add(Utility.GetLocalizedText(TextId.VeryHigh));
                    dlg.SelectedLabel = (int)schedule.SchedulePriority + 2;
                    dlg.DoModal(GetID);
                    if (dlg.SelectedLabel >= 0)
                    {
                        schedule.SchedulePriority = (SchedulePriority)(dlg.SelectedLabel - 2);
                        SchedulerAgent.SaveSchedule(schedule);
                    }
                }
            }
        }

        private void OnChangeName()
        {
            if (_upcomingProgram != null)
            {
                Schedule schedule = SchedulerAgent.GetScheduleById(_upcomingProgram.ScheduleId);
                if (schedule == null) return;
                string schedname = schedule.Name;
                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (keyboard != null)
                {
                    keyboard.Reset();
                    keyboard.IsSearchKeyboard = false;
                    keyboard.Text = schedname ?? String.Empty;
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        schedule.Name = keyboard.Text;
                        SchedulerAgent.SaveSchedule(schedule);
                    }
                }
            }
        }
        #endregion
    }
}
