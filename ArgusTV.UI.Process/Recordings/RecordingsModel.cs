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

namespace ArgusTV.UI.Process.Recordings
{
    [Serializable]
    public class RecordingsModel
    {
        private ChannelType _channelType = ChannelType.Television;

        public ChannelType ChannelType
        {
            get { return _channelType; }
            internal set { _channelType = value; }
        }

        private List<RecordingGroup> _recordingGroups = new List<RecordingGroup>();

        public List<RecordingGroup> RecordingGroups
        {
            get { return _recordingGroups; }
            internal set { _recordingGroups = value; }
        }

        private SerializableDictionary<int, RecordingSummary[]> _recordingsByGroup = new SerializableDictionary<int, RecordingSummary[]>();

        public SerializableDictionary<int, RecordingSummary[]> RecordingsByGroup
        {
            get { return _recordingsByGroup; }
        }
    }
}
