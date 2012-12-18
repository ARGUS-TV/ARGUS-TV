using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ArgusTV.DataContracts;

namespace ArgusTV.Client.Common
{
    public static class ScheduleRulesHelper
    {
        #region Get Rules Text

        public static string GetTitleRuleText(Schedule schedule, out TextRuleType typeIndex)
        {
            return GetTitleRuleExpression(schedule.Rules, ScheduleRuleType.TitleEquals, ScheduleRuleType.TitleStartsWith,
                ScheduleRuleType.TitleContains, ScheduleRuleType.TitleDoesNotContain, out typeIndex);
        }

        public static string GetEpisodeTitleRuleText(Schedule schedule, out TextRuleType typeIndex)
        {
            return GetTitleRuleExpression(schedule.Rules, ScheduleRuleType.SubTitleEquals, ScheduleRuleType.SubTitleStartsWith,
                ScheduleRuleType.SubTitleContains, ScheduleRuleType.SubTitleDoesNotContain, out typeIndex);
        }

        public static string GetEpisodeNumberRuleText(Schedule schedule, out TextRuleType typeIndex)
        {
            return GetTitleRuleExpression(schedule.Rules, ScheduleRuleType.EpisodeNumberEquals, ScheduleRuleType.EpisodeNumberStartsWith,
                ScheduleRuleType.EpisodeNumberContains, ScheduleRuleType.EpisodeNumberDoesNotContain, out typeIndex);
        }

        public static string GetDescriptionOrProgramInfoRuleText(Schedule schedule, out bool isProgramInfo)
        {
            isProgramInfo = true;
            string expression = GetContainsExpression(schedule.Rules, ScheduleRuleType.ProgramInfoContains, ScheduleRuleType.ProgramInfoDoesNotContain);
            if (String.IsNullOrEmpty(expression))
            {
                expression = GetContainsExpression(schedule.Rules, ScheduleRuleType.DescriptionContains, ScheduleRuleType.DescriptionDoesNotContain);
                isProgramInfo = String.IsNullOrEmpty(expression);
            }
            return expression;
        }

        public static string GetEpisodeNumberRuleText(ScheduleRules rules)
        {
            ScheduleRule rule = rules.FindRuleByType(ScheduleRuleType.EpisodeNumberEquals);
            if (rule != null)
            {
                return JoinORedArguments(rule.Arguments);
            }
            return String.Empty;
        }

        #endregion

        #region Set Rules Text

        public static void AppendTitleRule(ScheduleRules rules, TextRuleType textRuleType, string text)
        {
            text = (text ?? String.Empty).Trim();
            if (!String.IsNullOrEmpty(text))
            {
                switch (textRuleType)
                {
                    case TextRuleType.Equals:
                        AppendORableRule(rules, ScheduleRuleType.TitleEquals, text);
                        break;

                    case TextRuleType.StartsWith:
                        AppendORableRule(rules, ScheduleRuleType.TitleStartsWith, text);
                        break;

                    case TextRuleType.Contains:
                        AppendContainsRule(rules, ScheduleRuleType.TitleContains, ScheduleRuleType.TitleDoesNotContain, text);
                        break;
                }
            }
        }

        public static void AppendEpisodeTitleRule(ScheduleRules rules, TextRuleType textRuleType, string text)
        {
            text = (text ?? String.Empty).Trim();
            if (!String.IsNullOrEmpty(text))
            {
                switch (textRuleType)
                {
                    case TextRuleType.Equals:
                        AppendORableRule(rules, ScheduleRuleType.SubTitleEquals, text);
                        break;

                    case TextRuleType.StartsWith:
                        AppendORableRule(rules, ScheduleRuleType.SubTitleStartsWith, text);
                        break;

                    case TextRuleType.Contains:
                        AppendContainsRule(rules, ScheduleRuleType.SubTitleContains, ScheduleRuleType.SubTitleDoesNotContain, text);
                        break;
                }
            }
        }

        public static void AppendEpisodeNumberRule(ScheduleRules rules, TextRuleType textRuleType, string text)
        {
            text = (text ?? String.Empty).Trim();
            if (!String.IsNullOrEmpty(text))
            {
                switch (textRuleType)
                {
                    case TextRuleType.Equals:
                        AppendORableRule(rules, ScheduleRuleType.EpisodeNumberEquals, text);
                        break;

                    case TextRuleType.StartsWith:
                        AppendORableRule(rules, ScheduleRuleType.EpisodeNumberStartsWith, text);
                        break;

                    case TextRuleType.Contains:
                        AppendContainsRule(rules, ScheduleRuleType.EpisodeNumberContains, ScheduleRuleType.EpisodeNumberDoesNotContain, text);
                        break;
                }
            }
        }

        public static void AppendDescriptionRule(ScheduleRules rules, string text)
        {
            AppendContainsRule(rules, ScheduleRuleType.DescriptionContains, ScheduleRuleType.DescriptionDoesNotContain, text);
        }

        public static void AppendProgramInfoRule(ScheduleRules rules, string text)
        {
            AppendContainsRule(rules, ScheduleRuleType.ProgramInfoContains, ScheduleRuleType.ProgramInfoDoesNotContain, text);
        }

        #endregion

        #region Set "When" Rules

        public static void AppendOnDateAndDaysOfWeekRule(ScheduleRules rules, ScheduleDaysOfWeek daysOfWeek, DateTime? onDateTime)
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

        public static void AppendAroundTimeRule(ScheduleRules rules, DateTime aroundTime)
        {
            rules.Add(ScheduleRuleType.AroundTime,
                new ScheduleTime(aroundTime.Hour, aroundTime.Minute, aroundTime.Second));
        }

        public static void AppendStartingBetweenRule(ScheduleRules rules, DateTime lowerTime, DateTime upperTime)
        {
            rules.Add(ScheduleRuleType.StartingBetween,
                new ScheduleTime(lowerTime.Hour, lowerTime.Minute, lowerTime.Second),
                new ScheduleTime(upperTime.Hour, upperTime.Minute, upperTime.Second));
        }

        #endregion

        #region Private Methods

        private static string GetTitleRuleExpression(ScheduleRules rules, ScheduleRuleType equalsRule, ScheduleRuleType startsWithRule,
            ScheduleRuleType containsRule, ScheduleRuleType doesNotContainRule, out TextRuleType typeIndex)
        {
            string expression = GetContainsExpression(rules, containsRule, doesNotContainRule);
            if (String.IsNullOrEmpty(expression))
            {
                typeIndex = TextRuleType.Equals;
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
                        typeIndex = TextRuleType.StartsWith;
                        break;
                    }
                }
            }
            else
            {
                typeIndex = TextRuleType.Contains;
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
            expression = (expression ?? String.Empty).Trim();
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

        #endregion
    }
}
