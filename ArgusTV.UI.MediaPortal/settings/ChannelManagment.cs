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

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Profile;
using Action = MediaPortal.GUI.Library.Action;

using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.MediaPortal
{
    public class ChannelManagment: GUIWindow
    {
        [SkinControlAttribute(6)] protected GUIButtonControl _newChannelGroupButton = null;
        [SkinControlAttribute(7)] protected GUIButtonControl _channelTypeButton = null;
        [SkinControlAttribute(9)] protected GUIListControl _channelGroupsList = null;
        [SkinControlAttribute(10)] protected GUIListControl _channelsInGroupList = null;
        [SkinControlAttribute(11)] protected GUIListControl _allChannelsList = null;
        [SkinControlAttribute(12)] protected GUILabelControl _channelGroupsLabel = null;
        [SkinControlAttribute(13)] protected GUILabelControl _channelInGroupLabel = null;
        [SkinControlAttribute(14)] protected GUILabelControl _allChannelsLabel = null;
        [SkinControlAttribute(15)] protected GUITextScrollUpControl _infoLabel = null;

        private List<Guid> _channelIdsInList = new List<Guid>();
        private ChannelGroup _currentGroup = null;
        private ChannelType _currentChannelType = ChannelType.Television;
        private bool _savingNeeded = false;

        public ChannelManagment()
        {
            GetID = WindowId.ChannelManagment;
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

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _allChannelsLabel.Label = Utility.GetLocalizedText(TextId.ChannelsNotInGroup);
            _channelGroupsLabel.Label = Utility.GetLocalizedText(TextId.ChannelGroups);
            _channelInGroupLabel.Label = Utility.GetLocalizedText(TextId.ChannelsInGroup);
            _newChannelGroupButton.Label = Utility.GetLocalizedText(TextId.NewGroup);

            if (_infoLabel != null)
            {
                _infoLabel.Label = Utility.GetLocalizedText(TextId.ChannelManagmentInfo);
            }
        }

        public override bool Init()
        {
            return Load(GUIGraphicsContext.Skin + @"\ARGUS_ChannelManagment.xml");
        }

        protected override void OnPageLoad()
        {
            PluginMain.EnsureConnection();
            base.OnPageLoad();

            if (PluginMain.IsConnected())
            {
                _currentChannelType = ChannelType.Television;
                LoadAll(null);
                GUIPropertyManager.SetProperty("#currentmodule", Utility.GetLocalizedText(TextId.ChannelManager));
            }
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            SaveChannelsForCurrentGroup();

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
            base.OnPageDestroy(new_windowId);
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_MOVE_UP:
                    OnMoveUp();
                    break;

                case Action.ActionType.ACTION_MOVE_DOWN:
                    OnMoveDown();
                    break;
            }
            base.OnAction(action);
        }

        protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
        {
            if (control == _channelTypeButton)
            {
                ChangeChannelType();
                SaveChannelsForCurrentGroup();
                LoadAll(null);
            }
            else if (control == _newChannelGroupButton)
            {
                CreateNewChannelGroup();
                SaveChannelsForCurrentGroup();
                LoadAll(null);
            }
            else if (control == _channelsInGroupList)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, control.GetID, 0, 0, null);
                OnMessage(msg);
                int iItem = (int)msg.Param1;
                if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
                {
                    GUIListItem item = GetItem(iItem, _channelsInGroupList);
                    OnChannelSelected(item,iItem,false);
                }
            }
            else if (control == _allChannelsList)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, control.GetID, 0, 0, null);
                OnMessage(msg);
                int iItem = (int)msg.Param1;
                if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
                {
                    GUIListItem item = GetItem(iItem, _allChannelsList);
                    OnChannelSelected(item,iItem,true);
                }
            }
            else if (control == _channelGroupsList)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0, control.GetID, 0, 0, null);
                OnMessage(msg);
                int iItem = (int)msg.Param1;
                if (actionType == Action.ActionType.ACTION_SELECT_ITEM)
                {
                    GUIListItem item = GetItem(iItem, _channelGroupsList);
                    OnGroupSelected(item, iItem);
                }
            }
            base.OnClicked(controlId, control, actionType);
        }

        #endregion

        #region Private methods

        private void OnGroupSelected(GUIListItem item, int iItem)
        {
            ChannelGroup group = item.TVTag as ChannelGroup;
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(group.GroupName);

                dlg.Add(Utility.GetLocalizedText(TextId.SelectThisGroup));
                dlg.Add(Utility.GetLocalizedText(TextId.DeleteThisGroup));
                if (group.VisibleInGuide)
                {
                    dlg.Add(Utility.GetLocalizedText(TextId.HideFromGuide));
                }
                else
                {
                    dlg.Add(Utility.GetLocalizedText(TextId.ShowInGuide));
                }

                dlg.DoModal(GetID);
                if (dlg.SelectedId > 0)
                {
                    switch (dlg.SelectedLabel)
                    {
                        case 0:
                            {
                                foreach (GUIListItem Item in _channelGroupsList.ListItems)
                                {
                                    Item.IsRemote = false;
                                }
                                item.IsRemote = true;

                                SaveChannelsForCurrentGroup();
                                LoadAll(group);
                            }
                            break;

                        case 1:
                            {
                                //_channelGroupsList.RemoveItem(iItem);
                                SchedulerAgent.DeleteChannelGroup(group.ChannelGroupId, true, true);
                                LoadAll(null);
                            }
                            break;

                        case 2:
                            {
                                if (group.VisibleInGuide)
                                {
                                    item.IsPlayed = true;
                                    group.VisibleInGuide = false;
                                }
                                else
                                {
                                    item.IsPlayed = false;
                                    group.VisibleInGuide = true;
                                }
                                SchedulerAgent.SaveChannelGroup(group);
                            }
                            break;
                    }
                }
            }
        }

        private void LoadAll(ChannelGroup group)
        {
            if (group == null)
            {
                if (GetAllChannelGroups() != null && GetAllChannelGroups().Length > 0)
                {
                    group = GetAllChannelGroups()[0];
                }
                LoadChannelGroups(group);
            }

            _currentGroup = group;
            LoadChannelsForGroup(group);
            LoadAllChannels();

            switch(_currentChannelType)
            {
                case ChannelType.Television:
                    _channelTypeButton.Label = "Channeltype: television";
                    break;

                case ChannelType.Radio:
                    _channelTypeButton.Label = "Channeltye: radio";
                    break;
            }
        }

        private void LoadChannelGroups(ChannelGroup groupToSelect)
        {
            _channelGroupsList.Clear();

            ChannelGroup[] groups = GetAllChannelGroups();
            if (groups != null && groups.Length > 0)
            {
                foreach (ChannelGroup group in groups)
                {
                    GUIListItem item = new GUIListItem();
                    item.Label = group.GroupName;
                    item.TVTag = group;

                    if (!group.VisibleInGuide)
                    {
                        item.IsPlayed = true;
                    }
                    if (group.ChannelGroupId == groupToSelect.ChannelGroupId)
                    {
                        item.IsRemote = true;
                    }
                    _channelGroupsList.Add(item);
                }
            }
        }

        private void LoadChannelsForGroup(ChannelGroup group)
        {
            _channelsInGroupList.Clear();
            _channelIdsInList.Clear();

            if (group != null)
            {
                Channel[] channels = SchedulerAgent.GetChannelsInGroup(group.ChannelGroupId, false);
                if (channels != null && channels.Length > 0)
                {
                    foreach (Channel channel in channels)
                    {
                        _channelIdsInList.Add(channel.ChannelId);
                        GUIListItem item = new GUIListItem();
                        item.Label = channel.DisplayName;
                        item.TVTag = channel;

                        string logo = Utility.GetLogoImage(channel, SchedulerAgent);
                        if (!string.IsNullOrEmpty(logo))
                        {
                            item.IconImage = logo;
                        }
                        if (!channel.VisibleInGuide)
                        {
                            item.IsPlayed = true;
                        }
                        _channelsInGroupList.Add(item);
                    }
                }
            }
        }

        private void LoadAllChannels()
        {
            _allChannelsList.Clear();

            Channel[] channels = SchedulerAgent.GetAllChannels(_currentChannelType, false);
            if (channels != null && channels.Length > 0)
            {
                foreach (Channel channel in channels)
                {
                    if (!_channelIdsInList.Contains(channel.ChannelId))
                    {
                        GUIListItem item = new GUIListItem();
                        item.Label = channel.DisplayName;
                        item.TVTag = channel;
                        
                        string logo = Utility.GetLogoImage(channel, SchedulerAgent);
                        if (!string.IsNullOrEmpty(logo))
                        {
                            item.IconImage = logo;
                        }
                        if (!channel.VisibleInGuide)
                        {
                            item.IsPlayed = true;
                        }
                        _allChannelsList.Add(item);
                    }
                }
            }
        }

        private void OnChannelSelected (GUIListItem item,int iItem, bool isAllChannelList)
        {
            Channel channel = item.TVTag as Channel;
            if (channel == null) return;

            if (isAllChannelList)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg != null)
                {
                    dlg.Reset();
                    dlg.SetHeading(channel.DisplayName);

                    dlg.Add(Utility.GetLocalizedText(TextId.AddToGroup));
                    if (channel.VisibleInGuide)
                    {
                        dlg.Add(Utility.GetLocalizedText(TextId.HideFromGuide));
                    }
                    else
                    {
                        dlg.Add(Utility.GetLocalizedText(TextId.ShowInGuide));
                    }

                    string text = Utility.GetLocalizedText(TextId.None);
                    if (channel.GuideChannelId.HasValue)
                    {
                        GuideChannel ch = GetGuideChannelForChannel(channel);
                        if (ch != null)
                        {
                            text = ch.Name;
                        }
                    }
                    dlg.Add(Utility.GetLocalizedText(TextId.GuideChannel) + ": " + text);
                    dlg.Add(Utility.GetLocalizedText(TextId.DeleteChannel));

                    dlg.DoModal(GetID);
                    if (dlg.SelectedId > 0)
                    {
                        _savingNeeded = true;

                        switch (dlg.SelectedLabel)
                        {
                            case 0:
                                {
                                    _channelsInGroupList.Add(item);
                                    _allChannelsList.RemoveItem(iItem);
                                }
                                break;

                            case 1:
                                {
                                    if (channel.VisibleInGuide)
                                    {
                                        item.IsPlayed = true;
                                        channel.VisibleInGuide = false;
                                    }
                                    else
                                    {
                                        item.IsPlayed = false;
                                        channel.VisibleInGuide = true;
                                    }
                                    SchedulerAgent.SaveChannel(channel);
                                }
                                break;

                            case 2:
                                {
                                    OnChangeGuideChannel(channel);
                                    item.TVTag = SchedulerAgent.GetChannelById(channel.ChannelId);
                                }
                                break;

                            case 3:
                                {
                                    if (OnDeleteChannel(channel))
                                    {
                                        _allChannelsList.RemoveItem(iItem);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            else// channels in group list
            {
                if (!item.IsRemote)
                {
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    if (dlg != null)
                    {
                        dlg.Reset();
                        dlg.SetHeading(channel.DisplayName);

                        dlg.Add(Utility.GetLocalizedText(TextId.MoveChannel));
                        dlg.Add(Utility.GetLocalizedText(TextId.DeleteChannelFromGroup));
                        if (channel.VisibleInGuide)
                        {
                            dlg.Add(Utility.GetLocalizedText(TextId.HideFromGuide));
                        }
                        else
                        {
                            dlg.Add(Utility.GetLocalizedText(TextId.ShowInGuide));
                        }

                        string text = Utility.GetLocalizedText(TextId.None);
                        if (channel.GuideChannelId.HasValue)
                        {
                            GuideChannel ch = GetGuideChannelForChannel(channel);
                            if (ch != null)
                            {
                                text = ch.Name;
                            }
                        }
                        dlg.Add(Utility.GetLocalizedText(TextId.GuideChannel) + ": " + text);
                        dlg.Add(Utility.GetLocalizedText(TextId.DeleteChannel));

                        dlg.DoModal(GetID);
                        if (dlg.SelectedId > 0)
                        {
                            _savingNeeded = true;

                            switch (dlg.SelectedLabel)
                            {
                                case 0:
                                    item.IsRemote = true;
                                    break;

                                case 1:
                                    {
                                        _channelsInGroupList.RemoveItem(iItem);
                                        _allChannelsList.Add(item);
                                    }
                                    break;

                                case 2:
                                    {
                                        if (channel.VisibleInGuide)
                                        {
                                            item.IsPlayed = true;
                                            channel.VisibleInGuide = false;
                                        }
                                        else
                                        {
                                            item.IsPlayed = false;
                                            channel.VisibleInGuide = true;
                                        }
                                        SchedulerAgent.SaveChannel(channel);
                                    }
                                    break;

                                case 3:
                                    {
                                        OnChangeGuideChannel(channel);
                                        item.TVTag = SchedulerAgent.GetChannelById(channel.ChannelId);
                                    }
                                    break;

                                case 4:
                                    {
                                        if (OnDeleteChannel(channel))
                                        {
                                            _channelsInGroupList.RemoveItem(iItem);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    item.IsRemote = false;
                }
            }
        }

        private void OnChangeGuideChannel(Channel channel)
        {
            GuideChannel[] guideChannels = GuideAgent.GetAllChannels(_currentChannelType);
            if (guideChannels != null && guideChannels.Length > 0)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                if (dlg != null)
                {
                    dlg.Reset();
                    dlg.SetHeading(Utility.GetLocalizedText(TextId.SelectGuideChannel));
                    dlg.Add(Utility.GetLocalizedText(TextId.None));

                    int y = 0;
                    for (int i = 0; i < guideChannels.Length; i++)
                    {
                        GUIListItem item = new GUIListItem();
                        item.Label = guideChannels[i].Name;
                        if (channel.GuideChannelId.HasValue && (guideChannels[i].GuideChannelId == channel.GuideChannelId.Value))
                        {
                            item.IsPlayed = true;
                            y = i + 1;
                        }
                        dlg.Add(item);
                    }
                    dlg.SelectedLabel = y;

                    dlg.DoModal(GetID);
                    if (dlg.SelectedId > 0)
                    {
                        if (dlg.SelectedLabel <= 0)
                        {
                            channel.GuideChannelId = null;
                            SchedulerAgent.SaveChannel(channel);
                        }
                        else
                        {
                            GuideChannel guideChannel = guideChannels[dlg.SelectedLabel - 1];
                            SchedulerAgent.AttachChannelToGuide(channel.ChannelId, guideChannel.GuideChannelId);
                        }
                    }
                }
            }
        }

        private GuideChannel GetGuideChannelForChannel(Channel channel)
        {
            GuideChannel guideChannel = null;
            GuideChannel[] guideChannels = GuideAgent.GetAllChannels(_currentChannelType);
            if (guideChannels != null && guideChannels.Length > 0)
            {
                foreach (GuideChannel gChannel in guideChannels)
                {
                    if (gChannel.GuideChannelId == channel.GuideChannelId.Value)
                    {
                        guideChannel = gChannel;
                    }
                }
            }
            return guideChannel;
        }

        private bool OnDeleteChannel(Channel channel)
        {
            GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
            if (dlgYesNo != null)
            {
                dlgYesNo.Reset();
                dlgYesNo.SetHeading(channel.DisplayName);
                dlgYesNo.SetLine(1, Utility.GetLocalizedText(TextId.DeleteChannel));
                dlgYesNo.SetLine(2, Utility.GetLocalizedText(TextId.AreYouSure));
                dlgYesNo.SetLine(3, string.Empty);
                dlgYesNo.SetDefaultToYes(false);
                dlgYesNo.DoModal(GetID);

                if (dlgYesNo.IsConfirmed)
                {
                    SchedulerAgent.DeleteChannel(channel.ChannelId, true);
                    return true;
                }
            }
            return false;
        }

        private void SaveChannelsForCurrentGroup()
        {
            if (_savingNeeded)
            {
                _savingNeeded = false;

                Guid[] channelIds;
                if (_channelsInGroupList != null && _channelsInGroupList.Count > 0)
                {
                    channelIds = new Guid[_channelsInGroupList.Count];
                    for (int i = 0; i < _channelsInGroupList.Count; i++)
                    {
                        Channel channel = _channelsInGroupList[i].TVTag as Channel;
                        channelIds[i] = channel.ChannelId;
                    }
                }
                else
                {
                    channelIds = new Guid[0];
                }

                if (_currentGroup != null)
                {
                    SchedulerAgent.SetChannelGroupMembers(_currentGroup.ChannelGroupId, channelIds);
                }
            }
        }

        private void CreateNewChannelGroup()
        {
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
            if (keyboard != null)
            { 
                keyboard.Reset();
                keyboard.IsSearchKeyboard = false;
                keyboard.Text = string.Empty;
                keyboard.DoModal(GetID);
                if (keyboard.IsConfirmed && keyboard.Text != string.Empty)
                {
                    ChannelGroup group = new ChannelGroup()
                    {
                        Sequence = GetAllChannelGroups().Length,
                        ChannelType = _currentChannelType,
                        VisibleInGuide = true,
                        GroupName = keyboard.Text
                    };

                    SchedulerAgent.SaveChannelGroup(group);
                }
            }
        }

        private void DeleteChannelGroup(ChannelGroup group)
        {
            if (group != null)
            {
                SchedulerAgent.DeleteChannelGroup(group.ChannelGroupId, true, true);
            }
        }

        private ChannelGroup[] GetAllChannelGroups()
        {
            return SchedulerAgent.GetAllChannelGroups(_currentChannelType, true);
        }

        private void ChangeChannelType()
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(Utility.GetLocalizedText(TextId.Type));

                dlg.Add(Utility.GetLocalizedText(TextId.Television));
                dlg.Add(Utility.GetLocalizedText(TextId.Radio));
                switch (_currentChannelType)
                {
                    case ChannelType.Television:
                        dlg.SelectedLabel = 0;
                        break;

                    case ChannelType.Radio:
                        dlg.SelectedLabel = 1;
                        break;
                }

                dlg.DoModal(GetID);
                if (dlg.SelectedId > 0)
                {
                    switch (dlg.SelectedLabel)
                    {
                        case 0:
                            _currentChannelType = ChannelType.Television;
                            break;

                        case 1:
                            _currentChannelType = ChannelType.Radio;
                            break;
                    }
                }
            }
        }

        #endregion

        #region Listcontrol

        private void OnMoveUp()
        {
            if (_channelsInGroupList != null && _channelsInGroupList.Count > 0
                && _channelsInGroupList.IsFocused)
            {
                int index = GetSelectedItemIndex(_channelsInGroupList);
                if (_channelsInGroupList[index].IsRemote && index > 0)
                {
                    _channelsInGroupList.MoveItemUp(index);
                }
            }
        }

        private void OnMoveDown()
        {
            if (_channelsInGroupList != null && _channelsInGroupList.Count > 0
                && _channelsInGroupList.IsFocused)
            {
                int index = GetSelectedItemIndex(_channelsInGroupList);
                if (_channelsInGroupList[index].IsRemote && index < _channelsInGroupList.Count - 1)
                {
                    _channelsInGroupList.MoveItemDown(index);
                }
            }
        }

        private int GetSelectedItemIndex(GUIListControl listControl)
        {
            if (listControl.Count > 0)
            {
                return listControl.SelectedListItemIndex;
            }
            else
            {
                return -1;
            }
        }

        private void SelectItemByIndex(int index, GUIListControl listControl)
        {
            while (index >= listControl.Count && index > 0)
            {
                index--;
            }
            GUIControl.SelectItemControl(GetID, listControl.GetID, index);
        }

        private GUIListItem GetItem(int index, GUIListControl listControl)
        {
            return listControl[index];
        }

        #endregion
    }
}
