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

using ArgusTV.DataContracts;

namespace ArgusTV.UI.Process
{
    public class ChannelProgramView
    {
        private ChannelProgram _program;
        private Guid _upcomingProgramId;

        public ChannelProgramView(ChannelProgram program)
        {
            _program = program;
            _upcomingProgramId = program.GetUniqueUpcomingProgramId();
        }

        public ChannelProgram Program
        {
            get { return _program; }
        }

        public Guid UpcomingProgramId
        {
            get { return _upcomingProgramId; }
        }

        public Guid ChannelId
        {
            get { return _program.Channel.ChannelId; }
        }

        public string ChannelName
        {
            get { return _program.Channel.DisplayName; }
        }

        public string ProgramTitle
        {
            get
            {
                return GuideProgram.CreateProgramTitle(_program.Title, _program.SubTitle, _program.EpisodeNumberDisplay);
            }
        }

        public string ProgramTimes
        {
            get
            {
                return _program.StartTime.ToLongDateString() + " " + _program.StartTime.ToShortTimeString() + " - " + _program.StopTime.ToShortTimeString();
            }
        }

        public string Title
        {
            get { return _program.Title; }
        }

        public DateTime StartTime
        {
            get { return _program.StartTime; }
        }

        public DateTime StopTime
        {
            get { return _program.StopTime; }
        }

        public string SubTitle
        {
            get { return _program.SubTitle; }
        }

        public string Category
        {
            get { return _program.Category; }
        }

        public bool IsRepeat
        {
            get { return _program.IsRepeat; }
        }

        public bool IsPremiere
        {
            get { return _program.IsPremiere; }
        }

        public string VideoFlags
        {
            get
            {
                string result = String.Empty;
                if (0 != (_program.Flags & GuideProgramFlags.HighDefinition))
                {
                    if (result.Length > 0)
                    {
                        result += " ";
                    }
                    result += "[HD]";
                }
                if (0 != (_program.Flags & GuideProgramFlags.StandardAspectRatio))
                {
                    if (result.Length > 0)
                    {
                        result += " ";
                    }
                    result += "[4:3]";
                }
                else if (0 != (_program.Flags & GuideProgramFlags.WidescreenAspectRatio))
                {
                    if (result.Length > 0)
                    {
                        result += " ";
                    }
                    result += "[16:9]";
                }
                return result;
            }
        }

        public string EpisodeNumberDisplay
        {
            get { return _program.EpisodeNumberDisplay; }
        }

        public string Rating
        {
            get { return _program.Rating; }
        }

        public double? StarRating
        {
            get { return _program.StarRating; }
        }
    }
}
