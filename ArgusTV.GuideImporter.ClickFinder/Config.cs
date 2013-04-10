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

namespace ArgusTV.GuideImporter.ClickFinder
{
    public class Config
    {
        #region Private Members

        // provide valueable defaults here, so a config file can be regenerated.
        private string _pluginName = "ClickFinder";
        private string _clickFinderConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}\TV Movie\TV Movie ClickFinder\TVDaten.mdb;Mode=Share Deny None;Jet OLEDB:Engine Type=5;Jet OLEDB:Database Locking Mode=1;";       
        private string _tvUptodatePath = @"{0}\TV Movie\TV Movie ClickFinder\tvuptodate.exe";

        private bool _useShortDescription = false;
        private bool _launchClickFinderBeforeImport = true;
        #endregion

        public Config()
        { 
            // Set default paths :
            string programFilesFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            _clickFinderConnectionString = String.Format(_clickFinderConnectionString, programFilesFolder);
            _tvUptodatePath = String.Format(_tvUptodatePath, programFilesFolder);
        }

        #region Properties

        public string PluginName 
        {
            get {return _pluginName; }
            set { _pluginName = value; } 
        }

        public string ClickFinderConnectionString 
        {
            get {return _clickFinderConnectionString; }
            set { _clickFinderConnectionString = value; } 
        }

        public string TvUptodatePath
        {
            get { return _tvUptodatePath; }
            set { _tvUptodatePath = value; }
        }

        public bool UseShortDescription
        {
            get { return _useShortDescription; }
            set { _useShortDescription = value; }
        }

        public bool LaunchClickFinderBeforeImport
        {
            get { return _launchClickFinderBeforeImport; }
            set { _launchClickFinderBeforeImport = value; }
        }
        #endregion
    }
}
