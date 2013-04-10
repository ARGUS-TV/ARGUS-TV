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
using System.Xml;
using System.Xml.XPath;

using ArgusTV.GuideImporter.Interfaces;

namespace ArgusTV.GuideImporter
{
    internal class PluginSetting
    {
        #region Private Members

        private string _pluginName;
        private List<ImportGuideChannel> _channelsToSkip = new List<ImportGuideChannel>();
        #endregion

        public PluginSetting(string pluginName)
        {
            _pluginName = pluginName;
        }

        #region Public Methods

        public static PluginSetting Parse(XPathNavigator navigator)
        {
            PluginSetting result = null;
            string pluginName = navigator.GetAttribute("Name", String.Empty);
            if (!String.IsNullOrEmpty(pluginName))
            {
                XPathNodeIterator iterator = navigator.Select("ChannelsToSkip/Channel");
                List<ImportGuideChannel> channelsToSkip = new List<ImportGuideChannel>();
                while (iterator.MoveNext())
                {
                    string name = iterator.Current.GetAttribute("Name", String.Empty);
                    string externalId = iterator.Current.GetAttribute("ExternalId", String.Empty);
                    channelsToSkip.Add(new ImportGuideChannel(name, externalId));
                }
                result = new PluginSetting(pluginName);
                result.ChannelsToSkip = channelsToSkip;
            }            
            return result;
        }

        public void WriteToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Plugin");
            xmlWriter.WriteAttributeString("Name", this.PluginName);
            xmlWriter.WriteStartElement("ChannelsToSkip");
            
            foreach(ImportGuideChannel channelToSkip in _channelsToSkip)
            {
                xmlWriter.WriteStartElement("Channel");
                xmlWriter.WriteAttributeString("Name", channelToSkip.ChannelName);
                xmlWriter.WriteAttributeString("ExternalId", channelToSkip.ExternalId);
                xmlWriter.WriteEndElement();        
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }
        #endregion

        #region Properties

        public string PluginName
        {
            get { return _pluginName; }
            set { _pluginName = value; }
        }

        public List<ImportGuideChannel> ChannelsToSkip
        {
            get { return _channelsToSkip; }
            set { _channelsToSkip = value; }
        }
        #endregion
    }
}
