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
using System.Xml;
using System.Xml.XPath;
using ArgusTV.GuideImporter.Interfaces;

namespace ArgusTV.GuideImporter.SchedulesDirect
{
    internal class GuideChannelStore
    {
        #region Serialization

        public static List<ImportGuideChannel> Load(string fileName)
        {
            List<ImportGuideChannel> guideChannels = new List<ImportGuideChannel>();
            if (File.Exists(fileName))
            {
                using (StreamReader reader = new StreamReader(fileName, Encoding.UTF8))
                {
                    XPathDocument xpathDocument = new XPathDocument(reader);
                    XPathNavigator navigator = xpathDocument.CreateNavigator();
                    XPathNodeIterator iterator = navigator.Select("/AvailableGuideChannels/Channel");
                    while (iterator.MoveNext())
                    {
                        string name = iterator.Current.GetAttribute("Name", String.Empty);
                        string externalId = iterator.Current.GetAttribute("ExternalId", String.Empty);
                        guideChannels.Add(new ImportGuideChannel(name, externalId));
                    }
                }
            }
            return guideChannels;
        }

        public static void Save(string fileName, List<ImportGuideChannel> guideChannels)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }

            using (XmlTextWriter xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("AvailableGuideChannels");
                foreach (ImportGuideChannel channel in guideChannels)
                {
                    xmlWriter.WriteStartElement("Channel");
                    xmlWriter.WriteAttributeString("Name", channel.ChannelName);
                    xmlWriter.WriteAttributeString("ExternalId", channel.ExternalId);
                    xmlWriter.WriteEndElement(); 
                }
                xmlWriter.WriteEndElement();
            }
        }
        #endregion
    }
}
