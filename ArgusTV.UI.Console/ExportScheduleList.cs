using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console
{
    [XmlRoot("schedules")]
    [Obfuscation(Exclude = true)]
    public class ExportScheduleList : List<ExportSchedule>
    {
        public ExportScheduleList()
        {
        }

        public ExportScheduleList(SchedulerServiceProxy schedulerProxy, ControlServiceProxy controlProxy, List<Schedule> schedules)
        {
            foreach (Schedule schedule in schedules)
            {
                this.Add(new ExportSchedule(schedulerProxy, controlProxy, schedule));
            }
        }

        public List<ImportSchedule> Convert(SchedulerServiceProxy schedulerProxy, out List<string> errors)
        {
            errors = new List<string>();
            List<ImportSchedule> schedules = new List<ImportSchedule>();
            foreach (ExportSchedule schedule in this)
            {
                schedules.Add(schedule.Convert(schedulerProxy, errors));
            }
            return schedules;
        }

        public void Serialize(string fileName)
        {
            using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                GetXmlSerializer().Serialize(writer, this);
            }
        }

        public static ExportScheduleList Deserialize(string fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                return (ExportScheduleList)GetXmlSerializer().Deserialize(reader);
            }
        }

        private static XmlSerializer GetXmlSerializer()
        {
            return new XmlSerializer(typeof(ExportScheduleList),
                new Type[] { typeof(ScheduleDaysOfWeek), typeof(ScheduleTime) });
        }
    }
}
