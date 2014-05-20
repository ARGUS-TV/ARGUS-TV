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

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Process.Recordings
{
    public class RecordingsController
    {
        private RecordingsModel _model;

        public RecordingsController(RecordingsModel model)
        {
            _model = model;
        }

        public void Initialize()
        {
        }

        public void SetChannelType(ChannelType channelType)
        {
            _model.ChannelType = channelType;
            _model.RecordingGroups.Clear();
            _model.RecordingsByGroup.Clear();
        }

        public void ReloadRecordingGroups(RecordingGroupMode groupMode)
        {
            _model.RecordingGroups = new List<RecordingGroup>(Proxies.ControlService.GetAllRecordingGroups(_model.ChannelType, groupMode));
            _model.RecordingsByGroup.Clear();

            int index = 0;
            _model.RecordingGroups.ForEach(g =>
            {
                if (g.SingleRecording != null)
                {
                    _model.RecordingsByGroup[index] = new List<RecordingSummary>() { g.SingleRecording };
                }
                index++;
            });
        }

        public RecordingSummary GetRecordingById(Guid recordingId)
        {
            for (int index = 0; index < _model.RecordingGroups.Count; index++)
            {
                if (_model.RecordingsByGroup.ContainsKey(index))
                {
                    foreach (RecordingSummary recording in _model.RecordingsByGroup[index])
                    {
                        if (recording.RecordingId == recordingId)
                        {
                            return recording;
                        }
                    }
                }
            }
            return null;
        }

        public List<RecordingSummary> GetRecordingsForGroup(int groupIndex, bool includeNonExisting)
        {
            if (!_model.RecordingsByGroup.ContainsKey(groupIndex))
            {
                switch (_model.RecordingGroups[groupIndex].RecordingGroupMode)
                {
                    case RecordingGroupMode.GroupBySchedule:
                        _model.RecordingsByGroup[groupIndex] = Proxies.ControlService.GetRecordingsForSchedule(_model.RecordingGroups[groupIndex].ScheduleId, includeNonExisting);
                        break;
                    case RecordingGroupMode.GroupByChannel:
                        _model.RecordingsByGroup[groupIndex] = Proxies.ControlService.GetRecordingsOnChannel(_model.RecordingGroups[groupIndex].ChannelId, includeNonExisting);
                        break;
                    case RecordingGroupMode.GroupByProgramTitle:
                        _model.RecordingsByGroup[groupIndex] = Proxies.ControlService.GetRecordingsForProgramTitle(_model.ChannelType, _model.RecordingGroups[groupIndex].ProgramTitle, includeNonExisting);
                        break;
                    case RecordingGroupMode.GroupByCategory:
                        _model.RecordingsByGroup[groupIndex] = Proxies.ControlService.GetRecordingsForCategory(_model.ChannelType, _model.RecordingGroups[groupIndex].Category, includeNonExisting);
                        break;
                }
            }
            return _model.RecordingsByGroup[groupIndex];
        }

        public string GetGroupDisplayText(RecordingGroup recordingGroup, string recordingsText, string unknownText)
        {
            switch (recordingGroup.RecordingGroupMode)
            {
                case RecordingGroupMode.GroupByProgramTitle:
                    return String.Format("{0} - {2:g} ({1} {3})", recordingGroup.ProgramTitle,
                                                                  recordingGroup.RecordingsCount,
                                                                  recordingGroup.LatestProgramStartTime,
                                                                  recordingsText);
                case RecordingGroupMode.GroupByChannel:
                    return String.Format("{0} - {2:g} ({1} {3})", recordingGroup.ChannelDisplayName,
                                                                  recordingGroup.RecordingsCount,
                                                                  recordingGroup.LatestProgramStartTime,
                                                                  recordingsText);
                case RecordingGroupMode.GroupBySchedule:
                    return String.Format("{0} - {2:g} ({1} {3})", recordingGroup.ScheduleName,
                                                                  recordingGroup.RecordingsCount,
                                                                  recordingGroup.LatestProgramStartTime,
                                                                  recordingsText);
                case RecordingGroupMode.GroupByCategory:
                    return String.Format("{0} - {2:g} ({1} {3})", String.IsNullOrEmpty(recordingGroup.Category) ? unknownText : recordingGroup.Category,
                                                                  recordingGroup.RecordingsCount,
                                                                  recordingGroup.LatestProgramStartTime,
                                                                  recordingsText);
            }
            return "?";
        }

        public string GetRecordingDisplayText(RecordingSummary recording, bool addChannelName)
        {
            StringBuilder programTitle = new StringBuilder(recording.CreateProgramTitle());
            programTitle.Append(" - ");
            if (addChannelName)
            {
                programTitle.Append(recording.ChannelDisplayName).Append(" - ");
            }
            programTitle.AppendFormat("{0:g}", recording.ProgramStartTime);
            return programTitle.ToString();
        }
    }
}
