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
using System.Xml.Serialization;

using ArgusTV.DataContracts;
using System.Globalization;

namespace ArgusTV.UI.Console
{
    public class ImportSchedule
    {
        public ImportSchedule(Schedule schedule, ScheduleRecordedProgram[] history)
        {
            _schedule = schedule;
            _history = history;
        }

        private Schedule _schedule;

        public Schedule Schedule
        {
            get { return _schedule; }
        }

        private ScheduleRecordedProgram[] _history;

        public ScheduleRecordedProgram[] History
        {
            get { return _history; }
        }
    }
}
