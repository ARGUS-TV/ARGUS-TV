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
using System.Text;
using System.Threading;

using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Dialogs;
using MediaPortal.Profile;

using ArgusTV.DataContracts;
using ArgusTV.DataContracts.Tuning;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.MediaPortal
{
    internal class ChannelNavigator : IDisposable
    {
        private Dictionary<ChannelType, NavigatorChannels> _navigatorChannels = new Dictionary<ChannelType, NavigatorChannels>();
        private class NavigatorChannels
        {
            public NavigatorChannels()
            {
                this.Channels = new List<Channel>();
                this.Groups = new List<ChannelGroup>();
                this.ChannelsByNumber = new Dictionary<int, Channel>();
                this.GroupsByChannelNumber = new Dictionary<int, ChannelGroup>();
            }

            public List<ChannelGroup> Groups { set; get; }
            public List<Channel> Channels { set; get; }
            public Dictionary<int, Channel> ChannelsByNumber { set; get; }
            public Dictionary<int, ChannelGroup> GroupsByChannelNumber { set; get; }
            public Channel PreviousChannel { set; get; }
            public ChannelGroup PreviousChannelGroup { set; get; }
            public Channel LastChannel { set; get; }
            public ChannelGroup LastChannelGroup { set; get; }
        }

        private static LiveStreamResult result = LiveStreamResult.UnknownError;
        private ManualResetEvent _waitForBlackScreenEvent;
        private ChannelGroup _currentChannelGroup;
        private Channel _currentChannel;
        private ChannelGroup _zapGroup;
        private Channel _zapChannel;
        private LiveStream _liveStream = null;
        private LiveStream _streamToStopAsync = null;
        private DateTime _zapTime;
        private long _zapDelayMs;
        private int _framesBeforeStopRenderBlackImage;
        private int _zapChannelNr = -1;
        private bool _autoFullScreen;
        private bool _isAnalog = false;
        private bool _doingChannelChange = false;
        private bool _lastChannelChangeFailed = false;
        private bool _reentrant = false;

        public ChannelNavigator()
        {
            _navigatorChannels[ChannelType.Radio] = new NavigatorChannels();
            RefreshChannelGroups(ChannelType.Radio);
            _navigatorChannels[ChannelType.Television] = new NavigatorChannels();
            RefreshChannelGroups(ChannelType.Television);
            _waitForBlackScreenEvent = new ManualResetEvent(false);
            GUIGraphicsContext.OnBlackImageRendered += GUIGraphicsContext_OnBlackImageRendered;
            GUIGraphicsContext.OnVideoReceived += GUIGraphicsContext_OnVideoReceived;
        }
        
        #region Black Image

        private void RenderBlackImage()
        {
            if (!GUIGraphicsContext.RenderBlackImage)
            {
                Log.Debug("ChannelNavigator.RenderBlackImage()");
                _waitForBlackScreenEvent.Reset();
                _framesBeforeStopRenderBlackImage = 0;
                GUIGraphicsContext.RenderBlackImage = true;
                _waitForBlackScreenEvent.WaitOne(1000, false);
            }
        }

        private void StopRenderBlackImage()
        {
            if (GUIGraphicsContext.RenderBlackImage)
            {
                GUIGraphicsContext.RenderBlackImage = false;
                //   _framesBeforeStopRenderBlackImage = 3;
                // Ambass : we need to wait the 3rd frame to avoid persistance of previous channel....Why ?????
                // Morpheus: number of frames depends on hardware, from 1..5 or higher might be needed! 
                //           Probably the faster the graphics card is, the more frames required???
            }
        }

        private void GUIGraphicsContext_OnBlackImageRendered()
        {
            if (GUIGraphicsContext.RenderBlackImage)
            {
                _waitForBlackScreenEvent.Set();
            }
        }

        private void GUIGraphicsContext_OnVideoReceived()
        {
            if (GUIGraphicsContext.RenderBlackImage)
            {
                if (_framesBeforeStopRenderBlackImage > 0)
                {
                    Log.Debug("ChannelNavigator.OnVideoReceived() {0}", _framesBeforeStopRenderBlackImage);
                    if (--_framesBeforeStopRenderBlackImage == 0)
                    {
                        GUIGraphicsContext.RenderBlackImage = false;
                        Log.Debug("ChannelNavigator.StopRenderBlackImage()");
                    }
                }
            }
        }

        #endregion

        #region Service Proxies

        private ControlServiceProxy _controlServiceProxy;

        public ControlServiceProxy ControlServiceProxy
        {
            get
            {
                if (_controlServiceProxy == null)
                {
                    _controlServiceProxy = new ControlServiceProxy();
                }
                return _controlServiceProxy;
            }
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the channel that we currently watch.
        /// Returns empty string if there is no current channel.
        /// </summary>
        public string CurrentChannelName
        {
            get { return _currentChannel == null ? String.Empty : _currentChannel.DisplayName; }
        }

        public Channel CurrentChannel
        {
            get { return _currentChannel; }
        }

        public string ZapChannelName
        {
            get { return _zapChannel == null ? String.Empty : _zapChannel.DisplayName; }
        }

        public Channel ZapChannel
        {
            get { return _zapChannel; }
        }

        /// <summary>
        /// Gets the channel number that we will zap to. If not zapping by number or not zapping to anything, returns -1.
        /// </summary>
        public int ZapChannelNr
        {
            get { return _zapChannelNr; }
            set { _zapChannelNr = value; }
        }

        /// <summary>
        /// true if we are changing a channel
        /// </summary>
        public bool DoingChannelChange
        {
            get { return _doingChannelChange; }
        }

        /// <summary>
        /// true if last channel change failed
        /// </summary>
        public bool LastChannelChangeFailed
        {
            get { return _lastChannelChangeFailed; }
            set { _lastChannelChangeFailed = value; }
        }

        /// <summary>
        /// Gets and sets the last viewed channel, null if there is none.
        /// </summary>
        public Channel GetPreviousChannel(ChannelType channelType)
        {
            return _navigatorChannels[channelType].PreviousChannel;
        }

        /// <summary>
        /// Gets the currently active channel group.
        /// </summary>
        public ChannelGroup CurrentGroup
        {
            get { return _currentChannelGroup; }
        }

        /// <summary>
        /// Set the current channelgroup.
        /// </summary>
        /// <param name="channelType">Type of channelgroup.</param>
        /// <param name="currentGroup">Channelgroup to set as current.</param>
        public void SetCurrentGroup(ChannelType channelType, ChannelGroup currentGroup)
        {
            _currentChannelGroup = currentGroup;
            RefreshChannelsInGroup(channelType);
        }

        /// <summary>
        /// Gets the list of channel groups.
        /// </summary>
        public List<ChannelGroup> GetGroups(ChannelType channelType)
        {
            return _navigatorChannels[channelType].Groups;
        }

        public int TvChannelCount
        {
            get { return _navigatorChannels[ChannelType.Television].Channels == null ? 0 : _navigatorChannels[ChannelType.Television].Channels.Count; }
        }

        public bool IsLiveStreamOn
        {
            get { return _liveStream != null; }
        }

        public LiveStream LiveStream
        {
            get { return _liveStream; }
        }

        #endregion

        #region Public methods

        public void Reload()
        {
            Log.Debug("ChannelNavigator: Reload");
            SaveSettings();
            LoadSettings();
            RefreshChannelGroups(ChannelType.Television);
            RefreshChannelGroups(ChannelType.Radio);
        }
        
        public void RefreshChannelGroups(ChannelType channelType)
        {
            RefreshGroups(channelType);
        }

        private void RefreshGroups(ChannelType channelType)
        {
            try
            {
                var schedulerProxy = new SchedulerServiceProxy();

                List<ChannelGroup> groups = schedulerProxy.GetAllChannelGroups(channelType, true);
                if (_currentChannelGroup != null
                    && _currentChannelGroup.ChannelGroupId != ChannelGroup.AllTvChannelsGroupId
                    && _currentChannelGroup.ChannelGroupId != ChannelGroup.AllRadioChannelsGroupId)
                {
                    bool currentFound = false;
                    foreach (ChannelGroup group in groups)
                    {
                        if (group.ChannelGroupId == _currentChannelGroup.ChannelGroupId)
                        {
                            currentFound = true;
                            break;
                        }
                    }
                    if (!currentFound)
                    {
                        _currentChannelGroup = null;
                    }
                }

                bool hideAllChannelsGroup = false;
                using (Settings xmlreader = new MPSettings())
                {
                    hideAllChannelsGroup = xmlreader.GetValueAsBool("mytv", "hideAllChannelsGroup", false);
                }

                if (!hideAllChannelsGroup || groups.Count == 0)
                {
                    groups.Add(new ChannelGroup()
                    {
                        ChannelGroupId = channelType == ChannelType.Television ? ChannelGroup.AllTvChannelsGroupId : ChannelGroup.AllRadioChannelsGroupId,
                        ChannelType = channelType,
                        GroupName = Utility.GetLocalizedText(TextId.AllChannels),
                        Sequence = int.MaxValue,
                        VisibleInGuide = true
                    });
                }

                _navigatorChannels[channelType].Groups = groups;

                if (_currentChannelGroup == null && _navigatorChannels[channelType].Groups.Count > 0)
                {
                    _currentChannelGroup = _navigatorChannels[channelType].Groups[0];
                    RefreshChannelsInGroup(schedulerProxy, channelType);
                }
            }
            catch (Exception ex)
            {
                Log.Error("ChannelNavigator: Error in RefreshChannelGroups - {0}", ex.Message);
            }
        }

        public Channel GetChannelByIndex(ChannelType channelType, int channelIndex)
        {
            if (_navigatorChannels[channelType].Channels != null
                && channelIndex >= 1
                && channelIndex <= _navigatorChannels[channelType].Channels.Count)
            {
                return _navigatorChannels[channelType].Channels[channelIndex - 1];
            }
            return null;
        }

        public Channel GetChannelByNumber(ChannelType channelType, int channelNr)
        {
            ChannelGroup channelGroup;
            return GetChannelByNumber(channelType, channelNr, out channelGroup);
        }

        public Channel GetChannelByNumber(ChannelType channelType, int channelNr, out ChannelGroup channelGroup)
        {
            channelGroup = _currentChannelGroup;
            if (_navigatorChannels[channelType].ChannelsByNumber.ContainsKey(channelNr))
            {
                channelGroup = _navigatorChannels[channelType].GroupsByChannelNumber[channelNr];
                return _navigatorChannels[channelType].ChannelsByNumber[channelNr];
            }
            Channel channel = null;
            if (_navigatorChannels[channelType].Channels != null)
            {
                channel = FindChannelByNumber(_navigatorChannels[channelType].Channels, channelNr);
                if (channel == null)
                {
                    try
                    {
                        var schedulerProxy = new SchedulerServiceProxy();

                        foreach (ChannelGroup group in _navigatorChannels[channelType].Groups)
                        {
                            if (group != _currentChannelGroup)
                            {
                                channel = FindChannelInGroupByNumber(schedulerProxy, group.ChannelGroupId, channelNr);
                                if (channel != null)
                                {
                                    channelGroup = group;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("ChannelNavigator: Error in GetChannelByNumber - {0}", ex.Message);
                    }
                }
            }
            _navigatorChannels[channelType].ChannelsByNumber[channelNr] = channel;
            _navigatorChannels[channelType].GroupsByChannelNumber[channelNr] = channelGroup;
            return channel;
        }

        private Channel FindChannelByNumber(List<Channel> channels, int channelNr)
        {
            foreach (Channel channel in channels)
            {
                if (channel.LogicalChannelNumber.HasValue
                    && channel.LogicalChannelNumber.Value == channelNr)
                {
                    return channel;
                }
            }
            return null;
        }

        private Channel FindChannelInGroupByNumber(SchedulerServiceProxy schedulerProxy, Guid channelGroupId, int channelNr)
        {
            return FindChannelByNumber(new List<Channel>(schedulerProxy.GetChannelsInGroup(channelGroupId, true)), channelNr);
        }

        public void ZapNow()
        {
            _zapTime = DateTime.Now;
        }

        /// <summary>
        /// Checks if it is time to zap to a different channel. This is called during Process().
        /// </summary>
        public bool CheckChannelChange()
        {
            if (_reentrant)
            {
                return false;
            }
            try
            {                
                _reentrant = true;

                // Zapping to another group or channel?               
                if (_zapGroup != null || _zapChannel != null)
                {
                    // Time to zap?
                    if (DateTime.Now >= _zapTime)
                    {
                        Log.Debug("ChannelNavigator: CheckChannelChange()_DateTime.Now >= _zapTime,_zapgroup = {0} , _zapchannel = {1}", _zapGroup.GroupName, _zapChannel.DisplayName);
                        // Zapping to another group?
                        if (_zapGroup != null && _zapGroup != _currentChannelGroup)
                        {
                            // Change current group (and possibly zap to the first channel of the group)
                            if (_currentChannelGroup != null)
                            {
                                _navigatorChannels[_currentChannelGroup.ChannelType].PreviousChannelGroup = _currentChannelGroup;
                            }
                            _currentChannelGroup = _zapGroup;
                            RefreshChannelsInGroup(_zapGroup.ChannelType);
                            if (_zapChannel == null
                                && _navigatorChannels[_zapGroup.ChannelType].Channels.Count > 0)
                            {
                                _zapChannel = _navigatorChannels[_zapGroup.ChannelType].Channels[0];
                            }
                        }
                        
                        // Zap to desired channel
                        Channel zappingTo = _zapChannel;
                        _zapChannel = null;
                        _zapGroup = null;

                        if (PluginMain.Navigator.CurrentChannel != null
                            && PluginMain.Navigator.CurrentChannel.ChannelId == zappingTo.ChannelId
                            && _liveStream != null && !_lastChannelChangeFailed)
                        {
                            Log.Debug("ChannelNavigator: CheckChannelChange()_CurrentChannel.ChannelId = zappingTo.ChannelId --> break off zapping");
                            zappingTo = null;
                        }

                        if (zappingTo != null)
                        {
                            if ((this.IsLiveStreamOn && _currentChannel != null
                                && zappingTo.ChannelType != _currentChannel.ChannelType) 
                                || (g_Player.Playing && !this.IsLiveStreamOn))
                            {
                                //g_Player needs a comlete stop when the ChannelType changes
                                if (_liveStream != null)
                                {
                                    g_Player.PauseGraph();
                                    Thread.Sleep(100);
                                    this.StopLiveStream();
                                }
                                g_Player.Stop(true);
                                Thread.Sleep(250);
                            }

                            Channel prevChannel = _currentChannel;
                            ChannelGroup prevGroup = _currentChannelGroup;
                            TuneLiveStream(zappingTo);
                            if (prevChannel != null)
                            {
                                _navigatorChannels[prevChannel.ChannelType].PreviousChannel = prevChannel;
                                _navigatorChannels[prevGroup.ChannelType].PreviousChannelGroup = prevGroup;
                            }
                            _navigatorChannels[zappingTo.ChannelType].LastChannel = _currentChannel;
                            _navigatorChannels[zappingTo.ChannelType].LastChannelGroup = _currentChannelGroup;

                            if (zappingTo.ChannelType == ChannelType.Radio)
                            {
                                RadioHome.SetMusicProperties(zappingTo.DisplayName, zappingTo.ChannelId);
                            }

                        }
                        _zapChannelNr = -1;
                        _reentrant = false;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("ChannelNavigator: error CheckChannelChange() = {0}", ex.Message);
            }
            finally
            {
                _reentrant = false;
            }
            return false;
        }

        public bool ZapToChannelNumber(ChannelType channelType, int channelNr, bool useZapDelay)
        {
            ChannelGroup channelGroup;
            Channel channel = GetChannelByNumber(channelType, channelNr, out channelGroup);
            if (channel != null)
            {
                ZapToChannel(channelGroup, channel, useZapDelay);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Changes the current channel after a specified delay.
        /// </summary>
        /// <param name="channel">The channel to switch to.</param>
        /// <param name="useZapDelay">If true, the configured zap delay is used. Otherwise it zaps immediately.</param>
        public void ZapToChannel(Channel channel, bool useZapDelay)
        {
            ZapToChannel(null, channel, useZapDelay);
        }

        /// <summary>
        /// Changes the current channel and channelgroup after a specified delay.
        /// </summary>
        /// <param name="channelGroup">The channelgroup to switch to.</param>
        /// <param name="channel">The channel to switch to</param>
        /// <param name="useZapDelay">If true, the configured zap delay is used. Otherwise it zaps immediately.</param>
        public void ZapToChannel(ChannelGroup channelGroup, Channel channel, bool useZapDelay)
        {
            Log.Debug("ChannelNavigator.ZapToChannel {0} - zapdelay {1}", channel.DisplayName, useZapDelay);

            _zapGroup = channelGroup ?? _currentChannelGroup;
            _zapChannel = channel;

            if (useZapDelay)
            {
                _zapTime = DateTime.Now.AddMilliseconds(_zapDelayMs);
            }
            else
            {
                _zapTime = DateTime.Now;
            }
        }

        /// <summary>
        /// Changes to the next channel in the current group.
        /// </summary>
        /// <param name="useZapDelay">If true, the configured zap delay is used. Otherwise it zaps immediately.</param>
        public void ZapToNextChannel(bool useZapDelay)
        {
            _zapChannelNr = -1;
            if (_currentChannel != null)
            {
                ChannelType channelType = _currentChannel.ChannelType;
                Guid currentId = _zapChannel != null ? _zapChannel.ChannelId : _currentChannel.ChannelId;
                for (int index = 0; index < _navigatorChannels[channelType].Channels.Count; index++)
                {
                    if (_navigatorChannels[channelType].Channels[index].ChannelId == currentId)
                    {
                        int zapToIndex = index < _navigatorChannels[channelType].Channels.Count - 1 ? index + 1 : 0;
                        ZapToChannel(_navigatorChannels[channelType].Channels[zapToIndex],
                            useZapDelay && (GUIWindowManager.ActiveWindow == (int)(int)GUIWindow.Window.WINDOW_TVFULLSCREEN));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Changes to the previous channel in the current group.
        /// </summary>
        /// <param name="useZapDelay">If true, the configured zap delay is used. Otherwise it zaps immediately.</param>
        public void ZapToPreviousChannel(bool useZapDelay)
        {
            _zapChannelNr = -1;
            if (_currentChannel != null)
            {
                ChannelType channelType = _currentChannel.ChannelType;
                Guid currentId = _zapChannel != null ? _zapChannel.ChannelId : _currentChannel.ChannelId;
                for (int index = _navigatorChannels[channelType].Channels.Count - 1; index >= 0; index--)
                {
                    if (_navigatorChannels[channelType].Channels[index].ChannelId == currentId)
                    {
                        int zapToIndex = index > 0 ? index - 1 : _navigatorChannels[channelType].Channels.Count - 1;
                        ZapToChannel(_navigatorChannels[channelType].Channels[zapToIndex],
                            useZapDelay && (GUIWindowManager.ActiveWindow == (int)(int)GUIWindow.Window.WINDOW_TVFULLSCREEN));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Zaps to the last viewed Channel (without ZapDelay).  // mPod
        /// </summary>
        public bool ZapToLastViewedChannel(ChannelType channelType)
        {
            if (_navigatorChannels[channelType].PreviousChannel != null)
            {
                Channel prevChannel = _currentChannel;
                ChannelGroup prevGroup = _currentChannelGroup;
                ZapToChannel(_navigatorChannels[channelType].PreviousChannelGroup, _navigatorChannels[channelType].PreviousChannel, false);
                if (prevChannel != null)
                {
                    _navigatorChannels[prevChannel.ChannelType].PreviousChannel = prevChannel;
                }
                if (prevGroup != null)
                {
                    _navigatorChannels[prevGroup.ChannelType].PreviousChannelGroup = prevGroup;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region Live TV/Radio

        private void TuneLiveStream(Channel channel)
        {
            Log.Debug("ChannelNavigator: TuneLiveStream(), channel = {0}", channel.DisplayName);
            if (channel != null)
            {
                var schedulerProxy = new SchedulerServiceProxy();

                LiveStream liveStream = _liveStream;
                CurrentAndNextProgram currentAndNext = schedulerProxy.GetCurrentAndNextForChannel(channel.ChannelId, true, _liveStream);//null);

                _currentChannel = channel;
                _doingChannelChange = true;
                RenderBlackImage();

                if (liveStream != null)
                {
                    try
                    {
                        g_Player.PauseGraph();
                        g_Player.OnZapping(0x80);

                        result = this.ControlServiceProxy.TuneLiveStream(channel, ref liveStream);
                        Log.Debug("ChannelNavigator: First try to re-tune the existing TV stream (staying on the same card), result = {0}", result);

                        if (result == LiveStreamResult.Succeeded)
                        {
                            if (_isAnalog)
                                g_Player.OnZapping(-1);

                            double duration = g_Player.Duration;
                            if (g_Player.Duration < 0.0)
                                result = LiveStreamResult.UnknownError;
                            else
                            {
                                g_Player.SeekAbsolute(duration);
                                g_Player.ContinueGraph();
                            }
                        }
                        else if (result == LiveStreamResult.NoRetunePossible)// not mapped to card, card in use by recorder or other user ---> start new stream
                        {
                            // Now re-try the new channel with a new stream.
                            Log.Debug("ChannelNavigator: Seems a re-tune has failed, stop the current stream and start a new one");
                            SilentlyStopLiveStream(liveStream);
                            result = StartAndPlayNewLiveStream(channel, liveStream);
                        }
                    }
                    catch
                    {
                        result = LiveStreamResult.UnknownError;
                        Log.Error("ChannelNavigator: TuneLiveStream error");
                    }
                }
                else 
                {
                    result = StartAndPlayNewLiveStream(channel,liveStream);
                }

                _doingChannelChange = false;
                if (result == LiveStreamResult.Succeeded)
                {
                    _lastChannelChangeFailed = false;
                    StopRenderBlackImage();
                }
                else
                {
                    _lastChannelChangeFailed = true;
                    SilentlyStopLiveStream(liveStream);
                    ChannelTuneFailedNotifyUser(result, channel);
                }
            }
        }

        private LiveStreamResult StartAndPlayNewLiveStream(Channel channel,LiveStream liveStream)
        {
            LiveStreamResult result = this.ControlServiceProxy.TuneLiveStream(channel, ref liveStream);
            Log.Debug("ChannelNavigator: start a new live stream, result = {0}", result);

            if (result == LiveStreamResult.Succeeded)
            {
                result = PlayLiveStream(liveStream);
                if (result == LiveStreamResult.Succeeded)
                {
                    if (_autoFullScreen) g_Player.ShowFullScreenWindow();
                }
            }
            return result;
        }

        private void ChannelTuneFailedNotifyUser(LiveStreamResult result, Channel channel)
        {
            string TuningResult = string.Empty;
            switch (result)
            {
                case LiveStreamResult.ChannelTuneFailed:
                    TuningResult = (Utility.GetLocalizedText(TextId.ChannelTuneFailed));
                    break;

                case LiveStreamResult.NoFreeCardFound:
                    TuningResult = (Utility.GetLocalizedText(TextId.NoFreeCardFound));
                    break;

                case LiveStreamResult.NotSupported:
                    TuningResult = (Utility.GetLocalizedText(TextId.NotSupported));
                    break;

                case LiveStreamResult.NoRetunePossible:
                    TuningResult = (Utility.GetLocalizedText(TextId.NoRetunePossible));
                    break;

                case LiveStreamResult.IsScrambled:
                    TuningResult = (Utility.GetLocalizedText(TextId.IsScrambled));
                    break;

                case LiveStreamResult.UnknownError:
                    TuningResult = (Utility.GetLocalizedText(TextId.UnknownError));
                    break;
            }

            if (GUIWindowManager.ActiveWindow == (int)(int)GUIWindow.Window.WINDOW_TVFULLSCREEN)
            {
                // If failed and wasPlaying TV, left screen as it is and show zaposd with error message 
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_TV_ERROR_NOTIFY, GUIWindowManager.ActiveWindow, 0,
                                                0, 0, 0,
                                                null);

                msg.SendToTargetWindow = true;
                msg.Object = TuningResult; // forward error info object
                msg.TargetWindowId = (int)(int)GUIWindow.Window.WINDOW_TVFULLSCREEN;
                GUIGraphicsContext.SendMessage(msg);
            }
            else
            {
                // if not fulscreen, show notify dialog with the error message
                string caption = string.Empty;
                string tvlogo = string.Empty;
                if (channel != null)
                {
                    _navigatorChannels[_currentChannel.ChannelType].PreviousChannel = channel;
                    _currentChannel = null;

                    tvlogo = Utility.GetLogoImage(channel, new SchedulerServiceProxy());

                    if (channel.ChannelType == ChannelType.Television)
                        caption = GUILocalizeStrings.Get(605) + " - " + channel.DisplayName;
                    else
                        caption = GUILocalizeStrings.Get(665) + " - " + channel.DisplayName;
                }

                GUIDialogNotify pDlgNotify = (GUIDialogNotify)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_NOTIFY);
                if (pDlgNotify != null)
                {
                    pDlgNotify.Reset();
                    pDlgNotify.ClearAll();
                    pDlgNotify.SetHeading(caption);
                    if (!string.IsNullOrEmpty(TuningResult))
                    {
                        pDlgNotify.SetText(TuningResult);
                    }
                    pDlgNotify.SetImage(tvlogo);
                    pDlgNotify.TimeOut = 5;
                    pDlgNotify.DoModal(GUIWindowManager.ActiveWindow);
                }
            }
        }

        private LiveStreamResult PlayLiveStream(LiveStream liveStream)
        {
            Log.Debug("ChannelNavigator: PlayLiveStream()");
            LiveStreamResult result = LiveStreamResult.Succeeded;
            string fileName;
            bool isRTSP;
            GetPlayerFileNameAndOffset(liveStream, out fileName, out isRTSP);

            if (liveStream != null)
                _isAnalog = (ControlServiceProxy.GetLiveStreamTuningDetails(liveStream).CardType == CardType.Analog);

            if (!isRTSP)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (System.IO.File.Exists(fileName))
                        break;

                    Thread.Sleep(100);
                    Log.Info("Channelnavigator: startplay: waiting for TS file {0}", fileName);
                }
                if (!System.IO.File.Exists(fileName))
                {
                    result = LiveStreamResult.UnknownError;
                    g_Player.Stop();
                }
            }

            if (result == LiveStreamResult.Succeeded)
            {
                for (int i = 0; i < 50; i++)
                {
                    if (g_Player.Play(fileName, liveStream.Channel.ChannelType == ChannelType.Television ? g_Player.MediaType.TV : g_Player.MediaType.Radio))
                    {
                        double duration = g_Player.Duration;
                        if (duration > 0.0)
                        {
                            result = LiveStreamResult.Succeeded;
                            g_Player.SeekAbsolute(duration);
                            _liveStream = liveStream;
                            break;
                        }
                        else
                        {
                            result = LiveStreamResult.UnknownError;
                            g_Player.Stop();
                            Thread.Sleep(50);
                            Log.Debug("ChannelNavigator: PlayLiveStream_timeout = {0}", i);
                        }
                    }
                    else
                    {
                        result = LiveStreamResult.UnknownError;
                        break;
                    }
                }
            }
            Log.Debug("ChannelNavigator: PlayLiveStream_Result = {0}", result);
            return result;
        }

        private void SilentlyStopLiveStream(LiveStream liveStream)
        {
            Log.Debug("ChannelNavigator: SilentlyStopLiveStream()");
            g_Player.Stop();
            _liveStream = liveStream;
            StopLiveStream();
        }

        private static void GetPlayerFileNameAndOffset(LiveStream liveStream, out string fileName, out bool isRTSP)
        {
            Log.Debug("ChannelNavigator: GetPlayerFileNameAndOffset()");
            fileName = liveStream.TimeshiftFile;
            isRTSP = false;

            //TODO: temporary workaround to play radio over UNC instead of RTSP (some problems with tsreader in combination with rtsp radio)
            if (liveStream.Channel.ChannelType == ChannelType.Television)
            {
                if (String.IsNullOrEmpty(fileName)
                    || (PluginMain.PreferRtspForLiveTv && liveStream.RtspUrl.StartsWith("rtsp://", StringComparison.CurrentCultureIgnoreCase)))
                {
                    isRTSP = true;
                    fileName = liveStream.RtspUrl;
                }
            }
            Log.Debug("ChannelNavigator: filename = {0}", fileName);
        }

        /// <summary>
        /// Use this when you are sure that the livestream playback was stopped some time ago.
        /// </summary>
        public void StopLiveStream()
        {
            Log.Debug("ChannelNavigator: StopLiveStream()");
            if (_liveStream != null)
            {
                this.ControlServiceProxy.StopLiveStream(_liveStream);
                _liveStream = null;

                if (_currentChannel != null)
                    _navigatorChannels[_currentChannel.ChannelType].PreviousChannel = _currentChannel;
            }

            if (!_doingChannelChange && !_lastChannelChangeFailed)
                _currentChannel = null;
        }

        /// <summary>
        /// Use this when the livestream playback just stopped.
        /// </summary>
        public void AsyncStopLiveStream()
        {
            if (_liveStream != null)
            {
                _streamToStopAsync = _liveStream;
                _liveStream = null;

                if (_currentChannel != null)
                    _navigatorChannels[_currentChannel.ChannelType].PreviousChannel = _currentChannel;

                _asyncStopLiveStreamThread = new Thread(new ThreadStart(AsyncStopLiveStreamThreadMain));
                _asyncStopLiveStreamThread.Start();
            }

            if (!_doingChannelChange && !_lastChannelChangeFailed)
                _currentChannel = null;
        }

        private Thread _asyncStopLiveStreamThread;
        private void AsyncStopLiveStreamThreadMain()
        {
            if (_streamToStopAsync != null)
            {
                for (int i = 0; i < 50 ;i++)
                {
                    if (!GUIGraphicsContext.IsPlaying)
                        break;

                    Thread.Sleep(100);
                    Log.Debug("ChannelNavigator: AsyncStopLiveStream wait 100ms {0}",_lastChannelChangeFailed);
                }
                Thread.Sleep(200);

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        this.ControlServiceProxy.StopLiveStream(_streamToStopAsync);
                    }
                    catch
                    {
                        Thread.Sleep(25);
                    }
                }
                _streamToStopAsync = null;
            }
        }

        public void SendLiveStreamKeepAlive()
        {
            if (_liveStream != null)
            {
                if (!this.ControlServiceProxy.KeepLiveStreamAlive(_liveStream))
                {
                    if (g_Player.Playing && (g_Player.IsTV || g_Player.IsRadio))
                    {
                        g_Player.Stop();
                        Log.Warn("ChannelNavigator: stopped unknown live stream: {0}", _liveStream.RtspUrl);
                    }
                }
            }
        }

        #endregion

        #region Private methods

        private void RefreshChannelsInGroup(ChannelType channelType)
        {
            RefreshChannelsInGroup(new SchedulerServiceProxy(), channelType);
        }

        private void RefreshChannelsInGroup(SchedulerServiceProxy schedulerProxy, ChannelType channelType)
        {
            try
            {
                if (_currentChannelGroup != null)
                {
                    _navigatorChannels[channelType].Channels = new List<Channel>(
                        schedulerProxy.GetChannelsInGroup(_currentChannelGroup.ChannelGroupId, true));
                }
                else
                {
                    _navigatorChannels[channelType].Channels = new List<Channel>();
                }
                _navigatorChannels[channelType].ChannelsByNumber.Clear();
                _navigatorChannels[channelType].GroupsByChannelNumber.Clear();
            }
            catch (Exception ex)
            {
                Log.Error("ChannelNavigator: Error in RefreshChannelsInGroup - {0}", ex.Message);
            }
        }

        #endregion

        #region Teletext

        public bool HasTeletext()
        {
            bool hasTeletext = false;
            if (_liveStream != null)
            {
                hasTeletext = this.ControlServiceProxy.HasTeletext(_liveStream);
            }
            return hasTeletext;
        }

        public void StartGrabbingTeletext()
        {
            if (_liveStream != null)
            {
                new ControlServiceProxy().StartGrabbingTeletext(_liveStream);
            }
        }

        public void StopGrabbingTeletext()
        {
            if (_liveStream != null)
            {
                this.ControlServiceProxy.StopGrabbingTeletext(_liveStream);
            }
        }

        public bool IsGrabbingTeletext()
        {
            bool isGrabbingTeletext = false;
            if (_liveStream != null)
            {
                isGrabbingTeletext = this.ControlServiceProxy.IsGrabbingTeletext(_liveStream);
            }
            return isGrabbingTeletext;
        }

        public TeletextPage GetTeletextPage(int pageNumber, int subPageNumber)
        {
            if (_liveStream != null)
            {
                return this.ControlServiceProxy.GetTeletextPage(_liveStream, pageNumber, subPageNumber);
            }
            return null;
        }
        #endregion

        #region Serialization

        public void LoadSettings()
        {
            using (Settings xmlreader = new MPSettings())
            {
                Log.Info("ChannelNavigator::LoadSettings()");
                _autoFullScreen = xmlreader.GetValueAsBool("mytv", "autofullscreen", true);
                _zapDelayMs = 1000 * xmlreader.GetValueAsInt("movieplayer", "zapdelay", 2);
                LoadGroupAndChannelSettings(xmlreader, ChannelType.Radio, "myradio");
                LoadGroupAndChannelSettings(xmlreader, ChannelType.Television, "mytv");
            }
        }

        private void LoadGroupAndChannelSettings(Settings xmlreader, ChannelType channelType, string section)
        {
            string channelName = xmlreader.GetValueAsString(section, "channel", String.Empty);

            string groupname = xmlreader.GetValueAsString(section, "group", Utility.GetLocalizedText(TextId.AllChannels));
            foreach (ChannelGroup group in _navigatorChannels[channelType].Groups)
            {
                if (group.GroupName == groupname)
                {
                    _navigatorChannels[channelType].PreviousChannelGroup = group;
                    _currentChannelGroup = group;
                    RefreshChannelsInGroup(channelType);
                    break;
                }
            }
            _navigatorChannels[channelType].LastChannelGroup = _navigatorChannels[channelType].PreviousChannelGroup;

            _navigatorChannels[channelType].PreviousChannel = null;
            foreach (Channel channel in _navigatorChannels[channelType].Channels)
            {
                if (channel.DisplayName == channelName)
                {
                    _navigatorChannels[channelType].PreviousChannel = channel;
                    break;
                }
            }
            _navigatorChannels[channelType].LastChannel = _navigatorChannels[channelType].PreviousChannel;
        }

        public void SaveSettings()
        {
            using (Settings xmlwriter = new MPSettings())
            {
                SaveGroupAndChannelSettings(xmlwriter, ChannelType.Television, "mytv");
                SaveGroupAndChannelSettings(xmlwriter, ChannelType.Radio, "myradio");
            }
        }

        private void SaveGroupAndChannelSettings(Settings xmlwriter, ChannelType channelType, string section)
        {
            try
            {
                if (_currentChannelGroup != null
                    && _currentChannelGroup.ChannelType == channelType)
                {
                    xmlwriter.SetValue(section, "group", _currentChannelGroup.GroupName);
                }
                else if (_navigatorChannels[channelType].LastChannelGroup != null)
                {
                    xmlwriter.SetValue(section, "group", _navigatorChannels[channelType].LastChannelGroup.GroupName);
                }
                else
                {
                    xmlwriter.SetValue(section, "group", String.Empty);
                }
            }
            catch (Exception){}

            try
            {
                if (_currentChannel != null
                    && _currentChannel.ChannelType == channelType)
                {
                    xmlwriter.SetValue(section, "channel", _currentChannel.DisplayName);
                }
                else if (_navigatorChannels[channelType].LastChannel != null)
                {
                    xmlwriter.SetValue(section, "channel", _navigatorChannels[channelType].LastChannel.DisplayName);
                }
                else
                {
                    xmlwriter.SetValue(section, "channel", String.Empty);
                }
            }
            catch (Exception){}
        }

        #endregion

        #region IDisposable Implementation

        ~ChannelNavigator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _isDisposed;
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // Managed resources
                    GUIGraphicsContext.OnVideoReceived -= GUIGraphicsContext_OnVideoReceived;
                    GUIGraphicsContext.OnBlackImageRendered -= GUIGraphicsContext_OnBlackImageRendered;
                    _waitForBlackScreenEvent.Close();
                    _waitForBlackScreenEvent = null;
                }
                // No unmanaged resources to dispose
            }
            _isDisposed = true;
        }

        #endregion
    }
}
