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

namespace ArgusTV.UI.Process.Guide
{
    public class GuideUpcomingProgram
    {
        public GuideUpcomingProgram(ScheduleType type, UpcomingGuideProgram upcomingGuideProgram)
        {
            _type = type;
            _upcomingGuideProgram = upcomingGuideProgram;
        }

        public GuideUpcomingProgram(UpcomingRecording upcomingRecording)
        {
            _type = ScheduleType.Recording;
            _upcomingRecording = upcomingRecording;
        }

        private ScheduleType _type;

        public ScheduleType Type
        {
            get { return _type; }
        }

        public Guid UpcomingProgramId
        {
            get { return _upcomingRecording == null ? _upcomingGuideProgram.GetUniqueUpcomingProgramId() : _upcomingRecording.Program.UpcomingProgramId; }
        }

        public Guid GuideProgramId
        {
            get { return _upcomingRecording == null ? _upcomingGuideProgram.GuideProgramId : _upcomingRecording.Program.GuideProgramId.Value; }
        }

        private UpcomingGuideProgram _upcomingGuideProgram;

        public UpcomingGuideProgram UpcomingGuideProgram
        {
            get { return _upcomingGuideProgram; }
        }

        private UpcomingRecording _upcomingRecording;

        public UpcomingRecording UpcomingRecording
        {
            get { return _upcomingRecording; }
        }

        public Guid ChannelId
        {
            get { return _upcomingRecording == null ? _upcomingGuideProgram.ChannelId : _upcomingRecording.Program.Channel.ChannelId; }
        }

        public Guid ScheduleId
        {
            get { return _upcomingRecording == null ? _upcomingGuideProgram.ScheduleId : _upcomingRecording.Program.ScheduleId; }
        }

        public bool IsCancelled
        {
            get { return _upcomingRecording == null ? _upcomingGuideProgram.IsCancelled : _upcomingRecording.Program.IsCancelled; }
        }

        public UpcomingCancellationReason CancellationReason
        {
            get { return _upcomingRecording == null ? _upcomingGuideProgram.CancellationReason : _upcomingRecording.Program.CancellationReason; }
        }

        public bool IsPartOfSeries
        {
            get { return _upcomingRecording == null ? _upcomingGuideProgram.IsPartOfSeries : _upcomingRecording.Program.IsPartOfSeries; }
        }
    }
}
