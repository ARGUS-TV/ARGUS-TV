#region Copyright (C) 2005-2008 Team MediaPortal

/* 
 *	Copyright (C) 2005-2008 Team MediaPortal
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
using Action = MediaPortal.GUI.Library.Action;

using ForTheRecord.Entities;
using ForTheRecord.ServiceAgents;
using ForTheRecord.ServiceContracts;
using ForTheRecord.UI.Process.Recordings;
using ForTheRecord.UI.Process;

namespace ForTheRecord.UI.MediaPortal
{
    public class UpcomingPrograms : GUIWindow, IComparer<GUIListItem>
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
            Genre = 3,
            Played = 4,
            Duration = 5
        }

        private ScheduleType _currentProgramType = ScheduleType.Recording;
        private SortMethod _currentSortMethod = SortMethod.Date;

        private bool _sortAscending = true;
        private int _selectedItemIndex;

        [SkinControl(2)]
        protected GUIButtonControl _programTypeButton;
        [SkinControl(10)]
        protected GUIListControl _viewsList;

        #endregion

        public UpcomingPrograms()
        {
            GetID = (int)WindowId.UpcomingPrograms;
        }

        #region Service Agents

        private TvSchedulerServiceAgent _tvSchedulerAgent;

        public ITvSchedulerService TvSchedulerAgent
        {
            get
            {
                if (_tvSchedulerAgent == null)
                {
                    _tvSchedulerAgent = new TvSchedulerServiceAgent();
                }
                return _tvSchedulerAgent;
            }
        }

        private TvGuideServiceAgent _tvGuideAgent;

        public ITvGuideService TvGuideAgent
        {
            get
            {
                if (_tvGuideAgent == null)
                {
                    _tvGuideAgent = new TvGuideServiceAgent();
                }
                return _tvGuideAgent;
            }
        }

        private TvControlServiceAgent _tvControlAgent;

        public ITvControlService TvControlAgent
        {
            get
            {
                if (_tvControlAgent == null)
                {
                    _tvControlAgent = new TvControlServiceAgent();
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

        public override void OnAdded()
        {
            Log.Info("UpcomingPrograms:OnAdded");
            Restore();
            PreInit();
            ResetAllControls();
        }

        public override bool IsTv
        {
            get { return true; }
        }

        #region Serialisation

        private void LoadSettings()
        {
        }

        private void SaveSettings()
        {
        }

        #endregion

        #region overrides

        public override bool Init()
        {
            bool bResult = Load(GUIGraphicsContext.Skin + @"\4TR_Upcoming.xml");
            LoadSettings();
            Restore();
            PreInit();
            ResetAllControls();
            return bResult;
        }

        public override void OnAction(Action action)
        {
            if (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
            {
                //if (_albumsList.Focus || _viewsList.Focus)
                //{
                //    GUIListItem item = GetItem(0);
                //    if (item != null)
                //    {
                //        if (item.IsFolder && item.Label == _parentDirectoryLabel)
                //        {
                //            _currentGroupId = Guid.Empty;
                //            LoadDirectory(false);
                //            return;
                //        }
                //    }
                //}
            }
            switch (action.wID)
            {
                case Action.ActionType.ACTION_DELETE_ITEM:
                    {
                        //int item = GetSelectedItemNo();
                        //if (item >= 0)
                        //    OnDeleteRecording(item);
                        //UpdateProperties();
                    }
                    break;
            }
            base.OnAction(action);
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            _selectedItemIndex = GetSelectedItemNo();
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
            base.OnPageDestroy(newWindowId);
        }

        protected override void OnPageLoad()
        {
            ForTheRecordMain.EnsureConnection();

            base.OnPageLoad();

            if (ServiceChannelFactories.IsInitialized)
            {
                LoadSettings();
                LoadUpcomingPrograms();

                //_sortByButton.SortChanged += new SortEventHandler(SortChanged);
            }
        }

        protected override void OnClicked(int controlId, GUIControl control, global::MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            base.OnClicked(controlId, control, actionType);

            if (control == _programTypeButton)
            {
                switch (_currentProgramType)
                {
                    case ScheduleType.Recording:
                        _currentProgramType = ScheduleType.Alert;
                        break;
                    case ScheduleType.Alert:
                        _currentProgramType = ScheduleType.Suggestion;
                        break;
                    case ScheduleType.Suggestion:
                        _currentProgramType = ScheduleType.Recording;
                        break;
                }
                _selectedItemIndex = 0;
                LoadUpcomingPrograms();
            }

#if false
            if (control == _sortByButton) // sort by
            {
                switch (_currentSortMethod)
                {
                    case SortMethod.Channel:
                        _currentSortMethod = SortMethod.Date;
                        break;
                    case SortMethod.Date:
                        _currentSortMethod = SortMethod.Name;
                        break;
                    case SortMethod.Name:
                        _currentSortMethod = SortMethod.Genre;
                        break;
                    case SortMethod.Genre:
                        _currentSortMethod = SortMethod.Played;
                        break;
                    case SortMethod.Played:
                        _currentSortMethod = SortMethod.Duration;
                        break;
                    case SortMethod.Duration:
                        _currentSortMethod = SortMethod.Channel;
                        break;
                }
                OnSort();
            }
#endif

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
            if (pItem.IsFolder) return;
            UpcomingProgram upcoming = pItem.TVTag as UpcomingProgram;
            if (upcoming != null)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(upcoming.Title);
                dlg.AddLocalizedString(264); // Record
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
                    case 0: // Record
                        OnEditSchedule(upcoming);
                        break;

                    case 1: // Priority
                        OnChangePriority(upcoming);
                        break;

                    case 2: // (Un)Cancel
                        if (upcoming.IsCancelled)
                        {
                            this.TvSchedulerAgent.UncancelUpcomingProgram(upcoming.ScheduleId, upcoming.GuideProgramId, upcoming.Channel.ChannelId, upcoming.StartTime);
                        }
                        else
                        {
                            this.TvSchedulerAgent.CancelUpcomingProgram(upcoming.ScheduleId, upcoming.GuideProgramId, upcoming.Channel.ChannelId, upcoming.StartTime);
                        }
                        _selectedItemIndex = GetSelectedItemNo();
                        LoadUpcomingPrograms();
                        break;
                }
            }
        }

        public override void Process()
        {
            base.Process();
        }

        #endregion

        #region recording methods

        private void LoadUpcomingPrograms()
        {
            string strDefaultUnseenIcon = GUIGraphicsContext.Skin + @"\Media\defaultVideoBig.png";
            string strDefaultSeenIcon = GUIGraphicsContext.Skin + @"\Media\defaultVideoSeenBig.png";
            GUIControl.ClearControl(GetID, _viewsList.GetID);

            if (_currentProgramType == ScheduleType.Recording)
            {
                UpcomingOrActiveProgramsList upcomingRecordings = new UpcomingOrActiveProgramsList(
                    this.TvControlAgent.GetAllUpcomingRecordings(UpcomingRecordingsFilter.Recordings|UpcomingRecordingsFilter.CancelledByUser, false));
                foreach (UpcomingOrActiveProgramView recording in upcomingRecordings)
                {
                    GUIListItem item = CreateListItem(recording.UpcomingRecording.Program, recording.UpcomingRecording);
                    _viewsList.Add(item);
                }
            }
            else
            {
                List<UpcomingProgram> upcomingPrograms = new List<UpcomingProgram>(
                    this.TvSchedulerAgent.GetAllUpcomingPrograms(_currentProgramType, true));
                foreach (UpcomingProgram program in upcomingPrograms)
                {
                    GUIListItem item = CreateListItem(program, null);
                    _viewsList.Add(item);
                }
            }

            string strObjects = string.Format("{0} {1}", _viewsList.Count, Utility.GetLocalizedText(TextId.RecordingsListItemsSuffix));
            GUIPropertyManager.SetProperty("#itemcount", strObjects);

            UpdateButtonStates(); // OnSort();
            UpdateProperties();

            if (GetItemCount() > 0)
            {
                while (_selectedItemIndex >= GetItemCount() && _selectedItemIndex > 0)
                {
                    _selectedItemIndex--;
                }
                GUIControl.SelectItemControl(GetID, _viewsList.GetID, _selectedItemIndex);
            }
        }

        private GUIListItem CreateListItem(UpcomingProgram upcomingProgram, UpcomingRecording recording)
        {
            GUIListItem item = new GUIListItem();
            string title = upcomingProgram.CreateProgramTitle();
            item.Label = title;
            //item.OnItemSelected += new global::MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
            string logoImagePath = Utility.GetLogoImage(upcomingProgram.Channel, TvSchedulerAgent);
            if (logoImagePath == null
                || !System.IO.File.Exists(logoImagePath))
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
            return item;
        }

        private void UpdateButtonStates()
        {
            string strLine = string.Empty;
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
                case SortMethod.Genre:
                    strLine =  Utility.GetLocalizedText(TextId.SortByGenre);
                    break;
                case SortMethod.Played:
                    strLine = Utility.GetLocalizedText(TextId.SortByWatched);
                    break;
                case SortMethod.Duration:
                    strLine = Utility.GetLocalizedText(TextId.SortByDuration);
                    break;
            }
            //GUIControl.SetControlLabel(GetID, _sortByButton.GetID, strLine);
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

            //_sortByButton.IsAscending = _sortAscending;

            _viewsList.IsVisible = true;
        }

        private bool OnEditSchedule(int iItem)
        {
            GUIListItem pItem = GetItem(iItem);
            if (pItem != null
                && !pItem.IsFolder)
            {
                return OnEditSchedule(pItem.TVTag as UpcomingProgram);
            }
            return false;
        }

        private bool OnEditSchedule(UpcomingProgram upcoming)
        {
            if (upcoming != null
                && upcoming.GuideProgramId.HasValue)
            {
                TvProgramInfo.CurrentProgram = this.TvGuideAgent.GetProgramById(upcoming.GuideProgramId.Value);
                TvProgramInfo.Channel = upcoming.Channel;
                GUIWindowManager.ActivateWindow((int)WindowId.ProgramInfo);
                return true;
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
                        TvSchedulerAgent.SetUpcomingProgramPriority(upcoming.UpcomingProgramId, upcoming.StartTime, priority);
                        _selectedItemIndex = GetSelectedItemNo();
                        LoadUpcomingPrograms();
                    }
                }
            }
        }

        private void UpdateProperties()
        {
            GUIListItem pItem = GetItem(GetSelectedItemNo());
            if (pItem == null)
            {
                SetProperties(null);
                return;
            }
            UpcomingProgram upcoming = pItem.TVTag as UpcomingProgram;
            SetProperties(upcoming);
        }

        private void SetProperties(UpcomingProgram upcoming)
        {
            if (upcoming == null)
            {
                GUIPropertyManager.SetProperty("#TV.Upcoming.Channel", String.Empty);
                GUIPropertyManager.SetProperty("#TV.Upcoming.Title", String.Empty);
                GUIPropertyManager.SetProperty("#TV.Upcoming.Genre", String.Empty);
                GUIPropertyManager.SetProperty("#TV.Upcoming.Time", String.Empty);
                GUIPropertyManager.SetProperty("#TV.Upcoming.Description", " ");
                GUIPropertyManager.SetProperty("#TV.Upcoming.thumb", String.Empty);
            }
            else
            {
                GuideProgram guideProgram = upcoming.GuideProgramId.HasValue ?
                    this.TvGuideAgent.GetProgramById(upcoming.GuideProgramId.Value) : null;

                string strTime = string.Format("{0} {1} - {2}",
                  Utility.GetShortDayDateString(upcoming.StartTime),
                  upcoming.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                  upcoming.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                GUIPropertyManager.SetProperty("#TV.Upcoming.Channel", upcoming.Channel.DisplayName);
                GUIPropertyManager.SetProperty("#TV.Upcoming.Title", upcoming.Title);
                GUIPropertyManager.SetProperty("#TV.Upcoming.Genre", upcoming.Category);
                GUIPropertyManager.SetProperty("#TV.Upcoming.Time", strTime);
                string description;
                if (guideProgram == null)
                {
                    description = String.Empty;
                }
                else
                {
                    description = guideProgram.CreateCombinedDescription(true);
                }
                GUIPropertyManager.SetProperty("#TV.Upcoming.Description", description);

                string logo = Utility.GetLogoImage(upcoming.Channel.ChannelId, upcoming.Channel.DisplayName, TvSchedulerAgent);
                if (System.IO.File.Exists(logo))
                {
                    GUIPropertyManager.SetProperty("#TV.Upcoming.thumb", logo);
                }
                else
                {
                    GUIPropertyManager.SetProperty("#TV.Upcoming.thumb", "defaultVideoBig.png");
                }
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

            int resultLower = _sortAscending ? -1 : 1;
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
                    case SortMethod.Name:
                        result = resultUpper * String.Compare(item1.Label, item2.Label, true);
                        if (result == 0)
                        {
                            goto case SortMethod.Channel;
                        }
                        break;

                    case SortMethod.Channel:
                    case SortMethod.Played:
                    case SortMethod.Date:
                    case SortMethod.Genre:
                        RecordingGroup group1 = item1.TVTag as RecordingGroup;
                        RecordingGroup group2 = item2.TVTag as RecordingGroup;
                        result = (group1.LatestProgramStartTime < group2.LatestProgramStartTime) ? resultUpper : resultLower;
                        break;
                }
            }
            else
            {
                Recording rec1 = item1.TVTag as Recording;
                Recording rec2 = item2.TVTag as Recording;

                switch (_currentSortMethod)
                {
                    case SortMethod.Played:
                        //item1.Label2 = string.Format("{0} {1}", rec1.TimesWatched, Utility.GetLocalizedText(TextId.Times));//times
                        //item2.Label2 = string.Format("{0} {1}", rec2.TimesWatched, Utility.GetLocalizedText(TextId.Times));//times
                        if (rec1.LastWatchedPosition == rec2.LastWatchedPosition)
                        {
                            goto case SortMethod.Name;
                        }
                        result = rec2.LastWatchedPosition.HasValue ? resultUpper : resultLower;
                        break;

                    case SortMethod.Name:
                        result = resultUpper * String.Compare(rec1.CreateProgramTitle(), rec2.CreateProgramTitle(), true);
                        if (result == 0)
                        {
                            goto case SortMethod.Channel;
                        }
                        break;

                    case SortMethod.Channel:
                        result = resultUpper * String.Compare(rec1.ChannelDisplayName, rec2.ChannelDisplayName, true);
                        if (result == 0)
                        {
                            goto case SortMethod.Date;
                        }
                        break;

                    case SortMethod.Date:
                        if (rec1.ProgramStartTime != rec2.ProgramStartTime)
                        {
                            result = (rec1.ProgramStartTime < rec2.ProgramStartTime) ? resultUpper : resultLower;
                        }
                        break;

                    case SortMethod.Genre:
                        item1.Label2 = rec1.Category;
                        item2.Label2 = rec2.Category;
                        result = resultUpper * String.Compare(rec1.Category, rec2.Category, true);
                        if (result == 0)
                        {
                            if (rec1.ProgramStartTime != rec2.ProgramStartTime)
                            {
                                result = (rec1.ProgramStartTime < rec2.ProgramStartTime) ? resultUpper : resultLower;
                            }
                            else if (rec1.ChannelId != rec2.ChannelId)
                            {
                                result = resultUpper * String.Compare(rec1.ChannelDisplayName, rec2.ChannelDisplayName);
                            }
                            else
                            {
                                result = resultUpper * String.Compare(rec1.CreateProgramTitle(), rec2.CreateProgramTitle());
                            }
                        }
                        break;
                }
            }

            return result;
        }

        void SortChanged(object sender, SortEventArgs e)
        {
            _sortAscending = e.Order != System.Windows.Forms.SortOrder.Descending;
            OnSort();
        }

        #endregion
    }
}
