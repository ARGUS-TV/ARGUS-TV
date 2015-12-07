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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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
        public async Task<PingResult> Ping()
        {
            var request = NewRequest(HttpMethod.Get, "Ping");
            var data = await ExecuteAsync<InternalPingResult>(request).ConfigureAwait(false);
            return new PingResult
            {
                Success = data.result == ArgusTV.DataContracts.Constants.RecorderApiVersion,
                MacAddresses = data.macAddresses
            };
        }

        private class InternalPingResult
        {
            public int result { get; set; }
            public List<string> macAddresses { get; set; }
        }

        /// <summary>
        /// Ask the recorder to keep its machine alive.
        /// </summary>
        public void KeepAlive()
        {
            var request = NewRequest(HttpMethod.Put, "KeepAlive");
            ExecuteAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Ask the recorder to initialize by registering itself over the Recorder callback service's /RegisterRecorder method.
        /// </summary>
        /// <param name="recorderId">The unique ID of this recorder.</param>
        /// <param name="schedulerBaseUrl">The callback URL for the Recorder to communicate with the Scheduler.</param>
        public async Task Initialize(Guid recorderId, string schedulerBaseUrl)
        {
            var request = NewRequest(HttpMethod.Put, "Initialize/{0}", recorderId);
            request.AddBody(new
            {
                schedulerBaseUrl = schedulerBaseUrl
            });
            await ExecuteAsync(request).ConfigureAwait(false);
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
        public async Task<string> AllocateCard(Channel channel, IList<CardChannelAllocation> alreadyAllocated, bool useReversePriority)
        {
            var request = NewRequest(HttpMethod.Put, "AllocateCard");
            request.AddBody(new
            {
                channel = channel,
                alreadyAllocated = alreadyAllocated,
                useReversePriority = useReversePriority
            });
            return await ExecuteResult<string>(request).ConfigureAwait(false);
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
        public async Task<bool> StartRecording(string schedulerBaseUrl, CardChannelAllocation channelAllocation, DateTime startTimeUtc, DateTime stopTimeUtc, UpcomingProgram recordingProgram, string suggestedBaseFileName)
        {
            var request = NewRequest(HttpMethod.Post, "Recording/Start");
            request.AddBody(new
            {
                schedulerBaseUrl = schedulerBaseUrl,
                channelAllocation = channelAllocation,
                startTimeUtc = startTimeUtc,
                stopTimeUtc = stopTimeUtc,
                recordingProgram = recordingProgram,
                suggestedBaseFileName = suggestedBaseFileName
            });
            return await ExecuteResult<bool>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Validate a recording is still running, and update its actual stop time.
        /// </summary>
        /// <param name="channelAllocation">The card allocation for the channel.</param>
        /// <param name="recordingProgram">The program being recorded.</param>
        /// <param name="stopTimeUtc">The up-to-date stop time (UTC).</param>
        /// <returns>True if the recording was still running (and its stop time was succesfully updated), false if there was a problem or the recording is not running.</returns>
        public async Task<bool> ValidateAndUpdateRecording(CardChannelAllocation channelAllocation, UpcomingProgram recordingProgram, DateTime stopTimeUtc)
        {
            var request = NewRequest(HttpMethod.Put, "Recording/ValidateAndUpdate");
            request.AddBody(new
            {
                channelAllocation = channelAllocation,
                recordingProgram = recordingProgram,
                stopTimeUtc = stopTimeUtc
            });
            return await ExecuteResult<bool>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Tell the recorder to abort the recording of a program.  The implementation must call
        /// /Recording/End on the Recorder callback service.
        /// </summary>
        /// <param name="schedulerBaseUrl">The callback URL for the Recorder to communicate with the Scheduler.</param>
        /// <param name="recordingProgram">The program that is being recorded.</param>
        /// <returns>True if the recording was found and aborted.</returns>
        public async Task<bool> AbortRecording(string schedulerBaseUrl, UpcomingProgram recordingProgram)
        {
            var request = NewRequest(HttpMethod.Put, "Recording/Abort");
            request.AddBody(new
            {
                schedulerBaseUrl = schedulerBaseUrl,
                recordingProgram = recordingProgram
            });
            return await ExecuteResult<bool>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the recording shares of the recorder.
        /// </summary>
        public async Task<List<string>> GetRecordingShares()
        {
            var request = NewRequest(HttpMethod.Get, "RecordingShares");
            return await ExecuteResult<List<string>>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the timeshift shares of the recorder.
        /// </summary>
        public async Task<List<string>> GetTimeshiftShares()
        {
            var request = NewRequest(HttpMethod.Get, "TimeshiftShares");
            return await ExecuteResult<List<string>>(request).ConfigureAwait(false);
        }

        #region Live Streaming

        /// <summary>
        /// Tune to a channel, and get a live stream to that channel.
        /// </summary>
        /// <param name="channel">The channel to tune to.</param>
        /// <param name="upcomingRecordingAllocation">The allocation if the next upcoming recording, or null if there isn't one.</param>
        /// <param name="liveStream">A live stream that is either existing or null for a new one.</param>
        /// <param name="mode">The streaming mode (default is RTSP).</param>
        /// <returns>A LiveStreamResult value to indicate success or failure, and a new or updated live stream.</returns>
        public async Task<TuneLiveStreamResult> TuneLiveStream(Channel channel, CardChannelAllocation upcomingRecordingAllocation, LiveStream liveStream, StreamingMode mode)
        {
            var request = NewRequest(HttpMethod.Post, "Live/Tune?mode={0}", (int)mode);
            request.AddBody(new
            {
                channel = channel,
                upcomingRecordingAllocation = upcomingRecordingAllocation,
                stream = liveStream
            });
            var data = await ExecuteAsync<InternalTuneLiveStreamResult>(request).ConfigureAwait(false);
            return new TuneLiveStreamResult
            {
                Result = data.result,
                Stream = data.stream
            };
        }

        /// <summary>
        /// Tell the recorder we are still showing this stream and to keep it alive. Call this every 30 seconds or so.
        /// </summary>
        /// <param name="liveStream">The live stream that is stil in use.</param>
        /// <returns>True if the live stream is still running, false otherwise.</returns>
        public async Task<bool> KeepLiveStreamAlive(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Put, "Live/KeepAlive");
            request.AddBody(liveStream);
            return await ExecuteResult<bool>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Stop the live stream (if it is found and belongs to the recorder).
        /// </summary>
        /// <param name="liveStream">The live stream to stop.</param>
        public async Task StopLiveStream(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Put, "Live/Stop");
            request.AddBody(liveStream);
            await ExecuteAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Get all live streams.
        /// </summary>
        /// <returns>An array containing zero or more live streams.</returns>
        public async Task<List<LiveStream>> GetLiveStreams()
        {
            var request = NewRequest(HttpMethod.Get, "LiveStreams");
            return await ExecuteResult<List<LiveStream>>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Get the live tuning state of a number of channels.
        /// </summary>
        /// <param name="channels">The channels to get the live state from.</param>
        /// <param name="liveStream">The live stream you want to be ignored (since it's yours), or null.</param>
        /// <returns>An array with all the live states for the given channels.</returns>
        public async Task<List<ChannelLiveState>> GetChannelsLiveState(IList<Channel> channels, LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Put, "ChannelsLiveState");
            request.AddBody(new
            {
                channels = ChannelsAsArgument(channels),
                stream = liveStream
            });
            return await ExecuteResult<List<ChannelLiveState>>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Ask the recorder for the give live stream's tuning details (if possible).
        /// </summary>
        /// <param name="liveStream">The active live stream.</param>
        /// <returns>The service tuning details, or null if none are available.</returns>
        public async Task<ServiceTuning> GetLiveStreamTuningDetails(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Put, "Live/TuningDetails");
            request.AddBody(liveStream);
            return await ExecuteResult<ServiceTuning>(request).ConfigureAwait(false);
        }

        #endregion

        #region Teletext

        /// <summary>
        /// Ask the recorder whether the given liveStream has teletext.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <returns>True if teletext is present.</returns>
        public async Task<bool> HasTeletext(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Put, "Live/HasTeletext");
            request.AddBody(liveStream);
            return await ExecuteResult<bool>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Tell the recorder to start grabbing teletext for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        public async Task StartGrabbingTeletext(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Put, "Live/Teletext/StartGrabbing");
            request.AddBody(liveStream);
            await ExecuteAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Tell the recorder to stop grabbing teletext for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        public async Task StopGrabbingTeletext(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Put, "Live/Teletext/StopGrabbing");
            request.AddBody(liveStream);
            await ExecuteAsync(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Ask the recorder whether it is grabbing teletext for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <returns>True if the recorder is grabbing teletext.</returns>
        public async Task<bool> IsGrabbingTeletext(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Put, "Live/Teletext/IsGrabbing");
            request.AddBody(liveStream);
            return await ExecuteResult<bool>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// Request a teletext page/subpage from the recorder for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <param name="pageNumber">The teletext page number.</param>
        /// <param name="subPageNumber">The teletext subpage number.</param>
        /// <returns>The requested page content, or null if the page was not ready yet.</returns>
        public async Task<GetTeletextPageBytesResult> GetTeletextPageBytes(LiveStream liveStream, int pageNumber, int subPageNumber)
        {
            var request = NewRequest(HttpMethod.Put, "Live/Teletext/GetPage/{0}/{1}", pageNumber, subPageNumber);
            request.AddBody(liveStream);
            var data = await ExecuteAsync<InternalGetTeletextPageBytesResult>(request).ConfigureAwait(false);
            return new GetTeletextPageBytesResult
            {
                Bytes = Convert.FromBase64String(data.result),
                SubPageCount = data.subPageCount
            };
        }

        #endregion

        #region Helper Classes

        private class InternalTuneLiveStreamResult
        {
            public LiveStreamResult result { get; set; }
            public LiveStream stream { get; set; }
        }

        private class InternalGetTeletextPageBytesResult
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

    /// <summary>
    /// Result of Ping.
    /// </summary>
    public class PingResult
    {
        /// <summary>
        /// Returns true if the API on the recorder is correct, and a list of the MAC addresses of the Recorder machine.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// An array of MAC addresses of the server.
        /// </summary>
        public List<string> MacAddresses { get; set; }
    }

    /// <summary>
    /// Result of TuneLiveStream.
    /// </summary>
    public class TuneLiveStreamResult
    {
        /// <summary>
        /// The result code.
        /// </summary>
        public LiveStreamResult Result { get; set; }

        /// <summary>
        /// A new or updated live stream.
        /// </summary>
        public LiveStream Stream { get; set; }
    }

    /// <summary>
    /// Result of GetTeletextPageBytes.
    /// </summary>
    public class GetTeletextPageBytesResult
    {
        /// <summary>
        /// The requested page content, or null if the page was not ready yet.
        /// </summary>
        public byte[] Bytes { get; set; }

        /// <summary>
        /// The total number of subpages of this page.
        /// </summary>
        public int SubPageCount { get; set; }
    }
}
