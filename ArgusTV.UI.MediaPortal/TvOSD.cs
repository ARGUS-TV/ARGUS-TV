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

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Profile;
using MediaPortal.Util;
using Action = MediaPortal.GUI.Library.Action;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;

namespace ArgusTV.UI.MediaPortal
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class TvOsd : GUIInternalWindow
    {
        private enum Controls
        {
            OSD_VIDEOPROGRESS = 1,
            OSD_TIMEINFO = 100,
            Panel1 = 101,
            Panel2 = 150,
            OSD_PAUSE = 209,
            OSD_SKIPBWD = 210,
            OSD_REWIND = 211,
            OSD_STOP = 212,
            OSD_PLAY = 213,
            OSD_FFWD = 214,
            OSD_SKIPFWD = 215,
            OSD_MUTE = 216,
            OSD_SUBTITLES = 218,
            OSD_BOOKMARKS = 219,
            OSD_VIDEO = 220,
            OSD_AUDIO = 221,
            OSD_SUBMENU_BG_VOL = 300,
            OSD_SUBMENU_BG_SUBTITLES = 302,
            OSD_SUBMENU_BG_BOOKMARKS = 303,
            OSD_SUBMENU_BG_VIDEO = 304,
            OSD_SUBMENU_BG_AUDIO = 305,
            OSD_SUBMENU_NIB = 350,
            OSD_VOLUMESLIDER = 400,
            OSD_AVDELAY = 500,
            OSD_AVDELAY_LABEL = 550,
            OSD_CREATEBOOKMARK = 600,
            OSD_BOOKMARKS_LIST = 601,
            OSD_CLEARBOOKMARKS = 602,
            OSD_BOOKMARKS_LIST_LABEL = 650,
            OSD_VIDEOPOS = 700,
            OSD_SHARPNESS = 701,
            OSD_SATURATIONLABEL = 702,
            OSD_SATURATION = 703,
            OSD_BRIGHTNESS = 704,
            OSD_CONTRAST = 705,
            OSD_GAMMA = 706,
            OSD_SHARPNESSLABEL = 710,
            OSD_VIDEOPOS_LABEL = 750,
            OSD_BRIGHTNESSLABEL = 752,
            OSD_CONTRASTLABEL = 753,
            OSD_GAMMALABEL = 754,
            OSD_SUBTITLE_DELAY = 800,
            OSD_SUBTITLE_ONOFF = 801,
            OSD_SUBTITLE_LIST = 802,
            OSD_SUBTITLE_DELAY_LABEL = 850,
        } ;

        [SkinControl(10)] protected GUIImage imgTvChannelLogo = null;
        [SkinControl(31)] protected GUIButtonControl btnChannelUp = null;
        [SkinControl(32)] protected GUIButtonControl btnChannelDown = null;
        [SkinControl(33)] protected GUIButtonControl btnPreviousProgram = null;
        [SkinControl(34)] protected GUIButtonControl btnNextProgram = null;
        [SkinControl(35)] protected GUILabelControl lblCurrentChannel = null;
        [SkinControl(36)] protected GUITextControl tbOnTvNow = null;
        [SkinControl(37)] protected GUITextControl tbOnTvNext = null;
        [SkinControl(38)] protected GUITextScrollUpControl tbProgramDescription = null;
        [SkinControl(39)] protected GUIImage imgRecIcon = null;
        [SkinControl(100)] protected GUILabelControl lblCurrentTime = null;
        [SkinControl(501)] protected GUIListControl lstAudioStreamList = null;

        private bool isSubMenuVisible = false;
        private int m_iActiveMenu = 0;
        private int m_iActiveMenuButtonID = 0;
        private bool m_bNeedRefresh = false;
        private DateTime m_dateTime = DateTime.Now;
        private DateTime _RecIconLastCheck = DateTime.Now;
        private GuideProgram previousProgram = null;
        private bool _immediateSeekIsRelative = true;
        private int _immediateSeekValue = 10;
        private int m_subtitleDelay = 0;
        private int m_delayInterval = 0;

        public TvOsd()
        {
            GetID = (int)Window.WINDOW_TVOSD;
        }

        public override bool IsTv
        {
            get { return true; }
        }

        public override bool Init()
        {
            using (Settings xmlreader = new MPSettings())
            {
                _immediateSeekIsRelative = xmlreader.GetValueAsBool("movieplayer", "immediateskipstepsisrelative", true);
                _immediateSeekValue = xmlreader.GetValueAsInt("movieplayer", "immediateskipstepsize", 10);
            }
            bool bResult = Load(GUIGraphicsContext.Skin + @"\tvOSD.xml");
            return bResult;
        }

        public bool SubMenuVisible
        {
            get { return isSubMenuVisible; }
        }

        public override bool SupportsDelayedLoad
        {
            get { return false; }
        }

        public override void Render(float timePassed)
        {
            UpdateProgressBar();
            SetVideoProgress(); // get the percentage of playback complete so far
            Get_TimeInfo(); // show the time elapsed/total playing time
            SetRecorderStatus(); // BAV: fixing bug 1429: OSD is not updated with recording status 
            base.Render(timePassed); // render our controls to the screen
        }

        private void HideControl(int dwSenderId, int dwControlID)
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_HIDDEN, GetID, dwSenderId, dwControlID, 0, 0, null);
            OnMessage(msg);
        }

        private void ShowControl(int dwSenderId, int dwControlID)
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_VISIBLE, GetID, dwSenderId, dwControlID, 0, 0, null);
            OnMessage(msg);
        }

        private void FocusControl(int dwSenderId, int dwControlID, int dwParam)
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SETFOCUS, GetID, dwSenderId, dwControlID, dwParam,
                                            0, null);
            OnMessage(msg);
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                // translate movements (up, down, left right) back
                case Action.ActionType.ACTION_STEP_BACK:
                    action.wID = Action.ActionType.ACTION_MOVE_LEFT;
                    break;
                case Action.ActionType.ACTION_STEP_FORWARD:
                    action.wID = Action.ActionType.ACTION_MOVE_RIGHT;
                    break;
                case Action.ActionType.ACTION_BIG_STEP_BACK:
                    action.wID = Action.ActionType.ACTION_MOVE_DOWN;
                    break;
                case Action.ActionType.ACTION_BIG_STEP_FORWARD:
                    action.wID = Action.ActionType.ACTION_MOVE_UP;
                    break;
                case Action.ActionType.ACTION_OSD_SHOW_LEFT:
                    break;
                case Action.ActionType.ACTION_OSD_SHOW_RIGHT:
                    break;
                case Action.ActionType.ACTION_OSD_SHOW_UP:
                    break;
                case Action.ActionType.ACTION_OSD_SHOW_DOWN:
                    break;
                case Action.ActionType.ACTION_OSD_SHOW_SELECT:
                    break;

                case Action.ActionType.ACTION_OSD_HIDESUBMENU:
                    break;
                case Action.ActionType.ACTION_CONTEXT_MENU:
                case Action.ActionType.ACTION_PREVIOUS_MENU:
                case Action.ActionType.ACTION_SHOW_OSD:
                    {
                        if (isSubMenuVisible) // is sub menu on?
                        {
                            FocusControl(GetID, m_iActiveMenuButtonID, 0); // set focus to last menu button
                            ToggleSubMenu(0, m_iActiveMenu); // hide the currently active sub-menu
                        }
                        if (action.wID == Action.ActionType.ACTION_CONTEXT_MENU)
                        {
                            TvFullScreen tvWindow = (TvFullScreen)GUIWindowManager.GetWindow((int)Window.WINDOW_TVFULLSCREEN);
                            tvWindow.OnAction(new Action(Action.ActionType.ACTION_SHOW_OSD, 0, 0));
                            tvWindow.OnAction(action);
                        }
                        return;
                    }

                case Action.ActionType.ACTION_PAUSE:
                    {
                        // push a message through to this window to handle the remote control button
                        GUIMessage msgSet = new GUIMessage(GUIMessage.MessageType.GUI_MSG_CLICKED, GetID, (int)Controls.OSD_PAUSE,
                                                           (int)Controls.OSD_PAUSE, 0, 0, null);
                        OnMessage(msgSet);

                        if (g_Player.Paused)
                        {
                            GUIWindowManager.IsPauseOsdVisible = true;
                        }
                        else
                        {
                            GUIWindowManager.IsPauseOsdVisible = false;
                        }
                        return;
                    }


                case Action.ActionType.ACTION_PLAY:
                case Action.ActionType.ACTION_MUSIC_PLAY:
                    {
                        g_Player.Speed = 1; // Single speed
                        ToggleButton((int)Controls.OSD_REWIND, false); // pop all the relevant
                        ToggleButton((int)Controls.OSD_FFWD, false); // buttons back to
                        ToggleButton((int)Controls.OSD_PLAY, false); // their up state
                        ToggleButton((int)Controls.OSD_PLAY, false); // make sure play button is up (so it shows the play symbol)
                        GUIWindowManager.IsPauseOsdVisible = false;
                        return;
                    }


                case Action.ActionType.ACTION_STOP:
                    {
                        if (g_Player.IsTimeShifting)
                        {
                            Log.Debug("TvOSD: user request to stop");
                            g_Player.Stop();
                        }
                        if (g_Player.IsTVRecording)
                        {
                            Log.Debug("TvOSD: stop from recorded TV");
                            g_Player.Stop();
                        }
                        GUIWindowManager.IsPauseOsdVisible = false;
                        return;
                    }


                case Action.ActionType.ACTION_FORWARD:
                    {
                        // push a message through to this window to handle the remote control button
                        GUIMessage msgSet = new GUIMessage(GUIMessage.MessageType.GUI_MSG_CLICKED, GetID, (int)Controls.OSD_FFWD,
                                                           (int)Controls.OSD_FFWD, 0, 0, null);
                        OnMessage(msgSet);

                        GUIWindowManager.IsPauseOsdVisible = false;
                        return;
                    }


                case Action.ActionType.ACTION_REWIND:
                    {
                        // push a message through to this window to handle the remote control button
                        GUIMessage msgSet = new GUIMessage(GUIMessage.MessageType.GUI_MSG_CLICKED, GetID, (int)Controls.OSD_REWIND,
                                                           (int)Controls.OSD_REWIND, 0, 0, null);
                        OnMessage(msgSet);

                        GUIWindowManager.IsPauseOsdVisible = false;
                        return;
                    }


                case Action.ActionType.ACTION_OSD_SHOW_VALUE_PLUS:
                    {
                        // push a message through to this window to handle the remote control button
                        GUIMessage msgSet = new GUIMessage(GUIMessage.MessageType.GUI_MSG_CLICKED, GetID, (int)Controls.OSD_SKIPFWD,
                                                           (int)Controls.OSD_SKIPFWD, 0, 0, null);
                        OnMessage(msgSet);
                        return;
                    }

                case Action.ActionType.ACTION_OSD_SHOW_VALUE_MIN:
                    {
                        // push a message through to this window to handle the remote control button
                        GUIMessage msgSet = new GUIMessage(GUIMessage.MessageType.GUI_MSG_CLICKED, GetID, (int)Controls.OSD_SKIPBWD,
                                                           (int)Controls.OSD_SKIPBWD, 0, 0, null);
                        OnMessage(msgSet);
                        return;
                    }

                case Action.ActionType.ACTION_NEXT_CHANNEL:
                    {
                        GUIWindowManager.IsPauseOsdVisible = false;
                        OnNextChannel();
                        return;
                    }

                case Action.ActionType.ACTION_PREV_CHANNEL:
                    {
                        GUIWindowManager.IsPauseOsdVisible = false;
                        OnPreviousChannel();
                        return;
                    }
            }
            if ((action.wID == Action.ActionType.ACTION_KEY_PRESSED && action.m_key != null &&
                  (char)action.m_key.KeyChar >= '0' && (char)action.m_key.KeyChar <= '9'))
            {
                TvFullScreen TVWindow = (TvFullScreen)GUIWindowManager.GetWindow((int)Window.WINDOW_TVFULLSCREEN);
                if (TVWindow != null)
                {
                    TVWindow.OnKeyCode((char)action.m_key.KeyChar);
                }
            }
            base.OnAction(action);
        }

        public override bool OnMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_DEINIT: // fired when OSD is hidden
                    {
                        Dispose();
                        GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100000 + message.Param1));
                        return true;
                    }

                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT: // fired when OSD is shown
                    {
                        GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100000 + GetID));
                        previousProgram = null;
                        AllocResources();
                        ResetAllControls(); // make sure the controls are positioned relevant to the OSD Y offset
                        isSubMenuVisible = false;
                        m_iActiveMenuButtonID = 0;
                        m_iActiveMenu = 0;
                        m_bNeedRefresh = false;
                        m_dateTime = DateTime.Now;
                        Reset();
                        FocusControl(GetID, (int)Controls.OSD_PLAY, 0); // set focus to play button by default when window is shown
                        ShowPrograms();
                        QueueAnimation(AnimationType.WindowOpen);
                        for (int i = (int)Controls.Panel1; i < (int)Controls.Panel2; ++i)
                        {
                            ShowControl(GetID, i);
                        }
                        m_delayInterval = global::MediaPortal.Player.Subtitles.SubEngine.GetInstance().DelayInterval;
                        if (m_delayInterval > 0)
                            m_subtitleDelay = global::MediaPortal.Player.Subtitles.SubEngine.GetInstance().Delay / m_delayInterval;
                        return true;
                    }

                case GUIMessage.MessageType.GUI_MSG_SETFOCUS:
                    goto case GUIMessage.MessageType.GUI_MSG_LOSTFOCUS;

                case GUIMessage.MessageType.GUI_MSG_LOSTFOCUS:
                    {
                        if (message.SenderControlId == 13)
                        {
                            return true;
                        }
                    }
                    break;

                case GUIMessage.MessageType.GUI_MSG_CLICKED:
                    {
                        int iControl = message.SenderControlId; // get the ID of the control sending us a message

                        if (iControl == btnChannelUp.GetID)
                        {
                            OnNextChannel();
                        }

                        if (iControl == btnChannelDown.GetID)
                        {
                            OnPreviousChannel();
                        }

                        if (!g_Player.IsTVRecording)
                        {
                            if (iControl == btnPreviousProgram.GetID)
                            {
                                GuideProgram prog = PluginMain.GetProgramAt(m_dateTime);
                                if (prog != null)
                                {
                                    prog = PluginMain.GetProgramAt(prog.StartTime.Subtract(new TimeSpan(0, 1, 0)));
                                    if (prog != null)
                                    {
                                        m_dateTime = prog.StartTime.AddMinutes(1);
                                    }
                                }
                                ShowPrograms();
                            }
                            if (iControl == btnNextProgram.GetID)
                            {
                                GuideProgram prog = PluginMain.GetProgramAt(m_dateTime);
                                if (prog != null)
                                {
                                    prog = PluginMain.GetProgramAt(prog.StopTime.AddMinutes(+1));
                                    if (prog != null)
                                    {
                                        m_dateTime = prog.StartTime.AddMinutes(1);
                                    }
                                }
                                ShowPrograms();
                            }
                        }

                        if (iControl >= (int)Controls.OSD_VOLUMESLIDER)
                        // one of the settings (sub menu) controls is sending us a message
                        {
                            Handle_ControlSetting(iControl, message.Param1);
                        }

                        if (iControl == (int)Controls.OSD_PAUSE)
                        {
                            if (g_Player.Paused)
                            {
                                ToggleButton((int)Controls.OSD_PLAY, true);
                                // make sure play button is down (so it shows the pause symbol)                
                                ToggleButton((int)Controls.OSD_FFWD, false); // pop the button back to it's up state
                                ToggleButton((int)Controls.OSD_REWIND, false); // pop the button back to it's up state
                            }
                            else
                            {
                                ToggleButton((int)Controls.OSD_PLAY, false);
                                // make sure play button is up (so it shows the play symbol)
                                if (g_Player.Speed < 1) // are we not playing back at normal speed
                                {
                                    ToggleButton((int)Controls.OSD_REWIND, true); // make sure out button is in the down position
                                    ToggleButton((int)Controls.OSD_FFWD, false); // pop the button back to it's up state
                                }
                                else
                                {
                                    ToggleButton((int)Controls.OSD_REWIND, false); // pop the button back to it's up state
                                    if (g_Player.Speed == 1)
                                    {
                                        ToggleButton((int)Controls.OSD_FFWD, false); // pop the button back to it's up state
                                    }
                                }
                            }
                        }

                        if (iControl == (int)Controls.OSD_PLAY)
                        {
                            int iSpeed = g_Player.Speed;
                            if (iSpeed != 1) // we're in ffwd or rewind mode
                            {
                                g_Player.Speed = 1; // drop back to single speed
                                ToggleButton((int)Controls.OSD_REWIND, false); // pop all the relevant
                                ToggleButton((int)Controls.OSD_FFWD, false); // buttons back to
                                ToggleButton((int)Controls.OSD_PLAY, false); // their up state
                            }
                            else
                            {
                                g_Player.Pause(); //Pause/Un-Pause playback
                                if (g_Player.Paused)
                                {
                                    ToggleButton((int)Controls.OSD_PLAY, true);
                                    // make sure play button is down (so it shows the pause symbol)
                                }
                                else
                                {
                                    ToggleButton((int)Controls.OSD_PLAY, false);
                                    // make sure play button is up (so it shows the play symbol)
                                }
                            }
                        }

                        if (iControl == (int)Controls.OSD_STOP)
                        {
                            if (isSubMenuVisible) // sub menu currently active ?
                            {
                                FocusControl(GetID, m_iActiveMenuButtonID, 0); // set focus to last menu button
                                ToggleSubMenu(0, m_iActiveMenu); // hide the currently active sub-menu
                            }
                            Log.Debug("TVOSD:stop");
                        }
                        if (iControl == (int)Controls.OSD_REWIND)
                        {
                            if (g_Player.Paused)
                            {
                                g_Player.Pause(); // Unpause playback
                            }

                            if (g_Player.Speed < 1) // are we not playing back at normal speed
                            {
                                ToggleButton((int)Controls.OSD_REWIND, true); // make sure out button is in the down position
                                ToggleButton((int)Controls.OSD_FFWD, false); // pop the button back to it's up state
                            }
                            else
                            {
                                ToggleButton((int)Controls.OSD_REWIND, false); // pop the button back to it's up state
                                if (g_Player.Speed == 1)
                                {
                                    ToggleButton((int)Controls.OSD_FFWD, false); // pop the button back to it's up state
                                }
                            }
                        }

                        if (iControl == (int)Controls.OSD_FFWD)
                        {
                            if (g_Player.Paused)
                            {
                                g_Player.Pause(); // Unpause playback
                            }

                            if (g_Player.Speed > 1) // are we not playing back at normal speed
                            {
                                ToggleButton((int)Controls.OSD_FFWD, true); // make sure out button is in the down position
                                ToggleButton((int)Controls.OSD_REWIND, false); // pop the button back to it's up state
                            }
                            else
                            {
                                ToggleButton((int)Controls.OSD_FFWD, false); // pop the button back to it's up state
                                if (g_Player.Speed == 1)
                                {
                                    ToggleButton((int)Controls.OSD_REWIND, false); // pop the button back to it's up state
                                }
                            }
                        }

                        if (iControl == (int)Controls.OSD_SKIPBWD)
                        {
                            if (_immediateSeekIsRelative)
                            {
                                g_Player.SeekRelativePercentage(-_immediateSeekValue);
                            }
                            else
                            {
                                g_Player.SeekRelative(-_immediateSeekValue);
                            }
                            ToggleButton((int)Controls.OSD_SKIPBWD, false); // pop the button back to it's up state
                        }

                        if (iControl == (int)Controls.OSD_SKIPFWD)
                        {
                            if (_immediateSeekIsRelative)
                            {
                                g_Player.SeekRelativePercentage(_immediateSeekValue);
                            }
                            else
                            {
                                g_Player.SeekRelative(_immediateSeekValue);
                            }
                            ToggleButton((int)Controls.OSD_SKIPFWD, false); // pop the button back to it's up state
                        }

                        if (iControl == (int)Controls.OSD_MUTE)
                        {
                            ToggleSubMenu(iControl, (int)Controls.OSD_SUBMENU_BG_VOL); // hide or show the sub-menu
                            if (isSubMenuVisible) // is sub menu on?
                            {
                                ShowControl(GetID, (int)Controls.OSD_VOLUMESLIDER); // show the volume control
                                FocusControl(GetID, (int)Controls.OSD_VOLUMESLIDER, 0); // set focus to it
                            }
                            else // sub menu is off
                            {
                                FocusControl(GetID, (int)Controls.OSD_MUTE, 0); // set focus to the mute button
                            }
                        }

                        if (iControl == (int)Controls.OSD_SUBTITLES)
                        {
                            ToggleSubMenu(iControl, (int)Controls.OSD_SUBMENU_BG_SUBTITLES); // hide or show the sub-menu
                            if (isSubMenuVisible)
                            {
                                // set the controls values
                                GUISliderControl pControl = (GUISliderControl)GetControl((int)Controls.OSD_SUBTITLE_DELAY);
                                pControl.SpinType = GUISpinControl.SpinType.SPIN_CONTROL_TYPE_FLOAT;
                                pControl.FloatInterval = 1;
                                pControl.SetRange(-10, 10);
                                SetSliderValue(-10, 10, m_subtitleDelay, (int)Controls.OSD_SUBTITLE_DELAY);
                                SetCheckmarkValue(g_Player.EnableSubtitle, (int)Controls.OSD_SUBTITLE_ONOFF);
                                // show the controls on this sub menu
                                ShowControl(GetID, (int)Controls.OSD_SUBTITLE_DELAY);
                                ShowControl(GetID, (int)Controls.OSD_SUBTITLE_DELAY_LABEL);
                                ShowControl(GetID, (int)Controls.OSD_SUBTITLE_ONOFF);
                                ShowControl(GetID, (int)Controls.OSD_SUBTITLE_LIST);

                                FocusControl(GetID, (int)Controls.OSD_SUBTITLE_DELAY, 0);
                                // set focus to the first control in our group
                                PopulateSubTitles(); // populate the list control with subtitles for this video
                            }
                        }

                        if (iControl == (int)Controls.OSD_BOOKMARKS)
                        {
                            ToggleSubMenu(iControl, (int)Controls.OSD_SUBMENU_BG_BOOKMARKS); // hide or show the sub-menu
                            if (isSubMenuVisible)
                            {
                                // show the controls on this sub menu
                                ShowControl(GetID, (int)Controls.OSD_CREATEBOOKMARK);
                                ShowControl(GetID, (int)Controls.OSD_BOOKMARKS_LIST);
                                ShowControl(GetID, (int)Controls.OSD_BOOKMARKS_LIST_LABEL);
                                ShowControl(GetID, (int)Controls.OSD_CLEARBOOKMARKS);

                                FocusControl(GetID, (int)Controls.OSD_CREATEBOOKMARK, 0);
                                // set focus to the first control in our group
                            }
                        }

                        if (iControl == (int)Controls.OSD_VIDEO)
                        {
                            ToggleSubMenu(iControl, (int)Controls.OSD_SUBMENU_BG_VIDEO); // hide or show the sub-menu
                            if (isSubMenuVisible) // is sub menu on?
                            {
                                // set the controls values
                                float fPercent = (float)(100 * (g_Player.CurrentPosition / g_Player.Duration));
                                SetSliderValue(0.0f, 100.0f, (float)fPercent, (int)Controls.OSD_VIDEOPOS);


                                UpdateGammaContrastBrightness();
                                // show the controls on this sub menu
                                ShowControl(GetID, (int)Controls.OSD_VIDEOPOS);
                                ShowControl(GetID, (int)Controls.OSD_SATURATIONLABEL);
                                ShowControl(GetID, (int)Controls.OSD_SATURATION);
                                ShowControl(GetID, (int)Controls.OSD_SHARPNESSLABEL);
                                ShowControl(GetID, (int)Controls.OSD_SHARPNESS);
                                ShowControl(GetID, (int)Controls.OSD_VIDEOPOS_LABEL);
                                ShowControl(GetID, (int)Controls.OSD_BRIGHTNESS);
                                ShowControl(GetID, (int)Controls.OSD_BRIGHTNESSLABEL);
                                ShowControl(GetID, (int)Controls.OSD_CONTRAST);
                                ShowControl(GetID, (int)Controls.OSD_CONTRASTLABEL);
                                ShowControl(GetID, (int)Controls.OSD_GAMMA);
                                ShowControl(GetID, (int)Controls.OSD_GAMMALABEL);
                                FocusControl(GetID, (int)Controls.OSD_VIDEOPOS, 0); // set focus to the first control in our group
                            }
                        }

                        if (iControl == (int)Controls.OSD_AUDIO)
                        {
                            ToggleSubMenu(iControl, (int)Controls.OSD_SUBMENU_BG_AUDIO); // hide or show the sub-menu
                            if (isSubMenuVisible) // is sub menu on?
                            {
                                // show the controls on this sub menu
                                ShowControl(GetID, (int)Controls.OSD_AVDELAY);
                                ShowControl(GetID, (int)Controls.OSD_AVDELAY_LABEL);
                                lstAudioStreamList.Visible = true;

                                FocusControl(GetID, (int)Controls.OSD_AVDELAY, 0); // set focus to the first control in our group
                                PopulateAudioStreams(); // populate the list control with audio streams for this video
                            }
                        }
                        return true;
                    }
            }
            return base.OnMessage(message);
        }

        private void UpdateGammaContrastBrightness()
        {
            float fBrightNess = (float)GUIGraphicsContext.Brightness;
            float fContrast = (float)GUIGraphicsContext.Contrast;
            float fGamma = (float)GUIGraphicsContext.Gamma;
            float fSaturation = (float)GUIGraphicsContext.Saturation;
            float fSharpness = (float)GUIGraphicsContext.Sharpness;

            SetSliderValue(0.0f, 100.0f, (float)fBrightNess, (int)Controls.OSD_BRIGHTNESS);
            SetSliderValue(0.0f, 100.0f, (float)fContrast, (int)Controls.OSD_CONTRAST);
            SetSliderValue(0.0f, 100.0f, (float)fGamma, (int)Controls.OSD_GAMMA);
            SetSliderValue(0.0f, 100.0f, (float)fSaturation, (int)Controls.OSD_SATURATION);
            SetSliderValue(0.0f, 100.0f, (float)fSharpness, (int)Controls.OSD_SHARPNESS);
        }

        private void SetVideoProgress()
        {
            if (g_Player.Playing)
            {
                int iValue = g_Player.Volume;
                GUISliderControl pSlider = GetControl((int)Controls.OSD_VOLUMESLIDER) as GUISliderControl;
                if (null != pSlider)
                {
                    pSlider.Percentage = iValue; // Update our progress bar accordingly ...
                }
            }
        }

        private void Get_TimeInfo()
        {
            string strTime = "";
            if (!g_Player.IsTVRecording)
            {
                string strChannel = GetChannelName();
                strTime = strChannel;
                GuideProgram prog = PluginMain.GetCurrentProgram(ChannelType.Television);
                if (prog != null)
                {
                    strTime = String.Format("{0}-{1}",
                                            prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                            prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                }
            }
            else
            {
                long currentPosition = (long)(g_Player.CurrentPosition);
                int hh = (int)(currentPosition / 3600) % 100;
                int mm = (int)((currentPosition / 60) % 60);
                int ss = (int)((currentPosition / 1) % 60);
                DateTime startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, ss);

                long duration = (long)(g_Player.Duration);
                hh = (int)(duration / 3600) % 100;
                mm = (int)((duration / 60) % 60);
                ss = (int)((duration / 1) % 60);
                DateTime endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, ss);

                strTime = String.Format("{0}-{1}",
                                        startTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                        endTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
            }
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_SET, GetID, 0, (int)Controls.OSD_TIMEINFO, 0,
                                            0, null);
            msg.Label = strTime;
            OnMessage(msg); // ask our label to update it's caption
        }

        private void ToggleButton(int iButtonID, bool bSelected)
        {
            GUIControl pControl = GetControl(iButtonID) as GUIControl;

            if (pControl != null)
            {
                if (bSelected) // do we want the button to appear down?
                {
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SELECTED, GetID, 0, iButtonID, 0, 0, null);
                    OnMessage(msg);
                }
                else // or appear up?
                {
                    GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_DESELECTED, GetID, 0, iButtonID, 0, 0, null);
                    OnMessage(msg);
                }
            }
        }

        private void ToggleSubMenu(int iButtonID, int iBackID)
        {
            int iX, iY;

            GUIImage pImgNib = GetControl((int)Controls.OSD_SUBMENU_NIB) as GUIImage; // pointer to the nib graphic
            GUIImage pImgBG = GetControl(iBackID) as GUIImage; // pointer to the background graphic
            GUIToggleButtonControl pButton = GetControl(iButtonID) as GUIToggleButtonControl;
            // pointer to the OSD menu button

            // check to see if we are currently showing a sub-menu and it's position is different
            if (isSubMenuVisible && iBackID != m_iActiveMenu)
            {
                m_bNeedRefresh = true;
                isSubMenuVisible = false; // toggle it ready for the new menu requested
            }

            // Get button position
            if (pButton != null)
            {
                iX = (pButton.XPosition + (pButton.Width / 2)); // center of button
                iY = pButton.YPosition;
            }
            else
            {
                iX = 0;
                iY = 0;
            }

            // Set nib position
            if (pImgNib != null && pImgBG != null)
            {
                pImgNib.SetPosition(iX - (pImgNib.TextureWidth / 2), iY - pImgNib.TextureHeight);

                if (!isSubMenuVisible) // sub menu not currently showing?
                {
                    pImgNib.Visible = true; // make it show
                    pImgBG.Visible = true; // make it show
                }
                else
                {
                    pImgNib.Visible = false; // hide it
                    pImgBG.Visible = false; // hide it
                }
            }

            isSubMenuVisible = !isSubMenuVisible; // toggle sub menu visible status
            if (!isSubMenuVisible)
            {
                m_bNeedRefresh = true;
            }
            // Set all sub menu controls to hidden
            HideControl(GetID, (int)Controls.OSD_VOLUMESLIDER);
            HideControl(GetID, (int)Controls.OSD_VIDEOPOS);
            HideControl(GetID, (int)Controls.OSD_VIDEOPOS_LABEL);
            lstAudioStreamList.Visible = false;

            HideControl(GetID, (int)Controls.OSD_AVDELAY);
            HideControl(GetID, (int)Controls.OSD_SHARPNESSLABEL);
            HideControl(GetID, (int)Controls.OSD_SHARPNESS);
            HideControl(GetID, (int)Controls.OSD_SATURATIONLABEL);
            HideControl(GetID, (int)Controls.OSD_SATURATION);
            HideControl(GetID, (int)Controls.OSD_AVDELAY_LABEL);

            HideControl(GetID, (int)Controls.OSD_BRIGHTNESS);
            HideControl(GetID, (int)Controls.OSD_BRIGHTNESSLABEL);

            HideControl(GetID, (int)Controls.OSD_GAMMA);
            HideControl(GetID, (int)Controls.OSD_GAMMALABEL);

            HideControl(GetID, (int)Controls.OSD_CONTRAST);
            HideControl(GetID, (int)Controls.OSD_CONTRASTLABEL);

            HideControl(GetID, (int)Controls.OSD_CREATEBOOKMARK);
            HideControl(GetID, (int)Controls.OSD_BOOKMARKS_LIST);
            HideControl(GetID, (int)Controls.OSD_BOOKMARKS_LIST_LABEL);
            HideControl(GetID, (int)Controls.OSD_CLEARBOOKMARKS);
            HideControl(GetID, (int)Controls.OSD_SUBTITLE_DELAY);
            HideControl(GetID, (int)Controls.OSD_SUBTITLE_DELAY_LABEL);
            HideControl(GetID, (int)Controls.OSD_SUBTITLE_ONOFF);
            HideControl(GetID, (int)Controls.OSD_SUBTITLE_LIST);

            // Reset the other buttons back to up except the one that's active
            if (iButtonID != (int)Controls.OSD_MUTE)
            {
                ToggleButton((int)Controls.OSD_MUTE, false);
            }
            if (iButtonID != (int)Controls.OSD_SUBTITLES)
            {
                ToggleButton((int)Controls.OSD_SUBTITLES, false);
            }
            if (iButtonID != (int)Controls.OSD_BOOKMARKS)
            {
                ToggleButton((int)Controls.OSD_BOOKMARKS, false);
            }
            if (iButtonID != (int)Controls.OSD_VIDEO)
            {
                ToggleButton((int)Controls.OSD_VIDEO, false);
            }
            if (iButtonID != (int)Controls.OSD_AUDIO)
            {
                ToggleButton((int)Controls.OSD_AUDIO, false);
            }

            m_iActiveMenu = iBackID;
            m_iActiveMenuButtonID = iButtonID;
        }

        private void SetSliderValue(float fMin, float fMax, float fValue, int iControlID)
        {
            GUISliderControl pControl = GetControl(iControlID) as GUISliderControl;
            if (pControl == null)
            {
                return;
            }

            if (null != pControl)
            {
                switch (pControl.SpinType)
                {
                    case GUISpinControl.SpinType.SPIN_CONTROL_TYPE_FLOAT:
                        pControl.SetFloatRange(fMin, fMax);
                        pControl.FloatValue = fValue;
                        break;

                    case GUISpinControl.SpinType.SPIN_CONTROL_TYPE_INT:
                        pControl.SetRange((int)fMin, (int)fMax);
                        pControl.IntValue = (int)fValue;
                        break;

                    default:
                        pControl.Percentage = (int)fValue;
                        break;
                }
            }
        }

        private void SetCheckmarkValue(bool bValue, int iControlID)
        {
            if (bValue)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_SELECTED, GetID, 0, iControlID, 0, 0, null);
                OnMessage(msg);
            }
            else
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_DESELECTED, GetID, 0, iControlID, 0, 0, null);
                OnMessage(msg);
            }
        }

        private void Handle_ControlSetting(int iControlID, long wID)
        {
            string strMovie = g_Player.CurrentFile;

            switch (iControlID)
            {
                case (int)Controls.OSD_VOLUMESLIDER:
                    {
                        GUISliderControl pControl = GetControl(iControlID) as GUISliderControl;
                        if (null != pControl)
                        {
                            // no volume control yet so no code here at the moment
                            if (g_Player.Playing)
                            {
                                int iPercentage = pControl.Percentage;
                                g_Player.Volume = iPercentage;
                            }
                        }
                    }
                    break;

                case (int)Controls.OSD_VIDEOPOS:
                    {
                        GUISliderControl pControl = GetControl(iControlID) as GUISliderControl;
                        if (null != pControl)
                        {
                            // Set mplayer's seek position to the percentage requested by the user
                            g_Player.SeekAsolutePercentage(pControl.Percentage);
                        }
                    }
                    break;

                case (int)Controls.OSD_SATURATION:
                    {
                        GUISliderControl pControl = GetControl(iControlID) as GUISliderControl;
                        if (null != pControl)
                        {
                            // Set mplayer's seek position to the percentage requested by the user
                            GUIGraphicsContext.Saturation = pControl.Percentage;
                            UpdateGammaContrastBrightness();
                        }
                    }
                    break;

                case (int)Controls.OSD_SHARPNESS:
                    {
                        GUISliderControl pControl = GetControl(iControlID) as GUISliderControl;
                        if (null != pControl)
                        {
                            // Set mplayer's seek position to the percentage requested by the user
                            GUIGraphicsContext.Sharpness = pControl.Percentage;
                            UpdateGammaContrastBrightness();
                        }
                    }
                    break;

                case (int)Controls.OSD_BRIGHTNESS:
                    {
                        GUISliderControl pControl = GetControl(iControlID) as GUISliderControl;
                        if (null != pControl)
                        {
                            // Set mplayer's seek position to the percentage requested by the user
                            GUIGraphicsContext.Brightness = pControl.Percentage;
                            UpdateGammaContrastBrightness();
                        }
                    }
                    break;

                case (int)Controls.OSD_CONTRAST:
                    {
                        GUISliderControl pControl = GetControl(iControlID) as GUISliderControl;
                        if (null != pControl)
                        {
                            // Set mplayer's seek position to the percentage requested by the user
                            GUIGraphicsContext.Contrast = pControl.Percentage;
                            UpdateGammaContrastBrightness();
                        }
                    }
                    break;

                case (int)Controls.OSD_GAMMA:
                    {
                        GUISliderControl pControl = GetControl(iControlID) as GUISliderControl;
                        if (null != pControl)
                        {
                            // Set mplayer's seek position to the percentage requested by the user
                            GUIGraphicsContext.Gamma = pControl.Percentage;
                            UpdateGammaContrastBrightness();
                        }
                    }
                    break;

                case (int)Controls.OSD_SUBTITLE_ONOFF:
                    {
                        g_Player.EnableSubtitle = !g_Player.EnableSubtitle;
                    }
                    break;

                case (int)Controls.OSD_SUBTITLE_LIST:
                    {
                        if (wID != 0) // check to see if list control has an action ID, remote can cause 0 based events
                        {
                            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECTED, GetID, 0,
                                                            (int)Controls.OSD_SUBTITLE_LIST, 0, 0, null);
                            OnMessage(msg); // retrieve the selected list item
                            if (g_Player.SupportsCC) // Subtitle CC
                            {
                                if (g_Player.SupportsCC && msg.Param1 == 0)
                                {
                                    msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_GET_SELECTED_ITEM, GetID, 0,
                                                                (int)Controls.OSD_SUBTITLE_LIST, msg.Param1, 0, null);
                                    g_Player.EnableSubtitle = false;
                                    g_Player.CurrentSubtitleStream = -1;
                                    Log.Info("Subtitle CC selected ");
                                }
                                else
                                {
                                    Log.Info("Subtitle stream selected " + msg.Label);
                                    g_Player.CurrentSubtitleStream = msg.Param1 - 1; // set the current subtitle
                                    g_Player.EnableSubtitle = true;
                                }
                            }
                            else
                            {
                                Log.Info("Subtitle stream selected " + msg.Label);
                                g_Player.CurrentSubtitleStream = msg.Param1; // set the current subtitle
                            }
                            PopulateSubTitles();
                        }
                    }
                    break;

                case (int)Controls.OSD_SUBTITLE_DELAY:
                    {
                        GUISliderControl pControl = (GUISliderControl)GetControl(iControlID);
                        if (null != pControl && g_Player.EnableSubtitle)
                        {
                            if (pControl.FloatValue < m_subtitleDelay)
                            {
                                global::MediaPortal.Player.Subtitles.SubEngine.GetInstance().DelayMinus();
                            }
                            else
                            {
                                global::MediaPortal.Player.Subtitles.SubEngine.GetInstance().DelayPlus();
                            }
                            m_subtitleDelay = (int)pControl.FloatValue;
                        }
                    }
                    break;
            }
        }

        private void PopulateAudioStreams()
        {
            string strLabel = GUILocalizeStrings.Get(460); // "Audio Stream"
            string strActiveLabel = GUILocalizeStrings.Get(461); // "[active]"

            // tell the list control not to show the page x/y spin control
            lstAudioStreamList.SetPageControlVisible(false);

            // empty the list ready for population
            lstAudioStreamList.Clear();
        }

        private void PopulateSubTitles()
        {
            // get the number of subtitles in the current movie
            int subStreamsCount = g_Player.SubtitleStreams;

            // tell the list control not to show the page x/y spin control
            GUIListControl pControl = GetControl((int)Controls.OSD_SUBTITLE_LIST) as GUIListControl;
            if (null != pControl)
            {
                pControl.SetPageControlVisible(false);
            }

            // empty the list ready for population
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_RESET, GetID, 0,
                                            (int)Controls.OSD_SUBTITLE_LIST, 0, 0, null);
            OnMessage(msg);

            string strLabel = GUILocalizeStrings.Get(462); // "Subtitle"
            string strActiveLabel = GUILocalizeStrings.Get(461); // "[active]"

            // cycle through each subtitle and add it to our list control
            int currentSubtitleStream = g_Player.CurrentSubtitleStream;
            if (g_Player.SupportsCC) // The current subtitle CC is not in the list, add it on top of it
            {
                string strActive = (g_Player.SupportsCC) ? strActiveLabel : null;
                string CC1 = "CC1";

                // create a list item object to add to the list
                GUIListItem pItem = new GUIListItem();
                pItem.Label = CC1;

                if (currentSubtitleStream == -1)
                    if (strActive != null) pItem.Label = "CC1" + " " + strActiveLabel;

                // add it ...
                GUIMessage msg2 = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_ADD, GetID, 0,
                                                 (int)Controls.OSD_SUBTITLE_LIST, 0, 0, pItem);
                OnMessage(msg2);
            }
            for (int i = 0; i < subStreamsCount; ++i)
            {
                string strItem;
                string strLang = g_Player.SubtitleLanguage(i);
                // formats right label2 to '[active]'
                string strActive = (currentSubtitleStream == i) ? strActiveLabel : null;
                int ipos = strLang.IndexOf("[");
                if (ipos > 0)
                {
                    strLang = strLang.Substring(0, ipos);
                }
                // formats to 'Language'
                strItem = String.Format(strLang);

                // create a list item object to add to the list
                GUIListItem pItem = new GUIListItem(strItem);

                if (strActive != null) pItem.Label = strItem + " " + strActiveLabel;

                // add it ...
                GUIMessage msg2 = new GUIMessage(GUIMessage.MessageType.GUI_MSG_LABEL_ADD, GetID, 0,
                                                 (int)Controls.OSD_SUBTITLE_LIST, 0, 0, pItem);
                OnMessage(msg2);
            }

            // set the current active subtitle as the selected item in the list control
            if (g_Player.SupportsCC)
            {
                if (currentSubtitleStream == -1)
                {
                    GUIMessage msgSet = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GetID, 0,
                      (int)Controls.OSD_SUBTITLE_LIST, 0, 0, null);
                    OnMessage(msgSet);
                }
                else
                {
                    GUIMessage msgSet = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GetID, 0,
                      (int)Controls.OSD_SUBTITLE_LIST, currentSubtitleStream + 1, 0, null);
                    OnMessage(msgSet);
                }
            }
            else
            {
                GUIMessage msgSet = new GUIMessage(GUIMessage.MessageType.GUI_MSG_ITEM_SELECT, GetID, 0,
                  (int)Controls.OSD_SUBTITLE_LIST, g_Player.CurrentSubtitleStream, 0, null);
                OnMessage(msgSet);
            }
        }

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

        private void Reset()
        {
            // Set all sub menu controls to hidden
            HideControl(GetID, (int)Controls.OSD_SUBMENU_BG_AUDIO);
            HideControl(GetID, (int)Controls.OSD_SUBMENU_BG_VIDEO);
            HideControl(GetID, (int)Controls.OSD_SUBMENU_BG_BOOKMARKS);
            HideControl(GetID, (int)Controls.OSD_SUBMENU_BG_SUBTITLES);
            HideControl(GetID, (int)Controls.OSD_SUBMENU_BG_VOL);


            HideControl(GetID, (int)Controls.OSD_VOLUMESLIDER);
            HideControl(GetID, (int)Controls.OSD_VIDEOPOS);
            HideControl(GetID, (int)Controls.OSD_VIDEOPOS_LABEL);
            lstAudioStreamList.Visible = false;

            HideControl(GetID, (int)Controls.OSD_AVDELAY);
            HideControl(GetID, (int)Controls.OSD_SATURATIONLABEL);
            HideControl(GetID, (int)Controls.OSD_SATURATION);
            HideControl(GetID, (int)Controls.OSD_SHARPNESSLABEL);
            HideControl(GetID, (int)Controls.OSD_SHARPNESS);
            HideControl(GetID, (int)Controls.OSD_AVDELAY_LABEL);

            HideControl(GetID, (int)Controls.OSD_BRIGHTNESS);
            HideControl(GetID, (int)Controls.OSD_BRIGHTNESSLABEL);

            HideControl(GetID, (int)Controls.OSD_GAMMA);
            HideControl(GetID, (int)Controls.OSD_GAMMALABEL);

            HideControl(GetID, (int)Controls.OSD_CONTRAST);
            HideControl(GetID, (int)Controls.OSD_CONTRASTLABEL);

            HideControl(GetID, (int)Controls.OSD_CREATEBOOKMARK);
            HideControl(GetID, (int)Controls.OSD_BOOKMARKS_LIST);
            HideControl(GetID, (int)Controls.OSD_BOOKMARKS_LIST_LABEL);
            HideControl(GetID, (int)Controls.OSD_CLEARBOOKMARKS);
            HideControl(GetID, (int)Controls.OSD_SUBTITLE_DELAY);
            HideControl(GetID, (int)Controls.OSD_SUBTITLE_DELAY_LABEL);
            HideControl(GetID, (int)Controls.OSD_SUBTITLE_ONOFF);
            HideControl(GetID, (int)Controls.OSD_SUBTITLE_LIST);

            ToggleButton((int)Controls.OSD_MUTE, false);
            ToggleButton((int)Controls.OSD_SUBTITLES, false);
            ToggleButton((int)Controls.OSD_BOOKMARKS, false);
            ToggleButton((int)Controls.OSD_VIDEO, false);
            ToggleButton((int)Controls.OSD_AUDIO, false);

            ToggleButton((int)Controls.OSD_REWIND, false); // pop all the relevant
            ToggleButton((int)Controls.OSD_FFWD, false); // buttons back to
            ToggleButton((int)Controls.OSD_PLAY, false); // their up state

            ToggleButton((int)Controls.OSD_SKIPBWD, false); // pop all the relevant
            ToggleButton((int)Controls.OSD_STOP, false); // buttons back to
            ToggleButton((int)Controls.OSD_SKIPFWD, false); // their up state
            ToggleButton((int)Controls.OSD_MUTE, false); // their up state
        }

        public override bool NeedRefresh()
        {
            if (m_bNeedRefresh)
            {
                m_bNeedRefresh = false;
                return true;
            }
            return false;
        }

        private void OnPreviousChannel()
        {
            Log.Debug("GUITV OSD: OnPreviousChannel");
            if (!PluginMain.Navigator.IsLiveStreamOn && !g_Player.IsTVRecording)
            {
                return;
            }
            PluginMain.Navigator.ZapToPreviousChannel(false);

            ShowPrograms();
            m_dateTime = DateTime.Now;
        }

        private void OnNextChannel()
        {
            Log.Debug("GUITV OSD: OnNextChannel");
            if (!PluginMain.Navigator.IsLiveStreamOn && !g_Player.IsTVRecording)
            {
                return;
            }
            PluginMain.Navigator.ZapToNextChannel(false);

            ShowPrograms();
            m_dateTime = DateTime.Now;
        }

        public void UpdateChannelInfo()
        {
            ShowPrograms();
        }

        private void SetCurrentChannelLogo()
        {
            string strChannel = GetChannelName();
            Guid _channelId = GetChannelId();
            string strLogo = string.Empty;

            if (_channelId != Guid.Empty && strChannel != string.Empty)
            {
                using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                {
                    strLogo = Utility.GetLogoImage(_channelId, strChannel, tvSchedulerAgent);
                }
            }

            if (imgTvChannelLogo != null)
            {
                if (!string.IsNullOrEmpty(strLogo))
                {
                    imgTvChannelLogo.SetFileName(strLogo);
                    m_bNeedRefresh = true;
                    imgTvChannelLogo.Visible = true;
                }
                else
                {
                    imgTvChannelLogo.Visible = false;
                }
            }
        }

        private Guid GetChannelId()
        {
            Guid ID = Guid.Empty;
            if (g_Player.IsTVRecording)
            {
                Recording rec = TvRecorded.GetPlayingRecording();
                if (rec != null && rec.ChannelType == ChannelType.Television)
                {
                    ID = rec.ChannelId;
                }
                else
                {
                    ID = Guid.Empty;
                }
            }
            else
            {
                if (PluginMain.Navigator.CurrentChannel != null)
                {
                    ID = PluginMain.Navigator.CurrentChannel.ChannelId;
                }
            }
            return ID;
        }

        private string GetChannelName()
        {
            if (g_Player.IsTVRecording)
            {
                Recording rec = TvRecorded.GetPlayingRecording();
                if (rec != null && rec.ChannelType == ChannelType.Television)
                {
                    return rec.ChannelDisplayName;
                }
                return string.Empty;
            }
            else
            {
                return PluginMain.Navigator.CurrentChannelName;
            }
        }

        private void ShowPrograms()
        {
            if (tbProgramDescription != null)
            {
                tbProgramDescription.Clear();
            }
            if (tbOnTvNow != null)
            {
                tbOnTvNow.EnableUpDown = false;
                tbOnTvNow.Clear();
            }
            if (tbOnTvNext != null)
            {
                tbOnTvNext.EnableUpDown = false;
                tbOnTvNext.Clear();
            }

            SetRecorderStatus(true);

            // Channel icon
            if (imgTvChannelLogo != null)
            {
                SetCurrentChannelLogo();
            }

            if (lblCurrentChannel != null)
            {
                lblCurrentChannel.Label = GetChannelName();
            }

            GuideProgram prog = PluginMain.GetCurrentProgram(ChannelType.Television);

            if (prog != null && !g_Player.IsTVRecording)
            {
                string strTime = String.Format("{0}-{1}",
                                               prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                               prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                if (lblCurrentTime != null)
                {
                    lblCurrentTime.Label = strTime;
                }
                // On TV Now
                if (tbOnTvNow != null)
                {
                    strTime = String.Format("{0} ", prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                    tbOnTvNow.Label = strTime + prog.CreateProgramTitle();
                    GUIPropertyManager.SetProperty("#TV.View.start", strTime);

                    strTime = String.Format("{0} ", prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                    GUIPropertyManager.SetProperty("#TV.View.stop", strTime);
                    GUIPropertyManager.SetProperty("#TV.View.remaining", Utils.SecondsToHMSString(prog.StopTime - prog.StartTime));
                }
                if (tbProgramDescription != null)
                {
                    tbProgramDescription.Label = prog.CreateCombinedDescription(false);
                }

                // next program
                if (GetChannelId() != Guid.Empty)
                {
                    prog = PluginMain.GetNextprogram(ChannelType.Television);

                    if (prog != null)
                    {
                        if (tbOnTvNext != null)
                        {
                            tbOnTvNext.Label = strTime + "  " + prog.CreateProgramTitle();
                        }
                    }
                }
            }
            else if (g_Player.IsTVRecording)
            {
                Recording rec = null;
                string description = "";
                string title = "";
                string startTime = ""; 
                string endTime = "";

                rec = TvRecorded.GetPlayingRecording();
                if (rec != null
                    && rec.ChannelType == ChannelType.Television)
                {
                    description = rec.CreateCombinedDescription(false);
                    title = rec.Title;
                    Guid _channelId = rec.ChannelId;
                    string channelName = rec.ChannelDisplayName;

                    if (_channelId != Guid.Empty && channelName != string.Empty)
                    {
                        string strLogo = string.Empty;
                        using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                        {
                            strLogo = Utility.GetLogoImage(_channelId, channelName, tvSchedulerAgent);
                        }

                        if (string.IsNullOrEmpty(strLogo))
                        {
                            strLogo = "defaultVideoBig.png";
                        }
                        GUIPropertyManager.SetProperty("#TV.View.thumb", strLogo);
                    }

                    long currentPosition = (long)(g_Player.CurrentPosition);
                    startTime = Utils.SecondsToHMSString((int)currentPosition);

                    long duration = (long)(g_Player.Duration);
                    endTime = Utils.SecondsToHMSString((int)duration);
                  
                    if (tbOnTvNow != null)
                    {
                        tbOnTvNow.Label = title;
                    }

                    GUIPropertyManager.SetProperty("#TV.View.compositetitle", title);
                    GUIPropertyManager.SetProperty("#TV.View.start", startTime);
                    GUIPropertyManager.SetProperty("#TV.View.stop", endTime);

                    if (tbProgramDescription != null)
                    {
                        tbProgramDescription.Label = description;
                    }
                }
            }

            else
            {
                if (tbOnTvNow != null)
                {
                    tbOnTvNow.Label = GUILocalizeStrings.Get(736); // no epg for this channel
                }
                if (tbOnTvNext != null)
                {
                    tbOnTvNext.Label = GUILocalizeStrings.Get(736); // no epg for this channel
                }

                GUIPropertyManager.SetProperty("#TV.View.start", string.Empty);
                GUIPropertyManager.SetProperty("#TV.View.stop", string.Empty);
                GUIPropertyManager.SetProperty("#TV.View.remaining", string.Empty);
                if (lblCurrentTime != null)
                {
                    lblCurrentTime.Label = String.Empty;
                }
            }
            UpdateProgressBar();
        }

        private void SetRecorderStatus()
        {
            SetRecorderStatus(false);
        }

        private void SetRecorderStatus(bool forced)
        {
            if (imgRecIcon != null)
            {
                TimeSpan ts = DateTime.Now - _RecIconLastCheck;
                if (ts.TotalSeconds > 15 || forced)
                {
                    bool isRecording = false;
                    ActiveRecording activeRecording;
                    if (PluginMain.IsChannelRecording(GetChannelId(), out activeRecording))
                    {
                        if (g_Player.IsTVRecording)
                        {
                            Recording rec = TvRecorded.GetPlayingRecording();
                            if (rec != null)
                            {
                                isRecording = true;
                            }
                        }
                        else
                        {
                            isRecording = true;
                        }
                    }

                    imgRecIcon.Visible = isRecording;
                    _RecIconLastCheck = DateTime.Now;
                    Log.Info("OSD.SetRecorderStatus = {0}", imgRecIcon.Visible);
                }
            }
        }

        private void UpdateProgressBar()
        {
            if (!g_Player.IsTVRecording)
            {
                double fPercent;
                GuideProgram prog = PluginMain.GetCurrentProgram(ChannelType.Television);

                if (prog == null)
                {
                    GUIPropertyManager.SetProperty("#TV.View.Percentage", "0");
                    return;
                }
                string strTime = String.Format("{0}-{1}",
                                               prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                               prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                TimeSpan ts = prog.StopTime - prog.StartTime;
                double iTotalSecs = ts.TotalSeconds;
                ts = DateTime.Now - prog.StartTime;
                double iCurSecs = ts.TotalSeconds;
                fPercent = ((double)iCurSecs) / ((double)iTotalSecs);
                fPercent *= 100.0d;
                GUIPropertyManager.SetProperty("#TV.View.Percentage", fPercent.ToString());
                Get_TimeInfo();

                bool updateProperties = false;
                if (previousProgram == null)
                {
                    m_dateTime = DateTime.Now;
                    previousProgram = prog;
                    ShowPrograms();
                    updateProperties = true;
                }
                else if (previousProgram.StartTime != prog.StartTime || previousProgram.GuideChannelId != prog.GuideChannelId)
                {
                    m_dateTime = DateTime.Now;
                    previousProgram = prog;
                    ShowPrograms();
                    updateProperties = true;
                }

                if (updateProperties)
                {
                    GUIPropertyManager.SetProperty("#TV.View.channel", GetChannelName());
                    GUIPropertyManager.SetProperty("#TV.View.start",
                                                   prog.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                    GUIPropertyManager.SetProperty("#TV.View.stop",
                                                   prog.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));
                    GUIPropertyManager.SetProperty("#TV.View.remaining", Utils.SecondsToHMSString(prog.StopTime - prog.StartTime));
                    GUIPropertyManager.SetProperty("#TV.View.genre", prog.Category);
                    GUIPropertyManager.SetProperty("#TV.View.title", prog.Title);
                    GUIPropertyManager.SetProperty("#TV.View.compositetitle", prog.CreateProgramTitle());
                    GUIPropertyManager.SetProperty("#TV.View.subtitle", prog.SubTitle);
                    GUIPropertyManager.SetProperty("#TV.View.description", prog.CreateCombinedDescription(false));
                    GUIPropertyManager.SetProperty("#TV.View.episode", prog.EpisodeNumberDisplay);
                }
            }
            else
            {
                Recording rec = null;
                string startTime = "";
                string endTime = "";
                string channelDisplayName = "";

                rec = TvRecorded.GetPlayingRecording();
                if (rec != null
                    && rec.ChannelType == ChannelType.Television)
                {
                    long currentPosition = (long)(g_Player.CurrentPosition);
                    startTime = Utils.SecondsToHMSString((int)currentPosition);

                    long duration = (long)(g_Player.Duration);
                    endTime = Utils.SecondsToHMSString((int)duration);

                    channelDisplayName = rec.ChannelDisplayName + " " + Utility.GetLocalizedText(TextId.RecordedSuffix);

                    double fPercent;

                    fPercent = ((double)currentPosition) / ((double)duration);
                    fPercent *= 100.0d;

                    GUIPropertyManager.SetProperty("#TV.View.Percentage", fPercent.ToString());
                    Get_TimeInfo();

                    GUIPropertyManager.SetProperty("#TV.View.channel", channelDisplayName);
                    GUIPropertyManager.SetProperty("#TV.View.start", startTime);
                    GUIPropertyManager.SetProperty("#TV.View.stop", endTime);
                    GUIPropertyManager.SetProperty("#TV.View.genre", rec.Category);
                    GUIPropertyManager.SetProperty("#TV.View.title", rec.Title);
                    GUIPropertyManager.SetProperty("#TV.View.compositetitle", rec.CreateProgramTitle());
                    GUIPropertyManager.SetProperty("#TV.View.description", rec.CreateCombinedDescription(false));
                    GUIPropertyManager.SetProperty("#TV.View.subtitle", rec.SubTitle);
                    GUIPropertyManager.SetProperty("#TV.View.episode", rec.EpisodeNumberDisplay);
                }
                else
                {
                    GUIPropertyManager.SetProperty("#TV.View.Percentage", "0");
                }
            }
        }
    }
}