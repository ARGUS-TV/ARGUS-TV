#region Copyright (C) 2007-2014 ARGUS TV

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
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Profile;
using Action = MediaPortal.GUI.Library.Action;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.MediaPortal
{
    public class ServerSettingsBase : GUIWindow
    {
        [SkinControlAttribute(2)] protected GUIButtonControl _showLogsButton = null;
        [SkinControlAttribute(3)] protected GUIButtonControl _restoreDefaultsButton = null;
        [SkinControlAttribute(4)] protected GUIButtonControl _deleteAllGuideDataButton = null;
        [SkinControlAttribute(5)] protected GUIButtonControl _guideSourceButton = null;
        [SkinControlAttribute(6)] protected GUIButtonControl _keepUntilModeButton = null;
        [SkinControlAttribute(7)] protected GUIButtonControl _keepUntilValueButton = null;
        [SkinControlAttribute(8)] protected GUISpinButton _preRecordButton = null;
        [SkinControlAttribute(9)] protected GUISpinButton _postRecordButton = null;
        [SkinControlAttribute(11)] protected GUICheckButton _autoCreateThumbsButton = null;
        [SkinControlAttribute(12)] protected GUICheckButton _metaDataForRecsButton = null;
        [SkinControlAttribute(17)] protected GUISpinButton _freeDiskSpaceSpinButton = null;
        [SkinControlAttribute(18)] protected GUISpinButton _minFreeDiskSpaceSpinButton = null;
        [SkinControlAttribute(19)] protected GUILabelControl _preferedGuideSourceLabel = null;
        [SkinControlAttribute(20)] protected GUILabelControl _keepValueLabel = null;
        [SkinControlAttribute(21)] protected GUIFadeLabel _infoLabel = null;

        private GuideSource _currentGuideSource = GuideSource.Other;
        private KeepUntilMode _currentKeepUntilMode = KeepUntilMode.Forever;
        private int _currentKeepUntilValue = 0;
        private const int _daysToGetLogsFrom = 10;
        private const int _maxLogEntries = 250;

        public ServerSettingsBase()
        {
            GetID = WindowId.ServerSettings;
        }

        #region Overrides

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\ARGUS_ServerSettings.xml");
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _keepValueLabel.Label = Utility.GetLocalizedText(TextId.Value);
            _preferedGuideSourceLabel.Label = Utility.GetLocalizedText(TextId.PreferedEPGSource);
            _deleteAllGuideDataButton.Label = Utility.GetLocalizedText(TextId.DeleteAllGuideData);
            _restoreDefaultsButton.Label = Utility.GetLocalizedText(TextId.RestoreDefaults);
            _freeDiskSpaceSpinButton.Label = Utility.GetLocalizedText(TextId.FreeDiskSpace);
            _minFreeDiskSpaceSpinButton.Label = Utility.GetLocalizedText(TextId.MinFreeDiskSpace);
            _preRecordButton.Label = Utility.GetLocalizedText(TextId.DefaultPreRec);
            _postRecordButton.Label = Utility.GetLocalizedText(TextId.DefaultPostRec);
            _autoCreateThumbsButton.Label = Utility.GetLocalizedText(TextId.RecThumbs);
            _metaDataForRecsButton.Label = Utility.GetLocalizedText(TextId.MetaDataForRec);
            _showLogsButton.Label = Utility.GetLocalizedText(TextId.ShowLogs);

            if (_infoLabel != null)
            {
                _infoLabel.Label = Utility.GetLocalizedText(TextId.ServerSettingsInfo);
            }
        }

        protected override void OnPageLoad()
        {
            PluginMain.EnsureConnection();
            base.OnPageLoad();
            
            if (PluginMain.IsConnected())
            {
                LoadSettings();
                UpdateButtons();
                GUIPropertyManager.SetProperty("#currentmodule", Utility.GetLocalizedText(TextId.ArgusServerSettings));
            }
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            SaveSettings();
            base.OnPageDestroy(new_windowId);
        }

        protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
        {
            if (control == _guideSourceButton)
            {
                OnchangeEpgSource();
                UpdateButtons();
            }
            else if (control == _keepUntilModeButton)
            {
                OnChangeKeepUntilMode();
                UpdateButtons();
            }
            else if (control == _keepUntilValueButton)
            {
                OnChangeKeeUntilValue();
                UpdateButtons();
            }
            else if (control == _deleteAllGuideDataButton)
            {
                OnDeleteAllGuideData();
            }
            else if (control == _restoreDefaultsButton)
            {
                OnRestoreDefaults();
                LoadSettings();
                UpdateButtons();
            }
            else if (control == _showLogsButton)
            {
                OnShowLogs();
            }
            base.OnClicked(controlId, control, actionType);
        }

        #endregion

        #region Private methods

        private void LoadSettings()
        {
            Log.Debug("ServerSettingsBase: LoadSettings()");
            bool? _autoCreateThumbs = Proxies.ConfigurationService.GetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.CreateVideoThumbnails);
            bool? _metaDataForRecs = Proxies.ConfigurationService.GetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.AlwaysCreateMetadataFiles);
            bool? _swapTunerPriority = Proxies.ConfigurationService.GetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.SwapRecorderTunerPriorityForRecordings);
            bool? _autoCombineConsecutive = Proxies.ConfigurationService.GetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.AutoCombineConsecutiveRecordings);
            bool? _combineConsecutive = Proxies.ConfigurationService.GetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.CombineConsecutiveRecordings);
            bool? _combineOnlyOnSameChannel = Proxies.ConfigurationService.GetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.CombineRecordingsOnlyOnSameChannel);

            int? _preRecord = Proxies.ConfigurationService.GetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PreRecordsSeconds);
            int? _postRecord = Proxies.ConfigurationService.GetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PostRecordsSeconds);
            int? _keepUntilValue = Proxies.ConfigurationService.GetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.DefaultKeepUntilValue);
            int? _freeDiskSpace = Proxies.ConfigurationService.GetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.FreeDiskSpaceInMB);
            int? _minFreeDiskSpace = Proxies.ConfigurationService.GetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.MinimumFreeDiskSpaceInMB);

            string _keepUntilMode = Proxies.ConfigurationService.GetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.DefaultKeepUntilMode);
            string _guideSource = Proxies.ConfigurationService.GetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PreferredGuideSource);

            if (_autoCreateThumbs.HasValue) _autoCreateThumbsButton.Selected = _autoCreateThumbs.Value;
            if (_metaDataForRecs.HasValue) _metaDataForRecsButton.Selected = _metaDataForRecs.Value;
            if (_keepUntilValue.HasValue) _currentKeepUntilValue = _keepUntilValue.Value;
            if (_keepUntilMode != string.Empty) _currentKeepUntilMode = GetKeepUntilModeFromString(_keepUntilMode);
            if (_guideSource != string.Empty) _currentGuideSource = GetGuideSourceFromString(_guideSource);
            
            _autoCreateThumbsButton.IsEnabled = _autoCreateThumbs.HasValue;
            _metaDataForRecsButton.IsEnabled = _metaDataForRecs.HasValue;
            _preRecordButton.IsEnabled = _preRecord.HasValue;
            _postRecordButton.IsEnabled = _postRecord.HasValue;
            _keepUntilValueButton.IsEnabled = _keepUntilValue.HasValue;
            _freeDiskSpaceSpinButton.IsEnabled = _freeDiskSpace.HasValue;
            _minFreeDiskSpaceSpinButton.IsEnabled = _minFreeDiskSpace.HasValue;

            if (_preRecord.HasValue)
            {
                int prerec = _preRecord.Value/60;
                bool valueFoundInList = false;

                for (int i = 0; i <= 60; i++)
                {
                    _preRecordButton.AddSpinLabel(i.ToString(), 0);
                    if (i == prerec) valueFoundInList = true;
                }

                if (valueFoundInList)
                {
                    _preRecordButton.SpinValue = prerec;
                }
                else
                {
                    _preRecordButton.AddSpinLabel(prerec.ToString(), 0);
                    _preRecordButton.SpinValue = _preRecordButton.SpinMaxValue() - 1;
                }
            }
            if (_postRecord.HasValue)
            {
                int postrec = _postRecord.Value/60;
                bool valueFoundInList = false;

                for (int i = 0; i <= 60; i++)
                {
                    _postRecordButton.AddSpinLabel(i.ToString(), 0);
                    if (i == postrec) valueFoundInList = true;
                }

                if (valueFoundInList)
                {
                    _postRecordButton.SpinValue = postrec;
                }
                else
                {
                    _postRecordButton.AddSpinLabel(postrec.ToString(), 0);
                    _postRecordButton.SpinValue = _postRecordButton.SpinMaxValue() - 1;
                }
            }
            if (_freeDiskSpace.HasValue)
            {
                int freespace = _freeDiskSpace.Value/1000;
                bool valueFoundInList = false;

                for (int i = 1; i < 1000; i++)
                {
                    _freeDiskSpaceSpinButton.AddSpinLabel(i.ToString(), 0);
                    if (i == freespace) valueFoundInList = true;
                }

                if (valueFoundInList)
                {
                    _freeDiskSpaceSpinButton.SpinValue = freespace - 1;
                }
                else
                {
                    _freeDiskSpaceSpinButton.AddSpinLabel(freespace.ToString(), 0);
                    _freeDiskSpaceSpinButton.SpinValue = _freeDiskSpaceSpinButton.SpinMaxValue() - 1;
                }
            }
            if (_minFreeDiskSpace.HasValue)
            {
                int freespace = _minFreeDiskSpace.Value/1000;
                bool valueFoundInList = false;

                for (int i = 1; i <= 1000; i++)
                {
                    _minFreeDiskSpaceSpinButton.AddSpinLabel(i.ToString(), 0);
                    if (i == freespace) valueFoundInList = true;
                }

                if (valueFoundInList)
                {
                    _minFreeDiskSpaceSpinButton.SpinValue = freespace - 1;
                }
                else
                {
                    _minFreeDiskSpaceSpinButton.AddSpinLabel(freespace.ToString(), 0);
                    _minFreeDiskSpaceSpinButton.SpinValue = _minFreeDiskSpaceSpinButton.SpinMaxValue() - 1;
                }
            }
        }

        private void SaveSettings()
        {
            Log.Debug("ServerSettingsBase: SaveSettings()");
            if (_autoCreateThumbsButton.IsEnabled) Proxies.ConfigurationService.SetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.CreateVideoThumbnails, _autoCreateThumbsButton.Selected);
            if (_metaDataForRecsButton.IsEnabled) Proxies.ConfigurationService.SetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.AlwaysCreateMetadataFiles, _metaDataForRecsButton.Selected);
            if (_guideSourceButton.IsEnabled) Proxies.ConfigurationService.SetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PreferredGuideSource, _currentGuideSource.ToString());
            if (_preRecordButton.IsEnabled) Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PreRecordsSeconds, Int32.Parse(_preRecordButton.SpinLabel) * 60);
            if (_postRecordButton.IsEnabled) Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PostRecordsSeconds, Int32.Parse(_postRecordButton.SpinLabel) * 60);
            if (_keepUntilModeButton.IsEnabled) Proxies.ConfigurationService.SetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.DefaultKeepUntilMode, _currentKeepUntilMode.ToString());
            if (_keepUntilValueButton.IsEnabled) Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.DefaultKeepUntilValue, _currentKeepUntilValue);
            if (_freeDiskSpaceSpinButton.IsEnabled) Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.FreeDiskSpaceInMB, Int32.Parse(_freeDiskSpaceSpinButton.SpinLabel) * 1000);
            if (_minFreeDiskSpaceSpinButton.IsEnabled) Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.MinimumFreeDiskSpaceInMB, Int32.Parse(_minFreeDiskSpaceSpinButton.SpinLabel) * 1000);
        }

        private void OnRestoreDefaults()
        {
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            if (dlgYesNo != null)
            {
                dlgYesNo.Reset();
                dlgYesNo.SetHeading(927);
                dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.RestoreDefaults));
                dlgYesNo.SetLine(2, Utility.GetLocalizedText(TextId.AreYouSure));
                dlgYesNo.SetLine(3, string.Empty);
                dlgYesNo.SetDefaultToYes(false);
                dlgYesNo.DoModal(GetID);
        
                if (dlgYesNo.IsConfirmed)
                {
                    Proxies.ConfigurationService.SetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.CreateVideoThumbnails, null);
                    Proxies.ConfigurationService.SetBooleanValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.AlwaysCreateMetadataFiles, null);
                    Proxies.ConfigurationService.SetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PreferredGuideSource, null);
                    Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PreRecordsSeconds, null);
                    Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.PostRecordsSeconds, null);
                    Proxies.ConfigurationService.SetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.DefaultKeepUntilMode, null);
                    Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.DefaultKeepUntilValue, null);
                    Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.FreeDiskSpaceInMB, null);
                    Proxies.ConfigurationService.SetIntValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.MinimumFreeDiskSpaceInMB, null);
                }
            }
        }

        private void UpdateButtons()
        {
            if (_currentKeepUntilMode == KeepUntilMode.Forever || _currentKeepUntilMode == KeepUntilMode.UntilSpaceIsNeeded)
            {
                _currentKeepUntilValue = 0;
                _keepUntilValueButton.IsEnabled = false;
            }
            else
            {
                _keepUntilValueButton.IsEnabled = true;
            }
            _keepUntilValueButton.Label = _currentKeepUntilValue.ToString();

            switch (_currentKeepUntilMode)
            {
                case KeepUntilMode.UntilSpaceIsNeeded:
                    _keepUntilModeButton.Label = Utility.GetLocalizedText(TextId.UntilSpaceNeeded);
                    break;
                case KeepUntilMode.NumberOfDays:
                    _keepUntilModeButton.Label = Utility.GetLocalizedText(TextId.NumberOfDays);
                    break;
                case KeepUntilMode.NumberOfEpisodes:
                    _keepUntilModeButton.Label = Utility.GetLocalizedText(TextId.NumberOfEpisodes);
                    break;
                case KeepUntilMode.NumberOfWatchedEpisodes:
                    _keepUntilModeButton.Label = Utility.GetLocalizedText(TextId.NumberOfWatchedEpisodes);
                    break;
                case KeepUntilMode.Forever:
                    _keepUntilModeButton.Label = Utility.GetLocalizedText(TextId.Forever);
                    break;
            }

            switch (_currentGuideSource)
            {
                case GuideSource.DvbEpg:
                    _guideSourceButton.Label = GuideSource.DvbEpg.ToString();
                    break;
                case GuideSource.XmlTv:
                    _guideSourceButton.Label = GuideSource.XmlTv.ToString();
                    break;
                case GuideSource.Other:
                    _guideSourceButton.Label = "Other";
                    break;
            }
        }

        private void OnchangeEpgSource()
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(Utility.GetLocalizedText(TextId.PreferedEPGSource));

                dlg.Add(GuideSource.XmlTv.ToString());
                dlg.Add(GuideSource.DvbEpg.ToString());

                int selectedId = 0;
                if (_currentGuideSource == GuideSource.XmlTv)
                {
                    selectedId = 0;
                }
                else if (_currentGuideSource == GuideSource.DvbEpg)
                {
                    selectedId = 1;
                }
                dlg.SelectedLabel = selectedId;

                dlg.DoModal(GetID);
                if (dlg.SelectedId > 0)
                {
                    switch (dlg.SelectedLabel)
                    {
                        case 0:
                            _currentGuideSource = GuideSource.XmlTv;
                            break;
                        case 1 :
                            _currentGuideSource = GuideSource.DvbEpg;
                            break;
                    }
                }
            } 
        }

        private void OnChangeKeepUntilMode()
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(Utility.GetLocalizedText(TextId.DefaultKeepMode)); 
                dlg.Add(Utility.GetLocalizedText(TextId.UntilSpaceNeeded));
                dlg.Add(Utility.GetLocalizedText(TextId.NumberOfDays)); 
                dlg.Add(Utility.GetLocalizedText(TextId.NumberOfEpisodes));  
                dlg.Add(Utility.GetLocalizedText(TextId.NumberOfWatchedEpisodes));
                dlg.Add(Utility.GetLocalizedText(TextId.Forever));

                int selectedId = 0;
                switch (_currentKeepUntilMode)
                {
                    case KeepUntilMode.UntilSpaceIsNeeded:
                        selectedId = 0;
                        break;
                    case KeepUntilMode.NumberOfDays:
                        selectedId = 1;
                        break;
                    case KeepUntilMode.NumberOfEpisodes:
                        selectedId = 2;
                        break;
                    case KeepUntilMode.NumberOfWatchedEpisodes:
                        selectedId = 3;
                        break;
                    case KeepUntilMode.Forever:
                        selectedId = 4;
                        break;
                }
                dlg.SelectedLabel = selectedId;

                dlg.DoModal(GetID);
                if (dlg.SelectedId > 0)
                {
                    switch (dlg.SelectedLabel)
                    {
                        case 0:
                            _currentKeepUntilMode = KeepUntilMode.UntilSpaceIsNeeded;
                            break;
                        case 1:
                            _currentKeepUntilMode = KeepUntilMode.NumberOfDays;
                            break;
                        case 2:
                            _currentKeepUntilMode = KeepUntilMode.NumberOfEpisodes;
                            break;
                        case 3:
                            _currentKeepUntilMode = KeepUntilMode.NumberOfWatchedEpisodes;
                            break;
                        case 4:
                            _currentKeepUntilMode = KeepUntilMode.Forever;
                            break;
                    }

                    if (_currentKeepUntilMode != KeepUntilMode.Forever
                        && _currentKeepUntilMode != KeepUntilMode.UntilSpaceIsNeeded)
                    {
                        DataTable valueTable = KeepUntilControlUtility.CreateValueTable(_currentKeepUntilMode, null);
                        _currentKeepUntilValue = (int)valueTable.Rows[0][KeepUntilControlUtility.ValueColumnName];
                    }
                }
            }
        }

        private void OnChangeKeeUntilValue()
        {
            DataTable valueTable = KeepUntilControlUtility.CreateValueTable(_currentKeepUntilMode, null);
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.ShowQuickNumbers = false;
                dlg.SetHeading(Utility.GetLocalizedText(TextId.DefaultKeepValue));

                foreach (DataRow value in valueTable.Rows)
                {
                    dlg.Add(value[KeepUntilControlUtility.TextColumnName].ToString());
                }
                dlg.SelectedLabel = KeepUntilControlUtility.GetIndexToSelect(valueTable, _currentKeepUntilValue);

                dlg.DoModal(GetID);
                if (dlg.SelectedId > 0)
                {
                    _currentKeepUntilValue = (int)valueTable.Rows[dlg.SelectedLabel][KeepUntilControlUtility.ValueColumnName];
                }
            }
        }

        private void OnDeleteAllGuideData()
        {
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            if (dlgYesNo != null)
            {
                dlgYesNo.Reset();
                dlgYesNo.SetHeading(927);
                dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.DeleteAllGuideData));
                dlgYesNo.SetLine(2, Utility.GetLocalizedText(TextId.AreYouSure));
                dlgYesNo.SetLine(3, string.Empty);
                dlgYesNo.SetDefaultToYes(false);
                dlgYesNo.DoModal(GetID);

                if (dlgYesNo.IsConfirmed)
                {
                    Proxies.GuideService.DeleteAllPrograms();
                }
            }
        }

        private void OnShowLogs()
        {
            string text = string.Empty;
            var modules = Proxies.LogService.GetAllModules();
            string selectedModule = string.Empty;
            LogSeverity selectedSeverity = LogSeverity.Error;

            bool showAllSeverities = true;
            bool showAllModules = true;

            if (modules != null && modules.Count > 1)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg != null)
                {
                    dlg.Reset();
                    dlg.SetHeading("Select module");
                    dlg.Add(Utility.GetLocalizedText(TextId.All));
                    foreach (string module in modules)
                    {
                        dlg.Add(module);
                    }

                    dlg.DoModal(GetID);
                    if (dlg.SelectedId == -1) return;

                    if (dlg.SelectedId > 0)
                    {
                        if (dlg.SelectedLabel > 0) showAllModules = false;
                        selectedModule = dlg.SelectedLabelText;
                    }
                }
            }

            GUIDialogMenu dlg2 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg2 != null)
            {
                dlg2.Reset();
                dlg2.SetHeading("Select severity");
                dlg2.Add(Utility.GetLocalizedText(TextId.All));
                dlg2.Add(LogSeverity.Error.ToString());
                dlg2.Add(LogSeverity.Fatal.ToString());
                dlg2.Add(LogSeverity.Information.ToString());
                dlg2.Add(LogSeverity.Warning.ToString());

                dlg2.DoModal(GetID);
                if (dlg2.SelectedId == -1) return;

                if (dlg2.SelectedId > 0)
                {
                    if (dlg2.SelectedLabel > 0) showAllSeverities = false;
                    switch (dlg2.SelectedLabel)
                    {
                        case 1:
                            selectedSeverity = LogSeverity.Error;
                            break;

                        case 2:
                            selectedSeverity = LogSeverity.Fatal;
                            break;

                        case 3:
                            selectedSeverity = LogSeverity.Information;
                            break;

                        case 4:
                            selectedSeverity = LogSeverity.Warning;
                            break;
                    }
                }
            }

            bool maxReached;
            List<LogEntry> entries = null;

            if (showAllModules && showAllSeverities)
            {
                SortedList<DateTime, LogEntry> _entries = new SortedList<DateTime, LogEntry>();
                foreach (string module in modules)
                {
                    entries = Proxies.LogService.GetLogEntries(DateTime.Now.AddDays(-_daysToGetLogsFrom), DateTime.Now, _maxLogEntries / 2, module, null, out maxReached);
                    if (entries != null && entries.Count > 0)
                    {
                        foreach (LogEntry entry in entries)
                        {
                            if (!_entries.ContainsKey(entry.LogTime))
                            {
                                _entries.Add(entry.LogTime, entry);
                            }
                        }
                    }
                }

                if (_entries != null && _entries.Count > 0)
                {
                    for (int i = _entries.Count - 1; i >= 0; i--)
                    {
                        text += "#" + _entries.Values[i].LogSeverity.ToString() + ":" + _entries.Values[i].LogTime.ToString() + ":" + _entries.Values[i].Module.ToString() + ":" + _entries.Values[i].Message + "\n";
                    }
                }
            }
            else if (!showAllModules && showAllSeverities)
            {
                entries = Proxies.LogService.GetLogEntries(DateTime.Now.AddDays(-_daysToGetLogsFrom), DateTime.Now, _maxLogEntries, selectedModule, null, out maxReached);
                if (entries != null && entries.Count > 0)
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        text += "#" + entries[i].LogSeverity.ToString() + ":"+ entries[i].LogTime.ToString() + ":" + entries[i].Message + "\n";
                    }
                }
            }
            else if (showAllModules && !showAllSeverities)
            {
                entries = Proxies.LogService.GetLogEntries(DateTime.Now.AddDays(-_daysToGetLogsFrom), DateTime.Now, _maxLogEntries, null, selectedSeverity, out maxReached);
                if (entries != null && entries.Count > 0)
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        text += "#" + entries[i].LogTime.ToString() + ":" + entries[i].Module.ToString() + ":" + entries[i].Message + "\n";
                    }
                }
            }
            else if (!showAllModules && !showAllSeverities)
            {
                entries = Proxies.LogService.GetLogEntries(DateTime.Now.AddDays(-_daysToGetLogsFrom), DateTime.Now, _maxLogEntries, selectedModule, selectedSeverity, out maxReached);
                if (entries != null && entries.Count > 0)
                {
                    for (int i = 0; i < entries.Count; i++)
                    {
                        text += "#" + entries[i].LogTime.ToString() + ":" + entries[i].Message + "\n";
                    }
                }
            }

            GUIDialogText dlg3 = (GUIDialogText)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_TEXT);
            if (dlg3 != null)
            {
                dlg3.Reset();
                dlg3.SetHeading("Log entries");
                dlg3.SetText(text);
                dlg3.DoModal(GetID);
            }
        }

        private KeepUntilMode GetKeepUntilModeFromString(string mode)
        {
            KeepUntilMode keepUntilMode = KeepUntilMode.Forever;
            if (mode == KeepUntilMode.NumberOfDays.ToString())
            {
                keepUntilMode = KeepUntilMode.NumberOfDays;
            }
            else if (mode == KeepUntilMode.NumberOfEpisodes.ToString())
            {
                keepUntilMode = KeepUntilMode.NumberOfEpisodes;
            }
            else if (mode == KeepUntilMode.NumberOfWatchedEpisodes.ToString())
            {
                keepUntilMode = KeepUntilMode.NumberOfWatchedEpisodes;
            }
            else if (mode == KeepUntilMode.UntilSpaceIsNeeded.ToString())
            {
                keepUntilMode = KeepUntilMode.UntilSpaceIsNeeded;
            }
            else if (mode == KeepUntilMode.Forever.ToString())
            {
                keepUntilMode = KeepUntilMode.Forever;
            }
            return keepUntilMode;
        }

        private GuideSource GetGuideSourceFromString(string source)
        {
            GuideSource guideSource = GuideSource.Other;
            if (source == GuideSource.DvbEpg.ToString())
            {
                guideSource = GuideSource.DvbEpg;
            }
            else if (source == GuideSource.XmlTv.ToString())
            {
                guideSource = GuideSource.XmlTv;
            }
            else if (source == GuideSource.Other.ToString())
            {
                guideSource = GuideSource.Other;
            }
            return guideSource;
        }

        #endregion
    }
}
