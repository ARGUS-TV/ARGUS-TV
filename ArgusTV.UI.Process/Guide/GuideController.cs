/*
 *	Copyright (C) 2007-2013 ARGUS TV
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
using System.Text;

using ArgusTV.ServiceContracts;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.Process.Guide
{
    public class GuideController
    {
        private GuideModel _model;

        public GuideController(GuideModel model)
        {
            _model = model;
        }

        public void Initialize(ISchedulerService tvSchedulerAgent, ChannelType channelType, int epgHours, string allChannelsGroupName)
        {
            Initialize(tvSchedulerAgent, channelType, epgHours, 0, allChannelsGroupName);
        }

        public void Initialize(ISchedulerService tvSchedulerAgent, ChannelType channelType, int epgHours, int epgHoursOffset, string allChannelsGroupName)
        {
            _model.EpgHours = epgHours;
            _model.EpgHoursOffset = epgHoursOffset;
            _model.AllChannelsGroupName = allChannelsGroupName;
            ChangeChannelType(tvSchedulerAgent, channelType);
        }

        public void ChangeChannelType(ISchedulerService tvSchedulerAgent, ChannelType channelType)
        {
            _model.ChannelType = channelType;
            _model.ChannelGroups.Clear();
            _model.ChannelGroups.AddRange(tvSchedulerAgent.GetAllChannelGroups(channelType, true));
            _model.ChannelGroups.Add(new ChannelGroup(
                channelType == ChannelType.Television ? ChannelGroup.AllTvChannelsGroupId : ChannelGroup.AllRadioChannelsGroupId,
                (int)channelType, _model.AllChannelsGroupName, true, 0, 0));

            if (_model.GuideDateTime == DateTime.MinValue)
            {
                GotoNow();
                _model.CurrentChannelGroupId = _model.ChannelGroups[0].ChannelGroupId;
            }
            else
            {
                bool currentIsOk = false;
                foreach (ChannelGroup channelGroup in _model.ChannelGroups)
                {
                    if (channelGroup.ChannelGroupId == _model.CurrentChannelGroupId)
                    {
                        currentIsOk = true;
                        break;
                    }
                }
                if (!currentIsOk)
                {
                    _model.CurrentChannelGroupId = _model.ChannelGroups[0].ChannelGroupId;
                }
            }
        }

        public void SetEpgHours(int epgHours)
        {
            _model.EpgHours = epgHours;
        }

        public void GotoNow()
        {
            DateTime guideDate = DateTime.Now;
            int hours = (int)Math.Floor((decimal)guideDate.TimeOfDay.TotalHours);
            _model.GuideDateTime = guideDate.Date.Add(new TimeSpan(hours - hours % _model.EpgHours, 0, 0));
            if (_model.GuideDateTime.TimeOfDay.TotalHours < _model.EpgHoursOffset)
            {
                _model.GuideDateTime = _model.GuideDateTime.AddDays(-1).AddHours(_model.EpgHoursOffset);
            }
        }

        public delegate bool CancellationPendingDelegate();

        public void RefreshEpgData(ISchedulerService tvSchedulerAgent, IGuideService tvGuideAgent,
            IControlService tvControlAgent, bool reloadData, Guid currentChannelGroupId, DateTime guideDateTime)
        {
            RefreshEpgData(tvSchedulerAgent, tvGuideAgent, tvControlAgent, reloadData, currentChannelGroupId, guideDateTime, null);
        }

        public void RefreshEpgData(ISchedulerService tvSchedulerAgent, IGuideService tvGuideAgent,
            IControlService tvControlAgent, bool reloadData, Guid currentChannelGroupId, DateTime guideDateTime,
            CancellationPendingDelegate cancellationPending)
        {
            if (reloadData)
            {
                _model.ProgramsByChannel.Clear();
                RefreshUpcomingPrograms(tvSchedulerAgent, tvControlAgent);
                if (cancellationPending != null
                    && cancellationPending())
                {
                    return;
                }
            }
            if (guideDateTime != DateTime.MinValue)
            {
                SetChannelGroup(currentChannelGroupId);
                _model.GuideDateTime = guideDateTime;
                _model.Channels = new List<Channel>(tvSchedulerAgent.GetChannelsInGroup(currentChannelGroupId, true));
                if (cancellationPending != null
                    && cancellationPending())
                {
                    return;
                }
                RefreshChannelsEpgData(tvGuideAgent, _model.Channels, guideDateTime, guideDateTime.AddDays(1), cancellationPending);
            }
            else
            {
                _model.Channels = new List<Channel>();
            }
        }

        public void SetChannelGroup(Guid currentChannelGroupId)
        {
            _model.CurrentChannelGroupId = currentChannelGroupId;
        }

        public void RefreshChannelsEpgData(IGuideService tvGuideAgent, List<Channel> channels, DateTime fromDateTime, DateTime toDateTime)
        {
            RefreshChannelsEpgData(tvGuideAgent, channels, fromDateTime, toDateTime, null);
        }

        public void RefreshChannelsEpgData(IGuideService tvGuideAgent, List<Channel> channels,
            DateTime fromDateTime, DateTime toDateTime, CancellationPendingDelegate cancellationPending)
        {
            foreach (Channel channel in channels)
            {
                if (channel.GuideChannelId.HasValue)
                {
                    if (_model.ProgramsByChannel.ContainsKey(channel.ChannelId))
                    {
                        ChannelPrograms channelPrograms = _model.ProgramsByChannel[channel.ChannelId];
                        if (channelPrograms.LowerBoundTime >= toDateTime
                            || channelPrograms.UpperBoundTime <= fromDateTime)
                        {
                            _model.ProgramsByChannel.Remove(channel.ChannelId);
                        }
                        else if (channelPrograms.LowerBoundTime <= fromDateTime
                            && channelPrograms.UpperBoundTime <= toDateTime)
                        {
                            DateTime newUpperBoundTime = channelPrograms.UpperBoundTime.AddHours(8);
                            if (toDateTime > newUpperBoundTime)
                            {
                                newUpperBoundTime = toDateTime;
                            }
                            MergeExtraPrograms(tvGuideAgent, channel, channelPrograms, channelPrograms.UpperBoundTime, newUpperBoundTime);
                            channelPrograms.UpperBoundTime = newUpperBoundTime;
                        }
                        else if (channelPrograms.LowerBoundTime >= fromDateTime
                            && channelPrograms.UpperBoundTime >= toDateTime)
                        {
                            DateTime newLowerBoundTime = channelPrograms.LowerBoundTime.AddHours(-8);
                            if (fromDateTime < newLowerBoundTime)
                            {
                                newLowerBoundTime = fromDateTime;
                            }
                            MergeExtraPrograms(tvGuideAgent, channel, channelPrograms, newLowerBoundTime, channelPrograms.LowerBoundTime);
                            channelPrograms.LowerBoundTime = newLowerBoundTime;
                        }
                    }

                    if (cancellationPending != null
                        && cancellationPending())
                    {
                        return;
                    }

                    if (!_model.ProgramsByChannel.ContainsKey(channel.ChannelId))
                    {
                        _model.ProgramsByChannel[channel.ChannelId] = new ChannelPrograms(fromDateTime, toDateTime,
                            tvGuideAgent.GetChannelProgramsBetween(channel.GuideChannelId.Value, fromDateTime, toDateTime));
                    }
                }
                else
                {
                    _model.ProgramsByChannel[channel.ChannelId] = new ChannelPrograms(fromDateTime, toDateTime, new GuideProgramSummary[] { });
                }

                if (cancellationPending != null
                    && cancellationPending())
                {
                    break;
                }
            }
        }

        private void MergeExtraPrograms(IGuideService tvGuideAgent, Channel channel, ChannelPrograms channelPrograms, DateTime fromDateTime, DateTime toDateTime)
        {
            if (toDateTime > fromDateTime)
            {
                GuideProgramSummary[] guidePrograms =
                    tvGuideAgent.GetChannelProgramsBetween(channel.GuideChannelId.Value, fromDateTime, toDateTime);
                foreach (GuideProgramSummary guideProgram in guidePrograms)
                {
                    channelPrograms.InsertProgram(guideProgram);
                }
            }
        }

        public Channel GetChannelById(Guid channelId)
        {
            foreach (Channel channel in _model.Channels)
            {
                if (channel.ChannelId == channelId)
                {
                    return channel;
                }
            }
            return null;
        }

        public void RefreshUpcomingPrograms(ISchedulerService tvSchedulerAgent, IControlService tvControlAgent)
        {
            _model.UpcomingProgramsByType.Clear();
            RefreshGuideUpcomingRecordings(tvControlAgent);
            _model.UpcomingProgramsByType[ScheduleType.Alert] = GetGuideUpcomingPrograms(tvSchedulerAgent, ScheduleType.Alert);
            _model.UpcomingProgramsByType[ScheduleType.Suggestion] = GetGuideUpcomingPrograms(tvSchedulerAgent, ScheduleType.Suggestion);
            _model.UpcomingRecordingsById = BuildUpcomingProgramsDictionary(ScheduleType.Recording);
            _model.UpcomingAlertsById = BuildUpcomingProgramsDictionary(ScheduleType.Alert);
            _model.UpcomingSuggestionsById = BuildUpcomingProgramsDictionary(ScheduleType.Suggestion);
        }

        private void RefreshGuideUpcomingRecordings(IControlService tvControlAgent)
        {
            _model.UpcomingRecordings = new UpcomingOrActiveProgramsList(tvControlAgent.GetAllUpcomingRecordings(UpcomingRecordingsFilter.All, true));
            List<GuideUpcomingProgram> result = new List<GuideUpcomingProgram>();
            foreach (UpcomingOrActiveProgramView upcoming in _model.UpcomingRecordings)
            {
                if (upcoming.UpcomingProgram.GuideProgramId.HasValue)
                {
                    result.Add(new GuideUpcomingProgram(upcoming.UpcomingRecording));
                }
            }
            _model.UpcomingProgramsByType[ScheduleType.Recording] = result;
        }

        private List<GuideUpcomingProgram> GetGuideUpcomingPrograms(ISchedulerService tvSchedulerAgent, ScheduleType type)
        {
            UpcomingGuideProgram[] upcomingPrograms = tvSchedulerAgent.GetUpcomingGuidePrograms(type, true);
            List<GuideUpcomingProgram> result = new List<GuideUpcomingProgram>();
            foreach (UpcomingGuideProgram upcomingProgram in upcomingPrograms)
            {
                result.Add(new GuideUpcomingProgram(type, upcomingProgram));
            }
            return result;
        }

        private SerializableDictionary<Guid, GuideUpcomingProgram> BuildUpcomingProgramsDictionary(ScheduleType scheduleType)
        {
            SerializableDictionary<Guid, GuideUpcomingProgram> result = new SerializableDictionary<Guid, GuideUpcomingProgram>();
            foreach (GuideUpcomingProgram upcomingProgram in _model.UpcomingProgramsByType[scheduleType])
            {
                if (!result.ContainsKey(upcomingProgram.UpcomingProgramId))
                {
                    result.Add(upcomingProgram.UpcomingProgramId, upcomingProgram);
                }
            }
            return result;
        }

        public void SetZoomedChannel(Guid zoomedChannelId)
        {
            _model.ZoomedChannelId = zoomedChannelId;
        }

        public void ClearZoomedChannel()
        {
            _model.ZoomedChannelId = null;
        }

        public void CancelOrUncancelUpcomingProgram(ISchedulerService tvSchedulerAgent, IGuideService tvGuideAgent,
            IControlService tvControlAgent, Guid scheduleId, Guid channelId, Guid guideProgramId, bool cancel)
        {
            GuideProgram guideProgram = tvGuideAgent.GetProgramById(guideProgramId);
            if (guideProgram != null)
            {
                if (cancel)
                {
                    tvSchedulerAgent.CancelUpcomingProgram(scheduleId, guideProgramId, channelId, guideProgram.StartTime);
                }
                else
                {
                    tvSchedulerAgent.UncancelUpcomingProgram(scheduleId, guideProgramId, channelId, guideProgram.StartTime);
                }
                RefreshUpcomingPrograms(tvSchedulerAgent, tvControlAgent);
            }
        }

        public void AddRemoveHistoryUpcomingProgram(ISchedulerService tvSchedulerAgent, IControlService tvControlAgent,
            UpcomingProgram upcomingProgram, bool addToHistory)
        {
            if (addToHistory)
            {
                tvControlAgent.AddToPreviouslyRecordedHistory(upcomingProgram);
            }
            else
            {
                tvControlAgent.RemoveFromPreviouslyRecordedHistory(upcomingProgram);
            }
            RefreshUpcomingPrograms(tvSchedulerAgent, tvControlAgent);
        }

        public Schedule CreateRecordOnceSchedule(ISchedulerService tvSchedulerAgent, IGuideService tvGuideAgent, Guid channelId, Guid guideProgramId)
        {
            GuideProgram guideProgram = tvGuideAgent.GetProgramById(guideProgramId);
            if (guideProgram != null)
            {
                return GuideController.CreateRecordOnceSchedule(tvSchedulerAgent, _model.ChannelType, channelId, guideProgram.Title, guideProgram.SubTitle, guideProgram.EpisodeNumberDisplay, guideProgram.StartTime);
            }
            return null;
        }

        public static Schedule CreateRecordOnceSchedule(ISchedulerService tvSchedulerAgent, ChannelType channelType, Guid channelId, string title, string subTitle, string episodeNumber, DateTime startTime)
        {
            Schedule schedule = tvSchedulerAgent.CreateNewSchedule(channelType, ScheduleType.Recording);
            schedule.Name = GuideProgram.CreateProgramTitle(title, subTitle, episodeNumber);
            schedule.Rules.Add(ScheduleRuleType.Channels, channelId);
            schedule.Rules.Add(ScheduleRuleType.OnDate, startTime.Date);
            schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            schedule.Rules.Add(ScheduleRuleType.TitleEquals, title);
            if (!String.IsNullOrEmpty(subTitle))
            {
                schedule.Rules.Add(ScheduleRuleType.SubTitleEquals, subTitle);
            }
            else if (!String.IsNullOrEmpty(episodeNumber))
            {
                schedule.Rules.Add(ScheduleRuleType.EpisodeNumberEquals, episodeNumber);
            }
            return schedule;
        }

        public enum RepeatingType
        {
            Daily,
            Weekly,
            MonFri,
            SatSun,
            AnyTime,
            AnyTimeThisChannel
        }

        public Schedule CreateRecordRepeatingSchedule(ISchedulerService tvSchedulerAgent, IGuideService tvGuideAgent, RepeatingType repeatingType, Guid? channelId, Guid guideProgramId)
        {
            GuideProgram guideProgram = tvGuideAgent.GetProgramById(guideProgramId);
            if (guideProgram != null)
            {
                return GuideController.CreateRecordRepeatingSchedule(tvSchedulerAgent, repeatingType, _model.ChannelType, channelId, guideProgram.Title, guideProgram.StartTime,string.Empty);
            }
            return null;
        }

        public static Schedule CreateRecordRepeatingSchedule(ISchedulerService tvSchedulerAgent, RepeatingType repeatingType, ChannelType channelType,
            Guid? channelId, string title, DateTime startTime)
        {
            return CreateRecordRepeatingSchedule(tvSchedulerAgent, repeatingType, channelType, channelId, title, startTime, string.Empty);
        }

        public static Schedule CreateRecordRepeatingSchedule(ISchedulerService tvSchedulerAgent, RepeatingType repeatingType, ChannelType channelType,
            Guid? channelId, string title, DateTime startTime, string repeatingTime)
        {
            Schedule schedule = tvSchedulerAgent.CreateNewSchedule(channelType, ScheduleType.Recording);

            if (repeatingType == RepeatingType.AnyTime || 
                repeatingType == RepeatingType.AnyTimeThisChannel)
            {
                if (String.IsNullOrEmpty(repeatingTime)) 
                    repeatingTime = "(Any Time)";
                schedule.Name = title + " " + repeatingTime;
                schedule.Rules.Add(ScheduleRuleType.NewEpisodesOnly, true);
            }
            else if (repeatingType == RepeatingType.Weekly)
            {
                if (String.IsNullOrEmpty(repeatingTime))
                    repeatingTime = "(Weekly)";
                schedule.Name = title + " " + repeatingTime;
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, GetDaysOfWeek(startTime));
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            }
            else if (repeatingType == RepeatingType.MonFri)
            {
                if (String.IsNullOrEmpty(repeatingTime))
                    repeatingTime = "(Mon-Fri)";
                schedule.Name = title + " " + repeatingTime;
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, ScheduleDaysOfWeek.WorkingDays);
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            }
            else if (repeatingType == RepeatingType.SatSun)
            {
                if (String.IsNullOrEmpty(repeatingTime))
                    repeatingTime = "(Sat-Sun)";
                schedule.Name = title + " " + repeatingTime;
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, ScheduleDaysOfWeek.Weekends);
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            }
            else
            {
                if (String.IsNullOrEmpty(repeatingTime))
                    repeatingTime = "(Daily)";
                schedule.Name = title + " " + repeatingTime;
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            }

            if (channelId != null)
            {
                schedule.Rules.Add(ScheduleRuleType.Channels, channelId);
            }
            schedule.Rules.Add(ScheduleRuleType.TitleEquals, title);
            return schedule;
        }

        private static ScheduleDaysOfWeek GetDaysOfWeek(DateTime dateTime)
        {
            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return ScheduleDaysOfWeek.Mondays;
                case DayOfWeek.Tuesday:
                    return ScheduleDaysOfWeek.Tuesdays;
                case DayOfWeek.Wednesday:
                    return ScheduleDaysOfWeek.Wednesdays;
                case DayOfWeek.Thursday:
                    return ScheduleDaysOfWeek.Thursdays;
                case DayOfWeek.Friday:
                    return ScheduleDaysOfWeek.Fridays;
                case DayOfWeek.Saturday:
                    return ScheduleDaysOfWeek.Saturdays;
                case DayOfWeek.Sunday:
                    return ScheduleDaysOfWeek.Sundays;
            }
            return ScheduleDaysOfWeek.None;
        }
    }
}
