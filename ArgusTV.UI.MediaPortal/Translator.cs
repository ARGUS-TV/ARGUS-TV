/*
 *	Copyright (C) 2007-2012 ARGUS TV
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
using System.Text.RegularExpressions;

using MediaPortal.Configuration;
using MediaPortal.GUI.Library;

namespace ArgusTV.UI.MediaPortal
{
    internal static class Translator
    {
        #region Private Members
        private static TimeSpan _changedFileContentCheckInterval = new TimeSpan(0, 5, 0);
        private static string _fallBackIsoCode = "en";

        private static string _currentIsoCode = String.Empty;
        private static bool _shortIsoFileLoaded;
        private static bool _longIsoFileLoaded;
        private static bool _fallBackIsoFileLoaded;
        private static bool _shortIsoFallBackFileLoaded;

        private static Dictionary<int, string> _translatons = new Dictionary<int, string>();
        private static Dictionary<int, string> _translatonsFallBack = new Dictionary<int, string>();

        private static DateTime _lastFileCheck = DateTime.MinValue;
        private static DateTime _shortIsoFileTimeStamp = DateTime.MinValue;
        private static DateTime _longIsoFileTimeStamp = DateTime.MinValue;
        private static DateTime _fallBackIsoFileTimeStamp = DateTime.MinValue;
        #endregion

        #region Public Static methods

        public static string GetLocalizedText(string isoCode, TextId textId)
        {
            try
            {
                if (IsValidIsoCode(isoCode))
                {
                    if (!isoCode.Equals(_currentIsoCode) || FileContentHasChanged())
                    {
                        LoadTranslations(isoCode);
                    }
                    if (_translatons.ContainsKey((int)textId))
                    {
                        return _translatons[(int)textId];
                    }
                    else
                    {
                        if (_currentIsoCode != _fallBackIsoCode)
                        {
                            Log.Error("ARGUS Translator: No translation found for id {0}, fallback to english !!", (int)textId);
                            if (!_shortIsoFallBackFileLoaded && IsValidShortIsoCode(_fallBackIsoCode))
                            {
                                Log.Info("ARGUS Translator: Load the english fallback file");
                                string tmpIsoCode = GetShortIsoCode(_fallBackIsoCode);
                                _shortIsoFallBackFileLoaded = LoadFile(GetIsoFileName(tmpIsoCode), out _shortIsoFileTimeStamp, false, true);
                            }

                            if (_translatonsFallBack.ContainsKey((int)textId))
                            {
                                return _translatonsFallBack[(int)textId];
                            }
                        }
                        Log.Error("ARGUS Translator: No english translation found for id {0} !!", (int)textId);
                    }
                }
            }
            catch { }

            return null;
        }
        #endregion

        #region Private Static methods

        private static void InitBeforeLoad()
        {
            _shortIsoFileLoaded = false;
            _longIsoFileLoaded = false;
            _fallBackIsoFileLoaded = false;
            _shortIsoFileTimeStamp = DateTime.MinValue;
            _longIsoFileTimeStamp = DateTime.MinValue;
            _fallBackIsoFileTimeStamp = DateTime.MinValue;
            _translatons.Clear();
        }

        private static void LoadTranslations(string isoCode)
        {
            InitBeforeLoad();

            if (IsValidShortIsoCode(isoCode))
            {
                string tmpIsoCode = GetShortIsoCode(isoCode);
                _shortIsoFileLoaded = LoadFile(GetIsoFileName(tmpIsoCode), out _shortIsoFileTimeStamp, false,false);
                if (_shortIsoFileLoaded)
                {
                    _currentIsoCode = tmpIsoCode;
                }
            }

            if (IsValidLongIsoCode(isoCode))
            {
                string tmpIsoCode = GetLongIsoCode(isoCode);
                _longIsoFileLoaded = LoadFile(GetIsoFileName(tmpIsoCode), out _longIsoFileTimeStamp, true,false);
                if (_longIsoFileLoaded)
                {
                    _currentIsoCode = tmpIsoCode;
                }
            }

            // fallback needed ?
            if (!_shortIsoFileLoaded && !_longIsoFileLoaded)
            {
                _currentIsoCode = isoCode;
                _fallBackIsoFileLoaded = LoadFile(GetIsoFileName(_fallBackIsoCode), out _fallBackIsoFileTimeStamp, false,false);
            }
            _lastFileCheck = DateTime.Now;
        }

        private static bool LoadFile(string translationFileName, out DateTime lastWriteTime, bool overrideExistingTranslations, bool isfallback)
        {
            lastWriteTime = DateTime.MinValue;
            bool fileLoaded = false;
            if (File.Exists(translationFileName))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(translationFileName);
                
                XmlNodeList translationNodes = xmlDocument.SelectNodes("/Language/String");
                foreach (XmlNode translation in translationNodes)
                {
                    try
                    {
                        int id;
                        if (int.TryParse(translation.Attributes["id"].Value, out id) && !String.IsNullOrEmpty(translation.InnerText))
                        {
                            if (!isfallback)
                            {
                                if (!_translatons.ContainsKey(id))
                                {
                                    _translatons.Add(id, translation.InnerText);
                                }
                                else if (overrideExistingTranslations)
                                {
                                    _translatons[id] = translation.InnerText;
                                }
                            }
                            else
                            {
                                if (!_translatonsFallBack.ContainsKey(id))
                                {
                                    _translatonsFallBack.Add(id, translation.InnerText);
                                }
                                else if (overrideExistingTranslations)
                                {
                                    _translatonsFallBack[id] = translation.InnerText;
                                }
                            }
                        }
                    }
                    catch { }
                }
                lastWriteTime = File.GetLastWriteTime(translationFileName);
                fileLoaded = true;
            }
            return fileLoaded;
        }

        private static bool IsValidIsoCode(string isoCode)
        {
            bool validIsoCode = false;
            if (!String.IsNullOrEmpty(isoCode))
            {
                validIsoCode = IsValidShortIsoCode(isoCode) || IsValidLongIsoCode(isoCode);
            }
            return validIsoCode;
        }

        private static bool IsValidShortIsoCode(string isoCode)
        {
            Regex expression = new Regex(@"^[a-z][a-z]", RegexOptions.IgnoreCase);
            return expression.IsMatch(isoCode);
        }

        private static bool IsValidLongIsoCode(string isoCode)
        {
            Regex expression = new Regex(@"^[a-z][a-z]-[a-z][a-z]$", RegexOptions.IgnoreCase);
            return expression.IsMatch(isoCode);
        }

        private static string GetShortIsoCode(string isoCode)
        {
            string shortIsoCode = String.Empty;
            if (IsValidShortIsoCode(isoCode))
            {
                shortIsoCode = isoCode.Substring(0, 2);
            }
            return shortIsoCode;
        }

        private static string GetLongIsoCode(string isoCode)
        {
            string longIsoCode = String.Empty;
            if (IsValidLongIsoCode(isoCode))
            {
                longIsoCode = isoCode.Substring(0, 5);
            }
            return longIsoCode;
        }

        private static string GetIsoFileName(string isoCode)
        {
            return Config.GetFile(Config.Dir.Language, String.Format(@"argustv_{0}.xml", isoCode));
        }

        private static bool FileContentHasChanged()
        {
            bool contentHasChanged = false;
            if (DateTime.Now > _lastFileCheck.Add(_changedFileContentCheckInterval))
            {
                if (IsValidShortIsoCode(_currentIsoCode))
                {
                    contentHasChanged = CheckFileForChanges(GetIsoFileName(GetShortIsoCode(_currentIsoCode)), _shortIsoFileLoaded, _shortIsoFileTimeStamp);
                }
                if (!contentHasChanged && IsValidLongIsoCode(_currentIsoCode))
                {
                    contentHasChanged = CheckFileForChanges(GetIsoFileName(GetLongIsoCode(_currentIsoCode)), _longIsoFileLoaded, _longIsoFileTimeStamp);
                }
                if (!contentHasChanged && _fallBackIsoFileLoaded)
                {
                    contentHasChanged = CheckFileForChanges(GetIsoFileName(_fallBackIsoCode), true, _fallBackIsoFileTimeStamp);
                }
                _lastFileCheck = DateTime.Now;
            }
            return contentHasChanged;
        }

        private static bool CheckFileForChanges(string fileName, bool fileExistedAtLastCheck, DateTime fileLastWriteTime)
        {
            bool fileHasBeenChanged = false;
            // File existed, and has been modified since
            if (fileExistedAtLastCheck && (File.GetLastWriteTime(fileName) != fileLastWriteTime))
            {
                fileHasBeenChanged = true;
            }
            // File existed, but has been deleted since
            else if (fileExistedAtLastCheck && !File.Exists(fileName))
            {
                fileHasBeenChanged = true;
            }
            // File didn't exist before, but has been created since
            else if (!fileExistedAtLastCheck && File.Exists(fileName))
            {
                fileHasBeenChanged = true;
            }
            return fileHasBeenChanged;
        }
        #endregion
    }
}
