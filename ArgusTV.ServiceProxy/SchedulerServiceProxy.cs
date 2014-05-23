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

namespace ArgusTV.ServiceProxy
{
    /// <summary>
    /// Service to control/query all aspects of schedules.
    /// </summary>
    public partial class SchedulerServiceProxy : RestProxyBase
    {
        /// <summary>
        /// Constructs a channel to the service.
        /// </summary>
        internal SchedulerServiceProxy()
            : base("Scheduler")
        {
        }

        #region Channels

        /// <summary>
        /// Get all channel groups.
        /// </summary>
        /// <param name="channelType">The channel type of the channels in the group.</param>
        /// <param name="visibleOnly">Set to true to only receive groups that are marked visible.</param>
        /// <returns>A list containing zero or more channel groups.</returns>
        public List<ChannelGroup> GetAllChannelGroups(ChannelType channelType, bool visibleOnly = true)
        {
            var request = NewRequest(HttpMethod.Get, "ChannelGroups/{0}", channelType);
            if (!visibleOnly)
            {
                request.AddParameter("visibleOnly", false);
            }
            return Execute<List<ChannelGroup>>(request);
        }

        /// <summary>
        /// Get all channels.
        /// </summary>
        /// <param name="channelType">The channel type of the channels to retrieve.</param>
        /// <param name="visibleOnly">Set to true to only receive channels that are marked visible.</param>
        /// <returns>A list containing zero or more channels.</returns>
        public List<Channel> GetAllChannels(ChannelType channelType, bool visibleOnly = true)
        {
            var request = NewRequest(HttpMethod.Get, "Channels/{0}", channelType);
            if (!visibleOnly)
            {
                request.AddParameter("visibleOnly", false);
            }
            return Execute<List<Channel>>(request);
        }

        /// <summary>
        /// Get a channel by its ID.
        /// </summary>
        /// <param name="channelId">The ID of the channel.</param>
        /// <returns>The channel, or null if not found.</returns>
        public Channel GetChannelById(Guid channelId)
        {
            var request = NewRequest(HttpMethod.Get, "ChannelById/{0}", channelId);
            return Execute<Channel>(request);
        }

        /// <summary>
        /// Get a channel by its name.
        /// </summary>
        /// <param name="channelType">The channel type of the channel to retrieve.</param>
        /// <param name="displayName">The name of the channel.</param>
        /// <returns>The channel, or null if not found.</returns>
        public Channel GetChannelByDisplayName(ChannelType channelType, string displayName)
        {
            var request = NewRequest(HttpMethod.Post, "Channel/{0}/ByName", channelType);
            request.AddBody(new
            {
                Name = displayName
            });
            return Execute<Channel>(request);
        }

        /// <summary>
        /// Get a channel by its LCN.
        /// </summary>
        /// <param name="channelType">The channel type of the channel to retrieve.</param>
        /// <param name="logicalChannelNumber">The logical channel number of the channel.</param>
        /// <returns>The channel, or null if not found.</returns>
        public Channel GetChannelByLogicalChannelNumber(ChannelType channelType, int logicalChannelNumber)
        {
            var request = NewRequest(HttpMethod.Get, "ChannelByLCN/{0}/{1}", channelType, logicalChannelNumber);
            return Execute<Channel>(request);
        }

        /// <summary>
        /// Get all channels in a given group.
        /// </summary>
        /// <param name="channelGroupId">The ID of the channel group.</param>
        /// <param name="visibleOnly">Set to true to only receive channels that are marked visible.</param>
        /// <returns>A list containing zero or more channels.</returns>
        public List<Channel> GetChannelsInGroup(Guid channelGroupId, bool visibleOnly = true)
        {
            var request = NewRequest(HttpMethod.Get, "ChannelsInGroup/{0}", channelGroupId);
            if (!visibleOnly)
            {
                request.AddParameter("visibleOnly", false);
            }
            return Execute<List<Channel>>(request);
        }

        /// <summary>
        /// Get all channels that are attached to a guide channel.
        /// </summary>
        /// <param name="guideChannelId">The ID of the guide channel.</param>
        /// <returns>A list containing zero or more channels.</returns>
        public List<Channel> GetChannelsForGuideChannel(Guid guideChannelId)
        {
            var request = NewRequest(HttpMethod.Get, "ChannelsForGuide/{0}", guideChannelId);
            return Execute<List<Channel>>(request);
        }

        /// <summary>
        /// Make sure a channel exist and if it doesn't, add it and automatically link it to the
        /// guide channel with the same displayname (if found).
        /// </summary>
        /// <param name="channelType">The type of the channel.</param>
        /// <param name="displayName">The display name of the channel.</param>
        /// <param name="channelGroupName">Null, or a group name to place a newly created channel in.</param>
        /// <returns>The guid of the existing or new channel.</returns>
        public Guid EnsureChannel(ChannelType channelType, string displayName, string channelGroupName)
        {
            var request = NewRequest(HttpMethod.Post, "EnsureChannelExists");
            request.AddBody(new
            {
                ChannelType = channelType,
                DisplayName = displayName,
                ChannelGroupName = channelGroupName
            });
            var result = Execute<ChannelIdResult>(request);
            return result.ChannelId;
        }

        private class ChannelIdResult
        {
            public Guid ChannelId { get; set; }
        }

        /// <summary>
        /// Make sure the default channel for the given guide channel exists.  So we get a
        /// default one-to-one mapping.
        /// </summary>
        /// <param name="guideChannelId">The ID of the guide channel.</param>
        /// <param name="channelType">The type of the channel.</param>
        /// <param name="displayName">The display name of the guide channel.</param>
        /// <param name="channelGroupName">Null, or a group name to place a newly created channel in.</param>
        public void EnsureDefaultChannel(Guid guideChannelId, ChannelType channelType, string displayName, string channelGroupName)
        {
            var request = NewRequest(HttpMethod.Post, "EnsureDefaultChannel/{0}", guideChannelId);
            request.AddBody(new
            {
                ChannelType = channelType,
                DisplayName = displayName,
                ChannelGroupName = channelGroupName
            });
            Execute(request);
        }

        /// <summary>
        /// Attach a channel to a guide channel.
        /// </summary>
        /// <param name="channelId">The ID of the channel.</param>
        /// <param name="guideChannelId">The ID of the guide channel.</param>
        public void AttachChannelToGuide(Guid channelId, Guid guideChannelId)
        {
            var request = NewRequest(HttpMethod.Post, "AttachChannelToGuide/{0}/{1}", channelId, guideChannelId);
            Execute(request);
        }

        /// <summary>
        /// Delete a channel group.
        /// </summary>
        /// <param name="channelGroupId">The ID of the group to delete.</param>
        /// <param name="deleteOrphanedChannels">Also delete channels in the group that are not in any other group.</param>
        /// <param name="deleteOrphanedGuideChannels">Also delete the guide channels that are not used by any other channels.</param>
        public void DeleteChannelGroup(Guid channelGroupId, bool deleteOrphanedChannels = true, bool deleteOrphanedGuideChannels = true)
        {
            var request = NewRequest(HttpMethod.Post, "DeleteChannelGroup/{0}", channelGroupId);
            if (!deleteOrphanedChannels)
            {
                request.AddParameter("deleteOrphanedChannels", false);
            }
            if (!deleteOrphanedGuideChannels)
            {
                request.AddParameter("deleteOrphanedGuideChannels", false);
            }
            Execute(request);
        }

        /// <summary>
        /// Delete a channel.
        /// </summary>
        /// <param name="channelId">The ID of the channel to delete.</param>
        /// <param name="deleteOrphanedGuideChannel">Also delete the guide channel if it is not used by any other channel.</param>
        public void DeleteChannel(Guid channelId, bool deleteOrphanedGuideChannel = true)
        {
            var request = NewRequest(HttpMethod.Post, "DeleteChannel/{0}", channelId);
            if (!deleteOrphanedGuideChannel)
            {
                request.AddParameter("deleteOrphanedGuideChannel", false);
            }
            Execute(request);
        }

        /// <summary>
        /// Save a modified or new channel.  A new channel is recognized by a Guid.Empty ID.
        /// </summary>
        /// <param name="channel">The channel to save.</param>
        /// <returns>The saved channel.</returns>
        public Channel SaveChannel(Channel channel)
        {
            var request = NewRequest(HttpMethod.Post, "SaveChannel");
            request.AddBody(channel);
            return Execute<Channel>(request);
        }

        /// <summary>
        /// Save several modified or new channels.  New channels are recognized by a Guid.Empty ID.
        /// </summary>
        /// <param name="channels">A list of channels to save.</param>
        public void SaveChannels(IEnumerable<Channel> channels)
        {
            var request = NewRequest(HttpMethod.Post, "SaveChannels");
            request.AddBody(channels);
            Execute(request);
        }

        /// <summary>
        /// Save a modified or new channel group.  A new channel group is recognized by a Guid.Empty ID.
        /// </summary>
        /// <param name="channelGroup">The channel group to save.</param>
        /// <returns>The saved channel group.</returns>
        public ChannelGroup SaveChannelGroup(ChannelGroup channelGroup)
        {
            var request = NewRequest(HttpMethod.Post, "SaveChannelGroup");
            request.AddBody(channelGroup);
            return Execute<ChannelGroup>(request);
        }

        /// <summary>
        /// Get channels members of a group.  The order of the channels in the array is the sequence.
        /// </summary>
        /// <param name="channelGroupId">The ID of the parent channel group.</param>
        /// <returns>A list of channels that are in the group.</returns>
        public List<Guid> GetChannelGroupMembers(Guid channelGroupId)
        {
            var request = NewRequest(HttpMethod.Get, "ChannelGroupMembers/{0}", channelGroupId);
            return Execute<List<Guid>>(request);
        }

        /// <summary>
        /// Set channels members of a group.  The order of the channels in the array is saved as the sequence.
        /// </summary>
        /// <param name="channelGroupId">The ID of the parent channel group.</param>
        /// <param name="channelIds">A list of channels that are in the group.</param>
        public void SetChannelGroupMembers(Guid channelGroupId, IEnumerable<Guid> channelIds)
        {
            var request = NewRequest(HttpMethod.Post, "SetChannelGroupMembers/{0}", channelGroupId);
            request.AddBody(channelIds);
            Execute(request);
        }

        /// <summary>
        /// Get all the groups a channel belongs to.
        /// </summary>
        /// <param name="channelId">The ID of the channel.</param>
        /// <param name="visibleOnly">Set to true to only receive groups that are marked visible.</param>
        /// <returns>A list containing the zero or more groups the given channel belongs to.</returns>
        public List<ChannelGroup> GetChannelGroupsForChannel(Guid channelId, bool visibleOnly = true)
        {
            var request = NewRequest(HttpMethod.Get, "ChannelGroupsForChannel/{0}", channelId);
            if (!visibleOnly)
            {
                request.AddParameter("visibleOnly", false);
            }
            return Execute<List<ChannelGroup>>(request);
        }

        /// <summary>
        /// Get a resized channel logo, if newer than the provided timestamp. The aspect ratio of the
        /// original image is preserved so the returned image is potentially either smaller than
        /// the requested size, or centered on a transparent background.
        /// </summary>
        /// <param name="channelId">The ID of the channel.</param>
        /// <param name="width">The requested width.</param>
        /// <param name="height">The requested height.</param>
        /// <param name="modifiedAfterTime">Only return a logo if it is newer than the given timestamp.</param>
        /// <param name="useTransparentBackground">Use a transparent background when preserving the aspect ratio.</param>
        /// <param name="argbBackground">The background color to use in case useTransparentBackground is false.</param>
        /// <returns>A byte array containing the bytes of a (transparent) PNG of the resized logo, an empty array if no newer logo was found or null if no logo was found.</returns>
        public byte[] GetChannelLogo(Guid channelId, int width, int height, DateTime modifiedAfterTime, bool useTransparentBackground = true, int? argbBackground = null)
        {
            var request = NewRequest(HttpMethod.Get, "ChannelLogo/{0}/{1}", channelId, modifiedAfterTime);
            request.AddParameter("width", width);
            request.AddParameter("height", height);
            if (!useTransparentBackground)
            {
                request.AddParameter("useTransparentBackground", false);
            }
            if (argbBackground.HasValue)
            {
                request.AddParameter("argbBackground", argbBackground.Value);
            }
            using (var response = _client.SendAsync(request).Result)
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

        #region Schedules

        /// <summary>
        /// Create a new schedule. All default processing commands will be added to the new schedule.
        /// </summary>
        /// <param name="channelType">The channel-type of this schedule.</param>
        /// <param name="type">The type of the schedule.</param>
        /// <returns>A new schedule without rules (note that you still need to call SaveSchedule() to save it).</returns>
        public Schedule CreateNewSchedule(ChannelType channelType, ScheduleType type)
        {
            var request = NewRequest(HttpMethod.Get, "EmptySchedule/{0}/{1}", channelType, type);
            return Execute<Schedule>(request);
        }

        /// <summary>
        /// Get all schedule for the given type.
        /// </summary>
        /// <param name="channelType">The channel-type of the schedules.</param>
        /// <param name="type">The type of the schedules.</param>
        /// <param name="deleteObsoleteSchedules">Automatically delete obsolete schedules before returning the list.</param>
        /// <returns>A list with zero or more schedules.</returns>
        public List<ScheduleSummary> GetAllSchedules(ChannelType channelType, ScheduleType type, bool deleteObsoleteSchedules = true)
        {
            var request = NewRequest(HttpMethod.Get, "Schedules/{0}/{1}", channelType, type);
            if (!deleteObsoleteSchedules)
            {
                request.AddParameter("deleteObsoleteSchedules", false);
            }
            return Execute<List<ScheduleSummary>>(request);
        }

        /// <summary>
        /// Get a schedule by its ID.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule.</param>
        /// <returns>The requested schedule, or null if it is not found.</returns>
        public Schedule GetScheduleById(Guid scheduleId)
        {
            var request = NewRequest(HttpMethod.Get, "ScheduleById/{0}", scheduleId);
            return Execute<SerializableSchedule>(request).ToSchedule();
        }

        /// <summary>
        /// Save a new or modified schedule.  A new schedule is recognized by a Guid.Empty ID.
        /// </summary>
        /// <param name="schedule">The schedule to save.</param>
        /// <returns>The saved schedule.</returns>
        public Schedule SaveSchedule(Schedule schedule)
        {
            var request = NewRequest(HttpMethod.Post, "SaveSchedule");
            request.AddBody(schedule.ToSerializable());
            return Execute<SerializableSchedule>(request).ToSchedule();
        }

        /// <summary>
        /// Delete a schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule to delete.</param>
        public void DeleteSchedule(Guid scheduleId)
        {
            var request = NewRequest(HttpMethod.Post, "DeleteSchedule/{0}", scheduleId);
            Execute(request);
        }

        /// <summary>
        /// Save a modified schedule summary.
        /// </summary>
        /// <param name="scheduleSummary">The schedule summary to save.</param>
        /// <returns>The saved schedule.</returns>
        public ScheduleSummary SaveScheduleSummary(ScheduleSummary scheduleSummary)
        {
            var request = NewRequest(HttpMethod.Post, "SaveScheduleSummary");
            request.AddBody(scheduleSummary);
            return Execute<ScheduleSummary>(request);
        }

        #endregion

        #region Upcoming

        /// <summary>
        /// Get the first upcoming program for the given type.
        /// </summary>
        /// <param name="type">The schedule type.</param>
        /// <returns>An upcoming program.</returns>
        public UpcomingProgram GetNextUpcomingProgram(ScheduleType type)
        {
            var request = NewRequest(HttpMethod.Get, "NextUpcomingProgram/{0}", type);
            return Execute<UpcomingProgram>(request);
        }

        /// <summary>
        /// Get all upcoming programs for the given type.
        /// </summary>
        /// <param name="type">The schedule type.</param>
        /// <param name="includeCancelled">Set to true to also retrieve cancelled programs.</param>
        /// <returns>A list with zero or more upcoming programs.</returns>
        public List<UpcomingProgram> GetAllUpcomingPrograms(ScheduleType type, bool includeCancelled = false)
        {
            var request = NewRequest(HttpMethod.Get, "UpcomingPrograms/{0}", type);
            if (includeCancelled)
            {
                request.AddParameter("includeCancelled", true);
            }
            return Execute<List<UpcomingProgram>>(request);
        }

        /// <summary>
        /// Get all upcoming programs for the given type, with a start-time before the given date and time.
        /// </summary>
        /// <param name="type">The schedule type.</param>
        /// <param name="untilDateTime">Only return programs with a start-time before this time.</param>
        /// <param name="includeCancelled">Set to true to also retrieve cancelled programs.</param>
        /// <returns>A list with zero or more upcoming programs.</returns>
        public List<UpcomingProgram> GetAllUpcomingProgramsUntil(ScheduleType type, DateTime untilDateTime, bool includeCancelled = false)
        {
            var request = NewRequest(HttpMethod.Get, "UpcomingProgramsUntil/{0}/{1}", type, untilDateTime);
            if (includeCancelled)
            {
                request.AddParameter("includeCancelled", true);
            }
            return Execute<List<UpcomingProgram>>(request);
        }

        /// <summary>
        /// Get all upcoming guide programs for the given type.
        /// </summary>
        /// <param name="type">The schedule type.</param>
        /// <param name="includeCancelled">Set to true to also retrieve cancelled programs.</param>
        /// <returns>A list with zero or more upcoming guide programs.</returns>
        public List<UpcomingGuideProgram> GetUpcomingGuidePrograms(ScheduleType type, bool includeCancelled)
        {
            var request = NewRequest(HttpMethod.Get, "UpcomingGuidePrograms/{0}", type);
            if (includeCancelled)
            {
                request.AddParameter("includeCancelled", true);
            }
            return Execute<List<UpcomingGuideProgram>>(request);
        }

        /// <summary>
        /// Get all upcoming programs for the given schedule.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="includeCancelled">Set to true to also retrieve cancelled programs.</param>
        /// <returns>A list with zero or more upcoming programs.</returns>
        public List<UpcomingProgram> GetUpcomingPrograms(Schedule schedule, bool includeCancelled)
        {
            var request = NewRequest(HttpMethod.Post, "UpcomingProgramsForSchedule");
            request.AddBody(new
            {
                Schedule = schedule.ToSerializable(),
                IncludeCancelled = includeCancelled
            });
            return Execute<List<UpcomingProgram>>(request);
        }

        /// <summary>
        /// Cancel the specified program for the given schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule</param>
        /// <param name="guideProgramId">The ID of the guide program to cancel.</param>
        /// <param name="channelId">The ID of the channel of the program.</param>
        /// <param name="startTime">The start-time of the program to cancel.</param>
        public void CancelUpcomingProgram(Guid scheduleId, Guid? guideProgramId, Guid channelId, DateTime startTime)
        {
            var request = NewRequest(HttpMethod.Post, "CancelUpcomingProgram/{0}/{1}/{2}", scheduleId, channelId, startTime);
            if (guideProgramId.HasValue)
            {
                request.AddParameter("guideProgramId", guideProgramId.Value);
            }
            Execute(request);
        }

        /// <summary>
        /// Un-cancel the specified program for the given schedule.
        /// </summary>
        /// <param name="scheduleId">The ID of the schedule</param>
        /// <param name="guideProgramId">The ID of the guide program to cancel.</param>
        /// <param name="channelId">The ID of the channel of the program.</param>
        /// <param name="startTime">The start-time of the program to cancel.</param>
        public void UncancelUpcomingProgram(Guid scheduleId, Guid? guideProgramId, Guid channelId, DateTime startTime)
        {
            var request = NewRequest(HttpMethod.Post, "UncancelUpcomingProgram/{0}/{1}/{2}", scheduleId, channelId, startTime);
            if (guideProgramId.HasValue)
            {
                request.AddParameter("guideProgramId", guideProgramId.Value);
            }
            Execute(request);
        }

        /// <summary>
        /// Set a priority for a specific upcoming program.
        /// </summary>
        /// <param name="upcomingProgramId">The ID of the upcoming program.</param>
        /// <param name="startTime">The upcoming program's start time.</param>
        /// <param name="priority">The priority to use, or null to use the schedule's priority again.</param>
        public void SetUpcomingProgramPriority(Guid upcomingProgramId, DateTime startTime, UpcomingProgramPriority? priority)
        {
            var request = NewRequest(HttpMethod.Post, "SetUpcomingProgramPriority/{0}/{1}", upcomingProgramId, startTime);
            if (priority.HasValue)
            {
                request.AddParameter("priority", priority.Value);
            }
            Execute(request);
        }

        /// <summary>
        /// Set the pre-record time for a specific upcoming program.
        /// </summary>
        /// <param name="upcomingProgramId">The ID of the upcoming program.</param>
        /// <param name="startTime">The upcoming program's start time.</param>
        /// <param name="preRecordSeconds">The number of pre-record seconds to use, or null to use the schedule's pre-record setting again.</param>
        public void SetUpcomingProgramPreRecord(Guid upcomingProgramId, DateTime startTime, int? preRecordSeconds)
        {
            var request = NewRequest(HttpMethod.Post, "SetUpcomingProgramPreRecord/{0}/{1}", upcomingProgramId, startTime);
            if (preRecordSeconds.HasValue)
            {
                request.AddParameter("seconds", preRecordSeconds.Value);
            }
            Execute(request);
        }

        /// <summary>
        /// Set the post-record time for a specific upcoming program.
        /// </summary>
        /// <param name="upcomingProgramId">The ID of the upcoming program.</param>
        /// <param name="startTime">The upcoming program's start time.</param>
        /// <param name="postRecordSeconds">The number of post-record seconds to use, or null to use the schedule's post-record setting again.</param>
        public void SetUpcomingProgramPostRecord(Guid upcomingProgramId, DateTime startTime, int? postRecordSeconds)
        {
            var request = NewRequest(HttpMethod.Post, "SetUpcomingProgramPostRecord/{0}/{1}", upcomingProgramId, startTime);
            if (postRecordSeconds.HasValue)
            {
                request.AddParameter("seconds", postRecordSeconds.Value);
            }
            Execute(request);
        }

        #endregion

        #region Guide

        /// <summary>
        /// Get all program titles that start with, or have a word that starts with the given text.
        /// </summary>
        /// <param name="channelType">If not null, only search programs of this type.</param>
        /// <param name="partialTitle">The text to search for.</param>
        /// <param name="includeProgramsInPast">Set to true if you want to include programs that have already ended.</param>
        /// <returns>A list containing zero or more program titles, ordered alphabetically.</returns>
        public List<string> GetTitlesByPartialTitle(ChannelType? channelType, string partialTitle, bool includeProgramsInPast = false)
        {
            var request = NewRequest(HttpMethod.Post, "GetTitlesByPartialTitle/{0}", channelType);
            if (includeProgramsInPast)
            {
                request.AddParameter("includeProgramsInPast", true);
            }
            request.AddBody(new
            {
                PartialTitle = partialTitle
            });
            return Execute<List<string>>(request);
        }

        /// <summary>
        /// Search guide for programs whose title starts with, or has a word that starts with the given text.
        /// </summary>
        /// <param name="channelType">If not null, only search programs of this type.</param>
        /// <param name="partialTitle">The text to search for.</param>
        /// <param name="includeProgramsInPast">Set to true if you want to include programs that have already ended.</param>
        /// <returns>A list containing zero or more channel programs.</returns>
        public List<ChannelProgram> SearchGuideByPartialTitle(ChannelType? channelType, string partialTitle, bool includeProgramsInPast = false)
        {
            var request = NewRequest(HttpMethod.Post, "SearchGuideUsingPartialTitle/{0}", channelType);
            if (includeProgramsInPast)
            {
                request.AddParameter("includeProgramsInPast", true);
            }
            request.AddBody(new
            {
                PartialTitle = partialTitle
            });
            return Execute<List<ChannelProgram>>(request);
        }

        /// <summary>
        /// Search guide for programs whose title equals the given text.
        /// </summary>
        /// <param name="channelType">If not null, only search programs of this type.</param>
        /// <param name="title">The exact title to search for.</param>
        /// <param name="includeProgramsInPast">Set to true if you want to include programs that have already ended.</param>
        /// <returns>A list containing zero or more channel programs.</returns>
        public List<ChannelProgram> SearchGuideByTitle(ChannelType? channelType, string title, bool includeProgramsInPast = false)
        {
            var request = NewRequest(HttpMethod.Post, "SearchGuideUsingTitle/{0}", channelType);
            if (includeProgramsInPast)
            {
                request.AddParameter("includeProgramsInPast", true);
            }
            request.AddBody(new
            {
                Title = title
            });
            return Execute<List<ChannelProgram>>(request);
        }

        /// <summary>
        /// Get the current and next program on the given channel.
        /// </summary>
        /// <param name="channelId">The channel ID.</param>
        /// <param name="includeLiveState">Set to true to receive the live state of all channels.</param>
        /// <param name="liveStream">If you are including the live state, the live stream you want to be ignored (since it's yours), or null.</param>
        /// <returns>A CurrentAndNextProgram item.</returns>
        public CurrentAndNextProgram GetCurrentAndNextForChannel(Guid channelId, bool includeLiveState, LiveStream liveStream)
        {
            var request = NewRequest(HttpMethod.Post, "CurrentAndNextForChannel");
            request.AddBody(new
            {
                ChannelId = channelId,
                IncludeLiveState = includeLiveState,
                LiveStream = liveStream
            });
            return Execute<CurrentAndNextProgram>(request);
        }

        /// <summary>
        /// Get the current and next program on the given channels.
        /// </summary>
        /// <param name="channelIds">The channel IDs.</param>
        /// <param name="visibleOnly">Set to true to only receive channels that are marked visible.</param>
        /// <param name="includeLiveState">Set to true to receive the live state of all channels.</param>
        /// <param name="liveStream">If you are including the live state, the live stream you want to be ignored (since it's yours), or null.</param>
        /// <returns>A list containing zero or more CurrentAndNextProgram items.</returns>
        public List<CurrentAndNextProgram> GetCurrentAndNextForChannels(IEnumerable<Guid> channelIds, bool includeLiveState, LiveStream liveStream, bool visibleOnly = true)
        {
            var request = NewRequest(HttpMethod.Post, "CurrentAndNextForChannels");
            request.AddBody(new
            {
                ChannelIds = channelIds,
                VisibleOnly = visibleOnly,
                IncludeLiveState = includeLiveState,
                LiveStream = liveStream
            });
            return Execute<List<CurrentAndNextProgram>>(request);
        }

        /// <summary>
        /// Get the current and next program on all the channels in the given group.
        /// </summary>
        /// <param name="channelGroupId">The ID of the channel group.</param>
        /// <param name="visibleOnly">Set to true to only receive channels that are marked visible.</param>
        /// <param name="includeLiveState">Set to true to receive the live state of all channels.</param>
        /// <param name="liveStream">If you are including the live state, the live stream you want to be ignored (since it's yours), or null.</param>
        /// <returns>A list containing zero or more CurrentAndNextProgram items.</returns>
        public List<CurrentAndNextProgram> GetCurrentAndNextForGroup(Guid channelGroupId, bool includeLiveState, LiveStream liveStream, bool visibleOnly = true)
        {
            var request = NewRequest(HttpMethod.Post, "CurrentAndNextForGroup");
            request.AddBody(new
            {
                ChannelGroupId = channelGroupId,
                VisibleOnly = visibleOnly,
                IncludeLiveState = includeLiveState,
                LiveStream = liveStream
            });
            return Execute<List<CurrentAndNextProgram>>(request);
        }

        #endregion

        #region Processing Commands

        /// <summary>
        /// Get all processing commands.
        /// </summary>
        /// <returns>A list containing zero or more processing commands.</returns>
        public List<ProcessingCommand> GetAllProcessingCommands()
        {
            var request = NewRequest(HttpMethod.Get, "ProcessingCommands");
            return Execute<List<ProcessingCommand>>(request);
        }

        /// <summary>
        /// Save a modified or new processing command.  A new processing command is recognized by a Guid.Empty ID.
        /// </summary>
        /// <param name="processingCommand">The processing command to save.</param>
        public void SaveProcessingCommand(ProcessingCommand processingCommand)
        {
            var request = NewRequest(HttpMethod.Post, "SaveProcessingCommand");
            request.AddBody(processingCommand);
            Execute(request);
        }

        /// <summary>
        /// Delete a processing command.
        /// </summary>
        /// <param name="processingCommandId">The ID of the processing command to delete.</param>
        public void DeleteProcessingCommand(Guid processingCommandId)
        {
            var request = NewRequest(HttpMethod.Post, "DeleteProcessingCommand/{0}", processingCommandId);
            Execute(request);
        }

        #endregion

        #region Recording File Formats

        /// <summary>
        /// Get all recording formats.
        /// </summary>
        /// <returns>A list containing zero or more recording formats.</returns>
        public List<RecordingFileFormat> GetAllRecordingFileFormats()
        {
            var request = NewRequest(HttpMethod.Get, "RecordingFileFormats");
            return Execute<List<RecordingFileFormat>>(request);
        }

        /// <summary>
        /// Save a modified or new recording format.  A new recording format is recognized by a Guid.Empty ID.
        /// </summary>
        /// <param name="recordingFileFormat">The recording format to save.</param>
        public void SaveRecordingFileFormat(RecordingFileFormat recordingFileFormat)
        {
            var request = NewRequest(HttpMethod.Post, "SaveRecordingFileFormat");
            request.AddBody(recordingFileFormat);
            Execute(request);
        }

        /// <summary>
        /// Delete a recording format.
        /// </summary>
        /// <param name="recordingFileFormatId">The ID of the recording format to delete.</param>
        public void DeleteRecordingFileFormat(Guid recordingFileFormatId)
        {
            var request = NewRequest(HttpMethod.Post, "DeleteRecordingFileFormat/{0}", recordingFileFormatId);
            Execute(request);
        }

        #endregion
    }
}
