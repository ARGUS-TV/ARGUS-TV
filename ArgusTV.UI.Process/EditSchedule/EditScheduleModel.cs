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

namespace ArgusTV.UI.Process.EditSchedule
{
    [Serializable]
    public class EditScheduleModel
    {
        private SerializableDictionary<Guid, List<Channel>> _channelsByGroup = new SerializableDictionary<Guid, List<Channel>>();

        public SerializableDictionary<Guid, List<Channel>> ChannelsByGroup
        {
            get { return _channelsByGroup; }
        }

        private SerializableDictionary<Guid, Channel> _allChannels = new SerializableDictionary<Guid,Channel>();

        public SerializableDictionary<Guid, Channel> AllChannels
        {
            get { return _allChannels; }
        }

        private Schedule _schedule;

        public Schedule Schedule
        {
            get { return _schedule; }
            internal set { _schedule = value; }
        }

        private bool _isManual;

        public bool IsManual
        {
            get { return _isManual; }
            internal set { _isManual = value; }
        }

        private SortableBindingList<RecordingFileFormat> _recordingFormats;

        public SortableBindingList<RecordingFileFormat> RecordingFormats
        {
            get { return _recordingFormats; }
            internal set { _recordingFormats = value; }
        }

        private List<ChannelGroup> _channelGroups = new List<ChannelGroup>();

        public List<ChannelGroup> ChannelGroups
        {
            get { return _channelGroups; }
        }

        private string[] _categories = new string[0];

        public string[] Categories
        {
            get { return _categories; }
            internal set { _categories = value; }
        }

        private UpcomingProgram[] _upcomingPrograms = new UpcomingProgram[0];

        public UpcomingProgram[] UpcomingPrograms
        {
            get { return _upcomingPrograms; }
            internal set { _upcomingPrograms = value; }
        }

        public string ProcessingCommandsText
        {
            get
            {
                StringBuilder commandsText = new StringBuilder();
                if (_schedule != null)
                {
                    foreach (ScheduleProcessingCommand command in _schedule.ProcessingCommands)
                    {
                        if (commandsText.Length > 0)
                        {
                            commandsText.Append(", ");
                        }
                        commandsText.Append(command.Name);
                    }
                }
                return commandsText.ToString();
            }
        }
    }
}
