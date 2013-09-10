#region Copyright (C) 2005-2013 Team MediaPortal

/* 
 *	Copyright (C) 2005-2013 Team MediaPortal
 *	http://www.team-mediaportal.com
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

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Data;

using MediaPortal.Dialogs;
using MediaPortal.Util;
using MediaPortal.GUI.Library;
using Action = MediaPortal.GUI.Library.Action;

using ArgusTV.DataContracts;
using ArgusTV.ServiceContracts;
using ArgusTV.ServiceAgents;
using ArgusTV.UI.Process.Guide;
using ArgusTV.UI.Process;

namespace ArgusTV.UI.MediaPortal
{
    /// <summary>
    /// Summary description for GUITVProgramInfo.
    /// </summary>
    public class TvProgramInfo : GUIWindow
    {
        private List<int> _recordingIntervalValues = new List<int>();

        [SkinControl(17)] protected GUILabelControl _categoryLabel;
        [SkinControl(15)] protected GUITextScrollUpControl _descriptionScrollUp;
        [SkinControl(14)] protected GUILabelControl _programTimeLabel;
        [SkinControl(13)] protected GUIFadeLabel _programTitleFadeLabel;
        [SkinControl(16)] protected GUIFadeLabel _programChannelFadeLabel;
        [SkinControl(2)] protected GUIButtonControl _recordButton;
        [SkinControl(3)] protected GUIButtonControl _priorityButton;
        [SkinControl(4)] protected GUIButtonControl _keepButton;
        [SkinControl(5)] protected GUIToggleButtonControl _onlyNewEpisodesToggleButton;
        [SkinControl(11)] protected GUILabelControl _upcomingEpisodesLabel;
        [SkinControl(10)] protected GUIListControl _upcomingEpsiodesList;
        [SkinControl(8)] protected GUIButtonControl _preRecordButton;
        [SkinControl(9)] protected GUIButtonControl _postRecordButton;
        [SkinControl(6)] protected GUIButtonControl _alertMeButton;
        [SkinControl(18)] protected GUIButtonControl _changeNameButton;

        private GuideProgram _ProgramToShow = null;
        private int m_iSelectedItem = 0;

        static private GuideProgram _currentProgram;
        static public GuideProgram CurrentProgram
        {
            get { return _currentProgram; }
            set { _currentProgram = value; }
        }

        static private Channel _channel;
        static public Channel Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        public TvProgramInfo()
        {
            GetID = (int)WindowId.ProgramInfo;

            //Fill the list with all available pre & post intervals
            _recordingIntervalValues.Add(0);
            _recordingIntervalValues.Add(1);
            _recordingIntervalValues.Add(2);
            _recordingIntervalValues.Add(3);
            _recordingIntervalValues.Add(4);
            _recordingIntervalValues.Add(5);
            _recordingIntervalValues.Add(7);
            _recordingIntervalValues.Add(10);
            _recordingIntervalValues.Add(15);
            _recordingIntervalValues.Add(20);
            _recordingIntervalValues.Add(30);
            _recordingIntervalValues.Add(45);
            _recordingIntervalValues.Add(60);
            _recordingIntervalValues.Add(90);
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

        private GuideServiceAgent _tvGuideAgent;
        public IGuideService GuideAgent
        {
            get
            {
                if (_tvGuideAgent == null)
                {
                    _tvGuideAgent = new GuideServiceAgent();
                }
                return _tvGuideAgent;
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

        #region Overrides

        public override bool IsTv
        {
            get { return true; }
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\ARGUS_ProgramInfo2.xml");
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _priorityButton.Label = Utility.GetLocalizedText(TextId.Priority);
            _keepButton.Label = Utility.GetLocalizedText(TextId.Keep);
            _onlyNewEpisodesToggleButton.Label = Utility.GetLocalizedText(TextId.OnlyNewEpisodes);
            _preRecordButton.Label = Utility.GetLocalizedText(TextId.PreRecord);
            _postRecordButton.Label = Utility.GetLocalizedText(TextId.PostRecord);
            _upcomingEpisodesLabel.Label = Utility.GetLocalizedText(TextId.UpcomingPrograms);
            _alertMeButton.Label = Utility.GetLocalizedText(TextId.SetReminder);
            _recordButton.Label = Utility.GetLocalizedText(TextId.Record);
            if (_changeNameButton != null)
            {
                _changeNameButton.Label = Utility.GetLocalizedText(TextId.RenameShedule);
            }
        }

        protected override void OnPageLoad()
        {
            PluginMain.EnsureConnection();
            base.OnPageLoad();
            if (PluginMain.IsConnected())
            {
                Update();
                GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100748));
            }
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            if (_tvSchedulerAgent != null)
            {
                _tvSchedulerAgent.Dispose();
            }
            if (_tvGuideAgent != null)
            {
                _tvGuideAgent.Dispose();
            }
            if (_tvControlAgent != null)
            {
                _tvControlAgent.Dispose();
            }
            base.OnPageDestroy(newWindowId);
        }

        protected override void OnClicked(int controlId, GUIControl control, global::MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            if (control == _recordButton)
            {
                OnRecordProgram();
            }
            else if (control == _alertMeButton)
            {
                OnAlertProgram();
            }
            else if (control == _priorityButton)
            {
                OnPriority();
            }
            else if (control == _keepButton)
            {
                OnKeep();
            }
            else if (control == _preRecordButton)
            {
                OnPreRecordInterval();
            }
            else if (control == _postRecordButton)
            {
                OnPostRecordInterval();
            }
            else if (control == _onlyNewEpisodesToggleButton)
            {
                OnNewEpisodesToggle(_onlyNewEpisodesToggleButton.Selected);
            }
            else if (control == _changeNameButton)
            {
                OnRenameSchedule();
            }
            else if (control == _upcomingEpsiodesList)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, control.GetID, 0, 0, null);
                OnMessage(msg);
                int iItem = (int)msg.Param1;
                if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
                {
                    OnSelectItem(iItem);
                }
            }
            base.OnClicked(controlId, control, actionType);
        }

        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
                    UpdateLabels();
                    break;
            }
            return base.OnMessage(message);
        }

        #endregion

        #region Private methods

        private void UpdateLabels()
        {
            try
            {
                if (!_upcomingEpsiodesList.IsFocused)
                {
                    _upcomingEpsiodesList.SelectedListItemIndex = m_iSelectedItem;
                }

                GUIListItem pItem = GetItem(GetSelectedItemNo());
                if (pItem == null)
                {
                    SetLabels(null);
                    return;
                }
                UpcomingProgram program = pItem.TVTag as UpcomingProgram;
                if (program == null)
                {
                    SetLabels(null);
                    return;
                }
                SetLabels(program);
            }
            catch (Exception ex)
            {
                Log.Error("TVProgammInfo: Error updating properties - {0}", ex.ToString());
            }
        }

        private void SetLabels(UpcomingProgram program)
        {
            //set program description
            if (_currentProgram == null) return;
            Channel _channel = Channel;

            if (program != null && program.GuideProgramId.HasValue)
            {
                if (_ProgramToShow != null && _ProgramToShow.GuideProgramId == program.GuideProgramId.Value)
                {
                    return;
                }
                _ProgramToShow = GuideAgent.GetProgramById(program.GuideProgramId.Value);
                _channel = program.Channel;
            }
            else
            {
                _ProgramToShow = _currentProgram;
            }

            string strTime = String.Format("{0} {1} - {2}",
              Utility.GetShortDayDateString(_ProgramToShow.StartTime),
              _ProgramToShow.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
              _ProgramToShow.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

            if (_ProgramToShow.Category != null)
                _categoryLabel.Label = _ProgramToShow.Category;
            else
                _categoryLabel.Label = string.Empty;

            _programChannelFadeLabel.Label = _channel.DisplayName;
            _programTimeLabel.Label = strTime;
            _descriptionScrollUp.Label = _ProgramToShow.CreateCombinedDescription(true);
            _programTitleFadeLabel.Label = _ProgramToShow.Title;
        }

        private void Update()
        {
            _upcomingEpsiodesList.Clear();
            UpdateLabels();
            if (_currentProgram == null) return;
            
            Schedule scheduleProgram = null;
            Schedule schedule = null;
            UpcomingRecording upcomingRecording = null;
            UpcomingProgram upcomingProgram = null;

            if (GetUpcomingRecordingAndSchedule(_channel.ChannelId, _currentProgram, out upcomingRecording, out schedule))
            {
                bool cancelled = upcomingRecording.Program.IsCancelled;
                if (cancelled)
                    _recordButton.Label = Utility.GetLocalizedText(TextId.Record);
                else
                    _recordButton.Label = Utility.GetLocalizedText(TextId.DontRecord);

                bool isRecordOnce = (schedule.Rules.FindRuleByType(ScheduleRuleType.OnDate) != null);
                bool state = cancelled;
                if (!isRecordOnce) state = false;
                _onlyNewEpisodesToggleButton.Disabled = isRecordOnce;
                _alertMeButton.Disabled = true;
                _recordButton.Disabled = false;
                _preRecordButton.Disabled = state;
                _postRecordButton.Disabled = state;
                _priorityButton.Disabled = state;
                _keepButton.Disabled = state;

                if (_changeNameButton != null)
                {
                    _changeNameButton.Disabled = state;
                }
                if (isRecordOnce)
                {
                    _onlyNewEpisodesToggleButton.Selected = false;
                }
                else
                {
                    ScheduleRule newEpisodesRule = schedule.Rules.FindRuleByType(ScheduleRuleType.NewEpisodesOnly);
                    _onlyNewEpisodesToggleButton.Selected = (newEpisodesRule != null) ? (bool)newEpisodesRule.Arguments[0] : false;
                }
            }
            else if (GetUpcomingProgramAndSchedule(_channel.ChannelId, _currentProgram, out upcomingProgram, out scheduleProgram, ScheduleType.Alert))
            {
                bool cancelled = upcomingProgram.IsCancelled;
                if (cancelled)
                    _alertMeButton.Label = Utility.GetLocalizedText(TextId.SetReminder);
                else
                    _alertMeButton.Label = Utility.GetLocalizedText(TextId.CancelReminder);
                
                _recordButton.Label = Utility.GetLocalizedText(TextId.Record);
                bool isRecordOnce = (scheduleProgram.Rules.FindRuleByType(ScheduleRuleType.OnDate) != null);
                bool state = cancelled;
                if (!isRecordOnce) state = false;
                _onlyNewEpisodesToggleButton.Disabled = true;//isRecordOnce;
                //_recordButton.Disabled = true;
                _alertMeButton.Disabled = false;
                _preRecordButton.Disabled = true;
                _postRecordButton.Disabled = true;
                _priorityButton.Disabled = true;
                _keepButton.Disabled = true;
                _onlyNewEpisodesToggleButton.Selected = false;
                if (_changeNameButton != null)
                {
                    _changeNameButton.Disabled = state;
                }
            }

            if (upcomingProgram == null && upcomingRecording == null)
            {
                _recordButton.Label = Utility.GetLocalizedText(TextId.Record);
                _alertMeButton.Label = Utility.GetLocalizedText(TextId.SetReminder);
                _preRecordButton.Disabled = true;
                _postRecordButton.Disabled = true;
                _priorityButton.Disabled = true;
                _keepButton.Disabled = true;
                _alertMeButton.Disabled = false;
                _recordButton.Disabled = false;
                _onlyNewEpisodesToggleButton.Disabled = true;
                _onlyNewEpisodesToggleButton.Selected = false;
                if (_changeNameButton != null)
                {
                    _changeNameButton.Disabled = true;
                }
            }

            //find upcoming episodes
            _upcomingEpisodesLabel.IsVisible = (upcomingRecording != null || upcomingProgram != null);
            _upcomingEpsiodesList.Clear();
            int i = 0;

            if (upcomingRecording != null)
            {
                UpcomingRecording[] upcomingRecordings = ControlAgent.GetUpcomingRecordings(upcomingRecording.Program.ScheduleId, true);
                foreach (UpcomingRecording recording in upcomingRecordings)
                {
                    GUIListItem item = new GUIListItem();
                    string title = recording.Program.CreateProgramTitle();
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
                    item.TVTag = recording.Program;
                    item.ThumbnailImage = logoImagePath;
                    item.IconImageBig = logoImagePath;
                    item.IconImage = logoImagePath;
                    item.Label2 = String.Format("{0} {1} - {2}",
                                              Utility.GetShortDayDateString(recording.Program.StartTime),
                                              recording.Program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                              recording.Program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                    _upcomingEpsiodesList.Add(item);

                    if (recording.Program.GuideProgramId.HasValue
                        && _currentProgram.GuideProgramId == recording.Program.GuideProgramId.Value)
                    {
                        item.IsPlayed = true;
                        m_iSelectedItem = i;
                    }
                    i++;
                }
            }
            else if (upcomingProgram != null)
            {
                List<UpcomingProgram> upcomingPrograms = new List<UpcomingProgram>(
                        this.SchedulerAgent.GetUpcomingPrograms(scheduleProgram, true));
                foreach (UpcomingProgram program in upcomingPrograms)
                {
                    GUIListItem item = new GUIListItem();
                    string title = program.CreateProgramTitle();
                    item.Label = title;
                    string logoImagePath = Utility.GetLogoImage(program.Channel, SchedulerAgent);
                    if (logoImagePath == null
                        || !System.IO.File.Exists(logoImagePath))
                    {
                        item.Label = String.Format("[{0}] {1}", program.Channel.DisplayName, title);
                        logoImagePath = "defaultVideoBig.png";
                    }
                    item.PinImage = Utility.GetIconImageFileName(ScheduleType.Alert, program.IsPartOfSeries,
                        program.CancellationReason, null);
                    item.TVTag = program;
                    item.ThumbnailImage = logoImagePath;
                    item.IconImageBig = logoImagePath;
                    item.IconImage = logoImagePath;
                    item.Label2 = String.Format("{0} {1} - {2}",
                                              Utility.GetShortDayDateString(program.StartTime),
                                              program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                              program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                    _upcomingEpsiodesList.Add(item);

                    if (program.GuideProgramId.HasValue
                        && _currentProgram.GuideProgramId == program.GuideProgramId.Value)
                    {
                        item.IsPlayed = true;
                        m_iSelectedItem = i;
                    }
                    i++;
                }
            }

            if (_upcomingEpsiodesList != null && _upcomingEpsiodesList.Count == 1)
                _upcomingEpsiodesList[0].IsPlayed = false;

            if (_upcomingEpsiodesList != null && _upcomingEpsiodesList.Count > 0)
                GUIPropertyManager.SetProperty("#itemcount", _upcomingEpsiodesList.Count.ToString());
            else
                GUIPropertyManager.SetProperty("#itemcount", String.Empty);
        }

        private void OnSelectItem(int item)
        {
            GUIListItem Item = _upcomingEpsiodesList[item];
            UpcomingProgram program = Item.TVTag as UpcomingProgram;

            if (program != null && program.IsPartOfSeries
                && program.GuideProgramId.HasValue)
            {
                GuideProgram prog = GuideAgent.GetProgramById(program.GuideProgramId.Value);
                if (prog != null)
                {
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    if (dlgYesNo != null)
                    {
                        if (!program.IsCancelled)
                        {
                            dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.CancelThisShow));
                            dlgYesNo.SetLine(1, program.Channel.DisplayName);
                            dlgYesNo.SetLine(2, program.Title);
                            dlgYesNo.SetLine(3, string.Empty);
                            dlgYesNo.SetDefaultToYes(false);
                            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                            if (dlgYesNo.IsConfirmed)
                            {
                                SchedulerAgent.CancelUpcomingProgram(program.ScheduleId,
                                prog.GuideProgramId, program.Channel.ChannelId, prog.StartTime);
                            }
                        }
                        else
                        {
                            dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.UncancelThisShow));
                            dlgYesNo.SetLine(1, program.Channel.DisplayName);
                            dlgYesNo.SetLine(2, program.Title);
                            dlgYesNo.SetLine(3, string.Empty);
                            dlgYesNo.SetDefaultToYes(true);
                            dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                            if (dlgYesNo.IsConfirmed)
                            {
                                SchedulerAgent.UncancelUpcomingProgram(program.ScheduleId,
                                prog.GuideProgramId, program.Channel.ChannelId, prog.StartTime);
                            }
                        }
                    }
                }

                Update();
                if (_upcomingEpsiodesList != null && item < _upcomingEpsiodesList.Count)
                {
                    _upcomingEpsiodesList.SelectedListItemIndex = item;
                }
                UpdateLabels();
            }
        }

        private GUIListItem GetItem(int iItem)
        {
            if (iItem < 0 || iItem >= _upcomingEpsiodesList.Count)
            {
                return null;
            }
            return _upcomingEpsiodesList[iItem];
        }

        private int GetSelectedItemNo()
        {
            if (_upcomingEpsiodesList.Count > 0)
            {
                return _upcomingEpsiodesList.SelectedListItemIndex;
            }
            else
            {
                return -1;
            }
        }

        private void OnNewEpisodesToggle(bool newState)
        {
            bool _hasUpcomingRecording = false;
            bool _hasUpcomingAlert = false;
            UpcomingProgram ucomingProgram;
            UpcomingRecording upcomingRecording;
            Schedule schedule = null;

            if (GetUpcomingRecordingAndSchedule(_channel.ChannelId, _currentProgram, out upcomingRecording, out schedule))
            {
                _hasUpcomingRecording = true;
            }
            else if (GetUpcomingProgramAndSchedule(_channel.ChannelId, _currentProgram, out ucomingProgram, out schedule, ScheduleType.Alert))
            {
                _hasUpcomingAlert = true;
            }

            if (schedule != null && (_hasUpcomingRecording || _hasUpcomingAlert))
            {
                ScheduleRule newEpisodesRule = FindNewEpisodesOnlyRule(schedule);
                if (newEpisodesRule != null)
                {
                    newEpisodesRule.Arguments[0] = newState;
                }
                else
                {
                    schedule.Rules.Add(new ScheduleRule(ScheduleRuleType.NewEpisodesOnly, newState));
                }
                SchedulerAgent.SaveSchedule(schedule);
                Update();
            }
        }

        private void OnPriority()
        {
            Log.Debug("TVProgammInfo.OnPriority - programm = {0}", _currentProgram.CreateProgramTitle());

            UpcomingRecording upcomingRecording;
            Schedule schedule;
            if (GetUpcomingRecordingAndSchedule(_channel.ChannelId, _currentProgram, out upcomingRecording, out schedule))
            {
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
                        Update();
                    }
                }
            }
        }

        private void OnRecordProgram()
        {
            if (RecordProgram(_channel, _currentProgram, ScheduleType.Recording, false))
            {
                Update();
            }
        }

        private void OnAlertProgram()
        {
            if (RecordProgram(_channel, _currentProgram, ScheduleType.Alert, false))
            {
                Update();
            }
        }

        private void OnPreRecordInterval()
        {
            UpcomingRecording upcomingRecording;
            Schedule schedule;
            if (GetUpcomingRecordingAndSchedule(_channel.ChannelId, _currentProgram, out upcomingRecording, out schedule))
            {
                int? newSeconds;
                if (GetPrePostRecordValue(Utility.GetLocalizedText(TextId.PreRecord), schedule.PreRecordSeconds, out newSeconds))
                {
                    schedule.PreRecordSeconds = newSeconds;
                    SchedulerAgent.SaveSchedule(schedule);
                    Update();
                }
            }
        }

        private void OnPostRecordInterval()
        {
            UpcomingRecording upcomingRecording;
            Schedule schedule;
            if (GetUpcomingRecordingAndSchedule(_channel.ChannelId, _currentProgram, out upcomingRecording, out schedule))
            {
                int? newSeconds;
                if (GetPrePostRecordValue(Utility.GetLocalizedText(TextId.PostRecord), schedule.PostRecordSeconds, out newSeconds))
                {
                    schedule.PostRecordSeconds = newSeconds;
                    SchedulerAgent.SaveSchedule(schedule);
                    Update();
                }
            }
        }

        private bool GetPrePostRecordValue(string title, int? currentSeconds, out int? newSeconds)
        {
            newSeconds = currentSeconds;
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.ShowQuickNumbers = false;
                dlg.SetHeading(title);
                dlg.Add(Utility.GetLocalizedText(TextId.Default));
                foreach (int interval in _recordingIntervalValues)
                {
                    if (interval == 1)
                    {
                        dlg.Add(String.Format("1 {0}", Utility.GetLocalizedText(TextId.MinuteSuffix)));
                    }
                    else
                    {
                        dlg.Add(String.Format("{0} {1}", interval, Utility.GetLocalizedText(TextId.MinutesSuffix)));
                    }
                }
                dlg.SelectedLabel = 0;
                if (currentSeconds.HasValue)
                {
                    int count = 1;
                    foreach (int value in _recordingIntervalValues)
                    {
                        if (currentSeconds.Value <= value * 60)
                        {
                            dlg.SelectedLabel = count;
                            break;
                        }
                        count++;
                    }
                }
                dlg.DoModal(GetID);

                if (dlg.SelectedLabel == 0)
                {
                    newSeconds = null;
                    return true;
                }
                else if (dlg.SelectedLabel > 0)
                {
                    newSeconds = 60 * _recordingIntervalValues[dlg.SelectedLabel - 1];
                    return true;
                }
            }
            return false;
        }

        private void OnKeep()
        {
            UpcomingRecording upcomingRecording;
            Schedule schedule;
            if (GetUpcomingRecordingAndSchedule(_channel.ChannelId, _currentProgram, out upcomingRecording, out schedule))
            {
                KeepUntilMode newMode;
                int? newValue;
                if (GetKeepUntilValues(schedule.KeepUntilMode, schedule.KeepUntilValue, out newMode, out newValue))
                {
                    schedule.KeepUntilMode = newMode;
                    schedule.KeepUntilValue = newValue;
                    SchedulerAgent.SaveSchedule(schedule);
                    Update();
                }
            }
        }

        private bool GetKeepUntilValues(KeepUntilMode keepUntilMode, int? keepUntilValue,
            out KeepUntilMode newMode, out int? newValue)
        {
            newMode = keepUntilMode;
            newValue = keepUntilValue;
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(1042); // Keep until
                dlg.Add(Utility.GetLocalizedText(TextId.UntilSpaceNeeded));
                dlg.Add(Utility.GetLocalizedText(TextId.NumberOfDays));
                dlg.Add(Utility.GetLocalizedText(TextId.NumberOfEpisodes));
                dlg.Add(Utility.GetLocalizedText(TextId.NumberOfWatchedEpisodes));
                dlg.Add(Utility.GetLocalizedText(TextId.Forever)); 
                switch (keepUntilMode)
                {
                    case KeepUntilMode.UntilSpaceIsNeeded:
                        dlg.SelectedLabel = 0;
                        break;
                    case KeepUntilMode.NumberOfDays:
                        dlg.SelectedLabel = 1;
                        break;
                    case KeepUntilMode.NumberOfEpisodes:
                        dlg.SelectedLabel = 2;
                        break;
                    case KeepUntilMode.NumberOfWatchedEpisodes:
                        dlg.SelectedLabel = 3;
                        break;
                    case KeepUntilMode.Forever:
                        dlg.SelectedLabel = 4;
                        break;
                }
                dlg.DoModal(GetID);

                if (dlg.SelectedId > 0)
                {
                    switch (dlg.SelectedLabel)
                    {
                        case 0:
                            newMode = KeepUntilMode.UntilSpaceIsNeeded;
                            newValue = null;
                            return true;

                        case 1:
                            {
                                int? value = GetKeepValue(1045, KeepUntilMode.NumberOfDays, 3014, 3015,
                                    keepUntilMode == KeepUntilMode.NumberOfDays ? keepUntilValue : 7);
                                if (value.HasValue)
                                {
                                    newMode = KeepUntilMode.NumberOfDays;
                                    newValue = value;
                                    return true;
                                }
                            }
                            break;

                        case 2:
                            {
                                int? value = GetKeepValue(887, KeepUntilMode.NumberOfEpisodes, 682, 914,
                                    keepUntilMode == KeepUntilMode.NumberOfEpisodes ? keepUntilValue : 3);
                                if (value.HasValue)
                                {
                                    newMode = KeepUntilMode.NumberOfEpisodes;
                                    newValue = value;
                                    return true;
                                }
                            }
                            break;

                        case 3:
                            {
                                int? value = GetKeepValue(887, KeepUntilMode.NumberOfWatchedEpisodes, 682, 914,
                                    keepUntilMode == KeepUntilMode.NumberOfWatchedEpisodes ? keepUntilValue : 3);
                                if (value.HasValue)
                                {
                                    newMode = KeepUntilMode.NumberOfWatchedEpisodes;
                                    newValue = value;
                                    return true;
                                }
                            }
                            break;

                        case 4:
                            newMode = KeepUntilMode.Forever;
                            newValue = null;
                            return true;
                    }
                }
            }
            return false;
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

        private void OnRenameSchedule()
        {
            bool _hasUpcomingRecording = false;
            bool _hasUpcomingAlert = false;
            UpcomingProgram upcomingProgram;
            UpcomingRecording upcomingRecording;
            Schedule schedule = null;

            if (GetUpcomingRecordingAndSchedule(_channel.ChannelId, _currentProgram, out upcomingRecording, out schedule))
            {
                _hasUpcomingRecording = true;
            }
            else if (GetUpcomingProgramAndSchedule(_channel.ChannelId,_currentProgram, out upcomingProgram,out schedule,ScheduleType.Alert))
            {
                _hasUpcomingAlert = true;
            }

            if (_hasUpcomingRecording || _hasUpcomingAlert)
            {
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
                        Update();
                    }
                }
            }
        }

        private static ScheduleRule FindNewEpisodesOnlyRule(Schedule schedule)
        {
            foreach (ScheduleRule rule in schedule.Rules)
            {
                if (rule.Type == ScheduleRuleType.NewEpisodesOnly)
                {
                    return rule;
                }
            }
            return null;
        }

        #endregion

        #region Internal methods

        internal static bool RecordProgram(Channel channel, GuideProgram guideProgram, ScheduleType scheduleType, bool NeedConfirm)
        {
            Log.Debug("TVProgammInfo.RecordProgram - channel = {0}, program = {1}", channel.DisplayName, guideProgram.CreateProgramTitle());
            using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
            {
                bool hasUpcomingRecording = false;
                bool hasUpcomingAlert = false;
                
                if (scheduleType == ScheduleType.Recording)
                {
                    UpcomingRecording upcomingRecording;
                    if (HasUpcomingRecording(channel.ChannelId, guideProgram, out upcomingRecording))
                    {
                        hasUpcomingRecording = true;
                        if (upcomingRecording.Program.IsCancelled)
                        {
                            switch (upcomingRecording.Program.CancellationReason)
                            {
                                case UpcomingCancellationReason.Manual:
                                    tvSchedulerAgent.UncancelUpcomingProgram(upcomingRecording.Program.ScheduleId, guideProgram.GuideProgramId, channel.ChannelId, guideProgram.StartTime);
                                    return true;

                                case UpcomingCancellationReason.AlreadyQueued:
                                    {
                                        GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                        dlg.Reset();
                                        dlg.SetHeading(Utility.GetLocalizedText(TextId.Record));
                                        dlg.SetLine(1, Utility.GetLocalizedText(TextId.ThisProgramIsAlreadyQueued));
                                        dlg.SetLine(2, Utility.GetLocalizedText(TextId.ForRecordingAtAnEarlierTime));
                                        dlg.DoModal(GUIWindowManager.ActiveWindow);
                                    }
                                    break;

                                case UpcomingCancellationReason.PreviouslyRecorded:
                                    {
                                        GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                        dlg.Reset();
                                        dlg.SetHeading(Utility.GetLocalizedText(TextId.Record));
                                        dlg.SetLine(1, Utility.GetLocalizedText(TextId.ThisProgramWasPreviouslyRecorded));
                                        dlg.DoModal(GUIWindowManager.ActiveWindow);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (upcomingRecording.Program.IsPartOfSeries)
                            {
                                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                                if (dlg != null)
                                {
                                    dlg.Reset();
                                    dlg.SetHeading(Utility.GetLocalizedText(TextId.DeleteProgram));
                                    dlg.Add(Utility.GetLocalizedText(TextId.CancelThisShow));
                                    dlg.Add(Utility.GetLocalizedText(TextId.DeleteEntireSchedule));
                                    dlg.DoModal(GUIWindowManager.ActiveWindow);

                                    if (dlg.SelectedId > 0)
                                    {
                                        switch (dlg.SelectedLabel)
                                        {
                                            case 0: // Cancel
                                                tvSchedulerAgent.CancelUpcomingProgram(upcomingRecording.Program.ScheduleId,
                                                    guideProgram.GuideProgramId, channel.ChannelId, guideProgram.StartTime);
                                                return true;

                                            case 1: // Delete
                                                Schedule schedule = tvSchedulerAgent.GetScheduleById(upcomingRecording.Program.ScheduleId);
                                                if (schedule != null)
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
                                                            tvSchedulerAgent.DeleteSchedule(upcomingRecording.Program.ScheduleId);
                                                            return true;
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (PluginMain.IsActiveRecording(channel.ChannelId, guideProgram))
                                {
                                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                    if (dlgYesNo != null)
                                    {
                                        dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.StopRecording));
                                        dlgYesNo.SetLine(1, channel.DisplayName);
                                        dlgYesNo.SetLine(2, guideProgram.Title);
                                        dlgYesNo.SetLine(3, string.Empty);
                                        dlgYesNo.SetDefaultToYes(false);
                                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                                        if (!dlgYesNo.IsConfirmed)
                                        {
                                            return false;
                                        }
                                    }
                                }
                                else if (PluginMain.IsActiveRecording(channel.ChannelId, guideProgram) == false && NeedConfirm)
                                {
                                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                    if (dlgYesNo != null)
                                    {
                                        dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.DontRecord));
                                        dlgYesNo.SetLine(1, channel.DisplayName);
                                        dlgYesNo.SetLine(2, guideProgram.Title);
                                        dlgYesNo.SetLine(3, string.Empty);
                                        dlgYesNo.SetDefaultToYes(true);
                                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                                        if (!dlgYesNo.IsConfirmed)
                                        {
                                            return false;
                                        }
                                    }
                                }

                                tvSchedulerAgent.DeleteSchedule(upcomingRecording.Program.ScheduleId);
                                return true;
                            }
                        }
                    }
                }
                else if (scheduleType == ScheduleType.Alert)
                {
                    UpcomingProgram upcomingProgram;
                    if (HasUpcomingProgram(channel.ChannelId, guideProgram, out upcomingProgram, scheduleType))
                    {
                        hasUpcomingAlert = true;
                        if (upcomingProgram.IsCancelled)
                        {
                            switch (upcomingProgram.CancellationReason)
                            {
                                case UpcomingCancellationReason.Manual:
                                    tvSchedulerAgent.UncancelUpcomingProgram(upcomingProgram.ScheduleId, guideProgram.GuideProgramId, channel.ChannelId, guideProgram.StartTime);
                                    return true;

                                case UpcomingCancellationReason.AlreadyQueued:
                                    {
                                        GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                        dlg.Reset();
                                        dlg.SetHeading(Utility.GetLocalizedText(TextId.Record));
                                        dlg.SetLine(1, Utility.GetLocalizedText(TextId.ThisProgramIsAlreadyQueued));
                                        dlg.SetLine(2, Utility.GetLocalizedText(TextId.ForRecordingAtAnEarlierTime));
                                        dlg.DoModal(GUIWindowManager.ActiveWindow);
                                    }
                                    break;

                                case UpcomingCancellationReason.PreviouslyRecorded:
                                    {
                                        GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                                        dlg.Reset();
                                        dlg.SetHeading(Utility.GetLocalizedText(TextId.Record));
                                        dlg.SetLine(1, Utility.GetLocalizedText(TextId.ThisProgramWasPreviouslyRecorded));
                                        dlg.DoModal(GUIWindowManager.ActiveWindow);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (upcomingProgram.IsPartOfSeries)
                            {
                                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                                if (dlg != null)
                                {
                                    dlg.Reset();
                                    dlg.SetHeading(Utility.GetLocalizedText(TextId.DeleteProgram));
                                    dlg.Add(Utility.GetLocalizedText(TextId.CancelThisShow));
                                    dlg.Add(Utility.GetLocalizedText(TextId.DeleteEntireSchedule));
                                    dlg.DoModal(GUIWindowManager.ActiveWindow);

                                    if (dlg.SelectedId > 0)
                                    {
                                        switch (dlg.SelectedLabel)
                                        {
                                            case 0: // Cancel
                                                tvSchedulerAgent.CancelUpcomingProgram(upcomingProgram.ScheduleId,
                                                    guideProgram.GuideProgramId, channel.ChannelId, guideProgram.StartTime);
                                                return true;

                                            case 1: // Delete
                                                Schedule schedule = tvSchedulerAgent.GetScheduleById(upcomingProgram.ScheduleId);//GetScheduleById(upcomingRecording.Program.ScheduleId);
                                                if (schedule != null)
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
                                                            tvSchedulerAgent.DeleteSchedule(upcomingProgram.ScheduleId);
                                                            return true;
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (NeedConfirm)
                                {
                                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                    if (dlgYesNo != null)
                                    {
                                        dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.DontRecord));
                                        dlgYesNo.SetLine(1, channel.DisplayName);
                                        dlgYesNo.SetLine(2, guideProgram.Title);
                                        dlgYesNo.SetLine(3, string.Empty);
                                        dlgYesNo.SetDefaultToYes(true);
                                        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                                        if (!dlgYesNo.IsConfirmed)
                                        {
                                            return false;
                                        }
                                    }
                                }
                                tvSchedulerAgent.DeleteSchedule(upcomingProgram.ScheduleId);
                                return true;
                            }
                        }
                    }
                }

                if (!hasUpcomingRecording && !hasUpcomingAlert)
                {
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg != null)
                    {
                        Schedule newSchedule = null;
                        GuideController.RepeatingType dayRepeatingType;

                        dlg.Reset();
                        dlg.SetHeading(Utility.GetLocalizedText(TextId.SelectScheduleType));
                        dlg.Add(Utility.GetLocalizedText(TextId.Once));
                        dlg.Add(Utility.GetLocalizedText(TextId.EverytimeOnThisChannel));
                        dlg.Add(Utility.GetLocalizedText(TextId.EverytimeOnEveryChannel));
                        dlg.Add(Utility.GetLocalizedText(TextId.EveryWeekAtThisTime));
                        dlg.Add(Utility.GetLocalizedText(TextId.EveryDayAtThisTime));
                        if (guideProgram.StartTime.DayOfWeek == DayOfWeek.Saturday
                            || guideProgram.StartTime.DayOfWeek == DayOfWeek.Sunday)
                        {
                            dayRepeatingType = GuideController.RepeatingType.SatSun;
                            dlg.Add(Utility.GetLocalizedText(TextId.SatSun));
                        }
                        else
                        {
                            dayRepeatingType = GuideController.RepeatingType.MonFri;
                            dlg.Add(Utility.GetLocalizedText(TextId.MonFri));
                        }
                        dlg.DoModal(GUIWindowManager.ActiveWindow);

                        switch (dlg.SelectedLabel)
                        {
                            case 0: //once
                                newSchedule = GuideController.CreateRecordOnceSchedule(tvSchedulerAgent, channel.ChannelType, channel.ChannelId,
                                    guideProgram.Title, guideProgram.SubTitle, guideProgram.EpisodeNumberDisplay, guideProgram.StartTime);
                                break;

                            case 1: //everytime, this channel
                                newSchedule = GuideController.CreateRecordRepeatingSchedule(tvSchedulerAgent, GuideController.RepeatingType.AnyTimeThisChannel,
                                    channel.ChannelType, channel.ChannelId, guideProgram.Title, guideProgram.StartTime, "(" + Utility.GetLocalizedText(TextId.AlwaysThisChannel) + ")");
                                ScheduleRule newEpisodesRule = FindNewEpisodesOnlyRule(newSchedule);
                                if (newEpisodesRule != null)
                                {
                                    newEpisodesRule.Arguments[0] = false;
                                }
                                break;

                            case 2: //everytime, any channel
                                newSchedule = GuideController.CreateRecordRepeatingSchedule(tvSchedulerAgent, GuideController.RepeatingType.AnyTime,
                                    channel.ChannelType, null, guideProgram.Title, guideProgram.StartTime, "(" + Utility.GetLocalizedText(TextId.AlwaysEveryChannel) + ")");
                                ScheduleRule newEpisodesRule2 = FindNewEpisodesOnlyRule(newSchedule);
                                if (newEpisodesRule2 != null)
                                {
                                    newEpisodesRule2.Arguments[0] = false;
                                }
                                break;

                            case 3: //weekly
                                newSchedule = GuideController.CreateRecordRepeatingSchedule(tvSchedulerAgent, GuideController.RepeatingType.Weekly,
                                    channel.ChannelType, channel.ChannelId, guideProgram.Title, guideProgram.StartTime, "(" + Utility.GetLocalizedText(TextId.Weekly) + ")");
                                break;

                            case 4: //daily
                                newSchedule = GuideController.CreateRecordRepeatingSchedule(tvSchedulerAgent, GuideController.RepeatingType.Daily,
                                    channel.ChannelType, channel.ChannelId, guideProgram.Title, guideProgram.StartTime, "(" + Utility.GetLocalizedText(TextId.Daily) + ")");
                                break;

                            case 5: //Mon-Fri or Sat-Sun
                                string repeatingTime = string.Empty;
                                if (dayRepeatingType == GuideController.RepeatingType.MonFri)
                                {
                                    repeatingTime = "(" + Utility.GetLocalizedText(TextId.MonFri) + ")";
                                }
                                else if (dayRepeatingType == GuideController.RepeatingType.SatSun)
                                {
                                    repeatingTime = "(" + Utility.GetLocalizedText(TextId.SatSun) + ")";
                                }

                                newSchedule = GuideController.CreateRecordRepeatingSchedule(tvSchedulerAgent, dayRepeatingType,
                                    channel.ChannelType, channel.ChannelId, guideProgram.Title, guideProgram.StartTime, repeatingTime);
                                break;
                        }

                        if (newSchedule != null)
                        {
                            newSchedule.ScheduleType = scheduleType;
                            tvSchedulerAgent.SaveSchedule(newSchedule);
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        internal static bool HasUpcomingRecording(Guid channelId, GuideProgram program, out UpcomingRecording upcomingRecording)
        {
            using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
            {
                upcomingRecording = null;
                Guid upcomingProgramId = program.GetUniqueUpcomingProgramId(channelId);

                UpcomingRecording[] upcomingRecordings = tvControlAgent.GetAllUpcomingRecordings(UpcomingRecordingsFilter.All, true);
                foreach (UpcomingRecording recording in upcomingRecordings)
                {
                    if (recording.Program.UpcomingProgramId == upcomingProgramId)
                    {
                        upcomingRecording = recording;
                        return true;
                    }
                }
                return false;
            }
        }

        internal static bool GetUpcomingRecordingAndSchedule(Guid channelId, GuideProgram program, out UpcomingRecording upcomingRecording, out Schedule schedule)
        {
            schedule = null;
            if (HasUpcomingRecording(channelId, program, out upcomingRecording))
            {
                using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                {
                    schedule = tvSchedulerAgent.GetScheduleById(upcomingRecording.Program.ScheduleId);
                    return (schedule != null);
                }
            }
            return false;
        }

        internal static bool HasUpcomingProgram(Guid channelId, GuideProgram program, out UpcomingProgram upcomingProgram, ScheduleType scheduleType)
        {
            using (SchedulerServiceAgent SchedulerAgent = new SchedulerServiceAgent())
            {
                upcomingProgram = null;
                Guid upcomingProgramId = program.GetUniqueUpcomingProgramId(channelId);

                List<UpcomingProgram> upcomingPrograms = new List<UpcomingProgram>(
                    SchedulerAgent.GetAllUpcomingPrograms(scheduleType, true));
                foreach (UpcomingProgram upcoming in upcomingPrograms)
                {
                    if (upcoming.UpcomingProgramId == upcomingProgramId)
                    {
                        upcomingProgram = upcoming;
                        return true;
                    }
                }
                return false;
            }
        }

        internal static bool GetUpcomingProgramAndSchedule(Guid channelId, GuideProgram program, out UpcomingProgram upcomingProgram, out Schedule schedule, ScheduleType scheduleType)
        {
            schedule = null;
            if (HasUpcomingProgram(channelId, program, out upcomingProgram, scheduleType))
            {
                using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                {
                    schedule = tvSchedulerAgent.GetScheduleById(upcomingProgram.ScheduleId);
                    return (schedule != null);
                }
            }
            return false;
        }

        #endregion;
    }
}
