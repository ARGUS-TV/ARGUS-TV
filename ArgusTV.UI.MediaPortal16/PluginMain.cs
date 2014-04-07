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
using System.Net;

using MediaPortal.GUI.Library;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;
using MediaPortal.Profile;

using ArgusTV.ServiceProxy;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.MediaPortal
{
    public static class PluginMain
    {
        public const string _settingSection = "argustv";
        private const string _coreService = "ArgusTVScheduler";
        private const string _argusTvService = "ArgusTVRecorder";
        private const string _mpTvService = "TVService";
        private static DateTime _programUpdateTimer = DateTime.MinValue;
        private static Guid PrevchannelId = Guid.Empty;
        private static GuideProgram _nextProgram = null;
        private static GuideProgram _currentProgram = null;
        private static bool _connected = false;
        private static bool _connectionErrorShown;
        private static object _nextlock = new object();
        private static object _currlock = new object();
        private static object _activeRecordingsLock = new object();
        private static List<ActiveRecording> _activeRecordings;
        private static DateTime _activeRecordingsUpdateTime = DateTime.MinValue;

        public static bool UpcomingAlertsChanged { get; set; }
        public static bool UpcomingRecordingsChanged { get; set; }
        public static bool ActiveRecordingsChanged { get; set; }
        public static bool ForceNoConnection { get; set; }
        public static bool NetworkAvailable { get; set; }

        #region Active Recordings
        
        internal static List<ActiveRecording> ActiveRecordings
        {
            get
            {
                if (DateTime.Now >= _activeRecordingsUpdateTime || ActiveRecordingsChanged)
                {
                    lock (_activeRecordingsLock)
                    {
                        if (DateTime.Now >= _activeRecordingsUpdateTime || ActiveRecordingsChanged)
                        {
                            ActiveRecordingsChanged = false;
                            var controlProxy = new ControlServiceProxy();
                            _activeRecordings = controlProxy.GetActiveRecordings();
                            _activeRecordingsUpdateTime = DateTime.Now.AddSeconds(15);
                        }
                    }
                }
                return _activeRecordings;
            }
        }

        internal static bool IsActiveRecording(Guid channelId, GuideProgram guideProgram)
        {
            ActiveRecording activeRecording;
            return IsActiveRecording(channelId, guideProgram, out activeRecording);
        }

        internal static bool IsActiveRecording(Guid channelId, GuideProgram guideProgram, out ActiveRecording activeRecording)
        {
            Guid upcomingProgramId = guideProgram.GetUniqueUpcomingProgramId(channelId);

            foreach (ActiveRecording recording in ActiveRecordings)
            {
                if (recording.Program.UpcomingProgramId == upcomingProgramId)
                {
                    activeRecording = recording;
                    return true;
                }
            }
            activeRecording = null;
            return false;
        }

        internal static bool IsChannelRecording(Guid channelId, out ActiveRecording activeRecording)
        {
            foreach (ActiveRecording recording in ActiveRecordings)
            {
                if (recording.CardChannelAllocation.ChannelId == channelId)
                {
                    activeRecording = recording;
                    return true;
                }
            }
            activeRecording = null;
            return false;
        }

        internal static bool IsRecordingStillActive(Guid recordingID)
        {
            foreach (ActiveRecording rec in ActiveRecordings)
            {
                if (rec.RecordingId == recordingID)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Programs

        private static ChannelNavigator _navigator;
        internal static ChannelNavigator Navigator
        {
            get
            {
                if (_navigator == null && PluginMain.IsConnected())
                {
                    _navigator = new ChannelNavigator();
                    _navigator.LoadSettings();
                }
                return _navigator;
            }
            set
            {
                if (_navigator != null)
                {
                    _navigator.Dispose();
                }
                _navigator = value;
            }
        }

        public static GuideProgram GetCurrentProgram(ChannelType channeltype)
        {
            return getCurrentProgram(PluginMain.Navigator.CurrentChannel ?? PluginMain.Navigator.GetPreviousChannel(channeltype));
        }

        public static GuideProgram GetNextprogram(ChannelType channeltype)
        {
            return getNextProgram(PluginMain.Navigator.CurrentChannel ?? PluginMain.Navigator.GetPreviousChannel(channeltype));
        }

        public static GuideProgram GetCurrentForChannel(Channel channel)
        {
            return getCurrentProgram(channel);
        }

        public static GuideProgram GetNextForChannel(Channel channel)
        {
            return getNextProgram(channel);
        }

        public static GuideProgram GetZapProgramAt(DateTime time)
        {
            return getProgramAt(PluginMain.Navigator.ZapChannel, time);
        }

        public static GuideProgram GetProgramAt(DateTime time)
        {
            return getProgramAt(PluginMain.Navigator.CurrentChannel, time);
        }

        public static GuideProgram GetProgramAt(Channel channel, DateTime time)
        {
            return getProgramAt(channel, time);
        }
 
        private static GuideProgram getCurrentProgram(Channel channel)
        {
            lock (_currlock)
            {
                if (channel != null)
                {
                    if (channel.GuideChannelId.HasValue)
                    {
                        if (channel.ChannelId != PrevchannelId)
                        {
                            RefreshCurrentAndNext(channel, true);
                            PrevchannelId = channel.ChannelId;
                        }

                        if (_currentProgram != null)
                        {
                            if (_currentProgram.StartTime < DateTime.Now && _currentProgram.StopTime >= DateTime.Now
                                && _currentProgram.GuideChannelId == channel.GuideChannelId)
                            {
                                return _currentProgram;
                            }
                        }

                        RefreshCurrentAndNext(channel, false);
                        if (_currentProgram != null)
                        {
                            return _currentProgram;
                        }
                    }
                }
                return null;
            }
        }

        private static GuideProgram getNextProgram(Channel channel)
        {
            lock (_nextlock)
            {
                if (channel != null)
                {
                    if (channel.GuideChannelId.HasValue)
                    {
                        if (channel.ChannelId != PrevchannelId)
                        {
                            RefreshCurrentAndNext(channel, true);
                            PrevchannelId = channel.ChannelId;
                        }

                        if (_nextProgram != null)
                        {
                            if (_nextProgram.StartTime > DateTime.Now
                                && _nextProgram.GuideChannelId == channel.GuideChannelId)
                            {
                                return _nextProgram;
                            }
                        }

                        RefreshCurrentAndNext(channel, false);
                        if (_nextProgram != null)
                        {
                            return _nextProgram;
                        }
                    }
                }
                return null;
            }
        }

        private static object _refreshCurrAndNextLock = new object();
        private static void RefreshCurrentAndNext(Channel channel, bool forceUpdate)
        {
            TimeSpan ts = DateTime.Now - _programUpdateTimer;
            if (ts.TotalMilliseconds < 1000 && !forceUpdate)
            {
                return;
            }
            _programUpdateTimer = DateTime.Now;

            lock (_refreshCurrAndNextLock)
            {
                var guideProxy = new GuideServiceProxy();
                var schedulerProxy = new SchedulerServiceProxy();

                CurrentAndNextProgram currentAndNext = schedulerProxy.GetCurrentAndNextForChannel(channel.ChannelId, false, null);
                if (currentAndNext != null)
                {
                    if (currentAndNext.Current != null)
                    {
                        _currentProgram = guideProxy.GetProgramById(currentAndNext.Current.GuideProgramId);
                    }
                    else
                    {
                        _currentProgram = null;
                    }
                    if (currentAndNext.Next != null)
                    {
                        _nextProgram = guideProxy.GetProgramById(currentAndNext.Next.GuideProgramId);
                    }
                    else
                    {
                        _nextProgram = null;
                    }
                }
                else
                {
                    _nextProgram = null;
                    _currentProgram = null;
                }
            }
        }

        private static GuideProgram getProgramAt(Channel channel, DateTime time)
        {
            if (channel != null
                && channel.GuideChannelId.HasValue)
            {
                var guideProxy = new GuideServiceProxy();

                var programs = guideProxy.GetChannelProgramsBetween(channel.GuideChannelId.Value, time, time);
                if (programs.Count > 0)
                {
                    return guideProxy.GetProgramById(programs[0].GuideProgramId);
                }
            }
            return null;
        }

        #endregion

        #region Connected?

        /// <summary>
        /// Returns true if we are connected to the core service
        /// </summary>
        /// <returns></returns>
        internal static bool IsConnected()
        {
            if (ForceNoConnection)
            {
                return false;
            }
            if (!NetworkAvailable && !IsSingleSeat)
            {
                return false;
            }
            return _connected;
        }

        /// <summary>
        /// Try to connect to the core service, if not connected.
        /// </summary>
        /// <returns></returns>
        internal static bool EnsureConnection()
        {
            return EnsureConnection(true,true);
        }

        /// <summary>
        /// Try to connect to the core service, if not connected.
        /// </summary>
        /// <param name="showHomeOnError"></param>
        /// <param name="schowError"></param>
        /// <returns></returns>
        internal static bool EnsureConnection(bool showHomeOnError, bool schowError)
        {
            if (!ProxyFactory.IsInitialized)
            {
                bool succeeded = false;
                string errorMessage = string.Empty;
                ServerSettings serverSettings = LoadServerSettings();

                if (IsSingleSeat)
                {
                    StartServices(serverSettings.ServerName);
                }
                /*else
                {
                    if (!NetworkAvailable)
                    {
                        if (((showHomeOnError || !_connectionErrorShown)
                            && GUIWindowManager.ActiveWindow != 0) && schowError)
                        {
                            GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                            dlg.Reset();
                            dlg.SetHeading(Utility.GetLocalizedText(TextId.Error));
                            dlg.SetLine(1, "Failed to connect to ARGUS TV");
                            dlg.SetLine(2, "No internet connection!");
                            dlg.SetLine(3, "");
                            dlg.DoModal(GUIWindowManager.ActiveWindow);
                            _connectionErrorShown = true;
                        }

                        _connected = false;
                        return false;
                    }
                }*/

                succeeded = Utility.InitialiseServerSettings(serverSettings , out errorMessage);
                if (!succeeded && !IsSingleSeat)
                {
                    StartServices(serverSettings.ServerName);
                    succeeded = Utility.InitialiseServerSettings(serverSettings, out errorMessage);
                }

                if (!succeeded)
                {
                    if (((showHomeOnError || !_connectionErrorShown)
                        && GUIWindowManager.ActiveWindow != 0) && schowError)
                    {
                        GUIDialogOK dlg = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
                        dlg.Reset();
                        dlg.SetHeading("Failed to connect to ARGUS TV");
                        dlg.SetLine(1, serverSettings.ServiceUrlPrefix);
                        dlg.SetLine(2, errorMessage);
                        dlg.SetLine(3, "Check plugin setup in Configuration");
                        dlg.DoModal(GUIWindowManager.ActiveWindow);
                        _connectionErrorShown = true;
                    }
                }
            }

            if (!ProxyFactory.IsInitialized)
            {
                if (showHomeOnError && GUIWindowManager.ActiveWindow != 0)
                {
                    using (Settings xmlreader = new MPSettings())
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
                    }
                }
                _connected = false;
                return false;
            }
            
            _connected = true;
            return true;
        }

        private static void StartServices(string serverName)
        {
            Utility.StartService(_coreService,serverName,60);
            Utility.StartService(_argusTvService, serverName,30);
            Utility.StartService(_mpTvService, serverName,30);
        }

        #endregion

        #region Settings

        internal static bool NoClientStandbyWhenNotHome
        {
            get { return GetCachedBooleanSetting(TvHome.SettingName.NoClientStandbyWhenNotHome, true); }
        }

        internal static bool AutoStreamingMode
        {
            get { return GetCachedBooleanSetting(TvHome.SettingName.AutoStreamingMode, true); }
        }

        internal static bool PreferRtspForLiveTv
        {
            //If AutoStreamingMode, RTSP for multi-seat and UNC for single-seat.
            get { return AutoStreamingMode ? !IsSingleSeat : !GetCachedBooleanSetting(TvHome.SettingName.AvoidRtspForLiveTv, true);}
        }

        internal static bool PlayRecordingsOverRtsp
        {
            //If AutoStreamingMode, UNC for multi and single-seat.
            get { return AutoStreamingMode ? false : GetCachedBooleanSetting(TvHome.SettingName.PlayRecordingsOverRtsp, false);}
        }

        internal static bool IsSingleSeat
        {
            get { return GetCachedBooleanSetting(TvHome.SettingName.IsSingleSeat, true); }  
        }

        internal static bool DisableRadio
        {
            get { return GetCachedBooleanSetting(TvHome.SettingName.DisableRadio, false); }
        }

        private static Dictionary<string, bool> _booleanSettings = new Dictionary<string, bool>();
        private static bool GetCachedBooleanSetting(string settingName, bool defaultValue)
        {
            if (!_booleanSettings.ContainsKey(settingName))
            {
                using (Settings xmlreader = new MPSettings())
                {
                    _booleanSettings[settingName] = xmlreader.GetValueAsBool(_settingSection, settingName, defaultValue);
                }
            }
            return _booleanSettings[settingName];
        }

        internal static void ClearCachedBooleanSettings()
        {
            _booleanSettings.Clear();
        }

        internal static ServerSettings LoadServerSettings()
        {
            using (Settings xmlreader = new MPSettings())
            {
                ServerSettings serverSettings = new ServerSettings();
                serverSettings.ServerName = xmlreader.GetValueAsString(_settingSection, TvHome.SettingName.Server, "localhost");
#if DEBUG
                if (!Dns.GetHostName().Equals("tcf") && !Dns.GetHostName().Equals("Iznajar"))
                {
                    serverSettings.ServerName = "mediaserver";
                }
#endif
                serverSettings.Port = xmlreader.GetValueAsInt(_settingSection, TvHome.SettingName.TcpPort, ServerSettings.DefaultHttpPort);
                if (serverSettings.Port == 49942)
                {
                    // Auto-adjust old net.tcp port to HTTP.
                    serverSettings.Port = 49943;
                }
                serverSettings.WakeOnLan.MacAddresses = xmlreader.GetValueAsString(_settingSection, TvHome.SettingName.MacAddresses, String.Empty);
                serverSettings.WakeOnLan.IPAddress = xmlreader.GetValueAsString(_settingSection, TvHome.SettingName.IPAddress, String.Empty);
                serverSettings.WakeOnLan.Enabled = xmlreader.GetValueAsBool(_settingSection, TvHome.SettingName.UseWakeOnLan, false);
                serverSettings.WakeOnLan.TimeoutSeconds = xmlreader.GetValueAsInt(_settingSection, TvHome.SettingName.WakeOnLanTimeoutSeconds, 10);
                return serverSettings;
            }
        }

        #endregion
    }
}
