#region Copyright (C) 2005-2012 Team MediaPortal

// Copyright (C) 2005-2012 Team MediaPortal
// http://www.team-mediaportal.com
// 
// MediaPortal is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// MediaPortal is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with MediaPortal. If not, see <http://www.gnu.org/licenses/>.

#endregion

#region usings

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Timers;
using System.Windows.Forms;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Profile;
using MediaPortal.Util;
using MediaPortal.Video.Database;
using Timer = System.Timers.Timer;
using Action = MediaPortal.GUI.Library.Action;
using MediaPortal.Player.PostProcessing;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;
using ArgusTV.UI.Process.Guide;

#endregion

namespace ArgusTV.UI.MediaPortal
{
    /// <summary>
    /// 
    /// </summary>
    public class TvFullScreen : GUIInternalWindow, IRenderLayer, IMDB.IProgress
    {
        #region FullScreenState class

        private class FullScreenState
        {
            public int SeekStep = 1;
            public int Speed = 1;
            public bool OsdVisible = false;
            public bool Paused = false;
            public bool ContextMenuVisible = false;
            public bool ShowStatusLine = false;
            public bool ShowTime = false;
            public bool ZapOsdVisible = false;
            public bool MsgBoxVisible = false;
            public bool ShowGroup = false;
            public bool ShowInput = false;
            public bool _notifyDialogVisible = false;
            public bool _bottomDialogMenuVisible = false;
            public bool wasVMRBitmapVisible = false;
            public bool volumeVisible = false;
            public bool _dialogYesNoVisible = false;
        }

        #endregion

        #region variables

        private bool _stepSeekVisible = false;
        private bool _statusVisible = false;
        //private bool _groupVisible = false;
        private bool _byIndex = false;
        private DateTime _statusTimeOutTimer = DateTime.Now;
        private bool _enableRecNotification = false;

        private TvZapOsd _zapWindow = null;

        private TvOsd _osdWindow = null;

        private DateTime _osdTimeoutTimer;

        private DateTime _zapTimeOutTimer;
        //private DateTime _groupTimeOutTimer;
        private DateTime _vmr7UpdateTimer = DateTime.Now;
        //		string			m_sZapChannel;
        //		long				m_iZapDelay;
        private volatile bool _isOsdVisible = false;
        private volatile bool _isPauseOsdVisible = false;
        private volatile bool _zapOsdVisible = false;
        private bool _channelInputVisible = false;

        private long _timeOsdOnscreen;
        private long _zapTimeOutValue;
        private DateTime _updateTimer = DateTime.Now;
        private DateTime _updateTimerProgressbar = DateTime.Now;
        private bool _lastPause = false;
        private int _lastSpeed = 1;
        private DateTime _keyPressedTimer = DateTime.Now;
        private string _channelName = "";
        private bool _isDialogVisible = false;
        private bool _IsClosingDialog = false;
        private GUIDialogMenu dlg;
        //private GUIDialogMenuBottomRight _dialogBottomMenu = null;
        private GUIDialogYesNo _dlgYesNo = null;
        private GUIDialogMenu _dialogMenu = null;
        // Message box
        private bool _dialogYesNoVisible = false;
        private bool _notifyDialogVisible = false;
        private bool _bottomDialogMenuVisible = false;
        private bool _messageBoxVisible = false;
        private DateTime _msgTimer = DateTime.Now;
        private int _msgBoxTimeout = 0;
        private bool _needToClearScreen = false;
        private bool _useVMR9Zap = false;
        private bool _immediateSeekIsRelative = true;
        private int _immediateSeekValue = 10;
        private int _channelNumberMaxLength = 3;

        // Tv error handling
        // TODO
        private object _gotTvErrorMessage = null;

        private FullScreenState _screenState = new FullScreenState();

        private bool _isVolumeVisible = false;
        private DateTime _volumeTimer = DateTime.MinValue;
        private bool _isStartingTSForRecording = false;
        private bool _autoZapMode = false;
        private Timer _autoZapTimer = new Timer();
        [SkinControl(500)]
        protected GUIImage imgVolumeMuteIcon;
        [SkinControl(501)]
        protected GUIVolumeBar imgVolumeBar;

        private Guid _lastChannelWithNoSignalId;
        private VideoRendererStatistics.State videoState = VideoRendererStatistics.State.VideoPresent;
        private List<Geometry.Type> _allowedArModes = new List<Geometry.Type>();

        private bool _settingsLoaded;
        #endregion

        #region enums

        private enum Control
        {
            BLUE_BAR = 0,
            MSG_BOX = 2,
            MSG_BOX_LABEL1 = 3,
            MSG_BOX_LABEL2 = 4,
            MSG_BOX_LABEL3 = 5,
            MSG_BOX_LABEL4 = 6,
            LABEL_ROW1 = 10,
            LABEL_ROW2 = 11,
            LABEL_ROW3 = 12,
            IMG_PAUSE = 16,
            IMG_2X = 17,
            IMG_4X = 18,
            IMG_8X = 19,
            IMG_16X = 20,
            IMG_32X = 21,
            IMG_MIN2X = 23,
            IMG_MIN4X = 24,
            IMG_MIN8X = 25,
            IMG_MIN16X = 26,
            IMG_MIN32X = 27,
            OSD_VIDEOPROGRESS = 100,
            REC_LOGO = 39
        } ;

        #endregion

        public TvFullScreen()
        {
            Log.Debug("TvFullScreen:ctor");
            GetID = (int)GUIWindow.Window.WINDOW_TVFULLSCREEN;
        }

        public override bool SupportsDelayedLoad
        {
            get { return false; }
        }

        public override bool IsTv
        {
            get { return true; }
        }

        /// <summary>
        /// Gets called by the runtime when a  window will be destroyed
        /// Every window window should override this method and cleanup any resources
        /// </summary>
        /// <returns></returns>
        public override void DeInit()
        {
            OnPageDestroy(-1);
            _autoZapTimer.Elapsed -= new ElapsedEventHandler(_autoZapTimer_Elapsed);
        }

        public override bool Init()
        {
            using (Settings xmlreader = new MPSettings())
            {
                _useVMR9Zap = xmlreader.GetValueAsBool("general", "useVMR9ZapOSD", false);
                _immediateSeekIsRelative = xmlreader.GetValueAsBool("movieplayer", "immediateskipstepsisrelative", true);
                _immediateSeekValue = xmlreader.GetValueAsInt("movieplayer", "immediateskipstepsize", 10);
                _enableRecNotification = xmlreader.GetValueAsBool("mytv", "enableRecNotifier", false);
            }
            Load(GUIGraphicsContext.Skin + @"\mytvFullScreen.xml");
            GetID = (int)GUIWindow.Window.WINDOW_TVFULLSCREEN;

            SettingsLoaded = false;

            g_Player.PlayBackEnded += new g_Player.EndedHandler(g_Player_PlayBackEnded);
            g_Player.PlayBackStopped += new g_Player.StoppedHandler(g_Player_PlayBackStopped);
            g_Player.PlayBackChanged += new g_Player.ChangedHandler(g_Player_PlayBackChanged);

            Log.Debug("TvFullScreen:Init");
            return true;
        }

        #region serialisation

        private void LoadSettings()
        {
            using (Settings xmlreader = new MPSettings())
            {
                _timeOsdOnscreen = 1000 * xmlreader.GetValueAsInt("movieplayer", "osdtimeout", 5);
                _zapTimeOutValue = 1000 * xmlreader.GetValueAsInt("movieplayer", "zaptimeout", 5);
                _byIndex = xmlreader.GetValueAsBool("mytv", "byindex", true);
                _channelNumberMaxLength = xmlreader.GetValueAsInt("mytv", "channelnumbermaxlength", 3);
                _allowedArModes.Clear();
                if (xmlreader.GetValueAsBool("mytv", "allowarzoom", true))
                {
                    _allowedArModes.Add(Geometry.Type.Zoom);
                }
                if (xmlreader.GetValueAsBool("mytv", "allowarstretch", true))
                {
                    _allowedArModes.Add(Geometry.Type.Stretch);
                }
                if (xmlreader.GetValueAsBool("mytv", "allowarnormal", true))
                {
                    _allowedArModes.Add(Geometry.Type.Normal);
                }
                if (xmlreader.GetValueAsBool("mytv", "allowaroriginal", true))
                {
                    _allowedArModes.Add(Geometry.Type.Original);
                }
                if (xmlreader.GetValueAsBool("mytv", "allowarletterbox", true))
                {
                    _allowedArModes.Add(Geometry.Type.LetterBox43);
                }
                if (xmlreader.GetValueAsBool("mytv", "allowarnonlinear", true))
                {
                    _allowedArModes.Add(Geometry.Type.NonLinearStretch);
                }
                if (xmlreader.GetValueAsBool("mytv", "allowarzoom149", true))
                {
                    _allowedArModes.Add(Geometry.Type.Zoom14to9);
                }
                if (!TvHome.SettingsLoaded)
                {
                    string strValue = xmlreader.GetValueAsString("mytv", "defaultar", "Normal");
                    GUIGraphicsContext.ARType = Utils.GetAspectRatio(strValue);
                }
            }

            SettingsLoaded = true;
        }

        #endregion

        public override void ResetAllControls()
        {
            //reset all

            bool bOffScreen = false;
            int iCalibrationY = GUIGraphicsContext.OSDOffset;
            int iTop = GUIGraphicsContext.OverScanTop;
            int iMin = 0;

            foreach (CPosition pos in _listPositions)
            {
                pos.control.SetPosition((int)pos.XPos, (int)pos.YPos + iCalibrationY);
            }
            foreach (CPosition pos in _listPositions)
            {
                GUIControl pControl = pos.control;

                int dwPosY = pControl.YPosition;
                if (pControl.IsVisible)
                {
                    if (dwPosY < iTop)
                    {
                        int iSize = iTop - dwPosY;
                        if (iSize > iMin)
                        {
                            iMin = iSize;
                        }
                        bOffScreen = true;
                    }
                }
            }
            if (bOffScreen)
            {
                foreach (CPosition pos in _listPositions)
                {
                    GUIControl pControl = pos.control;
                    int dwPosX = pControl.XPosition;
                    int dwPosY = pControl.YPosition;
                    if (dwPosY < (int)100)
                    {
                        dwPosY += Math.Abs(iMin);
                        pControl.SetPosition(dwPosX, dwPosY);
                    }
                }
            }
            base.ResetAllControls();
        }

        public override void OnAction(Action action)
        {
            _needToClearScreen = true;

            if (action.wID == Action.ActionType.ACTION_SHOW_VOLUME)
            {
                _volumeTimer = DateTime.Now;
                _isVolumeVisible = true;
                RenderVolume(_isVolumeVisible);
            }

            if (action.wID == Action.ActionType.ACTION_MOUSE_CLICK && action.MouseButton == MouseButtons.Right)
            {
                // switch back to the menu
                _isOsdVisible = false;
                GUIWindowManager.IsOsdVisible = false;
                GUIGraphicsContext.IsFullScreenVideo = false;
                GUIWindowManager.ShowPreviousWindow();
                return;
            }
            if (_isOsdVisible)
            {
                if (((action.wID == Action.ActionType.ACTION_SHOW_OSD) || (action.wID == Action.ActionType.ACTION_SHOW_GUI) ||
                     (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU)) && !_osdWindow.SubMenuVisible) // hide the OSD
                {
                    lock (this)
                    {
                        HideMainOSD();
                        return;
                    }
                }
                else if (!_zapOsdVisible)
                {
                    _osdTimeoutTimer = DateTime.Now;
                    if (action.wID == Action.ActionType.ACTION_MOUSE_MOVE || action.wID == Action.ActionType.ACTION_MOUSE_CLICK)
                    {
                        int x = (int)action.fAmount1;
                        int y = (int)action.fAmount2;
                        if (!GUIGraphicsContext.MouseSupport)
                        {
                            _osdWindow.OnAction(action); // route keys to OSD window

                            return;
                        }
                        else
                        {
                            if (_osdWindow.InWindow(x, y))
                            {
                                _osdWindow.OnAction(action); // route keys to OSD window

                                HideZapOSD();

                                return;
                            }
                            else
                            {
                                HideMainOSD();
                                return;
                            }
                        }
                    }
                    Action newAction = new Action();
                    if (action.wID != Action.ActionType.ACTION_KEY_PRESSED && action.wID != Action.ActionType.ACTION_PAUSE &&
                        ActionTranslator.GetAction((int)Window.WINDOW_OSD, action.m_key, ref newAction))
                    {
                        _osdWindow.OnAction(newAction); // route keys to OSD window
                    }
                    else
                    {
                        // route unhandled actions to OSD window
                        _osdWindow.OnAction(action);
                    }
                }
                return;
            }

            else if (action.wID == Action.ActionType.ACTION_MOUSE_MOVE && GUIGraphicsContext.MouseSupport)
            {
                int y = (int)action.fAmount2;
                if (y > GUIGraphicsContext.Height - 100)
                {
                    ShowMainOSD();
                }
            }
            else if (_zapOsdVisible)
            {
                if ((action.wID == Action.ActionType.ACTION_SHOW_GUI) || (action.wID == Action.ActionType.ACTION_SHOW_OSD) ||
                    (action.wID == Action.ActionType.ACTION_PREVIOUS_MENU))
                {
                    HideZapOSD();
                }
            }
            //Log.DebugFile(Log.LogType.Error, "action:{0}",action.wID);
            switch (action.wID)
            {
                case Action.ActionType.ACTION_MOUSE_DOUBLECLICK:
                case Action.ActionType.ACTION_SELECT_ITEM:
                    {
                        if (!g_Player.IsTVRecording)
                        {
                            if (_autoZapMode)
                            {
                                StopAutoZap();
                            }
                            else if (_zapOsdVisible)
                            {
                                PluginMain.Navigator.ZapNow();
                            }
                            else
                            {
                                OnSelectChannel();
                            }
                        }
                    }
                    break;

                case Action.ActionType.ACTION_SHOW_INFO:
                case Action.ActionType.ACTION_SHOW_CURRENT_TV_INFO:
                    {
                        if (action.fAmount1 != 0)
                        {
                            _zapTimeOutTimer = DateTime.MaxValue;
                            _zapTimeOutTimer = DateTime.Now;
                        }
                        else
                        {
                            _zapTimeOutTimer = DateTime.Now;
                        }

                        if (!_zapOsdVisible && !g_Player.IsTVRecording)
                        {
                            if (!_useVMR9Zap)
                            {
                                ShowZapOSD(_gotTvErrorMessage);
                                _gotTvErrorMessage = null;
                            }
                        }
                        else
                        {
                            _zapWindow.UpdateChannelInfo();
                            _zapTimeOutTimer = DateTime.Now;
                        }
                    }
                    break;

                case Action.ActionType.ACTION_AUTOCROP:
                    {
                        Log.Debug("ACTION_AUTOCROP");
                        _statusVisible = true;
                        _statusTimeOutTimer = DateTime.Now;

                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1,
                                                        0, 0, null);
                        IAutoCrop cropper = GUIGraphicsContext.autoCropper;
                        if (cropper != null)
                        {
                            msg.Label = cropper.Crop();
                            if (msg.Label == null)
                            {
                                msg.Label = "N/A";
                            }
                        }
                        else
                        {
                            msg.Label = "N/A";
                        }

                        OnMessage(msg);
                        break;
                    }

                case Action.ActionType.ACTION_TOGGLE_AUTOCROP:
                    {
                        Log.Debug("ACTION_TOGGLE_AUTOCROP");
                        _statusVisible = true;
                        _statusTimeOutTimer = DateTime.Now;
                        IAutoCrop cropper = GUIGraphicsContext.autoCropper;

                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1,
                                                        0, 0, null);
                        msg.Label = "N/A";

                        if (cropper != null)
                        {
                            msg.Label = cropper.ToggleMode();
                        }
                        OnMessage(msg);
                        break;
                    }

                case Action.ActionType.ACTION_ASPECT_RATIO:
                    {
                        _statusVisible = true;
                        _statusTimeOutTimer = DateTime.Now;
                        string status = "";

                        Geometry.Type arMode = GUIGraphicsContext.ARType;

                        bool foundMode = false;
                        for (int i = 0; i < _allowedArModes.Count; i++)
                        {
                            if (_allowedArModes[i] == arMode)
                            {
                                arMode = _allowedArModes[(i + 1) % _allowedArModes.Count]; // select next allowed mode
                                foundMode = true;
                                break;
                            }
                        }
                        if (!foundMode && _allowedArModes.Count > 0)
                        {
                            arMode = _allowedArModes[0];
                        }

                        GUIGraphicsContext.ARType = arMode;
                        status = Utils.GetAspectRatioLocalizedString(arMode);
                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1,
                                                        0, 0, null);
                        msg.Label = status;
                        OnMessage(msg);
                    }
                    break;

                case Action.ActionType.ACTION_NEXT_SUBTITLE:
                    if (g_Player.SubtitleStreams > 0 || g_Player.SupportsCC)
                    {
                        _statusVisible = true;
                        _statusTimeOutTimer = DateTime.Now;

                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1,
                                                        0, 0, null);
                        g_Player.SwitchToNextSubtitle();
                        if (g_Player.EnableSubtitle)
                        {
                            if (g_Player.CurrentSubtitleStream == -1 && g_Player.SupportsCC)
                            {
                                msg.Label = "CC1";
                            }
                            else
                            {
                                msg.Label = string.Format("{0} ({1}/{2})", g_Player.SubtitleLanguage(g_Player.CurrentSubtitleStream),
                                                          g_Player.CurrentSubtitleStream + 1, g_Player.SubtitleStreams);
                            }
                        }
                        else
                        {
                            msg.Label = GUILocalizeStrings.Get(519); // Subtitles off
                        }
                        OnMessage(msg);
                        Log.Info("MyTV toggle subtitle: switched subtitle to {0}", msg.Label);
                    }
                    else
                    {
                        Log.Info("MyTV toggle subtitle: no subtitle streams available!");
                    }
                    break;

                case Action.ActionType.ACTION_PAGE_UP:
                    OnPageUp();
                    break;

                case Action.ActionType.ACTION_PAGE_DOWN:
                    OnPageDown();
                    break;

                case Action.ActionType.ACTION_KEY_PRESSED:
                    {
                        // msn related can be removed
                        //if ((action.m_key != null) && (!_msnWindowVisible))
                        if (action.m_key != null)
                        {
                            OnKeyCode((char)action.m_key.KeyChar);
                        }

                        _messageBoxVisible = false;
                    }
                    break;

                case Action.ActionType.ACTION_REWIND:
                    {
                        if (g_Player.IsTimeShifting || g_Player.IsTVRecording)
                        {
                            _isPauseOsdVisible = false;
                            GUIWindowManager.IsPauseOsdVisible = false;
                            ScreenStateChanged();
                            UpdateGUI();
                        }
                    }
                    break;

                case Action.ActionType.ACTION_FORWARD:
                    {
                        if (g_Player.IsTimeShifting || g_Player.IsTVRecording)
                        {
                            _isPauseOsdVisible = false;
                            GUIWindowManager.IsPauseOsdVisible = false;
                            ScreenStateChanged();
                            UpdateGUI();
                        }
                    }
                    break;

                case Action.ActionType.ACTION_PREVIOUS_MENU:
                case Action.ActionType.ACTION_SHOW_GUI:
                    Log.Debug("fullscreentv:show gui");
                    GUIWindowManager.ShowPreviousWindow();
                    return;

                case Action.ActionType.ACTION_SHOW_OSD: // Show the OSD
                    {
                        Log.Debug("OSD:ON");
                        ShowMainOSD();
                    }
                    break;

                case Action.ActionType.ACTION_MOVE_LEFT:
                case Action.ActionType.ACTION_STEP_BACK:
                    {
                        if (g_Player.IsTimeShifting || g_Player.IsTVRecording)
                        {
                            if (g_Player.Paused)
                            {
                                g_Player.Pause();
                                _isPauseOsdVisible = false;
                                GUIWindowManager.IsPauseOsdVisible = false;
                                ScreenStateChanged();
                                UpdateGUI();
                            }
                            _stepSeekVisible = true;
                            _statusTimeOutTimer = DateTime.Now;
                            g_Player.SeekStep(false);
                            string strStatus = g_Player.GetStepDescription();
                            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0,
                                                            (int)Control.LABEL_ROW1, 0, 0, null);
                            msg.Label = strStatus;
                            OnMessage(msg);
                        }
                    }
                    break;

                case Action.ActionType.ACTION_MOVE_RIGHT:
                case Action.ActionType.ACTION_STEP_FORWARD:
                    {
                        if (g_Player.IsTimeShifting || g_Player.IsTVRecording)
                        {
                            if (g_Player.Paused)
                            {
                                g_Player.Pause();
                                _isPauseOsdVisible = false;
                                GUIWindowManager.IsPauseOsdVisible = false;
                                ScreenStateChanged();
                                UpdateGUI();
                            }
                            _stepSeekVisible = true;
                            _statusTimeOutTimer = DateTime.Now;
                            g_Player.SeekStep(true);
                            string strStatus = g_Player.GetStepDescription();
                            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0,
                                                            (int)Control.LABEL_ROW1, 0, 0, null);
                            msg.Label = strStatus;
                            OnMessage(msg);
                        }
                    }
                    break;

                case Action.ActionType.ACTION_MOVE_DOWN:
                case Action.ActionType.ACTION_BIG_STEP_BACK:
                    {
                        if (g_Player.IsTimeShifting || g_Player.IsTVRecording)
                        {
                            if (g_Player.Paused)
                            {
                                g_Player.Pause();
                                _isPauseOsdVisible = false;
                                GUIWindowManager.IsPauseOsdVisible = false;
                                ScreenStateChanged();
                                UpdateGUI();
                            }
                            _statusVisible = true;
                            _statusTimeOutTimer = DateTime.Now;
                            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0,
                                                              (int)Control.LABEL_ROW1, 0, 0, null);
                            msg.Label = "";
                            OnMessage(msg);
                            if (_immediateSeekIsRelative)
                            {
                                g_Player.SeekRelativePercentage(-_immediateSeekValue);
                            }
                            else
                            {
                                g_Player.SeekRelative(-_immediateSeekValue);
                            }
                        }
                    }
                    break;

                case Action.ActionType.ACTION_MOVE_UP:
                case Action.ActionType.ACTION_BIG_STEP_FORWARD:
                    {
                        if (g_Player.IsTimeShifting || g_Player.IsTVRecording)
                        {
                            if (g_Player.Paused)
                            {
                                g_Player.Pause();
                                _isPauseOsdVisible = false;
                                GUIWindowManager.IsPauseOsdVisible = false;
                                ScreenStateChanged();
                                UpdateGUI();
                            }
                            _statusVisible = true;
                            _statusTimeOutTimer = DateTime.Now;
                            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0,
                                                              (int)Control.LABEL_ROW1, 0, 0, null);
                            msg.Label = "";
                            OnMessage(msg);
                            if (_immediateSeekIsRelative)
                            {
                                g_Player.SeekRelativePercentage(_immediateSeekValue);
                            }
                            else
                            {
                                g_Player.SeekRelative(_immediateSeekValue);
                            }
                        }
                    }
                    break;

                case Action.ActionType.ACTION_PAUSE:
                    {
                        ScreenStateChanged();
                        UpdateGUI();
                        if (g_Player.Paused)
                        {
                            if ((GUIGraphicsContext.Vmr9Active && VMR9Util.g_vmr9 != null))
                            {
                                VMR9Util.g_vmr9.SetRepaint();
                                VMR9Util.g_vmr9.Repaint(); // repaint vmr9
                            }
                            _osdTimeoutTimer = DateTime.Now;
                            _isPauseOsdVisible = true;
                            GUIWindowManager.IsPauseOsdVisible = true;
                        }
                        else
                        {
                            _isPauseOsdVisible = false;
                            GUIWindowManager.IsPauseOsdVisible = false;
                        }
                    }
                    break;

                case Action.ActionType.ACTION_PLAY:
                case Action.ActionType.ACTION_MUSIC_PLAY:
                    {
                        _isPauseOsdVisible = false;
                        GUIWindowManager.IsPauseOsdVisible = false;
                        break;
                    }


                case Action.ActionType.ACTION_CONTEXT_MENU:
                    ShowContextMenu();
                    break;

                case Action.ActionType.ACTION_AUTOZAP:
                    StartAutoZap();
                    break;

                case Action.ActionType.ACTION_AUDIO_NEXT_LANGUAGE:
                case Action.ActionType.ACTION_NEXT_AUDIO:
                    {
                        //IAudioStream[] streams = TVHome.Card.AvailableAudioStreams;

                        if (g_Player.AudioStreams > 1)
                        {
                            int newIndex = 0;
                            int oldIndex = 0;
                            string audioLang = g_Player.AudioLanguage(oldIndex);
                            oldIndex = g_Player.CurrentAudioStream;
                            g_Player.SwitchToNextAudio();

                            newIndex = g_Player.CurrentAudioStream;

                            if (newIndex + 1 > g_Player.AudioStreams)
                            {
                                newIndex = 0;
                            }

                            Log.Debug("Switching from audio stream {0} to {1}", oldIndex, newIndex);

                            // Show OSD Label
                            _statusVisible = true;
                            _statusTimeOutTimer = DateTime.Now;
                            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0,
                                                            (int)Control.LABEL_ROW1, 0, 0, null);
                            //msg.Label = string.Format("{0}:{1} ({2}/{3})", streams[newIndex].StreamType, streams[newIndex].Language, newIndex + 1, streams.Length);
                            msg.Label = string.Format("{0}:{1} ({2}/{3})", g_Player.AudioType(newIndex),
                                                      g_Player.AudioLanguage(newIndex), newIndex + 1, g_Player.AudioStreams);

                            Log.Debug(msg.Label);
                            OnMessage(msg);
                        }
                    }
                    break;

                case Action.ActionType.ACTION_STOP:
                    if (!g_Player.Playing && !g_Player.IsTVRecording 
                        && PluginMain.Navigator.LastChannelChangeFailed)
                    {
                        GUIWindowManager.ActivateWindow(WindowId.TvHome);
                    }
                    g_Player.Stop();
                    break;

                case Action.ActionType.ACTION_PREV_ITEM:
                case Action.ActionType.ACTION_PREV_CHAPTER:
                    JumpToPrevChapter();
                    break;

                case Action.ActionType.ACTION_NEXT_ITEM:
                case Action.ActionType.ACTION_NEXT_CHAPTER:
                    JumpToNextChapter();
                    break;
            }

            base.OnAction(action);
        }

        public override void SetObject(object obj)
        {
            base.SetObject(obj);
        }

        private void OnSelectChannel()
        {
            MiniGuide miniGuide = (MiniGuide)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_MINI_GUIDE);
            _isDialogVisible = true;
            miniGuide.ChannelType = ChannelType.Television;
            miniGuide.DoModal(GetID);
            if (!miniGuide.Canceled)
            {
                PluginMain.Navigator.SetCurrentGroup(miniGuide.ChannelType, miniGuide.SelectedGroup);
                PluginMain.Navigator.ZapToChannel(miniGuide.SelectedChannel, false);
            }
            _isDialogVisible = false;
        }

        public override bool OnMessage(GUIMessage message)
        {
            _needToClearScreen = true;

            #region case GUI_MSG_RECORD

            if (message.Message == GUIMessage.MessageType.GUI_MSG_RECORD)
            {
                if (_isDialogVisible)
                {
                    return false;
                }

                Channel currentChannel = PluginMain.Navigator.CurrentChannel;
                GuideProgram currentProgram = PluginMain.GetCurrentProgram(ChannelType.Television);
                if (currentChannel != null)
                {
                    ActiveRecording activeRecording;
                    if (currentProgram == null
                        || !PluginMain.IsActiveRecording(currentChannel.ChannelId, currentProgram, out activeRecording))
                    {
                        PluginMain.IsChannelRecording(currentChannel.ChannelId, out activeRecording);
                    }
                    if (activeRecording != null)
                    {
                        _dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                        _dlgYesNo.SetHeading(1449); // stop recording
                        _dlgYesNo.SetLine(1, 1450); // are you sure to stop recording?
                        _dlgYesNo.SetLine(2, activeRecording.Program.Title);
                        _dialogYesNoVisible = true;
                        _dlgYesNo.DoModal(GetID);
                        _dialogYesNoVisible = false;

                        if (_dlgYesNo.IsConfirmed)
                        {
                            using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                            {
                                Schedule schedule = tvSchedulerAgent.GetScheduleById(activeRecording.Program.ScheduleId);
                                if (schedule != null)
                                {
                                    if (activeRecording.Program.IsPartOfSeries)
                                    {
                                        tvSchedulerAgent.CancelUpcomingProgram(schedule.ScheduleId,
                                            currentProgram == null ? null : (Guid?)currentProgram.GuideProgramId,
                                            currentChannel.ChannelId, activeRecording.Program.StartTime);
                                    }
                                    else
                                    {
                                        tvSchedulerAgent.DeleteSchedule(schedule.ScheduleId);
                                    }
                                    string text = String.Format("{0} {1}-{2}",
                                          activeRecording.Program.Title,
                                          activeRecording.Program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                          activeRecording.Program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                                    ShowRecordingNotifyDialog(tvSchedulerAgent, currentChannel, text, TextId.RecordingStopped);
                                }
                            }
                        }
                    }
                    else
                    {
                        _dialogMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                        if (_dialogMenu != null)
                        {
                            using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                            {
                                _dialogMenu.Reset();
                                _dialogMenu.SetHeading(Utility.GetLocalizedText(TextId.Record));
                                if (currentProgram != null)
                                {
                                    _dialogMenu.Add(Utility.GetLocalizedText(TextId.CurrentProgram));
                                }
                                _dialogMenu.Add("15 " + Utility.GetLocalizedText(TextId.Minutes));
                                _dialogMenu.Add("30 " + Utility.GetLocalizedText(TextId.Minutes));
                                _dialogMenu.Add("1 " + Utility.GetLocalizedText(TextId.Hour));
                                _dialogMenu.Add("1 " + Utility.GetLocalizedText(TextId.Hour) + " 30 " + Utility.GetLocalizedText(TextId.Minutes));
                                _dialogMenu.Add("2 " + Utility.GetLocalizedText(TextId.Hours));
                                _bottomDialogMenuVisible = true;

                                _dialogMenu.DoModal(GetID);

                                _bottomDialogMenuVisible = false;

                                Schedule schedule = null;
                                string notifyText = String.Empty;

                                int selectedLabel = _dialogMenu.SelectedLabel;
                                if (currentProgram == null)
                                {
                                    selectedLabel++;
                                }
                                switch (selectedLabel)
                                {
                                    case 0:
                                        schedule = GuideController.CreateRecordOnceSchedule(tvSchedulerAgent,
                                            PluginMain.Navigator.CurrentChannel.ChannelType,
                                            PluginMain.Navigator.CurrentChannel.ChannelId, currentProgram.Title, currentProgram.SubTitle,
                                            currentProgram.EpisodeNumberDisplay, currentProgram.StartTime);
                                        notifyText = String.Format("{0} {1}-{2}",
                                              currentProgram.Title,
                                              currentProgram.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                              currentProgram.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                                        break;

                                    case 1:
                                        schedule = CreateManualSchedule(currentChannel, 15, out notifyText);
                                        break;

                                    case 2:
                                        schedule = CreateManualSchedule(currentChannel, 30, out notifyText);
                                        break;

                                    case 3:
                                        schedule = CreateManualSchedule(currentChannel, 60, out notifyText);
                                        break;

                                    case 4:
                                        schedule = CreateManualSchedule(currentChannel, 90, out notifyText);
                                        break;

                                    case 5:
                                        schedule = CreateManualSchedule(currentChannel, 2 * 60, out notifyText);
                                        break;
                                }

                                if (schedule != null)
                                {
                                    tvSchedulerAgent.SaveSchedule(schedule);
                                    ShowRecordingNotifyDialog(tvSchedulerAgent, currentChannel, notifyText, TextId.RecordingStarted);
                                }
                            }
                        }
                    }
                }
                return true;
            }

            #endregion

            #region case GUI_MSG_RECORDER_ABOUT_TO_START_RECORDING

            if (message.Message == GUIMessage.MessageType.GUI_MSG_RECORDER_ABOUT_TO_START_RECORDING)
            {
                /*
                        TVRecording rec = message.Object as TVRecording;
                        if (rec == null) return true;
                        if (rec.Channel == Recorder.TVChannelName) return true;
                        if (!Recorder.NeedChannelSwitchForRecording(rec)) return true;

                        _messageBoxVisible = false;
                        _msnWindowVisible = false;     // msn related can be removed
                        GUIWindowManager.IsOsdVisible = false;
                        if (_zapOsdVisible)
                        {
                          GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, _zapWindow.GetID, 0, 0, GetID, 0, null);
                          _zapWindow.OnMessage(msg);
                          _zapOsdVisible = false;
                          GUIWindowManager.IsOsdVisible = false;
                        }
                        if (_isOsdVisible)
                        {
                          GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, _osdWindow.GetID, 0, 0, GetID, 0, null);
                          _osdWindow.OnMessage(msg);
                          _isOsdVisible = false;
                          GUIWindowManager.IsOsdVisible = false;
                        }
                        if (_msnWindowVisible)     // msn related can be removed
                        {
                          GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, _msnWindow.GetID, 0, 0, GetID, 0, null);
                          _msnWindow.OnMessage(msg);	// Send a de-init msg to the OSD
                          _msnWindowVisible = false;
                          GUIWindowManager.IsOsdVisible = false;
                        }
                        if (_isDialogVisible && dlg != null)
                        {
                          GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, dlg.GetID, 0, 0, GetID, 0, null);
                          dlg.OnMessage(msg);	// Send a de-init msg to the OSD
                        }

                        _bottomDialogMenuVisible = true;
                        _dialogBottomMenu = (GUIDialogMenuBottomRight)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU_BOTTOM_RIGHT);
                        _dialogBottomMenu.TimeOut = 10;
                        _dialogBottomMenu.SetHeading(1004);//About to start recording
                        _dialogBottomMenu.SetHeadingRow2(String.Format("{0} {1}", GUILocalizeStrings.Get(1005), rec.Channel));
                        _dialogBottomMenu.SetHeadingRow3(rec.Title);
                        _dialogBottomMenu.AddLocalizedString(1006); //Allow recording to begin
                        _dialogBottomMenu.AddLocalizedString(1007); //Cancel recording and maintain watching tv
                        _dialogBottomMenu.DoModal(GetID);
                        if (_dialogBottomMenu.SelectedId == 1007) //cancel recording
                        {
                          if (rec.RecType == TVRecording.RecordingType.Once)
                          {
                            rec.Canceled = Utils.datetolong(DateTime.Now);
                          }
                          else
                          {
                            Program prog = message.Object2 as Program;
                            if (prog != null)
                              rec.CanceledSeries.Add(prog.Start);
                            else
                              rec.CanceledSeries.Add(Utils.datetolong(DateTime.Now));
                          }
                          TVDatabase.UpdateRecording(rec, TVDatabase.RecordingChange.Canceled);
                        }
                 */
                _bottomDialogMenuVisible = false;
            }

            #endregion

            #region case GUI_MSG_NOTIFY

            if (message.Message == GUIMessage.MessageType.GUI_MSG_NOTIFY)
            {
                GUIDialogNotify dlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_NOTIFY);
                if (dlgNotify == null)
                {
                    return true;
                }
                string channel = GUIPropertyManager.GetProperty("#TV.View.channel");
                string strLogo = Utils.GetCoverArt(Thumbs.TVChannel, channel);
                dlgNotify.Reset();
                dlgNotify.ClearAll();
                dlgNotify.SetImage(strLogo);
                dlgNotify.SetHeading(channel);
                dlgNotify.SetText(message.Label);
                dlgNotify.TimeOut = message.Param1;
                _notifyDialogVisible = true;
                dlgNotify.DoModal(GUIWindowManager.ActiveWindow);
                _notifyDialogVisible = false;
                Log.Debug("Notify Message:" + channel + ", " + message.Label);
                return true;
            }

            #endregion

            #region case GUI_MSG_TV_ERROR_NOTIFY

            // TEST for TV error handling
            if (message.Message == GUIMessage.MessageType.GUI_MSG_TV_ERROR_NOTIFY)
            {
                UpdateOSD(message.Object);
                return true;
            }

            #endregion

            #region case GUI_MSG_WINDOW_DEINIT

            if (_isOsdVisible)
            {
                if ((message.Message != GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT))
                {
                    _osdTimeoutTimer = DateTime.Now;
                    // route messages to OSD window
                    if (_osdWindow.OnMessage(message))
                    {
                        return true;
                    }
                }
                else if (message.Param1 == GetID)
                {
                    _osdTimeoutTimer = DateTime.Now;
                    _osdWindow.OnMessage(message);
                }
            }

            #endregion

            switch (message.Message)
            {
                #region case GUI_MSG_HIDE_MESSAGE

                case GUIMessage.MessageType.GUI_MSG_HIDE_MESSAGE:
                    {
                        _messageBoxVisible = false;
                    }
                    break;

                #endregion

                #region case GUI_MSG_SHOW_MESSAGE

                case GUIMessage.MessageType.GUI_MSG_SHOW_MESSAGE:
                    {
                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0,
                                                        (int)Control.MSG_BOX_LABEL1, 0, 0, null);
                        msg.Label = message.Label;
                        OnMessage(msg);

                        msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.MSG_BOX_LABEL2, 0, 0,
                                             null);
                        msg.Label = message.Label2;
                        OnMessage(msg);

                        msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.MSG_BOX_LABEL3, 0, 0,
                                             null);
                        msg.Label = message.Label3;
                        OnMessage(msg);

                        msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.MSG_BOX_LABEL4, 0, 0,
                                             null);
                        msg.Label = message.Label4;
                        OnMessage(msg);

                        _messageBoxVisible = true;
                        // Set specified timeout
                        _msgBoxTimeout = message.Param1;
                        _msgTimer = DateTime.Now;
                    }
                    break;

                #endregion

                #region case GUI_MSG_WINDOW_DEINIT

                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT:
                    {
                        lock (this)
                        {
                            Log.Debug("TvFullScreen:deinit->OSD:Off");
                            HideMainOSD();

                            _isOsdVisible = false;
                            _isPauseOsdVisible = false;
                            GUIWindowManager.IsOsdVisible = false;
                            GUIWindowManager.IsPauseOsdVisible = false;
                            _channelInputVisible = false;
                            _keyPressedTimer = DateTime.Now;
                            _channelName = "";

                            _stepSeekVisible = false;
                            _statusVisible = false;
                            //_groupVisible = false;
                            _notifyDialogVisible = false;
                            _dialogYesNoVisible = false;
                            _bottomDialogMenuVisible = false;
                            _statusTimeOutTimer = DateTime.Now;

                            _screenState.ContextMenuVisible = false;
                            _screenState.MsgBoxVisible = false;
                            _screenState.OsdVisible = false;
                            _screenState.Paused = false;
                            _screenState.ShowGroup = false;
                            _screenState.ShowInput = false;
                            _screenState.ShowStatusLine = false;
                            _screenState.ShowTime = false;
                            _screenState.ZapOsdVisible = false;
                            _needToClearScreen = false;

                            GUIGraphicsContext.IsFullScreenVideo = false;
                            GUILayerManager.UnRegisterLayer(this);

                            base.OnMessage(message);
                        }
                        return true;
                    }

                #endregion

                #region case GUI_MSG_WINDOW_INIT

                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    {
                        base.OnMessage(message);
                        if (!SettingsLoaded)
                            LoadSettings();
                        GUIGraphicsContext.IsFullScreenVideo = true;

                        _osdWindow = (TvOsd)GUIWindowManager.GetWindow((int)Window.WINDOW_TVOSD);
                        _zapWindow = (TvZapOsd)GUIWindowManager.GetWindow((int)Window.WINDOW_TVZAPOSD);

                        _lastPause = g_Player.Paused;
                        _lastSpeed = g_Player.Speed;

                        Log.Debug("TvFullScreen:init->OSD:Off");
                        Log.Debug("TvFullScreen: init, playing {0}, player.CurrentFile {1}", g_Player.Playing, g_Player.CurrentFile);

                        _isOsdVisible = false;
                        GUIWindowManager.IsOsdVisible = false;
                        _channelInputVisible = false;
                        _keyPressedTimer = DateTime.Now;
                        _channelName = "";

                        _isPauseOsdVisible = _lastPause;
                        GUIWindowManager.IsPauseOsdVisible = _lastPause;
                        //_zapTimeOutTimer=DateTime.Now;
                        _osdTimeoutTimer = DateTime.Now;

                        _stepSeekVisible = false;
                        _statusVisible = false;
                        //_groupVisible = false;
                        _notifyDialogVisible = false;
                        _dialogYesNoVisible = false;
                        _bottomDialogMenuVisible = false;
                        _statusTimeOutTimer = DateTime.Now;
                        //imgVolumeBar.Current = VolumeHandler.Instance.Step;
                        //imgVolumeBar.Maximum = VolumeHandler.Instance.StepMax;

                        ResetAllControls(); // make sure the controls are positioned relevant to the OSD Y offset
                        ScreenStateChanged();
                        UpdateGUI();

                        GUIGraphicsContext.IsFullScreenVideo = true;
                        GUILayerManager.RegisterLayer(this, GUILayerManager.LayerType.Osd);

                        RenderVolume(false);

                        //return base.OnMessage(message);
                        return true;
                    }

                #endregion

                #region case GUI_MSG_SETFOCUS

                case GUIMessage.MessageType.GUI_MSG_SETFOCUS:
                    goto case GUIMessage.MessageType.GUI_MSG_LOSTFOCUS;

                #endregion

                #region case GUI_MSG_LOSTFOCUS

                case GUIMessage.MessageType.GUI_MSG_LOSTFOCUS:
                    if (_isOsdVisible)
                    {
                        return true;
                    }
                    if (message.SenderControlId != (int)GUIWindow.Window.WINDOW_TVFULLSCREEN)
                    {
                        return true;
                    }
                    break;

                #endregion
            }
            return base.OnMessage(message);
        }

        private void ShowRecordingNotifyDialog(SchedulerServiceAgent tvSchedulerAgent,
            Channel currentChannel, string text, TextId headingTextId)
        {
            if (!_enableRecNotification)
            {
                GUIDialogNotify dlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                if (dlgNotify != null)
                {
                    string logo = Utility.GetLogoImage(currentChannel, tvSchedulerAgent);
                    dlgNotify.Reset();
                    dlgNotify.ClearAll();
                    dlgNotify.SetImage(logo);
                    dlgNotify.SetHeading(Utility.GetLocalizedText(headingTextId));
                    dlgNotify.SetText(text);
                    dlgNotify.TimeOut = 5;
                    _notifyDialogVisible = true;
                    dlgNotify.DoModal(GUIWindowManager.ActiveWindow);
                    _notifyDialogVisible = false;
                }
            }
        }

        private Schedule CreateManualSchedule(Channel channel, int durationMinutes, out string notifyText)
        {
            using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
            {
                Schedule schedule = tvSchedulerAgent.CreateNewSchedule(ChannelType.Television, ScheduleType.Recording);
                DateTime startTime = DateTime.Now;
                TimeSpan duration = new TimeSpan(0, durationMinutes, 0);
                schedule.Rules.Add(ScheduleRuleType.Channels, channel.ChannelId);
                schedule.Rules.Add(ScheduleRuleType.ManualSchedule, startTime, new ScheduleTime(duration));
                schedule.Name = String.Format(CultureInfo.CurrentCulture, "{0} {1:g}-{2:t}", channel.DisplayName, startTime, startTime.Add(duration));
                notifyText = schedule.Name;
                return schedule;
            }
        }

        void ShowContextMenu()
        {
            if (dlg == null)
                dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return;
            dlg.Reset();
            dlg.SetHeading(924); // menu

            if (!g_Player.IsTVRecording && GUIGraphicsContext.DBLClickAsRightClick)
            {
                dlg.AddLocalizedString(10104); // TV MiniEPG
            }

            dlg.AddLocalizedString(4); // TV Guide

            //TvBusinessLayer layer = new TvBusinessLayer();
            //IList linkages = layer.GetLinkagesForChannel(TVHome.Navigator.Channel);
            //if (linkages != null)
            //    if (linkages.Count > 0)
            //        dlg.AddLocalizedString(200042); // Linked Channels

            eAudioDualMonoMode dualMonoMode = g_Player.GetAudioDualMonoMode();

            if ((PluginMain.Navigator.IsLiveStreamOn && !PluginMain.Navigator.LastChannelChangeFailed) || g_Player.IsTVRecording)
            {
                if (!g_Player.IsTVRecording
                    && PluginMain.Navigator.HasTeletext())
                {
                    dlg.AddLocalizedString(1441); // Fullscreen teletext
                }

                dlg.AddLocalizedString(941); // Change aspect ratio

                if (g_Player.AudioStreams > 0)
                {
                    dlg.AddLocalizedString(492); // Audio language menu
                }

                if (dualMonoMode != eAudioDualMonoMode.UNSUPPORTED)
                {
                    dlg.AddLocalizedString(200059); // Audio dual mono mode menu
                }

                // SubTitle stream, show only when there exists any streams,
                //    dialog shows then the streams and an item to disable them
                if (g_Player.SubtitleStreams > 0 || g_Player.SupportsCC)
                {
                    dlg.AddLocalizedString(462);
                }

                // If the decoder supports postprocessing features (FFDShow)
                if (g_Player.HasPostprocessing)
                {
                    dlg.AddLocalizedString(200073);
                }

                dlg.AddLocalizedString(11000);  // Crop settings

                if (!g_Player.IsTVRecording)
                {
                    dlg.AddLocalizedString(100748); // Program Information
                }

                if (!g_Player.IsTVRecording && Utils.FileExistsInCache(GUIGraphicsContext.Skin + @"\ARGUS_TuningDetails.xml"))
                {
                    dlg.AddLocalizedString(200041); // tuning details
                }

                // TODO
                //if (!g_Player.IsTVRecording)
                //{
                //    dlg.AddLocalizedString(601);    //Record Now
                //}

                if (g_Player.HasChapters) // For recordings with chapters
                {
                    dlg.AddLocalizedString(200091);
                }

                if (!g_Player.IsTVRecording && Utils.FileExistsInCache(GUIGraphicsContext.Skin + @"\ARGUS_ChannelManagment.xml"))
                {
                    dlg.Add(Utility.GetLocalizedText(TextId.ChannelManager));
                }
            }
            //dlg.AddLocalizedString(368); // IMDB
            dlg.AddLocalizedString(970); // Previous window

            _isDialogVisible = true;

            dlg.DoModal(GetID);
            _isDialogVisible = false;

            Log.Debug("selected id:{0}", dlg.SelectedId);
            if (dlg.SelectedId == -1) return;

            if (dlg.SelectedLabelText == Utility.GetLocalizedText(TextId.ChannelManager))
            {
              GUIWindowManager.ActivateWindow(WindowId.ChannelManagment);
            }
            else
            {
              switch (dlg.SelectedId)
              {
                case 4: //TVGuide
                  {
                    GUIWindowManager.ActivateWindow(WindowId.TvGuide);
                    break;
                  }

                case 10104: // MiniEPG
                  {

                    Log.Debug("get miniguide");
                    MiniGuide miniGuide = (MiniGuide)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_MINI_GUIDE);
                    miniGuide.ChannelType = ChannelType.Television;
                    _isDialogVisible = true;
                    Log.Debug("show miniguide");
                    miniGuide.DoModal(GetID);
                    Log.Debug("done miniguide");
                    _isDialogVisible = false;
                    break;
                  }

                case 941: // Change aspect ratio
                  ShowAspectRatioMenu();
                  break;

                case 492: // Show audio language menu
                  ShowAudioLanguageMenu();
                  break;

                case 200059:
                  ShowAudioDualMonoModeMenu(dualMonoMode);
                  break;

                case 1441: // Fullscreen teletext
                  GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_FULLSCREEN_TELETEXT);
                  break;

                case 970:
                  // switch back to previous window
                  _isOsdVisible = false;
                  GUIWindowManager.IsOsdVisible = false;
                  GUIGraphicsContext.IsFullScreenVideo = false;
                  GUIWindowManager.ShowPreviousWindow();
                  break;

                case 11000:
                  TvCropSettings cropSettings = (TvCropSettings)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_TV_CROP_SETTINGS);
                  _isDialogVisible = true;
                  cropSettings.DoModal(GetID);
                  _isDialogVisible = false;
                  break;

                case 100748: // Show Program Info
                  ShowProgramInfo();
                  break;

                case 601: // RecordNow
                  GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_RECORD, GUIWindowManager.ActiveWindow, 0, 0, 0, 0, null);
                  this.OnMessage(msg);
                  break;

                //case 200042: // Linked channels
                //    Gentle.Common.CacheManager.Clear();
                //    linkages = layer.GetLinkagesForChannel(TVHome.Navigator.Channel);
                //    ShowLinkedChannelsMenu(linkages);
                //    break;

                case 200041: // tuning details
                  GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_TV_TUNING_DETAILS);
                  break;

                case 200091:
                  ShowChapterStreamsMenu();
                  break;

                case 462:
                  ShowSubtitleStreamsMenu();
                  break;

                case 200073:
                  ShowPostProcessingMenu();
                  break;
              }
            }
        }

        private void ShowChapterStreamsMenu()
        {
            if (dlg == null)
            {
                dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
            }
            if (dlg == null)
            {
                return;
            }
            dlg.Reset();
            dlg.SetHeading(200091); // Chapters Streams

            // Previous chapter
            dlg.Add(String.Format("{0}", GUILocalizeStrings.Get(975)));
            // Next chapter
            dlg.Add(String.Format("{0}", GUILocalizeStrings.Get(976)));

            //List all chapters
            double[] chaptersList = new double[0];
            //List all chapters Name
            string[] chaptersname = new string[0];

            chaptersname = g_Player.ChaptersName;
            chaptersList = g_Player.Chapters;
            for (int i = 0; i < chaptersList.Length; i++)
            {
                GUIListItem item = new GUIListItem();
                if (chaptersname == null)
                {
                    item.Label = (String.Format("{0} #{1}", GUILocalizeStrings.Get(200091), (i + 1)));
                    item.Label2 = global::MediaPortal.Util.Utils.SecondsToHMSString((int)chaptersList[i]);
                    dlg.Add(item);
                }
                else
                {
                    if (string.IsNullOrEmpty(chaptersname[i]))
                    {
                        item.Label = (String.Format("{0} #{1}", GUILocalizeStrings.Get(200091), (i + 1)));
                        item.Label2 = global::MediaPortal.Util.Utils.SecondsToHMSString((int)chaptersList[i]);
                        dlg.Add(item);
                    }
                    else
                    {
                        item.Label = (String.Format("{0} #{1}: {2}", GUILocalizeStrings.Get(200091), (i + 1), chaptersname[i]));
                        item.Label2 = global::MediaPortal.Util.Utils.SecondsToHMSString((int)chaptersList[i]);
                        dlg.Add(item);
                    }
                }
            }

            // show dialog and wait for result
            _isDialogVisible = true;
            dlg.DoModal(GetID);
            _isDialogVisible = false;

            if (dlg.SelectedId == -1)
            {
                return;
            }
            else if (dlg.SelectedLabel == 0)
            {
                Action actionPrevChapter = new Action(Action.ActionType.ACTION_PREV_CHAPTER, 0, 0);
                GUIGraphicsContext.OnAction(actionPrevChapter);
            }
            else if (dlg.SelectedLabel == 1)
            {
                Action actionNextChapter = new Action(Action.ActionType.ACTION_NEXT_CHAPTER, 0, 0);
                GUIGraphicsContext.OnAction(actionNextChapter);
            }
            else
            {
                // get selected Chapters
                int selectedChapterIndex = dlg.SelectedLabel - 2;

                // set mplayers play position
                g_Player.SeekAbsolute(chaptersList[selectedChapterIndex]);
            }
        }

        private void ShowProgramInfo()
        {
            GuideProgram currentProgram = PluginMain.GetCurrentProgram(ChannelType.Television);

            if (currentProgram == null)
            {
                return;
            }

            TvProgramInfo.Channel = PluginMain.Navigator.CurrentChannel;
            TvProgramInfo.CurrentProgram = currentProgram;
            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_TV_PROGRAM_INFO);
        }

        private void ShowAspectRatioMenu()
        {
            if (dlg == null)
            {
                return;
            }
            dlg.Reset();
            dlg.SetHeading(941); // Change aspect ratio

            // Add the allowed zoom  modes
            if (_allowedArModes.Contains(Geometry.Type.Stretch))
            {
                dlg.AddLocalizedString(942); // Stretch
            }
            if (_allowedArModes.Contains(Geometry.Type.Normal))
            {
                dlg.AddLocalizedString(943); // Normal
            }
            if (_allowedArModes.Contains(Geometry.Type.Original))
            {
                dlg.AddLocalizedString(944); // Original
            }
            if (_allowedArModes.Contains(Geometry.Type.LetterBox43))
            {
                dlg.AddLocalizedString(945); // Letterbox
            }
            if (_allowedArModes.Contains(Geometry.Type.NonLinearStretch))
            {
                dlg.AddLocalizedString(946); // Smart stretch
            }
            if (_allowedArModes.Contains(Geometry.Type.Zoom))
            {
                dlg.AddLocalizedString(947); // Zoom
            }
            if (_allowedArModes.Contains(Geometry.Type.Zoom14to9))
            {
                dlg.AddLocalizedString(1190); //14:9
            }

            // set the focus to currently used mode
            dlg.SelectedLabel = dlg.IndexOfItem(Utils.GetAspectRatioLocalizedString(GUIGraphicsContext.ARType));
            // show dialog and wait for result       
            _isDialogVisible = true;
            dlg.DoModal(GetID);
            _isDialogVisible = false;

            if (dlg.SelectedId == -1)
            {
                return;
            }
            _statusTimeOutTimer = DateTime.Now;

            string strStatus = "";

            GUIGraphicsContext.ARType = Utils.GetAspectRatioByLangID(dlg.SelectedId);
            strStatus = GUILocalizeStrings.Get(dlg.SelectedId);


            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1, 0, 0,
                                            null);
            msg.Label = strStatus;
            OnMessage(msg);
        }

        #region CI Menu

        /// <summary>
        /// Sets callbacks and calls EnterCiMenu; Actions are done from callbacks and handled in TVHome globally
        /// </summary>
        //private void PrepareCiMenu()
        //{
        //    TVHome.RegisterCiMenu(TVHome.Card.Id); // Ensure listener attached
        //    TVHome.Card.EnterCiMenu(); // Enter menu. Dialog shows up on callback
        //}

        #endregion

        //private void SortChannels()
        //{
        //    GUIWindowManager.ActivateWindow((int)Window.WINDOW_SETTINGS_SORT_CHANNELS);
        //    //ChannelSettings channelSettings = (ChannelSettings)GUIWindowManager.GetWindow((int)Window.WINDOW_SETTINGS_SORT_CHANNELS);
        //}

        private void ShowAudioLanguageMenu()
        {
            if (dlg == null)
            {
                return;
            }
            dlg.Reset();
            dlg.SetHeading(492); // set audio language menu

            dlg.ShowQuickNumbers = true;
            int selected = 0;
            int nrOfstreams = 0;

            int streamCurrent = g_Player.CurrentAudioStream;
            nrOfstreams = g_Player.AudioStreams;

            Log.Debug("TvFullScreen: ShowAudioLanguageMenu - got {0} audio streams", nrOfstreams);

            if (nrOfstreams >= streamCurrent)
            {
                selected = streamCurrent;
            }

            for (int i = 0; i < g_Player.AudioStreams; i++)
            {
                GUIListItem item = new GUIListItem();
                item.Label = String.Format("{0}: {1}", g_Player.AudioLanguage(i), g_Player.AudioType(i));
                dlg.Add(item);
            }

            dlg.SelectedLabel = selected;

            _isDialogVisible = true;

            dlg.DoModal(GetID);
            _isDialogVisible = false;

            if (dlg.SelectedLabel < 0)
            {
                return;
            }

            // Set new language			
            if ((dlg.SelectedLabel >= 0) && (dlg.SelectedLabel < nrOfstreams))
            {
                g_Player.CurrentAudioStream = dlg.SelectedLabel;
            }
        }

        private void ShowAudioDualMonoModeMenu(eAudioDualMonoMode dualMonoMode)
        {
            if (dlg == null)
            {
                return;
            }
            dlg.Reset();
            dlg.SetHeading(200059); // audio dual mono mode 

            dlg.AddLocalizedString(200060); // stereo 
            dlg.AddLocalizedString(200061); // Left channel to the left and right speakers
            dlg.AddLocalizedString(200062); // Right channel to the left and right speakers
            dlg.AddLocalizedString(200063); // Mix both

            dlg.SelectedLabel = (int)dualMonoMode;

            dlg.DoModal(GetID);

            if (dlg.SelectedLabel < 0)
            {
                return;
            }
            g_Player.SetAudioDualMonoMode((eAudioDualMonoMode)dlg.SelectedLabel);
        }

        //private void ShowLinkedChannelsMenu(IList<ChannelLinkageMap> linkages)
        //{
        //    if (dlg == null)
        //    {
        //        return;
        //    }
        //    dlg.Reset();
        //    dlg.SetHeading(200042); // Linked channels menu
        //    int selected = 0;
        //    int counter = 0;
        //    foreach (ChannelLinkageMap map in linkages)
        //    {
        //        GUIListItem item = new GUIListItem(map.DisplayName);
        //        if (map.IdLinkedChannel == TVHome.Navigator.Channel.IdChannel)
        //        {
        //            selected = counter;
        //        }
        //        dlg.Add(item);
        //        counter++;
        //    }
        //    dlg.SelectedLabel = selected;
        //    _isDialogVisible = true;
        //    dlg.DoModal(GetID);
        //    _isDialogVisible = false;
        //    if (dlg.SelectedLabel < 0)
        //    {
        //        return;
        //    }
        //    ChannelLinkageMap lmap = (ChannelLinkageMap)linkages[dlg.SelectedLabel];
        //    TVHome.Navigator.ZapToChannel(lmap.ReferringLinkedChannel(), false);
        //}

        private void ShowSubtitleStreamsMenu()
        {
            if (dlg == null)
            {
                return;
            }
            dlg.Reset();
            dlg.SetHeading(462); // SubTitle Streams

            dlg.AddLocalizedString(519); // disable Subtitles

            if (g_Player.SupportsCC)
            {
                dlg.Add("CC1");
            }

            // get the number of subtitles in the current movie
            int nbSubStreams = g_Player.SubtitleStreams;
            // cycle through each subtitle and add it to our list control
            for (int i = 0; i < nbSubStreams; ++i)
            {
                // remove (English) in: "English (English)", should be done by gplayer
                string strLang = g_Player.SubtitleLanguage(i);
                int ipos = strLang.IndexOf("(");
                if (ipos > 0)
                {
                    strLang = strLang.Substring(0, ipos);
                }
                dlg.Add(strLang);
            }

            // select/focus the subtitle, which is active atm.
            // There may be no subtitle streams selected at all (-1), which happens when a subtitle file is used instead
            if (g_Player.EnableSubtitle && nbSubStreams > 0)
            {
                if (g_Player.SupportsCC)
                {
                    dlg.SelectedLabel = g_Player.CurrentSubtitleStream + 2;
                }
                else
                {
                    int subStream = g_Player.CurrentSubtitleStream;
                    if (subStream != -1) dlg.SelectedLabel = subStream + 1;
                }
            }
            else
                dlg.SelectedLabel = 0;

            // show dialog and wait for result
            _isDialogVisible = true;
            dlg.DoModal(GetID);
            _isDialogVisible = false;

            if (dlg.SelectedId == -1)
            {
                return;
            }
            if (dlg.SelectedLabel == 0)
            {
                g_Player.EnableSubtitle = false;
            }
            else if (g_Player.SupportsCC && dlg.SelectedLabel == 1 && g_Player.CurrentSubtitleStream != -1)
            {
                g_Player.CurrentSubtitleStream = -1;
                g_Player.EnableSubtitle = true;
            }
            else
            {
                int i = 1;
                if (g_Player.SupportsCC)
                {
                    i = 2;
                }
                if (dlg.SelectedLabel != g_Player.CurrentSubtitleStream + i)
                {
                    Log.Info("Subtitle stream selected : " + (dlg.SelectedLabel - i));
                    g_Player.CurrentSubtitleStream = dlg.SelectedLabel - i;
                }
                g_Player.EnableSubtitle = true;
            }
        }

        private void ShowPostProcessingMenu()
        {
            if (dlg == null)
            {
                return;
            }

            do
            {
                dlg.Reset();
                dlg.SetHeading(200073); // Postprocessing
                IPostProcessingEngine engine = PostProcessingEngine.GetInstance();
                // Deblocking
                dlg.Add(String.Format("{0} {1}", GUILocalizeStrings.Get(200074), (engine.EnablePostProcess) ? GUILocalizeStrings.Get(461) : ""));
                // Resize
                dlg.Add(String.Format("{0} {1}", GUILocalizeStrings.Get(200075), (engine.EnableResize) ? GUILocalizeStrings.Get(461) : ""));
                // Crop
                dlg.Add(String.Format("{0} {1}", GUILocalizeStrings.Get(200078), (engine.EnableCrop) ? GUILocalizeStrings.Get(461) : ""));
                // Deinterlace
                dlg.Add(String.Format("{0} {1}", GUILocalizeStrings.Get(200077), (engine.EnableDeinterlace) ? GUILocalizeStrings.Get(461) : ""));
                dlg.AddLocalizedString(970); // Previous window
                dlg.SelectedLabel = 0;

                // show dialog and wait for result
                _isDialogVisible = true;
                dlg.DoModal(GetID);
                if (dlg.SelectedId == 970)
                {
                    // switch back to previous window
                    _isDialogVisible = false;
                    GUIWindowManager.ShowPreviousWindow();
                    return;
                }

                switch (dlg.SelectedLabel)
                {
                    case 0: engine.EnablePostProcess = !engine.EnablePostProcess; break;
                    case 1: engine.EnableResize = !engine.EnableResize; break;
                    case 2: engine.EnableCrop = !engine.EnableCrop; break;
                    case 3: engine.EnableDeinterlace = !engine.EnableDeinterlace; break;
                }
            } while (dlg.SelectedId != -1);
            _isDialogVisible = false;
        }

        public override void Process()
        {
            CheckTimeOuts();
            if (ScreenStateChanged())
            {
                UpdateGUI();
            }

            if ((_statusVisible || _stepSeekVisible || (!_isOsdVisible && g_Player.Speed != 1) ||
                 (!_isOsdVisible && _isPauseOsdVisible)) || _isOsdVisible)
            {
                TvHome.UpdateProgressPercentageBar();
            }
            else
            {
                // in fullscreen TV we still have to update the properties - since external displays depend on these.
                // one update per. minute should be enough
                TimeSpan tsProgressBar = DateTime.Now - _updateTimerProgressbar;

                if (tsProgressBar.TotalMilliseconds > 30000)
                {
                    TvHome.UpdateProgressPercentageBar();
                    _updateTimerProgressbar = DateTime.Now;
                }
            }

            if (!VideoRendererStatistics.IsVideoFound)
            {
                if (_lastChannelWithNoSignalId != PluginMain.Navigator.CurrentChannel.ChannelId
                    || videoState != VideoRendererStatistics.VideoState)
                {
                    if (!_zapOsdVisible)
                    {
                        GUIMessage message = new GUIMessage(GUIMessage.MessageType.GUI_MSG_NOTIFY, GetID, GetID, 0, 5, 0, null);
                        switch (VideoRendererStatistics.VideoState)
                        {
                            case VideoRendererStatistics.State.NoSignal:
                                message.Label = GUILocalizeStrings.Get(1034);
                                break;
                            case VideoRendererStatistics.State.Scrambled:
                                message.Label = GUILocalizeStrings.Get(1035);
                                break;
                            case VideoRendererStatistics.State.Signal:
                                message.Label = GUILocalizeStrings.Get(1036);
                                break;
                            default:
                                message.Label = GUILocalizeStrings.Get(1036);
                                break;
                        }
                        _lastChannelWithNoSignalId = PluginMain.Navigator.CurrentChannel.ChannelId;
                        videoState = VideoRendererStatistics.VideoState;
                        OnMessage(message);
                    }
                }
            }
            else
            {
                _lastChannelWithNoSignalId = Guid.Empty;
                videoState = VideoRendererStatistics.State.VideoPresent;
            }

            //check if the tv recording has reached the end ...if so, stop it.
            if (g_Player.IsTVRecording)
            {
                Recording rec = TvRecorded.GetPlayingRecording();
                bool isLiveRecording = (rec != null && PluginMain.IsRecordingStillActive(rec.RecordingId));
                if (!isLiveRecording)
                {
                    double currentPosition = (double)(g_Player.CurrentPosition);
                    double duration = (double)(g_Player.Duration);

                    if (currentPosition > duration)
                    {
                        g_Player.Stop();
                    }
                }
            }

            //TVHome.ShowCiMenu();

            if (!g_Player.Playing && !PluginMain.Navigator.LastChannelChangeFailed)
            {
                Log.Debug("Tvfullscreen:not viewing anymore");
                if (GUIWindowManager.GetPreviousActiveWindow() == (int)GUIWindow.Window.WINDOW_TV_TUNING_DETAILS)
                {
                    GUIWindowManager.ActivateWindow(WindowId.TvHome);
                }
                else
                {
                    GUIWindowManager.ShowPreviousWindow();
                }
            }
            else
                GUIGraphicsContext.IsFullScreenVideo = true;
        }

        public bool ScreenStateChanged()
        {
            bool updateGUI = false;
            if (g_Player.Speed != _screenState.Speed)
            {
                _screenState.Speed = g_Player.Speed;
                updateGUI = true;
            }
            if (g_Player.Paused != _screenState.Paused)
            {
                _screenState.Paused = g_Player.Paused;
                updateGUI = true;
            }
            if (_isOsdVisible != _screenState.OsdVisible)
            {
                _screenState.OsdVisible = _isOsdVisible;
                updateGUI = true;
            }
            if (_zapOsdVisible != _screenState.ZapOsdVisible)
            {
                _screenState.ZapOsdVisible = _zapOsdVisible;
                updateGUI = true;
            }

            if (_isDialogVisible != _screenState.ContextMenuVisible)
            {
                _screenState.ContextMenuVisible = _isDialogVisible;
                updateGUI = true;
            }

            bool bStart, bEnd;
            int step = g_Player.GetSeekStep(out bStart, out bEnd);
            if (step != _screenState.SeekStep)
            {
                if (step != 0)
                {
                    _stepSeekVisible = true;
                }
                else
                {
                    _stepSeekVisible = false;
                }
                _screenState.SeekStep = step;
                updateGUI = true;
            }
            if (_statusVisible != _screenState.ShowStatusLine)
            {
                _screenState.ShowStatusLine = _statusVisible;
                updateGUI = true;
            }
            if (_bottomDialogMenuVisible != _screenState._bottomDialogMenuVisible)
            {
                _screenState._bottomDialogMenuVisible = _bottomDialogMenuVisible;
                updateGUI = true;
            }
            if (_notifyDialogVisible != _screenState._notifyDialogVisible)
            {
                _screenState._notifyDialogVisible = _notifyDialogVisible;
                updateGUI = true;
            }
            if (_messageBoxVisible != _screenState.MsgBoxVisible)
            {
                _screenState.MsgBoxVisible = _messageBoxVisible;
                updateGUI = true;
            }
            //if (_groupVisible != _screenState.ShowGroup)
            //{
            //    _screenState.ShowGroup = _groupVisible;
            //    updateGUI = true;
            //}
            if (_channelInputVisible != _screenState.ShowInput)
            {
                _screenState.ShowInput = _channelInputVisible;
                updateGUI = true;
            }
            if (_isVolumeVisible != _screenState.volumeVisible)
            {
                _screenState.volumeVisible = _isVolumeVisible;
                updateGUI = true;
                _volumeTimer = DateTime.Now;
            }
            if (_dialogYesNoVisible != _screenState._dialogYesNoVisible)
            {
                _screenState._dialogYesNoVisible = _dialogYesNoVisible;
                updateGUI = true;
            }

            if (updateGUI)
            {
                _needToClearScreen = true;
            }
            return updateGUI;
        }

        private void UpdateGUI()
        {
            // Set recorder status
            // TODO
            //VirtualCard card;
            //var server = new TvServer();
            //if (server.IsRecording(TVHome.Navigator.Channel.IdChannel, out card))
            //{
            //    ShowControl(GetID, (int)Control.REC_LOGO);
            //}
            //else
            {
                HideControl(GetID, (int)Control.REC_LOGO);
            }

            int speed = g_Player.Speed;
            HideControl(GetID, (int)Control.IMG_2X);
            HideControl(GetID, (int)Control.IMG_4X);
            HideControl(GetID, (int)Control.IMG_8X);
            HideControl(GetID, (int)Control.IMG_16X);
            HideControl(GetID, (int)Control.IMG_32X);
            HideControl(GetID, (int)Control.IMG_MIN2X);
            HideControl(GetID, (int)Control.IMG_MIN4X);
            HideControl(GetID, (int)Control.IMG_MIN8X);
            HideControl(GetID, (int)Control.IMG_MIN16X);
            HideControl(GetID, (int)Control.IMG_MIN32X);

            switch (speed)
            {
                case 2:
                    ShowControl(GetID, (int)Control.IMG_2X);
                    break;
                case 4:
                    ShowControl(GetID, (int)Control.IMG_4X);
                    break;
                case 8:
                    ShowControl(GetID, (int)Control.IMG_8X);
                    break;
                case 16:
                    ShowControl(GetID, (int)Control.IMG_16X);
                    break;
                case 32:
                    ShowControl(GetID, (int)Control.IMG_32X);
                    break;
                case -2:
                    ShowControl(GetID, (int)Control.IMG_MIN2X);
                    break;
                case -4:
                    ShowControl(GetID, (int)Control.IMG_MIN4X);
                    break;
                case -8:
                    ShowControl(GetID, (int)Control.IMG_MIN8X);
                    break;
                case -16:
                    ShowControl(GetID, (int)Control.IMG_MIN16X);
                    break;
                case -32:
                    ShowControl(GetID, (int)Control.IMG_MIN32X);
                    break;
            }

            HideControl(GetID, (int)Control.LABEL_ROW1);
            HideControl(GetID, (int)Control.LABEL_ROW2);
            HideControl(GetID, (int)Control.LABEL_ROW3);
            HideControl(GetID, (int)Control.BLUE_BAR);
            if (_screenState.SeekStep != 0)
            {
                ShowControl(GetID, (int)Control.BLUE_BAR);
                ShowControl(GetID, (int)Control.LABEL_ROW1);
            }
            if (_statusVisible)
            {
                ShowControl(GetID, (int)Control.BLUE_BAR);
                ShowControl(GetID, (int)Control.LABEL_ROW1);
            }
            //if (_groupVisible)
            //{
            //    ShowControl(GetID, (int)Control.BLUE_BAR);
            //    ShowControl(GetID, (int)Control.LABEL_ROW1);
            //}
            //if (_channelInputVisible)
            //{
            //  ShowControl(GetID, (int)Control.LABEL_ROW1);
            //}
            HideControl(GetID, (int)Control.MSG_BOX);
            HideControl(GetID, (int)Control.MSG_BOX_LABEL1);
            HideControl(GetID, (int)Control.MSG_BOX_LABEL2);
            HideControl(GetID, (int)Control.MSG_BOX_LABEL3);
            HideControl(GetID, (int)Control.MSG_BOX_LABEL4);

            if (_messageBoxVisible)
            {
                ShowControl(GetID, (int)Control.MSG_BOX);
                ShowControl(GetID, (int)Control.MSG_BOX_LABEL1);
                ShowControl(GetID, (int)Control.MSG_BOX_LABEL2);
                ShowControl(GetID, (int)Control.MSG_BOX_LABEL3);
                ShowControl(GetID, (int)Control.MSG_BOX_LABEL4);
            }

            RenderVolume(_isVolumeVisible);
        }

        private void CheckTimeOuts()
        {
            if (_isVolumeVisible)
            {
                TimeSpan ts = DateTime.Now - _volumeTimer;
                // mantis 0002467: Keep Mute Icon on screen if muting is ON 
                if (ts.TotalSeconds >= 3 && !VolumeHandler.Instance.IsMuted)
                {
                    RenderVolume(false);
                }
            }
            //if (_groupVisible)
            //{
            //    TimeSpan ts = (DateTime.Now - _groupTimeOutTimer);
            //    if (ts.TotalMilliseconds >= _zapTimeOutValue)
            //    {
            //        _groupVisible = false;
            //    }
            //}

            if (_statusVisible || _stepSeekVisible)
            {
                TimeSpan ts = (DateTime.Now - _statusTimeOutTimer);
                if (ts.TotalMilliseconds >= 2000)
                {
                    _stepSeekVisible = false;
                    _statusVisible = false;
                }
            }

            // OSD Timeout?
            if (_isOsdVisible && _timeOsdOnscreen > 0)
            {
                TimeSpan ts = DateTime.Now - _osdTimeoutTimer;
                if (ts.TotalMilliseconds > _timeOsdOnscreen)
                {
                    //yes, then remove osd offscreen
                    HideMainOSD();
                }
            }
            if (g_Player.Paused && _timeOsdOnscreen > 0)
            {
                TimeSpan ts = DateTime.Now - _osdTimeoutTimer;
                if (ts.TotalMilliseconds > _timeOsdOnscreen)
                {
                    _isPauseOsdVisible = false;
                    GUIWindowManager.IsPauseOsdVisible = false;
                }
                else
                {
                    _isPauseOsdVisible = true;
                    GUIWindowManager.IsPauseOsdVisible = true;
                }
            }

            OnKeyTimeout();

            if (_messageBoxVisible && _msgBoxTimeout > 0)
            {
                TimeSpan ts = DateTime.Now - _msgTimer;
                if (ts.TotalSeconds > _msgBoxTimeout)
                {
                    _messageBoxVisible = false;
                }
            }

            // Let the navigator zap channel if needed
            if (PluginMain.Navigator.CheckChannelChange())
            {
                TvHome.UpdateProgressPercentageBar(true);
            }

            //Log.Debug("osd visible:{0} timeoutvalue:{1}", _zapOsdVisible ,_zapTimeOutValue);
            if (_zapOsdVisible && _zapTimeOutValue > 0)
            {
                TimeSpan ts = DateTime.Now - _zapTimeOutTimer;
                //Log.Debug("timeout :{0}", ts.TotalMilliseconds);
                if (ts.TotalMilliseconds > _zapTimeOutValue)
                {
                    //yes, then remove osd offscreen
                    Log.Debug("ZAP OSD:Off timeout");
                    HideZapOSD();
                }
            }
        }

        public override void Render(float timePassed)
        {
            if (GUIWindowManager.IsSwitchingToNewWindow)
            {
                return;
            }
            if (GUIGraphicsContext.Vmr9Active)
            {
                base.Render(timePassed);
            }
            if (_zapOsdVisible)
            {
                _zapWindow.Render(timePassed);
            }
            else if (_isOsdVisible)
            {
                _osdWindow.Render(timePassed);
            }

            if (g_Player.Playing || PluginMain.Navigator.DoingChannelChange || PluginMain.Navigator.LastChannelChangeFailed)
            {
                return;
            }

            if (_isStartingTSForRecording)
            {
                return;
            }

            //close window
            //GUIMessage msg2 = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, _osdWindow.GetID, 0, 0, GetID, 0,
            //                                 null);
            //_osdWindow.OnMessage(msg2); // Send a de-init msg to the OSD
            //msg2 = null;
            Log.Debug("timeout->OSD:Off");
            //_isOsdVisible = false;
            //GUIWindowManager.IsOsdVisible = false;
            HideMainOSD();
            HideZapOSD();

            //close window
            ///@
            /// msg2 = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, _msnWindow.GetID, 0, 0, GetID, 0, null);
            ///_msnWindow.OnMessage(msg2);	// Send a de-init msg to the OSD
            /// msg2 = null;

            //_msnWindowVisible = false;     // msn related can be removed

            GUIWindowManager.IsOsdVisible = false;
            //Log.Debug("Tvfullscreen:not viewing anymore");
            //GUIWindowManager.ShowPreviousWindow();
        }

        public void UpdateOSD()
        {
            UpdateOSD(null);
        }

        public void UpdateOSD(object message)
        {
            if (GUIWindowManager.ActiveWindow != GetID)
            {
                return;
            }
            Log.Debug("UpdateOSD()");
            if (!_zapOsdVisible && !g_Player.IsTVRecording)
            {
                _isPauseOsdVisible = false;
                GUIWindowManager.IsPauseOsdVisible = false;
                ShowZapOSD(message);
            }
            else
            {
                if (message != null)
                {
                    _zapWindow.LastError = message.ToString();
                }
                _zapWindow.UpdateChannelInfo();
                _zapTimeOutTimer = DateTime.Now;
            }
        }

        public void ShowZapOSD(object errorInfo)
        {
            if (!_zapOsdVisible)
            {
                GUIMessage msg;
                // Show zap OSD
                msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_INIT, _zapWindow.GetID, 0, 0,
                                                GetID, 0, null);
                if (errorInfo != null)
                {
                    _zapWindow.LastError = errorInfo.ToString();
                }
                _zapWindow.OnMessage(msg);
                Log.Debug("ZAP OSD:ON");
                _zapTimeOutTimer = DateTime.Now;
                _zapOsdVisible = true;
                // now even if main OSD is visible it won't be rendered

                if (_isOsdVisible)
                {
                    // hide main OSD
                    msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, _osdWindow.GetID, 0, 0,
                                         GetID, 0, null);
                    _osdWindow.OnMessage(msg); // Send a de-init msg to the OSD
                    GUIWindowManager.IsOsdVisible = false; // do we need this?
                }
                //GUIWindowManager.VisibleOsd = Window.WINDOW_TVZAPOSD;
            }
        }

        public void HideZapOSD()
        {
            // Hide zap OSD
            if (_zapOsdVisible)
            {
                GUIMessage msg;
                //GUIWindowManager.IsOsdVisible = false;
                // Show main OSD if it was temporarily hidden
                if (_isOsdVisible)
                {
                    msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_INIT, _osdWindow.GetID, 0, 0, GetID, 0,
                                         null);
                    _osdWindow.OnMessage(msg); // Send an init msg to the OSD
                    GUIWindowManager.VisibleOsd = Window.WINDOW_TVOSD;
                }

                msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, _zapWindow.GetID, 0, 0,
                                     GetID, 0, null);
                _zapWindow.OnMessage(msg);
                _zapOsdVisible = false;
            }

            if (g_Player.Paused)
            {
                _isPauseOsdVisible = true;
                GUIWindowManager.IsPauseOsdVisible = true;
            }
        }

        public void ShowMainOSD()
        {
            if (!_isOsdVisible)
            {
                _osdTimeoutTimer = DateTime.Now;
                if (!_zapOsdVisible)
                {
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_INIT, _osdWindow.GetID, 0, 0, GetID, 0,
                                                    null);
                    _osdWindow.OnMessage(msg); // Send an init msg to the OSD
                    GUIWindowManager.VisibleOsd = Window.WINDOW_TVOSD;
                }
                _isOsdVisible = true;
            }
        }

        public void HideMainOSD()
        {
            if (_isOsdVisible)
            {
                _isOsdVisible = false; // make sure osd won't be rendered
                if (!_zapOsdVisible)
                {
                    GUIWindowManager.IsOsdVisible = false;
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT, _osdWindow.GetID, 0, 0, GetID,
                                                    0, null);
                    _osdWindow.OnMessage(msg); // Send a de-init msg to the OSD
                }
            }
        }

        public void RenderForm(float timePassed)
        {
            if (_needToClearScreen)
            {
                _needToClearScreen = false;
                GUIGraphicsContext.graphics.Clear(Color.Black);
            }
            base.Render(timePassed);
            if (GUIGraphicsContext.graphics != null)
            {
                if (_isDialogVisible)
                {
                    dlg.Render(timePassed);
                }
            }
            // do we need 2 render the OSD?

            if (_zapOsdVisible)
            {
                _zapWindow.Render(timePassed);
            }
            else if (_isOsdVisible)
            {
                _osdWindow.Render(timePassed);
            }
        }

        private void HideControl(int idSender, int idControl)
        {
            GUIControl cntl = base.GetControl(idControl);
            if (cntl != null)
            {
                cntl.Visible = false;
            }
            cntl = null;
        }

        private void ShowControl(int idSender, int idControl)
        {
            GUIControl cntl = base.GetControl(idControl);
            if (cntl != null)
            {
                cntl.Visible = true;
            }
            cntl = null;
        }

        private void OnKeyTimeout()
        {
            if (_channelName.Length == 0)
            {
                return;
            }
            TimeSpan ts = DateTime.Now - _keyPressedTimer;
            if (ts.TotalMilliseconds >= 1000 || _channelName.Length == _channelNumberMaxLength)
            {
                // change channel
                int iChannel = -1;
                Int32.TryParse(_channelName, out iChannel);
                if (iChannel > -1)
                {
                    ChangeChannelNr(iChannel);
                }
                //HideZapOSD();
                _channelInputVisible = false;
                PluginMain.Navigator.ZapChannelNr = -1;
                _channelName = String.Empty;
            }
        }

        public void OnKeyCode(char chKey)
        {
            if (g_Player.IsTVRecording)
            {
                return;
            }

            if (_isDialogVisible)
            {
                return;
            }
            if (GUIWindowManager.IsRouted)
            {
                return;
            }
            if (chKey == 'o')
            {
                Action showInfo = new Action(Action.ActionType.ACTION_SHOW_CURRENT_TV_INFO, 0, 0);
                OnAction(showInfo);
                return;
            }
            if (chKey == '0' && !_channelInputVisible)
            {
                PluginMain.Navigator.ZapToLastViewedChannel(ChannelType.Television);
                UpdateOSD();
                return;
            }
            if (chKey >= '0' && chKey <= '9') //Make sure it's only for the remote
            {
                _channelInputVisible = true;

                _keyPressedTimer = DateTime.Now;
                _channelName += chKey;

                int channelNr = Int32.Parse(_channelName);
                PluginMain.Navigator.ZapChannelNr = channelNr;
                UpdateOSD();
            }
        }

        private void OnPageDown()
        {
            // Switch to the next channel group and tune to the first channel in the group

            if (g_Player.IsTVRecording)
            {
                return;
            }

            OnSelectChannel();

            // TODO
            //TVHome.Navigator.ZapToPreviousGroup(true);
            //_groupVisible = true;
            //_groupTimeOutTimer = DateTime.Now;
            //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1, 0, 0,
            //                                null);
            //msg.Label = String.Format("{0}:{1}", GUILocalizeStrings.Get(971), TVHome.Navigator.ZapGroupName);
            //OnMessage(msg);
        }

        private void OnPageUp()
        {
            if (g_Player.IsTVRecording)
            {
                return;
            }

            OnSelectChannel();

            // TODO
            //// Switch to the next channel group and tune to the first channel in the group
            //TVHome.Navigator.ZapToNextGroup(true);
            //_groupVisible = true;
            //_groupTimeOutTimer = DateTime.Now;
            //GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1, 0, 0,
            //                                null);
            //msg.Label = String.Format("{0}:{1}", GUILocalizeStrings.Get(971), TVHome.Navigator.ZapGroupName);
            //OnMessage(msg);
        }

        private void ChangeChannelNr(int channelNr)
        {
            ChangeChannelNr(channelNr, false);
        }

        private void ChangeChannelNr(int channelNr, bool useZapDelay)
        {
            if (g_Player.IsTVRecording)
            {
                return;
            }

            Log.Debug("ChangeChannelNr()");
            if (_byIndex)
            {
                Channel channel = PluginMain.Navigator.GetChannelByIndex(ChannelType.Television, channelNr);
                if (channel != null)
                {
                    PluginMain.Navigator.ZapToChannel(channel, false);
                }
            }
            else
            {
                PluginMain.Navigator.ZapToChannelNumber(ChannelType.Television, channelNr, false);
            }

            UpdateOSD();
        }

        public void ZapPreviousChannel()
        {
            if (g_Player.IsTVRecording)
            {
                return;
            }
            Log.Debug("ZapPreviousChannel()");
            PluginMain.Navigator.ZapToPreviousChannel(true);
            UpdateOSD();
        }

        public void ZapNextChannel()
        {
            if (g_Player.IsTVRecording)
            {
                return;
            }

            Log.Debug("ZapNextChannel()");
            PluginMain.Navigator.ZapToNextChannel(true);
            UpdateOSD();
        }

        public void JumpToPrevChapter()
        {
            if (g_Player.IsTVRecording)
            {
                if (g_Player.HasChapters)
                {
                    double position = g_Player.CurrentPosition;
                    if (g_Player.JumpToPrevChapter())
                    {
                        ShowChapterSkipInfo(position);
                    }
                }
            }
        }

        public void JumpToNextChapter()
        {
            if (g_Player.IsTVRecording)
            {
                if (g_Player.HasChapters)
                {
                    double position = g_Player.CurrentPosition;
                    if (g_Player.JumpToNextChapter())
                    {
                        ShowChapterSkipInfo(position);
                    }
                }
            }
        }

        private void ShowChapterSkipInfo(double position)
        {
            DateTime maxWaitTime = DateTime.Now.AddMilliseconds(200);
            double jump = g_Player.CurrentPosition - position;
            if (jump != 0)
            {
                _statusVisible = true;
                _statusTimeOutTimer = DateTime.Now;

                string sign = (jump >= 0 ? "+ " : "- ");
                TimeSpan span = TimeSpan.FromSeconds(Math.Abs(jump));

                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Control.LABEL_ROW1, 0, 0, null);
                msg.Label = sign + span.Minutes.ToString() + ":" + span.Seconds.ToString("00");

                OnMessage(msg);
                ResetUpdateGuiTimer();
            }
        }


        private void ResetUpdateGuiTimer()
        {
            _updateTimer = DateTime.Now.AddSeconds(-10);
        }

        public void StartAutoZap()
        {
            if (g_Player.IsTVRecording)
            {
                return;
            }

            Log.Debug("TVFullscreen: Start autozap mode");
            _autoZapMode = true;
            _autoZapTimer.Elapsed += new ElapsedEventHandler(_autoZapTimer_Elapsed);
            using (Settings xmlreader = new MPSettings())
            {
                _autoZapTimer.Interval = xmlreader.GetValueAsInt("capture", "autoZapTimer", 10000);
            }
            _autoZapTimer.Start();
            _autoZapTimer_Elapsed(null, null);
        }

        private void _autoZapTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_autoZapMode)
            {
                ZapNextChannel();
            }
            else
            {
                _autoZapTimer.Stop();
            }
        }

        public void StopAutoZap()
        {
            Log.Debug("Stop zap mode");
            _autoZapTimer.Elapsed -= new ElapsedEventHandler(_autoZapTimer_Elapsed);
            _autoZapMode = false;
        }

        public override int GetFocusControlId()
        {
            if (_isOsdVisible && !_zapOsdVisible)
            {
                return _osdWindow.GetFocusControlId();
            }

            return base.GetFocusControlId();
        }

        public override GUIControl GetControl(int iControlId)
        {
            if (_isOsdVisible && !_zapOsdVisible)
            {
                return _osdWindow.GetControl(iControlId);
            }

            return base.GetControl(iControlId);
        }

        protected override void OnPageDestroy(int newWindowId)
        {
            _autoZapMode = false;
            _autoZapTimer.Dispose();
            base.OnPageDestroy(newWindowId);
        }

        protected override void OnPageLoad()
        {
            _autoZapTimer = new Timer();
            base.OnPageLoad();
        }

        private void RenderVolume(bool show)
        {
            if (imgVolumeBar == null)
            {
                return;
            }

            if (!show)
            {
                _isVolumeVisible = false;
                imgVolumeBar.Visible = false;
                imgVolumeMuteIcon.Visible = false;
                return;
            }
            else
            {
                if (VolumeHandler.Instance.IsMuted)
                {
                    imgVolumeBar.Maximum = VolumeHandler.Instance.StepMax;
                    imgVolumeBar.Current = 0;
                    imgVolumeMuteIcon.Visible = true;
                    imgVolumeBar.Image1 = 1;
                }
                else
                {
                    imgVolumeBar.Maximum = VolumeHandler.Instance.StepMax;
                    imgVolumeBar.Current = VolumeHandler.Instance.Step;
                    imgVolumeMuteIcon.Visible = false;
                    imgVolumeBar.Image1 = 2;
                    imgVolumeBar.Image2 = 1;
                }
                imgVolumeBar.Visible = true;
            }
        }

        #region IRenderLayer

        public bool ShouldRenderLayer()
        {
            return true;
        }

        public void RenderLayer(float timePassed)
        {
            Render(timePassed);
        }

        #endregion

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

        public bool OnActorInfoStarting(IMDBFetcher fetcher)
        {
            // won't occure
            return true;
        }

        public bool OnSelectActor(IMDBFetcher fetcher, out int selected)
        {
            // won't occure
            selected = 0;
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

        private void g_Player_PlayBackChanged(g_Player.MediaType type, int stoptime, string filename)
        {
            SettingsLoaded = false; // we should reload
        }

        private void g_Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
        {
            SettingsLoaded = false; // we should reload
            // playback was stopped, if we are the current window, close our context menu, so we also get closed
            if (type != g_Player.MediaType.Recording && type != g_Player.MediaType.TV) return;
            if (GUIWindowManager.ActiveWindow != GetID) return;
            if (!_IsClosingDialog)
            {
                _IsClosingDialog = true;
                GUIDialogWindow.CloseRoutedWindow();
                _IsClosingDialog = false;
            }
        }

        private void g_Player_PlayBackEnded(g_Player.MediaType type, string filename)
        {
            SettingsLoaded = false; // we should reload
            // playback ended, if we are the current window, close our context menu, so we also get closed
            if (type != g_Player.MediaType.Recording && type != g_Player.MediaType.TV) return;
            if (GUIWindowManager.ActiveWindow != GetID) return;
            if (!_IsClosingDialog)
            {
                _IsClosingDialog = true;
                GUIDialogWindow.CloseRoutedWindow();
                _IsClosingDialog = false;
            }
        }

        #region Properties
        private bool SettingsLoaded
        {
            get
            {
                return _settingsLoaded;
            }
            set
            {
                _settingsLoaded = value;
                //maybe additional logic?
            }
        }
        #endregion

    }
}
