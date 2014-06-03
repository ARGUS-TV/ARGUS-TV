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

using ArgusTV.DataContracts;
using ArgusTV.DataContracts.Tuning;

namespace ArgusTV.ServiceProxy
{
    /// <summary>
    /// Service to control/query all aspects of recording and tuning.
    /// </summary>
    public partial class ControlServiceProxy : RestProxyBase
    {
        /// <summary>
        /// Constructs a channel to the service.
        /// </summary>
        internal ControlServiceProxy()
            : base("Control")
        {
        }

        #region Plugin Services

        /// <summary>
        /// Get all configured plugin services.
        /// </summary>
        /// <param name="activeOnly">Set to true to only receive active plugins.</param>
        /// <returns>An array containing zero or more plugin services.</returns>
        public List<PluginService> GetAllPluginServices(bool activeOnly = true)
        {
            var request = NewRequest(HttpMethod.Get, "PluginServices");
            if (!activeOnly)
            {
                request.AddParameter("activeOnly", false);
            }
            return Execute<List<PluginService>>(request);
        }

        /// <summary>
        /// Save a new or modified plugin service.  A new plugin service is recognized by a Guid.Empty ID.
        /// </summary>
        /// <param name="pluginService">The plugin service to save.</param>
        /// <returns>The saved plugin service.</returns>
        public PluginService SavePluginService(PluginService pluginService)
        {
            var request = NewRequest(HttpMethod.Post, "SavePluginService");
            request.AddBody(pluginService);
            return Execute<PluginService>(request);
        }

        /// <summary>
        /// Delete a plugin service.
        /// </summary>
        /// <param name="pluginServiceId">The ID of the plugin service to delete.</param>
        public void DeletePluginService(Guid pluginServiceId)
        {
            var request = NewRequest(HttpMethod.Post, "DeletePluginService/{0}", pluginServiceId);
            Execute(request);
        }

        /// <summary>
        /// Ask ARGUS TV to test the connection to a recorder by pinging it.
        /// </summary>
        /// <param name="pluginService">The plugin service to ping.</param>
        public void PingPluginService(PluginService pluginService)
        {
            var request = NewRequest(HttpMethod.Post, "PingPluginService");
            request.AddBody(pluginService);
            Execute(request);
        }

        /// <summary>
        /// Check if the ARGUS TV service has the needed access rights on the recording shares of the given pluginService.
        /// </summary>
        /// <param name="pluginService">The pluginService.</param>
        /// <returns>A list of RecordingShareAccessibilityInfos.</returns>
        public List<RecordingShareAccessibilityInfo> AreRecordingSharesAccessible(PluginService pluginService)
        {
            var request = NewRequest(HttpMethod.Post, "AreRecordingSharesAccessible");
            request.AddBody(pluginService);
            return Execute<List<RecordingShareAccessibilityInfo>>(request);
        }

        /// <summary>
        /// Get information (free disk space) from all recording disks.
        /// </summary>
        /// <returns>A RecordingDisksInfo entity with all disk(s) information.</returns>
        public RecordingDisksInfo GetRecordingDisksInfo()
        {
            var request = NewRequest(HttpMethod.Get, "RecordingDisksInfo");
            return Execute<RecordingDisksInfo>(request);
        }

        /// <summary>
        /// Get all the recording shares configured for the current recorder(s).
        /// </summary>
        /// <returns>A list containing zero or more recording shares.</returns>
        public List<string> GetRecordingShares()
        {
            var request = NewRequest(HttpMethod.Get, "RecordingShares");
            return Execute<List<string>>(request);
        }

        #endregion

        #region Recordings

        /// <summary>
        /// Get all recordings for the given criteria. You must specificy at least one criterium other than the channel type.
        /// </summary>
        /// <param name="channelType">The channel-type of the recordings.</param>
        /// <param name="scheduleId">The schedule ID of the recordings, or null.</param>
        /// <param name="programTitle">The program title of the recordings, or null.</param>
        /// <param name="category">The category of the recordings, or null.</param>
        /// <param name="channelId">The channel ID of the recordings, or null.</param>
        /// <param name="includeNonExisting">If true also return recording entries for which the recording file is missing.</param>
        /// <returns>Returns a list of zero or more recordings.</returns>
        public List<Recording> GetFullRecordings(ChannelType channelType, Guid? scheduleId, string programTitle, string category, Guid? channelId, bool includeNonExisting = false)
        {
            var request = NewRequest(HttpMethod.Post, "GetFullRecordings/{0}", channelType);
            request.AddBody(new
            {
                ScheduleId = scheduleId,
                ProgramTitle = programTitle,
                Category = category,
                ChannelId = channelId
            });
            return Execute<List<Recording>>(request);
        }

        /// <summary>
        /// Get all recording groups based on the recording group-mode.
        /// </summary>
        /// <param name="channelType">The channel-type of the recordings.</param>
        /// <param name="recordingGroupMode">The recording group-mode.</param>
        /// <returns>An array of zero or more recording schedule-groups.</returns>        
        public List<RecordingGroup> GetAllRecordingGroups(ChannelType channelType, RecordingGroupMode recordingGroupMode)
        {
            var request = NewRequest(HttpMethod.Get, "RecordingGroups/{0}/{1}", channelType, recordingGroupMode);
            return Execute<List<RecordingGroup>>(request);
        }

        /// <summary>
        /// Get all recordings for the given original schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <param name="includeNonExisting">If true also return recording entries for which the recording file is missing.</param>
        /// <returns>An array of zero or more recordings.</returns>
        public List<RecordingSummary> GetRecordingsForSchedule(Guid scheduleId, bool includeNonExisting = false)
        {
            var request = NewRequest(HttpMethod.Get, "RecordingsForSchedule/{0}", scheduleId);
            if (includeNonExisting)
            {
                request.AddParameter("includeNonExisting", true);
            }
            return Execute<List<RecordingSummary>>(request);
        }

        /// <summary>
        /// Get all recordings for the given program title.
        /// </summary>
        /// <param name="channelType">The channel-type of the recordings.</param>
        /// <param name="programTitle">The program title.</param>
        /// <param name="includeNonExisting">If true also return recording entries for which the recording file is missing.</param>
        /// <returns>An array of zero or more recordings.</returns>  
        public List<RecordingSummary> GetRecordingsForProgramTitle(ChannelType channelType, string programTitle, bool includeNonExisting = false)
        {
            var request = NewRequest(HttpMethod.Post, "RecordingsForProgramTitle/{0}", channelType);
            if (includeNonExisting)
            {
                request.AddParameter("includeNonExisting", true);
            }
            request.AddBody(new
            {
                ProgramTitle = programTitle
            });
            return Execute<List<RecordingSummary>>(request);
        }

        /// <summary>
        /// Get all recordings for the given program titles.
        /// </summary>
        /// <param name="channelType">The channel-type of the recordings.</param>
        /// <param name="programTitles">A list of program titles.</param>
        /// <param name="includeNonExisting">If true also return recording entries for which the recording file is missing.</param>
        /// <returns>A list of a list of zero or more recordings, so a list per given title.</returns>
        public List<List<RecordingSummary>> GetRecordingsForProgramTitles(ChannelType channelType, List<string> programTitles, bool includeNonExisting = false)
        {
            var request = NewRequest(HttpMethod.Post, "RecordingsForProgramTitles/{0}", channelType);
            if (includeNonExisting)
            {
                request.AddParameter("includeNonExisting", true);
            }
            request.AddBody(new
            {
                ProgramTitles = programTitles
            });
            return Execute<List<List<RecordingSummary>>>(request);
        }

        /// <summary>
        /// Get all recordings for the given program category.
        /// </summary>
        /// <param name="channelType">The channel-type of the recordings.</param>
        /// <param name="category">The program category.</param>
        /// <param name="includeNonExisting">If true also return recording entries for which the recording file is missing.</param>
        /// <returns>An array of zero or more recordings.</returns>  
        public List<RecordingSummary> GetRecordingsForCategory(ChannelType channelType, string category, bool includeNonExisting = false)
        {
            var request = NewRequest(HttpMethod.Post, "RecordingsForCategory/{0}", channelType);
            if (includeNonExisting)
            {
                request.AddParameter("includeNonExisting", true);
            }
            request.AddBody(new
            {
                Category = category
            });
            return Execute<List<RecordingSummary>>(request);
        }

        /// <summary>
        /// Get all recordings on the given channel.
        /// </summary>
        /// <param name="channelId">The ID of the channel.</param>
        /// <param name="includeNonExisting">If true also return recording entries for which the recording file is missing.</param>
        /// <returns>An array of zero or more recordings.</returns>
        public List<RecordingSummary> GetRecordingsOnChannel(Guid channelId, bool includeNonExisting = false)
        {
            var request = NewRequest(HttpMethod.Get, "RecordingsOnChannel/{0}", channelId);
            if (includeNonExisting)
            {
                request.AddParameter("includeNonExisting", true);
            }
            return Execute<List<RecordingSummary>>(request);
        }

        /// <summary>
        /// Get the recording associated with the given filename.
        /// </summary>
        /// <param name="recordingFileName">The full path of the recording file.</param>
        /// <returns>The recording, or null if none is found.</returns>
        public Recording GetRecordingByFileName(string recordingFileName)
        {
            var request = NewRequest(HttpMethod.Post, "RecordingByFile");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName
            });
            return Execute<Recording>(request);
        }

        /// <summary>
        /// Delete a recording.
        /// </summary>
        /// <param name="recordingFileName">The filename of the recording.</param>
        /// <param name="deleteRecordingFile">Set to true to also delete the recording file.</param>
        public void DeleteRecording(string recordingFileName, bool deleteRecordingFile = true)
        {
            var request = NewRequest(HttpMethod.Delete, "RecordingByFile");
            if (!deleteRecordingFile)
            {
                request.AddParameter("deleteRecordingFile", false);
            }
            request.AddBody(new
            {
                RecordingFileName = recordingFileName
            });
            Execute(request);
        }

        /// <summary>
        /// Get the recording by its unique ID.
        /// </summary>
        /// <param name="recordingId">The ID of the recording.</param>
        /// <returns>The recording, or null if none is found.</returns>
        public Recording GetRecordingById(Guid recordingId)
        {
            var request = NewRequest(HttpMethod.Get, "RecordingById/{0}", recordingId);
            return Execute<Recording>(request);
        }

        /// <summary>
        /// Get the position (in seconds) to where the recording was last watched.  Or null if it was never watched.
        /// </summary>
        /// <param name="recordingFileName">The filename of the recording.</param>
        /// <returns>The position in seconds or null.</returns>
        public int? GetRecordingLastWatchedPosition(string recordingFileName)
        {
            var request = NewRequest(HttpMethod.Post, "GetRecordingLastWatchedPosition");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName
            });
            var result = Execute<GetRecordingLastWatchedPositionResult>(request);
            return result.LastWatchedPositionSeconds;
        }

        private class GetRecordingLastWatchedPositionResult
        {
            public int? LastWatchedPositionSeconds { get; set; }
        }

        /// <summary>
        /// Set the position (in seconds) to where the recording was last watched, or null to reset the state to never-watched.
        /// </summary>
        /// <param name="recordingFileName">The filename of the recording.</param>
        /// <param name="lastWatchedPositionSeconds">The position in seconds or null.</param>
        public void SetRecordingLastWatchedPosition(string recordingFileName, int? lastWatchedPositionSeconds)
        {
            var request = NewRequest(HttpMethod.Post, "SetRecordingLastWatchedPosition");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName,
                LastWatchedPositionSeconds = lastWatchedPositionSeconds
            });
            Execute(request);
        }

        /// <summary>
        /// Set the number of times a recording was fully watched.
        /// </summary>
        /// <param name="recordingFileName">The filename of the recording.</param>
        /// <param name="fullyWatchedCount">The number of times the recording was fully watched.</param>
        public void SetRecordingFullyWatchedCount(string recordingFileName, int fullyWatchedCount)
        {
            var request = NewRequest(HttpMethod.Post, "SetRecordingFullyWatchedCount");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName,
                FullyWatchedCount = fullyWatchedCount
            });
            Execute(request);
        }

        /// <summary>
        /// Mark a recording as last watched now (can be used by playback engines that don't support the last-watched position).
        /// </summary>
        /// <param name="recordingFileName">The filename of the recording.</param>
        public void SetRecordingLastWatched(string recordingFileName)
        {
            var request = NewRequest(HttpMethod.Post, "SetRecordingWatched");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName
            });
            Execute(request);
        }

        /// <summary>
        /// Set the keep mode and value of an existing recording.
        /// </summary>
        /// <param name="recordingFileName">The filename of the recording.</param>
        /// <param name="keepUntilMode">The keep until mode to use for this recording.</param>
        /// <param name="keepUntilValue">The keep until value to use for this recording, or null if the mode doesn't require a value.</param>
        public void SetRecordingKeepUntil(string recordingFileName, KeepUntilMode keepUntilMode, int? keepUntilValue)
        {
            var request = NewRequest(HttpMethod.Post, "SetRecordingKeepUntil");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName,
                KeepUntilMode = keepUntilMode,
                KeepUntilValue = keepUntilValue
            });
            Execute(request);
        }

        /// <summary>
        /// Get the history of programs that have been recorded by this schedule. This list is the one used
        /// to make the NewEpisodesOnly and NewTitlesOnly rules work.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <returns>An array containing zero or more recorded programs.</returns>
        public List<ScheduleRecordedProgram> GetPreviouslyRecordedHistory(Guid scheduleId)
        {
            var request = NewRequest(HttpMethod.Get, "PreviouslyRecordedHistory/{0}", scheduleId);
            return Execute<List<ScheduleRecordedProgram>>(request);
        }

        /// <summary>
        /// Add an upcoming program to the list of previously recorded programs of its schedule. This list is the one used
        /// to make the NewEpisodesOnly and NewTitlesOnly rules work.
        /// </summary>
        /// <param name="upcomingProgram">The upcoming program to add to the history.</param>
        public void AddToPreviouslyRecordedHistory(UpcomingProgram upcomingProgram)
        {
            var request = NewRequest(HttpMethod.Post, "AddToPreviouslyRecordedHistory");
            request.AddBody(upcomingProgram);
            Execute(request);
        }

        /// <summary>
        /// Remove an upcoming program from the list of previously recorded programs of its schedule. This list is the one used
        /// to make the NewEpisodesOnly and NewTitlesOnly rules work.
        /// </summary>
        /// <param name="upcomingProgram">The upcoming program to remove from the history.</param>
        public void RemoveFromPreviouslyRecordedHistory(UpcomingProgram upcomingProgram)
        {
            var request = NewRequest(HttpMethod.Post, "RemoveFromPreviouslyRecordedHistory");
            request.AddBody(upcomingProgram);
            Execute(request);
        }

        /// <summary>
        /// Import a history of programs that have been recorded by this schedule. This list is the one used
        /// to make the NewEpisodesOnly and NewTitlesOnly rules work.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <param name="history">An array containing zero or more recorded programs.</param>
        public void ImportPreviouslyRecordedHistory(Guid scheduleId, IEnumerable<ScheduleRecordedProgram> history)
        {
            var request = NewRequest(HttpMethod.Post, "ImportPreviouslyRecordedHistory/{0}", scheduleId);
            request.AddBody(history);
            Execute(request);
        }

        /// <summary>
        /// Delete a recorded program from the previously recorded history of its schedule.
        /// </summary>
        /// <param name="scheduleRecordedProgramId">The ID of the recorded program.</param>
        public void DeleteFromPreviouslyRecordedHistory(int scheduleRecordedProgramId)
        {
            var request = NewRequest(HttpMethod.Post, "DeleteFromPreviouslyRecordedHistory/{0}", scheduleRecordedProgramId);
            Execute(request);
        }

        /// <summary>
        /// Delete the previously recorded history of a schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        public void ClearPreviouslyRecordedHistory(Guid scheduleId)
        {
            var request = NewRequest(HttpMethod.Post, "ClearPreviouslyRecordedHistory/{0}", scheduleId);
            Execute(request);
        }

        /// <summary>
        /// Import a new recording into the system. A new RecordingId will be auto-generated,
        /// so this can be left Guid.Empty. If ScheduleId and ChannelId are not known, you may
        /// generate your own (new unique) Guid and pass that in.
        /// </summary>
        /// <param name="recording">The recording to import.</param>
        /// <returns>True if the recording was imported succesfully, false if the recording filename was already imported.</returns>
        public bool ImportRecording(Recording recording)
        {
            var request = NewRequest(HttpMethod.Post, "ImportNewRecording");
            request.AddBody(recording);
            var result = Execute<BooleanResult>(request);
            return result.Result;
        }

        /// <summary>
        /// Change the recording filename for an existing recording. Can be used after moving or transcoding a file.
        /// </summary>
        /// <param name="recordingFileName">The full path of the current recording file (UNC).</param>
        /// <param name="newRecordingFileName">The full path of the new recording file to use (UNC).</param>
        /// <param name="newRecordingStartTime">The new recording start-time (in case of trimming), or null to keep the existing time.</param>
        /// <param name="newRecordingStopTime">The new recording stop-time (in case of trimming), or null to keep the existing time.</param>
        /// <returns>True if the recording was found and modified succesfully, false otherwise.</returns>
        public bool ChangeRecordingFile(string recordingFileName, string newRecordingFileName, DateTime? newRecordingStartTime, DateTime? newRecordingStopTime)
        {
            var request = NewRequest(HttpMethod.Post, "ModifyRecordingFile");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName,
                NewRecordingFileName = newRecordingFileName,
                NewRecordingStartTime = newRecordingStartTime,
                NewRecordingStopTime = newRecordingStopTime
            });
            var result = Execute<BooleanResult>(request);
            return result.Result;
        }

        private class BooleanResult
        {
            public bool Result { get; set; }
        }

        /// <summary>
        /// Schedule a processing command to run on a recording.
        /// </summary>
        /// <param name="recordingId">The unique ID of the recording.</param>
        /// <param name="processingCommandId">The unique ID of the processing command.</param>
        /// <param name="runAtTime">The time and date at which to run the command.</param>
        public void RunProcessingCommandOnRecording(Guid recordingId, Guid processingCommandId, DateTime runAtTime)
        {
            var request = NewRequest(HttpMethod.Post, "RunProcessingCommandOnRecording/{0}/{1}/{2}", recordingId, processingCommandId, runAtTime);
            Execute(request);
        }

        /// <summary>
        /// Get a resized recording thumbnail, if newer than the provided timestamp. The aspect ratio of the
        /// original thumbnail is preserved so the returned image will be potentially either smaller than
        /// the requested size, or centered on a background.  If both width and height are set to 0, the full
        /// size thumbnail will be returned.
        /// </summary>
        /// <param name="recordingId">The unique ID of the recording.</param>
        /// <param name="width">The requested width, 0 to get the width according to the aspect.</param>
        /// <param name="height">The requested height, 0 to get the height according to the aspect.</param>
        /// <param name="argbBackground">The optional RGB color of the background, null to return the scaled image as is.</param>
        /// <param name="modifiedAfterTime">Only return a thumbnail if it is newer than the given timestamp.</param>
        /// <returns>A byte array containing the bytes of a JPG of the resized thumbnail, an empty array if no newer thumbnail was found or null if no thumbnail was found.</returns>
        public byte[] GetRecordingThumbnail(Guid recordingId, int width, int height, int? argbBackground, DateTime modifiedAfterTime)
        {
            var request = NewRequest(HttpMethod.Get, "RecordingThumbnail/{0}/{1}/{2}/{3}", recordingId, width, height, modifiedAfterTime);
            if (argbBackground.HasValue)
            {
                request.AddParameter("argbBackground", argbBackground.Value);
            }
            using (var response = ExecuteRequest(request))
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        return null;

                    case HttpStatusCode.NotModified:
                        return new byte[0];

                    case HttpStatusCode.OK:
                        return response.Content.ReadAsByteArrayAsync().Result;
                }
                throw new ArgusTVException(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Start RTSP streaming of the given recording.
        /// </summary>
        /// <param name="recordingFileName">The filename of the recording.</param>
        /// <returns>The RTSP url of the recording stream.</returns>
        public string StartRecordingStream(string recordingFileName)
        {
            var request = NewRequest(HttpMethod.Post, "StartRecordingRtspStream");
            request.AddBody(new
            {
                RecordingFileName = recordingFileName
            });
            var result = Execute<StartRecordingStreamResult>(request);
            return result.RtspUrl;
        }

        private class StartRecordingStreamResult
        {
            public string RtspUrl { get; set; }
        }

        /// <summary>
        /// Stop RTSP streaming of the given recording.
        /// </summary>
        /// <param name="rtspUrl">The RTSP url of the recording stream.</param>
        public void StopRecordingStream(string rtspUrl)
        {
            var request = NewRequest(HttpMethod.Post, "StopRecordingRtspStream");
            request.AddBody(new
            {
                RtspUrl = rtspUrl
            });
            Execute(request);
        }

        #endregion

        #region Upcoming/Active Recordings

        /// <summary>
        /// Get all upcoming recordings.
        /// </summary>
        /// <param name="filter">Set filter to retrieve recordings and/or cancelled recordings.</param>
        /// <param name="includeActive">Set to true to include upcoming recordings that are currently being recorded.</param>
        /// <returns>An array with zero or more upcoming recordings.</returns>
        public List<UpcomingRecording> GetAllUpcomingRecordings(UpcomingRecordingsFilter filter, bool includeActive = false)
        {
            var request = NewRequest(HttpMethod.Get, "UpcomingRecordings/{0}", (int)filter);
            if (includeActive)
            {
                request.AddParameter("includeActive", true);
            }
            return Execute<List<UpcomingRecording>>(request);
        }

        /// <summary>
        /// Get the first upcoming recording that's not cancelled or unallocated.
        /// </summary>
        /// <param name="includeActive">Set to true to include upcoming recordings that are currently being recorded.</param>
        /// <returns>Null or an upcoming (or active) recording.</returns>
        public UpcomingRecording GetNextUpcomingRecording(bool includeActive)
        {
            var request = NewRequest(HttpMethod.Get, "NextUpcomingRecording");
            if (includeActive)
            {
                request.AddParameter("includeActive", true);
            }
            return Execute<UpcomingRecording>(request);
        }

        /// <summary>
        /// Get upcoming recordings for a specific schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <param name="includeCancelled">Set to true to also retrieve cancelled programs.</param>
        /// <returns>An array with zero or more upcoming recordings.</returns>
        public List<UpcomingRecording> GetUpcomingRecordings(Guid scheduleId, bool includeCancelled = false)
        {
            var request = NewRequest(HttpMethod.Get, "UpcomingRecordingsForSchedule/{0}", scheduleId);
            if (includeCancelled)
            {
                request.AddParameter("includeCancelled", true);
            }
            return Execute<List<UpcomingRecording>>(request);
        }

        /// <summary>
        /// Get the currently active recordings.
        /// </summary>
        /// <returns>An array with zero or more active recordings.</returns>
        public List<ActiveRecording> GetActiveRecordings()
        {
            var request = NewRequest(HttpMethod.Get, "ActiveRecordings");
            return Execute<List<ActiveRecording>>(request);
        }

        /// <summary>
        /// Check if a recording has started or is pending.
        /// </summary>
        /// <param name="upcomingProgramId">The ID of the recording's upcoming program.</param>
        /// <returns>True if the recording has started (or is pending), false if it has not.</returns>
        public bool IsRecordingPendingOrActive(Guid upcomingProgramId)
        {
            var request = NewRequest(HttpMethod.Get, "IsRecordingPendingOrActive/{0}", upcomingProgramId);
            var result = Execute<IsRecordingPendingOrActiveResult>(request);
            return result.IsPendingOrActive;
        }

        private class IsRecordingPendingOrActiveResult
        {
            public bool IsPendingOrActive { get; set; }
        }

        /// <summary>
        /// Abort an active recording.
        /// </summary>
        /// <param name="activeRecording">The active recording to abort.</param>
        public void AbortActiveRecording(ActiveRecording activeRecording)
        {
            var request = NewRequest(HttpMethod.Post, "AbortActiveRecording");
            request.AddBody(activeRecording);
            Execute(request);
        }

        #endregion

        #region Live TV/Radio

        /// <summary>
        /// Tune to a channel, and get a live RTSP stream to that channel.
        /// </summary>
        /// <param name="channel">The channel to tune to.</param>
        /// <param name="liveStream">Reference to a live stream (RTSP) that is either existing or null for a new one.</param>
        /// <returns>A LiveStreamResult value to indicate success or failure.</returns>
        public LiveStreamResult TuneLiveStream(Channel channel, ref LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "TuneLiveStream");
            request.AddBody(new
            {
                Channel = channel,
                LiveStream = liveStream
            });
            var result = Execute<TuneLiveStreamResult>(request);
            liveStream = result.LiveStream;
            return result.LiveStreamResult;
        }

        private class TuneLiveStreamResult
        {
            public LiveStreamResult LiveStreamResult { get; set; }
            public LiveStream LiveStream { get; set; }
        }

        /// <summary>
        /// Stop the live stream.
        /// </summary>
        /// <param name="liveStream">The live stream (RTSP) of the stream to stop.</param>
        public void StopLiveStream(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "StopLiveStream");
            request.AddBody(liveStream);
            Execute(request);
        }

        /// <summary>
        /// Get all live streams.
        /// </summary>
        /// <returns>An array of zero or more live streams.</returns>
        public List<LiveStream> GetLiveStreams()
        {
            var request = NewRequest(HttpMethod.Get, "GetLiveStreams");
            return Execute<List<LiveStream>>(request);
        }

        /// <summary>
        /// Tell the recorder we are still showing this stream and to keep it alive. Call this every 30 seconds or so.
        /// </summary>
        /// <param name="liveStream">The live stream (RTSP) that is stil in use.</param>
        /// <returns>True if the live stream is still running, false otherwise.</returns>
        public bool KeepLiveStreamAlive(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "KeepStreamAlive");
            request.AddBody(liveStream);
            var result = Execute<KeepStreamAliveResult>(request);
            return result.IsAlive;
        }

        private class KeepStreamAliveResult
        {
            public bool IsAlive { get; set; }
        }

        /// <summary>
        /// Ask the recorder for the give live stream's tuning details (if possible).
        /// </summary>
        /// <param name="liveStream">The active live stream.</param>
        /// <returns>The service tuning details, or null if none are available.</returns>
        public ServiceTuning GetLiveStreamTuningDetails(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "GetLiveStreamTuningDetails");
            request.AddBody(liveStream);
            return Execute<ServiceTuning>(request);
        }

        /// <summary>
        /// Get the live stream for a given RTSP url.
        /// </summary>
        /// <param name="rtspUrl">The RTSP url for which to find the live stream.</param>
        /// <returns>The corresponding live stream.</returns>
        public LiveStream GetLiveStreamByRtspUrl(string rtspUrl)
        {
            var request = NewRequest(HttpMethod.Post, "GetLiveStream");
            request.AddBody(new
            {
                RtspUrl = rtspUrl
            });
            return Execute<LiveStream>(request);
        }

        /// <summary>
        /// Get the live tuning state of one or more channels.
        /// </summary>
        /// <param name="channels">The channels to get the current state from.</param>
        /// <param name="liveStream">The live stream you want to be ignored (since it's yours), or null.</param>
        /// <returns>Null, or an array with the respective live state for each of the given channels.</returns>
        public List<ChannelLiveState> GetChannelsLiveState(IEnumerable<Channel> channels, LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "ChannelsLiveState");
            request.AddBody(new
            {
                Channels = channels,
                LiveStream = liveStream
            });
            return Execute<List<ChannelLiveState>>(request);
        }

        #endregion

        #region Teletext

        /// <summary>
        /// Ask the recorder whether the given live stream has teletext.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <returns>True if teletext is present.</returns>
        public bool HasTeletext(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "CanGrabTeletext");
            request.AddBody(liveStream);
            return Execute<CanGrabTeletextResult>(request).HasTeletext;
        }

        private class CanGrabTeletextResult
        {
            public bool HasTeletext { get; set; }
        }

        /// <summary>
        /// Tell the recorder to start grabbing teletext for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        public void StartGrabbingTeletext(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "StartGrabbingTeletext");
            request.AddBody(liveStream);
            Execute(request);
        }

        /// <summary>
        /// Tell the recorder to stop grabbing teletext for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        public void StopGrabbingTeletext(LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "StopGrabbingTeletext");
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
            var request = NewRequest(HttpMethod.Post, "GrabbingTeletext");
            request.AddBody(liveStream);
            return Execute<GrabbingTeletextResult>(request).IsGrabbingTeletext;
        }

        private class GrabbingTeletextResult
        {
            public bool IsGrabbingTeletext { get; set; }
        }

        /// <summary>
        /// Request a teletext page/subpage from the recorder for the given live stream.
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <param name="pageNumber">The teletext page number</param>
        /// <param name="subPageNumber">The teletext subpage number</param>
        /// <returns>The requested teletext page, or null if the page was not ready yet.</returns>
        public TeletextPage GetTeletextPage(LiveStream liveStream, int pageNumber, int subPageNumber)
        {
            var request = NewRequest(HttpMethod.Post, "TeletextPage");
            request.AddBody(new
            {
                LiveStream = liveStream,
                PageNumber = pageNumber,
                SubPageNumber = subPageNumber
            });
            return Execute<TeletextPage>(request);
        }

        /// <summary>
        /// Request a teletext page/subpage from the recorder for the given live stream, as an image. Note that the page
        /// may contain transparent parts even if 'useTransparentBackground' is set to false (e.g. subtitle or newsflash page).
        /// </summary>
        /// <param name="liveStream">The live stream.</param>
        /// <param name="pageNumber">The teletext page number</param>
        /// <param name="subPageNumber">The teletext subpage number</param>
        /// <param name="imageWidth">The width of the teletext image</param>
        /// <param name="imageHeight">The height of the teletext image</param>
        /// <param name="useTransparentBackground">Use a transparent background instead of black.</param>
        /// <param name="showHidden">Show the hidden teletext information.</param>
        /// <returns>The requested teletext page in form of an image, or null if the page was not ready yet.</returns>
        public byte[] GetTeletextPageImage(LiveStream liveStream, int pageNumber, int subPageNumber, int imageWidth, int imageHeight, bool useTransparentBackground = false, bool showHidden = false)
        {
            var request = NewRequest(HttpMethod.Post, "TeletextPageImage/{0}/{1}", imageWidth, imageHeight);
            if (useTransparentBackground)
            {
                request.AddParameter("useTransparentBackground", true);
            }
            if (showHidden)
            {
                request.AddParameter("showHidden", true);
            }
            request.AddBody(new
            {
                LiveStream = liveStream,
                PageNumber = pageNumber,
                SubPageNumber = subPageNumber
            });
            using (var response = ExecuteRequest(request))
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.NoContent:
                        return null;

                    case HttpStatusCode.NotModified:
                        return new byte[0];

                    case HttpStatusCode.OK:
                        return response.Content.ReadAsByteArrayAsync().Result;
                }
                throw new ArgusTVException(response.ReasonPhrase);
            }
        }

        #endregion
    }
}
