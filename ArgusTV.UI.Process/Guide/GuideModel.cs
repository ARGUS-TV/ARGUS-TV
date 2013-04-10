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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ArgusTV.ServiceContracts;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.Process.Guide
{
    [Serializable]
    public class GuideModel
    {
        public ChannelType ChannelType { get; internal set; }

        public int EpgHours { get; internal set; }

        public int EpgHoursOffset { get; internal set; }

        public string AllChannelsGroupName { get; internal set; }

        private List<ChannelGroup> _channelGroups = new List<ChannelGroup>();

        public List<ChannelGroup> ChannelGroups
        {
            get { return _channelGroups; }
            internal set { _channelGroups = value; }
        }

        private DateTime _guideDateTime = DateTime.MinValue;

        public DateTime GuideDateTime
        {
            get { return _guideDateTime; }
            internal set { _guideDateTime = value; }
        }

        private List<Channel> _channels = new List<Channel>();

        public List<Channel> Channels
        {
            get { return _channels; }
            internal set { _channels = value; }
        }

        public Guid? ZoomedChannelId { get; internal set; }

        public Guid CurrentChannelGroupId { get; internal set; }

        private SerializableDictionary<Guid, ChannelPrograms> _programsByChannel = new SerializableDictionary<Guid, ChannelPrograms>();

        public SerializableDictionary<Guid, ChannelPrograms> ProgramsByChannel
        {
            get { return _programsByChannel; }
        }

        private SerializableDictionary<ScheduleType, List<GuideUpcomingProgram>> _upcomingProgramsByType = new SerializableDictionary<ScheduleType, List<GuideUpcomingProgram>>();

        public SerializableDictionary<ScheduleType, List<GuideUpcomingProgram>> UpcomingProgramsByType
        {
            get { return _upcomingProgramsByType; }
        }

        public UpcomingOrActiveProgramsList UpcomingRecordings { get; set; }

        private SerializableDictionary<Guid, GuideUpcomingProgram> _upcomingRecordingsById = new SerializableDictionary<Guid, GuideUpcomingProgram>();

        public SerializableDictionary<Guid, GuideUpcomingProgram> UpcomingRecordingsById
        {
            get { return _upcomingRecordingsById; }
            internal set { _upcomingRecordingsById = value; }
        }

        private SerializableDictionary<Guid, GuideUpcomingProgram> _upcomingAlertsById = new SerializableDictionary<Guid, GuideUpcomingProgram>();

        public SerializableDictionary<Guid, GuideUpcomingProgram> UpcomingAlertsById
        {
            get { return _upcomingAlertsById; }
            internal set { _upcomingAlertsById = value; }
        }

        private SerializableDictionary<Guid, GuideUpcomingProgram> _upcomingSuggestionsById = new SerializableDictionary<Guid, GuideUpcomingProgram>();

        public SerializableDictionary<Guid, GuideUpcomingProgram> UpcomingSuggestionsById
        {
            get { return _upcomingSuggestionsById; }
            internal set { _upcomingSuggestionsById = value; }
        }
    }
}
