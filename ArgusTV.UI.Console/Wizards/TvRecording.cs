using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ArgusTV.UI.Console.Wizards
{
    // Class to handle backwards compatibility with old XML format.
    [Obfuscation(Exclude = true)]
    [XmlType("TvRecording")]
    public class TvRecording
    {
        public virtual String Actors { get; set; }
        public virtual String Category { get; set; }
        public virtual String TvChannelDisplayName { get; set; }
        public virtual Guid TvChannelId { get; set; }
        public virtual ArgusTV.DataContracts.ChannelType ChannelType { get; set; }
        public virtual String Description { get; set; }
        public virtual String Director { get; set; }
        public virtual Int32? EpisodeNumber { get; set; }
        public virtual String EpisodeNumberDisplay { get; set; }
        public virtual Int32? EpisodeNumberTotal { get; set; }
        public virtual Int32? EpisodePart { get; set; }
        public virtual Int32? EpisodePartTotal { get; set; }
        public virtual Boolean IsPartialRecording { get; set; }
        public virtual Boolean IsPartOfSeries { get; set; }
        public virtual Boolean IsPremiere { get; set; }
        public virtual Boolean IsRepeat { get; set; }
        public virtual ArgusTV.DataContracts.KeepUntilMode KeepUntilMode { get; set; }
        public virtual Int32? KeepUntilValue { get; set; }
        public virtual Int32? LastWatchedPosition { get; set; }
        public virtual DateTime? LastWatchedTime { get; set; }
        public virtual DateTime ProgramStartTime { get; set; }
        public virtual DateTime ProgramStopTime { get; set; }
        public virtual String Rating { get; set; }
        public virtual Guid TvRecordingId { get; set; }
        public virtual DateTime RecordingStartTime { get; set; }
        public virtual DateTime? RecordingStopTime { get; set; }
        public virtual Guid TvScheduleId { get; set; }
        public virtual String TvScheduleName { get; set; }
        public virtual ArgusTV.DataContracts.SchedulePriority SchedulePriority { get; set; }
        public virtual Int32? SeriesNumber { get; set; }
        public virtual Double? StarRating { get; set; }
        public virtual String SubTitle { get; set; }
        public virtual String Title { get; set; }
    }
}
