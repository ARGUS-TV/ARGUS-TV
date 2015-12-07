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

// Uncomment this to enable RTSP-test code
//#define USE_ARGUS_RTSP

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Globalization;

using Gentle.Framework;
using TvControl;
using TvLibrary.Interfaces;
using TvLibrary.Log;

using ArgusTV.DataContracts;
using ArgusTV.DataContracts.Tuning;
using ArgusTV.Recorder.MediaPortalTvServer.Channels;
using ArgusTV.Common.Recorders;
#if USE_ARGUS_RTSP
using ArgusTV.StreamingServer.ServiceProxy;
#endif

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    public class MediaPortalRecorderTunerService : ThreadedRecorderService<RecordingThread>
    {
        private const string _argusLiveUserName = "ARGUSLive";

#if USE_ARGUS_RTSP
        private const string _rtspUrlSuffixFormat = @"Live-Card{0}-{1}";
        private const string _rtspApplicationName = "MediaPortal TV Server";
#endif

        private object _liveStreamsLock = new object();
        private Dictionary<string, LiveStream> _liveStreams = new Dictionary<string, LiveStream>();
        private Dictionary<string, IUser> _liveStreamUsers = new Dictionary<string, IUser>();

        protected override string Name
        {
            get { return "MediaPortal TV Server"; }
        }

#if USE_ARGUS_RTSP
        public override void Initialize(Guid recorderTunerId, string serverHostName, int tcpPort)
        {
            base.Initialize(recorderTunerId, serverHostName, tcpPort);
            RegisterApplicationInRtspServer(true);
        }
#endif

        public override string AllocateCard(Channel channel, IList<CardChannelAllocation> alreadyAllocated, bool useReversePriority)
        {
            try
            {
                //
                // Find the channel in MediaPortal (match by DisplayName) and get all the
                // cards this channel is mapped to.
                //
                TvDatabase.Channel mpChannel;
                List<TvDatabase.Card> availableCards = GetCardsForChannel(channel, out mpChannel);

                // Sort the cards by reverse(!) priority.
                int order = useReversePriority ? -1 : 1;
                availableCards.Sort(delegate(TvDatabase.Card c1, TvDatabase.Card c2) { return order * c1.Priority.CompareTo(c2.Priority); });

                //
                // Now remove all cards that were previously allocated.
                //
                foreach (CardChannelAllocation allocation in alreadyAllocated)
                {
                    TvDatabase.Card allocatedCard = GetCardByCardId(allocation.CardId);
                    List<int> allocatedHybridGroupIds = Utility.GetHybridGroupIds(allocatedCard);
                    List<TvDatabase.Card> cardsToRemove = new List<TvDatabase.Card>();
                    foreach (TvDatabase.Card card in availableCards)
                    {
                        bool isHybrid = (card.IdCard != allocatedCard.IdCard)
                            && Utility.IsInSameHybridGroup(card, allocatedHybridGroupIds);
                        if (isHybrid
                            || !IsCardFreeOrSharedByAllocation(card, mpChannel, allocation, alreadyAllocated))
                        {
                            cardsToRemove.Add(card);
                        }
                    }
                    foreach (TvDatabase.Card cardToRemove in cardsToRemove)
                    {
                        availableCards.Remove(cardToRemove);
                    }
                }

                //
                // If there's still at least one card available, return the card
                // with the highest priority.
                //
                foreach (TvDatabase.Card card in availableCards)
                {
                    TvDatabase.TuningDetail tuning = Utility.FindTuningDetailOnCard(mpChannel, card.IdCard);
                    if (tuning != null)
                    {
                        return GetCardId(card);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(TraceEventType.Error, ex.Message);
            }
            return null;
        }

        private bool IsCardFreeOrSharedByAllocation(TvDatabase.Card card, TvDatabase.Channel mpChannel, CardChannelAllocation allocation,
            IList<CardChannelAllocation> alreadyAllocated)
        {
            TvDatabase.TuningDetail tuning = Utility.FindTuningDetailOnCard(mpChannel, card.IdCard);
            if (GetCardId(card) == allocation.CardId
                && tuning != null)
            {
                //
                // The card is allocated by this allocation, but may be able to reuse the card. So let's
                // check if the if the card (and CAM) allow this.
                //
                // Note: "!ChannelAlreadyAllocatedOn(alreadyAllocated, allocation.CardId, channelId)" was
                // not added since TV Server can record the same channel several times on the same transponder.
                if (card.DecryptLimit == 0
                    || CountNumTimesAllocated(alreadyAllocated, allocation.CardId) < card.DecryptLimit)
                {
                    // Get the previously allocated channel and its tuning details and let's check if the
                    // channel we want is on the same transponder as that channel.
                    TvDatabase.Channel allocatedChannel = GetLinkedMediaPortalChannel(allocation.ChannelType,
                        allocation.ChannelId, allocation.ChannelName);
                    TvDatabase.Card allocatedCard = GetCardByCardId(allocation.CardId);
                    if (allocatedChannel != null
                        && allocatedCard != null
                        && Utility.IsSameTransponder(allocatedCard.IdCard, tuning, allocatedChannel))
                    {
                        // Same transponder, so we can actually re-use this card and consider it free.
                        return true;
                    }
                }
                // The card is allocated by this allocation, and it's not for a channel on the same
                // transponder, so it's not free.
                return false;
            }

            // The card is not allocated by this allocation, so it's free.
            return true;
        }

        public override bool StartRecording(string schedulerBaseUrl, CardChannelAllocation channelAllocation,
            DateTime startTimeUtc, DateTime stopTimeUtc, UpcomingProgram recordingProgram, string suggestedBaseFileName)
        {
            bool result = false;

            TvDatabase.Card recordOnCard = GetCardByCardId(channelAllocation.CardId);
            if (recordOnCard != null)
            {
                TvDatabase.Channel channel = GetLinkedMediaPortalChannel(channelAllocation.ChannelType,
                    channelAllocation.ChannelId, channelAllocation.ChannelName);
                if (channel != null)
                {
                    result = this.RecordingThreads.StartNewThread(new RecordingThread(this.RecorderId, schedulerBaseUrl,
                        channelAllocation, startTimeUtc, stopTimeUtc, recordingProgram, suggestedBaseFileName, recordOnCard, channel));
                    if (!result)
                    {
                        Log(TraceEventType.Error, "{0} - Already recording {1}", this.Name, recordingProgram.CreateProgramTitle());
                    }
                }
                else
                {
                    Log(TraceEventType.Error, "{0} - Channel {1} not found", this.Name, channelAllocation.ChannelName);
                }
            }
            else
            {
                Log(TraceEventType.Error, "{0} - Card {1} not found", this.Name, channelAllocation.CardId);
            }

            return result;
        }

        public override List<string> GetRecordingShares()
        {
            return GetCardFolders(true);
        }

        public override List<string> GetTimeshiftShares()
        {
            return GetCardFolders(false);
        }

        private static List<string> GetCardFolders(bool getRecordingFolder)
        {
            List<string> shares = new List<string>();

            List<TvDatabase.Card> cards = Utility.GetAllCards();
            foreach (TvDatabase.Card card in cards)
            {
                string folder = getRecordingFolder ? card.RecordingFolder : card.TimeShiftFolder;
                if (!String.IsNullOrEmpty(folder))
                {
                    string uncRecordingFolder = Common.ShareExplorer.GetUncPathForLocalPath(folder);
                    shares.Add(String.IsNullOrEmpty(uncRecordingFolder) ? folder : uncRecordingFolder);
                }
                else
                {
                    shares.Add(String.Empty);
                }
            }

            return shares;
        }

        #region Live TV/Radio

        public override LiveStreamResult TuneLiveStream(Channel channel, CardChannelAllocation upcomingRecordingAllocation, ref LiveStream liveStream, StreamingMode mode)
        {
            try
            {
                TvDatabase.Channel mpChannel;
                List<TvDatabase.Card> availableCards = GetCardsForChannel(channel, out mpChannel);

                // Sort the cards by priority.
                availableCards.Sort(delegate(TvDatabase.Card c1, TvDatabase.Card c2) { return c1.Priority.CompareTo(c2.Priority); });

                if (liveStream != null
                    && _liveStreamUsers.ContainsKey(liveStream.RtspUrl))
                {
                    IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];
                    foreach (TvDatabase.Card card in availableCards)
                    {
                        if (card.IdCard == tve3User.CardId)
                        {
                            if (Utility.CardFreeOrUsingSameTransponder(card, mpChannel, tve3User))
                            {
                                Log("TuneLiveTvStream(): tuning on card {0} {1}", card.IdCard, card.Name);
                                lock (_liveStreamsLock)
                                {
                                    _liveStreams[liveStream.RtspUrl].StreamLastAliveTimeUtc = DateTime.UtcNow;
                                }
                                LiveStreamResult result = StartTimeShifting(channel, card, mpChannel, ref tve3User, ref liveStream);
                                if (result != LiveStreamResult.NoFreeCardFound)
                                {
                                    return result;
                                }
                            }
                        }
                    }

                    return LiveStreamResult.NoRetunePossible;
                }
                else
                {
                    if (liveStream != null)
                    {
                        StopLiveStream(liveStream);
                        liveStream = null;
                    }

                    List<int> inUseHybridGroups = new List<int>();

                    foreach (TvDatabase.Card card in availableCards)
                    {
                        if (!Utility.IsInSameHybridGroup(card, inUseHybridGroups)
                            && Utility.CardFreeOrUsingSameTransponder(card, mpChannel))
                        {
                            string userName = String.Format(CultureInfo.InvariantCulture, "{0}{1}", _argusLiveUserName, Guid.NewGuid());

                            IUser tve3User = new User(userName, true, card.IdCard);
                            tve3User.IdChannel = mpChannel.IdChannel;
                            tve3User.SubChannel = -1;

                            Log("TuneLiveTvStream(): tuning on card {0} {1}", card.IdCard, card.Name);

                            LiveStreamResult result = StartTimeShifting(channel, card, mpChannel, ref tve3User, ref liveStream);
                            if (result != LiveStreamResult.NoFreeCardFound)
                            {
                                return result;
                            }
                        }
                        // For hybrid cards, keep track of the groups that are in use.
                        inUseHybridGroups.AddRange(Utility.GetHybridGroupIds(card));
                    }

                    return LiveStreamResult.NoFreeCardFound;
                }
            }
            catch (Exception ex)
            {
                Log(TraceEventType.Error, ex.Message);
                return LiveStreamResult.UnknownError;
            }
        }

        private LiveStreamResult StartTimeShifting(Channel channel, TvDatabase.Card card, TvDatabase.Channel mpChannel,
            ref IUser tve3User, ref LiveStream liveStream)
        {
            IChannel tuningChannel = Utility.FindTuningChannelOnCard(mpChannel, card.IdCard);
            if (tuningChannel != null)
            {
                if (TvServerPlugin.TvController_Tune(ref tve3User, tuningChannel, mpChannel.IdChannel) == TvResult.Succeeded)
                {
                    string timeshiftFileName = Path.Combine(card.TimeShiftFolder,
                        String.Format(CultureInfo.InvariantCulture, @"live{0}-{1}", tve3User.CardId, tve3User.SubChannel));

                    switch (TvServerPlugin.TvController_StartTimeShifting(ref tve3User, ref timeshiftFileName))
                    {
                        case TvResult.Succeeded:
                            if (liveStream == null)
                            {
                                string rtspUrl = TvServerPlugin.TvController_GetStreamingUrl(tve3User);
                                string tsBufferFile = GetTsBufferFile(tve3User);
                                liveStream = new LiveStream(channel, rtspUrl);
                                liveStream.TimeshiftFile = tsBufferFile;
#if USE_ARGUS_RTSP
                                string rtspUrlSuffix = String.Format(_rtspUrlSuffixFormat, tve3User.CardId, tve3User.SubChannel);
                                liveStream.RtspUrl = StartRtspStream(tsBufferFile, rtspUrlSuffix);
#endif
                            }
                            liveStream.Channel = channel;
                            liveStream.CardId = tve3User.CardId.ToString(CultureInfo.InvariantCulture);
                            liveStream.StreamLastAliveTimeUtc = DateTime.UtcNow;
                            lock (_liveStreamsLock)
                            {
                                _liveStreams[liveStream.RtspUrl] = liveStream;
                                _liveStreamUsers[liveStream.RtspUrl] = tve3User;
                            }
                            return LiveStreamResult.Succeeded;

                        case TvResult.AllCardsBusy:
                            return LiveStreamResult.NoFreeCardFound;

                        case TvResult.ChannelIsScrambled:
                            return LiveStreamResult.IsScrambled;

                        case TvResult.ChannelNotMappedToAnyCard:
                        case TvResult.NoSignalDetected:
                        case TvResult.NoTuningDetails:
                        case TvResult.NoVideoAudioDetected:
                        case TvResult.UnknownChannel:
                            return LiveStreamResult.ChannelTuneFailed;

                        default:
                            return LiveStreamResult.UnknownError;
                    }
                }
                else
                {
                    Log(TraceEventType.Error, "StartTimeShifting(): failed to tune to {0}", tuningChannel.Name);
                    return LiveStreamResult.ChannelTuneFailed;
                }
            }
            else
            {
                Log(TraceEventType.Error, "StartTimeShifting(): no tuning channel found for {0}", mpChannel.DisplayName);
                return LiveStreamResult.ChannelTuneFailed;
            }
        }

        public override void StopLiveStream(LiveStream liveStream)
        {
            lock (_liveStreamsLock)
            {
                try
                {
                    if (_liveStreams.ContainsKey(liveStream.RtspUrl))
                    {
                        IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];

#if USE_ARGUS_RTSP
                        StopRtspStream(liveStream);
#endif

                        if (TvServerPlugin.TvController_IsTimeShifting(ref tve3User))
                        {
                            if (!TvServerPlugin.TvController_StopTimeShifting(ref tve3User))
                            {
                                Log(TraceEventType.Error, "Failed to stop live stream '{0}'", liveStream.RtspUrl);
                            }
                        }
                        _liveStreams.Remove(liveStream.RtspUrl);
                        _liveStreamUsers.Remove(liveStream.RtspUrl);
                    }
                    CleanUpTimeshiftingFiles(liveStream.TimeshiftFile);
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Error, ex.Message);
                }
            }
        }

        private void CleanUpTimeshiftingFiles(string timeshiftFile)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Path.GetDirectoryName(timeshiftFile));
                foreach (FileInfo fileInfo in dirInfo.GetFiles(Path.GetFileName(timeshiftFile) + "*"))
                {
                    try { File.Delete(fileInfo.FullName); }
                    catch { }
                }
            }
            catch
            {
                Log(TraceEventType.Error, "Failed to clean up timeshifting files for '{0}'", timeshiftFile);
            }
        }

        public override IList<LiveStream> GetLiveStreams()
        {
            List<LiveStream> liveStreams = new List<LiveStream>();
            lock (_liveStreamsLock)
            {
                try
                {
                    // Get cards in reverse priority.
                    List<TvDatabase.Card> cards = Utility.GetAllCards();
                    cards.Sort(delegate(TvDatabase.Card c1, TvDatabase.Card c2) { return -c1.Priority.CompareTo(c2.Priority); });

                    // Get the list of live streams from TV Server
                    Dictionary<string, IUser> mpStreamUsers = new Dictionary<string, IUser>();
                    foreach (TvDatabase.Card card in cards)
                    {
                        IUser[] cardUsers = TvServerPlugin.TvController_GetUsersForCard(card.IdCard);
                        if (cardUsers != null)
                        {
                            foreach (IUser user in cardUsers)
                            {
                                if (user.Name.StartsWith(_argusLiveUserName))
                                {
                                    IUser tve3User = user;
                                    if (TvServerPlugin.TvController_IsTimeShifting(ref tve3User))
                                    {
                                        mpStreamUsers.Add(TvServerPlugin.TvController_GetStreamingUrl(tve3User), tve3User);
                                    }
                                }
                            }
                        }
                    }

                    // Now loop our own list and check if all those streams are indeed still up.
                    List<string> keysToRemove = new List<string>();

                    foreach (LiveStream stream in _liveStreams.Values)
                    {
                        if (mpStreamUsers.ContainsKey(stream.RtspUrl))
                        {
                            liveStreams.Add(stream);
                        }
                        else
                        {
                            keysToRemove.Add(stream.RtspUrl);
                        }
                    }

                    // Remove streams that no longer exist.
                    foreach(string keyToRemove in keysToRemove)
                    {
                        _liveStreams.Remove(keyToRemove);
                        _liveStreamUsers.Remove(keyToRemove);
                    }

                    // Check if there are any live streams within MP that we don't know about.
                    // If so, stop those streams (they may be left-overs from client crashes).
                    foreach (string rtspUrl in mpStreamUsers.Keys)
                    {
                        if (!_liveStreams.ContainsKey(rtspUrl))
                        {
                            IUser tve3User = mpStreamUsers[rtspUrl];
                            TvServerPlugin.TvController_StopTimeShifting(ref tve3User, TvStoppedReason.KickedByAdmin);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Error, ex.Message);
                }
            }
            return liveStreams;
        }

        public override bool KeepLiveStreamAlive(LiveStream liveStream)
        {
            lock (_liveStreamsLock)
            {
                if (_liveStreams.ContainsKey(liveStream.RtspUrl))
                {
                    _liveStreams[liveStream.RtspUrl].StreamLastAliveTimeUtc = liveStream.StreamLastAliveTimeUtc;
                    IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];
                    if (TvServerPlugin.TvController_IsTimeShifting(ref tve3User))
                    {
                        TvServerPlugin.TvController_HeartBeat(tve3User);
                        return true;
                    }
                    else
                    {
                        _liveStreams.Remove(liveStream.RtspUrl);
                        _liveStreamUsers.Remove(liveStream.RtspUrl);
                    }
                }
            }
            return false;
        }

        public override IList<ChannelLiveState> GetChannelsLiveState(IList<Channel> channels, LiveStream liveStream)
        {
            List<ChannelLiveState> liveStates = new List<ChannelLiveState>();

            IUser tve3User = null;
            if (liveStream != null
                && _liveStreamUsers.ContainsKey(liveStream.RtspUrl))
            {
                tve3User = _liveStreamUsers[liveStream.RtspUrl];
            }

            foreach (Channel channel in channels)
            {
                TvDatabase.Channel mpChannel;
                List<TvDatabase.Card> availableCards = GetCardsForChannel(channel, out mpChannel);
                availableCards.Sort(delegate(TvDatabase.Card c1, TvDatabase.Card c2) { return c1.Priority.CompareTo(c2.Priority); });

                if (availableCards.Count > 0)
                {
                    ChannelLiveState liveState = ChannelLiveState.NoFreeCard;

                    List<int> inUseHybridGroups = new List<int>();
                    foreach (TvDatabase.Card card in availableCards)
                    {
                        if (!Utility.IsInSameHybridGroup(card, inUseHybridGroups)
                            && Utility.CardFreeOrUsingSameTransponder(card, mpChannel, tve3User))
                        {
                            IChannel tuningChannel = Utility.FindTuningChannelOnCard(mpChannel, card.IdCard);
                            if (tuningChannel != null)
                            {
                                liveState = ChannelLiveState.Tunable;
                                break;
                            }
                            else
                            {
                                liveState = ChannelLiveState.NotTunable;
                            }
                        }
                    }

                    liveStates.Add(liveState);
                }
                else
                {
                    liveStates.Add(ChannelLiveState.NotTunable);
                }
            }

            return liveStates;
        }

        public override ServiceTuning GetLiveStreamTuningDetails(LiveStream liveStream)
        {
            ServiceTuning result = null;

            lock (_liveStreamsLock)
            {
                if (_liveStreams.ContainsKey(liveStream.RtspUrl))
                {
                    IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];

                    IChannel channel = TvServerPlugin.TvController_CurrentChannel(tve3User);

                    var dvbSChannel = channel as TvLibrary.Channels.DVBSChannel;
                    if (dvbSChannel != null)
                    {
                        result = new ServiceTuning()
                        {
                            CardType = ArgusTV.DataContracts.Tuning.CardType.DvbS,
                            Frequency = (int)dvbSChannel.Frequency,
                            InnerFecRate = (ArgusTV.DataContracts.Tuning.FecCodeRate)dvbSChannel.InnerFecRate,
                            Modulation = (ArgusTV.DataContracts.Tuning.Modulation)dvbSChannel.ModulationType,
                            Name = dvbSChannel.Name,
                            ONID = dvbSChannel.NetworkId,
                            OrbitalPosition = dvbSChannel.SatelliteIndex, // TODO: check if this is the right number
                            Pilot = (ArgusTV.DataContracts.Tuning.Pilot)dvbSChannel.Pilot,
                            ProviderName = dvbSChannel.Provider,
                            IsFreeToAir = dvbSChannel.FreeToAir,
                            RollOff = (ArgusTV.DataContracts.Tuning.RollOff)dvbSChannel.Rolloff,
                            SID = dvbSChannel.ServiceId,
                            SignalPolarisation = (ArgusTV.DataContracts.Tuning.SignalPolarisation)dvbSChannel.Polarisation,
                            SymbolRate = dvbSChannel.SymbolRate,
                            TSID = dvbSChannel.TransportId
                        };
                    }
                    else
                    {
                        var dvbCChannel = channel as TvLibrary.Channels.DVBCChannel;
                        if (dvbCChannel != null)
                        {
                            result = new ServiceTuning()
                            {
                                CardType = ArgusTV.DataContracts.Tuning.CardType.DvbC,
                                Frequency = (int)dvbCChannel.Frequency,
                                Modulation = (ArgusTV.DataContracts.Tuning.Modulation)dvbCChannel.ModulationType,
                                Name = dvbCChannel.Name,
                                ONID = dvbCChannel.NetworkId,
                                ProviderName = dvbCChannel.Provider,
                                IsFreeToAir = dvbCChannel.FreeToAir,
                                SID = dvbCChannel.ServiceId,
                                SymbolRate = dvbCChannel.SymbolRate,
                                TSID = dvbCChannel.TransportId
                            };
                        }
                        else
                        {
                            var dvbTChannel = channel as TvLibrary.Channels.DVBTChannel;
                            if (dvbTChannel != null)
                            {
                                result = new ServiceTuning()
                                {
                                    CardType = ArgusTV.DataContracts.Tuning.CardType.DvbT,
                                    Frequency = (int)dvbTChannel.Frequency,
                                    Bandwidth = dvbTChannel.BandWidth,
                                    Name = dvbTChannel.Name,
                                    ONID = dvbTChannel.NetworkId,
                                    ProviderName = dvbTChannel.Provider,
                                    IsFreeToAir = dvbTChannel.FreeToAir,
                                    SID = dvbTChannel.ServiceId,
                                    TSID = dvbTChannel.TransportId
                                };
                            }
                            else
                            {
                                var atscChannel = channel as TvLibrary.Channels.ATSCChannel;
                                if (atscChannel != null)
                                {
                                    result = new ServiceTuning()
                                    {
                                        CardType = ArgusTV.DataContracts.Tuning.CardType.Atsc,
                                        Frequency = (int)atscChannel.Frequency,
                                        MajorChannel = atscChannel.MajorChannel,
                                        MinorChannel = atscChannel.MinorChannel,
                                        Name = atscChannel.Name,
                                        PhysicalChannel = atscChannel.PhysicalChannel,
                                        ProviderName = atscChannel.Provider,
                                        IsFreeToAir = atscChannel.FreeToAir,
                                        SID = atscChannel.ServiceId,
                                        TSID = atscChannel.TransportId
                                    };
                                }
                                else
                                {
                                    var analogChannel = channel as TvLibrary.Implementations.AnalogChannel;
                                    if (analogChannel != null)
                                    {
                                        result = new ServiceTuning()
                                        {
                                            CardType = ArgusTV.DataContracts.Tuning.CardType.Analog,
                                            Frequency = (int)analogChannel.Frequency,
                                            Name = analogChannel.Name,
                                            IsFreeToAir = analogChannel.FreeToAir,
                                            PhysicalChannel = analogChannel.ChannelNumber
                                        };
                                    }
                                    else
                                    {
                                        var dvbIPChannel = channel as TvLibrary.Channels.DVBIPChannel;
                                        if (dvbIPChannel != null)
                                        {
                                            result = new ServiceTuning()
                                            {
                                                CardType = ArgusTV.DataContracts.Tuning.CardType.DvbIP,
                                                Url = dvbIPChannel.Url,
                                                Name = dvbIPChannel.Name,
                                                ProviderName = dvbIPChannel.Provider,
                                                IsFreeToAir = dvbIPChannel.FreeToAir
                                            };
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (result != null)
                    {
                        result.SignalQuality = TvServerPlugin.TvController_SignalQuality(tve3User.CardId);
                        result.SignalStrength = TvServerPlugin.TvController_SignalLevel(tve3User.CardId);
                    }
                }
            }

            return result;
        }

        #endregion

        #region TeleText

        public override bool HasTeletext(LiveStream liveStream)
        {
            if (liveStream.Channel.ChannelType == ChannelType.Television)
            {
                try
                {
                    if (_liveStreams.ContainsKey(liveStream.RtspUrl))
                    {
                        IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];
                        return TvServerPlugin.TvController_HasTeletext(tve3User);
                    }
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Error, ex.Message);
                }
            }
            return false;
        }

        public override void StartGrabbingTeletext(LiveStream liveStream)
        {
            if (liveStream.Channel.ChannelType == ChannelType.Television)
            {
                try
                {
                    if (_liveStreams.ContainsKey(liveStream.RtspUrl))
                    {
                        IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];
                        TvServerPlugin.TvController_StartGrabbingTeletext(tve3User);
                    }
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Error, ex.Message);
                }
            }
        }

        public override void StopGrabbingTeletext(LiveStream liveStream)
        {
            if (liveStream.Channel.ChannelType == ChannelType.Television)
            {
                try
                {
                    if (_liveStreams.ContainsKey(liveStream.RtspUrl))
                    {
                        IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];
                        TvServerPlugin.TvController_StopGrabbingTeletext(tve3User);
                    }
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Error, ex.Message);
                }
            }
        }

        public override bool IsGrabbingTeletext(LiveStream liveStream)
        {
            if (liveStream.Channel.ChannelType == ChannelType.Television)
            {
                try
                {
                    if (_liveStreams.ContainsKey(liveStream.RtspUrl))
                    {
                        IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];
                        return TvServerPlugin.TvController_IsGrabbingTeletext(tve3User);
                    }
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Error, ex.Message);
                }
            }
            return false;
        }

        public override byte[] GetTeletextPageBytes(LiveStream liveStream, int pageNumber, int subPageNumber, out int subPageCount)
        {
            subPageCount = 0;
            if (liveStream.Channel.ChannelType == ChannelType.Television)
            {
                try
                {
                    if (_liveStreams.ContainsKey(liveStream.RtspUrl))
                    {
                        IUser tve3User = _liveStreamUsers[liveStream.RtspUrl];
                        byte[] result = TvServerPlugin.TvController_GetTeletextPage(tve3User, pageNumber, subPageNumber);
                        if (result != null)
                        {
                            subPageCount = TvServerPlugin.TvController_SubPageCount(tve3User, pageNumber);
                        }
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log(TraceEventType.Error, ex.Message);
                }
            }
            return null;
        }

        #endregion

        protected override void OnWriteLog(TraceEventType severity, string message)
        {
            if (severity == TraceEventType.Error)
            {
                TvLibrary.Log.Log.Error(message);
            }
            else
            {
                TvLibrary.Log.Log.Write(message);
            }
        }

        #region Private Methods

        private static string GetTsBufferFile(IUser tve3User)
        {
            string tsBufferFile = TvServerPlugin.TvController_TimeShiftFileName(ref tve3User);
            string uncTimeshiftFolder = Common.ShareExplorer.GetUncPathForLocalPath(Path.GetDirectoryName(tsBufferFile));
            if (!String.IsNullOrEmpty(uncTimeshiftFolder))
            {
                return Path.Combine(uncTimeshiftFolder, Path.GetFileName(tsBufferFile));
            }
            return null;
        }

        private List<TvDatabase.Card> GetCardsForChannel(Channel channel, out TvDatabase.Channel mpChannel)
        {
            List<TvDatabase.Card> availableCards = new List<TvDatabase.Card>();

            mpChannel = GetLinkedMediaPortalChannel(channel.ChannelType, channel.ChannelId, channel.DisplayName);
            if (mpChannel != null)
            {
                //
                // Now build a list of all available cards by getting a list of all the cards that
                // have this channel mapped.
                //
                IList<TvDatabase.ChannelMap> channelCardMap = mpChannel.ReferringChannelMap();
                IList<TvDatabase.Card> allCards = TvDatabase.Card.ListAll();
                foreach (TvDatabase.Card card in allCards)
                {
                    if (!availableCards.Contains(card)
                        && card.Enabled)
                    {
                        TvDatabase.ChannelMap channelMap = null;
                        foreach (TvDatabase.ChannelMap map in channelCardMap)
                        {
                            if (map.IdCard == card.IdCard
                                && !map.EpgOnly)
                            {
                                channelMap = map;
                                break;
                            }
                        }
                        if (channelMap != null)
                        {
                            availableCards.Add(card);
                        }
                    }
                }
            }
            return availableCards;
        }

        private static string GetCardId(TvDatabase.Card card)
        {
            return card.IdCard.ToString(CultureInfo.InvariantCulture);
        }

        private static TvDatabase.Card GetCardByCardId(string cardId)
        {
            IList<TvDatabase.Card> allCards = TvDatabase.Card.ListAll();
            foreach (TvDatabase.Card card in allCards)
            {
                if (GetCardId(card) == cardId)
                {
                    return card;
                }
            }
            return null;
        }

        private static TvDatabase.Channel GetLinkedMediaPortalChannel(ChannelType channelType, Guid channelId, string displayName)
        {
            TvDatabase.Channel allocatedChannel = null;
            LinkedMediaPortalChannel linkedChannel = ChannelLinks.GetLinkedMediaPortalChannel(channelType, channelId, displayName);
            if (linkedChannel != null)
            {
                allocatedChannel = TvDatabase.Channel.Retrieve(linkedChannel.Id);
            }
            return allocatedChannel;
        }

        #endregion

        #region RTSP

#if USE_ARGUS_RTSP
        private bool RegisterApplicationInRtspServer(bool logError)
        {
            bool succeeded = StreamingServerHost.Instance.RegisterApplication(_rtspApplicationName);
            if (!succeeded && logError)
            {
                TvLibrary.Log.Log.Error("Couldn't reach RTSP-server to register {0}.", _rtspApplicationName);
            }
            return succeeded;
        }

        private string StartRtspStream(string filePath, string rtspUrlSuffix)
        {
            string streamUri = null;
            // Ensure RTSP-Server is up and running, and we are registered
            if (RegisterApplicationInRtspServer(false))
            {
                try
                {
                    using (StreamingServiceAgent streamingServiceAgent = new StreamingServiceAgent())
                    {
                        streamUri = streamingServiceAgent.AddStream(filePath, rtspUrlSuffix);
                    }
                }
                catch (Exception ex)
                {
                    TvLibrary.Log.Log.Error(ex.Message);
                }
            }
            return streamUri;
        }

        private void StopRtspStream(LiveStream stream)
        {
            // Ensure RTSP-Server is up and running, and we are registered
            if (RegisterApplicationInRtspServer(false))
            {
                try
                {
                    using (StreamingServiceAgent streamingServiceAgent = new StreamingServiceAgent())
                    {
                        // "rtsp://MEDIASERVER:554/Argus-Card2-2"
                        int index = stream.RtspUrl.LastIndexOf('/') + 1;
                        string rtspUrlSuffix = stream.RtspUrl.Substring(index);
                        streamingServiceAgent.RemoveStream(rtspUrlSuffix);
                        //Logger.Info("Stopped RTSP-Stream " + rtspUrlSuffix);
                    }
                }
                catch (Exception ex)
                {
                    TvLibrary.Log.Log.Error(ex.Message);
                }
            }
        }
#endif

        #endregion
    }
}
