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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ArgusTV.ServiceContracts;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.Process.EditSchedule
{
    public class EditScheduleController
    {
        private EditScheduleModel _model;

        public EditScheduleController(EditScheduleModel model)
        {
            _model = model;
        }

        public void Initialize(IGuideService tvGuideAgent, ISchedulerService tvSchedulerAgent, Schedule schedule,
            bool forceManualSchedule, string allChannelsGroupName, string defaultFormatName)
        {
            _model.Schedule = schedule;
            _model.IsManual = forceManualSchedule
                || (_model.Schedule.Rules.FindRuleByType(ScheduleRuleType.ManualSchedule) != null);

            if (schedule.ScheduleType == ScheduleType.Recording)
            {
                _model.RecordingFormats = new SortableBindingList<RecordingFileFormat>(tvSchedulerAgent.GetAllRecordingFileFormats());
                _model.RecordingFormats.Insert(0, new RecordingFileFormat(Guid.Empty, defaultFormatName, String.Empty));
            }

            Channel[] channels = tvSchedulerAgent.GetAllChannels(schedule.ChannelType, false);
            _model.AllChannels.Clear();
            foreach (Channel channel in channels)
            {
                _model.AllChannels.Add(channel.ChannelId, channel);
            }

            _model.ChannelGroups.Clear();
            _model.ChannelGroups.AddRange(tvSchedulerAgent.GetAllChannelGroups(schedule.ChannelType, !_model.IsManual));
            _model.ChannelGroups.Add(new ChannelGroup(
                schedule.ChannelType == ChannelType.Television ? ChannelGroup.AllTvChannelsGroupId : ChannelGroup.AllRadioChannelsGroupId,
                (int)schedule.ChannelType, allChannelsGroupName, true, 0, 0));

            if (!_model.IsManual)
            {
                _model.Categories = tvGuideAgent.GetAllCategories();
            }
        }

        public void SaveSchedule(ISchedulerService tvSchedulerAgent)
        {
            tvSchedulerAgent.SaveSchedule(_model.Schedule);
            ScheduleNamesCache.Clear();
        }

        public void RefreshUpcomingPrograms(ISchedulerService tvSchedulerAgent)
        {
            _model.UpcomingPrograms = tvSchedulerAgent.GetUpcomingPrograms(_model.Schedule, true);
        }

        public void EnsureChannelsByGroup(ISchedulerService tvSchedulerAgent, Guid channelGroupId)
        {
            if (!_model.ChannelsByGroup.ContainsKey(channelGroupId))
            {
                _model.ChannelsByGroup[channelGroupId] =
                    new List<Channel>(tvSchedulerAgent.GetChannelsInGroup(channelGroupId, true));
            }
        }

        #region Update/Append Rules

        public void UpdateManualSchedule(Guid channelId, string channelDisplayName, DateTime startTime, TimeSpan duration, ScheduleDaysOfWeek daysOfWeek)
        {
            ScheduleRules rules = new ScheduleRules();
            rules.Add(ScheduleRuleType.Channels, channelId);
            rules.Add(ScheduleRuleType.ManualSchedule, startTime, new ScheduleTime(duration));
            if (daysOfWeek != ScheduleDaysOfWeek.None)
            {
                rules.Add(ScheduleRuleType.DaysOfWeek, daysOfWeek);
            }
            _model.Schedule.Name = BuildManualScheduleName(channelDisplayName, startTime, duration, daysOfWeek);
            _model.Schedule.Rules = rules;
        }

        public void AppendTitleRule(ScheduleRules rules, int titleRuleTypeIndex, string text)
        {
            text = text.Trim();
            if (!String.IsNullOrEmpty(text))
            {
                switch (titleRuleTypeIndex)
                {
                    case TitleRuleTypeIndex.Equals:
                        AppendORableRule(rules, ScheduleRuleType.TitleEquals, text);
                        break;

                    case TitleRuleTypeIndex.StartsWith:
                        AppendORableRule(rules, ScheduleRuleType.TitleStartsWith, text);
                        break;

                    case TitleRuleTypeIndex.Contains:
                        AppendContainsRule(rules, ScheduleRuleType.TitleContains, ScheduleRuleType.TitleDoesNotContain, text);
                        break;
                }
            }
        }

        public void AppendSubTitleRule(ScheduleRules rules, int titleRuleTypeIndex, string text)
        {
            text = text.Trim();
            if (!String.IsNullOrEmpty(text))
            {
                switch (titleRuleTypeIndex)
                {
                    case TitleRuleTypeIndex.Equals:
                        AppendORableRule(rules, ScheduleRuleType.SubTitleEquals, text);
                        break;

                    case TitleRuleTypeIndex.StartsWith:
                        AppendORableRule(rules, ScheduleRuleType.SubTitleStartsWith, text);
                        break;

                    case TitleRuleTypeIndex.Contains:
                        AppendContainsRule(rules, ScheduleRuleType.SubTitleContains, ScheduleRuleType.SubTitleDoesNotContain, text);
                        break;
                }
            }
        }

        public void AppendEpisodeNumberRule(ScheduleRules rules, int titleRuleTypeIndex, string text)
        {
            text = text.Trim();
            if (!String.IsNullOrEmpty(text))
            {
                switch (titleRuleTypeIndex)
                {
                    case TitleRuleTypeIndex.Equals:
                        AppendORableRule(rules, ScheduleRuleType.EpisodeNumberEquals, text);
                        break;

                    case TitleRuleTypeIndex.StartsWith:
                        AppendORableRule(rules, ScheduleRuleType.EpisodeNumberStartsWith, text);
                        break;

                    case TitleRuleTypeIndex.Contains:
                        AppendContainsRule(rules, ScheduleRuleType.EpisodeNumberContains, ScheduleRuleType.EpisodeNumberDoesNotContain, text);
                        break;
                }
            }
        }

        public void AppendDescriptionRule(ScheduleRules rules, string text)
        {
            AppendContainsRule(rules, ScheduleRuleType.DescriptionContains, ScheduleRuleType.DescriptionDoesNotContain, text);
        }

        public void AppendProgramInfoRule(ScheduleRules rules, string text)
        {
            AppendContainsRule(rules, ScheduleRuleType.ProgramInfoContains, ScheduleRuleType.ProgramInfoDoesNotContain, text);
        }

        public void AppendOnDateAndDaysOfWeekRule(ScheduleRules rules, ScheduleDaysOfWeek daysOfWeek, DateTime? onDateTime)
        {
            if (daysOfWeek == ScheduleDaysOfWeek.None)
            {
                if (onDateTime.HasValue)
                {
                    rules.Add(ScheduleRuleType.OnDate, onDateTime.Value.Date);
                }
            }
            else
            {
                if (onDateTime.HasValue)
                {
                    rules.Add(ScheduleRuleType.DaysOfWeek, daysOfWeek, onDateTime.Value.Date);
                }
                else
                {
                    rules.Add(ScheduleRuleType.DaysOfWeek, daysOfWeek);
                }
            }
        }

        public void AppendAroundTimeRule(ScheduleRules rules, DateTime? aroundTime)
        {
            if (aroundTime.HasValue)
            {
                rules.Add(ScheduleRuleType.AroundTime,
                    new ScheduleTime(aroundTime.Value.Hour, aroundTime.Value.Minute, aroundTime.Value.Second));
            }
        }

        public void AppendStartingBetweenRule(ScheduleRules rules, bool enabled, DateTime lowerTime, DateTime upperTime)
        {
            if (enabled)
            {
                rules.Add(ScheduleRuleType.StartingBetween,
                    new ScheduleTime(lowerTime.Hour, lowerTime.Minute, lowerTime.Second),
                    new ScheduleTime(upperTime.Hour, upperTime.Minute, upperTime.Second));
            }
        }

        public void AppendChannelsRule(ScheduleRules rules, bool notOnChannels, IList channelIds)
        {
            ScheduleRule channelsRule = new ScheduleRule(notOnChannels ? ScheduleRuleType.NotOnChannels : ScheduleRuleType.Channels);
            foreach (Guid channel in channelIds)
            {
                channelsRule.Arguments.Add(channel);
            }
            if (channelsRule.Arguments.Count > 0)
            {
                rules.Add(channelsRule);
            }
        }

        public void AppendCategoriesRule(ScheduleRules rules, bool doNotEqual, IList categories)
        {
            AppendStringArgumentsRule(rules, doNotEqual ? ScheduleRuleType.CategoryDoesNotEqual : ScheduleRuleType.CategoryEquals, categories);
        }

        public void AppendDirectedByRule(ScheduleRules rules, IList directors)
        {
            AppendStringArgumentsRule(rules, ScheduleRuleType.DirectedBy, directors);
        }

        public void AppendWithActorRule(ScheduleRules rules, IList actors)
        {
            AppendStringArgumentsRule(rules, ScheduleRuleType.WithActor, actors);
        }

        public void AppendNewEpisodesOnlyRule(ScheduleRules rules, bool newEpisodesOnly)
        {
            if (newEpisodesOnly)
            {
                rules.Add(ScheduleRuleType.NewEpisodesOnly, true);
            }
        }

        public void AppendNewTitlesOnlyRule(ScheduleRules rules, bool newTitlesOnly)
        {
            if (newTitlesOnly)
            {
                rules.Add(ScheduleRuleType.NewTitlesOnly, true);
            }
        }

        public void AppendSkipRepeatsRule(ScheduleRules rules, bool skipRepeats)
        {
            if (skipRepeats)
            {
                rules.Add(ScheduleRuleType.SkipRepeats, true);
            }
        }

        #endregion

        #region Get Rules Text

        public string GetTitleRuleText(out int typeIndex)
        {
            return GetTitleRuleExpression(_model.Schedule.Rules, ScheduleRuleType.TitleEquals, ScheduleRuleType.TitleStartsWith,
                ScheduleRuleType.TitleContains,ScheduleRuleType.TitleDoesNotContain, out typeIndex);
        }

        public string GetSubTitleRuleText(out int typeIndex)
        {
            return GetTitleRuleExpression(_model.Schedule.Rules, ScheduleRuleType.SubTitleEquals, ScheduleRuleType.SubTitleStartsWith,
                ScheduleRuleType.SubTitleContains, ScheduleRuleType.SubTitleDoesNotContain, out typeIndex);
        }

        public string GetEpisodeNumberRuleText(out int typeIndex)
        {
            return GetTitleRuleExpression(_model.Schedule.Rules, ScheduleRuleType.EpisodeNumberEquals, ScheduleRuleType.EpisodeNumberStartsWith,
                ScheduleRuleType.EpisodeNumberContains, ScheduleRuleType.EpisodeNumberDoesNotContain, out typeIndex);
        }

        public string GetDescriptionOrProgramInfoRuleText(out bool isProgramInfo)
        {
            isProgramInfo = true;
            string expression = GetContainsExpression(_model.Schedule.Rules, ScheduleRuleType.ProgramInfoContains, ScheduleRuleType.ProgramInfoDoesNotContain);
            if (String.IsNullOrEmpty(expression))
            {
                expression = GetContainsExpression(_model.Schedule.Rules, ScheduleRuleType.DescriptionContains, ScheduleRuleType.DescriptionDoesNotContain);
                isProgramInfo = String.IsNullOrEmpty(expression);
            }
            return expression;
        }

        public string GetEpisodeNumberRuleText()
        {
            ScheduleRule rule = _model.Schedule.Rules.FindRuleByType(ScheduleRuleType.EpisodeNumberEquals);
            if (rule != null)
            {
                return JoinORedArguments(rule.Arguments);
            }
            return String.Empty;
        }

        #endregion

        #region Private Methods

        private static string GetTitleRuleExpression(ScheduleRules rules, ScheduleRuleType equalsRule, ScheduleRuleType startsWithRule,
            ScheduleRuleType containsRule, ScheduleRuleType doesNotContainRule, out int typeIndex)
        {
            string expression = GetContainsExpression(rules, containsRule, doesNotContainRule);
            if (String.IsNullOrEmpty(expression))
            {
                typeIndex = TitleRuleTypeIndex.Equals;
                foreach (ScheduleRule rule in rules)
                {
                    if (rule.Type == equalsRule)
                    {
                        expression = JoinORedArguments(rule.Arguments);
                        break;
                    }
                    else if (rule.Type == startsWithRule)
                    {
                        expression = JoinORedArguments(rule.Arguments);
                        typeIndex = TitleRuleTypeIndex.StartsWith;
                        break;
                    }
                }
            }
            else
            {
                typeIndex = TitleRuleTypeIndex.Contains;
            }
            return expression;
        }

        private static string JoinORedArguments(List<object> arguments)
        {
            if (arguments.Count == 1)
            {
                return (string)arguments[0];
            }
            else
            {
                StringBuilder text = new StringBuilder();
                foreach (string argument in arguments)
                {
                    if (text.Length > 0)
                    {
                        text.Append(" OR ");
                    }
                    text.Append(argument);
                }
                return text.ToString();
            }
        }

        private static string GetContainsExpression(ScheduleRules rules, ScheduleRuleType containsRule, ScheduleRuleType doesNotContainRule)
        {
            StringBuilder expression = new StringBuilder();
            foreach (ScheduleRule rule in rules)
            {
                if (rule.Type == containsRule)
                {
                    if (expression.Length > 0)
                    {
                        expression.Append(" AND ");
                    }
                    foreach (string arg in rule.Arguments)
                    {
                        expression.Append(arg).Append(" OR ");
                    }
                    expression.Remove(expression.Length - 4, 4);
                }
                else if (rule.Type == doesNotContainRule)
                {
                    if (expression.Length > 0)
                    {
                        expression.Append(" ");
                    }
                    expression.Append("NOT ").Append(rule.Arguments[0]);
                }
            }
            return expression.ToString();
        }

        private enum Operator
        {
            None,
            Or,
            And,
            Not
        }

        private static void AppendORableRule(ScheduleRules rules, ScheduleRuleType rule, string expression)
        {
            expression = expression.Trim();
            if (!String.IsNullOrEmpty(expression))
            {
                List<object> arguments = new List<object>();

                int index = 0;
                while (index < expression.Length)
                {
                    int operatorIndex;
                    int nextIndex;
                    Operator op = GetNextOperator(expression, index, out operatorIndex, out nextIndex);
                    if (op == Operator.None)
                    {
                        arguments.Add(expression.Substring(index).Trim());
                        rules.Add(rule, arguments.ToArray());
                        break;
                    }
                    string fragment = expression.Substring(index, operatorIndex - index).Trim();
                    if (fragment.Length > 0
                        && fragment != "AND"
                        && fragment != "OR")
                    {
                        arguments.Add(fragment);
                    }
                    index = nextIndex;
                }
            }
        }

        private static void AppendContainsRule(ScheduleRules rules, ScheduleRuleType containsRule,
            ScheduleRuleType doesNotContainRule, string expression)
        {
            expression = expression.Trim();
            if (!String.IsNullOrEmpty(expression))
            {
                List<object> arguments = new List<object>();

                bool lastOperatorWasNot = false;
                int index = 0;
                while (index < expression.Length)
                {
                    int operatorIndex;
                    int nextIndex;
                    Operator op = GetNextOperator(expression, index, out operatorIndex, out nextIndex);
                    if (op == Operator.None)
                    {
                        arguments.Add(expression.Substring(index).Trim());
                        rules.Add(lastOperatorWasNot ? doesNotContainRule : containsRule, arguments.ToArray());
                        break;
                    }
                    string fragment = expression.Substring(index, operatorIndex - index).Trim();
                    if (fragment.Length > 0
                        && fragment != "AND"
                        && fragment != "OR")
                    {
                        if (lastOperatorWasNot)
                        {
                            rules.Add(doesNotContainRule, fragment);
                        }
                        else
                        {
                            arguments.Add(fragment);
                            if (op != Operator.Or)
                            {
                                rules.Add(containsRule, arguments.ToArray());
                                arguments.Clear();
                            }
                        }
                    }
                    lastOperatorWasNot = (op == Operator.Not);
                    index = nextIndex;
                }
            }
        }

        private static Operator GetNextOperator(string expression, int startIndex, out int operatorIndex, out int nextIndex)
        {
            string orOperator = " OR ";
            string andOperator = " AND ";
            string notOperator = "NOT ";

            int orOperatorIndex = expression.IndexOf(orOperator, startIndex);
            int andOperatorIndex = expression.IndexOf(andOperator, startIndex);
            int notOperatorIndex = expression.IndexOf(notOperator, startIndex);
            if (notOperatorIndex > startIndex)
            {
                notOperator = " NOT ";
                notOperatorIndex = expression.IndexOf(notOperator, startIndex);
            }
            if (orOperatorIndex >= 0
                && (andOperatorIndex < 0 || orOperatorIndex < andOperatorIndex)
                && (notOperatorIndex < 0 || orOperatorIndex < notOperatorIndex))
            {
                operatorIndex = orOperatorIndex;
                nextIndex = orOperatorIndex + orOperator.Length;
                return Operator.Or;
            }
            if (andOperatorIndex >= 0
                && (orOperatorIndex < 0 || andOperatorIndex < orOperatorIndex)
                && (notOperatorIndex < 0 || andOperatorIndex < notOperatorIndex))
            {
                operatorIndex = andOperatorIndex;
                nextIndex = andOperatorIndex + andOperator.Length;
                return Operator.And;
            }
            if (notOperatorIndex >= 0
                && (orOperatorIndex < 0 || notOperatorIndex < orOperatorIndex)
                && (andOperatorIndex < 0 || notOperatorIndex < andOperatorIndex))
            {
                operatorIndex = notOperatorIndex;
                nextIndex = notOperatorIndex + notOperator.Length;
                return Operator.Not;
            }
            operatorIndex = -1;
            nextIndex = expression.Length;
            return Operator.None;
        }

        private static void AppendStringArgumentsRule(ScheduleRules rules, ScheduleRuleType ruleType, IList arguments)
        {
            ScheduleRule rule = new ScheduleRule(ruleType);
            foreach (string arg in arguments)
            {
                rule.Arguments.Add(arg);
            }
            if (rule.Arguments.Count > 0)
            {
                rules.Add(rule);
            }
        }

        private string BuildManualScheduleName(string channelDisplayName, DateTime startTime, TimeSpan duration, ScheduleDaysOfWeek daysOfWeek)
        {
            StringBuilder name = new StringBuilder();
            if (daysOfWeek == ScheduleDaysOfWeek.None)
            {
                name.AppendFormat("{0} {1:g} ({2:00}:{3:00})", channelDisplayName, startTime, duration.Hours, duration.Minutes);
            }
            else
            {
                name.AppendFormat("{0} ", channelDisplayName);
                if ((daysOfWeek & ScheduleDaysOfWeek.Mondays) != 0)
                {
                    name.Append("Mo,");
                }
                if ((daysOfWeek & ScheduleDaysOfWeek.Tuesdays) != 0)
                {
                    name.Append("Tu,");
                }
                if ((daysOfWeek & ScheduleDaysOfWeek.Wednesdays) != 0)
                {
                    name.Append("We,");
                }
                if ((daysOfWeek & ScheduleDaysOfWeek.Thursdays) != 0)
                {
                    name.Append("Th,");
                }
                if ((daysOfWeek & ScheduleDaysOfWeek.Fridays) != 0)
                {
                    name.Append("Fr,");
                }
                if ((daysOfWeek & ScheduleDaysOfWeek.Saturdays) != 0)
                {
                    name.Append("Sa,");
                }
                if ((daysOfWeek & ScheduleDaysOfWeek.Sundays) != 0)
                {
                    name.Append("Su,");
                }
                name.Remove(name.Length - 1, 1);
                name.AppendFormat(" at {0:00}:{1:00} ({2:00}:{3:00})",
                    startTime.TimeOfDay.Hours, startTime.TimeOfDay.Minutes, duration.Hours, duration.Minutes);
            }
            return name.ToString();
        }

        #endregion
    }
}
