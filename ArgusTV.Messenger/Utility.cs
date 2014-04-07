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
using System.Text;
using System.Globalization;

using ArgusTV.DataContracts;

namespace ArgusTV.Messenger
{
    internal static class Utility
    {
        public static void AppendProgramDetails(StringBuilder reply, IProgramSummary program)
        {
            reply.AppendFormat("{0} {1} {2}",
                GetShortDayDateString(program.StartTime),
                program.StartTime.ToShortTimeString().PadLeft(5, '0'),
                program.CreateProgramTitle());
        }

        public static void AppendProgramDetails(StringBuilder reply, Channel channel, IProgramSummary program)
        {
            reply.AppendFormat("[{0}] {1} {2}-{3} {4}",
                channel.DisplayName,
                GetShortDayDateString(program.StartTime),
                program.StartTime.ToShortTimeString().PadLeft(5, '0'),
                program.StopTime.ToShortTimeString().PadLeft(5, '0'),
                program.CreateProgramTitle());
        }

        public static string GetShortDayDateString(DateTime dateTime)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            string dateFormat = culture.DateTimeFormat.MonthDayPattern.Replace("MMMM", "MM").Replace("MMM", "MM");
            dateFormat = dateFormat.Replace("dddd", "dd").Replace("ddd", "dd");
            dateFormat = dateFormat.Replace(". ", ".").Replace(" ", culture.DateTimeFormat.DateSeparator);
            return culture.DateTimeFormat.GetShortestDayName(dateTime.DayOfWeek) + " " + dateTime.ToString(dateFormat);
        }
    }
}
