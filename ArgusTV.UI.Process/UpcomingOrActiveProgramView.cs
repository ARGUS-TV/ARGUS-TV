/*
 *	Copyright (C) 2007-2013 ARGUS TV
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
    public class UpcomingOrActiveProgramView
    {
        private ActiveRecording _activeRecording;
        private UpcomingRecording _upcomingRecording;
        private UpcomingProgram _upcomingProgram;

        public UpcomingOrActiveProgramView(UpcomingRecording upcomingRecording)
        {
            _upcomingRecording = upcomingRecording;
            _upcomingProgram = upcomingRecording.Program;
        }

        public UpcomingOrActiveProgramView(UpcomingProgram upcomingProgram)
        {
            _upcomingProgram = upcomingProgram;
        }

        public UpcomingOrActiveProgramView(ActiveRecording activeRecording)
        {
            _activeRecording = activeRecording;
            _upcomingProgram = activeRecording.Program;
        }

        public ActiveRecording ActiveRecording
        {
            get { return _activeRecording; }
        }

        public UpcomingRecording UpcomingRecording
        {
            get { return _upcomingRecording; }
        }

        public UpcomingProgram UpcomingProgram
        {
            get { return _upcomingProgram; }
        }

        public string ChannelName
        {
            get { return _upcomingProgram.Channel.CombinedDisplayName; }
        }

        public UpcomingProgramPriority Priority
        {
            get { return _upcomingProgram.Priority; }
        }

        public int PreRecordSeconds
        {
            get { return _upcomingProgram.PreRecordSeconds; }
        }

        public int PostRecordSeconds
        {
            get { return _upcomingProgram.PostRecordSeconds; }
        }

        public string ProgramTitle
        {
            get
            {
                return _upcomingProgram.CreateProgramTitle();
            }
        }

        public string ProgramTimes
        {
            get
            {
                return _upcomingProgram.StartTime.ToLongDateString() + " " + _upcomingProgram.StartTime.ToShortTimeString() + " - " + _upcomingProgram.StopTime.ToShortTimeString();
            }
        }

        public string Title
        {
            get { return _upcomingProgram.Title; }
        }

        public DateTime StartTime
        {
            get { return _upcomingProgram.StartTime; }
        }

        public DateTime StopTime
        {
            get { return _upcomingProgram.StopTime; }
        }

        public string SubTitle
        {
            get { return _upcomingProgram.SubTitle; }
        }

        public string Category
        {
            get { return _upcomingProgram.Category; }
        }

        public bool IsRepeat
        {
            get { return _upcomingProgram.IsRepeat; }
        }

        public bool IsPremiere
        {
            get { return _upcomingProgram.IsPremiere; }
        }

        public bool IsCancelled
        {
            get { return _upcomingProgram.IsCancelled; }
        }

        public UpcomingCancellationReason CancellationReason
        {
            get { return _upcomingProgram.CancellationReason; }
        }

        public bool IsPartOfSeries
        {
            get { return _upcomingProgram.IsPartOfSeries; }
        }

        public string VideoFlags
        {
            get
            {
                string result = String.Empty;
                if (0 != (_upcomingProgram.Flags & GuideProgramFlags.HighDefinition))
                {
                    if (result.Length > 0)
                    {
                        result += " ";
                    }
                    result += "[HD]";
                }
                if (0 != (_upcomingProgram.Flags & GuideProgramFlags.StandardAspectRatio))
                {
                    if (result.Length > 0)
                    {
                        result += " ";
                    }
                    result += "[4:3]";
                }
                else if (0 != (_upcomingProgram.Flags & GuideProgramFlags.WidescreenAspectRatio))
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
            get { return _upcomingProgram.EpisodeNumberDisplay; }
        }

        public string Rating
        {
            get { return _upcomingProgram.Rating; }
        }

        public double? StarRating
        {
            get { return _upcomingProgram.StarRating; }
        }

        public string ScheduleName
        {
            get
            {
                return ScheduleNamesCache.GetScheduleNameById(_upcomingProgram.ScheduleId);
            }
        }

        public string RecorderTuner
        {
            get
            {
                CardChannelAllocation allocation = null;
                if (_upcomingRecording != null)
                {
                    allocation = _upcomingRecording.CardChannelAllocation;
                }
                else if (_activeRecording != null)
                {
                    allocation = _activeRecording.CardChannelAllocation;
                }
                if (allocation != null)
                {
                    PluginService recorderTuner = RecorderTunersCache.GetRecorderTunerById(allocation.RecorderTunerId);
                    if (recorderTuner == null)
                    {
                        return "?";
                    }
                    return recorderTuner.Name + " (" + allocation.CardId + ")";
                }
                return String.Empty;
            }
        }
    }
}
