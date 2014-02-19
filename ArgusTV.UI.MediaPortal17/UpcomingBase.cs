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
using System.IO;
using System.Threading;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Services;
using MediaPortal.Threading;
using MediaPortal.Util;
using MediaPortal.Configuration;
using MediaPortal.Profile;
using Action = MediaPortal.GUI.Library.Action;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;

namespace ArgusTV.UI.MediaPortal
{
    public abstract class UpcomingBase : GUIWindow, IComparer<GUIListItem>
    {
        #region variables

        private enum ControlId
        {
            LABEL_PROGRAMTITLE = 13,
            LABEL_PROGRAMTIME = 14,
            LABEL_PROGRAMDESCRIPTION = 15,
            LABEL_PROGRAMGENRE = 17,
        };

        private enum SortMethod
        {
            Channel = 0,
            Date = 1,
            Name = 2,
        }

        protected abstract string SettingsSection { get; }

        private ScheduleType _currentProgramType = ScheduleType.Recording;
        private SortMethod _currentSortMethod = SortMethod.Date;
        private ScheduleSummary _selectedSchedule = null;
        private ChannelType _channelType;

        private bool m_bSortAscending = false;
        private int m_iSelectedItem = 0;
        private int m_iSelectedItemInFolder = 0;
        private const string _parentDirectoryLabel = "..";
        private bool _isInSubDirectory = false;

        [SkinControl(2)] protected GUIButtonControl _programTypeButton;
        [SkinControl(9)] protected GUISortButtonControl _sortByButton;
        [SkinControl(10)] protected GUIListControl _viewsList;
        [SkinControl(11)] protected GUIButtonControl _newProgramButton;
        [SkinControl(12)] protected GUICheckButton _groupBySchedButton;

        #endregion

        public UpcomingBase(ChannelType channelType)
        {
            _channelType = channelType;
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

        private ConfigurationServiceAgent _configurationAgent;
        public IConfigurationService ConfigurationAgent
        {
            get
            {
                if (_configurationAgent == null)
                {
                    _configurationAgent = new ConfigurationServiceAgent();
                }
                return _configurationAgent;
            }
        }

        #endregion

        #region Serialisation

        private void LoadSettings()
        {
            using (Settings xmlreader = new MPSettings())
            {
                string strTmp = String.Empty;
                strTmp = (string)xmlreader.GetValue(SettingsSection, "sort");
                if (strTmp != null)
                {
                    if (strTmp == "channel")
                    {
                        _currentSortMethod = SortMethod.Channel;
                    }
                    else if (strTmp == "date")
                    {
                        _currentSortMethod = SortMethod.Date;
                    }
                    else if (strTmp == "name")
                    {
                        _currentSortMethod = SortMethod.Name;
                    }
                }

                strTmp = String.Empty;
                strTmp = (string)xmlreader.GetValue(SettingsSection, "programtype");
                if (strTmp != null)
                {
                    if (strTmp == "recordings")
                    {
                        _currentProgramType = ScheduleType.Recording;
                    }
                    else if (strTmp == "alerts")
                    {
                        _currentProgramType = ScheduleType.Alert;
                    }
                    else if (strTmp == "suggestions")
                    {
                        _currentProgramType = ScheduleType.Suggestion;
                    }
                }

                m_bSortAscending = xmlreader.GetValueAsBool(SettingsSection, "sortascending", false);

                if (_groupBySchedButton != null)
                {
                    _groupBySchedButton.Selected = xmlreader.GetValueAsBool(SettingsSection, "groupbyschedule", false);
                }
            }
        }

        private void SaveSettings()
        {
            using (Settings xmlwriter = new MPSettings())
            {
                switch (_currentSortMethod)
                {
                    case SortMethod.Channel:
                        xmlwriter.SetValue(SettingsSection, "sort", "channel");
                        break;
                    case SortMethod.Date:
                        xmlwriter.SetValue(SettingsSection, "sort", "date");
                        break;
                    case SortMethod.Name:
                        xmlwriter.SetValue(SettingsSection, "sort", "name");
                        break;
                }

                switch (_currentProgramType)
                {
                    case ScheduleType.Recording:
                        xmlwriter.SetValue(SettingsSection, "programtype", "recordings");
                        break;
                    case ScheduleType.Alert:
                        xmlwriter.SetValue(SettingsSection, "programtype", "alerts");
                        break;
                    case ScheduleType.Suggestion:
                        xmlwriter.SetValue(SettingsSection, "programtype", "suggestions");
                        break;
                }

                xmlwriter.SetValueAsBool(SettingsSection, "sortascending", m_bSortAscending);
                xmlwriter.SetValueAsBool(SettingsSection, "groupbyschedule", _groupBySchedButton.Selected);
            }
        }

        #endregion

        #region overrides

        public override bool IsTv
        {
            get { return true; }
        }

        public override bool Init()
        {
            return true;
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _groupBySchedButton.Label = Utility.GetLocalizedText(TextId.GroupByScheduleSelect);
        }

        public override void OnAction(Action action)
        {
            if (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
            {
                if (_viewsList != null)
                {
                    if (_viewsList.Focus)
                    {
                        GUIListItem item = GetItem(0);
                        if (item != null)
                        {
                            if (item.IsFolder && item.Label == _parentDirectoryLabel)
                            {
                                LoadUpcomingPrograms(null);
                                return;
                            }
                        }
                    }
                }
            }
            /*switch (action.wID)
            {
                case Action.ActionType.ACTION_DELETE_ITEM:
                    {
                        //int item = GetSelectedItemNo();
                        //if (item >= 0)
                        //    OnDeleteRecording(item);
                        //UpdateProperties();
                    }
                    break;
            }*/
            base.OnAction(action);
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            if (_isInSubDirectory)
            {
                m_iSelectedItemInFolder = GetSelectedItemNo();
            }
            else
            {
                m_iSelectedItem = GetSelectedItemNo();
            }

            SaveSettings();
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
            if (_configurationAgent != null)
            {
                _configurationAgent.Dispose();
            }

            if (newWindowId != WindowId.ProgramInfo
                && newWindowId != WindowId.ManualShedule)
            {
                _selectedSchedule = null;
            }
            base.OnPageDestroy(newWindowId);
        }

        protected override void OnPageLoad()
        {
            PluginMain.EnsureConnection();

            base.OnPageLoad();
            if (PluginMain.IsConnected())
            {
                LoadSettings();
                if (GUIWindowManager.GetPreviousActiveWindow() != WindowId.ProgramInfo
                    && GUIWindowManager.GetPreviousActiveWindow() != WindowId.ManualShedule)
                {
                    _selectedSchedule = null;
                }
                LoadUpcomingPrograms(_selectedSchedule);
                _sortByButton.SortChanged += new SortEventHandler(SortChanged);

                if (this._channelType == ChannelType.Television)
                    GUIPropertyManager.SetProperty("#currentmodule", Utility.GetLocalizedText(TextId.TVScheduler));
                else
                    GUIPropertyManager.SetProperty("#currentmodule", Utility.GetLocalizedText(TextId.RadioScheduler));
            }
        }

        protected override void OnClicked(int controlId, GUIControl control, global::MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            base.OnClicked(controlId, control, actionType);

            if (control == _programTypeButton)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
                if (dlg == null)
                {
                    return;
                }
                dlg.Reset();
                dlg.SetHeading(467);
                dlg.Add(Utility.GetLocalizedText(TextId.UpcomingTypeRecordings));
                dlg.Add(Utility.GetLocalizedText(TextId.UpcomingTypeAlerts));
                dlg.Add(Utility.GetLocalizedText(TextId.UpcomingTypeSuggestions));

                switch (_currentProgramType)
                {
                    case ScheduleType.Recording:
                        dlg.SelectedLabel = 0;
                        break;
                    case ScheduleType.Alert:
                        dlg.SelectedLabel = 1;
                        break;
                    case ScheduleType.Suggestion:
                        dlg.SelectedLabel = 2;
                        break;
                }

                // show dialog and wait for result
                dlg.DoModal(GetID);
                if (dlg.SelectedId == -1)
                {
                    return;
                }

                switch (dlg.SelectedLabel)
                {
                    case 0:
                        _currentProgramType = ScheduleType.Recording;
                        break;
                    case 1:
                        _currentProgramType = ScheduleType.Alert;
                        break;
                    case 2:
                        _currentProgramType = ScheduleType.Suggestion;
                        break;
                }
                m_iSelectedItem = 0;
                m_iSelectedItemInFolder = 0;
                LoadUpcomingPrograms(null);
            }

            if (control == _groupBySchedButton)
            {
                m_iSelectedItem = 0;
                m_iSelectedItemInFolder = 0;
                LoadUpcomingPrograms(null);
            }

            if (control == _newProgramButton)
            {
                OnNewSchedule();
            }

            if (control == _sortByButton) // sort by
            {
                if (!_groupBySchedButton.Selected || _isInSubDirectory)
                {
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
                    if (dlg == null)
                    {
                        return;
                    }
                    dlg.Reset();
                    dlg.SetHeading(495); //Sort Options
                    dlg.AddLocalizedString(620); //channel
                    dlg.AddLocalizedString(621); //date
                    dlg.AddLocalizedString(268); //title

                    // set the focus to currently used sort method
                    dlg.SelectedLabel = (int)_currentSortMethod;

                    // show dialog and wait for result
                    dlg.DoModal(GetID);
                    if (dlg.SelectedId == -1)
                    {
                        return;
                    }

                    _currentSortMethod = (SortMethod)dlg.SelectedLabel;
                }
                else
                {
                    //we don't have a time and channel for schedules,so we can only use the title/name sorting.
                    _currentSortMethod = SortMethod.Name;
                }
                OnSort();
            }

            if (control == _viewsList)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, control.GetID, 0, 0, null);
                OnMessage(msg);
                int iItem = (int)msg.Param1;
                if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
                {
                    OnEditSchedule(iItem);
                }
                if (actionType == Action.ActionType.ACTION_SHOW_INFO)
                {
                    OnShowContextMenu();
                }
            }
        }

        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
                    UpdateProperties();
                    break;
            }
            return base.OnMessage(message);
        }

        protected override void OnShowContextMenu()
        {
            int iItem = GetSelectedItemNo();
            GUIListItem pItem = GetItem(iItem);
            if (pItem == null) return;
            if (pItem.IsFolder)
            {
                ScheduleSummary schedule = pItem.TVTag as ScheduleSummary;
                if (schedule != null)
                {
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(schedule.Name?? string.Empty);
                    dlg.Add(Utility.GetLocalizedText(TextId.Settings));
                    if (schedule.IsActive)
                        dlg.Add(Utility.GetLocalizedText(TextId.CancelThisSchedule));
                    else
                        dlg.Add(Utility.GetLocalizedText(TextId.UnCancelThisSchedule));

                    dlg.Add(Utility.GetLocalizedText(TextId.DeleteThisSchedule));
                    dlg.DoModal(GetID);

                    Schedule _sched = SchedulerAgent.GetScheduleById(schedule.ScheduleId);
                    switch (dlg.SelectedLabel)
                    {
                        case 0:
                            if (_sched != null)
                            {
                                UpcomingProgram[] programs = SchedulerAgent.GetUpcomingPrograms(_sched, true);
                                if (programs != null && programs.Length > 0)
                                {
                                    OnEditSchedule(programs[0]);
                                }
                            }
                            break;

                        case 1:
                            if (_sched != null)
                            {
                                if (_sched.IsActive)
                                {
                                    _sched.IsActive = false;
                                    SchedulerAgent.SaveSchedule(_sched);
                                }
                                else
                                {
                                    _sched.IsActive = true;
                                    SchedulerAgent.SaveSchedule(_sched);
                                }
                            }
                            break;

                        case 2:
                            {
                                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                                if (dlgYesNo != null)
                                {
                                    dlgYesNo.Reset();
                                    dlgYesNo.SetHeading(schedule.Name);
                                    dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.DeleteThisSchedule));
                                    dlgYesNo.SetLine(2, Utility.GetLocalizedText(TextId.AreYouSure));
                                    dlgYesNo.SetLine(3, string.Empty);
                                    dlgYesNo.SetDefaultToYes(false);
                                    dlgYesNo.DoModal(GetID);

                                    if (dlgYesNo.IsConfirmed)
                                    {
                                        SchedulerAgent.DeleteSchedule(schedule.ScheduleId);
                                        _selectedSchedule = null;
                                    }
                                }
                            }
                            break;
                    }
                    m_iSelectedItem = GetSelectedItemNo();
                    LoadUpcomingPrograms(null);
                }
            }
            else
            {
                UpcomingProgram upcoming = pItem.TVTag as UpcomingProgram;
                if (upcoming != null)
                {
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg == null) return;
                    dlg.Reset();
                    dlg.SetHeading(upcoming.Title);
                    dlg.Add(Utility.GetLocalizedText(TextId.Settings));
                    dlg.Add(Utility.GetLocalizedText(TextId.Priority));
                    if (upcoming.IsCancelled)
                    {
                        if (upcoming.CancellationReason == UpcomingCancellationReason.Manual)
                        {
                            dlg.Add(Utility.GetLocalizedText(TextId.UncancelThisShow));
                        }
                    }
                    else
                    {
                        dlg.Add(Utility.GetLocalizedText(TextId.CancelThisShow));
                    }
                    dlg.DoModal(GetID);

                    switch (dlg.SelectedLabel)
                    {
                        case 0: // settings/information
                            OnEditSchedule(upcoming);
                            break;

                        case 1: // Priority
                            OnChangePriority(upcoming);
                            break;

                        case 2: // (Un)Cancel
                            if (upcoming.IsCancelled)
                            {
                                this.SchedulerAgent.UncancelUpcomingProgram(upcoming.ScheduleId, upcoming.GuideProgramId, upcoming.Channel.ChannelId, upcoming.StartTime);
                            }
                            else
                            {
                                this.SchedulerAgent.CancelUpcomingProgram(upcoming.ScheduleId, upcoming.GuideProgramId, upcoming.Channel.ChannelId, upcoming.StartTime);
                            }
                            m_iSelectedItem = GetSelectedItemNo();
                            LoadUpcomingPrograms(null);
                            break;
                    }
                }
            }
        }

        private void OnNewSchedule()
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(GUILocalizeStrings.Get(424));
                dlg.Add(Utility.GetLocalizedText(this._channelType == ChannelType.Television ? TextId.TvGuide : TextId.RadioGuide));
                dlg.Add(Utility.GetLocalizedText(TextId.SearchGuide));
                dlg.Add(Utility.GetLocalizedText(TextId.Manual));
                dlg.DoModal(GetID);

                switch (dlg.SelectedLabel)
                {
                    case 0:
                        GUIWindowManager.ActivateWindow(
                            this._channelType == ChannelType.Television ? WindowId.TvGuide : WindowId.RadioGuide);
                        break;

                    case 1:
                        GUIWindowManager.ActivateWindow(
                            this._channelType == ChannelType.Television ? WindowId.TvGuideSearch : WindowId.RadioGuideSearch);
                        break;

                    case 2:
                        ManualSchedule.upcomingProgram = null;
                        ManualSchedule.channelType = this._channelType;
                        GUIWindowManager.ActivateWindow(WindowId.ManualShedule);
                        break;
                }
            }
        }

        #endregion

        #region private methods

        private void LoadUpcomingPrograms(ScheduleSummary schedule)
        {
            GUIControl.ClearControl(GetID, _viewsList.GetID);

            if (schedule == null)
            {
                _isInSubDirectory = false;
                bool group = false;
                if (_groupBySchedButton != null)
                {
                    group = _groupBySchedButton.Selected;
                }

                if (group)
                {
                    ScheduleSummary[] schedules = SchedulerAgent.GetAllSchedules(this._channelType, _currentProgramType, true);
                    foreach (ScheduleSummary sched in schedules)
                    {
                        GUIListItem item = CreateListItem(null, null, sched);
                        _viewsList.Add(item);
                    }
                }
                else
                {
                    if (_currentProgramType == ScheduleType.Recording)
                    {
                        List<UpcomingRecording> upcomingRecordings = new List<UpcomingRecording>(
                            this.ControlAgent.GetAllUpcomingRecordings(UpcomingRecordingsFilter.Recordings | UpcomingRecordingsFilter.CancelledByUser, false));
                        foreach (UpcomingRecording recording in upcomingRecordings)
                        {
                            if (recording.Program.Channel.ChannelType == this._channelType)
                            {
                                GUIListItem item = CreateListItem(recording.Program , recording, null);
                                _viewsList.Add(item);
                            }
                        }
                    }
                    else
                    {
                        List<UpcomingProgram> upcomingPrograms = new List<UpcomingProgram>(
                            this.SchedulerAgent.GetAllUpcomingPrograms(_currentProgramType, true));
                        foreach (UpcomingProgram program in upcomingPrograms)
                        {
                            if (program.Channel.ChannelType == this._channelType)
                            {
                                GUIListItem item = CreateListItem(program, null, null);
                                _viewsList.Add(item);
                            }
                        }
                    }
                }
            }
            else if (schedule != null)
            {
                //add prev directory folder
                GUIListItem item = new GUIListItem();
                item.Label = _parentDirectoryLabel;
                item.IsFolder = true;
                Utils.SetDefaultIcons(item);
                _viewsList.Add(item);

                _selectedSchedule = schedule;
                if (_currentProgramType == ScheduleType.Recording)
                {
                    UpcomingRecording[] upcomingRecordings = ControlAgent.GetUpcomingRecordings(schedule.ScheduleId, true);
                    foreach (UpcomingRecording recording in upcomingRecordings)
                    {
                        item = CreateListItem(recording.Program, recording, null);
                        _viewsList.Add(item);
                    }
                }
                else
                {
                    Schedule sched = SchedulerAgent.GetScheduleById(schedule.ScheduleId);
                    UpcomingProgram[] upcomingPrograms = SchedulerAgent.GetUpcomingPrograms(sched, true);
                    foreach (UpcomingProgram upcomingProgram in upcomingPrograms)
                    {
                        item = CreateListItem(upcomingProgram, null, null);
                        _viewsList.Add(item);
                    }
                }

                _isInSubDirectory = true;
            }

            string strObjects = string.Format("{0}", _viewsList.Count - (_viewsList.Count > 0 && _viewsList[0].Label == _parentDirectoryLabel ? 1 : 0));
            GUIPropertyManager.SetProperty("#itemcount", strObjects);

            OnSort();
            UpdateButtonStates();
            UpdateProperties();

            if (GetItemCount() > 0)
            {
                if (_isInSubDirectory)
                {
                    while (m_iSelectedItemInFolder >= GetItemCount() && m_iSelectedItemInFolder > 0)
                    {
                        m_iSelectedItemInFolder--;
                    }
                    GUIControl.SelectItemControl(GetID, _viewsList.GetID, m_iSelectedItemInFolder);
                }
                else
                {
                    while (m_iSelectedItem >= GetItemCount() && m_iSelectedItem > 0)
                    {
                        m_iSelectedItem--;
                    }
                    GUIControl.SelectItemControl(GetID, _viewsList.GetID, m_iSelectedItem);
                }
            }
        }

        private GUIListItem CreateListItem(UpcomingProgram upcomingProgram, UpcomingRecording recording, ScheduleSummary schedule)
        {
            GUIListItem item = new GUIListItem();
            if (schedule != null)
            {
                //create list with schedules
                item.Label = schedule.Name;
                item.IsFolder = true;
                Utils.SetDefaultIcons(item);
                item.PinImage = Utility.GetLogoForSchedule(schedule.ScheduleType, schedule.IsOneTime, schedule.IsActive);
                item.TVTag = schedule;
                item.IsPlayed = !schedule.IsActive;
            }
            else
            {
                //create list with Upcoming Programs
                string title = upcomingProgram.CreateProgramTitle();
                item.Label = title;
                string logoImagePath = Utility.GetLogoImage(upcomingProgram.Channel, SchedulerAgent);
                if (!Utils.FileExistsInCache(logoImagePath))
                {
                    item.Label = String.Format("[{0}] {1}", upcomingProgram.Channel.DisplayName, title);
                    logoImagePath = "defaultVideoBig.png";
                }
                item.PinImage = Utility.GetIconImageFileName(_currentProgramType, upcomingProgram.IsPartOfSeries,
                    upcomingProgram.CancellationReason, recording);
                item.TVTag = upcomingProgram;
                item.ThumbnailImage = logoImagePath;
                item.IconImageBig = logoImagePath;
                item.IconImage = logoImagePath;
                item.Label2 = String.Format("{0} {1} - {2}", Utility.GetShortDayDateString(upcomingProgram.StartTime),
                    upcomingProgram.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                    upcomingProgram.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                item.IsPlayed = upcomingProgram.IsCancelled;
            }
            return item;
        }

        private void UpdateButtonStates()
        {
            string strLine = string.Empty;
            if (_isInSubDirectory || !_groupBySchedButton.Selected)
            {
                switch (_currentSortMethod)
                {
                    case SortMethod.Channel:
                        strLine = Utility.GetLocalizedText(TextId.SortByChannel);
                        break;
                    case SortMethod.Date:
                        strLine = Utility.GetLocalizedText(TextId.SortByDate);
                        break;
                    case SortMethod.Name:
                        strLine = Utility.GetLocalizedText(TextId.SortByTitle);
                        break;
                }
            }
            else
            {
                strLine = Utility.GetLocalizedText(TextId.SortByTitle);
            }
            GUIControl.SetControlLabel(GetID, _sortByButton.GetID, strLine);

            switch (_currentProgramType)
            {
                case ScheduleType.Recording:
                    strLine = Utility.GetLocalizedText(TextId.UpcomingTypeRecordings);
                    break;
                case ScheduleType.Alert:
                    strLine = Utility.GetLocalizedText(TextId.UpcomingTypeAlerts);
                    break;
                case ScheduleType.Suggestion:
                    strLine = Utility.GetLocalizedText(TextId.UpcomingTypeSuggestions);
                    break;
            }
            GUIControl.SetControlLabel(GetID, _programTypeButton.GetID, strLine);
            _sortByButton.IsAscending = m_bSortAscending;
        }

        private bool OnEditSchedule(int iItem)
        {
            GUIListItem pItem = GetItem(iItem);
            if (pItem != null)
            {
                if (pItem.Label == _parentDirectoryLabel)
                {
                    LoadUpcomingPrograms(null);
                    return false;
                }
                else if (!pItem.IsFolder)
                {
                    return OnEditSchedule(pItem.TVTag as UpcomingProgram);
                }
                else if (pItem.IsFolder)
                {
                    m_iSelectedItem = GetSelectedItemNo();
                    m_iSelectedItemInFolder = 0;
                    if (!pItem.IsPlayed)
                    {
                        LoadUpcomingPrograms(pItem.TVTag as ScheduleSummary);
                    }
                    return false;
                }
            }
            return false;
        }

        private bool OnEditSchedule(UpcomingProgram upcoming)
        {
            if (upcoming != null)
            {
                Schedule sched = null;
                using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                {
                    sched = _tvSchedulerAgent.GetScheduleById(upcoming.ScheduleId);
                    if (sched != null)
                    {
                        if (upcoming.GuideProgramId.HasValue && sched.Rules.FindRuleByType(ScheduleRuleType.ManualSchedule) == null)
                        {
                            TvProgramInfo.CurrentProgram = this.GuideAgent.GetProgramById(upcoming.GuideProgramId.Value);
                            TvProgramInfo.Channel = upcoming.Channel;
                            GUIWindowManager.ActivateWindow((int)WindowId.ProgramInfo);
                            return true;
                        }
                        else if (sched.Rules.FindRuleByType(ScheduleRuleType.ManualSchedule) != null)
                        {
                            ManualSchedule.channelType = upcoming.Channel.ChannelType;
                            ManualSchedule.upcomingProgram = upcoming;
                            GUIWindowManager.ActivateWindow((int)WindowId.ManualShedule);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void OnChangePriority(UpcomingProgram upcoming)
        {
            if (upcoming != null)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg != null)
                {
                    dlg.Reset();
                    dlg.SetHeading(Utility.GetLocalizedText(TextId.Priority));
                    dlg.Add(Utility.GetLocalizedText(TextId.VeryLow));
                    dlg.Add(Utility.GetLocalizedText(TextId.Low));
                    dlg.Add(Utility.GetLocalizedText(TextId.Normal));
                    dlg.Add(Utility.GetLocalizedText(TextId.High));
                    dlg.Add(Utility.GetLocalizedText(TextId.VeryHigh));
                    dlg.Add(Utility.GetLocalizedText(TextId.Highest));
                    dlg.Add(Utility.GetLocalizedText(TextId.ResetToSchedulePriority));
                    dlg.SelectedLabel = (int)upcoming.Priority + 2;
                    dlg.DoModal(GetID);
                    if (dlg.SelectedLabel >= 0)
                    {
                        int selectedInt = dlg.SelectedLabel - 2;
                        UpcomingProgramPriority? priority = null;
                        if (selectedInt >= (int)UpcomingProgramPriority.VeryLow
                            && selectedInt <= (int)UpcomingProgramPriority.Highest)
                        {
                            priority = (UpcomingProgramPriority)selectedInt;
                        }
                        SchedulerAgent.SetUpcomingProgramPriority(upcoming.UpcomingProgramId, upcoming.StartTime, priority);
                        m_iSelectedItem = GetSelectedItemNo();
                        LoadUpcomingPrograms(null);
                    }
                }
            }
        }

        private void UpdateProperties()
        {
            GUIListItem pItem = GetItem(GetSelectedItemNo());
            if (pItem == null)
            {
                SetProperties(null, null);
                return;
            }

            if (!pItem.IsFolder)
            {
                UpcomingProgram upcoming = pItem.TVTag as UpcomingProgram;
                SetProperties(upcoming, null);
            }
            else
            {
                ScheduleSummary schedule = pItem.TVTag as ScheduleSummary;
                SetProperties(null, schedule);
            }
        }

        private void SetProperties(UpcomingProgram upcoming, ScheduleSummary schedule)
        {
            string guiPropertyPrefix = this._channelType == ChannelType.Television ? "#TV" : "#Radio";
            if (schedule != null)
            {
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Channel", String.Empty);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Title", schedule.Name);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Genre", String.Empty);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Time", String.Empty);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Description", " ");
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.thumb", "defaultFolderBig.png");
            }
            else if (upcoming != null)
            {
                GuideProgram guideProgram = upcoming.GuideProgramId.HasValue ?
                    this.GuideAgent.GetProgramById(upcoming.GuideProgramId.Value) : null;

                string strTime = string.Format("{0} {1} - {2}",
                  Utility.GetShortDayDateString(upcoming.StartTime),
                  upcoming.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                  upcoming.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Channel", upcoming.Channel.DisplayName);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Title", upcoming.Title);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Genre", upcoming.Category);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Time", strTime);

                string description = String.Empty;
                if (guideProgram != null)
                {
                    description = guideProgram.CreateCombinedDescription(true);
                }
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Description", description);

                string logo = Utility.GetLogoImage(upcoming.Channel.ChannelId, upcoming.Channel.DisplayName, SchedulerAgent);
                if (!string.IsNullOrEmpty(logo))
                {
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.thumb", logo);
                }
                else
                {
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.thumb", "defaultVideoBig.png");
                }
            }
            else
            {
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Channel", String.Empty);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Title", String.Empty);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Genre", String.Empty);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Time", String.Empty);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.Description", " ");
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Upcoming.thumb", String.Empty);
            }
        }

        #endregion

        #region Album/List View Management

        private GUIListItem GetSelectedItem()
        {
            int controlId = _viewsList.GetID;
            return GUIControl.GetSelectedListItem(GetID, controlId);
        }

        private GUIListItem GetItem(int iItem)
        {
            if (iItem < 0 || iItem >= GetItemCount())
            {
                return null;
            }
            return _viewsList[iItem];
        }

        private int GetSelectedItemNo()
        {
            int controlId = _viewsList.GetID;
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, controlId, 0, 0, null);
            OnMessage(msg);
            return (int)msg.Param1;
        }

        private int GetItemCount()
        {
            return _viewsList.Count;
        }

        #endregion

        #region Sort Members

        private void OnSort()
        {
            _viewsList.Sort(this);
            UpdateButtonStates();
        }

        public int Compare(GUIListItem item1, GUIListItem item2)
        {
            int result = 0;

            int resultLower = m_bSortAscending ? -1 : 1;
            int resultUpper = -resultLower;

            if (item1 == item2 || item1 == null || item2 == null)
            {
                return 0;
            }
            if (item1.IsFolder && !item2.IsFolder)
            {
                return -1;
            }
            else if (!item1.IsFolder && item2.IsFolder)
            {
                return 1;
            }
            else if (item1.IsFolder && item2.IsFolder)
            {
                switch (_currentSortMethod)
                {
                    case SortMethod.Channel:
                    case SortMethod.Date:
                    case SortMethod.Name:
                        result = resultUpper * String.Compare(item1.Label, item2.Label, true);
                        if (result == 0)
                        {
                            ScheduleSummary schedule1 = item1.TVTag as ScheduleSummary;
                            ScheduleSummary schedule2 = item2.TVTag as ScheduleSummary;
                            result = (schedule1.LastModifiedTime < schedule2.LastModifiedTime) ? resultUpper : resultLower;
                        }
                        break;
                }
            }
            else
            {
                UpcomingProgram Upcoming1 = item1.TVTag as UpcomingProgram;
                UpcomingProgram Upcoming2 = item2.TVTag as UpcomingProgram;

                switch (_currentSortMethod)
                {
                    case SortMethod.Name:
                        result = resultUpper * String.Compare(Upcoming1.Title, Upcoming2.Title, true);
                        if (result == 0)
                        {
                            goto case SortMethod.Channel;
                        }
                        break;

                    case SortMethod.Channel:
                        result = resultUpper * String.Compare(Upcoming1.Channel.CombinedDisplayName, Upcoming2.Channel.DisplayName, true);
                        if (result == 0)
                        {
                            goto case SortMethod.Date;
                        }
                        break;

                    case SortMethod.Date:
                        if (Upcoming1.StartTime != Upcoming2.StartTime)
                        {
                            result = (Upcoming1.StartTime < Upcoming2.StartTime) ? resultUpper : resultLower;
                        }
                        break;
                }
            }
            return result;
        }

        private void SortChanged(object sender, SortEventArgs e)
        {
            m_bSortAscending = e.Order != System.Windows.Forms.SortOrder.Descending;
            OnSort();
        }

        #endregion
    }
}
