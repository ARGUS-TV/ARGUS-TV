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
using System.Xml.XPath;
using System.Xml;

namespace ArgusTV.GuideImporter
{
    internal class PluginSettings
    {
        private Dictionary<string, PluginSetting> _pluginSettings = new Dictionary<string, PluginSetting>();

        private PluginSettings()
        {
            LoadPluginSettings();
        }

        private string SettingsFileName
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ARGUS TV\ArgusTV.GuideImporter.config");
            }
        }

        #region Serialization

        private void LoadPluginSettings()
        {
            _pluginSettings.Clear();
            if (File.Exists(SettingsFileName))
            {
                using (StreamReader reader = new StreamReader(SettingsFileName, Encoding.UTF8))
                {
                    XPathDocument xpathDocument = new XPathDocument(reader);
                    XPathNavigator navigator = xpathDocument.CreateNavigator();
                    XPathNodeIterator iterator = navigator.Select("/Plugins/Plugin");
                    while (iterator.MoveNext())
                    {
                        PluginSetting pluginSetting = PluginSetting.Parse(iterator.Current);
                        if (pluginSetting != null)
                        {
                            _pluginSettings.Add(pluginSetting.PluginName, pluginSetting);
                        }
                    }
                }
            }
        }

        public void Save()
        {
            if (!Directory.Exists(Path.GetDirectoryName(SettingsFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsFileName));
            }
            
            using (XmlTextWriter xmlWriter = new XmlTextWriter(SettingsFileName, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Plugins");
                foreach (PluginSetting pluginSettings in _pluginSettings.Values)
                {
                    pluginSettings.WriteToXml(xmlWriter);
                }
                xmlWriter.WriteEndElement();
            }
        }
        #endregion

        #region Public Methods

        public static PluginSettings Load()
        {
            return new PluginSettings();
        }

        public PluginSetting GetPluginSetting(string pluginName)
        {
            if (!_pluginSettings.ContainsKey(pluginName))
            {
                _pluginSettings.Add(pluginName, new PluginSetting(pluginName));
            }
            return _pluginSettings[pluginName];
        }
        #endregion
    }
}
