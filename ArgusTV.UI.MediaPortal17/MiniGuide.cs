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

#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Util;
using Action = MediaPortal.GUI.Library.Action;
using MediaPortal.Dialogs;
using MediaPortal.Profile;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

#endregion

namespace ArgusTV.UI.MediaPortal
{
    /// <summary>
    /// GUIMiniGuide
    /// </summary>
    /// 
    public class MiniGuide : GUIDialogWindow
    {
        // Member variables                                  
        [SkinControlAttribute(34)] protected GUIButtonControl _exitButton = null;
        [SkinControlAttribute(35)] protected GUIListControl _channelsListControlNoStateIcons = null;
        [SkinControlAttribute(36)] protected GUISpinControl _spinGroup = null;
        [SkinControlAttribute(37)] protected GUIListControl _channelsListControlWithStateIcons = null;

        protected GUIListControl _channelsListControl;
        private bool _canceled = false;
        private Channel _selectedChannel = null;
        private List<ChannelGroup> _channelGroupList = new List<ChannelGroup>();
        private List<CurrentAndNextProgram> _currentAndNextPrograms = new List<CurrentAndNextProgram>();
        private ChannelGroup _currentGroup;

        //private Stopwatch benchClock = null;
        private StringBuilder sb = new StringBuilder();
        private StringBuilder sbTmp = new StringBuilder();

        private bool _byIndex = false;
        private bool _showStateIcons;
        private bool _showChannelNumber = false;
        //private int _channelNumberMaxLength = 3;

        private readonly string UnavailableIcon = Thumbs.TvIsUnavailableIcon;
        private readonly string AvailableIcon = Thumbs.TvIsAvailableIcon;
        private readonly string TimeshiftingIcon = Thumbs.TvIsTimeshiftingIcon;
        private readonly string RecordingIcon = Thumbs.TvIsRecordingIcon;
        private readonly string noDataAvailableText = Utility.GetLocalizedText(TextId.NoDataAvailable);
        private readonly string nowText = Utility.GetLocalizedText(TextId.Now);
        private readonly string nextText = Utility.GetLocalizedText(TextId.Next);
        private readonly string recordingText = Utility.GetLocalizedText(TextId.Recording);
        private readonly string timeshiftingText = Utility.GetLocalizedText(TextId.Timeshifting);
        private readonly string unavailableText = Utility.GetLocalizedText(TextId.Unavailable);

        /// <summary>
        /// Constructor
        /// </summary>
        public MiniGuide()
        {
            GetID = (int)GUIWindow.Window.WINDOW_MINI_GUIDE;
        }

        #region Serialisation

        private void LoadSettings()
        {
            using (Settings xmlreader = new MPSettings())
            {
                _byIndex = xmlreader.GetValueAsBool("mytv", "byindex", true);
                _showChannelNumber = xmlreader.GetValueAsBool("mytv", "showchannelnumber", false);
                //_channelNumberMaxLength = xmlreader.GetValueAsInt("mytv", "channelnumbermaxlength", 3);
            }
        }

        #endregion

        #region overrides

        public override bool SupportsDelayedLoad
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is tv.
        /// </summary>
        /// <value><c>true</c> if this instance is tv; otherwise, <c>false</c>.</value>
        public override bool IsTv
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the dialog was canceled. 
        /// </summary>
        /// <value><c>true</c> if dialog was canceled without a selection</value>
        public bool Canceled
        {
            get { return _canceled; }
        }

        /// <summary>
        /// Gets or sets the selected channel.
        /// </summary>
        /// <value>The selected channel.</value>
        public Channel SelectedChannel
        {
            get { return _selectedChannel; }
        }

        public ChannelGroup SelectedGroup
        {
            get { return _currentGroup; }
        }

        public ChannelType ChannelType { set; get; }

        /// <summary>
        /// Init method
        /// </summary>
        /// <returns></returns>
        public override bool Init()
        {
            bool bResult = Load(GUIGraphicsContext.Skin + @"\TVMiniGuide.xml");

            GetID = (int)GUIWindow.Window.WINDOW_MINI_GUIDE;
            _canceled = true;
            LoadSettings();
            return bResult;
        }

        /// <summary>
        /// On Message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    {
                        if (message.SenderControlId == 35 || message.SenderControlId == 37) // listbox
                        {
                            if ((int)Action.ActionType.ACTION_SELECT_ITEM == message.Param1)
                            {
                                // switching logic
                                _selectedChannel = (Channel)_channelsListControl.SelectedListItem.TVTag;

                                _canceled = false;
                                PageDestroy();
                            }
                        }
                        else if (message.SenderControlId == 36) // spincontrol
                        {
                            OnGroupChanged();
                        }
                        else if (message.SenderControlId == 34) // exit button
                        {
                            _canceled = true;
                            PageDestroy();
                        }
                        break;
                    }
            }
            return base.OnMessage(message);
        }

        /// <summary>
        /// On action
        /// </summary>
        /// <param name="action"></param>
        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_CONTEXT_MENU:
                    PageDestroy();
                    return;
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    _canceled = true;
                    PageDestroy();
                    return;
                case Action.ActionType.ACTION_MOVE_LEFT:
                case Action.ActionType.ACTION_TVGUIDE_PREV_GROUP:
                    _spinGroup.MoveUp();
                    return;
                case Action.ActionType.ACTION_MOVE_RIGHT:
                case Action.ActionType.ACTION_TVGUIDE_NEXT_GROUP:
                    _spinGroup.MoveDown();
                    return;
            }
            base.OnAction(action);
        }

        /// <summary>
        /// Page gets destroyed
        /// </summary>
        /// <param name="new_windowId"></param>
        protected override void OnPageDestroy(int new_windowId)
        {
            base.OnPageDestroy(new_windowId);
        }

        /// <summary>
        /// Page gets loaded
        /// </summary>
        protected override void OnPageLoad()
        {
            Log.Debug("miniguide: onpageload");
            AllocResources();
            ResetAllControls();
            InitializeChannelListControl();
            _spinGroup.CycleItems = true;
            FillGroupList();
            FillChannelList();
            base.OnPageLoad();
        }

        #endregion

        #region private methods

        private void InitializeChannelListControl()
        {
            if (_channelsListControlWithStateIcons != null
                && TvHome.ShowChannelStateIcons)
            {
                _channelsListControl = _channelsListControlWithStateIcons;
                _channelsListControlNoStateIcons.Visible = false;
                _showStateIcons = true;
            }
            else
            {
                _channelsListControl = _channelsListControlNoStateIcons;
                if (_channelsListControlWithStateIcons != null)
                {
                    _channelsListControlWithStateIcons.Visible = false;
                }
                _showStateIcons = false;
            }
            _channelsListControl.Visible = true;
        }

        private void OnGroupChanged()
        {
            if (_spinGroup.Visible)
            {
                GUIWaitCursor.Show();
                _currentGroup = _channelGroupList[_spinGroup.Value];
                string guiPropertyPrefix = this.ChannelType == ChannelType.Television ? "#TV" : "#Radio";
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Guide.Group", _spinGroup.GetLabel());
                FillChannelList();
                GUIWaitCursor.Hide();
            }
        }

        /// <summary>
        /// Fill up the list with groups
        /// </summary>
        private void FillGroupList()
        {
            PluginMain.Navigator.RefreshChannelGroups(this.ChannelType);
            _channelGroupList = PluginMain.Navigator.GetGroups(this.ChannelType);
            _currentGroup = PluginMain.Navigator.CurrentGroup;

            // empty list of groups currently in the spin control
            _spinGroup.Reset();

            // start to fill them up again
            bool groupFound = false;
            for (int index = 0; index < _channelGroupList.Count; index++)
            {
                ChannelGroup group = _channelGroupList[index];
                _spinGroup.AddLabel(group.GroupName, index);
                // set selected
                if (_currentGroup != null
                    && group.ChannelGroupId == _currentGroup.ChannelGroupId)
                {
                    _spinGroup.Value = index;
                    groupFound = true;
                }
            }

            if (!groupFound)
            {
                _currentGroup = (_channelGroupList.Count > 0) ? _channelGroupList[0] : null;
            }

            if (_channelGroupList.Count < 2)
            {
                _spinGroup.Visible = false;
            }
        }

        /// <summary>
        /// Fill the list with channels
        /// </summary>
        private void FillChannelList()
        {
            _channelsListControl.Clear();

            int i = 0;
            int SelectedID = 0;
            string ChannelLogo = string.Empty;

            if (_currentGroup != null)
            {
                _currentAndNextPrograms = Proxies.SchedulerService.GetCurrentAndNextForGroup(_currentGroup.ChannelGroupId, true, PluginMain.Navigator.LiveStream);
            }
            else
            {
                _currentAndNextPrograms = new List<CurrentAndNextProgram>();
            }

            Channel currentChannel = PluginMain.Navigator.CurrentChannel;
            Channel prevChannel = PluginMain.Navigator.GetPreviousChannel(this.ChannelType);

            foreach (CurrentAndNextProgram currentAndNext in _currentAndNextPrograms)
            {
                i++;
                sb.Length = 0;
                GUIListItem item = new GUIListItem("");
                item.TVTag = currentAndNext.Channel;

                sb.Append(currentAndNext.Channel.DisplayName);
                ChannelLogo = Utility.GetLogoImage(currentAndNext.Channel);

                if (!string.IsNullOrEmpty(ChannelLogo))
                {
                    item.IconImageBig = ChannelLogo;
                    item.IconImage = ChannelLogo;
                }
                else
                {
                    item.IconImageBig = string.Empty;
                    item.IconImage = string.Empty;
                }

                ActiveRecording activeRecording;
                if (PluginMain.IsChannelRecording(currentAndNext.Channel.ChannelId, out activeRecording))
                {
                    if (_showStateIcons)
                    {
                        item.PinImage = RecordingIcon;
                    }
                    else
                    {
                        sb.Append(" ");
                        sb.Append(recordingText);
                    }
                    item.IsPlayed = (currentAndNext.LiveState == ChannelLiveState.NotTunable
                        || currentAndNext.LiveState == ChannelLiveState.NoFreeCard);
                }
                else
                {
                    switch (currentAndNext.LiveState)
                    {
                        case ChannelLiveState.NotTunable:
                        case ChannelLiveState.NoFreeCard:
                            item.IsPlayed = true;
                            if (_showStateIcons)
                            {
                                item.PinImage = UnavailableIcon;
                            }
                            else
                            {
                                sb.Append(" ");
                                sb.Append(unavailableText);
                            }
                            break;

                        default:
                            item.IsPlayed = false;
                            if (_showStateIcons)
                            {
                                item.PinImage = AvailableIcon;
                            }
                            break;
                    }
                }

                if (currentChannel != null)
                {
                    if (currentChannel.ChannelId == currentAndNext.Channel.ChannelId)
                    {
                        item.IsRemote = true;
                        SelectedID = _channelsListControl.Count;

                        if (_showStateIcons && item.PinImage != RecordingIcon
                            && item.PinImage != UnavailableIcon)
                        {
                            item.PinImage = TimeshiftingIcon;
                        }
                        else if (!_showStateIcons)
                        {
                            sb.Append(" ");
                            sb.Append(timeshiftingText);
                        }
                    }
                }
                else if (prevChannel != null
                    && prevChannel.ChannelId == currentAndNext.Channel.ChannelId)
                {
                    item.IsRemote = true;
                    SelectedID = _channelsListControl.Count;
                }

                sbTmp.Length = 0;

                bool hasNow = currentAndNext.Current != null;
                if (hasNow)
                {
                    sbTmp.Append(currentAndNext.Current.CreateProgramTitle());
                }
                else
                {
                    sbTmp.Append(noDataAvailableText);
                }

                item.Label2 = sbTmp.ToString();
                sbTmp.Insert(0, nowText);
                item.Label3 = sbTmp.ToString();

                sbTmp.Length = 0;

                if (_showChannelNumber)
                {
                    sb.Append(" - ");
                    if (!_byIndex)
                    {
                        if (currentAndNext.Channel.LogicalChannelNumber.HasValue)
                        {
                            sb.Append(currentAndNext.Channel.LogicalChannelNumber.Value.ToString());
                        }
                    }
                    else
                    {
                        sb.Append(i);
                    }
                }

                if (hasNow)
                {
                    sb.Append(" - ");
                    sb.Append(currentAndNext.CurrentPercentageComplete);
                    sb.Append("%");
                }

                if (currentAndNext.Next != null)
                {
                    sbTmp.Append(/*currentAndNext.Next.StartTime.ToShortTimeString() + " " + */currentAndNext.Next.CreateProgramTitle());
                }
                else
                {
                    sbTmp.Append(noDataAvailableText);
                }

                item.Label2 = sb.ToString();
                sbTmp.Insert(0, nextText);
                item.Label = sbTmp.ToString();

                _channelsListControl.Add(item);
            }
            _channelsListControl.SelectedListItemIndex = SelectedID;
            Log.Debug("miniguide: state check + filling channel list completed");

            if (_channelsListControl.GetID == 37)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SETFOCUS, GetID, 0, 37, 0, 0, null);
                OnMessage(msg);
            }

            sb.Length = 0;
            sbTmp.Length = 0;
        }

        #endregion
    }
}
