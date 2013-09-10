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
using System.Data;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Services;
using MediaPortal.Threading;
using MediaPortal.Util;
using MediaPortal.Configuration;
using Action = MediaPortal.GUI.Library.Action;
using WindowPlugins;
using Layout = MediaPortal.GUI.Library.GUIFacadeControl.Layout;
using MediaPortal.Profile;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;
using ArgusTV.UI.Process.Recordings;
using ArgusTV.UI.Process;

namespace ArgusTV.UI.MediaPortal
{
    public abstract class RecordedBase : WindowPluginBase, IComparer<GUIListItem>
    {
        protected abstract string SettingsSection { get; }
        public static bool NeedUpdate { get; set; }

        public RecordedBase(ChannelType channelType)
        {
            _channelType = channelType;
        }
        
        #region variables

        [SkinControl(6)] protected GUIButtonControl _cleanUpButton;

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
            Name = 2
        }

        private enum cleanupMethod
        {
            None = 0,
            AllWatched = 1,
            AllInvalid = 2,
            AllInvalidAndWatched = 3,
            WatchedInThisFolder = 4
        }

        private const string _parentDirectoryLabel = "..";
        private static Recording _playingRecording = null;
        private static string _playingRecordingFileName = null;
        private RecordingsModel _model;
        private RecordingsController _controller;

        private RecordingGroupMode _currentGroupByMode = RecordingGroupMode.GroupByProgramTitle;
        private SortMethod _currentSortMethod = SortMethod.Date;
        private cleanupMethod _cleanUpMethod = cleanupMethod.None;

        private int _selectedItemIndex;
        private int _selectedParentItemIndex;
        private object _currentGroupId;
        private int _thumbSize = 500;
        private ChannelType _channelType;
        private DateTime _updateTimer = DateTime.MinValue;
        private DateTime _updateTimer2 = DateTime.MinValue;

        #endregion

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

        #region overrides

        public override bool IsTv
        {
            get { return true; }
        }

        public override bool Init()
        {
            g_Player.PlayBackStopped += new global::MediaPortal.Player.g_Player.StoppedHandler(OnPlayRecordingBackStopped);
            g_Player.PlayBackEnded += new global::MediaPortal.Player.g_Player.EndedHandler(OnPlayRecordingBackEnded);
            g_Player.PlayBackStarted += new global::MediaPortal.Player.g_Player.StartedHandler(OnPlayRecordingBackStarted);
            g_Player.PlayBackChanged += new global::MediaPortal.Player.g_Player.ChangedHandler(OnPlayRecordingBackChanged);
            return true;
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_DELETE_ITEM:
                    {
                        int item = GetSelectedItemIndex();
                        if (item >= 0)
                            OnDeleteRecording(item);
                        UpdateProperties();
                    }
                    break;

                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    if (facadeLayout != null)
                    {
                        if (facadeLayout.Focus)
                        {
                            GUIListItem item = GetItem(0);
                            if (item != null)
                            {
                                if (item.IsFolder && item.Label == _parentDirectoryLabel)
                                {
                                    _currentGroupId = null;
                                    LoadDirectory(false);
                                    SelectItemByIndex(ref _selectedParentItemIndex);
                                    return;
                                }
                            }
                        }
                    }
                    break;
            }
            base.OnAction(action);
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            _selectedItemIndex = GetSelectedItemIndex();
            StopLoadingThumbs();
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
            if (newWindowId != WindowId.RecordedTvInfo)
            {
                _model = null;
            }
            base.OnPageDestroy(newWindowId);
        }

        protected override void OnPageLoad()
        {
            PluginMain.EnsureConnection();
            base.OnPageLoad();

            InitViewSelections();
            if (PluginMain.IsConnected())
            {
                if (_model == null)
                {
                    _model = new RecordingsModel();
                    _controller = new RecordingsController(_model);
                    _controller.Initialize();
                    _controller.SetChannelType(_channelType);
                }

                LoadSettings();
                LoadDirectory(true);
                SelectItemByIndex(ref _selectedItemIndex);

                btnSortBy.SortChanged += new SortEventHandler(SortChanged);
                SetRecordingDiskInfo(true);

                if (this._channelType == ChannelType.Television)
                    GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100603));
                else
                    GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100763));
            }
        }

        protected override void OnClicked(int controlId, GUIControl control, global::MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            base.OnClicked(controlId, control, actionType);
            if (control == btnLayouts)
            {
                LoadDirectory(true);
            }
            else if (control == _cleanUpButton)
            {
                OnCleanUp();
            }
        }

        protected override void OnInfo(int iItem)
        {
            OnShowContextMenu();
        }

        protected override void OnClick(int iItem)
        {
            OnSelectItem(iItem);
        }

        protected override void OnShowSort()
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
            if (dlg == null)
            {
                return;
            }
            dlg.Reset();
            dlg.SetHeading(495); // Sort options
            dlg.AddLocalizedString(620); // channel
            dlg.AddLocalizedString(104); // date
            dlg.AddLocalizedString(268); // title

            // set the focus to currently used sort method
            dlg.SelectedLabel = (int)_currentSortMethod;

            // show dialog and wait for result
            dlg.DoModal(GetID);
            if (dlg.SelectedId == -1)
            {
                return;
            }

            _currentSortMethod = (SortMethod)dlg.SelectedLabel;
            OnSort();
        }

        protected override void InitViewSelections()
        {
            btnViews.ClearMenu();

            // Add the view options to the menu.
            int index = 0;
            btnViews.AddItem(Utility.GetLocalizedText(TextId.GroupByTitle), index++);
            btnViews.AddItem(Utility.GetLocalizedText(TextId.GroupBySchedule), index++); 
            btnViews.AddItem(Utility.GetLocalizedText(TextId.GroupByCategory), index++); 
            btnViews.AddItem(Utility.GetLocalizedText(TextId.GroupByChannel), index++);

            // Have the menu select the currently selected view.
            switch (_currentGroupByMode)
            {
                case RecordingGroupMode.GroupByProgramTitle:
                    btnViews.SetSelectedItemByValue(0);
                    break;
                case RecordingGroupMode.GroupBySchedule:
                    btnViews.SetSelectedItemByValue(1);
                    break;
                case RecordingGroupMode.GroupByCategory:
                    btnViews.SetSelectedItemByValue(2);
                    break;
                case RecordingGroupMode.GroupByChannel:
                    btnViews.SetSelectedItemByValue(3);
                    break;
            }
        }

        protected override void SetView(int selectedViewId)
        {
            try
            {
                switch (selectedViewId)
                {
                    case 0:
                        _currentGroupByMode = RecordingGroupMode.GroupByProgramTitle;
                        break;
                    case 1:
                        _currentGroupByMode = RecordingGroupMode.GroupBySchedule;
                        break;
                    case 2:
                        _currentGroupByMode = RecordingGroupMode.GroupByCategory;
                        break;
                    case 3:
                        _currentGroupByMode = RecordingGroupMode.GroupByChannel;
                        break;
                }

                // If we had been in 2nd group level - go up to root again
                //_currentLabel = String.Empty;
                LoadDirectory(true);
            }
            catch (Exception ex)
            {
                Log.Error("TvRecorded: Error in ShowViews - {0}", ex.ToString());
            }
        }

        protected override bool AllowLayout(Layout layout)
        {
            // Disable playlist for now as it makes no sense to move recording entries
            if (layout == Layout.Playlist)
            {
                return false;
            }
            return true;
        }

        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_ITEM_FOCUS_CHANGED:
                    UpdateProperties();
                    break;

                case GUIMessage.MessageType.GUI_MSG_ITEM_SELECT:
                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    // Depending on the mode, handle the GUI_MSG_ITEM_SELECT message from the dialog menu and
                    // the GUI_MSG_CLICKED message from the spin control.
                    // Respond to the correct control.  The value is retrived directly from the control by the called handler.

                    if (message.TargetControlId == btnViews.GetID)
                    {
                        // Set the new view.
                        SetView(btnViews.SelectedItemValue);
                    }
                    break;
            }
            return base.OnMessage(message);
        }

        protected override void OnShowContextMenu()
        {
            int itemIndex = GetSelectedItemIndex();
            GUIListItem item = GetItem(itemIndex);
            if (item == null
                || item.IsFolder)
            {
                return;
            }
            RecordingSummary rec = item.TVTag as RecordingSummary;
            if (rec != null)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(rec.Title);

                dlg.AddLocalizedString(655);     //Play recorded tv
                dlg.AddLocalizedString(656);     //Delete recorded tv
                if (item.IsPlayed)
                {
                    dlg.AddLocalizedString(830); //Reset watched status
                }
                dlg.AddLocalizedString(1042);//keep until

                dlg.DoModal(GetID);
                if (dlg.SelectedLabel == -1) return;
                switch (dlg.SelectedId)
                {
                    case 656: // delete
                        OnDeleteRecording(itemIndex);
                        break;

                    case 655: // play
                        OnSelectItem(itemIndex);
                        break;

                    case 830: // Reset watched status
                        ControlAgent.SetRecordingLastWatchedPosition(rec.RecordingFileName, null);
                        rec.LastWatchedPosition = null;
                        rec.LastWatchedTime = null;
                        item.IsPlayed = false;
                        GUIControl.RefreshControl(GetID, facadeLayout.GetID);
                        break;

                    case 1042: // keep until
                        OnkeepUntil(rec);
                        break;
                }
            }
        }

        public override void Process()
        {
            TimeSpan ts = DateTime.Now - _updateTimer;
            if (ts.TotalMilliseconds > 5000)
            {
                _updateTimer = DateTime.Now;
                SetRecordingDiskInfo(false);
            }

            if (NeedUpdate)
            {
                TimeSpan ts2 = DateTime.Now - _updateTimer2;
                if (ts2.TotalMilliseconds > 5000)
                {
                    NeedUpdate = false;
                    StartLoadingThumbs();
                }
            }
            else
            {
                _updateTimer2 = DateTime.Now;
            }
        }

        #endregion

        internal static Recording GetPlayingRecording()
        {
            return _playingRecording;
        }

        internal static bool PlayFromLivePoint(ActiveRecording rec)
        {
            using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
            {
                if (rec == null) return false;
                return PlayRecording(tvControlAgent.GetRecordingById(rec.RecordingId), false, true, null);
            }
        }

        internal static bool PlayFromPreRecPoint(Recording rec)
        {
            if (rec == null) return false;
            return PlayRecording(rec, true, false, null);
        }

        internal static bool PlayRecording(Recording rec, int? jumpTo)
        {
            return PlayRecording(rec, false, false, jumpTo);
        }

        private static bool PlayRecording(Recording rec,bool jumpToPrerecTime,bool jumpToLivePoint, int? jumpTo)
        {
            int jumpToTime = 0;
            if (!jumpTo.HasValue)
            {
                if (jumpToPrerecTime)
                {
                    jumpToTime = 0;
                }
                else if (jumpToLivePoint)
                {
                    jumpToTime = -1;
                }
                else
                {
                    bool _isLiveRecording = false;
                    if (PluginMain.IsRecordingStillActive(rec.RecordingId))
                    {
                        _isLiveRecording = true;
                    }

                    if (rec.LastWatchedPosition.HasValue)
                    {
                        jumpToTime = rec.LastWatchedPosition.Value;
                        if (jumpToTime <= 10) jumpToTime = 0;
                    }

                    if (_isLiveRecording || jumpToTime > 0)
                    {
                        GUIResumeDialog.MediaType _mediaType = GUIResumeDialog.MediaType.Recording;
                        if (_isLiveRecording)
                        {
                            _mediaType = GUIResumeDialog.MediaType.LiveRecording;
                        }

                        GUIResumeDialog.Result result =
                          GUIResumeDialog.ShowResumeDialog(rec.Title, jumpToTime, _mediaType);

                        switch (result)
                        {
                            case GUIResumeDialog.Result.Abort:
                                return false;
                            case GUIResumeDialog.Result.PlayFromBeginning:
                                jumpToTime = 0;
                                break;
                            case GUIResumeDialog.Result.PlayFromLivePoint:
                                jumpToTime = -1; // magic -1 is used for the live point
                                break;
                            default: // from last stop time and on error
                                break;
                        }
                    }
                }

                if (jumpToTime == 0)
                {
                    //start at prerec time
                    DateTime startTime = DateTime.Now.AddSeconds(-10);
                    if (rec.ProgramStartTime < startTime) startTime = rec.ProgramStartTime;
                    TimeSpan preRecordSpan = startTime - rec.RecordingStartTime;
                    jumpToTime = (int)preRecordSpan.TotalSeconds;
                    if (jumpToTime < 0) jumpToTime = 0;
                }
            }
            else
            {
                jumpToTime = jumpTo.Value;
                if (jumpToTime < 0) jumpToTime = 0;
            }

            if (g_Player.Playing)
            {
                if (_playingRecordingFileName != null
                    && g_Player.currentFileName == _playingRecordingFileName)
                {
                    int stoptime = g_Player.CurrentPosition < g_Player.Duration ? (int)g_Player.CurrentPosition : 0;
                    StopCurrentPlayback(_playingRecordingFileName, stoptime, true);
                }
                g_Player.Stop(true);
                Thread.Sleep(250);
            }

            if (StartPlayingRec(rec, jumpToTime))
            {
                using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                {
                    tvControlAgent.SetRecordingLastWatched(rec.RecordingFileName);
                    rec.LastWatchedTime = DateTime.Now;
                }
                return true;
            }
            return false;
        }

        private static bool StartPlayingRec(Recording rec, int jumpToTime)
        {
            string fileName = rec.RecordingFileName;
            if (PluginMain.PlayRecordingsOverRtsp)
            {
                using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                {
                    fileName = tvControlAgent.StartRecordingStream(fileName);
                }
            }

            PluginMain.Navigator.LastChannelChangeFailed = false;

            _playingRecording = rec;
            _playingRecordingFileName = fileName;

            //populates recording metadata to g_player;
            g_Player.currentFileName = fileName;
            g_Player.currentTitle = rec.Title;
            g_Player.currentDescription = rec.CreateCombinedDescription(true);

            if (g_Player.Play(fileName,
                rec.ChannelType == ChannelType.Television ? g_Player.MediaType.Recording : g_Player.MediaType.RadioRecording))
            {
                if (Utils.IsVideo(fileName) && !g_Player.IsRadio)
                {
                    g_Player.ShowFullScreenWindow();
                }

                double duration = g_Player.Duration - 5;
                if (jumpToTime > 0 && jumpToTime < duration)
                {
                    g_Player.SeekAbsolute(jumpToTime);
                }
                else if (jumpToTime == -1 || (jumpToTime > 0 && jumpToTime >= duration))
                {
                    // 5 second margin is used that the TsReader wont stop playback right after it has been started
                    if (duration > 0) g_Player.SeekAbsolute(duration);
                }
                return true;
            }
            else
            {
                StopCurrentPlayback();
            }
            return false;
        }

        #region Serialisation

        protected override void LoadSettings()
        {
            base.LoadSettings();
            using (Settings xmlreader = new MPSettings())
            {
                currentLayout = (Layout)xmlreader.GetValueAsInt(this.SettingsSection, "layout", (int)Layout.List);
                m_bSortAscending = xmlreader.GetValueAsBool(this.SettingsSection, "sortascending", true);

                string strTmp = xmlreader.GetValueAsString(this.SettingsSection, "sort", "date");
                if (strTmp == "channel") _currentSortMethod = SortMethod.Channel;
                else if (strTmp == "date") _currentSortMethod = SortMethod.Date;
                else if (strTmp == "name") _currentSortMethod = SortMethod.Name;

                strTmp = xmlreader.GetValueAsString(this.SettingsSection, "group", "title");
                if (strTmp == "channel") _currentGroupByMode = RecordingGroupMode.GroupByChannel;
                else if (strTmp == "title") _currentGroupByMode = RecordingGroupMode.GroupByProgramTitle;
                else if (strTmp == "schedule") _currentGroupByMode = RecordingGroupMode.GroupBySchedule;
                else if (strTmp == "category") _currentGroupByMode = RecordingGroupMode.GroupByCategory;
                else if (strTmp == "day") _currentGroupByMode = RecordingGroupMode.GroupByProgramTitle;

                int thumbQuality = xmlreader.GetValueAsInt("thumbnails", "quality", 2);
                switch (thumbQuality)
                {
                    case 2:
                    case 3:
                        _thumbSize = 500;
                        break;
                    case 4:
                        _thumbSize = 600;
                        break;
                    default:
                        _thumbSize = 400;
                        break;
                }
            }
        }

        protected override void SaveSettings()
        {
            base.SaveSettings();
            using (Settings xmlwriter = new MPSettings())
            {
                xmlwriter.SetValue(this.SettingsSection, "layout", (int)currentLayout);
                xmlwriter.SetValueAsBool(this.SettingsSection, "sortascending", m_bSortAscending);

                switch (_currentSortMethod)
                {
                    case SortMethod.Channel:
                        xmlwriter.SetValue(this.SettingsSection, "sort", "channel");
                        break;
                    case SortMethod.Date:
                        xmlwriter.SetValue(this.SettingsSection, "sort", "date");
                        break;
                    case SortMethod.Name:
                        xmlwriter.SetValue(this.SettingsSection, "sort", "name");
                        break;
                }

                switch (_currentGroupByMode)
                {
                    case RecordingGroupMode.GroupByChannel:
                        xmlwriter.SetValue(this.SettingsSection, "group", "channel");
                        break;
                    case RecordingGroupMode.GroupBySchedule:
                        xmlwriter.SetValue(this.SettingsSection, "group", "schedule");
                        break;
                    case RecordingGroupMode.GroupByCategory:
                        xmlwriter.SetValue(this.SettingsSection, "group", "category");
                        break;
                    case RecordingGroupMode.GroupByProgramTitle:
                    default:
                        xmlwriter.SetValue(this.SettingsSection, "group", "title");
                        break;
                }
            }
        }
        #endregion

        #region recording methods

        private void LoadDirectory(bool reload)
        {
            Log.Info("TvRecorded: LoadDirectory , reload = {0}",reload);
            StopLoadingThumbs();

            if (reload)
            {
                _controller.ReloadRecordingGroups(ControlAgent, _currentGroupByMode);
                if (_currentGroupId != null)
                {
                    _currentGroupId = CheckValidGroupId(_currentGroupId);
                }
            }

            string strDefaultUnseenIcon = GUIGraphicsContext.Skin + @"\Media\defaultVideoBig.png";
            string strDefaultSeenIcon = GUIGraphicsContext.Skin + @"\Media\defaultVideoSeenBig.png";
            GUIControl.ClearControl(GetID, facadeLayout.GetID);

            List<GUIListItem> itemlist = new List<GUIListItem>();
            bool isSubDirectory = false;
            int groupIndex = 0;

            foreach (RecordingGroup recordingGroup in _model.RecordingGroups)
            {
                RecordingSummary[] recordings = null;
                if (_currentGroupId == null)
                {
                    if (recordingGroup.RecordingGroupMode != RecordingGroupMode.GroupByChannel
                        && recordingGroup.RecordingGroupMode != RecordingGroupMode.GroupByCategory
                        && recordingGroup.RecordingsCount == 1)
                    {
                        recordings = _controller.GetRecordingsForGroup(ControlAgent, groupIndex, true);
                    }
                }
                else if ((recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupBySchedule && recordingGroup.ScheduleId == (Guid)_currentGroupId)
                    || (recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupByChannel && recordingGroup.ChannelId == (Guid)_currentGroupId)
                    || (recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupByProgramTitle && recordingGroup.ProgramTitle == (string)_currentGroupId)
                    || (recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupByCategory && EnsureCategoryName(recordingGroup.Category) == (string)_currentGroupId))
                {
                    recordings = _controller.GetRecordingsForGroup(ControlAgent, groupIndex, true);
                    isSubDirectory = true;

                    GUIListItem item = new GUIListItem();
                    item.Label = _parentDirectoryLabel;
                    item.IsFolder = true;
                    Utils.SetDefaultIcons(item);
                    item.ThumbnailImage = item.IconImageBig;
                    itemlist.Add(item);
                }
                if (recordings != null)
                {
                    foreach (RecordingSummary rec in recordings)
                    {
                        GUIListItem item = new GUIListItem();
                        item.Label = rec.CreateProgramTitle();
                        item.TVTag = rec;

                        // Get the channel logo for the small icons
                        string logo = Utility.GetLogoImage(rec.ChannelId, rec.ChannelDisplayName, SchedulerAgent);
                        if (!Utils.FileExistsInCache(logo))
                        {
                            logo = rec.LastWatchedTime.HasValue ? strDefaultSeenIcon : strDefaultUnseenIcon;
                        }
                        if (PluginMain.IsRecordingStillActive(rec.RecordingId))
                        {
                            item.PinImage = Thumbs.TvRecordingIcon;
                        }

                        item.IconImage = logo;
                        itemlist.Add(item);
                    }
                }
                else if (_currentGroupId == null)
                {
                    GUIListItem item = new GUIListItem();
                    item.IsFolder = true;
                    Utils.SetDefaultIcons(item);
                    if (recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupByChannel)
                    {
                        string logo = Utility.GetLogoImage(recordingGroup.ChannelId, recordingGroup.ChannelDisplayName, SchedulerAgent);
                        if (File.Exists(logo))
                        {
                            item.ThumbnailImage = logo;
                            item.IconImage = logo;
                            item.IconImageBig = logo;
                        }
                    }
                    item.ThumbnailImage = item.IconImageBig;
                    item.Label = GetLabelForGroup(recordingGroup, Utility.GetLocalizedText(TextId.RecordingsGroupNameRecordings), Utility.GetLocalizedText(TextId.Unknown));
                    item.Label2 = GetLabel2ForGroup(recordingGroup);
                    item.TVTag = recordingGroup;
                    itemlist.Add(item);
                }
                groupIndex++;
            }
            Log.Info("TvRecorded: LoadDirectory - GroupData loaded.");

            try
            {
                foreach (GUIListItem item in itemlist)
                {
                    facadeLayout.Add(item);
                }
            }
            catch (Exception ex2)
            {
                Log.Error("TvRecorded: Error adding recordings to list - {0}", ex2.Message);
            }
            Log.Info("TvRecorded: LoadDirectory - Gui loaded.");

            string strObjects = string.Format("{0}", itemlist.Count - (isSubDirectory ? 1 : 0));
            GUIPropertyManager.SetProperty("#itemcount", strObjects);

            SwitchLayout();
            OnSort();
            UpdateProperties();
            StartLoadingThumbs();

            Log.Info("RecordedBase: LoadDirectory - Leave.");
        }

        private string GetLabelForGroup(RecordingGroup recordingGroup, string recordingsText, string unknownText)
        {
            switch (recordingGroup.RecordingGroupMode)
            {
                case RecordingGroupMode.GroupByProgramTitle:
                    return String.Format("{0} ({1} {2})", recordingGroup.ProgramTitle,
                                                          recordingGroup.RecordingsCount,
                                                          recordingsText);
                case RecordingGroupMode.GroupByChannel:
                    return String.Format("{0} ({1} {2})", recordingGroup.ChannelDisplayName,
                                                          recordingGroup.RecordingsCount,
                                                          recordingsText);
                case RecordingGroupMode.GroupBySchedule:
                    return String.Format("{0} ({1} {2})", recordingGroup.ScheduleName,
                                                          recordingGroup.RecordingsCount,
                                                          recordingsText);
                case RecordingGroupMode.GroupByCategory:
                    return String.Format("{0} ({1} {2})", String.IsNullOrEmpty(recordingGroup.Category) ? unknownText : recordingGroup.Category,
                                                          recordingGroup.RecordingsCount,
                                                          recordingsText);
            }
            return "?";
        }

        private string GetLabel2ForGroup(RecordingGroup recordingGroup)
        {
            return String.Format("{0:g}", recordingGroup.LatestProgramStartTime);
        }

        private object CheckValidGroupId(object currentGroupId)
        {
            foreach (RecordingGroup recordingGroup in _model.RecordingGroups)
            {
                if ((recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupBySchedule && currentGroupId is Guid && recordingGroup.ScheduleId == (Guid)currentGroupId)
                    || (recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupByChannel && currentGroupId is Guid && recordingGroup.ChannelId == (Guid)currentGroupId)
                    || (recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupByProgramTitle && currentGroupId is string && recordingGroup.ProgramTitle == (string)currentGroupId)
                    || (recordingGroup.RecordingGroupMode == RecordingGroupMode.GroupByCategory && currentGroupId is string && EnsureCategoryName(recordingGroup.Category) == (string)currentGroupId))
                {
                    return currentGroupId;
                }
            }
            return null;
        }

        protected override void UpdateButtonStates()
        {
            base.UpdateButtonStates();
            try
            {
                string text = String.Empty;
                if (btnSortBy != null)
                {
                    switch (_currentSortMethod)
                    {
                        case SortMethod.Channel:
                            text = Utility.GetLocalizedText(TextId.SortByChannel);
                            break;
                        case SortMethod.Date:
                            text = Utility.GetLocalizedText(TextId.SortByDate);
                            break;
                        case SortMethod.Name:
                            text = Utility.GetLocalizedText(TextId.SortByTitle);
                            break;
                    }
                    GUIControl.SetControlLabel(GetID, btnSortBy.GetID, text);
                }

                if (null != facadeLayout)
                    facadeLayout.EnableScrollLabel = _currentSortMethod == SortMethod.Name;

                text = string.Empty;
                if (btnViews != null)
                {
                    switch (_currentGroupByMode)
                    {
                        case RecordingGroupMode.GroupByProgramTitle:
                            text = Utility.GetLocalizedText(TextId.GroupByTitle);
                            break;
                        case RecordingGroupMode.GroupBySchedule:
                            text = Utility.GetLocalizedText(TextId.GroupBySchedule);
                            break;
                        case RecordingGroupMode.GroupByChannel:
                            text = Utility.GetLocalizedText(TextId.GroupByChannel);
                            break;
                        case RecordingGroupMode.GroupByCategory:
                            text = Utility.GetLocalizedText(TextId.GroupByCategory);
                            break;
                    }
                    GUIControl.SetControlLabel(GetID,btnViews.GetID, text);
                }

                GUIControl.ShowControl(GetID, (int)ControlId.LABEL_PROGRAMTITLE);
                GUIControl.ShowControl(GetID, (int)ControlId.LABEL_PROGRAMDESCRIPTION);
                GUIControl.ShowControl(GetID, (int)ControlId.LABEL_PROGRAMGENRE);
                GUIControl.ShowControl(GetID, (int)ControlId.LABEL_PROGRAMTIME);
            }
            catch (Exception ex)
            {
                Log.Warn("TVRecorded: Error updating button states - {0}", ex.ToString());
            }
        }

        private void SetLabels()
        {
            for (int i = 0; i < facadeLayout.Count; ++i)
              {
                  try
                  {
                      GUIListItem item1 = facadeLayout[i];
                      if (item1.Label != _parentDirectoryLabel)
                      {
                          RecordingSummary rec = item1.TVTag as RecordingSummary;
                          if (rec != null)
                          {
                              item1.Label = rec.CreateProgramTitle();
                              TimeSpan ts = TimeSpan.Zero;
                              if (rec.RecordingStopTimeUtc.HasValue)
                              {
                                  ts = rec.RecordingStopTimeUtc.Value - rec.RecordingStartTimeUtc;
                              }
                              string strTime = string.Format("{0} {1} ({2})",
                                Utility.GetShortDayDateString(rec.RecordingStartTime),
                                rec.RecordingStartTime.ToShortTimeString(),
                                ts.TotalSeconds > 0 ? Utils.SecondsToHMString((int)ts.TotalSeconds) : Utility.GetLocalizedText(TextId.RecordingInProgress));
                              item1.Label2 = strTime;
                              if (currentLayout == Layout.List)
                              {
                                  if (_currentSortMethod == SortMethod.Channel)
                                  {
                                      item1.Label2 = rec.ChannelDisplayName;
                                  }
                              }
                              else
                              {
                                  if (_currentSortMethod == SortMethod.Channel)
                                  {
                                      item1.Label3 = rec.ChannelDisplayName;
                                  }
                                  else
                                  {
                                      item1.Label3 = rec.Category;
                                  }
                              }
                              item1.IsPlayed = !item1.IsFolder && rec.LastWatchedTime.HasValue;
                          }
                      }
                  }
                  catch (Exception ex)
                  {
                      Log.Warn("TVRecorded: error in SetLabels - {0}", ex.Message);
                  }
              }
        }

        private bool OnSelectItem(int iItem)
        {
            GUIListItem pItem = GetItem(iItem);
            if (pItem == null) return false;
            if (pItem.IsFolder)
            {
                if (pItem.Label == _parentDirectoryLabel)
                {
                    _currentGroupId = null;
                    LoadDirectory(false);
                    SelectItemByIndex(ref _selectedParentItemIndex);
                }
                else
                {
                    RecordingGroup recordingGroup = pItem.TVTag as RecordingGroup;
                    switch (recordingGroup.RecordingGroupMode)
                    {
                        case RecordingGroupMode.GroupByProgramTitle:
                            _currentGroupId = recordingGroup.ProgramTitle;
                            break;
                        case RecordingGroupMode.GroupByChannel:
                            _currentGroupId = recordingGroup.ChannelId;
                            break;
                        case RecordingGroupMode.GroupBySchedule:
                            _currentGroupId = recordingGroup.ScheduleId;
                            break;
                        case RecordingGroupMode.GroupByCategory:
                            _currentGroupId = EnsureCategoryName(recordingGroup.Category);
                            break;
                    }
                    _selectedParentItemIndex = GetSelectedItemIndex();
                    LoadDirectory(false);
                }
                return false;
            }

            RecordingSummary rec = pItem.TVTag as RecordingSummary;
            if (rec != null)
            {
                pItem.IsPlayed = true;
                GUIControl.RefreshControl(GetID, facadeLayout.GetID);
                return PlayRecording(ControlAgent.GetRecordingById(rec.RecordingId), null);
            }
            return false;
        }

        private void OnDeleteRecording(int itemIndex)
        {
            _selectedItemIndex = GetSelectedItemIndex();
            GUIListItem item = GetItem(itemIndex);
            if (item != null
                && !item.IsFolder)
            {
                RecordingSummary rec = (RecordingSummary)item.TVTag;
                if (rec == null) return;

                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                if (dlgYesNo != null)
                {
                    dlgYesNo.Reset();
                    if (rec.LastWatchedTime.HasValue) dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.DeleteThisRecording));
                    else dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.DeleteUnwatchedRecording));
                    dlgYesNo.SetLine(1, rec.ChannelDisplayName);
                    dlgYesNo.SetLine(2, rec.Title);
                    dlgYesNo.SetLine(3, string.Empty);
                    dlgYesNo.SetDefaultToYes(false);
                    dlgYesNo.DoModal(GetID);

                    if (dlgYesNo.IsConfirmed)
                    {
                        ControlAgent.DeleteRecording(rec.RecordingFileName, true);
                        LoadDirectory(true);
                        SelectItemByIndex(ref _selectedItemIndex);
                        SetRecordingDiskInfo(false);
                    }
                }
            }
        }

        private void OnkeepUntil(RecordingSummary rec)
        {
            GUIDialogMenu dialog = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dialog == null) return;
            dialog.Reset();

            dialog.SetHeading(1042); // Keep until
            dialog.Add(Utility.GetLocalizedText(TextId.UntilSpaceNeeded));
            dialog.Add(Utility.GetLocalizedText(TextId.NumberOfDays));
            dialog.Add(Utility.GetLocalizedText(TextId.NumberOfEpisodes));
            dialog.Add(Utility.GetLocalizedText(TextId.NumberOfWatchedEpisodes));
            dialog.Add(Utility.GetLocalizedText(TextId.Forever)); 

            switch (rec.KeepUntilMode)
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
                    ControlAgent.SetRecordingKeepUntil(rec.RecordingFileName, KeepUntilMode.UntilSpaceIsNeeded, null);
                    break;

                case 1:
                    {
                        int? value = GetKeepValue(1045, KeepUntilMode.NumberOfDays, 3014, 3015,
                            rec.KeepUntilMode == KeepUntilMode.NumberOfDays ? rec.KeepUntilValue : 7);
                        if (value.HasValue)
                        {
                            ControlAgent.SetRecordingKeepUntil(rec.RecordingFileName, KeepUntilMode.NumberOfDays, value);
                        }
                    }
                    break;

                case 2:
                    {
                        int? value = GetKeepValue(887, KeepUntilMode.NumberOfEpisodes, 682, 914,
                            rec.KeepUntilMode == KeepUntilMode.NumberOfEpisodes ? rec.KeepUntilValue : 3);
                        if (value.HasValue)
                        {
                            ControlAgent.SetRecordingKeepUntil(rec.RecordingFileName, KeepUntilMode.NumberOfEpisodes, value);
                        }
                    }
                    break;

                case 3:
                    {
                        int? value = GetKeepValue(887, KeepUntilMode.NumberOfWatchedEpisodes, 682, 914,
                            rec.KeepUntilMode == KeepUntilMode.NumberOfWatchedEpisodes ? rec.KeepUntilValue : 3);
                        if (value.HasValue)
                        {
                            ControlAgent.SetRecordingKeepUntil(rec.RecordingFileName, KeepUntilMode.NumberOfWatchedEpisodes, value);
                        }
                    }
                    break;

                case 4:
                    ControlAgent.SetRecordingKeepUntil(rec.RecordingFileName, KeepUntilMode.Forever, null);
                    break;
            }

            int oldIndex = GetSelectedItemIndex();
            LoadDirectory(true);
            SelectItemByIndex(ref oldIndex);
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

        private void UpdateProperties()
        {
            try
            {
                RecordingSummary rec;
                GUIListItem pItem = GetItem(GetSelectedItemIndex());
                if (pItem == null)
                {
                    GUIPropertyManager.SetProperty("#selectedthumb", String.Empty);
                    SetProperties(null);
                    return;
                }
                rec = pItem.TVTag as RecordingSummary;
                if (rec == null)
                {
                    SetProperties(null);
                    return;
                }
                SetProperties(rec);
                if (!pItem.IsFolder)
                {
                    GUIPropertyManager.SetProperty("#selectedthumb", pItem.ThumbnailImage);
                }
            }
            catch (Exception ex)
            {
                Log.Error("RecordedBase: Error updating properties - {0}", ex.ToString());
            }
        }

        private void SetProperties(RecordingSummary rec)
        {
            string guiPropertyPrefix = _channelType == ChannelType.Television ? "#TV" : "#Radio";
            if (rec == null)
            {
                Utility.ClearProperty(guiPropertyPrefix + ".RecordedTV.Channel");
                Utility.ClearProperty(guiPropertyPrefix + ".RecordedTV.Title");
                Utility.ClearProperty(guiPropertyPrefix + ".RecordedTV.Genre");
                Utility.ClearProperty(guiPropertyPrefix + ".RecordedTV.Time");
                Utility.ClearProperty(guiPropertyPrefix + ".RecordedTV.Description");
                Utility.ClearProperty(guiPropertyPrefix + ".RecordedTV.thumb");
                Utility.ClearProperty(guiPropertyPrefix + ".RecordedTV.PercentageWatched");
            }
            else
            {
                Recording recording = this.ControlAgent.GetRecordingById(rec.RecordingId);

                string strTime = string.Format("{0} {1} - {2}",
                  Utility.GetShortDayDateString(rec.ProgramStartTime),
                  rec.ProgramStartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                  rec.ProgramStopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Channel", rec.ChannelDisplayName);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Title", rec.Title);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Genre", rec.Category);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Time", strTime);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Description", recording.CreateCombinedDescription(true));

                if (rec.LastWatchedPosition.HasValue && rec.RecordingStopTimeUtc.HasValue)
                {
                    double percentage = (rec.LastWatchedPosition.Value / (rec.RecordingStopTimeUtc.Value - rec.RecordingStartTimeUtc).TotalSeconds) * 100;
                    if (percentage > 100) percentage = 100;
                    if (percentage < 0) percentage = 0;
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.PercentageWatched", Math.Round(percentage).ToString());
                }
                else
                {
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.PercentageWatched", "");
                }

                string logo = Utility.GetLogoImage(rec.ChannelId, rec.ChannelDisplayName, SchedulerAgent);
                if (Utils.FileExistsInCache(logo))
                {
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.thumb", logo);
                }
                else
                {
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.thumb", "defaultVideoBig.png");
                }
            }
        }

        private void SetRecordingDiskInfo(bool CheckFreeSize)
        {
            //Log.Debug("RecordedBase: SetRecordingDiskInfo");
            RecordingDisksInfo _recordingDisksInfo = ControlAgent.GetRecordingDisksInfo();

            //Byte to GB conversion
            double TotalSizeGBytes = _recordingDisksInfo.TotalSizeBytes;
            TotalSizeGBytes = TotalSizeGBytes / 1024 / 1024 / 1024;
            double UsedSpaceGBytes = (_recordingDisksInfo.TotalSizeBytes - _recordingDisksInfo.FreeSpaceBytes);
            UsedSpaceGBytes = UsedSpaceGBytes / 1024 / 1024 / 1024;
            double FreeSpaceGBytes = _recordingDisksInfo.FreeSpaceBytes;
            FreeSpaceGBytes = FreeSpaceGBytes / 1024 / 1024 / 1024;

            string guiPropertyPrefix = _channelType == ChannelType.Television ? "#TV" : "#Radio";
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Diskinfo.PercentageUsed", Math.Round(_recordingDisksInfo.PercentageUsed).ToString());
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Diskinfo.PercentageFree", Math.Round(100 - _recordingDisksInfo.PercentageUsed).ToString());
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Diskinfo.FreeSpaceGBytes", Math.Round(FreeSpaceGBytes).ToString());
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Diskinfo.UsedSpaceGBytes", Math.Round(UsedSpaceGBytes).ToString());
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".RecordedTV.Diskinfo.TotalSizeGBytes", Math.Round(TotalSizeGBytes).ToString());
            if (_channelType == ChannelType.Television)
            {
                GUIPropertyManager.SetProperty("#TV.RecordedTV.Diskinfo.FreeHoursSD", _recordingDisksInfo.FreeHoursSD.ToString());
                GUIPropertyManager.SetProperty("#TV.RecordedTV.Diskinfo.FreeHoursHD", _recordingDisksInfo.FreeHoursHD.ToString());
            }

            //show information dialog when there is almost no free space
            if (CheckFreeSize && _recordingDisksInfo.TotalSizeBytes != 0)
            {
                if (FreeSpaceGBytes < 10)
                {
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
                    if (dlgOk != null)
                    {
                        dlgOk.Reset();
                        string text = Utility.GetLocalizedText(TextId.RecordingDiskAlmostFull);
                        text = text.Replace("\\r", "\r");
                        string[] lines = text.Split('\r');
                        string line1 = "recording disk almost full";
                        if (lines.Length > 1)
                        {
                            line1 = lines[0] + Math.Round(FreeSpaceGBytes, 2) + lines[1];
                        }
                        dlgOk.SetHeading(Utility.GetLocalizedText(TextId.Information));
                        dlgOk.SetLine(1, line1);
                        dlgOk.DoModal(GetID);
                    }
                }
            }
        }

        private string EnsureCategoryName(string category)
        {
            if (String.IsNullOrEmpty(category))
            {
                return Utility.GetLocalizedText(TextId.Unknown);
            }
            return category;
        }

        private void OnCleanUp()
        {
            _cleanUpMethod = cleanupMethod.None;

            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
            if (dlg == null)
            {
                return;
            }
            dlg.Reset();
            dlg.SetHeading(GUILocalizeStrings.Get(200043)); //Cleanup recordings?

            if (facadeLayout != null && facadeLayout.Count > 0
                && facadeLayout[0].Label == _parentDirectoryLabel)
            {
                dlg.Add(Utility.GetLocalizedText(TextId.WatchedInThisFolder)); // Only watched recordings from this folder.
            }
            dlg.Add(Utility.GetLocalizedText(TextId.AllWatched));           // Only watched recordings?
            dlg.Add(Utility.GetLocalizedText(TextId.AllInvalid));           // Only invalid recordings?
            dlg.Add(Utility.GetLocalizedText(TextId.AllInvalidAndWatched)); // Both?
            dlg.Add(Utility.GetLocalizedText(TextId.Cancel));               // Cancel?
            dlg.DoModal(GetID);

            if (dlg.SelectedLabel == -1)
            {
                return;
            }

            string selectedText = dlg.SelectedLabelText;
            if (selectedText == Utility.GetLocalizedText(TextId.AllWatched))
            {
                _cleanUpMethod = cleanupMethod.AllWatched;
            }
            else if (selectedText == Utility.GetLocalizedText(TextId.AllInvalid))
            {
                _cleanUpMethod = cleanupMethod.AllInvalid;
            }
            else if (selectedText == Utility.GetLocalizedText(TextId.AllInvalidAndWatched))
            {
                _cleanUpMethod = cleanupMethod.AllInvalidAndWatched;
            }
            else if (selectedText == Utility.GetLocalizedText(TextId.WatchedInThisFolder))
            {
                _cleanUpMethod = cleanupMethod.WatchedInThisFolder;
            }
            if (_cleanUpMethod == cleanupMethod.None)
            {
                return;
            }

            if (_cleanUpMethod != cleanupMethod.AllInvalid)
            {
                GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_YES_NO);
                if (null == dlgYesNo)
                {
                    return;
                }
                dlgYesNo.Reset();
                dlgYesNo.SetHeading(GUILocalizeStrings.Get(200043)); //Cleanup recordings?
                dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.AreYouSure)); // Are you sure?
                dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
                if (!dlgYesNo.IsConfirmed)
                {
                    return;
                }
            }

            try
            {
                if (_cleanUpMethod == cleanupMethod.WatchedInThisFolder)
                {
                    if (facadeLayout != null && facadeLayout.Count > 0
                        && facadeLayout[0].Label == _parentDirectoryLabel)
                    {
                        RecordingSummary recording = null;
                        for (int i = 0; i < facadeLayout.Count; i++)
                        {
                            recording = facadeLayout[i].TVTag as RecordingSummary;
                            if (recording != null)
                            {
                                if (recording.LastWatchedTime.HasValue)
                                {
                                    ControlAgent.DeleteRecording(recording.RecordingFileName, true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    RecordingGroup[] groups = ControlAgent.GetAllRecordingGroups(this._channelType, RecordingGroupMode.GroupByProgramTitle);
                    foreach (RecordingGroup group in groups)
                    {
                        RecordingSummary[] recordings = ControlAgent.GetRecordingsForProgramTitle(this._channelType, group.ProgramTitle, true);
                        foreach (RecordingSummary recording in recordings)
                        {
                            if (_cleanUpMethod == cleanupMethod.AllInvalidAndWatched
                                || _cleanUpMethod == cleanupMethod.AllInvalid)
                            {
                                if (!recording.IsFileOnDisk ||
                                    (recording.RecordingStopTimeUtc.HasValue && (recording.RecordingStartTimeUtc.AddSeconds(15) > recording.RecordingStopTimeUtc)))
                                {
                                    Log.Debug("RecordedBase: remove invalid recording: {0}", recording.RecordingFileName);
                                    ControlAgent.DeleteRecording(recording.RecordingFileName, true);
                                    continue;
                                }
                            }

                            if (_cleanUpMethod == cleanupMethod.AllInvalidAndWatched
                                || _cleanUpMethod == cleanupMethod.AllWatched)
                            {
                                if (recording.LastWatchedTime.HasValue)
                                {
                                    ControlAgent.DeleteRecording(recording.RecordingFileName, true);
                                    continue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("RecordedBase: error deleting recordings: {0}", ex.Message);
            }
            finally
            {
                int index = GetSelectedItemIndex();
                LoadDirectory(true);
                SelectItemByIndex(ref index);
                SetRecordingDiskInfo(false);
            }
        }

        #endregion

        #region View management

        private GUIListItem GetSelectedItem()
        {
            return facadeLayout.SelectedListItem;
        }

        private GUIListItem GetItem(int iItem)
        {
            if (iItem < 0 || iItem >= facadeLayout.Count)
            {
                return null;
            }
            return facadeLayout[iItem];
        }

        private int GetSelectedItemIndex()
        {
            if (facadeLayout.Count > 0)
            {
                return facadeLayout.SelectedListItemIndex;
            }
            else
            {
                return -1;
            }
        }

        private void SelectItemByIndex(ref int index)
        {
            while (index >= GetItemCount() && index > 0)
            {
                index--;
            }
            GUIControl.SelectItemControl(GetID, facadeLayout.GetID, index);
        }

        private int GetItemCount()
        { 
            return facadeLayout.Count;
        }

        #endregion

        #region Sort Members

        private void OnSort()
        {
            SetLabels();
            facadeLayout.Sort(this);
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
            if (item1.IsFolder && item1.Label == _parentDirectoryLabel)
            {
                return -1;
            }
            else if (item2.IsFolder && item2.Label == _parentDirectoryLabel)
            {
                return 1;
            }

            string channel1, channel2;
            DateTime time1, time2;
            GetSortingDataForItem(item1, out time1, out channel1);
            GetSortingDataForItem(item2, out time2, out channel2);

            switch (_currentSortMethod)
            {
                case SortMethod.Name:
                    result = resultUpper * String.Compare(item1.Label, item2.Label, StringComparison.OrdinalIgnoreCase);
                    if (result == 0)
                    {
                        goto case SortMethod.Date;
                    }
                    break;

                case SortMethod.Channel:
                    if (channel1 == null && channel2 == null)
                    {
                        result = 0;
                    }
                    else if (channel1 == null)
                    {
                        result = resultLower;
                    }
                    else if (channel2 == null)
                    {
                        result = resultUpper;
                    }
                    else
                    {
                        result = resultUpper * String.Compare(channel1, channel2, StringComparison.OrdinalIgnoreCase);
                    }
                    if (result == 0)
                    {
                        goto case SortMethod.Date;
                    }
                    break;

                case SortMethod.Date:
                    if (time1 != time2)
                    {
                        result = (time1 < time2) ? resultUpper : resultLower;
                    }
                    break;
            }

            return result;
        }

        private static void GetSortingDataForItem(GUIListItem item1, out DateTime time1, out string channel1)
        {
            if (item1.IsFolder)
            {
                RecordingGroup group1 = item1.TVTag as RecordingGroup;
                time1 = group1.LatestProgramStartTime;
                channel1 = null;
            }
            else
            {
                RecordingSummary rec1 = item1.TVTag as RecordingSummary;
                time1 = rec1.ProgramStartTime;
                if (rec1.RecordingStartTime > time1)
                {
                    time1 = rec1.RecordingStartTime;
                }
                channel1 = rec1.ChannelDisplayName;
            }
        }

        private void SortChanged(object sender, SortEventArgs e)
        {
            m_bSortAscending = e.Order != System.Windows.Forms.SortOrder.Descending;
            OnSort();
        }

        #endregion

        #region Playback Events

        private void OnPlayRecordingBackChanged(global::MediaPortal.Player.g_Player.MediaType type, int stoptime, string filename)
        {
            OnPlayRecordingBackStoppedOrChanged(type, stoptime, filename,true);
        }

        private void OnPlayRecordingBackStopped(global::MediaPortal.Player.g_Player.MediaType type, int stoptime, string filename)
        {
            OnPlayRecordingBackStoppedOrChanged(type, stoptime, filename,false);
        }

        private void OnPlayRecordingBackEnded(global::MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (_playingRecording != null)
            {
                if (type == g_Player.MediaType.Recording || type == g_Player.MediaType.Radio)
                {
                    g_Player.Stop();
                    StopCurrentPlayback(filename, 0,false);
                }
                else
                {
                    StopCurrentPlayback();
                }
            }
        }

        private void OnPlayRecordingBackStarted(global::MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            if (_playingRecording != null)
            {
                Log.Info("RecordedBase:OnPlayRecordingBackStarted {0} {1}", type, filename);
                if (type == g_Player.MediaType.Recording || type == g_Player.MediaType.Radio)
                {
                    // set audio track based on user prefs. 
                    g_Player.CurrentAudioStream = HomeBase.GetPreferedAudioStreamIndex();
                    if (type == g_Player.MediaType.Radio)
                    {
                        RadioHome.SetMusicProperties(_playingRecording.ChannelDisplayName, _playingRecording.ChannelId);
                    } 
                }
                else
                {
                    StopCurrentPlayback();
                }
            }
        }

        private void OnPlayRecordingBackStoppedOrChanged(g_Player.MediaType type, int stoptime, string filename, bool onPlayBackChanged)
        {
            if (_playingRecording != null)
            {
                Log.Info("RecordedBase:OnStoppedOrChanged {0} {1} {2}", type,stoptime,filename);
                if (type == g_Player.MediaType.Recording || type == g_Player.MediaType.Radio)
                {
                    if (stoptime >= g_Player.Duration)
                    {
                        // Temporary workaround before end of stream gets properly implemented.
                        stoptime = 0;
                    }
                    StopCurrentPlayback(filename, stoptime,onPlayBackChanged);
                }
                else
                {
                    StopCurrentPlayback();
                }
            }
        }

        private static void StopCurrentPlayback()
        {
            StopCurrentPlayback(_playingRecordingFileName, -1,false);
        }

        private static void StopCurrentPlayback(string filename,int stoptime,bool onPlayBackChanged)
        {
            using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
            {
                if (filename != null && filename != string.Empty)
                {
                    if (filename.StartsWith("rtsp:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        //we need the real filename for saving the position, not the RTSP one.
                        if (_playingRecording != null ) filename = _playingRecording.RecordingFileName;
                        tvControlAgent.StopRecordingStream(filename);
                    }
                    if (stoptime >= 0)
                    {
                        tvControlAgent.SetRecordingLastWatchedPosition(filename, stoptime);
                    }
                }
                if (!onPlayBackChanged)
                {
                    _playingRecordingFileName = null;
                    _playingRecording = null;
                }
            }
        }

        #endregion

        #region Thumbnails Thread

        private Thread _LoadThumbs;
        private void StartLoadingThumbs()
        {
            if (_LoadThumbs != null
                && _LoadThumbs.IsAlive)
            {
                return;
            }
            _LoadThumbs = new Thread(LoadThumbNails);
            _LoadThumbs.Name = "AsyncLoadRecordingThums";
            _LoadThumbs.Priority = ThreadPriority.Lowest;
            _LoadThumbs.Start();
        }

        private void StopLoadingThumbs()
        {
            if (_LoadThumbs != null
                && _LoadThumbs.IsAlive)
            {
                _LoadThumbs.Abort();
            }
        }


        private void LoadThumbNails()
        {
            Log.Debug("RecordedBase: start loading recording thumbs");
            try
            {
                if (facadeLayout != null && facadeLayout.Count > 0)
                {
                    for (int i = 0; i < facadeLayout.Count; i++)
                    {
                        RecordingSummary rec = null;
                        rec = facadeLayout[i].TVTag as RecordingSummary;
                        if (rec != null)
                        {
                            if (PluginMain.IsRecordingStillActive(rec.RecordingId))
                                facadeLayout[i].PinImage = Thumbs.TvRecordingIcon;
                            else
                                facadeLayout[i].PinImage = "";

                            string _thumb = Utility.GetRecordingThumb(rec, true, _thumbSize, ControlAgent);
                            if (!Utils.FileExistsInCache(_thumb))
                            {
                                _thumb = facadeLayout[i].IconImage;
                            }
                            facadeLayout[i].ThumbnailImage = _thumb;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("RecordedBase: error loading recording thumbs, ex = {0}",ex.Message);
            }
            Log.Debug("RecordedBase: stop loading recording thumbs");
        }

        #endregion
    }
}
