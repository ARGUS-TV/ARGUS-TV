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
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;

using TvControl;
using TvEngine;
using TvEngine.Events;
using TvEngine.Interfaces;
using TvLibrary.Epg;
using TvLibrary.Channels;
using TvLibrary.Interfaces;
using TvLibrary.Implementations;
using TvLibrary.Log;
using SetupTv;
using Nancy.Hosting.Self;

using ArgusTV.ServiceAgents;
using ArgusTV.DataContracts;
using ArgusTV.Recorder.MediaPortalTvServer.Channels;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    public class TvServerPlugin : ITvServerPlugin
    {
        private const string _defaultServerName = "localhost";
        private const int _defaultPort = ServerSettings.DefaultTcpPort;
        private const string _defaultRecordingsRootUncPath = @"\\MACHINENAME\Recordings\";

        public const int DefaultRecorderTunerTcpPort = 49842;
        public const int DefaultSyncAllHours = 24;

        private ServerSettings _serverSettings = new ServerSettings();
        private int _recorderTunerTcpPort;
        private bool _restartTvServerOnResume;
        private bool _epgSyncOn;
        private bool _epgSyncAutoCreateChannels;
        private bool _epgSyncAutoCreateChannelsWithGroup;
        private int _epgSyncAllHours;

        #region Properties

        /// <summary>
        /// returns the name of the plugin
        /// </summary>
        public string Name
        {
            get { return "ARGUS TV Recorder"; }
        }

        /// <summary>
        /// returns the version of the plugin
        /// </summary>
        public string Version
        {
            get { return ArgusTV.DataContracts.Constants.AssemblyVersion; }
        }

        /// <summary>
        /// returns the author of the plugin
        /// </summary>
        public string Author
        {
            get { return "www.argus-tv.com"; }
        }

        /// <summary>
        /// returns if the plugin should only run on the master server
        /// or also on slave servers
        /// </summary>
        public bool MasterOnly
        {
            get { return true; }
        }

        public ServerSettings ServerSettings
        {
            get { return _serverSettings; }
        }

        public bool RestartTvServerOnResume
        {
            get { return _restartTvServerOnResume; }
            set { _restartTvServerOnResume = value; }
        }

        public bool EpgSyncOn
        {
            get { return _epgSyncOn; }
            set { _epgSyncOn = value; }
        }

        public bool EpgSyncAutoCreateChannels
        {
            get { return _epgSyncAutoCreateChannels; }
            set { _epgSyncAutoCreateChannels = value; }
        }

        public bool EpgSyncAutoCreateChannelsWithGroup
        {
            get { return _epgSyncAutoCreateChannelsWithGroup; }
            set { _epgSyncAutoCreateChannelsWithGroup = value; }
        }

        public int EpgSyncAllHours
        {
            get { return _epgSyncAllHours; }
            set { _epgSyncAllHours = value; }
        }

        public int RecorderTunerTcpPort
        {
            get { return _recorderTunerTcpPort; }
            set { _recorderTunerTcpPort = value; }
        }

        public bool IsArgusTVConnectionInitialized
        {
            get { return ServiceChannelFactories.IsInitialized; }
        }

        #endregion Properties

        #region IPlugin Methods

        private static IController _controller;
        private NancyHost _recorderRestHost;
        private DvbEpgThread _dvbEpgThread;
        private PowerEventHandler _powerEventHandler;

        public void Start(IController controller)
        {
            Log.Info("ArgusTV.Recorder.MediaPortalTvServer: Start");

            _controller = controller;
            LoadSettings();

            if (GlobalServiceProvider.Instance.IsRegistered<IPowerEventHandler>())
            {
                _powerEventHandler = new PowerEventHandler(OnPowerEvent);
                GlobalServiceProvider.Instance.Get<IPowerEventHandler>().AddPowerEventHandler(_powerEventHandler);
                Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: Registered OnPowerEvent with TV Server");
            }
            else
            {
                Log.Error("ArgusTV.Recorder.MediaPortalTvServer: Failed to register OnPowerEvent with TV Server!");
            }

            GlobalServiceProvider.Instance.Get<ITvServerEvent>().OnTvServerEvent += events_OnTvServerEvent;
            Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: Registered OnTvServerEvent with TV Server");

            HostConfiguration configuration = new HostConfiguration()
            {
                AllowChunkedEncoding = true,
                EnableClientCertificates = false
            };
            string recorderUrl = String.Format("http://localhost:{0}/ArgusTV/", _recorderTunerTcpPort);
            _recorderRestHost = new NancyHost(configuration, new Uri(recorderUrl));
            _recorderRestHost.Start();
            Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: Listening on " + recorderUrl + "Recorder/");

            _dvbEpgThread = new DvbEpgThread();
            _dvbEpgThread.Start();
        }

        public void Stop()
        {
            Log.Info("ArgusTV.Recorder.MediaPortalTvServer: Stop");

            if (GlobalServiceProvider.Instance.IsRegistered<ITvServerEvent>())
            {
                GlobalServiceProvider.Instance.Get<ITvServerEvent>().OnTvServerEvent -= events_OnTvServerEvent;
            }

            if (GlobalServiceProvider.Instance.IsRegistered<IPowerEventHandler>())
            {
                GlobalServiceProvider.Instance.Get<IPowerEventHandler>().RemovePowerEventHandler(_powerEventHandler);
                Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: Unregistered OnPowerEvent with TV Server");
            }

            if (_dvbEpgThread != null)
            {
                _dvbEpgThread.Stop(true);
                _dvbEpgThread = null;
            }
            if (_recorderRestHost != null)
            {
                _recorderRestHost.Stop();
                TvServerRecorderModule.DisposeModule();
                _recorderRestHost.Dispose();
                _recorderRestHost = null;
            }
        }

        public SetupTv.SectionSettings Setup
        {
            get
            {
                SetupForm setupForm = new SetupForm();
                setupForm.Plugin = this;
                return setupForm;
            }
        }

        #endregion

        #region Implementation

        private void events_OnTvServerEvent(object sender, EventArgs eventArgs)
        {
            TvServerEventArgs args = eventArgs as TvServerEventArgs;
            if (args != null
                && args.EventType == TvServerEventType.ImportEpgPrograms
                && args.EpgChannel != null
                && args.EpgChannel.Programs.Count > 0)
            {
                try
                {
                    ImportEpgPrograms(args.EpgChannel);
                }
                catch (Exception ex)
                {
                    Log.Error("ArgusTV.Recorder.MediaPortalTvServer: ImportEpgPrograms(): {0}", ex.Message);
                }
            }
        }

        private bool OnPowerEvent(PowerEventType powerStatus)
        {
            lock (this)
            {
                switch (powerStatus)
                {
                    case PowerEventType.QuerySuspend:
                    case PowerEventType.QueryStandBy:
                        return true;

                    case PowerEventType.Suspend:
                    case PowerEventType.StandBy:
                        Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: System is entering standby");
                        _tvServerNeedsRestart = true;
                        return true;

                    case PowerEventType.QuerySuspendFailed:
                    case PowerEventType.QueryStandByFailed:
                        Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: Entering standby was denied");
                        return true;

                    case PowerEventType.ResumeAutomatic:
                        Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: System has resumed automatically from standby");
                        Resume();
                        return true;

                    case PowerEventType.ResumeCritical:
                        Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: System has resumed from standby after a critical suspend");
                        Resume();
                        return true;

                    case PowerEventType.ResumeSuspend:
                        Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: System has resumed from standby");
                        Resume();
                        return true;
                }
                return true;
            }
        }

        private bool _tvServerNeedsRestart;

        private void Resume()
        {
            if (_tvServerNeedsRestart
                && _restartTvServerOnResume)
            {
                TvServerPlugin.TvController_Restart();
            }
            _tvServerNeedsRestart = false;
        }

        internal bool InitializeArgusTVConnection(IWin32Window settingsPanel)
        {
            try
            {
                ServiceChannelFactories.Initialize(_serverSettings, true);
            }
            catch (Exception ex)
            {
                if (settingsPanel != null)
                {
                    MessageBox.Show(settingsPanel, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Log.Error("ArgusTV.Recorder.MediaPortalTvServer: InitializeArgusTVConnection(): {0}", ex.Message);
                }
                return false;
            }
            return true;
        }

        internal void LoadSettings()
        {
            try
            {
                TvDatabase.TvBusinessLayer layer = new TvDatabase.TvBusinessLayer();
                _serverSettings.ServerName = layer.GetSetting(SettingName.ServerName, _defaultServerName).Value;
                _serverSettings.Transport = ServiceTransport.NetTcp;
                _serverSettings.Port = Convert.ToInt32(layer.GetSetting(SettingName.Port, _defaultPort.ToString()).Value);
                _restartTvServerOnResume = Convert.ToBoolean(layer.GetSetting(SettingName.ResetTvServerOnResume, false.ToString()).Value);
                _epgSyncOn = Convert.ToBoolean(layer.GetSetting(SettingName.EpgSyncOn, false.ToString()).Value);
                _epgSyncAutoCreateChannels = Convert.ToBoolean(layer.GetSetting(SettingName.EpgSyncAutoCreateChannels, false.ToString()).Value);
                _epgSyncAutoCreateChannelsWithGroup = Convert.ToBoolean(layer.GetSetting(SettingName.EpgSyncAutoCreateChannelsWithGroup, false.ToString()).Value);
                _epgSyncAllHours = Convert.ToInt32(layer.GetSetting(SettingName.EpgSyncAllHours, DefaultSyncAllHours.ToString()).Value);
                _recorderTunerTcpPort = Convert.ToInt32(layer.GetSetting(SettingName.RecorderTunerTcpPort, DefaultRecorderTunerTcpPort.ToString()).Value);
            }
            catch (Exception ex)
            {
                _serverSettings.ServerName = _defaultServerName;
                _serverSettings.Transport = ServiceTransport.NetTcp;
                _serverSettings.Port = _defaultPort;

                Log.Error("ArgusTV.Recorder.MediaPortalTvServer: LoadSettings(): {0}", ex.Message);
            }
        }

        internal void SaveSettings()
        {
            try
            {
                TvDatabase.TvBusinessLayer layer = new TvDatabase.TvBusinessLayer();
                TvDatabase.Setting setting;

                setting = layer.GetSetting(SettingName.ServerName);
                setting.Value = _serverSettings.ServerName;
                setting.Persist();

                setting = layer.GetSetting(SettingName.Port);
                setting.Value = _serverSettings.Port.ToString();
                setting.Persist();

                setting = layer.GetSetting(SettingName.ResetTvServerOnResume);
                setting.Value = _restartTvServerOnResume.ToString();
                setting.Persist();

                setting = layer.GetSetting(SettingName.EpgSyncOn);
                setting.Value = _epgSyncOn.ToString();
                setting.Persist();

                setting = layer.GetSetting(SettingName.EpgSyncAutoCreateChannels);
                setting.Value = _epgSyncAutoCreateChannels.ToString();
                setting.Persist();

                setting = layer.GetSetting(SettingName.EpgSyncAutoCreateChannelsWithGroup);
                setting.Value = _epgSyncAutoCreateChannelsWithGroup.ToString();
                setting.Persist();

                setting = layer.GetSetting(SettingName.EpgSyncAllHours);
                setting.Value = _epgSyncAllHours.ToString();
                setting.Persist();

                setting = layer.GetSetting(SettingName.RecorderTunerTcpPort);
                setting.Value = _recorderTunerTcpPort.ToString();
                setting.Persist();
            }
            catch (Exception ex)
            {
                Log.Error("ArgusTV.Recorder.MediaPortalTvServer: SaveSettings(): {0}", ex.Message);
            }
        }

        #endregion

        #region DVB-EPG

        private void ImportEpgPrograms(EpgChannel epgChannel)
        {
            if (!this.IsArgusTVConnectionInitialized)
            {
                InitializeArgusTVConnection(null);
            }
            if (this.IsArgusTVConnectionInitialized)
            {
                TvDatabase.TvBusinessLayer layer = new TvDatabase.TvBusinessLayer();
                bool epgSyncOn = Convert.ToBoolean(layer.GetSetting(SettingName.EpgSyncOn, false.ToString()).Value);
                if (epgSyncOn)
                {
                    DVBBaseChannel dvbChannel = epgChannel.Channel as DVBBaseChannel;
                    if (dvbChannel != null)
                    {
                        TvDatabase.Channel mpChannel = layer.GetChannelByTuningDetail(dvbChannel.NetworkId, dvbChannel.TransportId, dvbChannel.ServiceId);
                        if (mpChannel != null)
                        {
                            Log.Debug("ArgusTV.Recorder.MediaPortalTvServer: ImportEpgPrograms(): received {0} programs on {1}", epgChannel.Programs.Count, mpChannel.DisplayName);

                            using (CoreServiceAgent coreAgent = new CoreServiceAgent())
                            using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                            using (GuideServiceAgent tvGuideAgent = new GuideServiceAgent())
                            {
                                bool epgSyncAutoCreateChannels = Convert.ToBoolean(layer.GetSetting(SettingName.EpgSyncAutoCreateChannels, false.ToString()).Value);
                                bool epgSyncAutoCreateChannelsWithGroup = Convert.ToBoolean(layer.GetSetting(SettingName.EpgSyncAutoCreateChannelsWithGroup, false.ToString()).Value);
                                string epgLanguages = layer.GetSetting("epgLanguages").Value;

                                Channel channel = EnsureChannelForDvbEpg(tvSchedulerAgent, mpChannel, epgSyncAutoCreateChannels, epgSyncAutoCreateChannelsWithGroup);
                                if (channel != null)
                                {
                                    EnsureGuideChannelForDvbEpg(tvSchedulerAgent, tvGuideAgent, channel, mpChannel);

                                    List<GuideProgram> guidePrograms = new List<GuideProgram>();

                                    foreach (EpgProgram epgProgram in epgChannel.Programs)
                                    {
                                        string title;
                                        string description;
                                        string genre;
                                        int starRating;
                                        string classification;
                                        int parentalRating;
                                        GetProgramInfoForLanguage(epgProgram.Text, epgLanguages, out title, out description, out genre,
                                            out starRating, out classification, out parentalRating);

                                        if (!String.IsNullOrEmpty(title))
                                        {
                                            GuideProgram guideProgram = new GuideProgram();
                                            guideProgram.GuideChannelId = channel.GuideChannelId.Value;
                                            guideProgram.StartTime = epgProgram.StartTime;
                                            guideProgram.StopTime = epgProgram.EndTime;
                                            guideProgram.StartTimeUtc = epgProgram.StartTime.ToUniversalTime();
                                            guideProgram.StopTime = epgProgram.EndTime;
                                            guideProgram.StopTimeUtc = epgProgram.EndTime.ToUniversalTime();
                                            guideProgram.Title = title;
                                            guideProgram.Description = description;
                                            guideProgram.Category = genre;
                                            guideProgram.Rating = classification;
                                            guideProgram.StarRating = starRating / 7.0;
                                            guidePrograms.Add(guideProgram);
                                        }
                                    }

                                    _dvbEpgThread.ImportProgramsAsync(guidePrograms);
                                }
                                else
                                {
                                    Log.Info("ArgusTV.Recorder.MediaPortalTvServer: ImportEpgPrograms() failed to ensure channel.");
                                }
                            }
                        }
                        else
                        {
                            Log.Info("ArgusTV.Recorder.MediaPortalTvServer: ImportEpgPrograms() failed to find MP channel.");
                        }
                    }
                }
            }
        }

        private static void EnsureGuideChannelForDvbEpg(SchedulerServiceAgent tvSchedulerAgent, GuideServiceAgent tvGuideAgent, Channel channel, TvDatabase.Channel mpChannel)
        {
            if (!channel.GuideChannelId.HasValue)
            {
                string externalId = mpChannel.ExternalId;
                if (String.IsNullOrEmpty(externalId))
                {
                    externalId = mpChannel.DisplayName;
                }
                channel.GuideChannelId = tvGuideAgent.EnsureChannel(externalId, mpChannel.DisplayName, channel.ChannelType);
                tvSchedulerAgent.AttachChannelToGuide(channel.ChannelId, channel.GuideChannelId.Value);
            }
        }

        private static Channel EnsureChannelForDvbEpg(SchedulerServiceAgent tvSchedulerAgent, TvDatabase.Channel mpChannel,
            bool epgSyncAutoCreateChannels, bool epgSyncAutoCreateChannelsWithGroup)
        {
            ChannelLink channelLink = ChannelLinks.GetChannelLinkForMediaPortalChannel(mpChannel);
            ChannelType channelType = mpChannel.IsTv ? ChannelType.Television : ChannelType.Radio;
            Channel channel = null;
            if (channelLink != null)
            {
                channel = tvSchedulerAgent.GetChannelById(channelLink.ChannelId);
                if (channel == null)
                {
                    channel = tvSchedulerAgent.GetChannelByDisplayName(channelType, channelLink.ChannelName);
                }
            }
            if (channel == null)
            {
                channel = tvSchedulerAgent.GetChannelByDisplayName(channelType, mpChannel.DisplayName);
            }
            if (channel == null
                && (epgSyncAutoCreateChannels || epgSyncAutoCreateChannelsWithGroup))
            {
                string groupName = "DVB-EPG";
                if (epgSyncAutoCreateChannelsWithGroup)
                {
                    IList<TvDatabase.GroupMap> groupMaps = mpChannel.ReferringGroupMap();
                    foreach (TvDatabase.GroupMap groupMap in groupMaps)
                    {
                        TvDatabase.ChannelGroup channelGroup = TvDatabase.ChannelGroup.Retrieve(groupMap.IdGroup);
                        if (channelGroup != null)
                        {
                            groupName = channelGroup.GroupName;
                            break;
                        }
                    }
                }

                Guid channelId = tvSchedulerAgent.EnsureChannel(channelType, mpChannel.DisplayName, groupName);
                channel = tvSchedulerAgent.GetChannelById(channelId);

                if (!channel.LogicalChannelNumber.HasValue
                    && mpChannel.ChannelNumber > 0)
                {
                    channel.LogicalChannelNumber = mpChannel.ChannelNumber;
                    tvSchedulerAgent.SaveChannel(channel);
                }
            }
            return channel;
        }

        private void GetProgramInfoForLanguage(IList<EpgLanguageText> texts, string epgLanguages,
            out string title, out string description, out string genre, out int starRating, out string classification, out int parentalRating)
        {
            title = String.Empty;
            description = String.Empty;
            genre = String.Empty;
            starRating = -1;
            classification = String.Empty;
            parentalRating = -1;

            if (texts.Count > 0)
            {
                int offset = 0;
                for (int i = 0; i < texts.Count; ++i)
                {
                    if (texts[0].Language.Equals("all", StringComparison.InvariantCultureIgnoreCase))
                    {
                        offset = i;
                        break;
                    }
                    if (epgLanguages.Length == 0 ||
                        epgLanguages.IndexOf(texts[i].Language, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        offset = i;
                        break;
                    }
                }
                title = GetEpgString(texts[offset].Title);
                description = GetEpgString(texts[offset].Description);
                genre = GetEpgString(texts[offset].Genre);
                classification = GetEpgString(texts[offset].Classification);
                starRating = texts[offset].StarRating;
                parentalRating = texts[offset].ParentalRating;
            }
        }

        private string GetEpgString(string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                return text;
            }
            return String.Empty;
        }

        #endregion

        #region TV Controller

        private static object _tvControllerLock = new object();

        public static TvResult TvController_Tune(ref IUser user, IChannel channel, int idChannel)
        {
            lock (_tvControllerLock)
            {
                return _controller.Tune(ref user, channel, idChannel);
            }
        }

        public static CardType TvController_Type(int cardId)
        {
            lock (_tvControllerLock)
            {
                return _controller.Type(cardId);
            }
        }

        public static bool TvController_CanTune(int cardId, IChannel channel)
        {
            lock (_tvControllerLock)
            {
                return _controller.CanTune(cardId, channel);
            }
        }

        public static bool TvController_IsGrabbingEpg(int cardId)
        {
            lock (_tvControllerLock)
            {
                return _controller.IsGrabbingEpg(cardId);
            }
        }

        public static IUser[] TvController_GetUsersForCard(int cardId)
        {
            lock (_tvControllerLock)
            {
                return _controller.GetUsersForCard(cardId);
            }
        }

        public static TvResult TvController_StartRecording(ref IUser user, ref string fileName)
        {
            lock (_tvControllerLock)
            {
                return _controller.StartRecording(ref user, ref fileName);
            }
        }

        public static bool TvController_IsRecording(ref IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.IsRecording(ref user);
            }
        }

        public static bool TvController_StopRecording(ref IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.StopRecording(ref user);
            }
        }

        public static TvResult TvController_StartTimeShifting(ref IUser user, ref string fileName)
        {
            lock (_tvControllerLock)
            {
                return _controller.StartTimeShifting(ref user, ref fileName);
            }
        }

        public static bool TvController_IsTimeShifting(ref IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.IsTimeShifting(ref user);
            }
        }

        public static bool TvController_StopTimeShifting(ref IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.StopTimeShifting(ref user);
            }
        }

        public static bool TvController_StopTimeShifting(ref IUser user, TvStoppedReason reason)
        {
            lock (_tvControllerLock)
            {
                return _controller.StopTimeShifting(ref user, reason);
            }
        }

        public static string TvController_TimeShiftFileName(ref IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.TimeShiftFileName(ref user);
            }
        }

        public static string TvController_GetStreamingUrl(IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.GetStreamingUrl(user);
            }
        }

        public static IChannel TvController_CurrentChannel(IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.CurrentChannel(ref user);
            }
        }

        public static void TvController_HeartBeat(IUser user)
        {
            lock (_tvControllerLock)
            {
                _controller.HeartBeat(user);
            }
        }

        public static void TvController_Restart()
        {
            lock (_tvControllerLock)
            {
                _controller.Restart();
            }
        }

        public static bool TvController_HasTeletext(IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.HasTeletext(user);
            }
        }

        public static void TvController_StartGrabbingTeletext(IUser user)
        {
            lock (_tvControllerLock)
            {
                _controller.GrabTeletext(user, true);
            }
        }

        public static void TvController_StopGrabbingTeletext(IUser user)
        {
            lock (_tvControllerLock)
            {
                _controller.GrabTeletext(user, false);
            }
        }

        public static bool TvController_IsGrabbingTeletext(IUser user)
        {
            lock (_tvControllerLock)
            {
                return _controller.IsGrabbingTeletext(user);
            }
        }

        public static byte[] TvController_GetTeletextPage(IUser user, int pageNumber, int subPageNumber)
        {
            lock (_tvControllerLock)
            {
                return _controller.GetTeletextPage(user, pageNumber, subPageNumber);
            }
        }

        public static int TvController_SubPageCount(IUser user, int pageNumber)
        {
            lock (_tvControllerLock)
            {
                return _controller.SubPageCount(user, pageNumber);
            }
        }

        public static int TvController_SignalLevel(int cardId)
        {
            lock (_tvControllerLock)
            {
                return _controller.SignalLevel(cardId);
            }
        }

        public static int TvController_SignalQuality(int cardId)
        {
            lock (_tvControllerLock)
            {
                return _controller.SignalQuality(cardId);
            }
        }

        #endregion
    }
}
