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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using ArgusTV.DataContracts;
using ArgusTV.GuideImporter.Interfaces;

namespace ArgusTV.GuideImporter.ClickFinder
{
    public class GuideImporter : IGuideImportPlugin
    {
        #region Private Members

        private const int _keepAliveIntervalSeconds = 30;
        private const int _timeout = 2 * 60 * 60 * 1000;        
        private const string _description = "Imports ClickFinder guide data into ARGUS TV";
        private const string _name = "ClickFinder";

        private string _installationPath;
        private DbReader _dbReader = new DbReader();
        #endregion

        #region IGuideImport Members

        public string Name
        {
            get 
            {
                if (!String.IsNullOrEmpty(ConfigInstance.Current.PluginName))
                {
                    return ConfigInstance.Current.PluginName;
                }
                return _name; 
            }
        }

        public string Description
        {
            get {return _description; }
        }

        public string InstallationPath
        {
            get { return _installationPath; }
            set 
            { 
                _installationPath = value;
                ConfigInstance.Load(ConfigFileName);
            }
        }

        public string ConfigFileName 
        {
            get
            {
                return Path.Combine(InstallationPath, "ArgusTV.GuideImporter.ClickFinder.dll.config");
            }
        }

        public void ShowConfigurationDialog(Form parentDialog)
        {
            ConfigurationForm configurationForm = new ConfigurationForm();
            configurationForm.ShowDialog(parentDialog);
        }

        public bool IsConfigured()
        {            
            if(!String.IsNullOrEmpty(ConfigInstance.Current.ClickFinderConnectionString) && !String.IsNullOrEmpty(ConfigInstance.Current.TvUptodatePath))
            {
                if (DbReader.SafeIsValidConnectionString(ConfigInstance.Current.ClickFinderConnectionString))
                {
                    if (File.Exists(ConfigInstance.Current.TvUptodatePath))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public List<ImportGuideChannel> GetAllImportChannels(bool reload, ProgressCallback progressCallback, FeedbackCallback feedbackCallback)
        {
            return _dbReader.GetAllImportChannels();
        }

        public List<ImportGuideChannel> GetAllImportChannels(List<ImportGuideChannel> skipChannels)
        {
            List<ImportGuideChannel> importChannels = new List<ImportGuideChannel>();
            List<ImportGuideChannel> channels = _dbReader.GetAllImportChannels();
            foreach (ImportGuideChannel channel in channels)
            {
                if (!skipChannels.Contains(channel))
                {
                    importChannels.Add(channel);
                }
            }
            return importChannels;
        }

        public void PrepareImport(FeedbackCallback feedbackCallback, KeepImportServiceAliveCallback keepImportServiceAliveCallback)
        {
            if (ConfigInstance.Current.LaunchClickFinderBeforeImport)
            {
                try
                {
                    GiveFeedback(feedbackCallback, "Launch ClickFinder to retrieve uptodate data ...");
                    string tvUptodatePath = ConfigInstance.Current.TvUptodatePath;

                    keepImportServiceAliveCallback();

                    ProcessStartInfo startInfo = new ProcessStartInfo(tvUptodatePath);
                    startInfo.WorkingDirectory = Path.GetDirectoryName(Path.GetDirectoryName(tvUptodatePath));
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = false;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    using (Process process = Process.Start(startInfo))
                    {
                        process.WaitForExit(_timeout);
                        if (!process.HasExited)
                        {
                            EventLogger.WriteEntry("ClickFinder tvuptodate still running, check your config.", EventLogEntryType.Error);
                        }
                    }
                    GiveFeedback(feedbackCallback, "ClickFinder updated successfully !");
                    
                    keepImportServiceAliveCallback();
                }
                catch (Exception ex)
                {
                    EventLogger.WriteEntry(ex);
                }
            }
        }

        public void Import(List<ImportGuideChannel> skipChannels, ImportDataCallback importDataCallback, ProgressCallback progressCallback, FeedbackCallback feedbackCallback, KeepImportServiceAliveCallback keepImportServiceAliveCallback)
        {
            List<ImportGuideChannel> channelsToImport = GetAllImportChannels(skipChannels);
            int importedCount = 0;
            DateTime _lastKeepAliveTime = DateTime.Now;

            foreach (ImportGuideChannel channel in channelsToImport)
            {
                if (!skipChannels.Contains(channel))
                {
                    GiveFeedback(feedbackCallback, String.Format("Importing channel {0}.", channel.ChannelName));
                    List<GuideProgram> guideData = _dbReader.GetGuideDataByClickFinderChannel(channel);
                    importDataCallback(channel, ChannelType.Television, guideData.ToArray(), false);
                }
                importedCount++;
                int procentDone = ((100 * importedCount) / channelsToImport.Count);
                if (progressCallback != null)
                {
                    progressCallback(procentDone);
                }
                if((procentDone % 5)==0)
                {
                    Trace.WriteLine(String.Format("Progress : {0} % done.", procentDone));
                }

                if (_lastKeepAliveTime.AddSeconds(_keepAliveIntervalSeconds) <= DateTime.Now)
                {
                    keepImportServiceAliveCallback();
                    _lastKeepAliveTime = DateTime.Now;
                }
            }
        }
        #endregion

        private void GiveFeedback(FeedbackCallback feedbackCallback, string message)
        {
            if (feedbackCallback != null)
            {
                feedbackCallback(message);
            }
        }
    }   
}