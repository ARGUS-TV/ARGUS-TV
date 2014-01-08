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
using System.Text.RegularExpressions;
using System.Management;
using System.Threading;
using System.IO;

namespace ArgusTV.Common
{
    public class UncConverter
    {
        #region GetInstance

        /// <summary>
        /// Returns an instance of the UNCConverter
        /// </summary>
        /// <returns>The UNCConverter instance</returns>
        public static UncConverter GetInstance()
        {
            if (Object.Equals(_instance, null))
            {
                UncConverter instance = new UncConverter();
                if (_instance == null)
                {
                    _instance = instance;
                }
            }
            return _instance;
        }

        #endregion

        #region Variables
        private static UncConverter _instance;
        private List<UncToDrive> _drives;
        private bool _loaded;
        private Thread _loader;
        #endregion

        #region Constructor
        /// <summary>
        /// Private constructor. Loads the drive info from management objects.
        /// </summary>
        private UncConverter()
        {
            ThreadStart loaderStart = new ThreadStart(LoadDrives);
            _loader = new Thread(loaderStart);
            _loader.Start();
        }
        #endregion

        #region Loader Thread
        /// <summary>
        /// Uses ManagementObjectSearch to load all local and remote name mappings to the _drives list.
        /// </summary>
        private void LoadDrives()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT RemoteName, LocalName FROM Win32_NetworkConnection");
            List<UncToDrive> drives = new List<UncToDrive>();
            foreach (ManagementObject obj in searcher.Get())
            {
                object localObj = obj["LocalName"];
                if (!Object.Equals(localObj, null))
                    drives.Add(new UncToDrive(obj["RemoteName"].ToString(), localObj.ToString()));
            }
            _drives = drives;
            _loaded = true;
        }
        #endregion

        #region Searchers

        /// <summary>
        /// Converts the input source to a source with drive letter if possible. Returns source if no drive letter was found
        /// </summary>
        /// <param name="source">The source path in UNC style</param>
        /// <returns>The source in mounted drive letter if possible</returns>
        public string ConvertToDrive(string uncSource)
        {
            if (!_loaded)
            {
                _loader.Join();
            }
            if (_loaded)
            {
                Match match;
                foreach (UncToDrive drive in _drives)
                {
                    match = Regex.Match(uncSource, drive.Regex, RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string result = uncSource.Replace(match.Groups["unc"].Value, drive.Drive);
                        if (File.Exists(result) || Directory.Exists(result))
                            return result;
                    }
                }
            }
            return uncSource;
        }

        #endregion

        #region UncToDrive Class

        /// <summary>
        /// Class to store a combination of UNC path and Drive letter
        /// </summary>
        private class UncToDrive
        {
            private string _unc;

            /// <summary>
            /// Get the UNC path of this object
            /// </summary>
            public string Unc
            {
                get { return _unc; }
            }

            private string _drive;

            /// <summary>
            /// Get the drive letter for this object
            /// </summary>
            public string Drive
            {
                get { return _drive; }
            }

            private string _regex;

            /// <summary>
            /// Get the regex string for this object
            /// </summary>
            public string Regex
            {
                get { return _regex; }
            }

            /// <summary>
            /// Creates an UncToDrive object. 
            /// </summary>
            /// <param name="unc"></param>
            /// <param name="drive"></param>
            public UncToDrive(string unc, string drive)
            {
                _unc = unc;
                //Create regex where both slash and backslash are allowed for \
                _regex = String.Format("^(?<unc>{0})", _unc.Replace(@"\", @"(/|\\)").Replace("$", @"\$"));
                _drive = drive;
            }
        }

        #endregion
    }
}
