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
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Profile;
using MediaPortal.Util;
using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.MediaPortal
{
    public class TvHome : HomeBase, ISetupForm, IShowPlugin, IPluginReceiver
    {
        public TvHome()
            : base(ChannelType.Television){}

        private const int _keepAliveIntervalSeconds = 10;
        private const string _settingSection = "argustv";

        internal static class SettingName
        {
            public const string Server = "server";
            public const string TcpPort = "tcpPort";
            public const string MacAddresses = "macAddresses";
            public const string IPAddress = "ipAddress";
            public const string UseWakeOnLan = "useWakeOnLan";
            public const string WakeOnLanTimeoutSeconds = "wakeOnLanTimeoutSeconds";
            public const string NoClientStandbyWhenNotHome = "noClientStandbyWhenNotHome";
            public const string AutoStreamingMode = "autoStreamingMode";
            public const string AvoidRtspForLiveTv = "avoidRtspForLiveTv";
            public const string PlayRecordingsOverRtsp = "playRecordingsOverRtsp";
            public const string IsSingleSeat = "isSingleSeat";
            public const string DisableRadio = "disableRadio";
        }

        private static int _preNotifyConfig;
        private static int _notifyTVTimeout;
        private static bool _playNotifyBeep;
        private static bool _enableRecNotification;
        private static bool _autoTurnOnTv;
        private static bool _settingsLoaded;
        private static bool _started;
        private static bool _showlastactivemodule;
        private static bool _lastActivemoduleWasFullscreen;

        // Create the crop manager once to enable it.
        private TvCropManager _tvCropManager = new TvCropManager();
        private NotifyManager _notifyManager = new NotifyManager();

        #region ISetupForm Members

        public string Author()
        {
            return "http://www.argus-tv.com/ (dot-i)";
        }

        public bool CanEnable()
        {
            return true;
        }

        public bool DefaultEnabled()
        {
            return true;
        }

        public string Description()
        {
            return "ARGUS TV plugin";
        }

        public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = Utility.GetLocalizedText(TextId.MyTv);
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = "hover_my tv.png";
            return true;
        }

        public int GetWindowId()
        {
            return WindowId.TvHome;
        }

        public bool HasSetup()
        {
            return true;
        }

        public string PluginName()
        {
            return "ARGUS TV";
        }

        public void ShowPlugin()
        {
            using (Settings xmlwriter = new MPSettings())
            {
                SetupForm setupForm = new SetupForm();
                setupForm.ServerSettings = PluginMain.LoadServerSettings();
                setupForm.NoClientStandbyWhenNotHome = PluginMain.NoClientStandbyWhenNotHome;
                setupForm.PreferRtspForLiveTv = PluginMain.PreferRtspForLiveTv;
                setupForm.PlayRecordingsOverRtsp = PluginMain.PlayRecordingsOverRtsp;
                setupForm.AutoStreamingMode = PluginMain.AutoStreamingMode;
                setupForm.DisableRadio = PluginMain.DisableRadio;
                if (setupForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    xmlwriter.SetValue(_settingSection, SettingName.Server, setupForm.ServerSettings.ServerName);
                    xmlwriter.SetValue(_settingSection, SettingName.TcpPort, setupForm.ServerSettings.Port);
                    xmlwriter.SetValue(_settingSection, SettingName.MacAddresses, setupForm.ServerSettings.WakeOnLan.MacAddresses);
                    xmlwriter.SetValue(_settingSection, SettingName.IPAddress, setupForm.ServerSettings.WakeOnLan.IPAddress);
                    xmlwriter.SetValueAsBool(_settingSection, SettingName.UseWakeOnLan, setupForm.ServerSettings.WakeOnLan.Enabled);
                    xmlwriter.SetValue(_settingSection, SettingName.WakeOnLanTimeoutSeconds, setupForm.ServerSettings.WakeOnLan.TimeoutSeconds);
                    xmlwriter.SetValueAsBool(_settingSection, SettingName.NoClientStandbyWhenNotHome, setupForm.NoClientStandbyWhenNotHome);
                    xmlwriter.SetValueAsBool(_settingSection, SettingName.AvoidRtspForLiveTv, !setupForm.PreferRtspForLiveTv);
                    xmlwriter.SetValueAsBool(_settingSection, SettingName.PlayRecordingsOverRtsp, setupForm.PlayRecordingsOverRtsp);
                    xmlwriter.SetValueAsBool(_settingSection, SettingName.AutoStreamingMode, setupForm.AutoStreamingMode);
                    xmlwriter.SetValueAsBool(_settingSection, SettingName.IsSingleSeat, setupForm.IsSingleSeat);
                    xmlwriter.SetValueAsBool(_settingSection, SettingName.DisableRadio, setupForm.DisableRadio);
                    PluginMain.ClearCachedBooleanSettings();
                }
            }
        }

        #endregion

        #region Keep Alive thread

        private ManualResetEvent _keepAliveThreadStopEvent;
        private Thread _keepAliveThread;
        private DateTime _keepAliveServerTime = DateTime.MinValue;

        private void StartKeepAliveThread()
        {
            _keepAliveServerTime = DateTime.MinValue;
            _keepAliveThreadStopEvent = new ManualResetEvent(false);
            ThreadStart threadStart = new ThreadStart(KeepAliveThreadMain);
            _keepAliveThread = new Thread(threadStart);
            _keepAliveThread.Start();
        }

        private void StopKeepAliveThread()
        {
            if (_keepAliveThread != null)
            {
                _keepAliveThreadStopEvent.Set();
                _keepAliveThread.Join();
                _keepAliveThreadStopEvent.Close();
                _keepAliveThreadStopEvent = null;
                _keepAliveThread = null;
            }
        }

        /// <summary>
        /// To keep the stream, server and this system alive when needed.
        /// - System and server are kept alive when there is playing something, when a slideshow is active
        ///   or when you aren't on the home screen (if configured).
        /// - So no need for the powerscheduler plugin.
        /// </summary>
        private int DoKeepAlive(int param1, int param2, object data)
        {
            if (DateTime.Now >= _keepAliveServerTime)
            {
                _keepAliveServerTime = DateTime.Now.AddSeconds(110);
                bool systemNeeded = false;
                bool displayNeeded = false;

                if (!g_Player.Playing)
                {
                    int activeWindow = GUIWindowManager.ActiveWindow;
                    if (PluginMain.NoClientStandbyWhenNotHome)
                    {
                        if (activeWindow != (int)GUIWindow.Window.WINDOW_HOME
                            && activeWindow != (int)GUIWindow.Window.WINDOW_SECOND_HOME)
                        {
                            systemNeeded = true;
                        }
                    }
                    if (activeWindow == (int)GUIWindow.Window.WINDOW_SLIDESHOW)
                    {
                        systemNeeded = true;
                        displayNeeded = true;
                    }
                }
                else
                {
                    displayNeeded = true;
                    systemNeeded = true;
                }

                if (systemNeeded)
                {
                    try
                    {
                        //tell the system we need it
                        SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED);

                        if (PluginMain.IsConnected())
                        {
                            // Tell the server we need it.
                            Proxies.CoreService.KeepServerAlive();
                        }

                        if (displayNeeded)
                        {
                            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED);
                        }
                    }
                    catch { Log.Error("TvHome: DoKeepAlive error"); }
                }
            }

            try
            {
                if (PluginMain.IsConnected())
                {
                    PluginMain.Navigator.SendLiveStreamKeepAlive();
                }
            }
            catch(Exception ex)
            {
                Log.Error("DoKeepAlive: failed to keep live stream alive: {0}", ex.ToString());
            }
            return 0;
        }

        private void KeepAliveThreadMain()
        {
            do { GUIWindowManager.SendThreadCallbackAndWait(DoKeepAlive, 0, 0, null); }
            while (!_keepAliveThreadStopEvent.WaitOne(_keepAliveIntervalSeconds * 1000, false));
        }

        #endregion

        #region Cache Channels thread

        /// <summary>
        /// After standby, reboot,... miniguide takes long to load.
        /// Miniguide data gets cached on the first access, 
        /// so this thread access the miniguide data on mp startup, resume, 
        /// so miniguide will be faster on the firts opening.
        /// It's better to do this on the server side, but for now we do it in this way.
        /// </summary>
        /// 
        private Thread _CacheMiniGuide;
        private void StartCacheMiniGuideChannelsThread()
        {
            if (_CacheMiniGuide != null
                && _CacheMiniGuide.IsAlive)
            {
                return;
            }
            _CacheMiniGuide = new Thread(CacheMiniGuide);
            _CacheMiniGuide.Name = "CacheChannels";
            _CacheMiniGuide.Priority = ThreadPriority.Lowest;
            _CacheMiniGuide.Start();
        }

        private void StopCacheMiniGuideChannelsThread()
        {
            if (_CacheMiniGuide != null
                && _CacheMiniGuide.IsAlive)
            {
                _CacheMiniGuide.Abort();
            }
        }

        private object chacheLock = new object();
        private void CacheMiniGuide()
        {
            lock (chacheLock)
            {
                System.Threading.Thread.Sleep(8000);
                while (!PluginMain.IsConnected())
                {
                    Thread.Sleep(2000);
                }
                Log.Debug("CacheChannelsThread: started");

                List<CurrentAndNextProgram> _currentAndNextPrograms = new List<CurrentAndNextProgram>();
                try
                {
                    ChannelGroup currgroup = PluginMain.Navigator.CurrentGroup;
                    if (currgroup != null)
                    {
                        _currentAndNextPrograms = new List<CurrentAndNextProgram>(Proxies.SchedulerService.GetCurrentAndNextForGroup(PluginMain.Navigator.CurrentGroup.ChannelGroupId, true, null).Result);
                    }

                    List<ChannelGroup> groups = PluginMain.Navigator.GetGroups(ChannelType.Television);
                    foreach (ChannelGroup group in groups)
                    {
                        _currentAndNextPrograms = new List<CurrentAndNextProgram>(Proxies.SchedulerService.GetCurrentAndNextForGroup(group.ChannelGroupId, true, null).Result);
                    }
                }
                catch { Log.Error("CacheChannelsThread: error"); }

                _currentAndNextPrograms.Clear();
                _currentAndNextPrograms = null;

                Log.Debug("CacheChannelsThread: ended");
            }
        }

        #endregion

        #region overrides

        public override bool Init()
        {
            g_Player.PlayBackStopped += new global::MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackEnded += new global::MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStarted += new global::MediaPortal.Player.g_Player.StartedHandler(OnPlayBackStarted);
            g_Player.AudioTracksReady += new g_Player.AudioTracksReadyHandler(OnAudioTracksReady);
            GUIWindowManager.Receivers += new SendMessageHandler(OnGlobalMessage);
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            NetworkChange.NetworkAvailabilityChanged += OnNetworkStateChanged;
            PluginMain.NetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            
            bool result = Load(GUIGraphicsContext.Skin + @"\ARGUS_Home.xml");
            if (result)
            {
                LoadSettings();
                base.Init();
                OnStart(false);
            }
            return result;
        }

        public override void DeInit()
        {
            g_Player.PlayBackStopped -= new global::MediaPortal.Player.g_Player.StoppedHandler(OnPlayBackStopped);
            g_Player.PlayBackEnded -= new global::MediaPortal.Player.g_Player.EndedHandler(OnPlayBackEnded);
            g_Player.PlayBackStarted -= new global::MediaPortal.Player.g_Player.StartedHandler(OnPlayBackStarted);
            g_Player.AudioTracksReady -= new g_Player.AudioTracksReadyHandler(OnAudioTracksReady);
            GUIWindowManager.Receivers -= new SendMessageHandler(OnGlobalMessage);
            Application.ApplicationExit -= new EventHandler(OnApplicationExit);
            NetworkChange.NetworkAvailabilityChanged -= OnNetworkStateChanged;
            OnStop(false);
            base.DeInit();
        }

        public override void OnAdded()
        {
            Log.Debug("TvHome:OnAdded");
            g_Player.ShowFullScreenWindowTV = ShowFullScreenWindowTVHandler;
        }

        private static bool ShowFullScreenWindowTVHandler()
        {
            SetRecordingChaptersAndJumpPoints();
            if ((g_Player.IsTV && PluginMain.Navigator.IsLiveStreamOn) || g_Player.IsTVRecording)
            {
                // watching TV
                if (GUIWindowManager.ActiveWindow == (int)Window.WINDOW_TVFULLSCREEN)
                {
                    return true;
                }
                Log.Info("TVHome: ShowFullScreenWindow switching to fullscreen tv");
                GUIWindowManager.ActivateWindow((int)Window.WINDOW_TVFULLSCREEN);
                GUIGraphicsContext.IsFullScreenVideo = true;
                return true;
            }
            return g_Player.ShowFullScreenWindowTVDefault();
        }

        internal static void SetRecordingChaptersAndJumpPoints()
        {
            //push chapter and jumppoint information into the gui property manager
            if (g_Player.IsTVRecording && g_Player.HasChapters)
            {
                double[] chapters = g_Player.Chapters;
                double[] jumppoints = g_Player.JumpPoints;

                string strChapters = string.Empty;
                string strJumpPoints = string.Empty;

                double duration = g_Player.Duration;
                if (chapters != null)
                {
                    foreach (double chapter in chapters)
                    {
                        double chapterPercent = chapter / duration * 100.0d;
                        strChapters += String.Format("{0:0.00}", chapterPercent) + " ";
                    }
                }
                if (jumppoints != null)
                {
                    foreach (double jump in jumppoints)
                    {
                        double jumpPercent = jump / duration * 100.0d;
                        strJumpPoints += String.Format("{0:0.00}", jumpPercent) + " ";
                    }
                }
                GUIPropertyManager.SetProperty("#TV.Record.chapters", strChapters);
                GUIPropertyManager.SetProperty("#TV.Record.jumppoints", strJumpPoints);
                Log.Debug("TVHome.ShowFullScreenWindowTVHandler - setting chapters: " + strChapters);
                Log.Debug("TVHome.ShowFUllScreenWindowTVHandler - setting jumppoints: " + strJumpPoints);
            }
            else
            {
                GUIPropertyManager.SetProperty("#TV.Record.chapters", string.Empty);
                GUIPropertyManager.SetProperty("#TV.Record.jumppoints", string.Empty);
            }
        }

        protected override void OnWindowLoaded()
        {
            base.OnWindowLoaded();
            _tvGuideButton.Label = Utility.GetLocalizedText(TextId.TvGuide);
            _recordingsButton.Label = Utility.GetLocalizedText(TextId.RecordedTv);
            _teletextButton.IsEnabled = false;
        }

        protected override void OnPageDestroy(int new_windowId)
        {
            SaveSettings();
            base.OnPageDestroy(new_windowId);
        }

        public override int GetID
        {
            get { return WindowId.TvHome; }
            set { }
        }

        #endregion

        #region public methods

        public static void UpdateProgressPercentageBar()
        {
            UpdateProgressPercentageBar(false);
        }

        public static void UpdateProgressPercentageBar(bool ForceUpdate)
        {
            DoUpdateProgressPercentageBar(ChannelType.Television, ForceUpdate);
        }

        #endregion

        #region private methods

        private void OnStop(bool suspend)
        {
            if (_started)
            {
                try
                {
                    SaveSettings();
                    StopEventListenerTask();
                    StopKeepAliveThread();
                    StopCacheMiniGuideChannelsThread();
                    _notifyManager.stop();

                    g_Player.Stop();
                    if (PluginMain.Navigator != null)
                    {
                        PluginMain.Navigator.AsyncStopLiveStream();
                        if (!suspend)
                        {
                            PluginMain.Navigator = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("TvHome: OnStop error -{0}", ex.Message);
                }
                _started = false;
            }
        }

        private void OnStart(bool resume)
        {
            if (!_started)
            {
                try
                {
                    using (Settings xmlreader = new MPSettings())
                    {
                        _lastActivemoduleWasFullscreen = xmlreader.GetValueAsBool("general", "lastactivemoduleTvfullscreen", false);
                    }

                    PluginMain.EnsureConnection(true, false);
                    _notifyManager.start();
                    StartKeepAliveThread();
                    StartCacheMiniGuideChannelsThread();
                    EnsureEventListenerTaskStarted();
                    
                    if (PluginMain.IsConnected())
                    {
					    //TODO
                        if (_showlastactivemodule && _lastActivemoduleWasFullscreen)
                        {
                            Channel previousChannel = PluginMain.Navigator.GetPreviousChannel(ChannelType.Television);
                            if (previousChannel != null)
                            {
                                //PluginMain.Navigator.ZapToChannel(previousChannel, false);
                            }
                       }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("TvHome: OnStart error -{0}", ex.Message);
                }
                _started = true;
            }
        }

        #endregion

        #region global/mediaportal events

        private void OnApplicationExit(object sender, EventArgs e)
        {
            OnStop(false);
        }

        private void OnNetworkStateChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            //string heading;
            //string message;

            if (e.IsAvailable)
            {
                //Log.Info("TvHome: Network connected!");
                //heading = Utility.GetLocalizedText(TextId.Information);
                //message = "Network connected!";
                PluginMain.NetworkAvailable = true;
            }
            else
            {
                //Log.Error("TvHome: Network disconnected!");
                //heading = Utility.GetLocalizedText(TextId.Error);
                //message = "Network disconnected!";
                PluginMain.NetworkAvailable = false;

                //if (!PluginMain.IsSingleSeat)
                //{
                    /*if (g_Player.Playing && (g_Player.IsRadio || g_Player.IsTV || g_Player.IsTVRecording))
                    {
                        //g_Player.Stop();
                        GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_RECORDER_VIEW_CHANNEL, 0, 0, 0, 0, 0, null);
                        msg.Param1 = 1111;
                        //msg.Object = recording;
                        GUIGraphicsContext.SendMessage(msg);
                        msg = null;
                    }*/
                    
                    /*using (Settings xmlreader = new MPSettings())
                    {
                        bool _startWithBasicHome = xmlreader.GetValueAsBool("gui", "startbasichome", false);
                        if (_startWithBasicHome && System.IO.File.Exists(GUIGraphicsContext.Skin + @"\basichome.xml"))
                        {
                            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_SECOND_HOME);
                        }
                        else
                        {
                            GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_HOME);
                        }
                    }*/
                //}

                

                /*GUIDialogNotify pDlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                if (pDlgNotify != null)
                {
                    pDlgNotify.Reset();
                    pDlgNotify.ClearAll();
                    pDlgNotify.SetHeading(heading);
                    pDlgNotify.SetText(message);
                    pDlgNotify.TimeOut = 5;
                    //Utils.PlaySound("notify.wav", false, true);
                    pDlgNotify.DoModal(GUIWindowManager.ActiveWindow);
                }*/
            }

            /*GUIDialogNotify pDlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
            if (pDlgNotify != null)
            {
                pDlgNotify.Reset();
                pDlgNotify.ClearAll();
                pDlgNotify.SetHeading(heading);
                pDlgNotify.SetText(message);
                pDlgNotify.TimeOut = 5;
                Utils.PlaySound("notify.wav", false, true);
                pDlgNotify.DoModal(GUIWindowManager.ActiveWindow);
            }*/
        }

        public static void OnGlobalMessage(GUIMessage message)
        {
            switch (message.Message)
            {
                /// <summary>
                /// We need to stop the player if our livestream ends unexpectedly.
                /// If the stream stopped for a recording, we show it in a message.
                /// Without this mediaportal can hang,crash (c++ error in tsreader).
                /// </summary>
                case GUIMessage.MessageType.GUI_MSG_STOP_SERVER_TIMESHIFTING:
                    {
                        Log.Debug("TvHome: GUI_MSG_STOP_SERVER_TIMESHIFTING, param1 = {0}",message.Param1);
                        if (PluginMain.Navigator.IsLiveStreamOn)
                        {
                            if (message.Param1 == 4321)//fired by eventlistener
                            {
                                LiveStream liveStream = message.Object as LiveStream;
                                LiveStream navigatorLiveStream = PluginMain.Navigator.LiveStream;
                                Channel channel = PluginMain.Navigator.CurrentChannel;

                                if (liveStream != null && channel != null 
                                    && navigatorLiveStream.TimeshiftFile == liveStream.TimeshiftFile
                                    && liveStream.StreamStartedTime == navigatorLiveStream.StreamStartedTime)
                                {
                                    if (g_Player.Playing && (g_Player.IsTV || g_Player.IsRadio))
                                    {
                                        g_Player.Stop();
                                        Log.Info("TvHome: our live stream seems to be aborted, stop the playback now");
                                    }

                                    string text = GUILocalizeStrings.Get(1516);
                                    if (message.Label == LiveStreamAbortReason.RecordingStartedOnCard.ToString())
                                    {
                                        text = GUILocalizeStrings.Get(1513);
                                    }
                                    text = text.Replace("\\r", " ");

                                    string heading = string.Empty;
                                    if (channel.ChannelType == ChannelType.Television)
                                        heading = GUILocalizeStrings.Get(605) + " - " + channel.DisplayName; //my tv
                                    else
                                        heading = GUILocalizeStrings.Get(665) + " - " + channel.DisplayName; //my radio

                                    string tvlogo = Utility.GetLogoImage(channel);

                                    GUIDialogNotify pDlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                                    if (pDlgNotify != null)
                                    {
                                        pDlgNotify.Reset();
                                        pDlgNotify.ClearAll();
                                        pDlgNotify.SetHeading(heading);
                                        if (!string.IsNullOrEmpty(text))
                                        {
                                            pDlgNotify.SetText(text);
                                        }
                                        pDlgNotify.SetImage(tvlogo);
                                        pDlgNotify.TimeOut = 5;
                                        Utils.PlaySound("notify.wav", false, true);
                                        pDlgNotify.DoModal(GUIWindowManager.ActiveWindow);
                                    }
                                }
                            }
                            else//fired by mp player
                            {
                                PluginMain.Navigator.AsyncStopLiveStream();
                            }
                        }
                    }
                    break;

                case GUIMessage.MessageType.GUI_MSG_NOTIFY_REC:
                    {
                        if (_enableRecNotification)
                        {
                            Log.Debug("TvHome: GUI_MSG_NOTIFY_REC");
                            string head = string.Empty;
                            string logo = string.Empty;
                            Recording recording = message.Object as Recording;

                            if (message.Param1 == 1)
                                head = GUILocalizeStrings.Get(1446);
                            else
                                head = GUILocalizeStrings.Get(1447);

                            Channel chan = Proxies.SchedulerService.GetChannelById(recording.ChannelId).Result;
                            logo = Utility.GetLogoImage(chan);

                            string _text = String.Format("{0} {1}-{2}",
                                                  recording.Title,
                                                  recording.StartTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat),
                                                  recording.StopTime.ToString("t", CultureInfo.CurrentCulture.DateTimeFormat));

                            GUIDialogNotify DlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                            if (DlgNotify != null)
                            {
                                DlgNotify.Reset();
                                DlgNotify.ClearAll();
                                DlgNotify.SetHeading(head);
                                if (!string.IsNullOrEmpty(_text))
                                {
                                    DlgNotify.SetText(_text);
                                }
                                DlgNotify.SetImage(logo);
                                DlgNotify.TimeOut = 5;
                                if (_playNotifyBeep)
                                {
                                    Utils.PlaySound("notify.wav", false, true);
                                }
                                DlgNotify.DoModal(GUIWindowManager.ActiveWindow);
                            }
                        }
                    }
                    break;

                case GUIMessage.MessageType.GUI_MSG_NOTIFY_TV_PROGRAM:
                    {
                        Log.Debug("TvHome: GUI_MSG_NOTIFY_TV_PROGRAM");
                        TVNotifyYesNoDialog tvNotifyDlg = (TVNotifyYesNoDialog)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_TVNOTIFYYESNO);
                        UpcomingProgram prog = message.Object as UpcomingProgram;
                        if (tvNotifyDlg == null || prog == null)
                        {
                            return;
                        }

                        tvNotifyDlg.Reset();
                        if (prog.StartTime > DateTime.Now)
                        {
                            int minUntilStart = (prog.StartTime - DateTime.Now).Minutes;
                            if (minUntilStart > 1)
                            {
                                tvNotifyDlg.SetHeading(String.Format(GUILocalizeStrings.Get(1018), minUntilStart));
                            }
                            else
                            {
                                tvNotifyDlg.SetHeading(1019); // Program is about to begin
                            }
                        }
                        else
                        {
                            tvNotifyDlg.SetHeading(String.Format(GUILocalizeStrings.Get(1206), (DateTime.Now - prog.StartTime).Minutes.ToString()));
                        }

                        string description = GUILocalizeStrings.Get(736);

                        try
                        {
                            if (prog.GuideProgramId.HasValue)
                            {
                                GuideProgram Program = Proxies.GuideService.GetProgramById(prog.GuideProgramId.Value).Result;
                                description = Program.CreateCombinedDescription(false);
                            }
                        }
                        catch { }

                        tvNotifyDlg.SetLine(1, prog.Title);
                        tvNotifyDlg.SetLine(2, description);
                        tvNotifyDlg.SetLine(4, String.Format(GUILocalizeStrings.Get(1207), prog.Channel.DisplayName));
                        string strLogo = Utility.GetLogoImage(prog.Channel);

                        tvNotifyDlg.SetImage(strLogo);
                        tvNotifyDlg.TimeOut = _notifyTVTimeout;
                        if (_playNotifyBeep)
                        {
                            Utils.PlaySound("notify.wav", false, true);
                        }
                        tvNotifyDlg.SetDefaultToYes(false);
                        tvNotifyDlg.DoModal(GUIWindowManager.ActiveWindow);

                        if (tvNotifyDlg.IsConfirmed)
                        {
                            try
                            {
                                if (prog.Channel.ChannelType == ChannelType.Television)
                                {
                                    if (g_Player.Playing && g_Player.IsTV && PluginMain.Navigator.IsLiveStreamOn)
                                        GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_TVFULLSCREEN);
                                    else
                                        GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_TV);

                                    PluginMain.Navigator.ZapToChannel(prog.Channel, false);
                                    if (PluginMain.Navigator.CheckChannelChange())
                                    {
                                        TvHome.UpdateProgressPercentageBar(true);
                                        if (!PluginMain.Navigator.LastChannelChangeFailed)
                                        {
                                            g_Player.ShowFullScreenWindow();
                                        }
                                    }
                                }
                                else
                                {
                                    PluginMain.Navigator.ZapToChannel(prog.Channel, false);
                                    GUIWindowManager.ActivateWindow((int)GUIWindow.Window.WINDOW_RADIO);
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Error("TVHome: TVNotification: Error on starting channel {0} after notification: {1} {2} {3}", prog.Channel.DisplayName, e.Message, e.Source, e.StackTrace);
                            }
                        }
                    }
                    break;

                //this (GUI_MSG_RECORDER_VIEW_CHANNEL) event is used to let other plugins play a recording,
                //lastMediaHandler does this (with param1 = 5577 for indentification).
                case GUIMessage.MessageType.GUI_MSG_RECORDER_VIEW_CHANNEL:
                    {
                        if (message.Param1 == 5577)
                        {
                            try
                            {
                                Recording rec = message.Object as Recording;
                                RecordedBase.PlayRecording(rec, message.Param2);
                            }
                            catch { Log.Error("TVHome: GUI_MSG_RECORDER_VIEW_CHANNEL error"); }
                        }
                    }
                    break;
            }
        }

        #endregion

        #region Playback Events

        private void OnPlayBackStopped(global::MediaPortal.Player.g_Player.MediaType type, int stoptime, string filename)
        {
            Log.Debug("TvHome: OnPlayBackStopped");
            GUIGraphicsContext.RenderBlackImage = false;

            if (type == g_Player.MediaType.TV || type == g_Player.MediaType.Radio)
            {
                PluginMain.Navigator.AsyncStopLiveStream();
            }
        }

        private void OnPlayBackEnded(global::MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            Log.Debug("TvHome: OnPlayBackEnded");
            GUIGraphicsContext.RenderBlackImage = false;

            if (type == g_Player.MediaType.TV || type == g_Player.MediaType.Radio)
            {
                if (PluginMain.Navigator.IsLiveStreamOn)
                {
                    g_Player.Stop();
                    PluginMain.Navigator.StopLiveStream();
                }
            }
        }

        private void OnPlayBackStarted(global::MediaPortal.Player.g_Player.MediaType type, string filename)
        {
            Log.Debug("TvHome: OnPlayBackStarted");
            if (PluginMain.Navigator.IsLiveStreamOn)
            {
                if (type == g_Player.MediaType.TV || type == g_Player.MediaType.Radio)
                {   
                    // set audio track based on user prefs. 
                    g_Player.CurrentAudioStream = HomeBase.GetPreferedAudioStreamIndex();
                }
                else
                {   
                    // Playback of another type has started while live TV was still on, so let's end the live TV stream.
                    PluginMain.Navigator.StopLiveStream();
                }
            }
        }

        private void OnAudioTracksReady()
        {
            g_Player.CurrentAudioStream = HomeBase.GetPreferedAudioStreamIndex();
        }

        #endregion

        #region Serialisation

        public static void SettingChanged()
        {
            _settingsLoaded = false;
            LoadSettings();
        }

        private static void LoadSettings()
        {
            if (!_settingsLoaded)
            {
                Log.Debug("Tvhome: LoadSettings()");
                using (Settings xmlreader = new MPSettings())
                {
                    if (PluginMain.DisableRadio)
                    {
                        xmlreader.SetValueAsBool("pluginswindows", "ArgusTV.UI.MediaPortal.RadioHome", false);
                        xmlreader.SetValueAsBool("home", "ARGUS Radio", false);
                        xmlreader.SetValueAsBool("myplugins", "ARGUS Radio", false);
                    }

                    string strValue = xmlreader.GetValueAsString("mytv", "defaultar", "Normal");
                    _preNotifyConfig = xmlreader.GetValueAsInt("mytv", "notifyTVBefore", 300);
                    _notifyTVTimeout = xmlreader.GetValueAsInt("mytv", "notifyTVTimeout", 15);
                    _playNotifyBeep = xmlreader.GetValueAsBool("mytv", "notifybeep", true);
                    _enableRecNotification = xmlreader.GetValueAsBool("mytv", "enableRecNotifier", false);
                    _autoTurnOnTv = xmlreader.GetValueAsBool("mytv", "autoturnontv", false);
                    _showlastactivemodule = xmlreader.GetValueAsBool("general", "showlastactivemodule", false);
                    GUIGraphicsContext.ARType = global::MediaPortal.Util.Utils.GetAspectRatio(strValue);
                    _settingsLoaded = true;
                }
            }
        }

        private void SaveSettings()
        {
            if (PluginMain.IsConnected() && PluginMain.Navigator != null)
            {
                PluginMain.Navigator.SaveSettings();
                using (Settings xmlreader = new MPSettings())
                {
                    //xmlreader.SetValueAsBool("general", "lastactivemoduleTvfullscreen", (g_Player.IsTV && GUIGraphicsContext.IsFullScreenVideo));
                }
            }
        }

        protected override bool AutoTurnOnStream
        {
            get { return _autoTurnOnTv; }
        }

        public static bool ShowChannelStateIcons
        {
            get { return _showChannelStateIcons; }
        }

        public static bool SettingsLoaded
        {
            get { return _settingsLoaded; }
        }

        #endregion

        #region P/Invoke

        [FlagsAttribute]
        private enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000
        }
        
        [System.Runtime.InteropServices.DllImport("Kernel32.DLL", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private extern static EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE state);

        #endregion

        #region IPlugin Members

        private const int WM_POWERBROADCAST = 0x0218;
        private const int WM_QUERYENDSESSION = 0x0011;
        private const int PBT_APMQUERYSUSPEND = 0x0000;
        private const int PBT_APMQUERYSTANDBY = 0x0001;
        private const int PBT_APMQUERYSUSPENDFAILED = 0x0002;
        private const int PBT_APMQUERYSTANDBYFAILED = 0x0003;
        private const int PBT_APMSUSPEND = 0x0004;
        private const int PBT_APMSTANDBY = 0x0005;
        private const int PBT_APMRESUMECRITICAL = 0x0006;
        private const int PBT_APMRESUMESUSPEND = 0x0007;
        private const int PBT_APMRESUMESTANDBY = 0x0008;
        private const int PBT_APMRESUMEAUTOMATIC = 0x0012;

        public bool WndProc(ref System.Windows.Forms.Message msg)
        {
            if (msg.Msg == WM_POWERBROADCAST)
            {
                switch (msg.WParam.ToInt32())
                {
                    case PBT_APMSTANDBY:
                        Log.Info("TvHome.WndProc(): Windows is going to standby");
                        OnStop(true);
                        break;
                    case PBT_APMSUSPEND:
                        Log.Info("TvHome.WndProc(): Windows is suspending");
                        OnStop(true);
                        break;
                    case PBT_APMQUERYSUSPEND:
                    case PBT_APMQUERYSTANDBY:
                        Log.Info("TvHome.WndProc(): Windows is going into powerstate (hibernation/standby)");
                        break;
                    case PBT_APMRESUMESUSPEND:
                        Log.Info("TvHome.WndProc(): Windows has resumed from hibernate mode");
                        OnStart(true);
                        break;
                    case PBT_APMRESUMESTANDBY:
                        Log.Info("TvHome.WndProc(): Windows has resumed from standby mode");
                        OnStart(true);
                        break;
                }
                return true;
            }
            return false;
        }

        public void Start()
        {}

        public void Stop()
        {}

        #endregion

        #region IShowPlugin Members

        public bool ShowDefaultHome()
        {
            return true;
        }

        #endregion

        #region Events Listener

        private readonly string _eventsClientId = Dns.GetHostName() + "-be22d158859f40ce9fdb505818fd48fe"; // Unique for the MediaPortal client!
        private bool _eventListenerSubscribed;
        private Task _eventListenerTask;
        private CancellationTokenSource _connectionCancellationTokenSource;
        private SynchronizationContext _uiSyncContext;

        private void EnsureEventListenerTaskStarted()
        {
            if (_eventListenerTask == null)
            {
                _uiSyncContext = SynchronizationContext.Current;

                _connectionCancellationTokenSource = new CancellationTokenSource();
                _eventListenerTask = new Task(() => HandleServiceEvents(_connectionCancellationTokenSource.Token),
                    _connectionCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
                _eventListenerTask.Start();
            }
        }

        private void StopEventListenerTask()
        {
            try
            {
                if (_connectionCancellationTokenSource != null)
                {
                    _connectionCancellationTokenSource.Cancel();
                    _eventListenerTask.Wait();
                }
            }
            catch
            {
            }
            finally
            {
                if (_eventListenerTask != null)
                {
                    _eventListenerTask.Dispose();
                    _eventListenerTask = null;
                }
                if (_connectionCancellationTokenSource != null)
                {
                    _connectionCancellationTokenSource.Dispose();
                    _connectionCancellationTokenSource = null;
                }
            }
        }

        private int _eventsErrorCount = 0;

        private void HandleServiceEvents(CancellationToken cancellationToken)
        {
            for (;;)
            {
                if (Proxies.IsInitialized)
                {
                    IList<ServiceEvent> events = null;
                    if (!_eventListenerSubscribed)
                    {
                        try
                        {
                            Proxies.CoreService.SubscribeServiceEvents(_eventsClientId, EventGroup.RecordingEvents | EventGroup.ScheduleEvents).Wait();
                            _eventListenerSubscribed = true;
                            _eventsErrorCount = 0;
                        }
                        catch
                        {
                        }
                    }
                    if (_eventListenerSubscribed)
                    {
                        try
                        {
                            events = Proxies.CoreService.GetServiceEvents(_eventsClientId, cancellationToken).Result;
                            if (events == null)
                            {
                                _eventListenerSubscribed = false;
                            }
                            else
                            {
                                ProcessEvents(events);
                            }
                        }
                        catch
                        {
                            if (++_eventsErrorCount > 5)
                            {
                                _eventListenerSubscribed = false;
                            }
                        }
                    }
                }
                if (cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(_eventListenerSubscribed ? 0 : 2)))
                {
                    break;
                }
            }

            if (Proxies.IsInitialized
                && _eventListenerSubscribed)
            {
                try
                {
                    Proxies.CoreService.UnsubscribeServiceEvents(_eventsClientId).Wait();
                }
                catch
                {
                }
                _eventListenerSubscribed = false;
            }
        }

        private void ProcessEvents(IList<ServiceEvent> events)
        {
            foreach (var @event in events)
            {
                if (@event.Name == ServiceEventNames.UpcomingRecordingsChanged)
                {
                    _uiSyncContext.Post(s =>
                    {
                        Log.Debug("EventListener: UpcomingRecordingsChanged()");
                        PluginMain.UpcomingRecordingsChanged = true;
                        UpdateGuide();
                    }, null);
                }
                else if (@event.Name == ServiceEventNames.UpcomingAlertsChanged)
                {
                    _uiSyncContext.Post(s =>
                    {
                        Log.Debug("EventListener: UpcomingAlertsChanged()");
                        PluginMain.UpcomingAlertsChanged = true;
                        UpdateGuide();
                    }, null);
                }
                else if (@event.Name == ServiceEventNames.UpcomingSuggestionsChanged)
                {
                    _uiSyncContext.Post(s =>
                    {
                        Log.Debug("EventListener: UpcomingSuggestionsChanged()");
                        UpdateGuide();
                    }, null);
                }
                else if (@event.Name == ServiceEventNames.RecordingStarted)
                {
                    _uiSyncContext.Post(s =>
                    {
                        Recording recording = (Recording)@event.Arguments[0];
                        Log.Debug("EventListener: recording started: {0}", recording.Title);
                        OnRecordingStarted(recording);
                    }, null);
                }
                else if (@event.Name == ServiceEventNames.RecordingEnded)
                {
                    _uiSyncContext.Post(s =>
                    {
                        Recording recording = (Recording)@event.Arguments[0];
                        Log.Debug("EventListener: recording ended: {0}", recording.Title);
                        PluginMain.ActiveRecordingsChanged = true;
                        OnRecordingEnded(recording);
                        UpdateRecordings();
                    }, null);
                }
                else if (@event.Name == ServiceEventNames.LiveStreamEnded)
                {
                    _uiSyncContext.Post(s =>
                    {
                        Log.Debug("EventListener: LiveStreamEnded()");
                        OnLiveStreamEnded((LiveStream)@event.Arguments[0], LiveStreamAbortReason.Unknown, null);
                    }, null);
                }
                else if (@event.Name == ServiceEventNames.LiveStreamAborted)
                {
                    _uiSyncContext.Post(s =>
                    {
                        LiveStream stream = (LiveStream)@event.Arguments[0];
                        LiveStreamAbortReason reason = (LiveStreamAbortReason)@event.Arguments[1];
                        Log.Debug("Eventlistener: Livestreamaborted, stream = {0}, reason = {1}", stream.RtspUrl, reason.ToString());
                        OnLiveStreamEnded(stream, reason, (UpcomingProgram)@event.Arguments[2]);
                    }, null);
                }
            }
        }

        private void OnLiveStreamEnded(LiveStream liveStream, LiveStreamAbortReason reason, UpcomingProgram program)
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_STOP_SERVER_TIMESHIFTING, 0, 0, 0, 0, 0, null);
            msg.Object = liveStream;
            msg.Object2 = program;
            msg.Label = reason.ToString();
            msg.Param1 = 4321;//indentification
            GUIGraphicsContext.SendMessage(msg);
            msg = null;
        }

        private void OnRecordingStarted(Recording recording)
        {
            PluginMain.ActiveRecordingsChanged = true;
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_NOTIFY_REC, 0, 0, 0, 0, 0, null);
            msg.Param1 = 1;//started
            msg.Object = recording;
            GUIGraphicsContext.SendMessage(msg);
            msg = null;
        }

        private void OnRecordingEnded(Recording recording)
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_NOTIFY_REC, 0, 0, 0, 0, 0, null);
            msg.Param1 = 0;//ended
            msg.Object = recording;
            GUIGraphicsContext.SendMessage(msg);
            msg = null;
        }

        private void UpdateGuide()
        {
            if (GUIWindowManager.ActiveWindow == WindowId.TvGuide || GUIWindowManager.ActiveWindow == WindowId.RadioGuide)
            {
                GuideBase.ReloadSchedules = true;
            }
        }

        private void UpdateRecordings()
        {
            if (GUIWindowManager.ActiveWindow == WindowId.RecordedRadio || GUIWindowManager.ActiveWindow == WindowId.RecordedTv)
            {
                RecordedBase.NeedUpdate = true;
            }
        }

        #endregion
    }
}
