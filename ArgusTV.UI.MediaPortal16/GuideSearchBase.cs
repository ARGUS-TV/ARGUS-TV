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
using ArgusTV.UI.Process;
using ArgusTV.UI.Process.Guide;

namespace ArgusTV.UI.MediaPortal
{
    public abstract class GuideSearchBase : GUIWindow, IComparer<GUIListItem>
    {
        protected abstract string SettingsSection { get; }

        public GuideSearchBase(ChannelType channelType)
        {
            _channelType = channelType;
        }

        #region variables

        private enum SortMethod
        {
            Channel = 0,
            Date = 1,
            Name = 2
        }

        private enum SearchInMethod
        {
            Title = 0,
            Description = 1,
            ProgramInfo = 2,
            Actor = 3,
            DirectedBy = 4
        }

        private SortMethod _currentSortMethod = SortMethod.Date;
        private SearchInMethod _currentSearchMethod = SearchInMethod.Title;

        private const string _parentDirectoryLabel = "..";
        private bool _sortAscending = true;
        private int _selectedTitleIndex = 0;
        private int _selectedProgramIndex = 0;
        private bool _isInSubDirectory = false;
        private string _selectedTitle = string.Empty;

        private ChannelType _channelType;
        private List<ScheduleRule> _rules = new List<ScheduleRule>();
        List<object> _categorieArguments = new List<object>();
        List<object> _channelArguments = new List<object>();

        [SkinControlAttribute(2)] protected GUIButtonControl _searchButton;
        [SkinControlAttribute(6)] protected GUIButtonControl _searchMethodButton;
        [SkinControlAttribute(7)] protected GUIButtonControl _selectChannelsButton;
        [SkinControlAttribute(8)] protected GUIButtonControl _selectCategoriesButton;
        [SkinControlAttribute(3)] protected GUIButtonControl _clearButton;
        [SkinControlAttribute(4)] protected GUISortButtonControl _sortByButton;
        [SkinControlAttribute(10)] protected GUIListControl _viewsList;

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

        #endregion

        #region Serialisation

        private void LoadSettings()
        {
            using (Settings xmlreader = new MPSettings())
            {
                string strTmp = String.Empty;
                strTmp = xmlreader.GetValueAsString(this.SettingsSection, "sort", "date");
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
                strTmp = xmlreader.GetValueAsString(this.SettingsSection, "searchmethod", "title");
                if (strTmp != null)
                {
                    if (strTmp == "title")
                    {
                        _currentSearchMethod = SearchInMethod.Title;
                    }
                    else if (strTmp == "description")
                    {
                        _currentSearchMethod = SearchInMethod.Description;
                    }
                    else if (strTmp == "programinfo")
                    {
                        _currentSearchMethod = SearchInMethod.ProgramInfo;
                    }
                    else if (strTmp == "actor")
                    {
                        _currentSearchMethod = SearchInMethod.Actor;
                    }
                    else if (strTmp == "directedby")
                    {
                        _currentSearchMethod = SearchInMethod.DirectedBy;
                    }
                }

                _sortAscending = xmlreader.GetValueAsBool(this.SettingsSection, "sortascending", true);
            }
        }

        private void SaveSettings()
        {
            using (Settings xmlwriter = new MPSettings())
            {
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

                switch (_currentSearchMethod)
                {
                    case SearchInMethod.Title:
                        xmlwriter.SetValue(this.SettingsSection, "searchmethod", "title");
                        break;
                    case SearchInMethod.Description:
                        xmlwriter.SetValue(this.SettingsSection, "searchmethod", "description");
                        break;
                    case SearchInMethod.ProgramInfo:
                        xmlwriter.SetValue(this.SettingsSection, "searchmethod", "programinfo");
                        break;
                    case SearchInMethod.Actor:
                        xmlwriter.SetValue(this.SettingsSection, "searchmethod", "actor");
                        break;
                    case SearchInMethod.DirectedBy:
                        xmlwriter.SetValue(this.SettingsSection, "searchmethod", "directedby");
                        break;
                }

                xmlwriter.SetValueAsBool(this.SettingsSection, "sortascending", _sortAscending);
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

        public override void OnAction(Action action)
        {
            if (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)
            {
                if (_viewsList.Focus)
                {
                    GUIListItem item = GetItem(0);
                    if (item != null)
                    {
                        if (item.IsFolder && item.Label == _parentDirectoryLabel)
                        {
                            ShowSearchResults(string.Empty);
                            return;
                        }
                    }
                }
            }
            base.OnAction(action);
        }

        protected override void OnPageDestroy(int newWindowId)
        {
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
            if (newWindowId != WindowId.ProgramInfo)
            {
                OnClearRules(false);
                _selectedTitle = string.Empty;
                _selectedTitleIndex = 0;
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
                if (GUIWindowManager.GetPreviousActiveWindow() != WindowId.ProgramInfo)
                {
                    OnClearRules(false);
                    _selectedTitle = string.Empty;
                    _selectedTitleIndex = 0;
                }
                ShowSearchResults(_selectedTitle);
                UpdateButtonStates();
                _sortByButton.SortChanged += new SortEventHandler(SortChanged);

                if (this._channelType == ChannelType.Television)
                    GUIPropertyManager.SetProperty("#currentmodule", Utility.GetLocalizedText(TextId.TVGuideSearch));
                else
                    GUIPropertyManager.SetProperty("#currentmodule", Utility.GetLocalizedText(TextId.RadioGuideSearch));
            }
        }

        protected override void OnClicked(int controlId, GUIControl control, global::MediaPortal.GUI.Library.Action.ActionType actionType)
        {
            base.OnClicked(controlId, control, actionType);

            if (control == _searchButton)
            {
                _selectedTitleIndex = 0;
                _selectedProgramIndex = 0;

                if (_rules.Count > 0)
                {
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    if (dlgYesNo != null)
                    {
                        dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.Attention));
                        dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.ContinueWithPrevResults));
                        dlgYesNo.SetDefaultToYes(true);
                        dlgYesNo.DoModal(GetID);
                        if (!dlgYesNo.IsConfirmed)
                        {
                            _rules.Clear();
                            if (_viewsList != null && _viewsList.Count > 0)
                                _viewsList.Clear();
                        }
                    }
                }

                VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
                if (keyboard != null)
                {
                    keyboard.Reset();
                    keyboard.IsSearchKeyboard = true;
                    keyboard.Text = String.Empty;
                    keyboard.DoModal(GetID);
                    if (keyboard.IsConfirmed)
                    {
                        if (keyboard.Text == string.Empty)
                        {
                            GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
                            if (dlgOk != null)
                            {
                                dlgOk.SetHeading(Utility.GetLocalizedText(TextId.Information));
                                dlgOk.SetLine(1, Utility.GetLocalizedText(TextId.NoValidSearchText));
                                dlgOk.DoModal(GetID);
                            }
                        }
                        else
                        {
                            switch (_currentSearchMethod)
                            {
                                case SearchInMethod.Title:
                                    _rules.Add(ScheduleRuleType.TitleContains, keyboard.Text);
                                    break;
                                case SearchInMethod.Description:
                                    _rules.Add(ScheduleRuleType.DescriptionContains, keyboard.Text);
                                    break;
                                case SearchInMethod.ProgramInfo:
                                    _rules.Add(ScheduleRuleType.ProgramInfoContains, keyboard.Text);
                                    break;
                                case SearchInMethod.Actor:
                                    _rules.Add(ScheduleRuleType.WithActor, keyboard.Text);
                                    break;
                                case SearchInMethod.DirectedBy:
                                    _rules.Add(ScheduleRuleType.DirectedBy, keyboard.Text);
                                    break;
                            }
                            ShowSearchResults(string.Empty);
                        }
                    }
                }
            }
            else if (control == _searchMethodButton)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
                if (dlg == null)
                {
                    return;
                }
                dlg.Reset();
                dlg.SetHeading(467);
                dlg.Add(Utility.GetLocalizedText(TextId.SearchOnTitle));
                dlg.Add(Utility.GetLocalizedText(TextId.SearchOnDescription));
                dlg.Add(Utility.GetLocalizedText(TextId.SearchOnProgramInfo));
                dlg.Add(Utility.GetLocalizedText(TextId.SearchOnActor));
                dlg.Add(Utility.GetLocalizedText(TextId.SearchOnDirectedBy));
                
                dlg.SelectedLabel = (int)_currentSearchMethod;
                // show dialog and wait for result
                dlg.DoModal(GetID);
                if (dlg.SelectedId == -1)
                {
                    return;
                }
                _currentSearchMethod = (SearchInMethod)(dlg.SelectedLabel);
                UpdateButtonStates();
            }
            else if (control == _selectChannelsButton)
            {
                List<ChannelGroup> groups = new List<ChannelGroup>(PluginMain.Navigator.GetGroups(this._channelType));
                ChannelGroup _selectedGroup = new ChannelGroup();

                if (groups.Count > 0)
                {
                    if (groups.Count > 1)
                    {
                        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
                        if (dlg == null)
                        {
                            return;
                        }

                        dlg.Reset();
                        dlg.SetHeading(Utility.GetLocalizedText(TextId.SelectGroup));
                        foreach (ChannelGroup group in groups)
                        {
                            dlg.Add(group.GroupName);
                        }

                        // show dialog and wait for result
                        dlg.DoModal(GetID);
                        if (dlg.SelectedId == -1)
                        {
                            return;
                        }
                        _selectedGroup = groups[dlg.SelectedId - 1];
                    }
                    else
                    {
                        _selectedGroup = groups[0];
                    }

                    List<Channel> channels = new List<Channel>();
                    if (_channelArguments.Count > 0)
                    {
                        List<Channel> channels2 = new List<Channel>(SchedulerAgent.GetChannelsInGroup(_selectedGroup.ChannelGroupId, true));
                        foreach (Channel channel in channels2)
                        {
                            if (!_channelArguments.Contains(channel.ChannelId))
                            {
                                channels.Add(channel);
                            }
                        }
                    }
                    else
                    {
                        channels = new List<Channel>(SchedulerAgent.GetChannelsInGroup(_selectedGroup.ChannelGroupId, true));
                    }

                    if (channels.Count > 0)
                    {
                        GUIDialogMenu dlg2 = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
                        if (dlg2 == null)
                        {
                            return;
                        }

                        dlg2.Reset();
                        dlg2.SetHeading(GetChannelArgumentsString(true)); 
                        foreach (Channel channel in channels)
                        {
                            dlg2.Add(channel.DisplayName);
                        }

                        // show dialog and wait for result
                        dlg2.DoModal(GetID);
                        if (dlg2.SelectedId == -1)
                        {
                            return;
                        }
                        _channelArguments.Add(channels[dlg2.SelectedId - 1].ChannelId);
                        UpdateButtonStates();
                    }
                    else
                    {
                        GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
                        if (dlgOk != null)
                        {
                            dlgOk.SetHeading(Utility.GetLocalizedText(TextId.Information));
                            dlgOk.SetLine(1, Utility.GetLocalizedText(TextId.NoMoreChannelsToAdd));
                            dlgOk.DoModal(GetID);
                        }
                    }
                }
            }
            else if (control == _selectCategoriesButton)
            {
                List<string> categories = new List<string>();
                string[] _categories = new string[0];
                _categories = GuideAgent.GetAllCategories();

                foreach (string categorie in _categories)
                {
                    if (!_categorieArguments.Contains(categorie))
                    {
                        categories.Add(categorie);
                    }
                }

                if (categories.Count > 0)
                {
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
                    if (dlg == null)
                    {
                        return;
                    }

                    dlg.Reset();
                    dlg.SetHeading(GetCategorieArgumentString(true));
                    foreach (string categorie in categories)
                    {
                        dlg.Add(categorie);
                    }

                    // show dialog and wait for result
                    dlg.DoModal(GetID);
                    if (dlg.SelectedId == -1)
                    {
                        return;
                    }
                    _categorieArguments.Add(dlg.SelectedLabelText);
                    UpdateButtonStates();
                }
                else
                {
                    GUIDialogOK dlgOk = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
                    if (dlgOk != null)
                    {
                        dlgOk.SetHeading(Utility.GetLocalizedText(TextId.Information));
                        dlgOk.SetLine(1, Utility.GetLocalizedText(TextId.NoMoreCategoriesToAdd));
                        dlgOk.DoModal(GetID);
                    }
                }
            }
            else if (control == _clearButton)
            {
                OnClearRules(true);
                ShowSearchResults(string.Empty);
                UpdateButtonStates();
            }
            else if (control == _sortByButton)
            {
                if (_isInSubDirectory)
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
                    dlg.AddLocalizedString(268); //title/name

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
            }
            else if (control == _viewsList)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, control.GetID, 0, 0, null);
                OnMessage(msg);
                int iItem = (int)msg.Param1;
                if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
                {
                    OnSelectItem(iItem);
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
            int itemIndex = GetSelectedItemNo();
            GUIListItem item = GetItem(itemIndex);
            if (item == null
                || item.IsFolder)
            {
                return;
            }
            UpcomingProgram program = item.TVTag as UpcomingProgram;
            if (program != null)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg == null) return;
                dlg.Reset();
                dlg.SetHeading(program.Title);
                dlg.Add(Utility.GetLocalizedText(TextId.Information));
                dlg.DoModal(GetID);
                switch (dlg.SelectedLabel)
                {
                    case 0: // information
                        OnEditSchedule(program);
                        break;
                }
            }
        }

        #endregion

        #region Search methods

        private void ShowSearchResults(string selectedTitle)
        {
            GUIControl.ClearControl(GetID, _viewsList.GetID);
            if (_rules == null || _rules.Count == 0)
            {
                _viewsList.Clear();
                _selectedTitle = string.Empty;
                _isInSubDirectory = false;
                return;
            }

            List<ScheduleRule> rules = new List<ScheduleRule>();
            for (int i = 0; i < _rules.Count; i++)
            {
                rules.Add(_rules[i]);
            }

            if (selectedTitle != string.Empty)
            {
                rules.Add(ScheduleRuleType.TitleEquals, selectedTitle);
            }

            if (_categorieArguments.Count > 0)
            {
                rules.Add(ScheduleRuleType.CategoryEquals, _categorieArguments.ToArray());
            }
            if (_channelArguments.Count > 0)
            {
                rules.Add(ScheduleRuleType.Channels, _channelArguments.ToArray());
            }

            Schedule _schedule = SchedulerAgent.CreateNewSchedule(this._channelType, ScheduleType.Recording);
            _schedule.Rules = rules;
            UpcomingProgram[] _searchResults = SchedulerAgent.GetUpcomingPrograms(_schedule, true);

            if (_searchResults.Length < 1)
            {
                GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                dlg.Reset();
                dlg.SetHeading(Utility.GetLocalizedText(TextId.Information));
                dlg.SetLine(1, Utility.GetLocalizedText(TextId.NoProgrammesFound));
                dlg.DoModal(GUIWindowManager.ActiveWindow);
                _isInSubDirectory = false;
                _selectedTitle = string.Empty;
                return;
            }

            _selectedTitle = selectedTitle;
            if (selectedTitle == string.Empty)
            {
                List<String> upcomingTitles = new List<String>();
                foreach (UpcomingProgram program in _searchResults)
                {
                    if (!upcomingTitles.Contains(program.Title))
                    {
                        upcomingTitles.Add(program.Title);
                        _viewsList.Add(CreateListItem(program,true));
                    }
                }
                _isInSubDirectory = false;
            }
            else
            {
                GUIListItem item = new GUIListItem();
                item.Label = _parentDirectoryLabel;
                item.IsFolder = true;
                Utils.SetDefaultIcons(item);
                _viewsList.Add(item);

                foreach (UpcomingProgram program in _searchResults)
                {
                    _viewsList.Add(CreateListItem(program,false));
                }
                _isInSubDirectory = true;
            }

            string strObjects = string.Format("{0}", _viewsList.Count - (_isInSubDirectory ? 1 : 0));
            GUIPropertyManager.SetProperty("#itemcount", strObjects);

            OnSort(); 
            int selectedItemIndex;
            if (_isInSubDirectory)
                selectedItemIndex = _selectedProgramIndex;
            else
                selectedItemIndex = _selectedTitleIndex;

            while (selectedItemIndex >= _viewsList.Count && selectedItemIndex > 0)
            {
                selectedItemIndex--;
            }
            GUIControl.SelectItemControl(GetID, _viewsList.GetID, selectedItemIndex);
            UpdateProperties();
        }

        private GUIListItem CreateListItem(UpcomingProgram program, bool isFolder)
        {
            GUIListItem item = new GUIListItem();
            item.IsFolder = isFolder;
            item.TVTag = program;
            return item;
        }

        private void UpdateButtonStates()
        {
            if (_viewsList == null || _viewsList.Count == 0)
                _sortByButton.IsEnabled = false;
            else
                _sortByButton.IsEnabled = true;

            string text = String.Empty;
            if (_isInSubDirectory)
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
            }
            else
            {
                text = Utility.GetLocalizedText(TextId.SortByTitle);
            }
            GUIControl.SetControlLabel(GetID, _sortByButton.GetID, text);

            text = String.Empty;
            switch (_currentSearchMethod)
            {
                case SearchInMethod.Title:
                    text = Utility.GetLocalizedText(TextId.SearchOnTitle);
                    break;
                case SearchInMethod.Description:
                    text = Utility.GetLocalizedText(TextId.SearchOnDescription);
                    break;
                case SearchInMethod.ProgramInfo:
                    text = Utility.GetLocalizedText(TextId.SearchOnProgramInfo);
                    break;
                case SearchInMethod.Actor:
                    text = Utility.GetLocalizedText(TextId.SearchOnActor);
                    break;
                case SearchInMethod.DirectedBy:
                    text = Utility.GetLocalizedText(TextId.SearchOnDirectedBy);
                    break;
            }
            GUIControl.SetControlLabel(GetID, _searchMethodButton.GetID, text);

            GUIControl.SetControlLabel(GetID, _selectChannelsButton.GetID, GetChannelArgumentsString(false));
            GUIControl.SetControlLabel(GetID, _selectCategoriesButton.GetID , GetCategorieArgumentString(false));
            _sortByButton.IsAscending = _sortAscending;
        }

        private string GetChannelArgumentsString(bool isforDialog)
        {
            string text = string.Empty;
            text = Utility.GetLocalizedText(TextId.Channels) + " ";
            if (_channelArguments.Count > 0)
            {
                foreach (object argument in _channelArguments)
                {
                    Channel chan = SchedulerAgent.GetChannelById(new Guid(argument.ToString()));
                    text += chan.DisplayName + ",";
                }
                text = text.Remove(text.Length - 1);
            }
            else
            {
                if (isforDialog)
                    text = Utility.GetLocalizedText(TextId.SelectChannel);
                else
                    text += Utility.GetLocalizedText(TextId.All);
            }
            return text;
        }

        private string GetCategorieArgumentString(bool isforDialog)
        {
            string text = String.Empty;
            text = Utility.GetLocalizedText(TextId.Categories) + " ";
            if (_categorieArguments.Count > 0)
            {
                foreach (object argument in _categorieArguments)
                {
                    text += argument.ToString() + ",";
                }
                text = text.Remove(text.Length - 1);
            }
            else
            {
                if (isforDialog)
                    text = Utility.GetLocalizedText(TextId.SelectCategorie);
                else
                    text += Utility.GetLocalizedText(TextId.All);
            }
            return text;
        }

        private void OnClearRules(bool withDialog)
        {
            if (_categorieArguments.Count > 0 || _channelArguments.Count > 0)
            {
                if (withDialog)
                {
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    if (dlgYesNo != null)
                    {
                        dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.Attention));
                        dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.AlsoDeleteChannelAndCategorie));
                        dlgYesNo.SetLine(2, string.Empty);
                        dlgYesNo.SetLine(3, string.Empty);
                        dlgYesNo.SetDefaultToYes(true);
                        dlgYesNo.DoModal(GetID);
                        if (dlgYesNo.IsConfirmed)
                        {
                            _categorieArguments.Clear();
                            _channelArguments.Clear();
                        }
                    }
                }
                else
                {
                    _categorieArguments.Clear();
                    _channelArguments.Clear();
                }
            }
            _rules.Clear();
        }

        private void SetLabels()
        {
            UpcomingRecording[] upcomingRecordings = ControlAgent.GetAllUpcomingRecordings(UpcomingRecordingsFilter.All, true);
            UpcomingGuideProgram[] upcomingAlerts = SchedulerAgent.GetUpcomingGuidePrograms(ScheduleType.Alert, true);
            UpcomingGuideProgram[] upcomingSuggestions = SchedulerAgent.GetUpcomingGuidePrograms(ScheduleType.Suggestion, true);
            UpcomingGuideProgramsDictionary AllUpcomingGuidePrograms = new UpcomingGuideProgramsDictionary(upcomingRecordings, upcomingAlerts, upcomingSuggestions);

            foreach (GUIListItem item in _viewsList.ListItems)
            {
                if (item.Label != _parentDirectoryLabel)
                {
                    UpcomingProgram program = item.TVTag as UpcomingProgram;
                    if (program != null)
                    {
                        if (!item.IsFolder)
                        {
                            item.PinImage = null;
                            Guid upcomingProgramId = program.UpcomingProgramId;
                            if (AllUpcomingGuidePrograms.ContainsKey(upcomingProgramId))
                            {
                                GuideUpcomingProgram programInfo = AllUpcomingGuidePrograms[upcomingProgramId];
                                item.PinImage = Utility.GetIconImageFileName(programInfo.Type, programInfo.IsPartOfSeries,
                                programInfo.CancellationReason, programInfo.UpcomingRecording);
                            }

                            string title = GuideProgram.CreateProgramTitle(program.Title, program.SubTitle, program.EpisodeNumberDisplay);
                            item.Label = title;
                            string logoImagePath = Utility.GetLogoImage(program.Channel, _tvSchedulerAgent);
                            if (logoImagePath == null
                                || !System.IO.File.Exists(logoImagePath))
                            {
                                item.Label = String.Format("[{0}] {1}", program.Channel.DisplayName, title);
                                logoImagePath = "defaultVideoBig.png";
                            }

                            item.ThumbnailImage = logoImagePath;
                            item.IconImageBig = logoImagePath;
                            item.IconImage = logoImagePath;

                            item.Label2 = String.Format("{0} {1} - {2}", Utility.GetShortDayDateString(program.StartTime),
                                program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                            if (_currentSortMethod == SortMethod.Channel)
                            {
                                item.Label3 = program.Channel.DisplayName;
                            }
                            else
                            {
                                item.Label3 = program.Category;
                            }
                        }
                        else
                        {
                            Utils.SetDefaultIcons(item);
                            item.Label = program.Title;
                        }
                    }
                }
            }
            AllUpcomingGuidePrograms.Clear();
            AllUpcomingGuidePrograms = null;
        }

        private bool OnSelectItem(int iItem)
        {
            GUIListItem item = GetItem(iItem);
            if (item != null)
            {
                if (item.Label == _parentDirectoryLabel)
                {
                    ShowSearchResults(string.Empty);
                }
                else if (item.IsFolder)
                {
                    _selectedTitleIndex = GetSelectedItemNo();
                    _selectedProgramIndex = 0;
                    ShowSearchResults(item.Label);
                }
                else
                {
                    _selectedProgramIndex = GetSelectedItemNo();
                    OnEditSchedule(item.TVTag as UpcomingProgram);
                }
            }
            return false;
        }

        private bool OnEditSchedule(UpcomingProgram program)
        {
            if (program != null)
            {
                TvProgramInfo.CurrentProgram = _tvGuideAgent.GetProgramById(program.GuideProgramId.Value);
                TvProgramInfo.Channel = program.Channel;
                GUIWindowManager.ActivateWindow((int)WindowId.ProgramInfo);
                return true;
            }
            return false;
        }

        private void UpdateProperties()
        {
            GUIListItem item = GetItem(GetSelectedItemNo());
            if (item == null || item.IsFolder)
            {
                SetProperties(null);
            }
            else
            {
                SetProperties(item.TVTag as UpcomingProgram);
            }
        }

        private void SetProperties(UpcomingProgram program)
        {
            string guiPropertyPrefix = _channelType == ChannelType.Television ? "#TV" : "#Radio";
            if (program == null)
            {
                Utility.ClearProperty(guiPropertyPrefix + ".Search.Channel");
                Utility.ClearProperty(guiPropertyPrefix + ".Search.Title");
                Utility.ClearProperty(guiPropertyPrefix + ".Search.Genre");
                Utility.ClearProperty(guiPropertyPrefix + ".Search.Time");
                Utility.ClearProperty(guiPropertyPrefix + ".Search.Description");
                Utility.ClearProperty(guiPropertyPrefix + ".Search.thumb");
            }
            else
            {
                string strTime = string.Format("{0} {1} - {2}",
                  Utility.GetShortDayDateString(program.StartTime),
                  program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                  program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Search.Channel", program.Channel.DisplayName);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Search.Title", program.Title);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Search.Genre", program.Category);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Search.Time", strTime);
                string logo = Utility.GetLogoImage(program.Channel.ChannelId, program.Channel.DisplayName, _tvSchedulerAgent);
                if (System.IO.File.Exists(logo))
                {
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Search.thumb", logo);
                }
                else
                {
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Search.thumb", "defaultVideoBig.png");
                }

                GuideProgram guideProgram = GuideAgent.GetProgramById(program.GuideProgramId.Value);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Search.Description", guideProgram.CreateCombinedDescription(true));
            }
        }

        #endregion

        #region Album/List View Management

        private GUIListItem GetSelectedItem()
        {
            return GUIControl.GetSelectedListItem(GetID, _viewsList.GetID);
        }

        private GUIListItem GetItem(int iItem)
        {
            if (iItem < 0 || iItem >= _viewsList.Count)
            {
                return null;
            }
            return _viewsList[iItem];
        }

        private int GetSelectedItemNo()
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, _viewsList.GetID, 0, 0, null);
            OnMessage(msg);
            return (int)msg.Param1;
        }

        #endregion

        #region Sort Members

        private void OnSort()
        {
            SetLabels();
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
            if (item1.IsFolder && item1.Label == _parentDirectoryLabel)
            {
                return -1;
            }
            else if (item2.IsFolder && item2.Label == _parentDirectoryLabel)
            {
                return 1;
            }

            if (item1.IsFolder && item2.IsFolder)
            {
                switch (_currentSortMethod)
                {
                    case SortMethod.Channel:
                    case SortMethod.Date:
                    case SortMethod.Name:
                        if (item1.Label == null && item2.Label == null)
                        {
                            result = 0;
                        }
                        else if (item1.Label == null)
                        {
                            result = resultLower;
                        }
                        else if (item2.Label == null)
                        {
                            result = resultUpper;
                        }
                        else
                        {
                            result = resultUpper * String.Compare(item1.Label, item2.Label, StringComparison.OrdinalIgnoreCase);
                        }
                        break;
                }
            }
            else
            {
                UpcomingProgram program1 = item1.TVTag as UpcomingProgram;
                UpcomingProgram program2 = item2.TVTag as UpcomingProgram;
                switch (_currentSortMethod)
                {
                    case SortMethod.Channel:
                        string channel1 = program1.Channel.DisplayName;
                        string channel2 = program2.Channel.DisplayName;

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

                    case SortMethod.Name:
                        string title1 = program1.Title;
                        string title2 = program2.Title;

                        if (title1 == null && title2 == null)
                        {
                            result = 0;
                        }
                        else if (title1 == null)
                        {
                            result = resultLower;
                        }
                        else if (title2 == null)
                        {
                            result = resultUpper;
                        }
                        else
                        {
                            result = resultUpper * String.Compare(title1, title2, StringComparison.OrdinalIgnoreCase);
                        }
                        if (result == 0)
                        {
                            goto case SortMethod.Date;
                        }
                        break;

                    case SortMethod.Date:
                        DateTime time1 = program1.StartTime;
                        DateTime time2 = program2.StartTime;

                        if (time1 != time2)
                        {
                            result = (time1 < time2) ? resultLower : resultUpper;
                        }
                        break;
                }
            }
            return result;
        }

        private void SortChanged(object sender, SortEventArgs e)
        {
            _sortAscending = e.Order != System.Windows.Forms.SortOrder.Descending;
            OnSort();
        }

        #endregion
    }
}
