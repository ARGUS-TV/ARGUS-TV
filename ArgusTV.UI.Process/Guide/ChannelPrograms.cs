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

namespace ArgusTV.UI.Process.Guide
{
    [Serializable]
    public class ChannelPrograms
    {
        public ChannelPrograms(DateTime lowerBoundTime, DateTime upperBoundTime, GuideProgramSummary[] guidePrograms)
        {
            _lowerBoundTime = lowerBoundTime;
            _upperBoundTime = upperBoundTime;
            _programs = new SortedList<DateTime, GuideProgramSummary>();
            foreach (GuideProgramSummary guideProgram in guidePrograms)
            {
                // Normally this should not happen, but in case there's bad guide data and two programs start at
                // the same time, let's make sure we only display one of them.
                if (!_programs.ContainsKey(guideProgram.StartTime))
                {
                    _programs.Add(guideProgram.StartTime, guideProgram);
                }
            }
        }

        private SortedList<DateTime, GuideProgramSummary> _programs;

        public IList<GuideProgramSummary> Programs
        {
            get { return _programs.Values; }
        }

        private DateTime _lowerBoundTime;

        public DateTime LowerBoundTime
        {
            get { return _lowerBoundTime; }
            internal set { _lowerBoundTime = value; }
        }

        private DateTime _upperBoundTime;

        public DateTime UpperBoundTime
        {
            get { return _upperBoundTime; }
            internal set { _upperBoundTime = value; }
        }

        internal void InsertProgram(GuideProgramSummary guideProgram)
        {
            if (!_programs.ContainsKey(guideProgram.StartTime)
                && !ContainsProgram(guideProgram.GuideProgramId))
            {
                _programs.Add(guideProgram.StartTime, guideProgram);
            }
        }

        private bool ContainsProgram(Guid guideProgramId)
        {
            foreach (GuideProgramSummary guideProgram in this.Programs)
            {
                if (guideProgram.GuideProgramId == guideProgramId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
