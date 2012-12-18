using System;
using System.Collections.Generic;
using System.Text;

using ArgusTV.DataContracts;
using ArgusTV.ServiceContracts;
using ArgusTV.UI.Process.Guide;

namespace ArgusTV.UI.Process
{
    [Serializable]
    public class UpcomingGuideProgramsDictionary
        : SerializableDictionary<Guid, GuideUpcomingProgram> 
    {
        public UpcomingGuideProgramsDictionary()
        {
            _upcomingRecordings = new UpcomingOrActiveProgramsList(new UpcomingProgram[0]);
        }

        public UpcomingGuideProgramsDictionary(UpcomingRecording[] upcomingRecordings,
            UpcomingGuideProgram[] upcomingAlerts, UpcomingGuideProgram[] upcomingSuggestions)
        {
            _upcomingRecordings = new UpcomingOrActiveProgramsList(AddAllUpcomingRecordings(upcomingRecordings));
            AddAllUpcomingProgramsForType(ScheduleType.Alert, upcomingAlerts);
            AddAllUpcomingProgramsForType(ScheduleType.Suggestion, upcomingSuggestions);
        }

        public new void Clear()
        {
            base.Clear();
            _upcomingRecordings.Clear();
        }

        private UpcomingOrActiveProgramsList _upcomingRecordings;

        public UpcomingOrActiveProgramsList UpcomingRecordings
    	{
	        get { return _upcomingRecordings;}
        }

        private UpcomingRecording[] AddAllUpcomingRecordings(UpcomingRecording[] upcomingRecordings)
        {
            foreach (UpcomingRecording upcomingRecording in upcomingRecordings)
            {
                this[upcomingRecording.Program.UpcomingProgramId] = new GuideUpcomingProgram(upcomingRecording);
            }
            return upcomingRecordings;
        }

        private void AddAllUpcomingProgramsForType(ScheduleType type, UpcomingGuideProgram[] upcomingGuidePrograms)
        {
            foreach (UpcomingGuideProgram upcomingGuideProgram in upcomingGuidePrograms)
            {
                this[upcomingGuideProgram.GetUniqueUpcomingProgramId()] = new GuideUpcomingProgram(type, upcomingGuideProgram);
            }
        }
    }
}
