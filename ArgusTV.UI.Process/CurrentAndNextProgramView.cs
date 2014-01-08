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

using ArgusTV.DataContracts;

namespace ArgusTV.UI.Process
{
    public class CurrentAndNextProgramView
    {
        private CurrentAndNextProgram _currentAndNext;

        public CurrentAndNextProgramView(CurrentAndNextProgram currentAndNext)
        {
            _currentAndNext = currentAndNext;
        }

        public CurrentAndNextProgram CurrentAndNextProgram
        {
            get { return _currentAndNext; }
        }

        public string ChannelName
        {
            get { return _currentAndNext.Channel.DisplayName; }
        }

        public string CurrentProgramTitle
        {
            get
            {
                if (_currentAndNext.Current == null)
                {
                    return String.Empty;
                }
                return GetProgramDescription(_currentAndNext.Current)
                    + " [" + _currentAndNext.CurrentPercentageComplete.ToString(CultureInfo.CurrentCulture) + "%]";
            }
        }

        public string NextProgramTitle
        {
            get
            {
                if (_currentAndNext.Next == null)
                {
                    return String.Empty;
                }
                return GetProgramDescription(_currentAndNext.Next);
            }
        }

        private string GetProgramDescription(GuideProgramSummary program)
        {
            return program.StartTime.ToShortTimeString() + " - " + program.StopTime.ToShortTimeString() + " " + program.CreateProgramTitle();
        }
    }
}
