#region Copyright (C) 2005-2012 Team MediaPortal

/* 
 *	Copyright (C) 2005-2012 Team MediaPortal
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
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Windows.Media.Animation;

using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortal.Dialogs;
using MediaPortal.Player;
using MediaPortal.Configuration;
using MediaPortal.Video.Database;
using Action = MediaPortal.GUI.Library.Action;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;
using ArgusTV.ServiceContracts;
using ArgusTV.UI.Process.Guide;
#endregion

namespace ArgusTV.UI.MediaPortal
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class GuideBase : GUIWindow, IMDB.IProgress
    {
        protected abstract string SettingsSection { get; }

        public static GUIControl.Alignment TimeAlignment { get; set; }
        public static bool ReloadSchedules { get; set; }

        #region constants

        private class GuideBaseChannel
        {
            public Channel channel;
            public int channelNum;
            public string strLogo;
        }

        private const int MaxDaysInGuide = 30;
        private const int RowID = 1000;
        private const int ColID = 10;

        private const int GUIDE_COMPONENTID_START = 50000;
        // Start for numbering IDs of automaticaly generated TVguide components for channels and programs

        private int _loopDelay = 100; // wait at the last item this amount of msec until loop to the first item

        private string SkinPropertyPrefix = "#TV";

        #endregion

        #region enums

        private enum Controls
        {
            PANEL_BACKGROUND = 2,
            SPINCONTROL_DAY = 6,
            SPINCONTROL_TIME_INTERVAL = 8,
            CHANNEL_IMAGE_TEMPLATE = 7,
            CHANNEL_LABEL_TEMPLATE = 18,
            LABEL_GENRE_TEMPLATE = 23,
            LABEL_TITLE_TEMPLATE = 24,
            VERTICAL_LINE = 25,
            LABEL_TITLE_DARK_TEMPLATE = 26,
            LABEL_GENRE_DARK_TEMPLATE = 30,
            CHANNEL_TEMPLATE = 20, // Channel rectangle and row height

            HORZ_SCROLLBAR = 28,
            VERT_SCROLLBAR = 29,
            LABEL_TIME1 = 40, // first and template
            IMG_CHAN1 = 150,
            IMG_CHAN1_LABEL = 170,
            IMG_TIME1 = 90, // first and template
            IMG_REC_PIN = 31,
            SINGLE_CHANNEL_LABEL = 32,
            SINGLE_CHANNEL_IMAGE = 33,
            LABEL_KEYED_CHANNEL = 34,
            BUTTON_PROGRAM_RUNNING = 35,
            BUTTON_PROGRAM_NOT_RUNNING = 36,
            BUTTON_PROGRAM_NOTIFY = 37,
            BUTTON_PROGRAM_RECORD = 38,
            BUTTON_PROGRAM_PARTIAL_RECORD = 39,

            CHANNEL_GROUP_BUTTON = 100
        } ;

        #endregion

        #region variables

        private ChannelType _channelType;
        private GuideModel _model;
        private GuideController _controller;

        //private Channel _recordingExpected = null;
        private DateTime _updateTimerRecExpected = DateTime.Now;
        private DateTime _viewingTime = DateTime.Now;
        private int _channelOffset = 0;
        private List<GuideBaseChannel> _channelList = new List<GuideBaseChannel>();
        private IList<Schedule> _recordingList = new List<Schedule>();
        //private Dictionary<int, GUIButton3PartControl> _controls = new Dictionary<int, GUIButton3PartControl>();

        private int _timePerBlock = 30; // steps of 30 minutes
        private int _channelCount = 5;
        private int _numberOfBlocks = 4;
        private int _cursorY = 0;
        private int _cursorX = 0;
        private string _currentTitle = String.Empty;
        private string _currentTime = String.Empty;
        private Channel _currentChannel = null;
        private bool _currentRecOrNotify = false;
        private long _currentStartTime = 0;
        private long _currentEndTime = 0;
        private GuideProgram _currentProgram = null;
        private bool _needUpdate = false;
        private DateTime m_dtStartTime = DateTime.Now;
        private ArrayList _colorList = new ArrayList();
        private bool _singleChannelView = false;
        private int _programOffset = 0;
        private int _totalProgramCount = 0;
        private int _singleChannelNumber = 0;
        private bool _showChannelLogos = false;
        //private TvServer _server = null;
        //private IList<GuideProgram> _programs = null;

        private int _backupCursorX = 0;
        private int _backupCursorY = 0;
        private int _backupChannelOffset = 0;
        private DateTime _updateTimer = DateTime.Now;

        private DateTime _keyPressedTimer = DateTime.Now;
        private string _lineInput = String.Empty;

        private bool _byIndex = false;
        private bool _showChannelNumber = false;
        private int _channelNumberMaxLength = 3;
        private bool _useNewRecordingButtonColor = false;
        private bool _useNewPartialRecordingButtonColor = false;
        private bool _useNewNotifyButtonColor = false;
        private bool _recalculateProgramOffset;
        private bool _useHdProgramIcon = false;
        private string _hdtvProgramText = String.Empty;
        private bool _guideContinuousScroll = false;

        private GUILabelControl _titleDarkTemplate;
        private GUILabelControl _titleTemplate;
        private GUILabelControl _genreDarkTemplate;
        private GUILabelControl _genreTemplate;

        private GUIButton3PartControl _programPartialRecordTemplate;
        private GUIButton3PartControl _programRecordTemplate;
        private GUIButton3PartControl _programNotifyTemplate;
        private GUIButton3PartControl _programNotRunningTemplate;
        private GUIButton3PartControl _programRunningTemplate;

        // current minimum/maximum indexes
        //private int MaxXIndex; // means rows here (channels)
        private int MinYIndex; // means cols here (programs/time)

        protected double _lastCommandTime = 0;

        /// <summary>
        /// Logic to decide if channel group button is available and visible
        /// </summary>
        protected bool GroupButtonAvail
        {
            get
            {
                // show/hide channel group button
                GUIButtonControl btnChannelGroup = GetControl((int)Controls.CHANNEL_GROUP_BUTTON) as GUIButtonControl;

                // visible only if more than one group? and not in single channel, and button exists in skin!
                return (PluginMain.Navigator.GetGroups(_channelType).Count > 1 && !_singleChannelView && btnChannelGroup != null);
            }
        }

        #endregion

        #region ctor

        public GuideBase(ChannelType channelType)
        {
            _channelType = channelType;
            this.SkinPropertyPrefix = _channelType == ChannelType.Radio ? "#Radio" : "#TV";
            _colorList.Add(Color.Red);
            _colorList.Add(Color.Green);
            _colorList.Add(Color.Blue);
            _colorList.Add(Color.Cyan);
            _colorList.Add(Color.Magenta);
            _colorList.Add(Color.DarkBlue);
            _colorList.Add(Color.Brown);
            _colorList.Add(Color.Fuchsia);
            _colorList.Add(Color.Khaki);
            _colorList.Add(Color.SteelBlue);
            _colorList.Add(Color.SaddleBrown);
            _colorList.Add(Color.Chocolate);
            _colorList.Add(Color.DarkMagenta);
            _colorList.Add(Color.DarkSeaGreen);
            _colorList.Add(Color.Coral);
            _colorList.Add(Color.DarkGray);
            _colorList.Add(Color.DarkOliveGreen);
            _colorList.Add(Color.DarkOrange);
            _colorList.Add(Color.ForestGreen);
            _colorList.Add(Color.Honeydew);
            _colorList.Add(Color.Gray);
            _colorList.Add(Color.Tan);
            _colorList.Add(Color.Silver);
            _colorList.Add(Color.SeaShell);
            _colorList.Add(Color.RosyBrown);
            _colorList.Add(Color.Peru);
            _colorList.Add(Color.OldLace);
            _colorList.Add(Color.PowderBlue);
            _colorList.Add(Color.SpringGreen);
            _colorList.Add(Color.LightSalmon);
        }

        #endregion

        #region Serialisation

        private void LoadSettings()
        {
            string currentChannelName;
            LoadSettings(out currentChannelName);
        }

        private void LoadSettings(out string currentChannelName)
        {
            using (global::MediaPortal.Profile.Settings xmlreader = new global::MediaPortal.Profile.MPSettings())
            {
                _cursorX = xmlreader.GetValueAsInt(this.SettingsSection, "ypos", 0);
                _channelOffset = xmlreader.GetValueAsInt(this.SettingsSection, "yoffset", 0);
                _byIndex = xmlreader.GetValueAsBool("mytv", "byindex", true);
                _showChannelNumber = xmlreader.GetValueAsBool("mytv", "showchannelnumber", false);
                _channelNumberMaxLength = xmlreader.GetValueAsInt("mytv", "channelnumbermaxlength", 3);
                _timePerBlock = xmlreader.GetValueAsInt(this.SettingsSection, "timeperblock", 30);
                _hdtvProgramText = xmlreader.GetValueAsString("mytv", "hdtvProgramText", "(HDTV)");
                _guideContinuousScroll = xmlreader.GetValueAsBool("mytv", "continuousScrollGuide", false);
                _loopDelay = xmlreader.GetValueAsInt("gui", "listLoopDelay", 0);
                currentChannelName = xmlreader.GetValueAsString(this.SettingsSection, "channel", String.Empty);

                _currentChannel = PluginMain.Navigator.CurrentChannel ?? PluginMain.Navigator.GetPreviousChannel(_channelType);
                if (_currentChannel != null)
                {
                    currentChannelName = _currentChannel.DisplayName;
                }

                if (_channelOffset < 0)
                {
                    _channelOffset = 0;
                }
            }
            _useNewRecordingButtonColor =
              Utils.FileExistsInCache(Path.Combine(GUIGraphicsContext.Skin, @"media\tvguide_recButton_Focus_middle.png"));
            _useNewPartialRecordingButtonColor =
              Utils.FileExistsInCache(Path.Combine(GUIGraphicsContext.Skin, @"media\tvguide_partRecButton_Focus_middle.png"));
            _useNewNotifyButtonColor =
              Utils.FileExistsInCache(Path.Combine(GUIGraphicsContext.Skin, @"media\tvguide_notifyButton_Focus_middle.png"));
            _useHdProgramIcon =
              Utils.FileExistsInCache(Path.Combine(GUIGraphicsContext.Skin, @"media\tvguide_hd_program.png"));
        }

        private void SaveSettings()
        {
            using (global::MediaPortal.Profile.Settings xmlwriter = new global::MediaPortal.Profile.MPSettings())
            {
                xmlwriter.SetValue(this.SettingsSection, "channel", _currentChannel == null ? String.Empty : _currentChannel.DisplayName);
                xmlwriter.SetValue(this.SettingsSection, "ypos", _cursorX.ToString());
                xmlwriter.SetValue(this.SettingsSection, "yoffset", _channelOffset.ToString());
                xmlwriter.SetValue(this.SettingsSection, "timeperblock", _timePerBlock);
            }
        }

        #endregion

        #region overrides

        protected override void OnPageLoad()
        {
            if (PluginMain.EnsureConnection()
                && _model == null)
            {
                _model = new GuideModel();
                _controller = new GuideController(_model);
                using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                {
                    _controller.Initialize(tvSchedulerAgent, _channelType, 3, Utility.GetLocalizedText(TextId.AllChannels));
                }
            }

            if (PluginMain.Navigator.CurrentGroup != null
                && PluginMain.Navigator.CurrentGroup.ChannelType == _channelType)
            {
                _controller.SetChannelGroup(PluginMain.Navigator.CurrentGroup.ChannelGroupId);
            }

            base.OnPageLoad();
        }

        public override int GetFocusControlId()
        {
            int focusedId = base.GetFocusControlId();
            if (_cursorX >= 0 ||
                focusedId == (int)Controls.SPINCONTROL_DAY ||
                focusedId == (int)Controls.SPINCONTROL_TIME_INTERVAL ||
                focusedId == (int)Controls.CHANNEL_GROUP_BUTTON
               )
            {
                return focusedId;
            }
            else
            {
                return -1;
            }
        }

        protected void Initialize()
        {
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                    if (_singleChannelView)
                    {
                        OnSwitchMode();
                        return; // base.OnAction would close the EPG as well
                    }
                    else
                    {
                        GUIWindowManager.ShowPreviousWindow();
                        return;
                    }

                case Action.ActionType.ACTION_KEY_PRESSED:
                    if (action.m_key != null)
                    {
                        OnKeyCode((char)action.m_key.KeyChar);
                    }
                    break;

                case Action.ActionType.ACTION_RECORD:
                    if ((GetFocusControlId() != -1) && (_cursorY > 0) && (_cursorX >= 0))
                    {
                        OnRecordOrAlert(ScheduleType.Recording);
                    }
                    break;

                case Action.ActionType.ACTION_MOUSE_MOVE:
                    {
                        int x = (int)action.fAmount1;
                        int y = (int)action.fAmount2;
                        foreach (GUIControl control in controlList)
                        {
                            if (control.GetID >= (int)Controls.IMG_CHAN1 + 0 &&
                                control.GetID <= (int)Controls.IMG_CHAN1 + _channelCount)
                            {
                                if (x >= control.XPosition && x < control.XPosition + control.Width)
                                {
                                    if (y >= control.YPosition && y < control.YPosition + control.Height)
                                    {
                                        UnFocus();
                                        _cursorX = control.GetID - (int)Controls.IMG_CHAN1;
                                        _cursorY = 0;

                                        if (_singleChannelNumber != _cursorX + _channelOffset)
                                        {
                                            Update(false);
                                        }
                                        UpdateCurrentProgram(false);
                                        UpdateHorizontalScrollbar();
                                        UpdateVerticalScrollbar();
                                        updateSingleChannelNumber();
                                        return;
                                    }
                                }
                            }
                            if (control.GetID >= GUIDE_COMPONENTID_START)
                            {
                                if (x >= control.XPosition && x < control.XPosition + control.Width)
                                {
                                    if (y >= control.YPosition && y < control.YPosition + control.Height)
                                    {
                                        int iControlId = control.GetID;
                                        if (iControlId >= GUIDE_COMPONENTID_START)
                                        {
                                            iControlId -= GUIDE_COMPONENTID_START;
                                            int iCursorY = (iControlId / RowID);
                                            iControlId -= iCursorY * RowID;
                                            if (iControlId % ColID == 0)
                                            {
                                                int iCursorX = (iControlId / ColID) + 1;
                                                if (iCursorY != _cursorX || iCursorX != _cursorY)
                                                {
                                                    UnFocus();
                                                    _cursorX = iCursorY;
                                                    _cursorY = iCursorX;
                                                    UpdateCurrentProgram(false);
                                                    SetFocus();
                                                    UpdateHorizontalScrollbar();
                                                    UpdateVerticalScrollbar();
                                                    updateSingleChannelNumber();
                                                    return;
                                                }
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        UnFocus();
                        _cursorY = -1;
                        _cursorX = -1;
                        base.OnAction(action);
                    }
                    break;

                case Action.ActionType.ACTION_TVGUIDE_RESET:
                    _cursorY = 0;
                    _viewingTime = DateTime.Now;
                    Update(false);
                    break;


                case Action.ActionType.ACTION_CONTEXT_MENU:
                    {
                        if (_cursorY >= 0 && _cursorX >= 0)
                        {
                            if (_cursorY == 0)
                            {
                                OnSwitchMode();
                                return;
                            }
                            else
                            {
                                ShowContextMenu();
                            }
                        }
                        else
                        {
                            action.wID = Action.ActionType.ACTION_SELECT_ITEM;
                            GUIWindowManager.OnAction(action);
                        }
                    }
                    break;

                case Action.ActionType.ACTION_PAGE_UP:
                    OnPageUp();
                    updateSingleChannelNumber();
                    break;

                case Action.ActionType.ACTION_PAGE_DOWN:
                    OnPageDown();
                    updateSingleChannelNumber();
                    break;

                case Action.ActionType.ACTION_MOVE_LEFT:
                    {
                        if (_cursorX >= 0)
                        {
                            OnLeft();
                            updateSingleChannelNumber();
                            UpdateHorizontalScrollbar();
                            return;
                        }
                    }
                    break;
                case Action.ActionType.ACTION_MOVE_RIGHT:
                    {
                        if (_cursorX >= 0)
                        {
                            OnRight();
                            UpdateHorizontalScrollbar();
                            return;
                        }
                    }
                    break;
                case Action.ActionType.ACTION_MOVE_UP:
                    {
                        if (_cursorX >= 0)
                        {
                            OnUp(true, false);
                            updateSingleChannelNumber();
                            UpdateVerticalScrollbar();
                            return;
                        }
                    }
                    break;
                case Action.ActionType.ACTION_MOVE_DOWN:
                    {
                        if (_cursorX >= 0)
                        {
                            OnDown(true);
                            updateSingleChannelNumber();
                            UpdateVerticalScrollbar();
                        }
                        else
                        {
                            _cursorX = 0;
                            SetFocus();
                            updateSingleChannelNumber();
                            UpdateVerticalScrollbar();
                        }
                        return;
                    }
                //break;
                case Action.ActionType.ACTION_SHOW_INFO:
                    {
                        ShowContextMenu();
                    }
                    break;
                case Action.ActionType.ACTION_INCREASE_TIMEBLOCK:
                    {
                        _timePerBlock += 15;
                        if (_timePerBlock > 60)
                        {
                            _timePerBlock = 60;
                        }
                        GUISpinControl cntlTimeInterval = GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL) as GUISpinControl;
                        cntlTimeInterval.Value = (_timePerBlock / 15) - 1;
                        Update(false);
                        SetFocus();
                    }
                    break;

                case Action.ActionType.ACTION_REWIND:
                case Action.ActionType.ACTION_MUSIC_REWIND:
                    _viewingTime = _viewingTime.AddHours(-3);
                    Update(false);
                    SetFocus();
                    break;

                case Action.ActionType.ACTION_FORWARD:
                case Action.ActionType.ACTION_MUSIC_FORWARD:
                    _viewingTime = _viewingTime.AddHours(3);
                    Update(false);
                    SetFocus();
                    break;

                case Action.ActionType.ACTION_DECREASE_TIMEBLOCK:
                    {
                        if (_timePerBlock > 15)
                        {
                            _timePerBlock -= 15;
                        }
                        GUISpinControl cntlTimeInterval = GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL) as GUISpinControl;
                        cntlTimeInterval.Value = (_timePerBlock / 15) - 1;
                        Update(false);
                        SetFocus();
                    }
                    break;
                case Action.ActionType.ACTION_DEFAULT_TIMEBLOCK:
                    {
                        _timePerBlock = 30;
                        GUISpinControl cntlTimeInterval = GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL) as GUISpinControl;
                        cntlTimeInterval.Value = (_timePerBlock / 15) - 1;
                        Update(false);
                        SetFocus();
                    }
                    break;
                case Action.ActionType.ACTION_TVGUIDE_INCREASE_DAY:
                    OnNextDay();
                    break;

                case Action.ActionType.ACTION_TVGUIDE_DECREASE_DAY:
                    OnPreviousDay();
                    break;

                // TV group changing actions
                case Action.ActionType.ACTION_TVGUIDE_NEXT_GROUP:
                    OnChangeChannelGroup(1);
                    break;

                case Action.ActionType.ACTION_TVGUIDE_PREV_GROUP:
                    OnChangeChannelGroup(-1);
                    break;
            }
            base.OnAction(action);
        }

        /// <summary>
        /// changes the current tv group and refreshes guide display
        /// </summary>
        /// <param name="Direction"></param>
        protected virtual void OnChangeChannelGroup(int Direction)
        {
#if false
            // in single channel view there would be errors when changing group
            if (_singleChannelView) return;
            int newIndex, oldIndex;
            int countGroups = PluginMain.Navigator.GetGroups(_channelType).Count; // all

            newIndex = oldIndex = PluginMain.Navigator.CurrentGroupIndex;
            if (
               (newIndex >= 1 && Direction < 0) ||
               (newIndex < countGroups - 1 && Direction > 0)
              )
            {
                newIndex += Direction; // change group
            }
            else // Cycle handling
                if ((newIndex == countGroups - 1) && Direction > 0)
                {
                    newIndex = 0;
                }
                else
                    if (newIndex == 0 && Direction < 0)
                    {
                        newIndex = countGroups - 1;
                    }

            if (oldIndex != newIndex)
            {
                // update list
                GUIWaitCursor.Show();
                TVHome.Navigator.SetCurrentGroup(newIndex);
                // set name only, if group button not avail (avoids short "flashing" of text after switching group)
                if (!TvGroupButtonAvail)
                {
                    string guiPropertyPrefix = _channelType == ChannelType.Television ? "#TV" : "#Radio";
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Guide.Group", PluginMain.Navigator.CurrentGroup.GroupName);
                }
                _channelOffset = 0; // reset to top; otherwise focus could be out of screen if new group has less then old position
                _cursorX = 0; // first channel
                GetChannels(true);
                Update(false);
                SetFocus();
                GUIWaitCursor.Hide();
            }
#endif
        }

        private void UpdateOverlayAllowed()
        {
            if (_isOverlayAllowedCondition == 0)
            {
                return;
            }
            bool bWasAllowed = GUIGraphicsContext.Overlay;
            _isOverlayAllowed = GUIInfoManager.GetBool(_isOverlayAllowedCondition, GetID);
            if (bWasAllowed != _isOverlayAllowed)
            {
                GUIGraphicsContext.Overlay = _isOverlayAllowed;
            }
        }

        public override bool OnMessage(GUIMessage message)
        {
            try
            {
                switch (message.Message)
                {
                    case GUIMessage.MessageType.GUI_MSG_PERCENTAGE_CHANGED:
                        if (message.SenderControlId == (int)Controls.HORZ_SCROLLBAR)
                        {
                            _needUpdate = true;
                            float fPercentage = (float)message.Param1;
                            fPercentage /= 100.0f;
                            fPercentage *= 24.0f;
                            fPercentage *= 60.0f;
                            _viewingTime = new DateTime(_viewingTime.Year, _viewingTime.Month, _viewingTime.Day, 0, 0, 0, 0);
                            _viewingTime = _viewingTime.AddMinutes((int)fPercentage);
                        }

                        if (message.SenderControlId == (int)Controls.VERT_SCROLLBAR)
                        {
                            _needUpdate = true;
                            float fPercentage = (float)message.Param1;
                            fPercentage /= 100.0f;
                            if (_singleChannelView)
                            {
                                fPercentage *= (float)_totalProgramCount;
                                int iChan = (int)fPercentage;
                                _channelOffset = 0;
                                _cursorX = 0;
                                while (iChan >= _channelCount)
                                {
                                    iChan -= _channelCount;
                                    _channelOffset += _channelCount;
                                }
                                _cursorX = iChan;
                            }
                            else
                            {
                                fPercentage *= (float)_channelList.Count;
                                int iChan = (int)fPercentage;
                                _channelOffset = 0;
                                _cursorX = 0;
                                while (iChan >= _channelCount)
                                {
                                    iChan -= _channelCount;
                                    _channelOffset += _channelCount;
                                }
                                _cursorX = iChan;
                            }
                        }
                        break;

                    case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
                        {
                            base.OnMessage(message);
                            SaveSettings();
                            //_recordingList.Clear();

                            //_controls = new Dictionary<int, GUIButton3PartControl>();
                            _channelList = null;
                            _recordingList = null;
                            _currentProgram = null;

                            return true;
                        }

                    case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                        {
                            GUIPropertyManager.SetProperty("#itemcount", string.Empty);
                            GUIPropertyManager.SetProperty("#selecteditem", string.Empty);
                            GUIPropertyManager.SetProperty("#selecteditem2", string.Empty);
                            GUIPropertyManager.SetProperty("#selectedthumb", string.Empty);

                            if (_shouldRestore)
                            {
                                DoRestoreSkin();
                            }
                            else
                            {
                                LoadSkin();
                                AllocResources();
                            }

                            InitControls();

                            base.OnMessage(message);

                            string currentChannelName;
                            LoadSettings(out currentChannelName);

                            UpdateOverlayAllowed();
                            GUIGraphicsContext.Overlay = _isOverlayAllowed;

                            // set topbar autohide
                            switch (_autoHideTopbarType)
                            {
                                case AutoHideTopBar.No:
                                    _autoHideTopbar = false;
                                    break;
                                case AutoHideTopBar.Yes:
                                    _autoHideTopbar = true;
                                    break;
                                default:
                                    _autoHideTopbar = GUIGraphicsContext.DefaultTopBarHide;
                                    break;
                            }
                            GUIGraphicsContext.AutoHideTopBar = _autoHideTopbar;
                            GUIGraphicsContext.TopBarHidden = _autoHideTopbar;
                            GUIGraphicsContext.DisableTopBar = _disableTopBar;
                            LoadSettings();
                            GUIControl cntlPanel = GetControl((int)Controls.PANEL_BACKGROUND);
                            GUIImage cntlChannelTemplate = (GUIImage)GetControl((int)Controls.CHANNEL_TEMPLATE);

                            int iHeight = cntlPanel.Height + cntlPanel.YPosition - cntlChannelTemplate.YPosition;
                            int iItemHeight = cntlChannelTemplate.Height;
                            _channelCount = (int)(((float)iHeight) / ((float)iItemHeight));

                            bool isPreviousWindowTvGuideRelated = (message.Param1 == (int)Window.WINDOW_TV_PROGRAM_INFO ||
                                                     message.Param1 == (int)Window.WINDOW_VIDEO_INFO);

                            if (!isPreviousWindowTvGuideRelated)
                            {
                                UnFocus();
                            }

                            GetChannels(true);
                            LoadSchedules(true);
                            _currentProgram = null;
                            if (!isPreviousWindowTvGuideRelated)
                            {
                                _viewingTime = DateTime.Now;
                                _cursorY = 0;
                                _cursorX = 0;
                                _channelOffset = 0;
                                _singleChannelView = false;
                                _showChannelLogos = false;

                                _currentChannel = null;
                                int index = 0;
                                foreach (GuideBaseChannel channel in _channelList)
                                {
                                    if (channel.channel.DisplayName == currentChannelName)
                                    {
                                        _currentChannel = channel.channel;
                                        _cursorX = index;
                                        break;
                                    }
                                    index++;
                                }
                            }

                            while (_cursorX >= _channelCount)
                            {
                                _cursorX -= _channelCount;
                                _channelOffset += _channelCount;
                            }

                            // Mantis 3579: the above lines can lead to too large channeloffset. 
                            // Now we check if the offset is too large, and if it is, we reduce it and increase the cursor position accordingly
                            if (!_guideContinuousScroll && (_channelOffset > _channelList.Count - _channelCount) && (_channelList.Count - _channelCount > 0))
                            {
                                _cursorX += _channelOffset - (_channelList.Count - _channelCount);
                                _channelOffset = _channelList.Count - _channelCount;
                            }

                            GUISpinControl cntlDay = GetControl((int)Controls.SPINCONTROL_DAY) as GUISpinControl;
                            if (cntlDay != null)
                            {
                                DateTime dtNow = DateTime.Now;
                                cntlDay.Reset();
                                cntlDay.SetRange(0, MaxDaysInGuide - 1);
                                for (int iDay = 0; iDay < MaxDaysInGuide; iDay++)
                                {
                                    DateTime dtTemp = dtNow.AddDays(iDay);
                                    string day;
                                    switch (dtTemp.DayOfWeek)
                                    {
                                        case DayOfWeek.Monday:
                                            day = GUILocalizeStrings.Get(657);
                                            break;
                                        case DayOfWeek.Tuesday:
                                            day = GUILocalizeStrings.Get(658);
                                            break;
                                        case DayOfWeek.Wednesday:
                                            day = GUILocalizeStrings.Get(659);
                                            break;
                                        case DayOfWeek.Thursday:
                                            day = GUILocalizeStrings.Get(660);
                                            break;
                                        case DayOfWeek.Friday:
                                            day = GUILocalizeStrings.Get(661);
                                            break;
                                        case DayOfWeek.Saturday:
                                            day = GUILocalizeStrings.Get(662);
                                            break;
                                        default:
                                            day = GUILocalizeStrings.Get(663);
                                            break;
                                    }
                                    day = GetLocalDate(day, dtTemp.Day, dtTemp.Month);
                                    cntlDay.AddLabel(day, iDay);
                                }
                            }
                            else
                            {
                                Log.Debug("TvGuideBase: SpinControl cntlDay is null!");
                            }

                            GUISpinControl cntlTimeInterval = GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL) as GUISpinControl;
                            if (cntlTimeInterval != null)
                            {
                                cntlTimeInterval.Reset();
                                for (int i = 1; i <= 4; i++)
                                {
                                    cntlTimeInterval.AddLabel(String.Empty, i);
                                }
                                cntlTimeInterval.Value = (_timePerBlock / 15) - 1;
                            }
                            else
                            {
                                Log.Debug("TvGuideBase: SpinControl cntlTimeInterval is null!");
                            }

                            if (!isPreviousWindowTvGuideRelated)
                            {
                                Update(true);
                            }
                            else
                            {
                                Update(false);
                            }

                            SetFocus();

                            if (_currentProgram != null)
                            {
                                m_dtStartTime = _currentProgram.StartTime;
                            }
                            UpdateCurrentProgram(false);

                            return true;
                        }
                    //break;

                    case GUIMessage.MessageType.GUI_MSG_CLICKED:
                        int iControl = message.SenderControlId;
                        if (iControl == (int)Controls.SPINCONTROL_DAY)
                        {
                            GUISpinControl cntlDay = GetControl((int)Controls.SPINCONTROL_DAY) as GUISpinControl;
                            int iDay = cntlDay.Value;

                            _viewingTime = DateTime.Now;
                            _viewingTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _viewingTime.Hour,
                                                        _viewingTime.Minute, 0, 0);
                            _viewingTime = _viewingTime.AddDays(iDay);
                            _recalculateProgramOffset = true;
                            Update(false);
                            SetFocus();
                            return true;
                        }
                        if (iControl == (int)Controls.SPINCONTROL_TIME_INTERVAL)
                        {
                            GUISpinControl cntlTimeInt = GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL) as GUISpinControl;
                            int iInterval = (cntlTimeInt.Value) + 1;
                            if (iInterval > 4)
                            {
                                iInterval = 4;
                            }
                            _timePerBlock = iInterval * 15;
                            Update(false);
                            SetFocus();
                            return true;
                        }
                        if (iControl == (int)Controls.CHANNEL_GROUP_BUTTON)
                        {
                            OnSelectChannelGroup();
                            return true;
                        }
                        if (iControl >= GUIDE_COMPONENTID_START)
                        {
                            if (OnSelectItem(true))
                            {
                                Update(false);
                                SetFocus();
                            }
                        }
                        else if (_cursorY == 0)
                        {
                            OnSwitchMode();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("TvGuideBase: {0}", ex);
            }
            return base.OnMessage(message);
        }

        public override void Process()
        {
            TvHome.UpdateProgressPercentageBar();

            OnKeyTimeout();

            //if we did a manual rec. on the tvguide directly, then we have to wait for it to start and the update the GUI.
            //if (_recordingExpected != null)
            //{
            //    TimeSpan ts = DateTime.Now - _updateTimerRecExpected;
            //    if (ts.TotalMilliseconds > 1000)
            //    {
            //        _updateTimerRecExpected = DateTime.Now;
            //        VirtualCard card;
            //        if (_server.IsRecording(_recordingExpected.IdChannel, out card))
            //        {
            //            _recordingExpected = null;
            //            GetChannels(true);
            //            LoadSchedules(true);
            //            _needUpdate = true;
            //        }
            //    }
            //}

            //reload schedules when upcoming recordings/alerts/suggestions are changed
            if (ReloadSchedules)
            {
                ReloadSchedules = false;
                LoadSchedules(true);
                _needUpdate = true;
            }

            if (PluginMain.Navigator.CheckChannelChange())
            {
                if (!PluginMain.Navigator.LastChannelChangeFailed)
                {
                    g_Player.ShowFullScreenWindow();
                }
                TvHome.UpdateProgressPercentageBar(true);
            }

            if (_needUpdate)
            {
                _needUpdate = false;
                Update(false);
                SetFocus();
            }

            GUIImage vertLine = GetControl((int)Controls.VERTICAL_LINE) as GUIImage;
            if (vertLine != null)
            {
                if (_singleChannelView)
                {
                    vertLine.IsVisible = false;
                }
                else
                {
                    vertLine.IsVisible = true;

                    DateTime dateNow = DateTime.Now.Date;
                    DateTime datePrev = _viewingTime.Date;
                    TimeSpan ts = dateNow - datePrev;
                    if (ts.TotalDays == 1)
                    {
                        _viewingTime = DateTime.Now;
                    }


                    if (_viewingTime.Date.Equals(DateTime.Now.Date) && _viewingTime < DateTime.Now)
                    {
                        int iStartX = GetControl((int)Controls.LABEL_TIME1).XPosition;
                        int iWidth = GetControl((int)Controls.LABEL_TIME1 + 1).XPosition - iStartX;
                        iWidth *= 4;

                        int iMin = _viewingTime.Minute;
                        int iStartTime = _viewingTime.Hour * 60 + iMin;
                        int iCurTime = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                        if (iCurTime >= iStartTime)
                        {
                            iCurTime -= iStartTime;
                        }
                        else
                        {
                            iCurTime = 24 * 60 + iCurTime - iStartTime;
                        }

                        int iTimeWidth = (_numberOfBlocks * _timePerBlock);
                        float fpos = ((float)iCurTime) / ((float)(iTimeWidth));
                        fpos *= (float)iWidth;
                        fpos += (float)iStartX;
                        int width = vertLine.Width / 2;
                        vertLine.IsVisible = true;
                        vertLine.SetPosition((int)fpos - width, vertLine.YPosition);
                        vertLine.Select(0);
                        ts = DateTime.Now - _updateTimer;
                        if (ts.TotalMinutes >= 1)
                        {
                            if ((DateTime.Now - _viewingTime).TotalMinutes >= iTimeWidth / 2)
                            {
                                _cursorY = 0;
                                _viewingTime = DateTime.Now;
                            }
                            Update(false);
                        }
                    }
                    else
                    {
                        vertLine.IsVisible = false;
                    }
                }
            }
        }

        public override void Render(float timePassed)
        {
            lock (this)
            {
                GUIImage vertLine = GetControl((int)Controls.VERTICAL_LINE) as GUIImage;
                base.Render(timePassed);
                if (vertLine != null)
                {
                    vertLine.Render(timePassed);
                }
            }
        }

        #endregion

        #region private members

        private void Update(bool selectCurrentShow)
        {
            lock (this)
            {
                if (GUIWindowManager.ActiveWindowEx != this.GetID)
                {
                    return;
                }

                // sets button visible state
                UpdateGroupButton();

                _updateTimer = DateTime.Now;
                GUISpinControl cntlDay = GetControl((int)Controls.SPINCONTROL_DAY) as GUISpinControl;

                // Find first day in TVGuide and set spincontrol position
                int iDay = CalcDays();
                for (; iDay < 0; ++iDay)
                {
                    _viewingTime = _viewingTime.AddDays(1.0);
                }
                for (; iDay >= MaxDaysInGuide; --iDay)
                {
                    _viewingTime = _viewingTime.AddDays(-1.0);
                }
                cntlDay.Value = iDay;

                int xpos, ypos;
                GUIControl cntlPanel = GetControl((int)Controls.PANEL_BACKGROUND);
                GUIImage cntlChannelImg = (GUIImage)GetControl((int)Controls.CHANNEL_IMAGE_TEMPLATE);
                GUILabelControl cntlChannelLabel = (GUILabelControl)GetControl((int)Controls.CHANNEL_LABEL_TEMPLATE);
                GUILabelControl labelTime = (GUILabelControl)GetControl((int)Controls.LABEL_TIME1);
                GUIImage cntlHeaderBkgImg = (GUIImage)GetControl((int)Controls.IMG_TIME1);
                GUIImage cntlChannelTemplate = (GUIImage)GetControl((int)Controls.CHANNEL_TEMPLATE);


                _titleDarkTemplate = GetControl((int)Controls.LABEL_TITLE_DARK_TEMPLATE) as GUILabelControl;
                _titleTemplate = GetControl((int)Controls.LABEL_TITLE_TEMPLATE) as GUILabelControl;
                _genreDarkTemplate = GetControl((int)Controls.LABEL_GENRE_DARK_TEMPLATE) as GUILabelControl;
                _genreTemplate = GetControl((int)Controls.LABEL_GENRE_TEMPLATE) as GUILabelControl;

                _programPartialRecordTemplate = GetControl((int)Controls.BUTTON_PROGRAM_PARTIAL_RECORD) as GUIButton3PartControl;
                _programRecordTemplate = GetControl((int)Controls.BUTTON_PROGRAM_RECORD) as GUIButton3PartControl;
                _programNotifyTemplate = GetControl((int)Controls.BUTTON_PROGRAM_NOTIFY) as GUIButton3PartControl;
                _programNotRunningTemplate = GetControl((int)Controls.BUTTON_PROGRAM_NOT_RUNNING) as GUIButton3PartControl;
                _programRunningTemplate = GetControl((int)Controls.BUTTON_PROGRAM_RUNNING) as GUIButton3PartControl;

                _showChannelLogos = cntlChannelImg != null;
                if (_showChannelLogos)
                {
                    cntlChannelImg.IsVisible = false;
                }
                cntlChannelLabel.IsVisible = false;
                cntlHeaderBkgImg.IsVisible = false;
                labelTime.IsVisible = false;
                cntlChannelTemplate.IsVisible = false;
                int iLabelWidth = (cntlPanel.XPosition + cntlPanel.Width - labelTime.XPosition) / 4;

                // add labels for time blocks 1-4
                int iHour, iMin;
                iMin = _viewingTime.Minute;
                _viewingTime = _viewingTime.AddMinutes(-iMin);
                iMin = (iMin / _timePerBlock) * _timePerBlock;
                _viewingTime = _viewingTime.AddMinutes(iMin);

                DateTime dt = new DateTime();
                dt = _viewingTime;

                for (int iLabel = 0; iLabel < 4; iLabel++)
                {
                    xpos = iLabel * iLabelWidth + labelTime.XPosition;
                    ypos = labelTime.YPosition;

                    GUIImage img = GetControl((int)Controls.IMG_TIME1 + iLabel) as GUIImage;
                    if (img == null)
                    {
                        img = new GUIImage(GetID, (int)Controls.IMG_TIME1 + iLabel, xpos, ypos, iLabelWidth - 4,
                                           cntlHeaderBkgImg.RenderHeight, cntlHeaderBkgImg.FileName, 0x0);
                        img.AllocResources();
                        GUIControl cntl2 = (GUIControl)img;
                        Add(ref cntl2);
                    }

                    img.IsVisible = !_singleChannelView;
                    img.Width = iLabelWidth - 4;
                    img.Height = cntlHeaderBkgImg.RenderHeight;
                    img.SetFileName(cntlHeaderBkgImg.FileName);
                    img.SetPosition(xpos, ypos);
                    img.DoUpdate();

                    GUILabelControl label = GetControl((int)Controls.LABEL_TIME1 + iLabel) as GUILabelControl;
                    if (label == null)
                    {
                        label = new GUILabelControl(GetID, (int)Controls.LABEL_TIME1 + iLabel, xpos, ypos, iLabelWidth,
                                                    cntlHeaderBkgImg.RenderHeight, labelTime.FontName, String.Empty,
                                                    labelTime.TextColor, GuideBase.TimeAlignment, labelTime.TextVAlignment, false,
                                                    labelTime.ShadowAngle, labelTime.ShadowDistance, labelTime.ShadowColor);
                        label.AllocResources();
                        GUIControl cntl = (GUIControl)label;
                        this.Add(ref cntl);
                    }
                    iHour = dt.Hour;
                    iMin = dt.Minute;
                    string strTime = dt.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat);
                    label.Label = " " + strTime;
                    dt = dt.AddMinutes(_timePerBlock);

                    label.TextAlignment = GuideBase.TimeAlignment;
                    label.IsVisible = !_singleChannelView;
                    label.Width = iLabelWidth;
                    label.Height = cntlHeaderBkgImg.RenderHeight;
                    label.FontName = labelTime.FontName;
                    label.TextColor = labelTime.TextColor;
                    label.SetPosition(xpos, ypos);
                }

                // add channels...
                int iHeight = cntlPanel.Height + cntlPanel.YPosition - cntlChannelTemplate.YPosition;
                int iItemHeight = cntlChannelTemplate.Height;

                _channelCount = (int)(((float)iHeight) / ((float)iItemHeight));
                for (int iChan = 0; iChan < _channelCount; ++iChan)
                {
                    xpos = cntlChannelTemplate.XPosition;
                    ypos = cntlChannelTemplate.YPosition + iChan * iItemHeight;

                    //this.Remove((int)Controls.IMG_CHAN1+iChan);
                    GUIButton3PartControl imgBut = GetControl((int)Controls.IMG_CHAN1 + iChan) as GUIButton3PartControl;
                    if (imgBut == null)
                    {
                        string strChannelImageFileName = String.Empty;
                        if (_showChannelLogos)
                        {
                            strChannelImageFileName = cntlChannelImg.FileName;
                        }

                        // Use a template control if it exists, otherwise use default values.
                        GUIButton3PartControl buttonTemplate = GetControl((int)Controls.BUTTON_PROGRAM_NOT_RUNNING) as GUIButton3PartControl;
                        if (buttonTemplate != null)
                        {
                            buttonTemplate.IsVisible = false;
                            imgBut = new GUIButton3PartControl(GetID, (int)Controls.IMG_CHAN1 + iChan, xpos, ypos,
                                                               cntlChannelTemplate.Width - 2, cntlChannelTemplate.Height - 2,
                                                               buttonTemplate.TexutureFocusLeftName,
                                                               buttonTemplate.TexutureFocusMidName,
                                                               buttonTemplate.TexutureFocusRightName,
                                                               buttonTemplate.TexutureNoFocusLeftName,
                                                               buttonTemplate.TexutureNoFocusMidName,
                                                               buttonTemplate.TexutureNoFocusRightName,
                                                               strChannelImageFileName);

                            imgBut.TileFillTFL = buttonTemplate.TileFillTFL;
                            imgBut.TileFillTNFL = buttonTemplate.TileFillTNFL;
                            imgBut.TileFillTFM = buttonTemplate.TileFillTFM;
                            imgBut.TileFillTNFM = buttonTemplate.TileFillTNFM;
                            imgBut.TileFillTFR = buttonTemplate.TileFillTFR;
                            imgBut.TileFillTNFR = buttonTemplate.TileFillTNFR;
                        }
                        else
                        {
                            imgBut = new GUIButton3PartControl(GetID, (int)Controls.IMG_CHAN1 + iChan, xpos, ypos,
                                                               cntlChannelTemplate.Width - 2, cntlChannelTemplate.Height - 2,
                                                               "tvguide_button_selected_left.png",
                                                               "tvguide_button_selected_middle.png",
                                                               "tvguide_button_selected_right.png",
                                                               "tvguide_button_light_left.png",
                                                               "tvguide_button_light_middle.png",
                                                               "tvguide_button_light_right.png",
                                                               strChannelImageFileName);
                        }
                        imgBut.AllocResources();
                        GUIControl cntl = (GUIControl)imgBut;
                        Add(ref cntl);
                    }

                    imgBut.Width = cntlChannelTemplate.Width - 2; //labelTime.XPosition-cntlChannelImg.XPosition;
                    imgBut.Height = cntlChannelTemplate.Height - 2; //iItemHeight-2;
                    imgBut.SetPosition(xpos, ypos);
                    imgBut.FontName1 = cntlChannelLabel.FontName;
                    imgBut.TextColor1 = cntlChannelLabel.TextColor;
                    imgBut.Label1 = String.Empty;
                    imgBut.RenderLeft = false;
                    imgBut.RenderRight = false;
                    imgBut.SetShadow1(cntlChannelLabel.ShadowAngle, cntlChannelLabel.ShadowDistance, cntlChannelLabel.ShadowColor);

                    if (_showChannelLogos)
                    {
                        imgBut.TexutureIcon = cntlChannelImg.FileName;
                        imgBut.IconOffsetX = cntlChannelImg.XPosition;
                        imgBut.IconOffsetY = cntlChannelImg.YPosition;
                        imgBut.IconWidth = cntlChannelImg.RenderWidth;
                        imgBut.IconHeight = cntlChannelImg.RenderHeight;
                        imgBut.IconKeepAspectRatio = cntlChannelImg.KeepAspectRatio;
                        imgBut.IconCentered = cntlChannelImg.Centered;
                        imgBut.IconZoom = cntlChannelImg.Zoom;
                    }
                    imgBut.TextOffsetX1 = cntlChannelLabel.XPosition;
                    imgBut.TextOffsetY1 = cntlChannelLabel.YPosition;
                    imgBut.ColourDiffuse = 0xffffffff;
                    imgBut.DoUpdate();
                }

                UpdateHorizontalScrollbar();
                UpdateVerticalScrollbar();

                GetChannels(false);


                string day;
                switch (_viewingTime.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        day = GUILocalizeStrings.Get(657);
                        break;
                    case DayOfWeek.Tuesday:
                        day = GUILocalizeStrings.Get(658);
                        break;
                    case DayOfWeek.Wednesday:
                        day = GUILocalizeStrings.Get(659);
                        break;
                    case DayOfWeek.Thursday:
                        day = GUILocalizeStrings.Get(660);
                        break;
                    case DayOfWeek.Friday:
                        day = GUILocalizeStrings.Get(661);
                        break;
                    case DayOfWeek.Saturday:
                        day = GUILocalizeStrings.Get(662);
                        break;
                    default:
                        day = GUILocalizeStrings.Get(663);
                        break;
                }
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.View.SDOW", day);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.View.Month", _viewingTime.Month.ToString());
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.View.Day", _viewingTime.Day.ToString());

                //day = String.Format("{0} {1}-{2}", day, _viewingTime.Day, _viewingTime.Month);
                day = Utils.GetShortDayString(_viewingTime);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Day", day);

                //2004 03 31 22 20 00
                string strStart = String.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}",
                                                _viewingTime.Year, _viewingTime.Month, _viewingTime.Day,
                                                _viewingTime.Hour, _viewingTime.Minute, 0);
                DateTime dtStop = new DateTime();
                dtStop = _viewingTime;
                dtStop = dtStop.AddMinutes(_numberOfBlocks * _timePerBlock - 1);
                iMin = dtStop.Minute;
                string strEnd = String.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}",
                                              dtStop.Year, dtStop.Month, dtStop.Day,
                                              dtStop.Hour, iMin, 0);

                long iStart = Int64.Parse(strStart);
                long iEnd = Int64.Parse(strEnd);


                LoadSchedules(false);

                if (_channelOffset > _channelList.Count)
                {
                    _channelOffset = 0;
                    _cursorX = 0;
                }

                for (int i = 0; i < controlList.Count; ++i)
                {
                    GUIControl cntl = (GUIControl)controlList[i];
                    if (cntl.GetID >= GUIDE_COMPONENTID_START)
                    {
                        cntl.IsVisible = false;
                    }
                }

                if (_singleChannelView)
                {
                    // show all buttons (could be less visible if channels < rows)
                    for (int iChannel = 0; iChannel < _channelCount; iChannel++)
                    {
                        GUIButton3PartControl imgBut = GetControl((int)Controls.IMG_CHAN1 + iChannel) as GUIButton3PartControl;
                        if (imgBut != null)
                            imgBut.IsVisible = true;
                    }

                    Channel channel = (Channel)_channelList[_singleChannelNumber].channel;
                    setGuideHeadingVisibility(false);
                    using (GuideServiceAgent tvGuideAgent = new GuideServiceAgent())
                    using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                    {
                        RenderSingleChannel(tvSchedulerAgent, tvGuideAgent, channel);
                    }
                }
                else
                {
                    List<Channel> visibleChannels = new List<Channel>();

                    int chan = _channelOffset;
                    for (int iChannel = 0; iChannel < _channelCount; iChannel++)
                    {
                        if (chan < _channelList.Count)
                        {
                            visibleChannels.Add(_channelList[chan].channel);
                        }
                        chan++;
                        if (chan >= _channelList.Count && visibleChannels.Count < _channelList.Count)
                        {
                            chan = 0;
                        }
                    }
                    using (GuideServiceAgent tvGuideAgent = new GuideServiceAgent())
                    using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                    {
                        _controller.RefreshChannelsEpgData(tvGuideAgent, visibleChannels,
                            Utils.longtodate(iStart), Utils.longtodate(iEnd));

                        // make sure the TV Guide heading is visiable and the single channel labels are not.
                        setGuideHeadingVisibility(true);
                        SetSingleChannelLabelVisibility(false);
                        chan = _channelOffset;

                        int firstButtonYPos = 0;
                        int lastButtonYPos = 0;

                        for (int iChannel = 0; iChannel < _channelCount; iChannel++)
                        {
                            if (chan < _channelList.Count)
                            {
                                GuideBaseChannel tvGuideChannel = _channelList[chan];
                                RenderChannel(tvSchedulerAgent, tvGuideAgent, iChannel, tvGuideChannel, iStart, iEnd, selectCurrentShow);
                                // remember bottom y position from last visible button
                                GUIButton3PartControl imgBut = GetControl((int)Controls.IMG_CHAN1 + iChannel) as GUIButton3PartControl;
                                if (imgBut != null)
                                {
                                    if (iChannel == 0)
                                        firstButtonYPos = imgBut.YPosition;

                                    lastButtonYPos = imgBut.YPosition + imgBut.Height;
                                }
                            }
                            chan++;
                            if (chan >= _channelList.Count && _channelList.Count > _channelCount)
                            {
                                chan = 0;
                            }
                            if (chan > _channelList.Count)
                            {
                                GUIButton3PartControl imgBut = GetControl((int)Controls.IMG_CHAN1 + iChannel) as GUIButton3PartControl;
                                if (imgBut != null)
                                {
                                    imgBut.IsVisible = false;
                                }
                            }
                        }

                        GUIImage vertLine = GetControl((int)Controls.VERTICAL_LINE) as GUIImage;
                        if (vertLine != null)
                        {
                            // height taken from last button (bottom) minus the yposition of slider plus the offset of slider in relation to first button
                            vertLine.Height = lastButtonYPos - vertLine.YPosition + (firstButtonYPos - vertLine.YPosition);
                        }
                        // update selected channel
                        _singleChannelNumber = _cursorX + _channelOffset;
                        if (_singleChannelNumber >= _channelList.Count)
                        {
                            _singleChannelNumber -= _channelList.Count;
                        }

                        // instead of direct casting us "as"; else it fails for other controls!
                        GUIButton3PartControl img = GetControl(_cursorX + (int)Controls.IMG_CHAN1) as GUIButton3PartControl;
                        if (null != img)
                        {
                            _currentChannel = (Channel)img.Data;
                        }
                    }
                }
                UpdateVerticalScrollbar();
            }
        }

        private string GetChannelLogo(Channel channel)
        {
            string logoImagePath;
            using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
            {
                logoImagePath = Utility.GetLogoImage(channel, tvSchedulerAgent);
                if (!System.IO.File.Exists(logoImagePath))
                {
                    logoImagePath = "defaultVideoBig.png";
                }
            }
            return logoImagePath;
        }

        private void SetProperties()
        {
            if (_channelList == null)
            {
                return;
            }
            if (_channelList.Count == 0)
            {
                return;
            }
            int channel = _cursorX + _channelOffset;
            while (channel >= _channelList.Count)
            {
                channel -= _channelList.Count;
            }
            if (channel < 0)
            {
                channel = 0;
            }
            Channel chan = (Channel)_channelList[channel].channel;
            if (chan == null)
            {
                return;
            }
            string strChannel = chan.DisplayName;

            if (!_singleChannelView)
            {
                string strLogo = GetChannelLogo(chan);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.thumb", strLogo);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.ChannelName", strChannel);
                string channelNumber = String.Empty;
                if (_showChannelNumber
                    && chan.LogicalChannelNumber.HasValue)
                {
                    channelNumber = chan.LogicalChannelNumber.Value.ToString();
                    GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.ChannelNumber", channelNumber);
                }
                else
                {
                    GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.ChannelNumber", String.Empty);
                }
            }

            if (_cursorY == 0 || _currentProgram == null)
            {
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Title", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.CompositeTitle", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Time", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Description", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Genre", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.SubTitle", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Episode", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.EpisodeDetail", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Date", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.StarRating", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Classification", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Duration", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.DurationMins", String.Empty);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.TimeFromNow", String.Empty);

                _currentStartTime = 0;
                _currentEndTime = 0;
                _currentTitle = String.Empty;
                _currentTime = String.Empty;
                _currentChannel = chan;
                GUIControl.HideControl(GetID, (int)Controls.IMG_REC_PIN);
            }
            else if (_currentProgram != null)
            {
                string strTime = String.Format("{0}-{1}",
                                               _currentProgram.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                               _currentProgram.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Title", _currentProgram.Title);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.CompositeTitle", _currentProgram.Title);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Time", strTime);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Description", _currentProgram.CreateCombinedDescription(true));
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Genre", _currentProgram.Category);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Duration", GetDuration(_currentProgram));
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.DurationMins", GetDurationAsMinutes(_currentProgram));
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.TimeFromNow", GetStartTimeFromNow(_currentProgram));
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Episode", _currentProgram.EpisodeNumberDisplay);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.SubTitle", _currentProgram.SubTitle);

                if (String.IsNullOrEmpty(_currentProgram.Rating))
                {
                    GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Classification", "No Rating");
                }
                else
                {
                    GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Classification", _currentProgram.Rating);
                }

                _currentStartTime = Utils.datetolong(_currentProgram.StartTime);
                _currentEndTime = Utils.datetolong(_currentProgram.StopTime);
                _currentTitle = _currentProgram.Title;
                _currentTime = strTime;
                _currentChannel = chan;

                string recordIconImage = GetChannelProgramIcon(chan, _currentProgram.GuideProgramId);
                if (String.IsNullOrEmpty(recordIconImage))
                {
                    GUIControl.HideControl(GetID, (int)Controls.IMG_REC_PIN);
                }
                else
                {
                    GUIImage img = (GUIImage)GetControl((int)Controls.IMG_REC_PIN);
                    img.SetFileName(recordIconImage);
                    GUIControl.ShowControl(GetID, (int)Controls.IMG_REC_PIN);
                }
            }

            _currentRecOrNotify = _currentProgram != null && PluginMain.IsActiveRecording(_currentChannel.ChannelId, _currentProgram);
        }

        private void RenderSingleChannel(SchedulerServiceAgent tvSchedulerAgent, GuideServiceAgent tvGuideAgent, Channel channel)
        {
            string strLogo;
            int chan = _channelOffset;
            for (int iChannel = 0; iChannel < _channelCount; iChannel++)
            {
                if (chan < _channelList.Count)
                {
                    Channel tvChan = _channelList[chan].channel;

                    strLogo = GetChannelLogo(tvChan);
                    GUIButton3PartControl img = GetControl(iChannel + (int)Controls.IMG_CHAN1) as GUIButton3PartControl;
                    if (img != null)
                    {
                        if (_showChannelLogos)
                        {
                            img.TexutureIcon = strLogo;
                        }
                        img.Label1 = tvChan.DisplayName;
                        img.Data = tvChan;
                        img.IsVisible = true;
                    }
                }
                chan++;
            }

            List<GuideProgramSummary> programs = new List<GuideProgramSummary>();
            DateTime dtStart = DateTime.Now;
            dtStart = dtStart.AddDays(-1);
            DateTime dtEnd = dtStart.AddDays(30);
            long iStart = Utils.datetolong(dtStart);
            long iEnd = Utils.datetolong(dtEnd);

            if (channel.GuideChannelId.HasValue)
            {
                programs = new List<GuideProgramSummary>(
                    tvGuideAgent.GetChannelProgramsBetween(channel.GuideChannelId.Value, Utils.longtodate(iStart), Utils.longtodate(iEnd)));
            }

            _totalProgramCount = programs.Count;
            if (_totalProgramCount == 0)
            {
                _totalProgramCount = _channelCount;
            }

            GUILabelControl channelLabel = GetControl((int)Controls.SINGLE_CHANNEL_LABEL) as GUILabelControl;
            GUIImage channelImage = GetControl((int)Controls.SINGLE_CHANNEL_IMAGE) as GUIImage;

            strLogo = GetChannelLogo(channel);
            if (channelImage == null)
            {
                if (strLogo.Length > 0)
                {
                    channelImage = new GUIImage(GetID, (int)Controls.SINGLE_CHANNEL_IMAGE,
                                                GetControl((int)Controls.LABEL_TIME1).XPosition,
                                                GetControl((int)Controls.LABEL_TIME1).YPosition - 15,
                                                40, 40, strLogo, Color.White);
                    channelImage.AllocResources();
                    GUIControl temp = (GUIControl)channelImage;
                    Add(ref temp);
                }
            }
            else
            {
                channelImage.SetFileName(strLogo);
            }

            if (channelLabel == null)
            {
                channelLabel = new GUILabelControl(GetID, (int)Controls.SINGLE_CHANNEL_LABEL,
                                                   channelImage.XPosition + 44,
                                                   channelImage.YPosition + 10,
                                                   300, 40, "font16", channel.DisplayName, 4294967295, GUIControl.Alignment.Left,
                                                   GUIControl.VAlignment.Top,
                                                   true, 0, 0, 0xFF000000);
                channelLabel.AllocResources();
                GUIControl temp = channelLabel;
                Add(ref temp);
            }

            SetSingleChannelLabelVisibility(true);

            channelLabel.Label = channel.DisplayName;
            if (strLogo.Length > 0)
            {
                channelImage.SetFileName(strLogo);
            }

            if (channelLabel != null)
            {
                channelLabel.Label = channel.DisplayName;
            }
            if (_recalculateProgramOffset)
            {
                _recalculateProgramOffset = false;
                bool found = false;
                for (int i = 0; i < programs.Count; i++)
                {
                    GuideProgramSummary program = programs[i];
                    if (program.StartTime <= _viewingTime && program.StopTime >= _viewingTime)
                    {
                        _programOffset = i;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    _programOffset = 0;
                }
            }
            else if (_programOffset < programs.Count)
            {
                int day = programs[_programOffset].StartTime.DayOfYear;
                bool changed = false;
                while (day > _viewingTime.DayOfYear)
                {
                    _viewingTime = _viewingTime.AddDays(1.0);
                    changed = true;
                }
                while (day < _viewingTime.DayOfYear)
                {
                    _viewingTime = _viewingTime.AddDays(-1.0);
                    changed = true;
                }
                if (changed)
                {
                    GUISpinControl cntlDay = GetControl((int)Controls.SPINCONTROL_DAY) as GUISpinControl;

                    // Find first day in TVGuide and set spincontrol position
                    int iDay = CalcDays();
                    for (; iDay < 0; ++iDay)
                    {
                        _viewingTime = _viewingTime.AddDays(1.0);
                    }
                    for (; iDay >= MaxDaysInGuide; --iDay)
                    {
                        _viewingTime = _viewingTime.AddDays(-1.0);
                    }
                    cntlDay.Value = iDay;
                }
            }
            // ichan = number of rows
            for (int ichan = 0; ichan < _channelCount; ++ichan)
            {
                GUIButton3PartControl imgCh = GetControl(ichan + (int)Controls.IMG_CHAN1) as GUIButton3PartControl;
                imgCh.TexutureIcon = "";

                int iStartXPos = GetControl(0 + (int)Controls.LABEL_TIME1).XPosition;
                int height = GetControl((int)Controls.IMG_CHAN1 + 1).YPosition;
                height -= GetControl((int)Controls.IMG_CHAN1).YPosition;
                int width = GetControl((int)Controls.LABEL_TIME1 + 1).XPosition;
                width -= GetControl((int)Controls.LABEL_TIME1).XPosition;

                int iTotalWidth = width * _numberOfBlocks;

                GuideProgramSummary program;
                int offset = _programOffset;
                if (offset + ichan < programs.Count)
                {
                    program = programs[offset + ichan];
                }
                else
                {
                    // bugfix for 0 items
                    if (programs.Count == 0)
                    {
                        program = new GuideProgramSummary()
                        {
                            StartTime = _viewingTime,
                            StopTime = _viewingTime,
                            StartTimeUtc = _viewingTime.ToUniversalTime(),
                            StopTimeUtc = _viewingTime.ToUniversalTime(),
                            Title = String.Empty,
                            Category = String.Empty
                        };
                    }
                    else
                    {
                        program = programs[programs.Count - 1];
                        if (program.StopTime.DayOfYear == _viewingTime.DayOfYear)
                        {
                            program = new GuideProgramSummary()
                            {
                                StartTime = program.StopTime,
                                StopTime = program.StopTime,
                                StartTimeUtc = program.StartTimeUtc.ToUniversalTime(),
                                StopTimeUtc = program.StopTimeUtc.ToUniversalTime(),
                                Title = String.Empty,
                                Category = String.Empty
                            };
                        }
                        else
                        {
                            program = new GuideProgramSummary()
                            {
                                StartTime = _viewingTime,
                                StopTime = _viewingTime,
                                StartTimeUtc = _viewingTime.ToUniversalTime(),
                                StopTimeUtc = _viewingTime.ToUniversalTime(),
                                Title = String.Empty,
                                Category = String.Empty
                            };
                        }
                    }
                }

                int ypos = GetControl(ichan + (int)Controls.IMG_CHAN1).YPosition;
                int iControlId = GUIDE_COMPONENTID_START + ichan * RowID + 0 * ColID;
                GUIButton3PartControl img = GetControl(iControlId) as GUIButton3PartControl;
                GUIButton3PartControl buttonTemplate = GetControl((int)Controls.BUTTON_PROGRAM_NOT_RUNNING) as GUIButton3PartControl;

                if (img == null)
                {
                    if (buttonTemplate != null)
                    {
                        buttonTemplate.IsVisible = false;
                        img = new GUIButton3PartControl(GetID, iControlId, iStartXPos, ypos, iTotalWidth, height - 2,
                                                        buttonTemplate.TexutureFocusLeftName,
                                                        buttonTemplate.TexutureFocusMidName,
                                                        buttonTemplate.TexutureFocusRightName,
                                                        buttonTemplate.TexutureNoFocusLeftName,
                                                        buttonTemplate.TexutureNoFocusMidName,
                                                        buttonTemplate.TexutureNoFocusRightName,
                                                        String.Empty);

                        img.TileFillTFL = buttonTemplate.TileFillTFL;
                        img.TileFillTNFL = buttonTemplate.TileFillTNFL;
                        img.TileFillTFM = buttonTemplate.TileFillTFM;
                        img.TileFillTNFM = buttonTemplate.TileFillTNFM;
                        img.TileFillTFR = buttonTemplate.TileFillTFR;
                        img.TileFillTNFR = buttonTemplate.TileFillTNFR;
                    }
                    else
                    {
                        img = new GUIButton3PartControl(GetID, iControlId, iStartXPos, ypos, iTotalWidth, height - 2,
                                                        "tvguide_button_selected_left.png",
                                                        "tvguide_button_selected_middle.png",
                                                        "tvguide_button_selected_right.png",
                                                        "tvguide_button_light_left.png",
                                                        "tvguide_button_light_middle.png",
                                                        "tvguide_button_light_right.png",
                                                        String.Empty);
                    }
                    img.AllocResources();
                    img.ColourDiffuse = GetColorForGenre(program.Category);
                    GUIControl cntl = (GUIControl)img;
                    Add(ref cntl);
                }
                else
                {
                    if (buttonTemplate != null)
                    {
                        buttonTemplate.IsVisible = false;
                        img.TexutureFocusLeftName = buttonTemplate.TexutureFocusLeftName;
                        img.TexutureFocusMidName = buttonTemplate.TexutureFocusMidName;
                        img.TexutureFocusRightName = buttonTemplate.TexutureFocusRightName;
                        img.TexutureNoFocusLeftName = buttonTemplate.TexutureNoFocusLeftName;
                        img.TexutureNoFocusMidName = buttonTemplate.TexutureNoFocusMidName;
                        img.TexutureNoFocusRightName = buttonTemplate.TexutureNoFocusRightName;
                        img.TileFillTFL = buttonTemplate.TileFillTFL;
                        img.TileFillTNFL = buttonTemplate.TileFillTNFL;
                        img.TileFillTFM = buttonTemplate.TileFillTFM;
                        img.TileFillTNFM = buttonTemplate.TileFillTNFM;
                        img.TileFillTFR = buttonTemplate.TileFillTFR;
                        img.TileFillTNFR = buttonTemplate.TileFillTNFR;
                    }
                    else
                    {
                        img.TexutureFocusLeftName = "tvguide_button_selected_left.png";
                        img.TexutureFocusMidName = "tvguide_button_selected_middle.png";
                        img.TexutureFocusRightName = "tvguide_button_selected_right.png";
                        img.TexutureNoFocusLeftName = "tvguide_button_light_left.png";
                        img.TexutureNoFocusMidName = "tvguide_button_light_middle.png";
                        img.TexutureNoFocusRightName = "tvguide_button_light_right.png";
                    }
                    img.Focus = false;
                    img.SetPosition(iStartXPos, ypos);
                    img.Width = iTotalWidth;
                    img.ColourDiffuse = GetColorForGenre(program.Category);
                    img.IsVisible = true;
                    img.DoUpdate();
                }
                img.RenderLeft = false;
                img.RenderRight = false;

                bool isRecording;
                bool isAlert;
                string recordIconImage = GetChannelProgramIcon(channel, program.GuideProgramId, out isRecording,out isAlert);

                img.Data = program;
                img.ColourDiffuse = GetColorForGenre(program.Category);
                height = height - 10;
                height /= 2;
                int iWidth = iTotalWidth;
                if (iWidth > 10)
                {
                    iWidth -= 10;
                }
                else
                {
                    iWidth = 1;
                }

                DateTime dt = DateTime.Now;

                img.TextOffsetX1 = 5;
                img.TextOffsetY1 = 5;
                img.FontName1 = "font13";
                img.TextColor1 = 0xffffffff;

                img.Label1 = program.CreateProgramTitle();

                string strTimeSingle = String.Format("{0}",
                                                     program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                if (program.StartTime.DayOfYear != _viewingTime.DayOfYear)
                {
                    img.Label1 = String.Format("{0} {1}", Utility.GetShortDayDateString(program.StartTime), program.CreateProgramTitle());
                }

                GUILabelControl labelTemplate;
                if (IsRunningAt(program, dt))
                {
                    labelTemplate = _titleDarkTemplate;
                }
                else
                {
                    labelTemplate = _titleTemplate;
                }

                if (labelTemplate != null)
                {
                    img.FontName1 = labelTemplate.FontName;
                    img.TextColor1 = labelTemplate.TextColor;
                    img.TextOffsetX1 = labelTemplate.XPosition;
                    img.TextOffsetY1 = labelTemplate.YPosition;
                    img.SetShadow1(labelTemplate.ShadowAngle, labelTemplate.ShadowDistance, labelTemplate.ShadowColor);
                }
                img.TextOffsetX2 = 5;
                img.TextOffsetY2 = img.Height / 2;
                img.FontName2 = "font13";
                img.TextColor2 = 0xffffffff;
                img.Label2 = "";
                if (IsRunningAt(program, dt))
                {
                    img.TextColor2 = 0xff101010;
                    labelTemplate = _genreDarkTemplate;
                }
                else
                {
                    labelTemplate = _genreTemplate;
                }

                if (labelTemplate != null)
                {
                    img.FontName2 = labelTemplate.FontName;
                    img.TextColor2 = labelTemplate.TextColor;
                    img.Label2 = program.Category;
                    img.TextOffsetX2 = labelTemplate.XPosition;
                    img.TextOffsetY2 = labelTemplate.YPosition;
                    img.SetShadow2(labelTemplate.ShadowAngle, labelTemplate.ShadowDistance, labelTemplate.ShadowColor);
                }
                imgCh.Label1 = strTimeSingle;
                imgCh.TexutureIcon = "";

                if (IsRunningAt(program, dt))
                {
                    GUIButton3PartControl buttonRunningTemplate = _programRunningTemplate;
                    if (buttonRunningTemplate != null)
                    {
                        buttonRunningTemplate.IsVisible = false;
                        img.TexutureFocusLeftName = buttonRunningTemplate.TexutureFocusLeftName;
                        img.TexutureFocusMidName = buttonRunningTemplate.TexutureFocusMidName;
                        img.TexutureFocusRightName = buttonRunningTemplate.TexutureFocusRightName;
                        img.TexutureNoFocusLeftName = buttonRunningTemplate.TexutureNoFocusLeftName;
                        img.TexutureNoFocusMidName = buttonRunningTemplate.TexutureNoFocusMidName;
                        img.TexutureNoFocusRightName = buttonRunningTemplate.TexutureNoFocusRightName;
                        img.TileFillTFL = buttonRunningTemplate.TileFillTFL;
                        img.TileFillTNFL = buttonRunningTemplate.TileFillTNFL;
                        img.TileFillTFM = buttonRunningTemplate.TileFillTFM;
                        img.TileFillTNFM = buttonRunningTemplate.TileFillTNFM;
                        img.TileFillTFR = buttonRunningTemplate.TileFillTFR;
                        img.TileFillTNFR = buttonRunningTemplate.TileFillTNFR;
                    }
                    else
                    {
                        img.TexutureFocusLeftName = "tvguide_button_selected_left.png";
                        img.TexutureFocusMidName = "tvguide_button_selected_middle.png";
                        img.TexutureFocusRightName = "tvguide_button_selected_right.png";
                        img.TexutureNoFocusLeftName = "tvguide_button_left.png";
                        img.TexutureNoFocusMidName = "tvguide_button_middle.png";
                        img.TexutureNoFocusRightName = "tvguide_button_right.png";
                    }
                }

                img.SetPosition(img.XPosition, img.YPosition);

                img.TexutureIcon = String.Empty;

                if (recordIconImage != null && isAlert)
                {
                    GUIButton3PartControl buttonNotifyTemplate = GetControl((int)Controls.BUTTON_PROGRAM_NOTIFY) as GUIButton3PartControl;
                    if (buttonNotifyTemplate != null)
                    {
                        buttonNotifyTemplate.IsVisible = false;
                        img.TexutureFocusLeftName = buttonNotifyTemplate.TexutureFocusLeftName;
                        img.TexutureFocusMidName = buttonNotifyTemplate.TexutureFocusMidName;
                        img.TexutureFocusRightName = buttonNotifyTemplate.TexutureFocusRightName;
                        img.TexutureNoFocusLeftName = buttonNotifyTemplate.TexutureNoFocusLeftName;
                        img.TexutureNoFocusMidName = buttonNotifyTemplate.TexutureNoFocusMidName;
                        img.TexutureNoFocusRightName = buttonNotifyTemplate.TexutureNoFocusRightName;
                        img.TileFillTFL = buttonNotifyTemplate.TileFillTFL;
                        img.TileFillTNFL = buttonNotifyTemplate.TileFillTNFL;
                        img.TileFillTFM = buttonNotifyTemplate.TileFillTFM;
                        img.TileFillTNFM = buttonNotifyTemplate.TileFillTNFM;
                        img.TileFillTFR = buttonNotifyTemplate.TileFillTFR;
                        img.TileFillTNFR = buttonNotifyTemplate.TileFillTNFR;

                        // Use of the button template control implies use of the icon.  Use a blank image if the icon is not desired.
                        img.TexutureIcon = Thumbs.TvNotifyIcon;
                        img.IconOffsetX = buttonNotifyTemplate.IconOffsetX;
                        img.IconOffsetY = buttonNotifyTemplate.IconOffsetY;
                        img.IconAlign = buttonNotifyTemplate.IconAlign;
                        img.IconVAlign = buttonNotifyTemplate.IconVAlign;
                        img.IconInlineLabel1 = buttonNotifyTemplate.IconInlineLabel1;
                    }
                    else
                    {
                        if (_useNewNotifyButtonColor)
                        {
                            img.TexutureFocusLeftName = "tvguide_notifyButton_Focus_left.png";
                            img.TexutureFocusMidName = "tvguide_notifyButton_Focus_middle.png";
                            img.TexutureFocusRightName = "tvguide_notifyButton_Focus_right.png";
                            img.TexutureNoFocusLeftName = "tvguide_notifyButton_noFocus_left.png";
                            img.TexutureNoFocusMidName = "tvguide_notifyButton_noFocus_middle.png";
                            img.TexutureNoFocusRightName = "tvguide_notifyButton_noFocus_right.png";
                        }
                        else
                        {
                            img.TexutureIcon = Thumbs.TvNotifyIcon;
                        }
                    }
                }

                if (recordIconImage != null)
                {
                    //bool bPartialRecording = program.IsPartialRecordingSeriesPending;
                    GUIButton3PartControl buttonRecordTemplate = GetControl((int)Controls.BUTTON_PROGRAM_RECORD) as GUIButton3PartControl;

                    // Select the partial recording template if needed.
                    //if (bPartialRecording)
                    //{
                    //    buttonRecordTemplate = GetControl((int)Controls.BUTTON_PROGRAM_PARTIAL_RECORD) as GUIButton3PartControl;
                    //}

                    if (isRecording)
                    {
                        if (buttonRecordTemplate != null)
                        {
                            buttonRecordTemplate.IsVisible = false;
                            img.TexutureFocusLeftName = buttonRecordTemplate.TexutureFocusLeftName;
                            img.TexutureFocusMidName = buttonRecordTemplate.TexutureFocusMidName;
                            img.TexutureFocusRightName = buttonRecordTemplate.TexutureFocusRightName;
                            img.TexutureNoFocusLeftName = buttonRecordTemplate.TexutureNoFocusLeftName;
                            img.TexutureNoFocusMidName = buttonRecordTemplate.TexutureNoFocusMidName;
                            img.TexutureNoFocusRightName = buttonRecordTemplate.TexutureNoFocusRightName;
                            img.TileFillTFL = buttonRecordTemplate.TileFillTFL;
                            img.TileFillTNFL = buttonRecordTemplate.TileFillTNFL;
                            img.TileFillTFM = buttonRecordTemplate.TileFillTFM;
                            img.TileFillTNFM = buttonRecordTemplate.TileFillTNFM;
                            img.TileFillTFR = buttonRecordTemplate.TileFillTFR;
                            img.TileFillTNFR = buttonRecordTemplate.TileFillTNFR;

                            // Use of the button template control implies use of the icon.  Use a blank image if the icon is not desired.
                            //if (bConflict)
                            //{
                            //    img.TexutureFocusLeftName = "tvguide_recButton_Focus_left.png";
                            //    img.TexutureFocusMidName = "tvguide_recButton_Focus_middle.png";
                            //    img.TexutureFocusRightName = "tvguide_recButton_Focus_right.png";
                            //    img.TexutureNoFocusLeftName = "tvguide_recButton_noFocus_left.png";
                            //    img.TexutureNoFocusMidName = "tvguide_recButton_noFocus_middle.png";
                            //    img.TexutureNoFocusRightName = "tvguide_recButton_noFocus_right.png";
                            //}
                            img.IconOffsetX = buttonRecordTemplate.IconOffsetX;
                            img.IconOffsetY = buttonRecordTemplate.IconOffsetY;
                            img.IconAlign = buttonRecordTemplate.IconAlign;
                            img.IconVAlign = buttonRecordTemplate.IconVAlign;
                            img.IconInlineLabel1 = buttonRecordTemplate.IconInlineLabel1;
                        }
                        else
                        {
                            //if (bPartialRecording && _useNewPartialRecordingButtonColor)
                            //{
                            //    img.TexutureFocusLeftName = "tvguide_partRecButton_Focus_left.png";
                            //    img.TexutureFocusMidName = "tvguide_partRecButton_Focus_middle.png";
                            //    img.TexutureFocusRightName = "tvguide_partRecButton_Focus_right.png";
                            //    img.TexutureNoFocusLeftName = "tvguide_partRecButton_noFocus_left.png";
                            //    img.TexutureNoFocusMidName = "tvguide_partRecButton_noFocus_middle.png";
                            //    img.TexutureNoFocusRightName = "tvguide_partRecButton_noFocus_right.png";
                            //}
                            //else
                            {
                                if (_useNewRecordingButtonColor)
                                {
                                    img.TexutureFocusLeftName = "tvguide_recButton_Focus_left.png";
                                    img.TexutureFocusMidName = "tvguide_recButton_Focus_middle.png";
                                    img.TexutureFocusRightName = "tvguide_recButton_Focus_right.png";
                                    img.TexutureNoFocusLeftName = "tvguide_recButton_noFocus_left.png";
                                    img.TexutureNoFocusMidName = "tvguide_recButton_noFocus_middle.png";
                                    img.TexutureNoFocusRightName = "tvguide_recButton_noFocus_right.png";
                                }
                            }
                        }
                    }

                    img.TexutureIcon = recordIconImage;
                }
            }
        }

        private void RenderChannel(SchedulerServiceAgent tvSchedulerAgent, GuideServiceAgent tvGuideAgent,
            int iChannel, GuideBaseChannel tvGuideChannel, long iStart, long iEnd, bool selectCurrentShow)
        {
            Channel channel = tvGuideChannel.channel;
            int channelNum = 0;

            if (!_byIndex)
            {
                if (channel.LogicalChannelNumber.HasValue)
                {
                    channelNum = channel.LogicalChannelNumber.Value;
                }
            }
            else
            {
                channelNum = _channelList.IndexOf(tvGuideChannel) + 1;
            }

            GUIButton3PartControl img = GetControl(iChannel + (int)Controls.IMG_CHAN1) as GUIButton3PartControl;
            if (img != null)
            {
                if (_showChannelLogos)
                {
                    img.TexutureIcon = tvGuideChannel.strLogo;
                }
                if (channelNum > 0 && _showChannelNumber)
                {
                    img.Label1 = channelNum + " " + channel.DisplayName;
                }
                else
                {
                    img.Label1 = channel.DisplayName;
                }
                img.Data = channel;
                img.IsVisible = true;
            }

            IList<GuideProgramSummary> programs = new List<GuideProgramSummary>();
            if (_model.ProgramsByChannel.ContainsKey(channel.ChannelId))
            {
                programs = _model.ProgramsByChannel[channel.ChannelId].Programs;
            }

            bool noEPG = (programs == null || programs.Count == 0);
            if (noEPG)
            {
                DateTime dt = Utils.longtodate(iEnd);
                long iProgEnd = Utils.datetolong(dt);
                GuideProgramSummary prog = new GuideProgramSummary()
                {
                    GuideChannelId = channel.GuideChannelId.HasValue ? channel.GuideChannelId.Value : Guid.Empty,
                    StartTime = Utils.longtodate(iStart),
                    StopTime = Utils.longtodate(iProgEnd),
                    Title = Utility.GetLocalizedText(TextId.NoDataAvailable),
                    SubTitle = String.Empty,
                    Category = String.Empty
                };
                programs = new List<GuideProgramSummary>();
                programs.Add(prog);
            }

            int iProgram = 0;
            int iPreviousEndXPos = 0;

            int width = GetControl((int)Controls.LABEL_TIME1 + 1).XPosition;
            width -= GetControl((int)Controls.LABEL_TIME1).XPosition;

            int height = GetControl((int)Controls.IMG_CHAN1 + 1).YPosition;
            height -= GetControl((int)Controls.IMG_CHAN1).YPosition;

            foreach (GuideProgramSummary program in programs)
            {
                if (Utils.datetolong(program.StopTime) <= iStart
                    || Utils.datetolong(program.StartTime) >= iEnd)
                    continue;

                string strTitle = program.Title;
                bool bStartsBefore = false;
                bool bEndsAfter = false;

                if (Utils.datetolong(program.StartTime) < iStart)
                    bStartsBefore = true;

                if (Utils.datetolong(program.StopTime) > iEnd)
                    bEndsAfter = true;

                DateTime dtBlokStart = new DateTime();
                dtBlokStart = _viewingTime;
                dtBlokStart = dtBlokStart.AddMilliseconds(-dtBlokStart.Millisecond);
                dtBlokStart = dtBlokStart.AddSeconds(-dtBlokStart.Second);

                bool isAlert;
                bool isRecording;
                string recordIconImage = GetChannelProgramIcon(channel, program.GuideProgramId, out isRecording,out isAlert);

                if (noEPG && !isRecording)
                {
                    ActiveRecording rec;
                    isRecording = PluginMain.IsChannelRecording(channel.ChannelId, out rec);
                }

                bool bProgramIsHD = (program.Flags & GuideProgramFlags.HighDefinition) != 0;

                int iStartXPos = 0;
                int iEndXPos = 0;
                for (int iBlok = 0; iBlok < _numberOfBlocks; iBlok++)
                {
                    float fWidthEnd = (float)width;
                    DateTime dtBlokEnd = dtBlokStart.AddMinutes(_timePerBlock - 1);
                    if (IsRunningAt(program, dtBlokStart, dtBlokEnd))
                    {
                        //dtBlokEnd = dtBlokStart.AddSeconds(_timePerBlock * 60);
                        if (program.StopTime <= dtBlokEnd)
                        {
                            TimeSpan dtSpan = dtBlokEnd - program.StopTime;
                            int iEndMin = _timePerBlock - (dtSpan.Minutes);

                            fWidthEnd = (((float)iEndMin) / ((float)_timePerBlock)) * ((float)(width));
                            if (bEndsAfter)
                            {
                                fWidthEnd = (float)width;
                            }
                        }

                        if (iStartXPos == 0)
                        {
                            TimeSpan ts = program.StartTime - dtBlokStart;
                            int iStartMin = ts.Hours * 60;
                            iStartMin += ts.Minutes;
                            if (ts.Seconds == 59)
                            {
                                iStartMin += 1;
                            }
                            float fWidth = (((float)iStartMin) / ((float)_timePerBlock)) * ((float)(width));

                            if (bStartsBefore)
                            {
                                fWidth = 0;
                            }

                            iStartXPos = GetControl(iBlok + (int)Controls.LABEL_TIME1).XPosition;
                            iStartXPos += (int)fWidth;
                            iEndXPos = GetControl(iBlok + (int)Controls.LABEL_TIME1).XPosition + (int)fWidthEnd;
                        }
                        else
                        {
                            iEndXPos = GetControl(iBlok + (int)Controls.LABEL_TIME1).XPosition + (int)fWidthEnd;
                        }
                    }
                    dtBlokStart = dtBlokStart.AddMinutes(_timePerBlock);
                }

                if (iStartXPos >= 0)
                {
                    if (iPreviousEndXPos > iStartXPos)
                    {
                        iStartXPos = iPreviousEndXPos;
                    }
                    if (iEndXPos <= iStartXPos + 5)
                    {
                        iEndXPos = iStartXPos + 6; // at least 1 pixel width
                    }

                    int ypos = GetControl(iChannel + (int)Controls.IMG_CHAN1).YPosition;
                    int iControlId = GUIDE_COMPONENTID_START + iChannel * RowID + iProgram * ColID;
                    int iWidth = iEndXPos - iStartXPos;
                    if (iWidth > 3)
                    {
                        iWidth -= 3;
                    }
                    else
                    {
                        iWidth = 1;
                    }

                    string TexutureFocusLeftName = "tvguide_button_selected_left.png";
                    string TexutureFocusMidName = "tvguide_button_selected_middle.png";
                    string TexutureFocusRightName = "tvguide_button_selected_right.png";
                    string TexutureNoFocusLeftName = "tvguide_button_light_left.png";
                    string TexutureNoFocusMidName = "tvguide_button_light_middle.png";
                    string TexutureNoFocusRightName = "tvguide_button_light_right.png";

                    bool TileFillTFL = false;
                    bool TileFillTNFL = false;
                    bool TileFillTFM = false;
                    bool TileFillTNFM = false;
                    bool TileFillTFR = false;
                    bool TileFillTNFR = false;

                    if (_programNotRunningTemplate != null)
                    {
                        _programNotRunningTemplate.IsVisible = false;
                        TexutureFocusLeftName = _programNotRunningTemplate.TexutureFocusLeftName;
                        TexutureFocusMidName = _programNotRunningTemplate.TexutureFocusMidName;
                        TexutureFocusRightName = _programNotRunningTemplate.TexutureFocusRightName;
                        TexutureNoFocusLeftName = _programNotRunningTemplate.TexutureNoFocusLeftName;
                        TexutureNoFocusMidName = _programNotRunningTemplate.TexutureNoFocusMidName;
                        TexutureNoFocusRightName = _programNotRunningTemplate.TexutureNoFocusRightName;
                        TileFillTFL = _programNotRunningTemplate.TileFillTFL;
                        TileFillTNFL = _programNotRunningTemplate.TileFillTNFL;
                        TileFillTFM = _programNotRunningTemplate.TileFillTFM;
                        TileFillTNFM = _programNotRunningTemplate.TileFillTNFM;
                        TileFillTFR = _programNotRunningTemplate.TileFillTFR;
                        TileFillTNFR = _programNotRunningTemplate.TileFillTNFR;
                    }

                    img = GetControl(iControlId) as GUIButton3PartControl;
                    if (img == null)
                    {
                        img = new GUIButton3PartControl(GetID, iControlId, iStartXPos, ypos, iWidth, height - 2,
                                                        TexutureFocusLeftName,
                                                        TexutureFocusMidName,
                                                        TexutureFocusRightName,
                                                        TexutureNoFocusLeftName,
                                                        TexutureNoFocusMidName,
                                                        TexutureNoFocusRightName,
                                                        String.Empty);
                        img.AllocResources();
                        GUIControl cntl = (GUIControl)img;
                        Add(ref cntl);
                    }
                    else
                    {
                        img.Focus = false;
                        img.SetPosition(iStartXPos, ypos);
                        img.Width = iWidth;
                        img.IsVisible = true;
                        img.DoUpdate();
                    }

                    img.RenderLeft = false;
                    img.RenderRight = false;

                    img.TexutureIcon = String.Empty;
                    if (recordIconImage != null && isAlert)
                    {
                        if (_programNotifyTemplate != null)
                        {
                            _programNotifyTemplate.IsVisible = false;
                            TexutureFocusLeftName = _programNotifyTemplate.TexutureFocusLeftName;
                            TexutureFocusMidName = _programNotifyTemplate.TexutureFocusMidName;
                            TexutureFocusRightName = _programNotifyTemplate.TexutureFocusRightName;
                            TexutureNoFocusLeftName = _programNotifyTemplate.TexutureNoFocusLeftName;
                            TexutureNoFocusMidName = _programNotifyTemplate.TexutureNoFocusMidName;
                            TexutureNoFocusRightName = _programNotifyTemplate.TexutureNoFocusRightName;
                            TileFillTFL = _programNotifyTemplate.TileFillTFL;
                            TileFillTNFL = _programNotifyTemplate.TileFillTNFL;
                            TileFillTFM = _programNotifyTemplate.TileFillTFM;
                            TileFillTNFM = _programNotifyTemplate.TileFillTNFM;
                            TileFillTFR = _programNotifyTemplate.TileFillTFR;
                            TileFillTNFR = _programNotifyTemplate.TileFillTNFR;

                            // Use of the button template control implies use of the icon.  Use a blank image if the icon is not desired.
                            img.TexutureIcon = Thumbs.TvNotifyIcon;
                            img.IconOffsetX = _programNotifyTemplate.IconOffsetX;
                            img.IconOffsetY = _programNotifyTemplate.IconOffsetY;
                            img.IconAlign = _programNotifyTemplate.IconAlign;
                            img.IconVAlign = _programNotifyTemplate.IconVAlign;
                            img.IconInlineLabel1 = _programNotifyTemplate.IconInlineLabel1;
                        }
                        else
                        {
                            if (_useNewNotifyButtonColor)
                            {
                                TexutureFocusLeftName = "tvguide_notifyButton_Focus_left.png";
                                TexutureFocusMidName = "tvguide_notifyButton_Focus_middle.png";
                                TexutureFocusRightName = "tvguide_notifyButton_Focus_right.png";
                                TexutureNoFocusLeftName = "tvguide_notifyButton_noFocus_left.png";
                                TexutureNoFocusMidName = "tvguide_notifyButton_noFocus_middle.png";
                                TexutureNoFocusRightName = "tvguide_notifyButton_noFocus_right.png";
                            }
                            else
                            {
                                img.TexutureIcon = Thumbs.TvNotifyIcon;
                            }
                        }
                    }
                    if (recordIconImage != null)
                    {
                        // Select the partial recording template if needed.
                        //if (bPartialRecording)
                        //{
                        //    buttonRecordTemplate = _programPartialRecordTemplate;
                        //}

                        if (isRecording)
                        {
                            GUIButton3PartControl buttonRecordTemplate = _programRecordTemplate;
                            if (buttonRecordTemplate != null)
                            {
                                buttonRecordTemplate.IsVisible = false;
                                TexutureFocusLeftName = buttonRecordTemplate.TexutureFocusLeftName;
                                TexutureFocusMidName = buttonRecordTemplate.TexutureFocusMidName;
                                TexutureFocusRightName = buttonRecordTemplate.TexutureFocusRightName;
                                TexutureNoFocusLeftName = buttonRecordTemplate.TexutureNoFocusLeftName;
                                TexutureNoFocusMidName = buttonRecordTemplate.TexutureNoFocusMidName;
                                TexutureNoFocusRightName = buttonRecordTemplate.TexutureNoFocusRightName;
                                TileFillTFL = buttonRecordTemplate.TileFillTFL;
                                TileFillTNFL = buttonRecordTemplate.TileFillTNFL;
                                TileFillTFM = buttonRecordTemplate.TileFillTFM;
                                TileFillTNFM = buttonRecordTemplate.TileFillTNFM;
                                TileFillTFR = buttonRecordTemplate.TileFillTFR;
                                TileFillTNFR = buttonRecordTemplate.TileFillTNFR;

                                // Use of the button template control implies use of the icon.  Use a blank image if the icon is not desired.
                                //if (bConflict)
                                //{
                                //    TexutureFocusLeftName = "tvguide_recButton_Focus_left.png";
                                //    TexutureFocusMidName = "tvguide_recButton_Focus_middle.png";
                                //    TexutureFocusRightName = "tvguide_recButton_Focus_right.png";
                                //    TexutureNoFocusLeftName = "tvguide_recButton_noFocus_left.png";
                                //    TexutureNoFocusMidName = "tvguide_recButton_noFocus_middle.png";
                                //    TexutureNoFocusRightName = "tvguide_recButton_noFocus_right.png";
                                //}
                                img.IconOffsetX = buttonRecordTemplate.IconOffsetX;
                                img.IconOffsetY = buttonRecordTemplate.IconOffsetY;
                                img.IconAlign = buttonRecordTemplate.IconAlign;
                                img.IconVAlign = buttonRecordTemplate.IconVAlign;
                                img.IconInlineLabel1 = buttonRecordTemplate.IconInlineLabel1;
                            }
                            else
                            {
                                //if (bPartialRecording && _useNewPartialRecordingButtonColor)
                                //{
                                //    TexutureFocusLeftName = "tvguide_partRecButton_Focus_left.png";
                                //    TexutureFocusMidName = "tvguide_partRecButton_Focus_middle.png";
                                //    TexutureFocusRightName = "tvguide_partRecButton_Focus_right.png";
                                //    TexutureNoFocusLeftName = "tvguide_partRecButton_noFocus_left.png";
                                //    TexutureNoFocusMidName = "tvguide_partRecButton_noFocus_middle.png";
                                //    TexutureNoFocusRightName = "tvguide_partRecButton_noFocus_right.png";
                                //}
                                //else
                                {
                                    if (_useNewRecordingButtonColor)
                                    {
                                        TexutureFocusLeftName = "tvguide_recButton_Focus_left.png";
                                        TexutureFocusMidName = "tvguide_recButton_Focus_middle.png";
                                        TexutureFocusRightName = "tvguide_recButton_Focus_right.png";
                                        TexutureNoFocusLeftName = "tvguide_recButton_noFocus_left.png";
                                        TexutureNoFocusMidName = "tvguide_recButton_noFocus_middle.png";
                                        TexutureNoFocusRightName = "tvguide_recButton_noFocus_right.png";
                                    }
                                }
                            }
                        }
                        img.TexutureIcon = recordIconImage;
                    }

                    img.TexutureIcon2 = String.Empty;
                    if (bProgramIsHD)
                    {
                        if (_programNotRunningTemplate != null)
                        {
                            img.TexutureIcon2 = _programNotRunningTemplate.TexutureIcon2;
                        }
                        else
                        {
                            if (_useHdProgramIcon)
                            {
                                img.TexutureIcon2 = "tvguide_hd_program.png";
                            }
                        }
                        img.Icon2InlineLabel1 = true;
                        img.Icon2VAlign = GUIControl.VAlignment.ALIGN_MIDDLE;
                        img.Icon2OffsetX = 5;
                    }
                    img.Data = program;
                    img.ColourDiffuse = GetColorForGenre(program.Category);

                    iWidth = iEndXPos - iStartXPos;
                    if (iWidth > 10)
                    {
                        iWidth -= 10;
                    }
                    else
                    {
                        iWidth = 1;
                    }

                    DateTime dt = DateTime.Now;

                    img.TextOffsetX1 = 5;
                    img.TextOffsetY1 = 5;
                    img.FontName1 = "font13";
                    img.TextColor1 = 0xffffffff;
                    img.Label1 = strTitle;
                    GUILabelControl labelTemplate;
                    if (IsRunningAt(program, dt))
                    {
                        labelTemplate = _titleDarkTemplate;
                    }
                    else
                    {
                        labelTemplate = _titleTemplate;
                    }

                    if (labelTemplate != null)
                    {
                        img.FontName1 = labelTemplate.FontName;
                        img.TextColor1 = labelTemplate.TextColor;
                        img.TextColor2 = labelTemplate.TextColor;
                        img.TextOffsetX1 = labelTemplate.XPosition;
                        img.TextOffsetY1 = labelTemplate.YPosition;
                        img.SetShadow1(labelTemplate.ShadowAngle, labelTemplate.ShadowDistance, labelTemplate.ShadowColor);

                        // This is a legacy behavior check.  Adding labelTemplate.XPosition and labelTemplate.YPosition requires
                        // skinners to add these values to the skin xml unless this check exists.  Perform a sanity check on the
                        // x,y position to ensure it falls into the bounds of the button.  If it does not then fall back to use the
                        // legacy values.  This check is necessary because the x,y position (without skin file changes) will be taken
                        // from either the references.xml control template or the controls coded defaults.
                        if (img.TextOffsetY1 > img.Height)
                        {
                            // Set legacy values.
                            img.TextOffsetX1 = 5;
                            img.TextOffsetY1 = 5;
                        }
                    }
                    img.TextOffsetX2 = 5;
                    img.TextOffsetY2 = img.Height / 2;
                    img.FontName2 = "font13";
                    img.TextColor2 = 0xffffffff;

                    if (IsRunningAt(program, dt))
                    {
                        labelTemplate = _genreDarkTemplate;
                    }
                    else
                    {
                        labelTemplate = _genreTemplate;
                    }
                    if (labelTemplate != null)
                    {
                        img.FontName2 = labelTemplate.FontName;
                        img.TextColor2 = labelTemplate.TextColor;
                        img.Label2 = program.Category;
                        img.TextOffsetX2 = labelTemplate.XPosition;
                        img.TextOffsetY2 = labelTemplate.YPosition;
                        img.SetShadow2(labelTemplate.ShadowAngle, labelTemplate.ShadowDistance, labelTemplate.ShadowColor);

                        // This is a legacy behavior check.  Adding labelTemplate.XPosition and labelTemplate.YPosition requires
                        // skinners to add these values to the skin xml unless this check exists.  Perform a sanity check on the
                        // x,y position to ensure it falls into the bounds of the button.  If it does not then fall back to use the
                        // legacy values.  This check is necessary because the x,y position (without skin file changes) will be taken
                        // from either the references.xml control template or the controls coded defaults.
                        if (img.TextOffsetY2 > img.Height)
                        {
                            // Set legacy values.
                            img.TextOffsetX2 = 5;
                            img.TextOffsetY2 = 5;
                        }
                    }

                    if (IsRunningAt(program, dt))
                    {
                        GUIButton3PartControl buttonRunningTemplate = _programRunningTemplate;
                        if (!isRecording && !isAlert && buttonRunningTemplate != null)
                        {
                            buttonRunningTemplate.IsVisible = false;
                            TexutureFocusLeftName = buttonRunningTemplate.TexutureFocusLeftName;
                            TexutureFocusMidName = buttonRunningTemplate.TexutureFocusMidName;
                            TexutureFocusRightName = buttonRunningTemplate.TexutureFocusRightName;
                            TexutureNoFocusLeftName = buttonRunningTemplate.TexutureNoFocusLeftName;
                            TexutureNoFocusMidName = buttonRunningTemplate.TexutureNoFocusMidName;
                            TexutureNoFocusRightName = buttonRunningTemplate.TexutureNoFocusRightName;
                            TileFillTFL = buttonRunningTemplate.TileFillTFL;
                            TileFillTNFL = buttonRunningTemplate.TileFillTNFL;
                            TileFillTFM = buttonRunningTemplate.TileFillTFM;
                            TileFillTNFM = buttonRunningTemplate.TileFillTNFM;
                            TileFillTFR = buttonRunningTemplate.TileFillTFR;
                            TileFillTNFR = buttonRunningTemplate.TileFillTNFR;
                        }
                        else if (isRecording && _useNewRecordingButtonColor)
                        {
                            TexutureFocusLeftName = "tvguide_recButton_Focus_left.png";
                            TexutureFocusMidName = "tvguide_recButton_Focus_middle.png";
                            TexutureFocusRightName = "tvguide_recButton_Focus_right.png";
                            TexutureNoFocusLeftName = "tvguide_recButton_noFocus_left.png";
                            TexutureNoFocusMidName = "tvguide_recButton_noFocus_middle.png";
                            TexutureNoFocusRightName = "tvguide_recButton_noFocus_right.png";
                        }
                        else if (isAlert && _useNewRecordingButtonColor)
                        {
                            TexutureFocusLeftName = "tvguide_notifyButton_Focus_left.png";
                            TexutureFocusMidName = "tvguide_notifyButton_Focus_middle.png";
                            TexutureFocusRightName = "tvguide_notifyButton_Focus_right.png";
                            TexutureNoFocusLeftName = "tvguide_notifyButton_noFocus_left.png";
                            TexutureNoFocusMidName = "tvguide_notifyButton_noFocus_middle.png";
                            TexutureNoFocusRightName = "tvguide_notifyButton_noFocus_right.png";
                        }
                        else
                        {
                            TexutureFocusLeftName = "tvguide_button_selected_left.png";
                            TexutureFocusMidName = "tvguide_button_selected_middle.png";
                            TexutureFocusRightName = "tvguide_button_selected_right.png";
                            TexutureNoFocusLeftName = "tvguide_button_left.png";
                            TexutureNoFocusMidName = "tvguide_button_middle.png";
                            TexutureNoFocusRightName = "tvguide_button_right.png";
                        }

                        if (selectCurrentShow && iChannel == _cursorX)
                        {
                            _cursorY = iProgram + 1;
                            _currentProgram = tvGuideAgent.GetProgramById(program.GuideProgramId);
                            m_dtStartTime = program.StartTime;
                            SetProperties();
                        }
                    }

                    if (bEndsAfter)
                    {
                        img.RenderRight = true;

                        TexutureFocusRightName = "tvguide_arrow_selected_right.png";
                        TexutureNoFocusRightName = "tvguide_arrow_light_right.png";
                        if (IsRunningAt(program, dt))
                        {
                            TexutureNoFocusRightName = "tvguide_arrow_right.png";
                        }
                    }
                    if (bStartsBefore)
                    {
                        img.RenderLeft = true;
                        TexutureFocusLeftName = "tvguide_arrow_selected_left.png";
                        TexutureNoFocusLeftName = "tvguide_arrow_light_left.png";
                        if (IsRunningAt(program, dt))
                        {
                            TexutureNoFocusLeftName = "tvguide_arrow_left.png";
                        }
                    }

                    img.TexutureFocusLeftName = TexutureFocusLeftName;
                    img.TexutureFocusMidName = TexutureFocusMidName;
                    img.TexutureFocusRightName = TexutureFocusRightName;
                    img.TexutureNoFocusLeftName = TexutureNoFocusLeftName;
                    img.TexutureNoFocusMidName = TexutureNoFocusMidName;
                    img.TexutureNoFocusRightName = TexutureNoFocusRightName;

                    img.TileFillTFL = TileFillTFL;
                    img.TileFillTNFL = TileFillTNFL;
                    img.TileFillTFM = TileFillTFM;
                    img.TileFillTNFM = TileFillTNFM;
                    img.TileFillTFR = TileFillTFR;
                    img.TileFillTNFR = TileFillTNFR;

                    iProgram++;
                }
                iPreviousEndXPos = iEndXPos;
            }
        }

        private int ProgramCount(int iChannel)
        {
            int iProgramCount = 0;
            for (int iProgram = 0; iProgram < _numberOfBlocks * 5; ++iProgram)
            {
                int iControlId = GUIDE_COMPONENTID_START + iChannel * RowID + iProgram * ColID;
                GUIControl cntl = GetControl(iControlId);
                if (cntl != null && cntl.IsVisible)
                {
                    iProgramCount++;
                }
                else
                {
                    return iProgramCount;
                }
            }
            return iProgramCount;
        }

        private void OnDown(bool updateScreen)
        {
            if (updateScreen)
            {
                UnFocus();
            }
            if (_cursorX < 0)
            {
                _cursorY = 0;
                _cursorX = 0;
                if (updateScreen)
                {
                    SetFocus();
                    GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL).Focus = false;
                }
                return;
            }

            if (_singleChannelView)
            {
                if (_cursorX + 1 < _channelCount)
                {
                    _cursorX++;
                }
                else
                {
                    if (_cursorX + _programOffset + 1 < _totalProgramCount)
                    {
                        _programOffset++;
                    }
                }
                if (updateScreen)
                {
                    Update(false);
                    SetFocus();
                    UpdateCurrentProgram(false);
                    SetProperties();
                }
                return;
            }

            if (_cursorY == 0)
            {
                MoveDown();

                if (updateScreen)
                {
                    Update(false);
                    SetFocus();
                    SetProperties();
                }
                return;
            }

            // not on tvguide button
            if (_cursorY > 0)
            {
                // if cursor is on a program in guide, try to find the "best time matching" program in new channel
                SetBestMatchingProgram(updateScreen, true);
            }
        }

        private void MoveDown()
        {
            // Move the cursor only if there are more channels in the view.
            if (_cursorX + 1 < Math.Min(_channelList.Count, _channelCount))
            {
                _cursorX++;
                _lastCommandTime = AnimationTimer.TickCount;
            }
            else
            {
                // reached end of screen
                // more channels than rows?
                if (_channelList.Count > _channelCount)
                {
                    // Guide may be allowed to loop continuously bottom to top.
                    if (_guideContinuousScroll)
                    {
                        // We're at the bottom of the last page of channels.
                        if (_channelOffset >= _channelList.Count)
                        {
                            // Position to first channel in guide without moving the cursor (implements continuous loops of channels).
                            _channelOffset = 0;
                        }
                        else
                        {
                            // Advance to next channel, wrap around if at end of list.
                            _channelOffset++;
                            if (_channelOffset >= _channelList.Count)
                            {
                                _channelOffset = 0;
                            }
                        }
                    }
                    else
                    {
                        // Are we at the bottom of the lst page of channels?
                        if (_channelOffset > 0 && _channelOffset >= (_channelList.Count - 1) - _cursorX)
                        {
                            // We're at the bottom of the last page of channels.
                            // Reposition the guide to the top only after the key/button has been released and pressed again.
                            if ((AnimationTimer.TickCount - _lastCommandTime) > _loopDelay)
                            {
                                _channelOffset = 0;
                                _cursorX = 0;
                                _lastCommandTime = AnimationTimer.TickCount;
                            }
                        }
                        else
                        {
                            // Advance to next channel.
                            _channelOffset++;
                            _lastCommandTime = AnimationTimer.TickCount;
                        }
                    }
                }
                else if ((AnimationTimer.TickCount - _lastCommandTime) > _loopDelay)
                {
                    // Move the highlight back to the top of the list only after the key/button has been released and pressed again.
                    _cursorX = 0;
                    _lastCommandTime = AnimationTimer.TickCount;
                }
            }
        }

        private void OnUp(bool updateScreen, bool isPaging)
        {
            if (updateScreen)
            {
                UnFocus();
            }

            if (_singleChannelView)
            {
                if (_cursorX == 0 && _cursorY == 0 && !isPaging)
                {
                    // Don't focus the control when it is not visible.
                    if (GetControl((int)Controls.SPINCONTROL_DAY).IsVisible)
                    {
                        _cursorX = -1;
                        GetControl((int)Controls.SPINCONTROL_DAY).Focus = true;
                    }
                    return;
                }

                if (_cursorX > 0)
                {
                    _cursorX--;
                }
                else if (_programOffset > 0)
                {
                    _programOffset--;
                }

                if (updateScreen)
                {
                    Update(false);
                    SetFocus();
                    UpdateCurrentProgram(false);
                    SetProperties();
                }
                return;
            }
            else
            {
                if (_cursorY == -1)
                {
                    _cursorX = -1;
                    _cursorY = 0;
                    GetControl((int)Controls.CHANNEL_GROUP_BUTTON).Focus = false;
                    GetControl((int)Controls.SPINCONTROL_DAY).Focus = true;
                    return;
                }

                if (_cursorY == 0 && _cursorX == 0 && !isPaging)
                {
                    // Only focus the control if it is visible.
                    if (GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL).Visible)
                    {
                        _cursorX = -1;
                        GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL).Focus = true;
                        return;
                    }
                }
            }

            if (_cursorY == 0)
            {
                if (_cursorX == 0)
                {
                    if (_channelOffset > 0)
                    {
                        // Somewhere in the middle of the guide; just scroll up.
                        _channelOffset--;
                    }
                    else if (_channelOffset == 0)
                    {
                        // We're at the top of the first page of channels.
                        // Reposition the guide to the bottom only after the key/button has been released and pressed again.
                        if ((AnimationTimer.TickCount - _lastCommandTime) > _loopDelay)
                        {
                            if (_channelList.Count > _channelCount)
                            {
                                _channelOffset = _channelList.Count - _channelCount;
                                _cursorX = _channelCount - 1;
                            }
                            else
                            {
                                _channelOffset = 0;
                                _cursorX = _channelList.Count - 1;
                            }
                        }
                    }
                }
                else if (_cursorX > 0)
                {
                    _cursorX--;
                }

                if (updateScreen)
                {
                    Update(false);
                    SetFocus();
                    SetProperties();
                }
            }

            // not on tvguide button
            if (_cursorY > 0)
            {
                // if cursor is on a program in guide, try to find the "best time matching" program in new channel
                SetBestMatchingProgram(updateScreen, false);
            }
        }

        private void MoveUp()
        {
            if (_cursorX == 0)
            {
                if (_guideContinuousScroll)
                {
                    if (_channelOffset == 0 && _channelList.Count > _channelCount)
                    {
                        // We're at the top of the first page of channels.  Position to last channel in guide.
                        _channelOffset = _channelList.Count - 1;
                    }
                    else if (_channelOffset > 0)
                    {
                        // Somewhere in the middle of the guide; just scroll up.
                        _channelOffset--;
                    }
                }
                else
                {
                    if (_channelOffset > 0)
                    {
                        // Somewhere in the middle of the guide; just scroll up.
                        _channelOffset--;
                        _lastCommandTime = AnimationTimer.TickCount;
                    }
                    // Are we at the top of the first page of channels?
                    else if (_channelOffset == 0 && _cursorX == 0)
                    {
                        // We're at the top of the first page of channels.
                        // Reposition the guide to the bottom only after the key/button has been released and pressed again.
                        if ((AnimationTimer.TickCount - _lastCommandTime) > _loopDelay)
                        {
                            if (_channelList.Count > _channelCount)
                            {
                                _channelOffset = _channelList.Count - _channelCount;
                                _cursorX = _channelCount - 1;
                            }
                            else
                            {
                                _channelOffset = 0;
                                _cursorX = _channelList.Count - 1;
                            }
                            _lastCommandTime = AnimationTimer.TickCount;
                        }
                    }
                }
            }
            else
            {
                _cursorX--;
                _lastCommandTime = AnimationTimer.TickCount;
            }
        }

        /// <summary>
        /// Sets the best matching program in new guide row
        /// </summary>
        /// <param name="updateScreen"></param>
        private void SetBestMatchingProgram(bool updateScreen, bool DirectionIsDown)
        {
            // if cursor is on a program in guide, try to find the "best time matching" program in new channel
            int iCurY = _cursorX;
            int iCurOff = _channelOffset;
            int iX1, iX2;
            int iControlId = GUIDE_COMPONENTID_START + _cursorX * RowID + (_cursorY - 1) * ColID;
            GUIControl control = GetControl(iControlId);
            if (control == null)
            {
                return;
            }
            iX1 = control.XPosition;
            iX2 = control.XPosition + control.Width;

            bool bOK = false;
            int iMaxSearch = _channelList.Count;

            // TODO rewrite the while loop, the code is a little awkward.
            while (!bOK && (iMaxSearch > 0))
            {
                iMaxSearch--;
                if (DirectionIsDown == true)
                {
                    MoveDown();
                }
                else // Direction "Up"
                {
                    MoveUp();
                }
                if (updateScreen)
                {
                    Update(false);
                }

                for (int x = 1; x < ColID; x++)
                {
                    iControlId = GUIDE_COMPONENTID_START + _cursorX * RowID + (x - 1) * ColID;
                    control = GetControl(iControlId);
                    if (control != null)
                    {
                        GuideProgramSummary prog = (GuideProgramSummary)control.Data;

                        if (_singleChannelView)
                        {
                            _cursorY = x;
                            bOK = true;
                            break;
                        }

                        bool isvalid = false;
                        DateTime time = DateTime.Now;
                        if (time < prog.StopTime) // present & future
                        {
                            if (m_dtStartTime <= prog.StartTime)
                            {
                                isvalid = true;
                            }
                            else if (m_dtStartTime >= prog.StartTime && m_dtStartTime < prog.StopTime)
                            {
                                isvalid = true;
                            }
                            else if (m_dtStartTime < time)
                            {
                                isvalid = true;
                            }
                        }
                        // this one will skip past programs
                        else if (_currentProgram != null && time > _currentProgram.StopTime) // history
                        {
                            if (prog.StopTime > m_dtStartTime)
                            {
                                isvalid = true;
                            }
                        }

                        if (isvalid)
                        {
                            _cursorY = x;
                            bOK = true;
                            break;
                        }
                    }
                }
            }
            if (!bOK)
            {
                _cursorX = iCurY;
                _channelOffset = iCurOff;
            }
            if (updateScreen)
            {
                Correct();
                if (iCurOff == _channelOffset)
                {
                    UpdateCurrentProgram(false);
                    return;
                }
                SetFocus();
            }
        }

        private void OnLeft()
        {
            if (_cursorX < 0)
            {
                return;
            }
            UnFocus();
            if (_cursorY <= 0)
            {
                // custom focus handling only if button available
                if (MinYIndex == -1)
                {
                    _cursorY--; // decrease by 1,
                    if (_cursorY == -1) // means tvgroup entered (-1) or moved left (-2)
                    {
                        SetFocus();
                        return;
                    }
                }
                _viewingTime = _viewingTime.AddMinutes(-_timePerBlock);
                // Check new day
                int iDay = CalcDays();
                if (iDay < 0)
                {
                    _viewingTime = _viewingTime.AddMinutes(+_timePerBlock);
                }
            }
            else
            {
                if (_cursorY == 1)
                {
                    _cursorY = 0;

                    SetFocus();
                    SetProperties();
                    return;
                }
                _cursorY--;
                Correct();
                UpdateCurrentProgram(false);
                if (_currentProgram != null)
                {
                    m_dtStartTime = _currentProgram.StartTime;
                }
                return;
            }
            Correct();
            Update(false);
            SetFocus();
            if (_currentProgram != null)
            {
                m_dtStartTime = _currentProgram.StartTime;
            }
        }
        
        private void UpdateCurrentProgram(bool updateIcon)
        {
            if (_cursorX < 0)
            {
                return;
            }
            if (_cursorY < 0)
            {
                return;
            }
            if (_cursorY == 0)
            {
                SetProperties();
                SetFocus();
                return;
            }
            int iControlId = GUIDE_COMPONENTID_START + _cursorX * RowID + (_cursorY - 1) * ColID;
            GUIButton3PartControl img = GetControl(iControlId) as GUIButton3PartControl;
            if (null != img)
            {
                SetFocus();
                using (GuideServiceAgent tvGuideAgent = new GuideServiceAgent())
                {
                    _currentProgram = tvGuideAgent.GetProgramById(((GuideProgramSummary)img.Data).GuideProgramId);
                }
                if (updateIcon)
                {
                    bool isRecording;
                    bool isAlert;
                    string recordIconImage = GetChannelProgramIcon(_currentChannel, _currentProgram.GuideProgramId, out isRecording, out isAlert);
                    img.TexutureIcon = recordIconImage == null ? String.Empty : recordIconImage;
                }
                SetProperties();
            }
        }

        /// <summary>
        /// Show or hide group button
        /// </summary>
        protected void UpdateGroupButton()
        {
            // text for button
            String GroupButtonText = " ";

            // show/hide tvgroup button
            GUIButtonControl btnTvGroup = GetControl((int)Controls.CHANNEL_GROUP_BUTTON) as GUIButtonControl;

            if (btnTvGroup != null)
                btnTvGroup.Visible = GroupButtonAvail;

            // set min index for focus handling
            if (GroupButtonAvail)
            {
                MinYIndex = -1; // allow focus of button
                GroupButtonText = String.Format("{0}: {1}", GUILocalizeStrings.Get(971), PluginMain.Navigator.CurrentGroup.GroupName);
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Group", PluginMain.Navigator.CurrentGroup.GroupName);
            }
            else
            {
                GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.Group", PluginMain.Navigator.CurrentGroup.GroupName);
                MinYIndex = 0;
            }

            // Set proper text for group change button; Empty string to hide text if only 1 group
            // (split between button and rotated label due to focusing issue of rotated buttons)
            GUIPropertyManager.SetProperty(SkinPropertyPrefix + ".Guide.ChangeGroup", GroupButtonText); // existing string "group"
        }

        private void OnRight()
        {
            if (_cursorX < 0)
            {
                return;
            }
            UnFocus();
            if (_cursorY < ProgramCount(_cursorX))
            {
                _cursorY++;
                Correct();
                UpdateCurrentProgram(false);
                if (_currentProgram != null)
                {
                    m_dtStartTime = _currentProgram.StartTime;
                }
                return;
            }
            else
            {
                _viewingTime = _viewingTime.AddMinutes(_timePerBlock);
                // Check new day
                int iDay = CalcDays();
                if (iDay >= MaxDaysInGuide)
                {
                    _viewingTime = _viewingTime.AddMinutes(-_timePerBlock);
                }
            }
            Correct();
            Update(false);
            SetFocus();
            if (_currentProgram != null)
            {
                m_dtStartTime = _currentProgram.StartTime;
            }
        }

        private void updateSingleChannelNumber()
        {
            // update selected channel
            if (!_singleChannelView)
            {
                _singleChannelNumber = _cursorX + _channelOffset;
                if (_singleChannelNumber < 0)
                {
                    _singleChannelNumber = 0;
                }
                if (_singleChannelNumber >= _channelList.Count)
                {
                    _singleChannelNumber -= _channelList.Count;
                }
                // instead of direct casting us "as"; else it fails for other controls!
                GUIButton3PartControl img = GetControl(_cursorX + (int)Controls.IMG_CHAN1) as GUIButton3PartControl;
                ;
                if (null != img)
                {
                    _currentChannel = (Channel)img.Data;
                }
            }
        }

        private void UnFocus()
        {
            if (_cursorX < 0)
            {
                return;
            }
            if (_cursorY == 0 || _cursorY == MinYIndex) // either channel or group button
            {
                int controlid = (int)Controls.IMG_CHAN1 + _cursorX;
                GUIControl.UnfocusControl(GetID, controlid);
            }
            else
            {
                Correct();
                int iControlId = GUIDE_COMPONENTID_START + _cursorX * RowID + (_cursorY - 1) * ColID;
                GUIButton3PartControl img = GetControl(iControlId) as GUIButton3PartControl;
                if (null != img && img.IsVisible)
                {
                    if (_currentProgram != null)
                    {
                        img.ColourDiffuse = GetColorForGenre(_currentProgram.Category);
                    }
                }
                GUIControl.UnfocusControl(GetID, iControlId);
            }
        }

        private void SetFocus()
        {
            if (_cursorX < 0)
            {
                return;
            }
            if (_cursorY == 0 || _cursorY == MinYIndex) // either channel or group button
            {
                int controlid;
                GUIControl.UnfocusControl(GetID, (int)Controls.SPINCONTROL_DAY);
                GUIControl.UnfocusControl(GetID, (int)Controls.SPINCONTROL_TIME_INTERVAL);

                if (_cursorY == -1)
                    controlid = (int)Controls.CHANNEL_GROUP_BUTTON;
                else
                    controlid = (int)Controls.IMG_CHAN1 + _cursorX;

                GUIControl.FocusControl(GetID, controlid);
            }
            else
            {
                Correct();
                int iControlId = GUIDE_COMPONENTID_START + _cursorX * RowID + (_cursorY - 1) * ColID;
                GUIButton3PartControl img = GetControl(iControlId) as GUIButton3PartControl;
                if (null != img && img.IsVisible)
                {
                    img.ColourDiffuse = 0xffffffff;
                    using (GuideServiceAgent tvGuideAgent = new GuideServiceAgent())
                    {
                        _currentProgram = tvGuideAgent.GetProgramById(((GuideProgramSummary)img.Data).GuideProgramId);
                    }
                    SetProperties();
                }
                GUIControl.FocusControl(GetID, iControlId);
            }
        }

        private void Correct()
        {
            int iControlId;
            if (_cursorY < MinYIndex) // either channel or group button
            {
                _cursorY = MinYIndex;
            }
            if (_cursorY > 0)
            {
                while (_cursorY > 0)
                {
                    iControlId = GUIDE_COMPONENTID_START + _cursorX * RowID + (_cursorY - 1) * ColID;
                    GUIControl cntl = GetControl(iControlId);
                    if (cntl == null)
                    {
                        _cursorY--;
                    }
                    else if (!cntl.IsVisible)
                    {
                        _cursorY--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (_cursorX < 0)
            {
                _cursorX = 0;
            }
            if (!_singleChannelView)
            {
                while (_cursorX > 0)
                {
                    iControlId = GUIDE_COMPONENTID_START + _cursorX * RowID + (0) * ColID;
                    GUIControl cntl = GetControl(iControlId);
                    if (cntl == null)
                    {
                        _cursorX--;
                    }
                    else if (!cntl.IsVisible)
                    {
                        _cursorX--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void ShowContextMenu()
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            {
                dlg.Reset();
                dlg.SetHeading(GUILocalizeStrings.Get(924)); //Menu

                bool isRecording = false;
                bool isAlert = false;
                ActiveRecording activeRecording = null;

                if (_currentChannel != null)
                {
                    dlg.AddLocalizedString(938); // View this channel

                    if (_currentProgram != null)
                    {
                        dlg.AddLocalizedString(1020); // Information
                        string recordIconImage = GetChannelProgramIcon(_currentChannel, _currentProgram.GuideProgramId, out isRecording, out isAlert);
                    }
                }

                if (_currentProgram != null && _currentProgram.StartTime > DateTime.Now)
                {
                    if (isAlert)
                    {
                        dlg.AddLocalizedString(1212); // cancel reminder
                    }
                    else if (!isRecording)
                    {
                        dlg.AddLocalizedString(1040); // set reminder
                    }
                }

                if (_currentProgram != null && _currentChannel != null)
                {
                    if (PluginMain.IsActiveRecording(_currentChannel.ChannelId, _currentProgram, out activeRecording))
                    {
                        dlg.AddLocalizedString(1449); // Abort active recording
                    }
                    else if (isRecording)
                    {
                        dlg.AddLocalizedString(610); // don't record
                    }
                    else
                    {
                        dlg.AddLocalizedString(264); // Record
                    }
                }

                if (PluginMain.Navigator.GetGroups(_channelType).Count > 1)
                {
                    dlg.AddLocalizedString(971); // Group
                }

                dlg.AddLocalizedString(939); // Switch mode
                dlg.AddLocalizedString(368); // IMDB

                dlg.DoModal(GetID);
                if (dlg.SelectedLabel == -1)
                {
                    return;
                }
                switch (dlg.SelectedId)
                {
                    case 368: // IMDB
                        OnGetIMDBInfo();
                        break;

                    case 971: //group
                        OnSelectChannelGroup();
                        //dlg.Reset();
                        //dlg.SetHeading(GUILocalizeStrings.Get(971));//Group
                        //foreach (ChannelGroup group in TVHome.Navigator.Groups)
                        //{
                        //  dlg.Add(group.GroupName);
                        //}
                        //dlg.DoModal(GetID);
                        //if (dlg.SelectedLabel == -1) return;
                        //TVHome.Navigator.SetCurrentGroup(dlg.SelectedLabelText);

                        GetChannels(true);
                        Update(false);
                        SetFocus();
                        break;

                    case 938: // view channel
                        PluginMain.Navigator.ZapToChannel(_currentChannel, false);
                        break;

                    case 939: // switch mode
                        OnSwitchMode();
                        break;

                    case 1020: // information
                        ShowProgramInfo();
                        break;

                    case 610: // don't record
                    case 264: // record
                        OnRecordOrAlert(ScheduleType.Recording);
                        break;

                    case 1212: // don't notify
                    case 1040: // notify
                        OnRecordOrAlert(ScheduleType.Alert);
                        break;

                    case 1449:
                        //TODO: put this somewhere else
                        GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        if (dlgYesNo != null)
                        {
                            dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.StopRecording));
                            dlgYesNo.SetLine(1, _currentProgram.Title);
                            dlgYesNo.SetLine(2, string.Empty);
                            dlgYesNo.SetLine(3, string.Empty);
                            dlgYesNo.SetDefaultToYes(false);
                            dlgYesNo.DoModal(GetID);

                            if (dlgYesNo.IsConfirmed && activeRecording != null)
                            {
                                using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                                {
                                    tvControlAgent.AbortActiveRecording(activeRecording);
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void OnSelectChannelGroup()
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg != null)
            { 
                dlg.Reset();
                dlg.SetHeading(Utility.GetLocalizedText(TextId.ChangeChannelGroup));
                int count = 0;
                foreach (ChannelGroup group in _model.ChannelGroups)
                {
                    if (group.VisibleInGuide)
                    {
                        dlg.Add(group.GroupName);
                        if (group.ChannelGroupId == _model.CurrentChannelGroupId)
                        {
                            dlg.SelectedLabel = count;
                        }
                        count++;
                    }
                }
                dlg.DoModal(GetID);
                if (dlg.SelectedId > 0)
                {
                    PluginMain.Navigator.SetCurrentGroup(_model.ChannelType, _model.ChannelGroups[dlg.SelectedId - 1]);
                    _controller.SetChannelGroup(PluginMain.Navigator.CurrentGroup.ChannelGroupId);
                    GetChannels(true);
                    Update(false);

                    _cursorY = 1;//set cursor to programm
                    _channelOffset = 0;
                    _cursorX = 0; // set to top, otherwise index could be out of range in new group
                    SetFocus();
                }
            }
        }

        private void OnSwitchMode()
        {
            UnFocus();
            _singleChannelView = !_singleChannelView;
            if (_singleChannelView)
            {
                _backupCursorX = _cursorY;
                _backupCursorY = _cursorX;
                _backupChannelOffset = _channelOffset;

                _programOffset = _cursorY = _cursorX = 0;
                _recalculateProgramOffset = true;
            }
            else
            {
                //focus current channel
                _cursorY = 0;
                _cursorX = _backupCursorY;
                _channelOffset = _backupChannelOffset;
            }
            Update(true);
            SetFocus();
        }

        private void ShowProgramInfo()
        {
            Channel channel = _currentChannel;
            if (_singleChannelView)
            {
                channel = _channelList[_singleChannelNumber].channel;
            }
            if (channel != null
                && _currentProgram != null)
            {
                TvProgramInfo.Channel = channel;
                TvProgramInfo.CurrentProgram = _currentProgram;
                GUIWindowManager.ActivateWindow((int)WindowId.ProgramInfo);
            }
        }

        private void OnGetIMDBInfo()
        {
            IMDBMovie movieDetails = new IMDBMovie();
            movieDetails.SearchString = _currentProgram.Title;
            if (IMDBFetcher.GetInfoFromIMDB(this, ref movieDetails, true, false))
            {
                //TvBusinessLayer dbLayer = new TvBusinessLayer();

                //IList<Program> progs = dbLayer.GetProgramExists(Channel.Retrieve(_currentProgram.IdChannel),
                //                                                _currentProgram.StartTime, _currentProgram.EndTime);
                //if (progs != null && progs.Count > 0)
                //{
                //    Program prog = (Program)progs[0];
                //    prog.Description = movieDetails.Plot;
                //    prog.Genre = movieDetails.Genre;
                //    prog.StarRating = (int)movieDetails.Rating;
                //    prog.Persist();
                //}

                global::MediaPortal.GUI.Video.GUIVideoInfo videoInfo = (global::MediaPortal.GUI.Video.GUIVideoInfo)GUIWindowManager.GetWindow((int)Window.WINDOW_VIDEO_INFO);
                videoInfo.Movie = movieDetails;
                GUIButtonControl btnPlay = (GUIButtonControl)videoInfo.GetControl(2);
                btnPlay.Visible = false;
                GUIWindowManager.ActivateWindow((int)Window.WINDOW_VIDEO_INFO);
            }
            else
            {
                Log.Info("IMDB Fetcher: Nothing found");
            }
        }

        private bool OnSelectItem(bool isItemSelected)
        {
            if (_currentProgram == null)
            {
                if (DateTime.Now < _viewingTime.AddMinutes(_timePerBlock * 4) && DateTime.Now > _viewingTime)
                {
                    DoViewThisChannel();
                }
                return true;
            }

            if (_currentProgram.StartTime <= DateTime.Now
                && _currentProgram.StopTime > DateTime.Now)
            {
                ActiveRecording activeRecording;
                if (PluginMain.IsActiveRecording(_currentChannel.ChannelId, _currentProgram, out activeRecording))
                {
                    Log.Info("TVGuide: clicked on a currently running recording");
                    GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
                    if (dlg == null)
                    {
                        return true;
                    }

                    dlg.Reset();
                    dlg.SetHeading(_currentProgram.Title);
                    dlg.AddLocalizedString(979); //Play recording from beginning
                    dlg.AddLocalizedString(938); //View this channel
                    dlg.DoModal(GetID);

                    if (dlg.SelectedLabel == -1)
                        return true;

                    switch (dlg.SelectedId)
                    {
                        case 979: // Play recording from beginning 
                            using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                            {
                                Recording recording = tvControlAgent.GetRecordingById(activeRecording.RecordingId);
                                if (recording != null)
                                {
                                    RecordedBase.PlayFromPreRecPoint(recording);
                                }
                            }
                            break;

                        case 938: //View this channel
                            DoViewThisChannel();
                            break;
                    }
                }
                else
                {
                    DoViewThisChannel();
                }
            }
            else
            {
                ShowProgramInfo();
                return false;
            }
            return true;
        }

        private void DoViewThisChannel()
        {
            if (g_Player.Playing && g_Player.IsTVRecording)
            {
                g_Player.Stop(true);
            }
            try
            {
            	PluginMain.Navigator.ZapToChannel(_currentChannel, false);
            }
            finally
            {
                if (VMR9Util.g_vmr9 != null)
                {
                    VMR9Util.g_vmr9.Enable(true);
                }
            }
        }

        /// <summary>
        /// "Record" via REC button
        /// </summary>
        private void OnRecordOrAlert(ScheduleType scheduleType)
        {
            Channel channel = _currentChannel;
            if (_singleChannelView)
            {
                channel = _channelList[_singleChannelNumber].channel;
            }
            if (channel != null
                && _currentProgram != null)
            {
                if (TvProgramInfo.RecordProgram(channel, _currentProgram, scheduleType, true))
                {
                    LoadSchedules(true);
                    UpdateCurrentProgram(true);
                    UnFocus();
                    Update(false);
                    SetFocus();
                }
            }
        }

        /// <summary>
        /// "Record" entry in context menu
        /// </summary>
        private void OnRecordContext()
        {
            if (_currentProgram == null)
            {
                return;
            }
            ShowProgramInfo();
        }

        private void CheckRecordingConflicts()
        {
        }

        private void OnPageUp()
        {
            int Steps;
            if (_singleChannelView)
                Steps = _channelCount; // all available rows
            else
            {
                if (_guideContinuousScroll)
                {
                    Steps = _channelCount; // all available rows
                }
                else
                {
                    // If we're on the first channel in the guide then allow one step to get back to the end of the guide.
                    if (_channelOffset == 0 && _cursorX == 0)
                    {
                        Steps = 1;
                    }
                    else
                    {
                        // only number of additional avail channels
                        Steps = Math.Min(_channelOffset + _cursorX, _channelCount);
                    }
                }
            }

            UnFocus();
            for (int i = 0; i < Steps; ++i)
            {
                OnUp(false, true);
            }
            Correct();
            Update(false);
            SetFocus();
        }

        private void OnPageDown()
        {
            int Steps;
            if (_singleChannelView)
                Steps = _channelCount; // all available rows
            else
            {
                if (_guideContinuousScroll)
                {
                    Steps = _channelCount; // all available rows
                }
                else
                {
                    // If we're on the last channel in the guide then allow one step to get back to top of guide.
                    if (_channelOffset + (_cursorX + 1) == _channelList.Count)
                    {
                        Steps = 1;
                    }
                    else
                    {
                        // only number of additional avail channels
                        Steps = Math.Min(_channelList.Count - _channelOffset - _cursorX - 1, _channelCount);
                    }
                }
            }

            UnFocus();
            for (int i = 0; i < Steps; ++i)
            {
                OnDown(false);
            }
            Correct();
            Update(false);
            SetFocus();
        }

        private void OnNextDay()
        {
            _viewingTime = _viewingTime.AddDays(1.0);
            _recalculateProgramOffset = true;
            Update(false);
            SetFocus();
        }

        private void OnPreviousDay()
        {
            _viewingTime = _viewingTime.AddDays(-1.0);
            _recalculateProgramOffset = true;
            Update(false);
            SetFocus();
        }

        private long GetColorForGenre(string genre)
        {
            ///@
            /*
            if (!_useColorsForGenres) return Color.White.ToArgb();
            List<string> genres = new List<string>();
            TVDatabase.GetGenres(ref genres);

            genre = genre.ToLower();
            for (int i = 0; i < genres.Count; ++i)
            {
              if (String.Compare(genre, (string)genres[i], true) == 0)
              {
                Color col = (Color)_colorList[i % _colorList.Count];
                return col.ToArgb();
              }
            }*/
            return Color.White.ToArgb();
        }

        private void OnKeyTimeout()
        {
            if (_lineInput.Length == 0)
            {
                // Hide label if no keyed channel number to display.
                GUILabelControl label = GetControl((int)Controls.LABEL_KEYED_CHANNEL) as GUILabelControl;
                if (label != null)
                {
                    label.IsVisible = false;
                }
                return;
            }
            TimeSpan ts = DateTime.Now - _keyPressedTimer;
            if (ts.TotalMilliseconds >= 1000)
            {
                // change channel
                int iChannel = Int32.Parse(_lineInput);
                ChangeChannelNr(iChannel);
                _lineInput = String.Empty;
            }
        }

        private void OnKeyCode(char chKey)
        {
            // Don't accept keys when in single channel mode.
            if (_singleChannelView)
            {
                return;
            }

            if (chKey >= '0' && chKey <= '9') //Make sure it's only for the remote
            {
                TimeSpan ts = DateTime.Now - _keyPressedTimer;
                if (_lineInput.Length >= _channelNumberMaxLength || ts.TotalMilliseconds >= 1000)
                {
                    _lineInput = String.Empty;
                }
                _keyPressedTimer = DateTime.Now;
                _lineInput += chKey;

                // give feedback to user that numbers are being entered
                // Check for new standalone label control for keyed in channel numbers.
                GUILabelControl label;
                label = GetControl((int)Controls.LABEL_KEYED_CHANNEL) as GUILabelControl;
                if (label != null)
                {
                    // Show the keyed channel number.
                    label.IsVisible = true;
                }
                else
                {
                    label = GetControl((int)Controls.LABEL_TIME1) as GUILabelControl;
                }
                label.Label = _lineInput;

                // Add an underscore "cursor" to visually indicate that more numbers may be entered.
                if (_lineInput.Length < _channelNumberMaxLength)
                {
                    label.Label += "_";
                }

                if (_lineInput.Length == _channelNumberMaxLength)
                {
                    // change channel
                    int iChannel = Int32.Parse(_lineInput);
                    ChangeChannelNr(iChannel);

                    // Hide the keyed channel number label.
                    GUILabelControl labelKeyed = GetControl((int)Controls.LABEL_KEYED_CHANNEL) as GUILabelControl;
                    if (labelKeyed != null)
                    {
                        labelKeyed.IsVisible = false;
                    }
                }
            }
        }

        private void ChangeChannelNr(int iChannelNr)
        {
            int iCounter = 0;
            bool found = false;
            int searchChannel = iChannelNr;

            Channel chan;
            int channelDistance = 99999;

            if (_byIndex == false)
            {
                while (iCounter < _channelList.Count && found == false)
                {
                    chan = (Channel)_channelList[iCounter].channel;
                    if (chan.LogicalChannelNumber.HasValue)
                    {
                        int chanNumber = chan.LogicalChannelNumber.Value;
                        if (chanNumber == searchChannel)
                        {
                            iChannelNr = iCounter;
                            found = true;
                        } //find closest channel number
                        else if ((int)Math.Abs(chanNumber - searchChannel) < channelDistance)
                        {
                            channelDistance = (int)Math.Abs(chanNumber - searchChannel);
                            iChannelNr = iCounter;
                        }
                    }
                    iCounter++;
                }
            }
            else
            {
                iChannelNr--; // offset for indexed channel number
            }
            if (iChannelNr >= 0 && iChannelNr < _channelList.Count)
            {
                UnFocus();
                _channelOffset = 0;
                _cursorX = 0;

                // Last page adjust (To get a full page channel listing)
                if ((_channelList.Count > _channelCount) && (iChannelNr > _channelList.Count - Math.Min(_channelList.Count, _channelCount) + 1)) // minimum of available channel/max visible channels
                {
                    _channelOffset = _channelList.Count - _channelCount;
                    iChannelNr = iChannelNr - _channelOffset;
                }

                while (iChannelNr >= Math.Min(_channelList.Count, _channelCount))
                {
                    iChannelNr -= Math.Min(_channelList.Count, _channelCount);
                    _channelOffset += Math.Min(_channelList.Count, _channelCount);
                }
                _cursorX = iChannelNr;

                Update(false);
                SetFocus();
            }
        }

        private void LoadSchedules(bool refresh)
        {
            if (refresh)
            {
                using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                {
                    _controller.RefreshUpcomingPrograms(tvSchedulerAgent, tvControlAgent);
                }
            }
        }

        private void GetChannels(bool refresh)
        {
            if (refresh || _channelList == null)
            {
                _channelList = new List<GuideBaseChannel>();
            }

            if (_channelList.Count == 0)
            {
                try
                {
                    using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                    {
                        foreach (Channel chan in tvSchedulerAgent.GetChannelsInGroup(_model.CurrentChannelGroupId, true))
                        {
                            GuideBaseChannel tvGuidChannel = new GuideBaseChannel();
                            tvGuidChannel.channel = chan;

                            if (tvGuidChannel.channel.VisibleInGuide && tvGuidChannel.channel.ChannelType == _channelType)
                            {
                                if (_showChannelNumber)
                                {
                                    if (_byIndex)
                                    {
                                        tvGuidChannel.channelNum = _channelList.Count + 1;
                                    }
                                    else if (chan.LogicalChannelNumber.HasValue)
                                    {
                                        tvGuidChannel.channelNum = chan.LogicalChannelNumber.Value;
                                    }
                                }
                                tvGuidChannel.strLogo = GetChannelLogo(tvGuidChannel.channel);
                                _channelList.Add(tvGuidChannel);
                            }
                        }
                    }
                }
                catch { }

                if (_channelList.Count == 0)
                {
                    GuideBaseChannel tvGuidChannel = new GuideBaseChannel()
                    {
                        channel = new Channel()
                        {
                            DisplayName = Utility.GetLocalizedText(TextId.NoChannels),
                            VisibleInGuide = true
                        }
                    };
                    for (int i = 0; i < 10; ++i)
                    {
                        _channelList.Add(tvGuidChannel);
                    }
                }
            }
        }

        private void UpdateVerticalScrollbar()
        {
            if (_channelList == null || _channelList.Count <= 0)
            {
                return;
            }
            int channel = _cursorX + _channelOffset;
            while (channel > 0 && channel >= _channelList.Count)
            {
                channel -= _channelList.Count;
            }
            float current = (float)(_cursorX + _channelOffset);
            float total = (float)_channelList.Count - 1;

            if (_singleChannelView)
            {
                current = (float)(_cursorX + _programOffset);
                total = (float)_totalProgramCount - 1;
            }
            if (total == 0)
            {
                total = _channelCount;
            }

            float percentage = (current / total) * 100.0f;
            if (percentage < 0)
            {
                percentage = 0;
            }
            if (percentage > 100)
            {
                percentage = 100;
            }
            GUIVerticalScrollbar scrollbar = GetControl((int)Controls.VERT_SCROLLBAR) as GUIVerticalScrollbar;
            if (scrollbar != null)
            {
                scrollbar.Percentage = percentage;
            }
        }

        private void UpdateHorizontalScrollbar()
        {
            if (_channelList == null)
            {
                return;
            }
            GUIHorizontalScrollbar scrollbar = GetControl((int)Controls.HORZ_SCROLLBAR) as GUIHorizontalScrollbar;
            if (scrollbar != null)
            {
                float percentage = (float)_viewingTime.Hour * 60 + _viewingTime.Minute +
                  (float)_timePerBlock * ((float)_viewingTime.Hour / 24.0f);
                percentage /= (24.0f * 60.0f);
                percentage *= 100.0f;
                if (percentage < 0)
                {
                    percentage = 0;
                }
                if (percentage > 100)
                {
                    percentage = 100;
                }
                if (_singleChannelView)
                {
                    percentage = 0;
                }

                if ((int)percentage != (int)scrollbar.Percentage)
                {
                    scrollbar.Percentage = percentage;
                }
            }
        }

        /// <summary>
        /// returns true if Mediaportal should send a notification when the program specified is about to start
        /// </summary>
        /// <param name="program">Program</param>
        /// <returns>true : MP shows a notification when program is about to start</returns>
        private bool ShouldNotifyProgram(GuideProgramSummary program)
        {
            return false;
        }

        protected int CalcDays()
        {
            int iDay = _viewingTime.DayOfYear - DateTime.Now.DayOfYear;
            if (_viewingTime.Year > DateTime.Now.Year)
            {
                iDay += (new DateTime(DateTime.Now.Year, 12, 31)).DayOfYear;
            }
            return iDay;
        }

        #region TV Database callbacks

        protected void TVDatabase_On_notifyListChanged()
        {
            /// @
            /*
            if (_notifyList != null)
            {
              _notifyList.Clear();
              TVDatabase.GetNotifies(_notifyList, false);
              _needUpdate = true;
            }
            */
        }

        protected void ConflictManager_OnConflictsUpdated()
        {
            _needUpdate = true;
        }

        protected void TVDatabase_OnProgramsChanged()
        {
            _needUpdate = true;
        }

        #endregion

        #endregion

        /// <summary>
        /// Calculates the duration of a program and sets the Duration property
        /// </summary>
        private string GetDuration(GuideProgram program)
        {
            if (program.GuideProgramId == Guid.Empty)
            {
                return "";
            }
            string space = " ";
            DateTime progStart = program.StartTime;
            DateTime progEnd = program.StopTime;
            TimeSpan progDuration = progEnd.Subtract(progStart);
            string duration = "";
            switch (progDuration.Hours)
            {
                case 0:
                    duration = progDuration.Minutes + space + GUILocalizeStrings.Get(3004);
                    break;
                case 1:
                    if (progDuration.Minutes == 1)
                    {
                        duration = progDuration.Hours + space + GUILocalizeStrings.Get(3001) + ", " + progDuration.Minutes + space +
                                   GUILocalizeStrings.Get(3003);
                    }
                    else if (progDuration.Minutes > 1)
                    {
                        duration = progDuration.Hours + space + GUILocalizeStrings.Get(3001) + ", " + progDuration.Minutes + space +
                                   GUILocalizeStrings.Get(3004);
                    }
                    else
                    {
                        duration = progDuration.Hours + space + GUILocalizeStrings.Get(3001);
                    }
                    break;
                default:
                    if (progDuration.Minutes == 1)
                    {
                        duration = progDuration.Hours + " Hours" + ", " + progDuration.Minutes + space +
                                   GUILocalizeStrings.Get(3003);
                    }
                    else if (progDuration.Minutes > 0)
                    {
                        duration = progDuration.Hours + " Hours" + ", " + progDuration.Minutes + space +
                                   GUILocalizeStrings.Get(3004);
                    }
                    else
                    {
                        duration = progDuration.Hours + space + GUILocalizeStrings.Get(3002);
                    }
                    break;
            }
            return duration;
        }

        private string GetDurationAsMinutes(GuideProgram program)
        {
            if (program.Title == "No TVGuide data available")
            {
                return "";
            }
            DateTime progStart = program.StartTime;
            DateTime progEnd = program.StopTime;
            TimeSpan progDuration = progEnd.Subtract(progStart);
            return progDuration.TotalMinutes + " " + GUILocalizeStrings.Get(2998);
        }

        /// <summary>
        /// Calculates how long from current time a program starts or started, set the TimeFromNow property
        /// </summary>
        private string GetStartTimeFromNow(GuideProgram program)
        {
            string timeFromNow = String.Empty;
            if (program.GuideProgramId == Guid.Empty)
            {
                return timeFromNow;
            }
            string space = " ";
            string strRemaining = String.Empty;
            DateTime progStart = program.StartTime;
            TimeSpan timeRelative = progStart.Subtract(DateTime.Now);
            if (timeRelative.Days == 0)
            {
                if (timeRelative.Hours >= 0 && timeRelative.Minutes >= 0)
                {
                    switch (timeRelative.Hours)
                    {
                        case 0:
                            if (timeRelative.Minutes == 1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3009) + " " + timeRelative.Minutes + space +
                                              GUILocalizeStrings.Get(3003); // starts in 1 minute
                            }
                            else if (timeRelative.Minutes > 1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3009) + " " + timeRelative.Minutes + space +
                                              GUILocalizeStrings.Get(3004); //starts in x minutes
                            }
                            else
                            {
                                timeFromNow = GUILocalizeStrings.Get(3013);
                            }
                            break;
                        case 1:
                            if (timeRelative.Minutes == 1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3009) + " " + timeRelative.Hours + space +
                                              GUILocalizeStrings.Get(3001) + ", " + timeRelative.Minutes + space +
                                              GUILocalizeStrings.Get(3003); //starts in 1 hour, 1 minute
                            }
                            else if (timeRelative.Minutes > 1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3009) + " " + timeRelative.Hours + space +
                                              GUILocalizeStrings.Get(3001) + ", " + timeRelative.Minutes + space +
                                              GUILocalizeStrings.Get(3004); //starts in 1 hour, x minutes
                            }
                            else
                            {
                                timeFromNow = GUILocalizeStrings.Get(3009) + " " + timeRelative.Hours + GUILocalizeStrings.Get(3001);
                                //starts in 1 hour
                            }
                            break;
                        default:
                            if (timeRelative.Minutes == 1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3009) + " " + timeRelative.Hours + space +
                                              GUILocalizeStrings.Get(3002) + ", " + timeRelative.Minutes + space +
                                              GUILocalizeStrings.Get(3003); //starts in x hours, 1 minute
                            }
                            else if (timeRelative.Minutes > 1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3009) + " " + timeRelative.Hours + space +
                                              GUILocalizeStrings.Get(3002) + ", " + timeRelative.Minutes + space +
                                              GUILocalizeStrings.Get(3004); //starts in x hours, x minutes
                            }
                            else
                            {
                                timeFromNow = GUILocalizeStrings.Get(3009) + " " + timeRelative.Hours + space +
                                              GUILocalizeStrings.Get(3002); //starts in x hours
                            }
                            break;
                    }
                }
                else //already started
                {
                    DateTime progEnd = program.StopTime;
                    TimeSpan tsRemaining = DateTime.Now.Subtract(progEnd);
                    if (tsRemaining.Minutes > 0)
                    {
                        timeFromNow = GUILocalizeStrings.Get(3016);
                        return timeFromNow;
                    }
                    switch (tsRemaining.Hours)
                    {
                        case 0:
                            if (timeRelative.Minutes == 1)
                            {
                                strRemaining = "(" + -tsRemaining.Minutes + space + GUILocalizeStrings.Get(3018) + ")";
                                //(1 Minute Remaining)
                            }
                            else
                            {
                                strRemaining = "(" + -tsRemaining.Minutes + space + GUILocalizeStrings.Get(3010) + ")";
                                //(x Minutes Remaining)
                            }
                            break;
                        case -1:
                            if (timeRelative.Minutes == 1)
                            {
                                strRemaining = "(" + -tsRemaining.Hours + space + GUILocalizeStrings.Get(3001) + ", " +
                                               -tsRemaining.Minutes + space + GUILocalizeStrings.Get(3018) + ")";
                                //(1 Hour,1 Minute Remaining)
                            }
                            else if (timeRelative.Minutes > 1)
                            {
                                strRemaining = "(" + -tsRemaining.Hours + space + GUILocalizeStrings.Get(3001) + ", " +
                                               -tsRemaining.Minutes + space + GUILocalizeStrings.Get(3010) + ")";
                                //(1 Hour,x Minutes Remaining)
                            }
                            else
                            {
                                strRemaining = "(" + -tsRemaining.Hours + space + GUILocalizeStrings.Get(3012) + ")";
                                //(1 Hour Remaining)
                            }
                            break;
                        default:
                            if (timeRelative.Minutes == 1)
                            {
                                strRemaining = "(" + -tsRemaining.Hours + space + GUILocalizeStrings.Get(3002) + ", " +
                                               -tsRemaining.Minutes + space + GUILocalizeStrings.Get(3018) + ")";
                                //(x Hours,1 Minute Remaining)
                            }
                            else if (timeRelative.Minutes > 1)
                            {
                                strRemaining = "(" + -tsRemaining.Hours + space + GUILocalizeStrings.Get(3002) + ", " +
                                               -tsRemaining.Minutes + space + GUILocalizeStrings.Get(3010) + ")";
                                //(x Hours,x Minutes Remaining)
                            }
                            else
                            {
                                strRemaining = "(" + -tsRemaining.Hours + space + GUILocalizeStrings.Get(3012) + ")";
                                //(x Hours Remaining)
                            }
                            break;
                    }
                    switch (timeRelative.Hours)
                    {
                        case 0:
                            if (timeRelative.Minutes == -1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3017) + -timeRelative.Minutes + space +
                                              GUILocalizeStrings.Get(3007) + space + strRemaining; //Started 1 Minute ago
                            }
                            else if (timeRelative.Minutes < -1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3017) + -timeRelative.Minutes + space +
                                              GUILocalizeStrings.Get(3008) + space + strRemaining; //Started x Minutes ago
                            }
                            else
                            {
                                timeFromNow = GUILocalizeStrings.Get(3013); //Starting Now
                            }
                            break;
                        case -1:
                            if (timeRelative.Minutes == -1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3017) + -timeRelative.Hours + space + GUILocalizeStrings.Get(3001) +
                                              ", " + -timeRelative.Minutes + space + GUILocalizeStrings.Get(3007) + " " + strRemaining;
                                //Started 1 Hour,1 Minute ago
                            }
                            else if (timeRelative.Minutes < -1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3017) + -timeRelative.Hours + space + GUILocalizeStrings.Get(3001) +
                                              ", " + -timeRelative.Minutes + space + GUILocalizeStrings.Get(3008) + " " + strRemaining;
                                //Started 1 Hour,x Minutes ago
                            }
                            else
                            {
                                timeFromNow = GUILocalizeStrings.Get(3017) + -timeRelative.Hours + space + GUILocalizeStrings.Get(3005) +
                                              space + strRemaining; //Started 1 Hour ago
                            }
                            break;
                        default:
                            if (timeRelative.Minutes == -1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3017) + -timeRelative.Hours + space + GUILocalizeStrings.Get(3006) +
                                              ", " + -timeRelative.Minutes + space + GUILocalizeStrings.Get(3008) + " " + strRemaining;
                                //Started x Hours,1 Minute ago
                            }
                            else if (timeRelative.Minutes < -1)
                            {
                                timeFromNow = GUILocalizeStrings.Get(3017) + -timeRelative.Hours + space + GUILocalizeStrings.Get(3006) +
                                              ", " + -timeRelative.Minutes + space + GUILocalizeStrings.Get(3008) + " " + strRemaining;
                                //Started x Hours,x Minutes ago
                            }
                            else
                            {
                                timeFromNow = GUILocalizeStrings.Get(3017) + -timeRelative.Hours + space + GUILocalizeStrings.Get(3006) +
                                              space + strRemaining; //Started x Hours ago
                            }
                            break;
                    }
                }
            }
            else
            {
                if (timeRelative.Days == 1)
                {
                    timeFromNow = GUILocalizeStrings.Get(3009) + space + timeRelative.Days + space + GUILocalizeStrings.Get(3014);
                    //Starts in 1 Day
                }
                else
                {
                    timeFromNow = GUILocalizeStrings.Get(3009) + space + timeRelative.Days + space + GUILocalizeStrings.Get(3015);
                    //Starts in x Days
                }
            }
            return timeFromNow;
        }

        private void setGuideHeadingVisibility(bool visible)
        {
            // can't rely on the heading text control having a unique id, so locate it using the localised heading string.
            // todo: update all skins to have a unique id for this control...?
            foreach (GUIControl control in controlList)
            {
                if (control is GUILabelControl)
                {
                    if (((GUILabelControl)control).Label == GUILocalizeStrings.Get(4)) // TV Guide heading
                    {
                        control.Visible = visible;
                    }
                }
            }
        }

        private void SetSingleChannelLabelVisibility(bool visible)
        {
            GUILabelControl channelLabel = GetControl((int)Controls.SINGLE_CHANNEL_LABEL) as GUILabelControl;
            GUIImage channelImage = GetControl((int)Controls.SINGLE_CHANNEL_IMAGE) as GUIImage;
            GUISpinControl timeInterval = GetControl((int)Controls.SPINCONTROL_TIME_INTERVAL) as GUISpinControl;

            if (channelLabel != null)
            {
                channelLabel.Visible = visible;
            }

            if (channelImage != null)
            {
                channelImage.Visible = visible;
            }

            if (timeInterval != null)
            {
                // If the x position of the control is negative then we assume that the control is not in the viewable area
                // and so it should not be made visible.  Skinners can set the x position negative to effectively remove the control
                // from the window.
                if (timeInterval.XPosition < 0)
                {
                    timeInterval.Visible = false;
                }
                else
                {
                    timeInterval.Visible = !visible;
                }
            }
        }

        #region IMDB.IProgress

        public bool OnDisableCancel(IMDBFetcher fetcher)
        {
            GUIDialogProgress pDlgProgress =
              (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS);
            if (pDlgProgress.IsInstance(fetcher))
            {
                pDlgProgress.DisableCancel(true);
            }
            return true;
        }

        public void OnProgress(string line1, string line2, string line3, int percent)
        {
            if (!GUIWindowManager.IsRouted)
            {
                return;
            }
            GUIDialogProgress pDlgProgress =
              (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS);
            pDlgProgress.ShowProgressBar(true);
            pDlgProgress.SetLine(1, line1);
            pDlgProgress.SetLine(2, line2);
            if (percent > 0)
            {
                pDlgProgress.SetPercentage(percent);
            }
            pDlgProgress.Progress();
        }

        public bool OnSearchStarting(IMDBFetcher fetcher)
        {
            GUIDialogProgress pDlgProgress =
              (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS);
            // show dialog that we're busy querying www.imdb.com
            pDlgProgress.Reset();
            pDlgProgress.SetHeading(GUILocalizeStrings.Get(197));
            pDlgProgress.SetLine(1, fetcher.MovieName);
            pDlgProgress.SetLine(2, string.Empty);
            pDlgProgress.SetObject(fetcher);
            pDlgProgress.StartModal(GUIWindowManager.ActiveWindow);
            return true;
        }

        public bool OnSearchStarted(IMDBFetcher fetcher)
        {
            GUIDialogProgress pDlgProgress =
              (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS);
            pDlgProgress.SetObject(fetcher);
            pDlgProgress.DoModal(GUIWindowManager.ActiveWindow);
            if (pDlgProgress.IsCanceled)
            {
                return false;
            }
            return true;
        }

        public bool OnSearchEnd(IMDBFetcher fetcher)
        {
            GUIDialogProgress pDlgProgress =
              (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS);
            if ((pDlgProgress != null) && (pDlgProgress.IsInstance(fetcher)))
            {
                pDlgProgress.Close();
            }
            return true;
        }

        public bool OnMovieNotFound(IMDBFetcher fetcher)
        {
            Log.Info("IMDB Fetcher: OnMovieNotFound");
            // show dialog...
            GUIDialogOK pDlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
            pDlgOK.SetHeading(195);
            pDlgOK.SetLine(1, fetcher.MovieName);
            pDlgOK.SetLine(2, string.Empty);
            pDlgOK.DoModal(GUIWindowManager.ActiveWindow);
            return true;
        }

        public bool OnDetailsStarting(IMDBFetcher fetcher)
        {
            GUIDialogProgress pDlgProgress =
              (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS);
            // show dialog that we're downloading the movie info
            pDlgProgress.Reset();
            pDlgProgress.SetHeading(GUILocalizeStrings.Get(198));
            //pDlgProgress.SetLine(0, strMovieName);
            pDlgProgress.SetLine(1, fetcher.MovieName);
            pDlgProgress.SetLine(2, string.Empty);
            pDlgProgress.SetObject(fetcher);
            pDlgProgress.StartModal(GUIWindowManager.ActiveWindow);
            return true;
        }

        public bool OnDetailsStarted(IMDBFetcher fetcher)
        {
            GUIDialogProgress pDlgProgress =
              (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS);
            pDlgProgress.SetObject(fetcher);
            pDlgProgress.DoModal(GUIWindowManager.ActiveWindow);
            if (pDlgProgress.IsCanceled)
            {
                return false;
            }
            return true;
        }

        public bool OnDetailsEnd(IMDBFetcher fetcher)
        {
            GUIDialogProgress pDlgProgress =
              (GUIDialogProgress)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_PROGRESS);
            if ((pDlgProgress != null) && (pDlgProgress.IsInstance(fetcher)))
            {
                pDlgProgress.Close();
            }
            return true;
        }

        public bool OnActorsStarting(IMDBFetcher fetcher)
        {
            // won't occure
            return true;
        }

        public bool OnActorsStarted(IMDBFetcher fetcher)
        {
            // won't occure
            return true;
        }

        public bool OnActorsEnd(IMDBFetcher fetcher)
        {
            // won't occure
            return true;
        }

        public bool OnDetailsNotFound(IMDBFetcher fetcher)
        {
            Log.Info("IMDB Fetcher: OnDetailsNotFound");
            // show dialog...
            GUIDialogOK pDlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
            // show dialog...
            pDlgOK.SetHeading(195);
            pDlgOK.SetLine(1, fetcher.MovieName);
            pDlgOK.SetLine(2, string.Empty);
            pDlgOK.DoModal(GUIWindowManager.ActiveWindow);
            return false;
        }

        public bool OnRequestMovieTitle(IMDBFetcher fetcher, out string movieName)
        {
            // won't occure
            movieName = "";
            return true;
        }

        public bool OnSelectMovie(IMDBFetcher fetcher, out int selectedMovie)
        {
            // won't occure
            selectedMovie = 0;
            return true;
        }

        public bool OnScanStart(int total)
        {
            // won't occure
            return true;
        }

        public bool OnScanEnd()
        {
            // won't occure
            return true;
        }

        public bool OnScanIterating(int count)
        {
            // won't occure
            return true;
        }

        public bool OnScanIterated(int count)
        {
            // won't occure
            return true;
        }

        #endregion

        protected bool GetKeyboard(ref string strLine)
        {
            VirtualKeyboard keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)Window.WINDOW_VIRTUAL_KEYBOARD);
            if (null == keyboard)
            {
                return false;
            }
            keyboard.Reset();
            keyboard.Text = strLine;
            keyboard.DoModal(GetID);
            if (keyboard.IsConfirmed)
            {
                strLine = keyboard.Text;
                return true;
            }
            return false;
        }

        private string GetChannelProgramIcon(Channel channel, Guid guideProgramId)
        {
            GuideUpcomingProgram guideUpcomingProgram;
            return GetChannelProgramIcon(channel, guideProgramId, out guideUpcomingProgram);
        }

        private string GetChannelProgramIcon(Channel channel, Guid guideProgramId, out bool isRecording, out bool isAlert)
        {
            GuideUpcomingProgram guideUpcomingProgram;
            string result = GetChannelProgramIcon(channel, guideProgramId, out guideUpcomingProgram);
            isRecording = (guideUpcomingProgram != null && guideUpcomingProgram.Type == ScheduleType.Recording
                && guideUpcomingProgram.CancellationReason == UpcomingCancellationReason.None);
            isAlert = (guideUpcomingProgram != null && guideUpcomingProgram.Type == ScheduleType.Alert);
            return result;
        }

        private string GetChannelProgramIcon(Channel channel, Guid guideProgramId, out GuideUpcomingProgram guideUpcomingProgram)
        {
            Guid upcomingProgramId = UpcomingProgram.GetUniqueUpcomingProgramId(guideProgramId, channel.ChannelId);

            guideUpcomingProgram = null;
            string recordIconImage = null;
            GuideUpcomingProgram upcoming = null;
            ScheduleType type = ScheduleType.Recording;
            if (_model.UpcomingRecordingsById.ContainsKey(upcomingProgramId))
            {
                type = ScheduleType.Recording;
                upcoming = _model.UpcomingRecordingsById[upcomingProgramId];
                guideUpcomingProgram = upcoming;
            }
            else if (_model.UpcomingAlertsById.ContainsKey(upcomingProgramId))
            {
                type = ScheduleType.Alert;
                upcoming = _model.UpcomingAlertsById[upcomingProgramId];
                guideUpcomingProgram = upcoming;
            }
            else if (_model.UpcomingSuggestionsById.ContainsKey(upcomingProgramId))
            {
                type = ScheduleType.Suggestion;
                upcoming = _model.UpcomingSuggestionsById[upcomingProgramId];
            }
            if (upcoming != null
                && upcoming.ChannelId == channel.ChannelId)
            {
                recordIconImage = Utility.GetIconImageFileName(type, upcoming);
            }
            return recordIconImage;
        }

        private bool IsRunningAt(GuideProgramSummary program, DateTime fromTime, DateTime toTime)
        {
            return (fromTime < program.StopTime && toTime > program.StartTime);
        }

        private bool IsRunningAt(GuideProgramSummary program, DateTime time)
        {
            return (time >= program.StartTime && time <= program.StopTime);
        }

        private string GetLocalDate(string dayOfWeek, int day, int month)
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            string monthDayFormat = currentCulture.DateTimeFormat.MonthDayPattern;

            // Get the starting position of the day and month in the
            // MonthDay pattern of the current culture
            int dayPosition = monthDayFormat.IndexOf('d');
            int monthPosition = monthDayFormat.IndexOf('M');

            // See if month is first in the pattern
            string format = "{0} {1}{2}{3}";
            if (monthPosition >= 0
                && monthPosition < dayPosition)
            {
                // Month is first - Display month before day
                format = "{0} {3}{2}{1}";
            }

            return String.Format(format, dayOfWeek, day, currentCulture.DateTimeFormat.DateSeparator, month);
        }
    }
}
