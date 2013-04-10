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
using System.Xml;
using System.Xml.XPath;
using System.Globalization;

namespace ArgusTV.GuideImporter.SchedulesDirect
{
    public class Config
    {
        #region Private Members

        // provide valueable defaults here, so a config file can be regenerated.
        //private string _sdWebServiceUrl = "http://webservices.schedulesdirect.tmsdatadirect.com/schedulesdirect/tvlistings/xtvdService";
        private int _nrOfDaysToImport = 1;

        #endregion

        public Config()
        {
            PluginName = "SchedulesDirectGuideImporter";
            
            NrOfDaysToImport = 1;
            LastImportedDay = DateTime.Now.AddDays(-1).Year * 10000 + DateTime.Now.AddDays(-1).Month * 100 + DateTime.Now.AddDays(-1).Day;
            UpdateMode = UpDateMode.AllDays;
            ChannelNameFormat = ChannelNameFormats[0];
            UpdateChannelNames = true;
        }

        public enum UpDateMode { AllDays, NextDayOnly };

        #region Properties

        public string PluginName {get; set;}

        public string SDUserName {get; set;}

        public string SDPassword {get; set;}

        public int NrOfDaysToImport
        {
            get 
            { 
                return _nrOfDaysToImport; 
            }
            set 
            {                 
                _nrOfDaysToImport = Math.Max(Math.Min(21, value), 1); 
            }
        }

        public int LastImportedDay { get; set; }

        public UpDateMode UpdateMode {get; set;}

        public string ChannelNameFormat {get; set;}

        public bool UpdateChannelNames { get; set; }

        public string[] ChannelNameFormats
        {
            get
            {
                return new string[]{ "{Name}", "{LogicalChannelNumber} {Name}", "{LogicalChannelNumber} {Callsign}", "{Name} {Affiliate}" };
            }
        }

        public string ProxyUserName {get; set;}

        public string ProxyPassword { get; set; }

        #endregion
    }
}
