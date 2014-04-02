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
using System.Xml.XPath;
using System.Xml;
using ArgusTV.DataContracts;

namespace ArgusTV.Recorder.MediaPortalTvServer.Channels
{
    internal class ChannelLink
    {
        public ChannelLink(ChannelType channelType, Guid channelId, string channelName, int mpChannelId, string mpChannelName)
        {
            this.ChannelType = channelType;
            this.ChannelId = channelId;
            this.ChannelName = channelName;
            this.MPChannelId = mpChannelId;
            this.MPChannelName = mpChannelName;
        }

        public static ChannelLink Parse(XPathNavigator navigator)
        {
            ChannelLink result = null;
            Guid channelId = Guid.Empty;
            string id = navigator.GetAttribute("id", String.Empty);
            if (!String.IsNullOrEmpty(id))
            {
                channelId = new Guid(id);
            }
            int mpChannelId = -1;
            string mpId = navigator.GetAttribute("mpId", String.Empty);
            if (!String.IsNullOrEmpty(id))
            {
                mpChannelId = Int32.Parse(mpId);
            }
            if (channelId != Guid.Empty
                && mpChannelId >= 0)
            {
                string channelName = navigator.GetAttribute("name", String.Empty);
                ChannelType channelType;
                string type = navigator.GetAttribute("type", String.Empty);
                if (String.IsNullOrEmpty(type))
                {
                    channelType = ChannelType.Television;
                }
                else
                {
                    channelType = (ChannelType)Enum.Parse(typeof(ChannelType), type);
                }
                string mpChannelName = navigator.GetAttribute("mpName", String.Empty);
                result = new ChannelLink(channelType, channelId, channelName, mpChannelId, mpChannelName);
            }
            return result;
        }

        public void WriteToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("link");
            xmlWriter.WriteAttributeString("id", this.ChannelId.ToString());
            xmlWriter.WriteAttributeString("name", this.ChannelName.ToString());
            xmlWriter.WriteAttributeString("type", this.ChannelType.ToString());
            xmlWriter.WriteAttributeString("mpId", this.MPChannelId.ToString());
            xmlWriter.WriteAttributeString("mpName", this.MPChannelName.ToString());
            xmlWriter.WriteEndElement();
        }

        public ChannelType ChannelType { get; set; }

        public Guid ChannelId { get; set; }

        public string ChannelName { get; set; }

        public int MPChannelId { get; set; }

        public string MPChannelName { get; set; }
    }
}
