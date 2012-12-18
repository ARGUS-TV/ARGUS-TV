using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ArgusTV.DataContracts;
using ArgusTV.ServiceContracts;

namespace ArgusTV.Client.Common
{
    public static class SchedulesHelper
    {
        public static Schedule CreateRecordOnceSchedule(ISchedulerService tvSchedulerAgent, IGuideService tvGuideAgent,
            ChannelType channelType, Guid channelId, Guid guideProgramId)
        {
            GuideProgram guideProgram = tvGuideAgent.GetProgramById(guideProgramId);
            if (guideProgram != null)
            {
                return CreateRecordOnceSchedule(tvSchedulerAgent, channelType,
                    channelId, guideProgram.Title, guideProgram.SubTitle, guideProgram.EpisodeNumberDisplay, guideProgram.StartTime);
            }
            return null;
        }

        public static Schedule CreateRecordOnceSchedule(ISchedulerService tvSchedulerAgent,
            ChannelType channelType, Guid channelId, string title, string subTitle, string episodeNumber, DateTime startTime)
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
            AnyTime
        }

        public static Schedule CreateRecordRepeatingSchedule(ISchedulerService tvSchedulerAgent, IGuideService tvGuideAgent,
            RepeatingType repeatingType, ChannelType channelType, Guid channelId, Guid guideProgramId, string titleSuffix = null)
        {
            GuideProgram guideProgram = tvGuideAgent.GetProgramById(guideProgramId);
            if (guideProgram != null)
            {
                return CreateRecordRepeatingSchedule(tvSchedulerAgent, repeatingType, channelType, channelId, guideProgram.Title, guideProgram.StartTime, titleSuffix);
            }
            return null;
        }

        public static Schedule CreateRecordRepeatingSchedule(ISchedulerService tvSchedulerAgent, RepeatingType repeatingType, ChannelType channelType,
            Guid channelId, string title, DateTime startTime, string titleSuffix = null)
        {
            Schedule schedule = tvSchedulerAgent.CreateNewSchedule(channelType, ScheduleType.Recording);

            if (repeatingType == RepeatingType.AnyTime)
            {
                schedule.Name = title + (titleSuffix ?? " (Any Time)");
                schedule.Rules.Add(ScheduleRuleType.NewEpisodesOnly, true);
            }
            else if (repeatingType == RepeatingType.Weekly)
            {
                schedule.Name = title + (titleSuffix ?? " (Weekly)");
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, GetDaysOfWeek(startTime));
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            }
            else if (repeatingType == RepeatingType.MonFri)
            {
                schedule.Name = title + (titleSuffix ?? " (Mon-Fri)");
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, ScheduleDaysOfWeek.WorkingDays);
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            }
            else if (repeatingType == RepeatingType.SatSun)
            {
                schedule.Name = title + (titleSuffix ?? " (Sat-Sun)");
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, ScheduleDaysOfWeek.Weekends);
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            }
            else
            {
                schedule.Name = title + (titleSuffix ?? " (Daily)");
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(startTime.TimeOfDay));
            }

            schedule.Rules.Add(ScheduleRuleType.Channels, channelId);
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
