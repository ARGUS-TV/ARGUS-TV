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
using System.Linq;
using System.Text;

using ArgusTV.DataContracts;

namespace ArgusTV.Messenger.Session
{
    internal class Programs
    {
        public Channel _channel;
        public IList<GuideProgramSummary> _guidePrograms;
        public IList<ChannelProgram> _searchedPrograms;
        private IList<UpcomingProgram> _upcomingPrograms;
        private IList<UpcomingRecording> _upcomingRecordings;

        public Programs(Channel channel, IList<GuideProgramSummary> guidePrograms)
        {
            _channel = channel;
            _guidePrograms = guidePrograms;
        }

        public Programs(IList<ChannelProgram> searchedPrograms)
        {
            _searchedPrograms = searchedPrograms;
        }

        public Programs(IList<UpcomingProgram> upcomingPrograms)
        {
            _upcomingPrograms = upcomingPrograms;
        }

        public Programs(IList<UpcomingRecording> upcomingRecordings)
        {
            _upcomingRecordings = upcomingRecordings;
        }

        public Channel Channel
        {
            get { return _channel; }
        }

        public IList<GuideProgramSummary> GuidePrograms
        {
            get { return _guidePrograms; }
        }

        public IList<ChannelProgram> SearchedPrograms
        {
            get { return _searchedPrograms; }
        }

        public IList<UpcomingProgram> UpcomingPrograms
        {
            get { return _upcomingPrograms; }
        }

        public IList<UpcomingRecording> UpcomingRecordings
        {
            get { return _upcomingRecordings; }
        }

        public IProgramSummary GetProgramAt(int number, out Channel channel)
        {
            Guid? upcomingProgramId;
            return GetProgramAt(number, out channel, out upcomingProgramId);
        }

        public IProgramSummary GetProgramAt(int number, out Channel channel, out Guid? upcomingProgramId)
        {
            if (--number >= 0)
            {
                if (_guidePrograms != null
                    && number < _guidePrograms.Count)
                {
                    channel = this.Channel;
                    upcomingProgramId = _guidePrograms[number].GetUniqueUpcomingProgramId(channel.ChannelId);
                    return _guidePrograms[number];
                }
                else if (_searchedPrograms != null
                    && number < _searchedPrograms.Count)
                {
                    channel = _searchedPrograms[number].Channel;
                    upcomingProgramId = _searchedPrograms[number].GetUniqueUpcomingProgramId();
                    return _searchedPrograms[number];
                }
                else if (_upcomingPrograms != null
                    && number < _upcomingPrograms.Count)
                {
                    channel = _upcomingPrograms[number].Channel;
                    upcomingProgramId = _upcomingPrograms[number].UpcomingProgramId;
                    return _upcomingPrograms[number];
                }
                else if (_upcomingRecordings != null
                    && number < _upcomingRecordings.Count)
                {
                    channel = _upcomingRecordings[number].Program.Channel;
                    upcomingProgramId = _upcomingRecordings[number].Program.UpcomingProgramId;
                    return _upcomingRecordings[number];
                }
            }
            channel = null;
            upcomingProgramId = null;
            return null;
        }
    }
}
