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
using System.Globalization;
using System.Text;
using System.Net;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.Messenger
{
    internal class IMCommands
    {
        private static class SessionKey
        {
            public const string FoundTitles = "IMBot.FoundTitles";
            public const string Programs = "IMBot.Programs";
            public const string ChannelType = "IMBot.ChannelType";
        }

        public IMCommands()
        {
            _commandParser.AddCommand("alerts", "al", this.DoAlertsCommand);
            _commandParser.AddCommand("notifications", "not", this.DoNotificationsCommand);
            _commandParser.AddCommand("help", "h", this.DoHelpCommand);
            _commandParser.AddCommand("status", "?", this.DoStatusCommand);
            _commandParser.AddCommand("record", "r", this.DoRecordCommand);
            _commandParser.AddCommand("cancel", "ca", delegate(IMBotConversation conversation, IList<string> arguments) { return DoCancelCommand(conversation, arguments, true); });
            _commandParser.AddCommand("uncancel", "un", delegate(IMBotConversation conversation, IList<string> arguments) { return DoCancelCommand(conversation, arguments, false); });
            _commandParser.AddCommand("television", "tv", this.DoTelevisionCommand);
            _commandParser.AddCommand("radio", this.DoRadioCommand);
            CommandParser showParser = _commandParser.AddCommand("show");
            showParser.AddCommand("groups", "gr", this.DoShowChannelGroupsCommand);
            showParser.AddCommand("channels", "ch", this.DoShowChannelsCommand);
            showParser.AddCommand("guide", "g", this.DoShowGuideCommand);
            _commandParser.AddCommand("gr", this.DoShowChannelGroupsCommand);
            _commandParser.AddCommand("ch", this.DoShowChannelsCommand);
            _commandParser.AddCommand("g", this.DoShowGuideCommand);
            AddUpcomingIMCommands(showParser.AddCommand("upcoming", "up"));
            AddUpcomingIMCommands(_commandParser.AddCommand("up"));
            _commandParser.AddCommand("search", "s", this.DoSearchCommand);
            _commandParser.AddCommand("del", this.DoDeleteScheduleCommand);
            CommandParser deleteParser = _commandParser.AddCommand("delete");
            deleteParser.AddCommand("schedule", this.DoDeleteScheduleCommand);
        }

        private void AddUpcomingIMCommands(CommandParser showUpcomingParser)
        {
            showUpcomingParser.AddCommand("recordings", "r", delegate(IMBotConversation conversation, IList<string> arguments) { return DoShowUpcomingCommand(conversation, ScheduleType.Recording); });
            showUpcomingParser.AddCommand("alerts", "a", delegate(IMBotConversation conversation, IList<string> arguments) { return DoShowUpcomingCommand(conversation, ScheduleType.Alert); });
            showUpcomingParser.AddCommand("suggestions", "s", delegate(IMBotConversation conversation, IList<string> arguments) { return DoShowUpcomingCommand(conversation, ScheduleType.Suggestion); });
        }

        public IMBotMessage ProcessIMCommand(IMBotConversation conversation, string commandLine)
        {
            IMBotMessage reply = new IMBotMessage();
            if (!Proxies.IsInitialized)
            {
                reply.BodyText = "Not connected to ARGUS TV, check configuration.";
                reply.TextColor = IMBotMessage.ErrorColor;
            }
            else
            {
                try
                {
                    commandLine = commandLine.TrimStart();
                    reply = _commandParser.ProcessIMCommand(conversation, ArgusTV.Common.Utility.ParseCommandArguments(commandLine));
                }
                catch
                {
                    reply.BodyText = "Internal error occurred.";
                    reply.TextColor = IMBotMessage.ErrorColor;
                }
            }
            return reply;
        }

        private ChannelType GetChannelType(IMBotConversation conversation)
        {
            if (conversation.Session.ContainsKey(SessionKey.ChannelType))
            {
                return (ChannelType)conversation.Session[SessionKey.ChannelType];
            }
            return ChannelType.Television;
        }

        private void SetChannelType(IMBotConversation conversation, ChannelType channelType)
        {
            conversation.Session[SessionKey.ChannelType] = channelType;
        }

        private IMBotMessage DoTelevisionCommand(IMBotConversation conversation, IList<string> arguments)
        {
            SetChannelType(conversation, ChannelType.Television);
            return new IMBotMessage("Now in television mode.");
        }

        private IMBotMessage DoRadioCommand(IMBotConversation conversation, IList<string> arguments)
        {
            SetChannelType(conversation, ChannelType.Radio);
            return new IMBotMessage("Now in radio mode.");
        }

        private IMBotMessage DoAlertsCommand(IMBotConversation conversation, IList<string> arguments)
        {
            string reply = Msn.MsnThread.HandleAlertsCommand(conversation, arguments);
            return new IMBotMessage(reply);
        }

        private IMBotMessage DoNotificationsCommand(IMBotConversation conversation, IList<string> arguments)
        {
            string reply = Msn.MsnThread.HandleNotificationsCommand(conversation, arguments);
            return new IMBotMessage(reply);
        }

        private IMBotMessage DoHelpCommand(IMBotConversation conversation, IList<string> arguments)
        {
            return new IMBotMessage("List of commands:" + Environment.NewLine
                + " ● ?,status" + Environment.NewLine
                + " ● al,alerts [ on | off ]" + Environment.NewLine
                + " ● ca,cancel <program-number>" + Environment.NewLine
                + " ● del,delete schedule <program-number>" + Environment.NewLine
                + " ● h,help" + Environment.NewLine
                + " ● m,more" + Environment.NewLine
                + " ● not,notifications [ on | off ]" + Environment.NewLine
                + " ● r,record <program-number> { o,once | d,daily | w,weekly | wd,workingdays | we,weekends | a,anytime } [ new | all ]" + Environment.NewLine
                + "   (default is all episodes)" + Environment.NewLine
                + " ● s,search <text> | <title-number> (start with \\ to search for numbers, e.g. 's \\24')" + Environment.NewLine
                + " ● ch,show channels <group>" + Environment.NewLine
                + " ● gr,show groups" + Environment.NewLine
                + " ● g,show guide <channel> [day-number]" + Environment.NewLine
                + " ● up,show upcoming { r,recordings | a,alerts | s,suggestions }" + Environment.NewLine
                + " ● tv,television | radio" + Environment.NewLine
                + " ● un,uncancel <program-number>");
        }

        private IMBotMessage DoStatusCommand(IMBotConversation conversation, IList<string> arguments)
        {
            bool fixedWidth = false;

            // Check if currently recording :
            var controlProxy = Proxies.ControlService;
            var activeRecordings = controlProxy.GetActiveRecordings();
            var liveStreams = controlProxy.GetLiveStreams();
            UpcomingRecording upcomingRecording = controlProxy.GetNextUpcomingRecording(false);

            StringBuilder reply = new StringBuilder();

            if (activeRecordings.Count > 0)
            {
                reply.Append("Currently recording:");
                foreach (ActiveRecording activeRecording in activeRecordings)
                {
                    PluginService pluginService = null;
                    if (activeRecording.CardChannelAllocation != null)
                    {
                        pluginService =
                            RecorderTunersCache.GetRecorderTunerById(activeRecording.CardChannelAllocation.RecorderTunerId);
                    }

                    reply.AppendLine();
                    Utility.AppendProgramDetails(reply, activeRecording.Program.Channel, activeRecording.Program);
                    reply.AppendFormat(" [{0}]", pluginService == null ? "-" : pluginService.Name);
                }
                fixedWidth = true;

                if (liveStreams.Count > 0
                    || upcomingRecording != null)
                {
                    reply.AppendLine();
                }
            }
            if (liveStreams.Count > 0)
            {
                reply.Append("Currently streaming:");
                foreach (LiveStream liveStream in liveStreams)
                {
                    reply.AppendLine();
                    reply.AppendFormat("[{0}]", liveStream.Channel.DisplayName);
                }
                fixedWidth = true;

                if (upcomingRecording != null)
                {
                    reply.AppendLine();
                }
            }
            if (upcomingRecording != null)
            {
                if (reply.Length == 0)
                {
                    reply.AppendLine("Idle, next scheduled recording:");
                }
                else
                {
                    reply.AppendLine("Next scheduled recording:");
                }

                PluginService pluginService = null;
                if (upcomingRecording.CardChannelAllocation != null)
                {
                    pluginService = RecorderTunersCache.GetRecorderTunerById(upcomingRecording.CardChannelAllocation.RecorderTunerId);
                }

                Utility.AppendProgramDetails(reply, upcomingRecording.Program.Channel, upcomingRecording.Program);
                reply.AppendFormat(" [{0}]", pluginService == null ? "-" : pluginService.Name);

                fixedWidth = true;
            }
            if (reply.Length == 0)
            {
                reply.Append("Idle");
            }
            reply.AppendLine().AppendLine();
            reply.Append("ARGUS TV Messenger " + Constants.ProductVersion + @", running on server \\").AppendLine(Dns.GetHostName());
            reply.Append("http://www.argus-tv.com");
            return new IMBotMessage(reply.ToString(), fixedWidth);
        }

        private IMBotMessage DoShowUpcomingCommand(IMBotConversation conversation, ScheduleType type)
        {
            if (type == ScheduleType.Recording)
            {
                var upcomingRecordings = Proxies.ControlService.GetAllUpcomingRecordings(UpcomingRecordingsFilter.Recordings, false);

                StringBuilder replyText = new StringBuilder();

                if (upcomingRecordings.Count > 0)
                {
                    int index = 0;
                    foreach (UpcomingRecording upcomingRecording in upcomingRecordings)
                    {
                        if (replyText.Length > 0)
                        {
                            replyText.AppendLine();
                        }

                        PluginService pluginService = null;
                        if (upcomingRecording.CardChannelAllocation != null)
                        {
                            pluginService =
                                RecorderTunersCache.GetRecorderTunerById(upcomingRecording.CardChannelAllocation.RecorderTunerId);
                        }

                        replyText.AppendFormat("{0,3}» ", ++index);
                        Utility.AppendProgramDetails(replyText, upcomingRecording.Program.Channel, upcomingRecording.Program);
                        replyText.AppendFormat(" [{0}]", pluginService == null ? "-" : pluginService.Name);
                    }

                    conversation.Session[SessionKey.Programs] = new Session.Programs(upcomingRecordings);

                    return new IMBotMessage(replyText.ToString(), true)
                    {
                        Footer = "Use 'cancel', 'uncancel' or 'delete schedule' with <number>."
                    };
                }
            }
            else
            {
                var upcomingPrograms = Proxies.SchedulerService.GetAllUpcomingPrograms(type, false);

                StringBuilder replyText = new StringBuilder();

                if (upcomingPrograms.Count > 0)
                {
                    int index = 0;
                    foreach (UpcomingProgram upcomingProgram in upcomingPrograms)
                    {
                        if (replyText.Length > 0)
                        {
                            replyText.AppendLine();
                        }
                        replyText.AppendFormat("{0,3}» ", ++index);
                        Utility.AppendProgramDetails(replyText, upcomingProgram.Channel, upcomingProgram);
                    }

                    conversation.Session[SessionKey.Programs] = new Session.Programs(upcomingPrograms);

                    return new IMBotMessage(replyText.ToString(), true)
                    {
                        Footer = "Use 'record', 'cancel', 'uncancel' or 'delete schedule' with <number>."
                    };
                }
            }
            return new IMBotMessage("There are no upcoming " + type.ToString().ToLowerInvariant() + "s.");
        }

        private IMBotMessage DoShowChannelGroupsCommand(IMBotConversation conversation, IList<string> arguments)
        {
            List<ChannelGroup> groups = GetAllGroups(GetChannelType(conversation));

            StringBuilder replyText = new StringBuilder();

            int index = 1;
            foreach (ChannelGroup group in groups)
            {
                if (replyText.Length > 0)
                {
                    replyText.AppendLine();
                }
                replyText.AppendFormat("{0,3}» {1}", index++, group.GroupName);
            }

            return new IMBotMessage(replyText.ToString(), true)
            {
                Footer = "Use 'show channels <number | name>' to see the programs in a group."
            };
        }

        private IMBotMessage DoShowChannelsCommand(IMBotConversation conversation, IList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                return new IMBotMessage("Group name or number missing.", IMBotMessage.ErrorColor);
            }

            List<ChannelGroup> groups = GetAllGroups(GetChannelType(conversation));

            ChannelGroup group = null;

            int index;
            if (int.TryParse(arguments[0], out index))
            {
                if (index < 1 || index > groups.Count)
                {
                    return new IMBotMessage("Unknown group number.", IMBotMessage.ErrorColor);
                }
                group = groups[index - 1];
            }
            else
            {
                foreach (ChannelGroup channelGroup in groups)
                {
                    if (channelGroup.GroupName.Equals(arguments[0], StringComparison.CurrentCultureIgnoreCase))
                    {
                        group = channelGroup;
                        break;
                    }
                }
                if (group == null)
                {
                    return new IMBotMessage("Unknown group name.", IMBotMessage.ErrorColor);
                }
            }

            var channels = Proxies.SchedulerService.GetChannelsInGroup(group.ChannelGroupId, true);

            StringBuilder replyText = new StringBuilder();
            replyText.AppendFormat("Channels in {0}:", group.GroupName);

            foreach (Channel channel in channels)
            {
                replyText.AppendLine();
                replyText.AppendFormat("{0,3} {1}",
                    channel.LogicalChannelNumber.HasValue ? channel.LogicalChannelNumber.Value.ToString() : "-",
                    channel.DisplayName);
            }

            return new IMBotMessage(replyText.ToString(), true)
            {
                Footer = "Use 'show guide <number | name> [day-number]' to see the channel guide."
            };
        }

        private IMBotMessage DoShowGuideCommand(IMBotConversation conversation, IList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                return new IMBotMessage("Channel name or number missing.", IMBotMessage.ErrorColor);
            }

            Channel selectedChannel = null;
            ChannelType channelType = GetChannelType(conversation);

            int lcn;
            if (int.TryParse(arguments[0], out lcn))
            {
                selectedChannel = Proxies.SchedulerService.GetChannelByLogicalChannelNumber(channelType, lcn);
                if (selectedChannel == null)
                {
                    return new IMBotMessage("Unknown channel number.", IMBotMessage.ErrorColor);
                }
            }
            else
            {
                selectedChannel = Proxies.SchedulerService.GetChannelByDisplayName(channelType, arguments[0]);
                if (selectedChannel == null)
                {
                    return new IMBotMessage("Unknown channel name.", IMBotMessage.ErrorColor);
                }
            }

            if (selectedChannel.GuideChannelId.HasValue)
            {
                DateTime lowerTime = DateTime.Today;
                if (arguments.Count > 1)
                {
                    int dayNumber;
                    if (!int.TryParse(arguments[1], out dayNumber))
                    {
                        return new IMBotMessage("Bad day number, use 0 for today, 1 for tomorrow, etc...", IMBotMessage.ErrorColor);
                    }
                    lowerTime = lowerTime.AddDays(dayNumber);
                }
                    
                DateTime upperTime = lowerTime.AddDays(1);
                if (lowerTime.Date == DateTime.Today)
                {
                    lowerTime = DateTime.Now;
                }

                Dictionary<Guid, UpcomingGuideProgram> upcomingRecordingsById = BuildUpcomingDictionary(
                    Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Recording, true));
                Dictionary<Guid, UpcomingGuideProgram> upcomingAlertsById = BuildUpcomingDictionary(
                    Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Alert, false));
                Dictionary<Guid, UpcomingGuideProgram> upcomingSuggestionsById = BuildUpcomingDictionary(
                    Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Suggestion, false));

                var programs = Proxies.GuideService.GetChannelProgramsBetween(selectedChannel.GuideChannelId.Value, lowerTime, upperTime);
                if (programs.Count == 0)
                {
                    return new IMBotMessage(String.Format(CultureInfo.CurrentCulture, "No guide data for {0} on {1}.", selectedChannel.DisplayName, lowerTime.ToLongDateString()),
                        IMBotMessage.ErrorColor);
                }
                else
                {
                    StringBuilder replyText = new StringBuilder();
                    replyText.AppendFormat("{0} on {1}:", lowerTime.ToLongDateString(), selectedChannel.DisplayName);
                    int index = 0;
                    foreach (GuideProgramSummary program in programs)
                    {
                        replyText.AppendLine();
                        replyText.AppendFormat("{0,3}» ", ++index);
                        string appendText = AppendProgramIndicatorsPrefix(replyText,
                            program.GetUniqueUpcomingProgramId(selectedChannel.ChannelId),
                            upcomingRecordingsById, upcomingAlertsById, upcomingSuggestionsById);
                        Utility.AppendProgramDetails(replyText, program);
                        replyText.Append(appendText);
                    }

                    conversation.Session[SessionKey.Programs] = new Session.Programs(selectedChannel, programs);

                    return new IMBotMessage(replyText.ToString(), true)
                    {
                        Footer = "Use 'record', 'cancel', 'uncancel' or 'delete schedule' with <number>."
                    };
                }
            }
            else
            {
                return new IMBotMessage("Channel has no guide data.", IMBotMessage.ErrorColor);
            }
        }

        private enum RepeatingType
        {
            None,
            Once,
            Daily,
            Weekly,
            WorkingDays,
            Weekends,
            AnyTime
        }

        private IMBotMessage DoRecordCommand(IMBotConversation conversation, IList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                return new IMBotMessage("Program number missing.", IMBotMessage.ErrorColor);
            }

            RepeatingType repeatingType = RepeatingType.None;
            if (arguments.Count > 1)
            {
                switch (arguments[1].ToLowerInvariant())
                {
                    case "once": case "o": repeatingType = RepeatingType.Once; break;
                    case "daily": case "d": repeatingType = RepeatingType.Daily; break;
                    case "weekly": case "w": repeatingType = RepeatingType.Weekly; break;
                    case "workingdays": case "wd": repeatingType = RepeatingType.WorkingDays; break;
                    case "weekends": case "we": repeatingType = RepeatingType.Weekends; break;
                    case "anytime": case "a": repeatingType = RepeatingType.AnyTime; break;
                }
            }

            int programNumber;
            if (repeatingType == RepeatingType.None
                || !int.TryParse(arguments[0], out programNumber))
            {
                return new IMBotMessage("Please specify program number and once, daily, weekly, workingdays, weekends or anytime.", IMBotMessage.ErrorColor);
            }

            Session.Programs sessionPrograms = null;
            if (conversation.Session.ContainsKey(SessionKey.Programs))
            {
                sessionPrograms = conversation.Session[SessionKey.Programs] as Session.Programs;
            }

            IProgramSummary program = null;
            Channel channel = null;

            if (sessionPrograms != null)
            {
                program = sessionPrograms.GetProgramAt(programNumber, out channel);
                if (program == null)
                {
                    return new IMBotMessage("Bad program number.", IMBotMessage.ErrorColor);
                }
            }
            else
            {
                return new IMBotMessage("No programs.", IMBotMessage.ErrorColor);
            }

            Schedule schedule = Proxies.SchedulerService.CreateNewSchedule(GetChannelType(conversation), ScheduleType.Recording);

            bool newEpisodesOnly = arguments.Count > 2 && arguments[2].Equals("new", StringComparison.CurrentCultureIgnoreCase);
            string repeatingText = String.Empty;

            if (repeatingType == RepeatingType.Once)
            {
                schedule.Name = GuideProgram.CreateProgramTitle(program.Title, program.SubTitle, program.EpisodeNumberDisplay);
                schedule.Rules.Add(ScheduleRuleType.OnDate, program.StartTime.Date);
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(program.StartTime.TimeOfDay));
                if (!String.IsNullOrEmpty(program.SubTitle))
                {
                    schedule.Rules.Add(ScheduleRuleType.SubTitleEquals, program.SubTitle);
                }
                else if (!String.IsNullOrEmpty(program.EpisodeNumberDisplay))
                {
                    schedule.Rules.Add(ScheduleRuleType.EpisodeNumberEquals, program.EpisodeNumberDisplay);
                }
                newEpisodesOnly = false;
            }
            else if (repeatingType == RepeatingType.AnyTime)
            {
                schedule.Name = program.Title + " (Any Time)";
                repeatingText = " any time";
            }
            else if (repeatingType == RepeatingType.Weekly)
            {
                schedule.Name = program.Title + " (Weekly)";
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, GetDaysOfWeek(program.StartTime));
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(program.StartTime.TimeOfDay));
                repeatingText = " weekly";
            }
            else if (repeatingType == RepeatingType.WorkingDays)
            {
                schedule.Name = program.Title + " (Mon-Fri)";
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, ScheduleDaysOfWeek.WorkingDays);
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(program.StartTime.TimeOfDay));
                repeatingText = " Mon-Fri";
            }
            else if (repeatingType == RepeatingType.Weekends)
            {
                schedule.Name = program.Title + " (Sat-Sun)";
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, ScheduleDaysOfWeek.Weekends);
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(program.StartTime.TimeOfDay));
                repeatingText = " Sat-Sun";
            }
            else if (repeatingType == RepeatingType.Weekly)
            {
                schedule.Name = program.Title + " (Weekly)";
                schedule.Rules.Add(ScheduleRuleType.DaysOfWeek, GetDaysOfWeek(program.StartTime));
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(program.StartTime.TimeOfDay));
                repeatingText = " weekly";
            }
            else
            {
                schedule.Name = program.Title + " (Daily)";
                schedule.Rules.Add(ScheduleRuleType.AroundTime, new ScheduleTime(program.StartTime.TimeOfDay));
                repeatingText = " daily";
            }
            if (newEpisodesOnly)
            {
                schedule.Rules.Add(ScheduleRuleType.NewEpisodesOnly, true);
            }
            schedule.Rules.Add(ScheduleRuleType.Channels, channel.ChannelId);
            schedule.Rules.Add(ScheduleRuleType.TitleEquals, program.Title);
            Proxies.SchedulerService.SaveSchedule(schedule);

            StringBuilder replyText = new StringBuilder();
            replyText.Append("Created schedule to record ");
            Utility.AppendProgramDetails(replyText, channel, program);
            replyText.Append(repeatingText);
            if (newEpisodesOnly)
            {
                replyText.Append(" (record episodes once)");
            }
            replyText.Append(".");

            return new IMBotMessage(replyText.ToString());
        }

        private IMBotMessage DoSearchCommand(IMBotConversation conversation, IList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                return new IMBotMessage("Search text or result number is missing.", IMBotMessage.ErrorColor);
            }

            int resultNumber;
            if (int.TryParse(arguments[0], out resultNumber))
            {
                return DoSearchResults(conversation, resultNumber);
            }

            string searchText = arguments[0];
            if (searchText.StartsWith(@"\"))
            {
                searchText = searchText.Substring(1);
            }

            var titles = Proxies.SchedulerService.GetTitlesByPartialTitle(GetChannelType(conversation), searchText, false);

            StringBuilder replyText = new StringBuilder();
            replyText.AppendFormat("Found {0} in the following titles:", searchText);
            int index = 0;
            foreach (string title in titles)
            {
                replyText.AppendLine();
                replyText.AppendFormat("{0,3}» {1}", ++index, title);
            }
                
            conversation.Session.Remove(SessionKey.Programs);
            conversation.Session[SessionKey.FoundTitles] = titles;

            return new IMBotMessage(replyText.ToString(), true)
            {
                Footer = "Use 'search <number>' to see the programs."
            };
        }

        private IMBotMessage DoSearchResults(IMBotConversation conversation, int resultNumber)
        {
            if (!conversation.Session.ContainsKey(SessionKey.FoundTitles))
            {
                return new IMBotMessage("No search results found, use search command.", IMBotMessage.ErrorColor);
            }

            string[] titles = (string[])conversation.Session[SessionKey.FoundTitles];

            if (resultNumber < 1
                || resultNumber > titles.Length)
            {
                return new IMBotMessage("Bad search result number.", IMBotMessage.ErrorColor);
            }

            Dictionary<Guid, UpcomingGuideProgram> upcomingRecordingsById = BuildUpcomingDictionary(
                Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Recording, true));
            Dictionary<Guid, UpcomingGuideProgram> upcomingAlertsById = BuildUpcomingDictionary(
                Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Alert, false));
            Dictionary<Guid, UpcomingGuideProgram> upcomingSuggestionsById = BuildUpcomingDictionary(
                Proxies.SchedulerService.GetUpcomingGuidePrograms(ScheduleType.Suggestion, false));

            var programs = Proxies.SchedulerService.SearchGuideByTitle(GetChannelType(conversation), titles[resultNumber - 1], false);

            StringBuilder replyText = new StringBuilder();
            int index = 0;
            foreach (ChannelProgram program in programs)
            {
                if (replyText.Length > 0)
                {
                    replyText.AppendLine();
                }
                replyText.AppendFormat("{0,3}» ", ++index);
                string appendText = AppendProgramIndicatorsPrefix(replyText, program.GetUniqueUpcomingProgramId(),
                    upcomingRecordingsById, upcomingAlertsById, upcomingSuggestionsById);
                Utility.AppendProgramDetails(replyText, program.Channel, program);
                replyText.Append(appendText);
            }

            conversation.Session[SessionKey.Programs] = new Session.Programs(programs);

            return new IMBotMessage(replyText.ToString(), true)
            {
                Footer = "Use 'record', 'cancel', 'uncancel' or 'delete schedule' with <number>."
            };
        }

        private IMBotMessage DoCancelCommand(IMBotConversation conversation, IList<string> arguments, bool cancel)
        {
            if (arguments.Count == 0)
            {
                return new IMBotMessage("Program number is missing.", IMBotMessage.ErrorColor);
            }

            UpcomingProgram upcomingRecording;
            IMBotMessage result = FindUpcomingRecording(conversation, arguments, out upcomingRecording);
            if (result == null)
            {
                StringBuilder replyText = new StringBuilder();
                if (cancel)
                {
                    if (upcomingRecording.IsPartOfSeries)
                    {
                        Proxies.SchedulerService.CancelUpcomingProgram(upcomingRecording.ScheduleId, upcomingRecording.GuideProgramId,
                            upcomingRecording.Channel.ChannelId, upcomingRecording.StartTime);
                        replyText.Append("Cancelled ");
                    }
                    else
                    {
                        Proxies.SchedulerService.DeleteSchedule(upcomingRecording.ScheduleId);
                        replyText.Append("Deleted schedule for ");
                    }
                }
                else
                {
                    Proxies.SchedulerService.UncancelUpcomingProgram(upcomingRecording.ScheduleId, upcomingRecording.GuideProgramId,
                        upcomingRecording.Channel.ChannelId, upcomingRecording.StartTime);
                    replyText.Append("Uncancelled ");
                }

                Utility.AppendProgramDetails(replyText, upcomingRecording.Channel, upcomingRecording);
                replyText.Append(".");

                result = new IMBotMessage(replyText.ToString());
            }
            return result;
        }

        private IMBotMessage DoDeleteScheduleCommand(IMBotConversation conversation, IList<string> arguments)
        {
            if (arguments.Count == 0)
            {
                return new IMBotMessage("Program number is missing.", IMBotMessage.ErrorColor);
            }

            UpcomingProgram upcomingRecording;
            IMBotMessage result = FindUpcomingRecording(conversation, arguments, out upcomingRecording);
            if (result == null)
            {
                StringBuilder replyText = new StringBuilder();
                Proxies.SchedulerService.DeleteSchedule(upcomingRecording.ScheduleId);
                replyText.Append("Deleted schedule for ");
                Utility.AppendProgramDetails(replyText, upcomingRecording.Channel, upcomingRecording);
                replyText.Append(".");
                result = new IMBotMessage(replyText.ToString());
            }
            return result;
        }

        #region Parser

        private delegate IMBotMessage HandleCommandDelegate(IMBotConversation conversation, IList<string> arguments);

        private CommandParser _commandParser = new CommandParser();

        private class CommandParser
        {
            private HandleCommandDelegate _handleCommandDelegate;
            private Dictionary<string, CommandParser> _subCommandParser;
            private Dictionary<string, CommandParser> _subCommandShortcutParser;

            public CommandParser()
            {
                _subCommandParser = new Dictionary<string, CommandParser>();
                _subCommandShortcutParser = new Dictionary<string, CommandParser>();
            }

            public CommandParser(HandleCommandDelegate handleCommandDelegate)
            {
                _handleCommandDelegate = handleCommandDelegate;
            }

            public CommandParser(Dictionary<string, CommandParser> subCommandParser)
            {
                _subCommandParser = subCommandParser;
            }

            public IMBotMessage ProcessIMCommand(IMBotConversation conversation, IList<string> args)
            {
                List<string> arguments = new List<string>(args);

                if (_handleCommandDelegate != null)
                {
                    return _handleCommandDelegate(conversation, args);
                }
                else if (arguments.Count > 0)
                {
                    string commandName = arguments[0].ToLowerInvariant();
                    arguments.RemoveAt(0);
                    if (_subCommandParser.ContainsKey(commandName))
                    {
                        return _subCommandParser[commandName].ProcessIMCommand(conversation, arguments);
                    }
                    else if (_subCommandShortcutParser.ContainsKey(commandName))
                    {
                        return _subCommandShortcutParser[commandName].ProcessIMCommand(conversation, arguments);
                    }
                }

                return new IMBotMessage("Unknown command, type help for a list of commands.", IMBotMessage.ErrorColor);
            }

            public CommandParser AddCommand(string commandName)
            {
                return AddCommand(commandName, String.Empty);
            }

            public CommandParser AddCommand(string commandName, string shortcut)
            {
                CommandParser subCommandParser = new CommandParser();
                AddCommand(commandName, shortcut, subCommandParser);
                return subCommandParser;
            }

            public void AddCommand(string commandName, HandleCommandDelegate handleCommandDelegate)
            {
                AddCommand(commandName, String.Empty, handleCommandDelegate);
            }

            public void AddCommand(string commandName, string shortcut, HandleCommandDelegate handleCommandDelegate)
            {
                AddCommand(commandName, shortcut, new CommandParser(handleCommandDelegate));
            }

            private void AddCommand(string commandName, string shortcut, CommandParser commandParser)
            {
                _subCommandParser.Add(commandName.ToLowerInvariant(), commandParser);
                if (!String.IsNullOrEmpty(shortcut))
                {
                    _subCommandShortcutParser.Add(shortcut.ToLowerInvariant(), commandParser);
                }
            }
        }

        #endregion

        #region Miscellanous

        private static IMBotMessage FindUpcomingRecording(IMBotConversation conversation, IList<string> arguments, out UpcomingProgram upcomingRecording)
        {
            upcomingRecording = null;

            IProgramSummary program = null;
            Channel channel = null;
            Guid? upcomingProgramId = null;

            int programNumber;
            if (!int.TryParse(arguments[0], out programNumber))
            {
                return new IMBotMessage("Bad program number.", IMBotMessage.ErrorColor);
            }

            Session.Programs sessionPrograms = null;
            if (conversation.Session.ContainsKey(SessionKey.Programs))
            {
                sessionPrograms = conversation.Session[SessionKey.Programs] as Session.Programs;
            }
            if (sessionPrograms != null)
            {
                program = sessionPrograms.GetProgramAt(programNumber, out channel, out upcomingProgramId);
                if (program == null)
                {
                    return new IMBotMessage("Bad program number.", IMBotMessage.ErrorColor);
                }
            }
            else
            {
                return new IMBotMessage("No programs.", IMBotMessage.ErrorColor);
            }

            var upcomingPrograms = Proxies.SchedulerService.GetAllUpcomingPrograms(ScheduleType.Recording, true);
            foreach (UpcomingProgram upcomingProgram in upcomingPrograms)
            {
                bool idMatches = upcomingProgramId.HasValue
                    && upcomingProgram.UpcomingProgramId == upcomingProgramId.Value;
                if ((idMatches || upcomingProgram.Title == program.Title)
                    && upcomingProgram.Channel.ChannelId == channel.ChannelId
                    && upcomingProgram.StartTime == program.StartTime)
                {
                    upcomingRecording = upcomingProgram;
                    return null;
                }
            }

            return new IMBotMessage("Program not found in upcoming recordings.", IMBotMessage.ErrorColor);
        }

        private static List<ChannelGroup> GetAllGroups(ChannelType channelType)
        {
            List<ChannelGroup> groups = Proxies.SchedulerService.GetAllChannelGroups(channelType, true);
            Guid allChannelsGroupId = (channelType == ChannelType.Television) ? ChannelGroup.AllTvChannelsGroupId : ChannelGroup.AllRadioChannelsGroupId;
            groups.Add(new ChannelGroup()
            {
                ChannelGroupId = allChannelsGroupId,
                ChannelType = channelType,
                GroupName = "All channels",
                Sequence = int.MaxValue,
                VisibleInGuide = true
            });
            return groups;
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

        private static Dictionary<Guid, UpcomingGuideProgram> BuildUpcomingDictionary(IEnumerable<UpcomingGuideProgram> upcomingGuidePrograms)
        {
            Dictionary<Guid, UpcomingGuideProgram> result = new Dictionary<Guid, UpcomingGuideProgram>();
            foreach (UpcomingGuideProgram upcomingGuideProgram in upcomingGuidePrograms)
            {
                Guid upcomingProgramId = upcomingGuideProgram.GetUniqueUpcomingProgramId();
                if (!result.ContainsKey(upcomingProgramId))
                {
                    result.Add(upcomingProgramId, upcomingGuideProgram);
                }
            }
            return result;
        }

        private static string AppendProgramIndicatorsPrefix(StringBuilder replyText, Guid upcomingProgramId,
            Dictionary<Guid, UpcomingGuideProgram> upcomingRecordingsById, Dictionary<Guid, UpcomingGuideProgram> upcomingAlertsById,
            Dictionary<Guid, UpcomingGuideProgram> upcomingSuggestionsById)
        {
            string appendText = String.Empty;
            if (upcomingRecordingsById.ContainsKey(upcomingProgramId))
            {
                string indicator = "R";
                switch (upcomingRecordingsById[upcomingProgramId].CancellationReason)
                {
                    case UpcomingCancellationReason.Manual:
                        appendText = " [cancelled rec]";
                        indicator = "c";
                        break;

                    case UpcomingCancellationReason.AlreadyQueued:
                    case UpcomingCancellationReason.PreviouslyRecorded:
                        appendText = " [skipped rec]";
                        indicator = "s";
                        break;
                }
                replyText.Append(indicator);
            }
            else
            {
                replyText.Append("·");
            }
            replyText.Append(upcomingAlertsById.ContainsKey(upcomingProgramId) ? "A" : "·");
            replyText.Append(upcomingSuggestionsById.ContainsKey(upcomingProgramId) ? "S " : "· ");
            return appendText;
        }

        #endregion
    }
}
