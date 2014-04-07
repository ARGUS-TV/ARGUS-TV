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
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console
{
    [XmlType("schedule")]
    [Obfuscation(Exclude = true)]
    public class ExportSchedule
    {
        public ExportSchedule()
        {
        }

        public ExportSchedule(SchedulerServiceProxy schedulerProxy, ControlServiceProxy controlProxy, Schedule schedule)
        {
            _isActive = schedule.IsActive;
            _name = schedule.Name;
            _channelType = schedule.ChannelType;
            _keepUntilMode = schedule.KeepUntilMode;
            _keepUntilValue = schedule.KeepUntilValue;
            _preRecordSeconds = schedule.PreRecordSeconds;
            _postRecordSeconds = schedule.PostRecordSeconds;
            _schedulePriority = schedule.SchedulePriority;
            _scheduleType = schedule.ScheduleType;
            foreach (ScheduleRule rule in schedule.Rules)
            {
                if (rule.Type == ScheduleRuleType.Channels
                    || rule.Type == ScheduleRuleType.NotOnChannels)
                {
                    List<string> channelNames = new List<string>();
                    foreach (Guid channelId in rule.Arguments)
                    {
                        Channel channel = schedulerProxy.GetChannelById(channelId);
                        if (channel != null)
                        {
                            channelNames.Add(channel.DisplayName);
                        }
                    }
                    if (channelNames.Count > 0)
                    {
                        ScheduleRule channelsRule = new ScheduleRule(rule.Type);
                        foreach (string channelName in channelNames)
                        {
                            channelsRule.Arguments.Add(channelName);
                        }
                        _rules.Add(channelsRule);
                    }
                }
                else
                {
                    _rules.Add(rule);
                }
            }
            _history = new List<ScheduleRecordedProgram>(controlProxy.GetPreviouslyRecordedHistory(schedule.ScheduleId));
        }

        #region Properties

        private bool _isActive;

        [XmlAttribute("isActive")]
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        private string _name;

        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private ChannelType _channelType;

        [XmlAttribute("ChannelType")]
        public ChannelType ChannelType
        {
            get { return _channelType; }
            set { _channelType = value; }
        }

        private ScheduleType _scheduleType;

        [XmlAttribute("type")]
        public ScheduleType ScheduleType
        {
            get { return _scheduleType; }
            set { _scheduleType = value; }
        }

        private SchedulePriority _schedulePriority;

        [XmlAttribute("priority")]
        public SchedulePriority SchedulePriority
        {
            get { return _schedulePriority; }
            set { _schedulePriority = value; }
        }

        private int? _preRecordSeconds;

        [XmlElement("preRecord")]
        public int? PreRecordSeconds
        {
            get { return _preRecordSeconds; }
            set { _preRecordSeconds = value; }
        }

        private int? _postRecordSeconds;

        [XmlElement("postRecord")]
        public int? PostRecordSeconds
        {
            get { return _postRecordSeconds; }
            set { _postRecordSeconds = value; }
        }

        private List<ScheduleRule> _rules = new List<ScheduleRule>();

        [XmlArray("rules")]
        public List<ScheduleRule> Rules
        {
            get { return _rules; }
        }

        private List<ScheduleRecordedProgram> _history = new List<ScheduleRecordedProgram>();

        [XmlArray("history")]
        public List<ScheduleRecordedProgram> History
        {
            get { return _history; }
        }

        private KeepUntilMode _keepUntilMode;

        [XmlAttribute("keepUntilMode")]
        public KeepUntilMode KeepUntilMode
        {
            get { return _keepUntilMode; }
            set { _keepUntilMode = value; }
        }

        private int? _keepUntilValue;

        [XmlElement("keepUntilValue")]
        public int? KeepUntilValue
        {
            get { return _keepUntilValue; }
            set { _keepUntilValue = value; }
        }

        #endregion

        public ImportSchedule Convert(SchedulerServiceProxy schedulerProxy, List<string> errors)
        {
            Schedule schedule = new Schedule();
            schedule.IsActive = this.IsActive;
            schedule.Name = this.Name;
            schedule.ChannelType = this.ChannelType;
            schedule.KeepUntilMode = this.KeepUntilMode;
            schedule.KeepUntilValue = this.KeepUntilValue;
            schedule.PreRecordSeconds = this.PreRecordSeconds;
            schedule.PostRecordSeconds = this.PostRecordSeconds;
            schedule.SchedulePriority = this.SchedulePriority;
            schedule.ScheduleType = this.ScheduleType;
            foreach (ScheduleRule rule in this.Rules)
            {
                if (rule.Type == ScheduleRuleType.TvChannels)
                {
                    rule.Type = ScheduleRuleType.Channels; /* For backwards compatibility. */
                }
                if (rule.Type == ScheduleRuleType.Channels
                    || rule.Type == ScheduleRuleType.NotOnChannels)
                {
                    List<Guid> tvChannelIds = new List<Guid>();
                    foreach (string channelName in rule.Arguments)
                    {
                        Channel tvChannel = schedulerProxy.GetChannelByDisplayName(schedule.ChannelType, channelName);
                        if (tvChannel == null)
                        {
                            errors.Add(String.Format(CultureInfo.CurrentCulture, "Channel '{0}' not found.", channelName));
                        }
                        else
                        {
                            tvChannelIds.Add(tvChannel.ChannelId);
                        }
                    }
                    if (tvChannelIds.Count > 0)
                    {
                        ScheduleRule channelsRule = new ScheduleRule(rule.Type);
                        foreach (Guid tvChannelId in tvChannelIds)
                        {
                            channelsRule.Arguments.Add(tvChannelId);
                        }
                        schedule.Rules.Add(channelsRule);
                    }
                }
                else
                {
                    schedule.Rules.Add(rule);
                }
            }
            return new ImportSchedule(schedule, _history.ToArray());
        }
    }
}
