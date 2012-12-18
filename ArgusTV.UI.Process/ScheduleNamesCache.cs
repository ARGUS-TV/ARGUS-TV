/*
 *	Copyright (C) 2007-2012 ARGUS TV
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
using System.Text;

using ArgusTV.DataContracts;
using ArgusTV.ServiceAgents;

namespace ArgusTV.UI.Process
{
    public class ScheduleNamesCache
    {
        private Dictionary<Guid, string> _scheduleNames = new Dictionary<Guid, string>();

        private ScheduleNamesCache()
        {
        }

        #region Singleton

        private static ScheduleNamesCache Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly ScheduleNamesCache instance = new ScheduleNamesCache();
        }

        #endregion

        public static void Clear()
        {
            Instance.InternalClear();
        }

        public static string GetScheduleNameById(Guid scheduleId)
        {
            return Instance.InternalGetScheduleNameById(scheduleId);
        }

        private void InternalClear()
        {
            lock (_scheduleNames)
            {
                _scheduleNames.Clear();
            }
        }

        private string InternalGetScheduleNameById(Guid scheduleId)
        {
            lock (_scheduleNames)
            {
                if (!_scheduleNames.ContainsKey(scheduleId))
                {
                    try
                    {
                        using (SchedulerServiceAgent tvSchedulerAgent = new SchedulerServiceAgent())
                        {
                            _scheduleNames.Clear();
                            GetScheduleNames(tvSchedulerAgent, ScheduleType.Recording);
                            GetScheduleNames(tvSchedulerAgent, ScheduleType.Alert);
                            GetScheduleNames(tvSchedulerAgent, ScheduleType.Suggestion);
                        }
                    }
                    catch { }
                }
                if (_scheduleNames.ContainsKey(scheduleId))
                {
                    return _scheduleNames[scheduleId];
                }
                return "?";
            }
        }

        private void GetScheduleNames(SchedulerServiceAgent tvSchedulerAgent, ScheduleType type)
        {
            ScheduleSummary[] schedules = tvSchedulerAgent.GetAllSchedules(ChannelType.Television, type, false);
            foreach (ScheduleSummary schedule in schedules)
            {
                _scheduleNames.Add(schedule.ScheduleId, schedule.Name);
            }
            schedules = tvSchedulerAgent.GetAllSchedules(ChannelType.Radio, type, false);
            foreach (ScheduleSummary schedule in schedules)
            {
                _scheduleNames.Add(schedule.ScheduleId, schedule.Name);
            }
        }
    }
}
