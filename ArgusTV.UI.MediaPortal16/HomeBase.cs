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
using System.Collections.Generic;
using System.Globalization;

using MediaPortal.GUI.Library;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.Player;
using MediaPortal.Profile;
using TvLibrary.Interfaces;
using TvLibrary.Implementations.DVB;
using Action = MediaPortal.GUI.Library.Action;

using ArgusTV.ServiceProxy;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.MediaPortal
{
    public abstract class HomeBase : GUIWindow
    {
        public HomeBase(ChannelType channelType)
        {
            _channelType = channelType;
            ClearProperties(_channelType);
        }

        private enum Controls
        {
            IMG_REC_CHANNEL = 21,
            LABEL_REC_INFO = 22,
            IMG_REC_RECTANGLE = 23,
        };

        public static int LogoIconWidth { get; set; }
        public static int LogoIconHeight { get; set; }
        protected abstract bool AutoTurnOnStream { get; }

        private ChannelType _channelType;
        private static string[] _preferredLanguages;
        private static bool _preferAC3;
        private static bool _preferAudioTypeOverLang;
        protected static bool _showChannelStateIcons;
        private static bool _settingsLoaded = false;
        private bool _turnOnStreamNow = false;

        [SkinControlAttribute(2)] protected GUIButtonControl _tvGuideButton;
        [SkinControlAttribute(3)] protected GUIButtonControl _searchGuideButton;
        [SkinControlAttribute(4)] protected GUIButtonControl _recordingsButton;
        [SkinControlAttribute(5)] protected GUIButtonControl _channelButton;
        [SkinControlAttribute(6)] protected GUICheckButton _tvOnOffButton;
        [SkinControlAttribute(7)] protected GUIButtonControl _upcomingProgramsButton;
        [SkinControlAttribute(8)] protected GUIButtonControl _activeRecordingsButton;
        [SkinControlAttribute(9)] protected GUIButtonControl _teletextButton = null;
        [SkinControlAttribute(24)] protected GUIImage _recordImage;
        [SkinControlAttribute(420)] protected GUILabelControl _settingsLabel;

        #region Service Proxies

        private CoreServiceProxy _coreServiceProxy;

        public CoreServiceProxy CoreServiceProxy
        {
            get
            {
                if (_coreServiceProxy == null)
                {
                    _coreServiceProxy = new CoreServiceProxy();
                }
                return _coreServiceProxy;
            }
        }

        #endregion

        #region overrides

        public override bool Init()
        {
            LoadSettings();
            return true;
        }

        public override void DeInit()
        {
            SaveSettings();
            base.DeInit();
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _tvGuideButton.Label = Utility.GetLocalizedText(TextId.TvGuide);
            _searchGuideButton.Label = Utility.GetLocalizedText(TextId.SearchGuide);
            _recordingsButton.Label = Utility.GetLocalizedText(TextId.RecordedTv);
            _upcomingProgramsButton.Label = Utility.GetLocalizedText(TextId.UpcomingPrograms);
            _activeRecordingsButton.Label = Utility.GetLocalizedText(TextId.ActiveRecordings);
            if (_teletextButton != null)
            {
                _teletextButton.Label = Utility.GetLocalizedText(TextId.Teletext);
            }
        }

        protected override void OnPageLoad()
        {
            PluginMain.EnsureConnection();
            base.OnPageLoad();
            
            HomeBase.LogoIconWidth = 84;
            HomeBase.LogoIconHeight = 84;
            GuideBase.TimeAlignment = GUIControl.Alignment.ALIGN_LEFT;

            if (_settingsLabel != null
                && _settingsLabel.Label.Length > 10)
            {
                const string guideTimeSetting = "guide_time=";
                const string logoIconSizeSetting = "logo-icon-size=";
                string[] values = _settingsLabel.Label.Substring(10).Split(';');
                foreach (string value in values)
                {
                    if (value.StartsWith(guideTimeSetting, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // The skin has set a different alignment for the guide time labels.
                        string align = value.Substring(guideTimeSetting.Length);
                        try
                        {
                            GuideBase.TimeAlignment = (GUIControl.Alignment)Enum.Parse(typeof(GUIControl.Alignment), align);
                        }
                        catch{ }
                    }
                    else if (value.StartsWith(logoIconSizeSetting, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // The skin has set a different icon size.
                        string[] sizeValues = value.Substring(logoIconSizeSetting.Length).Split(',');
                        if (sizeValues.Length == 2)
                        {
                            int val;
                            if (Int32.TryParse(sizeValues[0], out val))
                            {
                                HomeBase.LogoIconWidth = val;
                            }
                            if (Int32.TryParse(sizeValues[1], out val))
                            {
                                HomeBase.LogoIconHeight = val;
                            }
                        }
                    }
                }
            }

            GUIPropertyManager.SetProperty("#itemcount", String.Empty);
            GUIPropertyManager.SetProperty("#selecteditem", String.Empty);
            if (this._channelType == ChannelType.Television)
            {
                GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100001));
            }
            else
            {
                GUIPropertyManager.SetProperty("#currentmodule", GUILocalizeStrings.Get(100030));
            }

            if (PluginMain.IsConnected()
                && !PluginMain.Navigator.IsLiveStreamOn
                && this.AutoTurnOnStream
                && !PreviousWindowWasPluginWindow())
            {
                GUIControl.FocusControl(GetID, _tvOnOffButton.GetID);
                _turnOnStreamNow = true;
            }
        }

        public override void OnAction(Action action)
        {
            switch (action.wID)
            {
                case Action.ActionType.ACTION_RECORD:
                    if (GUIWindowManager.ActiveWindowEx == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN)
                    {
                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_RECORD, GUIWindowManager.ActiveWindow, 0, 0, 0, 0, null);
                        msg.SendToTargetWindow = true;
                        msg.TargetWindowId = (int)GUIWindow.Window.WINDOW_TVFULLSCREEN;
                        GUIGraphicsContext.SendMessage(msg);
                    }
                    break;

                case Action.ActionType.ACTION_PREV_CHAPTER:
                case Action.ActionType.ACTION_PREV_CHANNEL:
                case Action.ActionType.ACTION_PAGE_DOWN:
                    if (g_Player.IsTVRecording)
                    {
                        OnPrevChapter();
                    }
                    else if (action.wID != Action.ActionType.ACTION_PREV_CHAPTER)
                    {
                        OnPreviousChannel();
                    }
                    break;

                case Action.ActionType.ACTION_NEXT_CHAPTER:
                case Action.ActionType.ACTION_NEXT_CHANNEL:
                case Action.ActionType.ACTION_PAGE_UP:
                    if (g_Player.IsTVRecording)
                    {
                        OnNextChapter();
                    }
                    else if (action.wID != Action.ActionType.ACTION_NEXT_CHAPTER)
                    {
                        OnNextChannel();
                    }
                    break;

                case Action.ActionType.ACTION_LAST_VIEWED_CHANNEL:
                    PluginMain.Navigator.ZapToLastViewedChannel(_channelType);
                    break;

                case Action.ActionType.ACTION_KEY_PRESSED:
                    if ((char)action.m_key.KeyChar == '0')
                    {
                        PluginMain.Navigator.ZapToLastViewedChannel(_channelType);
                    }
                    break;
                case Action.ActionType.ACTION_SHOW_GUI:
                    // If we are in tvhome and TV is currently off and no fullscreen TV then turn ON TV now!
                    if (!g_Player.FullScreen && !PluginMain.Navigator.IsLiveStreamOn)
                    {
                        OnClicked(6, _tvOnOffButton, Action.ActionType.ACTION_MOUSE_CLICK); //6=togglebutton
                    }
                    break;
            }
            base.OnAction(action);
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            SaveSettings();
            base.OnPageDestroy(new_windowId);
        }

        private static DateTime _updateTimer = DateTime.MinValue;
        public override void Process()
        {
            if (!PluginMain.IsConnected())
            {
                return;
            }

            if (_turnOnStreamNow)
            {
                _turnOnStreamNow = false;
                TurnOnStream();
            }

            if (PluginMain.Navigator.CheckChannelChange())
            {
                DoUpdateProgressPercentageBar(_channelType, true);
            }

            TimeSpan ts = DateTime.Now - _updateTimer;
            if (ts.TotalMilliseconds < 1000)
            {
                return;
            }

            _updateTimer = DateTime.Now;
            DoUpdateProgressPercentageBar(_channelType, false);

            GUIControl.HideControl(GetID, (int)Controls.LABEL_REC_INFO);
            GUIControl.HideControl(GetID, (int)Controls.IMG_REC_RECTANGLE);
            GUIControl.HideControl(GetID, (int)Controls.IMG_REC_CHANNEL);

            if (_tvOnOffButton != null
                && _tvOnOffButton.Selected != PluginMain.Navigator.IsLiveStreamOn)
            {
                _tvOnOffButton.Selected = PluginMain.Navigator.IsLiveStreamOn
                    && PluginMain.Navigator.CurrentChannel != null
                    && PluginMain.Navigator.CurrentChannel.ChannelType == _channelType;
            }

            if (_teletextButton != null && _teletextButton.IsEnabled !=
                (PluginMain.Navigator.HasTeletext() && !g_Player.IsTVRecording && PluginMain.Navigator.IsLiveStreamOn))
            {
                _teletextButton.IsEnabled = (PluginMain.Navigator.HasTeletext()
                    && !g_Player.IsTVRecording && PluginMain.Navigator.IsLiveStreamOn);
            }
            base.Process();
        }

        protected override void OnClicked(int controlId, GUIControl control, Action.ActionType actionType)
        {
            if (control == _tvGuideButton)
            {
                GUIWindowManager.ActivateWindow(
                    _channelType == ChannelType.Television ? WindowId.TvGuide : WindowId.RadioGuide); 
            }
            else if (control == _searchGuideButton)
            {
                GUIWindowManager.ActivateWindow(
                    _channelType == ChannelType.Television ? WindowId.TvGuideSearch : WindowId.RadioGuideSearch);
            }
            else if (control == _recordingsButton)
            {
                GUIWindowManager.ActivateWindow(
                    _channelType == ChannelType.Television ? WindowId.RecordedTv : WindowId.RecordedRadio);
            }
            else if (control == _channelButton)
            {
                OnSelectChannel();
            }
            else if (control == _tvOnOffButton)
            {
                if (PluginMain.Navigator.IsLiveStreamOn
                    && PluginMain.Navigator.LiveStream.Channel.ChannelType == this._channelType)
                {
                    g_Player.Stop();
                }
                else
                {
                    _updateTimer = DateTime.Now;
                    TurnOnStream();
                }
            }
            else if (control == _activeRecordingsButton)
            {
                OnActiveRecordings();
            }
            else if (control == _upcomingProgramsButton)
            {
                GUIWindowManager.ActivateWindow(
                    _channelType == ChannelType.Television ? WindowId.UpcomingTvPrograms : WindowId.UpcomingRadioPrograms);
            }
            else if (control == _teletextButton)
            {
                GUIWindowManager.ActivateWindow((int)WindowId.Teletext);
            }
            base.OnClicked(controlId, control, actionType);
        }

        #endregion

        #region private methods

        private void OnPreviousChannel()
        {
            if (GUIGraphicsContext.IsFullScreenVideo)
            {
                // where in fullscreen so delayzap channel instead of immediatly tune..
                TvFullScreen tvWindow = (TvFullScreen)GUIWindowManager.GetWindow((int)(int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
                if (tvWindow != null)
                {
                    tvWindow.ZapPreviousChannel();
                    return;
                }
            }

            // Zap to previous channel immediately
            PluginMain.Navigator.ZapToPreviousChannel(false);
        }

        private void OnNextChannel()
        {
            if (GUIGraphicsContext.IsFullScreenVideo)
            {
                // where in fullscreen so delayzap channel instead of immediatly tune..
                TvFullScreen tvWindow = (TvFullScreen)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
                if (tvWindow != null)
                {
                    tvWindow.ZapNextChannel();
                    return;
                }
            }

            // Zap to next channel immediately
            PluginMain.Navigator.ZapToNextChannel(false);
        }

        private void OnPrevChapter()
        {
            if (GUIGraphicsContext.IsFullScreenVideo)
            {
                TvFullScreen tvWindow = (TvFullScreen)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
                if (tvWindow != null)
                {
                    tvWindow.JumpToPrevChapter();
                    return;
                }
            }

            if (g_Player.HasChapters)
            {
                g_Player.JumpToPrevChapter();
            }
        }

        private void OnNextChapter()
        {
            if (GUIGraphicsContext.IsFullScreenVideo)
            {
                TvFullScreen tvWindow = (TvFullScreen)GUIWindowManager.GetWindow((int)(int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
                if (tvWindow != null)
                {
                    tvWindow.JumpToNextChapter();
                    return;
                }
            }

            if (g_Player.HasChapters)
            {
                g_Player.JumpToNextChapter();
            }
        }

        public static int GetPreferedAudioStreamIndex() // also used from tvrecorded class
        {
            int idxFirstAc3 = -1;         // the index of the first avail. ac3 found
            int idxFirstmpeg = -1;        // the index of the first avail. mpg found
            int idxLangAc3 = -1;          // the index of ac3 found based on lang. pref
            int idxLangmpeg = -1;         // the index of mpg found based on lang. pref   
            int idx = -1;                 // the chosen audio index we return
            string langSel = "";          // find audio based on this language.
            string ac3BasedOnLang = "";   // for debugging, what lang. in prefs. where used to choose the ac3 audio track ?
            string mpegBasedOnLang = "";  // for debugging, what lang. in prefs. where used to choose the mpeg audio track ?

            IAudioStream[] streams;

            List<IAudioStream> streamsList = new List<IAudioStream>();
            for (int i = 0; i < g_Player.AudioStreams; i++)
            {
                DVBAudioStream stream = new DVBAudioStream();
                string streamType = g_Player.AudioType(i);

                switch (streamType)
                {
                    case "AC3":
                        stream.StreamType = AudioStreamType.AC3;
                        break;
                    case "Mpeg1":
                        stream.StreamType = AudioStreamType.Mpeg1;
                        break;
                    case "Mpeg2":
                        stream.StreamType = AudioStreamType.Mpeg2;
                        break;
                    case "AAC":
                        stream.StreamType = AudioStreamType.AAC;
                        break;
                    case "LATMAAC":
                        stream.StreamType = AudioStreamType.LATMAAC;
                        break;
                    default:
                        stream.StreamType = AudioStreamType.Unknown;
                        break;
                }

                stream.Language = g_Player.AudioLanguage(i);
                streamsList.Add(stream);
            }
            streams = (IAudioStream[])streamsList.ToArray();

            if (_preferredLanguages != null)
            {
                Log.Debug("TVHome.GetPreferedAudioStreamIndex(): preferred LANG(s):{0} preferAC3:{1} preferAudioTypeOverLang:{2}", String.Join(";", _preferredLanguages), _preferAC3, _preferAudioTypeOverLang);
            }
            else
            {
                Log.Debug("TVHome.GetPreferedAudioStreamIndex(): preferred LANG(s):{0} preferAC3:{1} _preferAudioTypeOverLang:{2}", "n/a", _preferAC3, _preferAudioTypeOverLang);
            }
            Log.Debug("Audio streams avail: {0}", streams.Length);

            if (streams.Length == 1)
            {
                Log.Info("Audio stream: switching to preferred AC3/MPEG audio stream 0 (only 1 track avail.)");
                return 0;
            }

            for (int i = 0; i < streams.Length; i++)
            {
                //tag the first found ac3 and mpeg indexes
                if (streams[i].StreamType == AudioStreamType.AC3)
                {
                    if (idxFirstAc3 == -1) idxFirstAc3 = i;
                }
                else
                {
                    if (idxFirstmpeg == -1) idxFirstmpeg = i;
                }

                //now find the ones based on LANG prefs.
                if (_preferredLanguages != null)
                {
                    for (int j = 0; j < _preferredLanguages.Length; j++)
                    {
                        if (_preferredLanguages[j].Length == 0) continue;
                        langSel = _preferredLanguages[j];
                        if (streams[i].Language.Contains(langSel) && langSel.Length > 0)
                        {
                            if ((streams[i].StreamType == AudioStreamType.AC3)) //is the audio track an AC3 track ?
                            {
                                if (idxLangAc3 == -1)
                                {
                                    idxLangAc3 = i;
                                    ac3BasedOnLang = langSel;
                                }
                            }
                            else //audiotrack is mpeg
                            {
                                if (idxLangmpeg == -1)
                                {
                                    idxLangmpeg = i;
                                    mpegBasedOnLang = langSel;
                                }
                            }
                        }
                        if (idxLangAc3 > -1 && idxLangmpeg > -1) break;
                    } //for loop
                }
                if (idxFirstAc3 > -1 && idxFirstmpeg > -1 && idxLangAc3 > -1 && idxLangmpeg > -1) break;
            }

            if (_preferAC3)
            {
                if (_preferredLanguages != null)
                {
                    //did we find an ac3 track that matches our LANG prefs ?
                    if (idxLangAc3 > -1)
                    {
                        idx = idxLangAc3;
                        Log.Info("Audio stream: switching to preferred AC3 audio stream {0}, based on LANG {1}", idx, ac3BasedOnLang);
                    }
                    //if not, did we even find an ac3 track ?
                    else if (idxFirstAc3 > -1)
                    {
                        //we did find an AC3 track, but not based on LANG - should we choose this or the mpeg track which is based on LANG.
                        if (_preferAudioTypeOverLang || (idxLangmpeg == -1 && _preferAudioTypeOverLang))
                        {
                            idx = idxFirstAc3;
                            Log.Info("Audio stream: switching to preferred AC3 audio stream {0}, NOT based on LANG (none avail. matching {1})", idx, ac3BasedOnLang);
                        }
                        else
                        {
                            Log.Info("Audio stream: ignoring AC3 audio stream {0}", idxFirstAc3);
                        }
                    }
                    //if not then proceed with mpeg lang. selection below.
                }
                else
                {
                    //did we find an ac3 track ?
                    if (idxFirstAc3 > -1)
                    {
                        idx = idxFirstAc3;
                        Log.Info("Audio stream: switching to preferred AC3 audio stream {0}, NOT based on LANG", idx);
                    }
                    //if not then proceed with mpeg lang. selection below.
                }
            }

            if (idx == -1 && _preferAC3)
            {
                Log.Info("Audio stream: no preferred AC3 audio stream found, trying mpeg instead.");
            }

            if (idx == -1 || !_preferAC3) // we end up here if ac3 selection didnt happen (no ac3 avail.) or if preferac3 is disabled.
            {
                if (_preferredLanguages != null)
                {
                    //did we find a mpeg track that matches our LANG prefs ?
                    if (idxLangmpeg > -1)
                    {
                        idx = idxLangmpeg;
                        Log.Info("Audio stream: switching to preferred MPEG audio stream {0}, based on LANG {1}", idx, mpegBasedOnLang);
                    }
                    //if not, did we even find a mpeg track ?
                    else if (idxFirstmpeg > -1)
                    {
                        //we did find a AC3 track, but not based on LANG - should we choose this or the mpeg track which is based on LANG.
                        if (_preferAudioTypeOverLang || (idxLangAc3 == -1 && _preferAudioTypeOverLang))
                        {
                            idx = idxFirstmpeg;
                            Log.Info("Audio stream: switching to preferred MPEG audio stream {0}, NOT based on LANG (none avail. matching {1})", idx, mpegBasedOnLang);
                        }
                        else
                        {
                            if (idxLangAc3 > -1)
                            {
                                idx = idxLangAc3;
                                Log.Info("Audio stream: ignoring MPEG audio stream {0}", idx);
                            }
                        }
                    }
                }
                else
                {
                    idx = idxFirstmpeg;
                    Log.Info("Audio stream: switching to preferred MPEG audio stream {0}, NOT based on LANG", idx);
                }
            }

            if (idx == -1)
            {
                idx = 0;
                Log.Info("Audio stream: switching to preferred AC3/MPEG audio stream {0}", idx);
            }

            return idx;
        }

        private void TurnOnStream()
        {
            Channel previousChannel = PluginMain.Navigator.GetPreviousChannel(this._channelType);
            if (previousChannel != null
                && previousChannel.ChannelType == this._channelType)
            {
                PluginMain.Navigator.ZapToChannel(previousChannel, false);
            }
            else
            {
                OnSelectChannel();
            }
        }

        public void OnSelectChannel()
        {
            MiniGuide miniGuide = (MiniGuide)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_MINI_GUIDE);
            miniGuide.ChannelType = _channelType;
            miniGuide.DoModal(GetID);
            if (!miniGuide.Canceled)
            {
                PluginMain.Navigator.SetCurrentGroup(_channelType, miniGuide.SelectedGroup);
                PluginMain.Navigator.ZapToChannel(miniGuide.SelectedChannel, false);
            }
        }

        #endregion

        #region Progress

        private static void ClearProperties(ChannelType channelType)
        {
            string guiPropertyPrefix = channelType == ChannelType.Television ? "#TV" : "#Radio";
            Utility.ClearProperty(guiPropertyPrefix + ".View.channel");
            Utility.ClearProperty(guiPropertyPrefix + ".View.thumb");
            Utility.ClearProperty(guiPropertyPrefix + ".View.start");
            Utility.ClearProperty(guiPropertyPrefix + ".View.stop");
            Utility.ClearProperty(guiPropertyPrefix + ".View.remaining");
            Utility.ClearProperty(guiPropertyPrefix + ".View.genre");
            Utility.ClearProperty(guiPropertyPrefix + ".View.title");
            Utility.ClearProperty(guiPropertyPrefix + ".View.compositetitle");
            Utility.ClearProperty(guiPropertyPrefix + ".View.subtitle");
            Utility.ClearProperty(guiPropertyPrefix + ".View.episode");
            Utility.ClearProperty(guiPropertyPrefix + ".View.description");
            Utility.ClearProperty(guiPropertyPrefix + ".Next.start");
            Utility.ClearProperty(guiPropertyPrefix + ".Next.stop");
            Utility.ClearProperty(guiPropertyPrefix + ".Next.genre");
            Utility.ClearProperty(guiPropertyPrefix + ".Next.title");
            Utility.ClearProperty(guiPropertyPrefix + ".Next.compositetitle");
            Utility.ClearProperty(guiPropertyPrefix + ".Next.subtitle");
            Utility.ClearProperty(guiPropertyPrefix + ".Next.episode");
            Utility.ClearProperty(guiPropertyPrefix + ".Next.description");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.Percentage", "0");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent1", "0");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent2", "0");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent3", "0");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.chapters", string.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.jumppoints", string.Empty);
        }

        /// <summary>
        /// Update the the progressbar in the GUI which shows
        /// how much of the current tv program has elapsed
        /// </summary>
        private static DateTime _updateProgressTimer = DateTime.MinValue;
        protected static void DoUpdateProgressPercentageBar(ChannelType channelType, bool ForceUpdate)
        {
            string guiPropertyPrefix = channelType == ChannelType.Television ? "#TV" : "#Radio";

            TimeSpan ts = DateTime.Now - _updateProgressTimer;
            if (ts.TotalMilliseconds < 1000 && !ForceUpdate)
            {
                return;
            }
            _updateProgressTimer = DateTime.Now;

            //set audio video related media info properties.
            int currAudio = g_Player.CurrentAudioStream;
            if (currAudio > -1)
            {
                UpdateAudioProperties(currAudio, guiPropertyPrefix);
            }

            // Check for recordings vs liveTv/Radio or Idle
            if (!g_Player.IsTVRecording)
            {
                try
                {
                    Channel infochannel = PluginMain.Navigator.CurrentChannel ?? PluginMain.Navigator.GetPreviousChannel(channelType);
                    if (infochannel == null)
                    {
                        ClearProperties(channelType);
                    }
                    else
                    {
                        SetHomeChannelProperties(guiPropertyPrefix, infochannel);
                    }

                    GuideProgram current = PluginMain.GetCurrentProgram(channelType);
                    GuideProgram next = PluginMain.GetNextprogram(channelType);

                    if (current == null)
                    {
                        ClearCurrentProgramProperties(guiPropertyPrefix);
                    }
                    else
                    {
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.start", current.StartTime.ToShortTimeString());
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.stop", current.StopTime.ToShortTimeString());
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.remaining", global::MediaPortal.Util.Utils.SecondsToHMSString(current.StopTime - current.StartTime));
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.genre", current.Category);
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.title", current.Title);
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.subtitle", current.SubTitle);
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.episode", current.EpisodeNumberDisplay);
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.compositetitle", current.CreateProgramTitle());
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.description", current.CreateCombinedDescription(true));
                    }

                    if (next == null)
                    {
                        ClearNextProgramProperties(guiPropertyPrefix, false);
                    }
                    else
                    {
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.start", next.StartTime.ToShortTimeString());
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.stop", next.StopTime.ToShortTimeString());
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.remaining", global::MediaPortal.Util.Utils.SecondsToHMSString(next.StopTime - next.StartTime));
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.genre", next.Category);
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.title", next.Title);
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.compositetitle", next.CreateProgramTitle());
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.description", next.CreateCombinedDescription(true));
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.subtitle", next.SubTitle);
                        GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.episode", next.EpisodeNumberDisplay);
                    }

                    if (current != null)
                    {
                        TimeSpan ts2 = current.StopTime - current.StartTime;
                        if (ts2.TotalSeconds > 0)
                        {
                            // calculate total duration of the current program
                            double programDuration = ts2.TotalSeconds;

                            //calculate where the program is at this time
                            ts2 = (DateTime.Now - current.StartTime);
                            double livePoint = ts2.TotalSeconds;

                            //calculate when timeshifting was started
                            double timeShiftStartPoint = livePoint - g_Player.Duration;
                            double playingPoint = timeShiftStartPoint + g_Player.CurrentPosition;
                            if (timeShiftStartPoint < 0)
                            {
                                timeShiftStartPoint = 0;
                            }

                            double timeShiftStartPointPercent = timeShiftStartPoint / programDuration;
                            timeShiftStartPointPercent *= 100.0d;
                            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent1", timeShiftStartPointPercent.ToString());

                            double playingPointPercent = playingPoint / programDuration;
                            playingPointPercent *= 100.0d;
                            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent2", playingPointPercent.ToString());

                            double percentLivePoint = livePoint / programDuration;
                            percentLivePoint *= 100.0d;
                            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.Percentage", percentLivePoint.ToString());
                            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent3", percentLivePoint.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Info("UpdateProgressPercentageBar:{0}", ex.Source, ex.StackTrace);
                }
            }
            else //recording is playing
            {
                Recording playingRecording = RecordedBase.GetPlayingRecording();

                if (playingRecording == null
                    || playingRecording.ChannelType != channelType)
                {
                    ClearProperties(channelType);
                }
                else
                {
                    ClearNextProgramProperties(guiPropertyPrefix, true);

                    double currentPosition = g_Player.CurrentPosition;
                    double duration = g_Player.Duration;

                    string startTime = global::MediaPortal.Util.Utils.SecondsToHMSString((int)currentPosition);
                    string endTime = global::MediaPortal.Util.Utils.SecondsToHMSString((int)duration);

                    double percentLivePoint = currentPosition / duration;
                    percentLivePoint *= 100.0d;

                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent1", percentLivePoint.ToString());
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent2", "0");
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent3", "0");
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.title", playingRecording.Title);
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.compositetitle", playingRecording.CreateProgramTitle());
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.subtitle", playingRecording.SubTitle);
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.genre", playingRecording.Category);
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.episode", playingRecording.EpisodeNumberDisplay);
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.description", playingRecording.CreateCombinedDescription(true));
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.start", startTime);
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.stop", endTime);

                    SetHomeChannelProperties(guiPropertyPrefix, playingRecording.ChannelId,
                        playingRecording.ChannelDisplayName + " " + Utility.GetLocalizedText(TextId.RecordedSuffix));
                }
            }
        }

        private static void ClearCurrentProgramProperties(string guiPropertyPrefix)
        {
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.start", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.stop", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.subtitle", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.episode", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.genre", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.description", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.Percentage", "0");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent1", "0");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent2", "0");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Record.percent3", "0");
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.remaining", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.title", Utility.GetLocalizedText(TextId.NoDataAvailable));
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.compositetitle", Utility.GetLocalizedText(TextId.NoDataAvailable));
        }

        private static void ClearNextProgramProperties(string guiPropertyPrefix, bool isRecordingPlaying)
        {
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.start", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.stop", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.description", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.subtitle", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.episode", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.genre", String.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.remaining", String.Empty);

            if (isRecordingPlaying)
            {
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.title", String.Empty);
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.compositetitle", String.Empty);
            }
            else
            {
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.title", Utility.GetLocalizedText(TextId.NoDataAvailable));
                GUIPropertyManager.SetProperty(guiPropertyPrefix + ".Next.compositetitle", Utility.GetLocalizedText(TextId.NoDataAvailable));
            }
        }

        private static void UpdateAudioProperties(int currAudio, string guiPropertyPrefix)
        {
            string streamType = g_Player.AudioType(currAudio);

            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsAC3", string.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsMP1A", string.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsMP2A", string.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsAAC", string.Empty);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsLATMAAC", string.Empty);

            switch (streamType)
            {
                case "AC3":
                case "AC3plus": // just for the time being use the same icon for AC3 & AC3plus
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsAC3",
                                                   string.Format("{0}{1}{2}", GUIGraphicsContext.Skin, @"\Media\Logos\",
                                                                 "ac3.png"));
                    break;

                case "Mpeg1":
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsMP1A",
                                                   string.Format("{0}{1}{2}", GUIGraphicsContext.Skin, @"\Media\Logos\",
                                                                 "mp1a.png"));
                    break;

                case "Mpeg2":
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsMP2A",
                                                   string.Format("{0}{1}{2}", GUIGraphicsContext.Skin, @"\Media\Logos\",
                                                                 "mp2a.png"));
                    break;

                case "AAC":
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsAAC",
                                                   string.Format("{0}{1}{2}", GUIGraphicsContext.Skin, @"\Media\Logos\",
                                                                 "aac.png"));
                    break;

                case "LATMAAC":
                    GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.IsLATMAAC",
                                                   string.Format("{0}{1}{2}", GUIGraphicsContext.Skin, @"\Media\Logos\",
                                                            "latmaac3.png"));
                    break;
            }
        }

        private static void SetHomeChannelProperties(string guiPropertyPrefix, Channel channel)
        {
            if (channel == null)
            {
                SetHomeChannelProperties(guiPropertyPrefix, Guid.Empty, String.Empty);
            }
            else
            {
                SetHomeChannelProperties(guiPropertyPrefix, channel.ChannelId, channel.DisplayName);
            }
        }

        private static void SetHomeChannelProperties(string guiPropertyPrefix, Guid channelId, string channelName)
        {
            string logo = String.Empty;
            if (channelId != Guid.Empty)
            {
                logo = Utility.GetLogoImage(channelId, channelName, new SchedulerServiceProxy());
                if (String.IsNullOrEmpty(logo))
                {
                    logo = "defaultVideoBig.png";
                }
            }
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.channel", channelName);
            GUIPropertyManager.SetProperty(guiPropertyPrefix + ".View.thumb", logo);
        }

        #endregion

        #region ActiveRecordingsDialog

        private void OnActiveRecordings()
        {
            List<Guid> ignoreActiveRecordings = new List<Guid>();
            OnActiveRecordings(ignoreActiveRecordings);
        }

        private void OnActiveRecordings(List<Guid> ignoreActiveRecordings)
        {
            var controlProxy = new ControlServiceProxy();

            List<ActiveRecording> activeRecordings = controlProxy.GetActiveRecordings();

            if (activeRecordings != null && activeRecordings.Count > 0)
            {
                GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);
                if (dlg == null)
                {
                    return;
                }

                dlg.Reset();
                dlg.SetHeading(200052); // Active Recordings   

                List<ActiveRecording> listedRecordings = new List<ActiveRecording>();
                foreach (ActiveRecording activeRecording in activeRecordings)
                {
                    if (!ignoreActiveRecordings.Contains(activeRecording.RecordingId))
                    {
                        GUIListItem item = new GUIListItem();
                        string channelName = activeRecording.Program.Channel.DisplayName;
                        string programTitle = activeRecording.Program.Title;
                        string time = String.Format("{0}-{1}",
                            activeRecording.Program.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                            activeRecording.Program.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                        item.Label = channelName;
                        item.Label2 = programTitle + "  " + time;

                        string strLogo = Utility.GetLogoImage(activeRecording.Program.Channel, new SchedulerServiceProxy());
                        if (string.IsNullOrEmpty(strLogo))
                        {
                            strLogo = "defaultVideoBig.png";
                        }

                        item.IconImage = strLogo;
                        item.IconImageBig = strLogo;
                        dlg.Add(item);
                        listedRecordings.Add(activeRecording);
                    }
                }

                dlg.SelectedLabel = listedRecordings.Count - 1;

                dlg.DoModal(this.GetID);
                if (dlg.SelectedLabel < 0 || listedRecordings.Count == 0 || (dlg.SelectedLabel - 1 > listedRecordings.Count))
                {
                    return;
                }

                ActiveRecording selectedRecording = listedRecordings[dlg.SelectedLabel];
                listedRecordings = null;

                bool deleted = OnAbortActiveRecording(selectedRecording);
                if (deleted && !ignoreActiveRecordings.Contains(selectedRecording.RecordingId))
                {
                    ignoreActiveRecordings.Add(selectedRecording.RecordingId);
                }

                if (deleted)
                {
                    OnActiveRecordings(ignoreActiveRecordings); //keep on showing the list until --> 1) user leaves menu, 2) no more active recordings
                }
            }
            else if (ignoreActiveRecordings == null || ignoreActiveRecordings.Count == 0)
            {
                GUIDialogOK pDlgOK = (GUIDialogOK)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_OK);
                if (pDlgOK != null)
                {
                    pDlgOK.SetHeading(200052); //my tv
                    pDlgOK.SetLine(1, GUILocalizeStrings.Get(200053)); // No Active recordings
                    pDlgOK.SetLine(2, "");
                    pDlgOK.DoModal(this.GetID);
                }
            }
        }

        private bool OnAbortActiveRecording(ActiveRecording rec)
        {
            var controlProxy = new ControlServiceProxy();

            if (rec == null) return false;
            bool aborted = false;
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            if (dlg == null) return false;
            dlg.Reset();
            dlg.SetHeading(rec.Program.Title);
            dlg.AddLocalizedString(1449); //Stop recording
            dlg.AddLocalizedString(979);  //Play recorded from beginning
            dlg.AddLocalizedString(980);  //Play recorded from live point

            Recording recording = controlProxy.GetRecordingById(rec.RecordingId);
            if (recording != null && recording.LastWatchedPosition.HasValue)
            {
                dlg.AddLocalizedString(900);//play from last point
            }

            dlg.DoModal(GetID);
            switch (dlg.SelectedId)
            {
                case 979:
                    if (recording != null)
                    {
                        RecordedBase.PlayFromPreRecPoint(recording);
                    }
                    break;

                case 980: 
                    RecordedBase.PlayFromLivePoint(rec);
                    break;

                case 900:
                    if (recording != null)
                    {
                        RecordedBase.PlayRecording(recording, recording.LastWatchedPosition.Value);
                    }
                    break;

                case 1449: // Abort
                    GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
                    if (dlgYesNo != null)
                    {
                        dlgYesNo.Reset();
                        dlgYesNo.SetHeading(Utility.GetLocalizedText(TextId.StopRecording));
                        dlgYesNo.SetLine(1, rec.Program.Channel.DisplayName);
                        dlgYesNo.SetLine(2, rec.Program.Title);
                        dlgYesNo.SetLine(3, string.Empty);
                        dlgYesNo.SetDefaultToYes(false);
                        dlgYesNo.DoModal(GetID);

                        if (dlgYesNo.IsConfirmed)
                        {
                            controlProxy.AbortActiveRecording(rec);
                            aborted = true;
                        }
                    }
                    break;
            }
            return aborted;
        }

        #endregion

        #region Serialisation

        public static void OnSettingChanged()
        {
            _settingsLoaded = false;
            LoadSettings();
        }

        private static void LoadSettings()
        {
            if (!_settingsLoaded)
            {
                Log.Debug("HomeBase: LoadSettings()");
                using (Settings xmlreader = new MPSettings())
                {
                    string preferredLanguages = xmlreader.GetValueAsString("tvservice", "preferredaudiolanguages", String.Empty);
                    _preferredLanguages = preferredLanguages.Split(';');
                    _preferAC3 = xmlreader.GetValueAsBool("tvservice", "preferac3", false);
                    _preferAudioTypeOverLang = xmlreader.GetValueAsBool("tvservice", "preferAudioTypeOverLang", true);
                    _showChannelStateIcons = xmlreader.GetValueAsBool("mytv", "showChannelStateIcons", true);
                    if (xmlreader.GetValueAsBool("general", "wait for tvserver", false))
                    {
                        xmlreader.SetValueAsBool("general", "wait for tvserver", false);
                    }

                    //hack for minidisplay plugin, minidislay don't work with ARGUS TV as it wants to connect to mp tv server.
                    //by clearing the hostname, minidisplay don't search for a tv server and minidisplay is working.
                    string mpTvserviceHostname = xmlreader.GetValueAsString("tvservice", "hostname", "");
                    if (mpTvserviceHostname != string.Empty)
                    {
                        xmlreader.SetValue("tvservice", "hostname", string.Empty);
                    }
                    _settingsLoaded = true;
                }
            }
        }

        private void SaveSettings()
        {
        }

        #endregion

        #region Others

        protected bool PreviousWindowWasPluginWindow()
        {
            int prevId = GUIWindowManager.GetWindow(GUIWindowManager.ActiveWindow).PreviousWindowId;
            return prevId == (int)GUIWindow.Window.WINDOW_TV_CROP_SETTINGS
                || prevId == (int)GUIWindow.Window.WINDOW_SETTINGS_SORT_CHANNELS
                || prevId == (int)GUIWindow.Window.WINDOW_SETTINGS_TV_EPG
                || prevId == (int)GUIWindow.Window.WINDOW_TVFULLSCREEN
                || prevId == (int)GUIWindow.Window.WINDOW_MINI_GUIDE
                || prevId == (int)GUIWindow.Window.WINDOW_TV_SEARCH
                || prevId == (int)GUIWindow.Window.WINDOW_TV_SEARCHTYPE
                || prevId == (int)GUIWindow.Window.WINDOW_TV_SCHEDULER_PRIORITIES
                || prevId == (int)GUIWindow.Window.WINDOW_TV_RECORDED_INFO
                || prevId == (int)GUIWindow.Window.WINDOW_SETTINGS_RECORDINGS
                || prevId == (int)GUIWindow.Window.WINDOW_SCHEDULER
                || prevId == (int)GUIWindow.Window.WINDOW_SEARCHTV
                || prevId == (int)GUIWindow.Window.WINDOW_TV_TUNING_DETAILS
                || prevId == (int)GUIWindow.Window.WINDOW_TV
                || prevId == (int)GUIWindow.Window.WINDOW_TVGUIDE
                || prevId == (int)GUIWindow.Window.WINDOW_RADIO
                || prevId == (int)GUIWindow.Window.WINDOW_RADIO_GUIDE
                || prevId == WindowId.TvGuide
                || prevId == WindowId.ProgramInfo
                || prevId == WindowId.RecordedTv
                || prevId == WindowId.UpcomingTvPrograms
                || prevId == WindowId.UpcomingRadioPrograms
                || prevId == WindowId.ActiveRecordings
                || prevId == WindowId.RecordedTvInfo
                || prevId == WindowId.TvGuideSearch
                || prevId == WindowId.RadioGuide
                || prevId == WindowId.RadioGuideSearch
                || prevId == WindowId.RecordedRadio
                || prevId == WindowId.TvGuideSearch
                || prevId == WindowId.ManualShedule;
        }

        #endregion
    }
}
