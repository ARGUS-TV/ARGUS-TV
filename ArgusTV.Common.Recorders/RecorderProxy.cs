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
using System.Collections.Generic;
using RestSharp;
using ArgusTV.DataContracts;
using ArgusTV.DataContracts.Tuning;
using ArgusTV.Common.Recorders.Utility;

namespace ArgusTV.Common.Recorders
{
    public class RecorderProxy : RestProxyBase
    {
        public RecorderProxy(string recorderBaseUrl)
            : base (recorderBaseUrl)
        {
        }

        /// <summary>
        /// Ping the Recorder service.
        /// </summary>
        /// <returns>Returns true if the API on the recorder is correct, and a list of the MAC addresses of the Recorder machine.</returns>
        public bool Ping(out List<string> macAddresses)
        {
            var request = NewRequest("/Ping", Method.GET);
            var data = Execute<PingResult>(request);
            macAddresses = data.macAddresses;
            return data.result == ArgusTV.DataContracts.Constants.RecorderApiVersion;
        }

        private class PingResult
        {
            public int result { get; set; }
            public List<string> macAddresses { get; set; }
        }

        /// <summary>
        /// Ask the recorder to keep its machine alive.
        /// </summary>
        public void KeepAlive()
        {
            var request = NewRequest("/KeepAlive", Method.PUT);
            ExecuteAsync(request);
        }

        /// <summary>
        /// Ask the recorder to initialize by registering itself over the Recorder callback service's /RegisterRecorder method.
        /// </summary>
        /// <param name="recorderId">The unique ID of this recorder.</param>
        /// <param name="schedulerBaseUrl">The callback URL for the Recorder to communicate with the Scheduler.</param>
        public void Initialize(Guid recorderId, string schedulerBaseUrl)
        {
            var request = NewRequest("/Initialize/{recorderId}", Method.PUT);
            request.AddParameter("recorderId", recorderId, ParameterType.UrlSegment);
            request.AddBody(new
            {
                schedulerBaseUrl = schedulerBaseUrl
            });
            Execute(request);
        }

        /// <summary>
        /// Ask the recorder to allocate a (virtual) card for a channel.  The previously allocated
        /// cards are also passed in, so the implementation must take into account that these cards
        /// are no longer available when this call is made.  Note that the implementation must *not*
        /// worry about cards being actually free at the moment the call is made!  This is purely a
        /// theoretical calculation that is used by ARGUS TV to manage its upcoming recordings.
        /// </summary>
        /// <param name="channel">The channel to allocate.</param>
        /// <param name="alreadyAllocated">All previously allocated channels/cards.</param>
        /// <param name="useReversePriority">Use reverse cards priority to avoid conflicts with live streaming.</param>
        /// <returns>The unique card ID of the card that can record this channel, or null if no free card was found.</returns>
        public string AllocateCard(Channel channel, IList<CardChannelAllocation> alreadyAllocated, bool useReversePriority)
        {
            var request = NewRequest("/AllocateCard", Method.PUT);
            request.AddBody(new
            {
                channel = channel,
                alreadyAllocated = alreadyAllocated,
                useReversePriority = useReversePriority
            });
            return ExecuteResult<string>(request);
        }

        /// <summary>
        /// Tell the recorder to actually start a recording on the given card.  The implementation
        /// must call /Recording/New on the Recorder callback service when the recording actually
        /// starts.  If the recording can't start for some reason, StartRecordingFailed() must be called.
        /// In case the recording ends (prematurely or on time) /Recording/End must be called.  IMPORTANT:
        /// If the suggested relative path and filename was used the recorder should
        /// return 'false' to /Recording/End's 'okToMoveFile'!
        /// </summary>
        /// <param name="schedulerBaseUrl">The callback URL for the Recorder to communicate with the Scheduler.</param>
        /// <param name="channelAllocation">The card allocation for the channel.</param>
        /// <param name="startTimeUtc">The actual time to start the recording (UTC).</param>
        /// <param name="stopTimeUtc">The actual time to stop the recording (UTC).</param>
        /// <param name="recordingProgram">The program to record.</param>
        /// <param name="suggestedBaseFileName">The suggested relative path and filename (without extension) of the recording file.</param>
        /// <returns>A boolean indicating the recording was initiated succesfully.</returns>
        public bool StartRecording(string schedulerBaseUrl, CardChannelAllocation channelAllocation, DateTime startTimeUtc, DateTime stopTimeUtc, UpcomingProgram recordingProgram, string suggestedBaseFileName)
        {
            var request = NewRequest("/Recording/Start", Method.POST);
            request.AddBody(new
            {
                schedulerBaseUrl = schedulerBaseUrl,
                channelAllocation = channelAllocation,
                startTimeUtc = startTimeUtc,
                stopTimeUtc = stopTimeUtc,
                recordingProgram = recordingProgram,
                suggestedBaseFileName = suggestedBaseFileName
            });
            return ExecuteResult<bool>(request);
        }

        /// <summary>
        /// Validate a recording is still running, and update its actual stop time.
        /// </summary>
        /// <param name="channelAllocation">The card allocation for the channel.</param>
        /// <param name="recordingProgram">The program being recorded.</param>
        /// <param name="stopTimeUtc">The up-to-date stop time (UTC).</param>
        /// <returns>True if the recording was still running (and its stop time was succesfully updated), false if there was a problem or the recording is not running.</returns>
        public bool ValidateAndUpdateRecording(CardChannelAllocation channelAllocation, UpcomingProgram recordingProgram, DateTime stopTimeUtc)
        {
            var request = NewRequest("/Recording/ValidateAndUpdate", Method.PUT);
            request.AddBody(new
            {
                channelAllocation = channelAllocation,
                recordingProgram = recordingProgram,
                stopTimeUtc = stopTimeUtc
            });
            return ExecuteResult<bool>(request);
        }

        /// <summary>
        /// Tell the recorder to abort the recording of a program.  The implementation must call
        /// /Recording/End on the Recorder callback service.
        /// </summary>
        /// <param name="schedulerBaseUrl">The callback URL for the Recorder to communicate with the Scheduler.</param>
        /// <param name="recordingProgram">The program that is being recorded.</param>
        /// <returns>True if the recording was found and aborted.</returns>
        public bool AbortRecording(string schedulerBaseUrl, UpcomingProgram recordingProgram)
        {
            var request = NewRequest("/Recording/Abort", Method.PUT);
            request.AddBody(new
            {
                schedulerBaseUrl = schedulerBaseUrl,
                recordingProgram = recordingProgram
            });
            return ExecuteResult<bool>(request);
        }

        /// <summary>
        /// Retrieves the recording shares of the recorder.
        /// </summary>
        public List<string> GetRecordingShares()
        {
            var request = NewRequest("/RecordingShares", Method.GET);
            return ExecuteResult<List<string>>(request);
        }

        /// <summary>
        /// Retrieves the timeshift shares of the recorder.
        /// </summary>
        public List<string> GetTimeshiftShares()
        {
            var request = NewRequest("/TimeshiftShares", Method.GET);
            return ExecuteResult<List<string>>(request);
        }

        #region Live Streaming

        /// <summary>
        /// Tune to a channel, and get a live stream to that channel.
        /// </summary>
        /// <param name="channel">The channel to tune to.</param>
        /// <param name="upcomingRecordingAllocation">The allocation if the next upcoming recording, or null if there isn't one.</param>
        /// <param name="liveStream">Reference to a live stream that is either existing or null for a new one.</param>
        /// <returns>A LiveStreamResult value to indicate success or failure.</returns>
        public LiveStreamResult TuneLiveStream(Channel channel, CardChannelAllocation upcomingRecordingAllocation, ref LiveStream liveStream)
        {
            var request = NewRequest("/Live/Tune", Method.POST);
            request.AddBody(new
            {
                channel = channel,
                upcomingRecordingAllocation = upcomingRecordingAllocation,
                stream = liveStream
            });
            var data = Execute<TuneLiveStreamResult>(request);
            liveStream = data.stream;
            return data.result;
        }

        /// <summary>
        /// Tell the recorder we are still showing this stream and to keep it alive. Call this every 30 seconds or so.
        /// </summary>
        /// <param name="liveStream">The live stream that is stil in use.</param>
        /// <returns>True if the live stream is still running, false otherwise.</returns>
        public bool KeepLiveStreamAlive(LiveStream liveStream)
        {
            var request = NewRequest("/Live/KeepAlive", Method.PUT);
            request.AddBody(liveStream);
            return ExecuteResult<bool>(request);
        }

        /// <summary>
        /// Stop the live stream (if it is found and belongs to the recorder).
        /// </summary>
        /// <param name="liveStream">The live stream to stop.</param>
        public void StopLiveStream(LiveStream liveStream)
        {
            var request = NewRequest("/Live/Stop", Method.PUT);
            request.AddBody(liveStream);
            Execute(request);
        }

        /// <summary>
        /// Get all live streams.
        /// </summary>
        /// <returns>An array containing zero or more live streams.</returns>
        public List<LiveStream> GetLiveStreams()
        {
            var request = NewRequest("/LiveStreams", Method.GET);
            return ExecuteResult<List<LiveStream>>(request);
        }

        /// <summary>
        /// Get the live tuning state of a number of channels.
        /// </summary>
        /// <param name="channels">The channels to get the live state from.</param>
        /// <param name="liveStream">The live stream you want to be ignored (since it's yours), or null.</param>
        /// <returns>An array with all the live states for the given channels.</returns>
        public List<ChannelLiveState> GetChannelsLiveState(IList<Channel> channels, LiveStream liveStream)
        {
            var request = NewRequest("/ChannelsLiveState", Method.PUT);
            request.AddBody(new
            {
                channels = ChannelsAsArgument(channels),
                stream = liveStream
            });
            return ExecuteResult<List<ChannelLiveState>>(request);
        }

        /// <summary>
        /// Ask the recorder for the give live stream's tuning details (if possible).
        /// </summary>
        /// <param name="liveStream">The active live stream.</param>
        /// <returns>The service tuning details, or null if none are available.</returns>
        public ServiceTuning GetLiveStreamTuningDetails(LiveStream liveStream)
        {
            var request = NewRequest("/Live/TuningDetails", Method.PUT);
            request.AddBody(liveStream);
            return ExecuteResult<ServiceTuning>(request);
        }

        #endregion

        #region Teletext

        /// <summary>
        /// Ask the recorder whether the given liveStream has teletext.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <returns>True if teletext is present.</returns>
        public bool HasTeletext(LiveStream liveStream)
        {
            var request = NewRequest("/Live/HasTeletext", Method.PUT);
            request.AddBody(liveStream);
            return ExecuteResult<bool>(request);
        }

        /// <summary>
        /// Tell the recorder to start grabbing teletext for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        public void StartGrabbingTeletext(LiveStream liveStream)
        {
            var request = NewRequest("/Live/Teletext/StartGrabbing", Method.PUT);
            request.AddBody(liveStream);
            Execute(request);
        }

        /// <summary>
        /// Tell the recorder to stop grabbing teletext for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        public void StopGrabbingTeletext(LiveStream liveStream)
        {
            var request = NewRequest("/Live/Teletext/StopGrabbing", Method.PUT);
            request.AddBody(liveStream);
            Execute(request);
        }

        /// <summary>
        /// Ask the recorder whether it is grabbing teletext for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <returns>True if the recorder is grabbing teletext.</returns>
        public bool IsGrabbingTeletext(LiveStream liveStream)
        {
            var request = NewRequest("/Live/Teletext/IsGrabbing", Method.PUT);
            request.AddBody(liveStream);
            return ExecuteResult<bool>(request);
        }

        /// <summary>
        /// Request a teletext page/subpage from the recorder for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <param name="pageNumber">The teletext page number.</param>
        /// <param name="subPageNumber">The teletext subpage number.</param>
        /// <param name="subPageCount">The total number of subpages of this page.</param>
        /// <returns>The requested page content, or null if the page was not ready yet.</returns>
        public byte[] GetTeletextPageBytes(LiveStream liveStream, int pageNumber, int subPageNumber, out int subPageCount)
        {
            var request = NewRequest("/Live/Teletext/GetPage/{pageNumber}/{subPageNumber}", Method.PUT);
            request.AddParameter("pageNumber", pageNumber, ParameterType.UrlSegment);
            request.AddParameter("subPageNumber", subPageNumber, ParameterType.UrlSegment);
            request.AddBody(liveStream);
            var data = Execute<GetTeletextPageBytesResult>(request);
            subPageCount = data.subPageCount;
            return Convert.FromBase64String(data.result);
        }

        #endregion

        #region Helper Classes

        private class TuneLiveStreamResult
        {
            public LiveStreamResult result { get; set; }
            public LiveStream stream { get; set; }
        }

        private class GetTeletextPageBytesResult
        {
            public string result { get; set; }
            public int subPageCount { get; set; }
        }

        private static List<object> ChannelsAsArgument(IEnumerable<Channel> channels)
        {
            List<object> result = new List<object>();
            foreach(var channel in channels)
            {
                result.Add(channel);
            }
            return result;
        }

        #endregion
    }
}
