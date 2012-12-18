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

namespace ForTheRecord.UI.MediaPortal
{
    public class ActiveRecordings : GUIWindow, IComparer<GUIListItem>
    {
        #region variables

        private enum SortMethod
        {
            Channel = 0,
            Date = 1,
            Name = 2,
            Genre = 3,
            Played = 4,
            Duration = 5
        }

        private SortMethod _currentSortMethod = SortMethod.Date;

        private bool _sortAscending = true;
        private int _selectedItemIndex;

        [SkinControl(10)]
        protected GUIListControl _viewsList;

        #endregion

        public ActiveRecordings()
        {
            GetID = (int)WindowId.ActiveRecordings;
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
            Log.Info("ActiveRecordings:OnAdded");
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
            bool bResult = Load(GUIGraphicsContext.Skin + @"\4TR_Active.xml");
            LoadSettings();
            Restore();
            PreInit();
            ResetAllControls();
            return bResult;
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_DELETE_ITEM:
                    {
                        int itemIndex = GetSelectedItemNo();
                        if (itemIndex >= 0)
                        {
                            OnAbortRecording(GetItem(itemIndex));
                        }
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
            base.OnPageLoad();

            LoadSettings();
            LoadActiveRecordings();

            GUIControl.FocusControl(GetID, 10);

            //_sortByButton.SortChanged += new SortEventHandler(SortChanged);
        }

        protected override void OnClicked(int controlId, GUIControl control, global::MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            base.OnClicked(controlId, control, actionType);

            if (control == _viewsList)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, control.GetID, 0, 0, null);
                OnMessage(msg);
                int iItem = (int)msg.Param1;
                if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
                {
                    OnPlayRecording(iItem);
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
            ActiveRecording activeRecording = pItem.TVTag as ActiveRecording;
            if (activeRecording != null)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(activeRecording.Program.Title);
                dlg.AddLocalizedString(655);  //Play recorded tv
                dlg.AddLocalizedString(1449); //Stop recording
                dlg.DoModal(GetID);
                switch (dlg.SelectedId)
                {
                    case 655: // Play
                        PlayRecording(activeRecording, false);
                        break;

                    case 1449: // Abort
                        OnAbortRecording(pItem);
                        break;
                }
            }
        }

        private void OnAbortRecording(GUIListItem item)
        {
            if (item == null) return;
            ActiveRecording activeRecording = item.TVTag as ActiveRecording;
            if (activeRecording != null)
            {
                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                if (dlgYesNo != null)
                {
                    UpcomingProgram program = activeRecording.Program;

                    dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.StopRecording));
                    dlgYesNo.SetLine(1, program.Channel.DisplayName);
                    dlgYesNo.SetLine(2, program.Title);
                    dlgYesNo.SetLine(3, string.Empty);
                    dlgYesNo.SetDefaultToYes(false);
                    dlgYesNo.DoModal(GetID);

                    if (dlgYesNo.IsConfirmed)
                    {
                        this.TvSchedulerAgent.CancelUpcomingProgram(program.ScheduleId, program.GuideProgramId,
                            program.Channel.ChannelId, program.StartTime);
                        _viewsList.ListItems.Remove(item);
                        GUIControl.RefreshControl(GetID, _viewsList.GetID);
                        UpdateProperties();
                        _selectedItemIndex = GetSelectedItemNo();
                    }
                }
            }
        }

        private bool OnPlayRecording(int itemIndex)
        {
            GUIListItem item = GetItem(itemIndex);
            if (item == null) return false;

            ActiveRecording activeRecording = item.TVTag as ActiveRecording;
            if (activeRecording != null)
            {
                return PlayRecording(activeRecording, false);
            }
            return false;
        }

        internal static bool PlayRecording(ActiveRecording activeRecording, bool jumpToLivePoint)
        {
            using (TvControlServiceAgent tvControlAgent = new TvControlServiceAgent())
            {
                Recording recording = tvControlAgent.GetRecordingById(activeRecording.RecordingId);
                if (recording != null)
                {
                    int? jumpTo = null;
                    if (jumpToLivePoint)
                    {
                        TimeSpan duration = DateTime.Now - activeRecording.RecordingStartTime;
                        jumpTo = (int)duration.TotalSeconds - 3;
                    }
                    RecordedBase.PlayRecording(recording, jumpTo);
                    return true;
                }
                return false;
            }
        }

        public override void Process()
        {
            base.Process();
        }

        #endregion

        #region recording methods

        private void LoadActiveRecordings()
        {
            string strDefaultUnseenIcon = GUIGraphicsContext.Skin + @"\Media\defaultVideoBig.png";
            string strDefaultSeenIcon = GUIGraphicsContext.Skin + @"\Media\defaultVideoSeenBig.png";
            GUIControl.ClearControl(GetID, _viewsList.GetID);

            List<ActiveRecording> activeRecordings = new List<ActiveRecording>(
                this.TvControlAgent.GetActiveRecordings());
            foreach (ActiveRecording recording in activeRecordings)
            {
                GUIListItem item = CreateListItem(recording);
                _viewsList.Add(item);
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

        private GUIListItem CreateListItem(ActiveRecording activeRecording)
        {
            GUIListItem item = new GUIListItem();
            string title = activeRecording.Program.CreateProgramTitle();
            item.Label = title;
            //item.OnItemSelected += new global::MediaPortal.GUI.Library.GUIListItem.ItemSelectedHandler(item_OnItemSelected);
            string logoImagePath = Utility.GetLogoImage(activeRecording.Program.Channel, TvSchedulerAgent);
            if (logoImagePath == null
                || !System.IO.File.Exists(logoImagePath))
            {
                item.Label = String.Format("[{0}] {1}", activeRecording.Program.Channel.DisplayName, title);
                logoImagePath = "defaultVideoBig.png";
            }
            item.PinImage = Utility.GetIconImageFileName(activeRecording);
            item.TVTag = activeRecording;
            item.ThumbnailImage = logoImagePath;
            item.IconImageBig = logoImagePath;
            item.IconImage = logoImagePath;
            item.Label2 = String.Format("{0} {1} - {2}", Utility.GetShortDayDateString(activeRecording.Program.StartTime),
                activeRecording.Program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                activeRecording.Program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
            return item;
        }

        private void UpdateButtonStates()
        {
            //_sortByButton.IsAscending = _sortAscending;
            _viewsList.IsVisible = true;
        }

        private void UpdateProperties()
        {
            ActiveRecording recording = null;
            GUIListItem item = GetItem(GetSelectedItemNo());
            if (item != null)
            {
                recording = item.TVTag as ActiveRecording;
            }
            SetProperties(recording);
        }

        private void SetProperties(ActiveRecording recording)
        {
            if (recording == null)
            {
                GUIPropertyManager.SetProperty("#TV.Active.Channel", String.Empty);
                GUIPropertyManager.SetProperty("#TV.Active.Title", String.Empty);
                GUIPropertyManager.SetProperty("#TV.Active.Genre", String.Empty);
                GUIPropertyManager.SetProperty("#TV.Active.Time", String.Empty);
                GUIPropertyManager.SetProperty("#TV.Active.Description", " ");
                GUIPropertyManager.SetProperty("#TV.Active.thumb", String.Empty);
            }
            else
            {
                GuideProgram guideProgram = recording.Program.GuideProgramId.HasValue ?
                    this.TvGuideAgent.GetProgramById(recording.Program.GuideProgramId.Value) : null;

                string strTime = string.Format("{0} {1} - {2}",
                  Utility.GetShortDayDateString(recording.Program.StartTime),
                  recording.Program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                  recording.Program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                GUIPropertyManager.SetProperty("#TV.Active.Channel", recording.Program.Channel.DisplayName);
                GUIPropertyManager.SetProperty("#TV.Active.Title", recording.Program.Title);
                GUIPropertyManager.SetProperty("#TV.Active.Genre", recording.Program.Category);
                GUIPropertyManager.SetProperty("#TV.Active.Time", strTime);
                string description;
                if (guideProgram == null)
                {
                    description = String.Empty;
                }
                else
                {
                    description = guideProgram.CreateCombinedDescription(true);
                }
                GUIPropertyManager.SetProperty("#TV.Active.Description", description);

                string logo = Utility.GetLogoImage(recording.Program.Channel.ChannelId,
                    recording.Program.Channel.DisplayName, TvSchedulerAgent);
                if (System.IO.File.Exists(logo))
                {
                    GUIPropertyManager.SetProperty("#TV.Active.thumb", logo);
                }
                else
                {
                    GUIPropertyManager.SetProperty("#TV.Active.thumb", "defaultVideoBig.png");
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
                        result = (group1.LatestProgramStartTime < group1.LatestProgramStartTime) ? resultUpper : resultLower;
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
                        //item1.Label2 = string.Format("{0} {1}", rec1.TimesWatched, Utility.GetLocalizedText(TextId.Times));
                        //item2.Label2 = string.Format("{0} {1}", rec2.TimesWatched, Utility.GetLocalizedText(TextId.Times));
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
