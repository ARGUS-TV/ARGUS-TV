﻿/*
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

using ArgusTV.DataContracts;
using ArgusTV.ServiceContracts;
using ArgusTV.UI.Process.Guide;

namespace ArgusTV.UI.Process
{
    [Serializable]
    public class UpcomingGuideProgramsDictionary
        : SerializableDictionary<Guid, GuideUpcomingProgram> 
    {
        public UpcomingGuideProgramsDictionary()
        {
            _upcomingRecordings = new UpcomingOrActiveProgramsList(new UpcomingProgram[0]);
        }

        public UpcomingGuideProgramsDictionary(UpcomingRecording[] upcomingRecordings,
            UpcomingGuideProgram[] upcomingAlerts, UpcomingGuideProgram[] upcomingSuggestions)
        {
            _upcomingRecordings = new UpcomingOrActiveProgramsList(AddAllUpcomingRecordings(upcomingRecordings));
            AddAllUpcomingProgramsForType(ScheduleType.Alert, upcomingAlerts);
            AddAllUpcomingProgramsForType(ScheduleType.Suggestion, upcomingSuggestions);
        }

        public new void Clear()
        {
            base.Clear();
            _upcomingRecordings.Clear();
        }

        private UpcomingOrActiveProgramsList _upcomingRecordings;

        public UpcomingOrActiveProgramsList UpcomingRecordings
    	{
	        get { return _upcomingRecordings;}
        }

        private UpcomingRecording[] AddAllUpcomingRecordings(UpcomingRecording[] upcomingRecordings)
        {
            foreach (UpcomingRecording upcomingRecording in upcomingRecordings)
            {
                this[upcomingRecording.Program.UpcomingProgramId] = new GuideUpcomingProgram(upcomingRecording);
            }
            return upcomingRecordings;
        }

        private void AddAllUpcomingProgramsForType(ScheduleType type, UpcomingGuideProgram[] upcomingGuidePrograms)
        {
            foreach (UpcomingGuideProgram upcomingGuideProgram in upcomingGuidePrograms)
            {
                this[upcomingGuideProgram.GetUniqueUpcomingProgramId()] = new GuideUpcomingProgram(type, upcomingGuideProgram);
            }
        }
    }
}
