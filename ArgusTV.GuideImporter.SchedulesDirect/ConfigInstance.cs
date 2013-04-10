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
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace ArgusTV.GuideImporter.SchedulesDirect
{
    internal class ConfigInstance
    {
        #region Static Methods/Properties

        private static Config _current;
        private static string _configFileName;

        public static Config Current
        {
            get { return _current; }
        }

        public static void Save()
        {
            Save(_configFileName);
        }

        public static void Save(string configFileName)
        {
            using (StreamWriter writer = new StreamWriter(configFileName, false))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                serializer.Serialize(writer, ConfigInstance.Current);
            }
        }

        public static void Load(string configFileName)
        {
            if (File.Exists(configFileName))
            {
                try
                {
                    using (StreamReader reader = new StreamReader(configFileName))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Config));
                        _current = (Config)serializer.Deserialize(reader);
                        _configFileName = configFileName;
                    }
                }
                catch (Exception ex)
                {
                    EventLogger.WriteEntry(String.Format("Ignoring invalid config: {0}", ex.Message), EventLogEntryType.Error);
                    _current = new Config();
                }
            }
            else
            {
                _current = new Config();
            }
        }
        #endregion
    }
}
