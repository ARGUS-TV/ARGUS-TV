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
using System.Diagnostics;

using ArgusTV.GuideImporter.Interfaces;
using ArgusTV.ServiceProxy;
using ArgusTV.DataContracts;

namespace ArgusTV.GuideImporter
{
    internal class Hoster
    {
        #region Private Members

        private const int _savingBlockSize = 10;

        private GuideImporterPluginLoader _pluginLoader;
        private PluginSettings _pluginSettings;
        #endregion

        #region P/Invoke

        [FlagsAttribute]
        private enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000
        }

        [System.Runtime.InteropServices.DllImport("Kernel32.DLL", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private extern static EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE state);

        #endregion

        #region Constructor

        public Hoster()
        {
            _pluginLoader = new GuideImporterPluginLoader();
            _pluginSettings = PluginSettings.Load();
        }
        #endregion

        #region Plugins

        public void ReloadPlugins()
        {
            _pluginLoader = null;
            _pluginLoader = new GuideImporterPluginLoader();
        }

        public string[] PluginNames
        {
            get
            {
                return _pluginLoader.PluginNames;
            }
        }

        public IGuideImportPlugin GetPluginByName(string pluginName)
        {
            return _pluginLoader.GetPluginByName(pluginName);
        }
        #endregion

        #region PluginSettings

        public PluginSetting GetPluginSetting(string pluginName)
        {
            return _pluginSettings.GetPluginSetting(pluginName);
        }

        public void SavePluginSettings()
        {
            _pluginSettings.Save();
        }
        #endregion

        #region Import

        public void Import(string pluginName)
        {
            Import(pluginName, null, null);
        }

        public void Import(string pluginName, ProgressCallback progressCallback, FeedbackCallback feedbackCallback)
        {
            try
            {
                LogMessageInArgusTV(String.Format("Guide import, using plugin {0} started.", pluginName), LogSeverity.Information);

                IGuideImportPlugin plugin = GetPluginByName(pluginName);
                if (plugin != null)
                {
                    if (!plugin.IsConfigured())
                    {
                        LogMessageInArgusTV(String.Format("Plugin {0} is not completely configured, check it's configuration.", pluginName), LogSeverity.Error);
                    }
                    else
                    {
                        plugin.PrepareImport(feedbackCallback, new KeepImportServiceAliveCallback(KeepImportServiceAlive));

                        Proxies.GuideService.StartGuideImport();
                        try
                        {
                            PluginSetting pluginSetting = GetPluginSetting(pluginName);
                            plugin.Import(pluginSetting.ChannelsToSkip, new ImportDataCallback(SaveGuideDataInArgusTV), progressCallback, feedbackCallback, new KeepImportServiceAliveCallback(KeepImportServiceAlive));
                        }
                        finally
                        {
                            Proxies.GuideService.EndGuideImport();
                        }
                    }
                }
                else
                {
                    LogMessageInArgusTV(String.Format("Plugin {0} not found, check your configuration.", pluginName), LogSeverity.Error);                
                }
            }
            catch (Exception ex)
            {
                LogMessageInArgusTV(String.Format("Guide import using plugin {0} failed, error : {1}", pluginName, ex.Message), LogSeverity.Error);
                throw;
            }
            finally
            {
                LogMessageInArgusTV(String.Format("Guide import using plugin {0} ended.", pluginName), LogSeverity.Information);
            }
        }

        public void SaveGuideDataInArgusTV(ImportGuideChannel channel, ChannelType channelType, GuideProgram[] guideProgramData, bool updateChannelName )
        {
            if (guideProgramData.Length > 0)
            {
                Guid guideChannelId = EnsureDefaultChannel(channel, channelType, updateChannelName);
                foreach (GuideProgram guideProgram in guideProgramData)
                {
                    guideProgram.GuideChannelId = guideChannelId;
                    try
                    {
                        Proxies.GuideService.ImportProgram(guideProgram, GuideSource.XmlTv);
                    }
                    catch { }
                }
            }
        }

        public static void LogMessageInArgusTV(string message, LogSeverity severity)
        {
            Proxies.LogService.LogMessage("GuideImporter", severity, message);
        }
        #endregion

        #region Private Methods

        private Guid EnsureDefaultChannel(ImportGuideChannel channel, ChannelType channelType, bool updateChannelName)
        {
            Guid guideChannelId = Proxies.GuideService.EnsureChannelExists(channel.ExternalId, channel.ChannelName, channelType);

            // If we have exactly one channel, check LCN and DisplayName :
            var channels = Proxies.SchedulerService.GetChannelsForGuideChannel(guideChannelId);
            if (channels.Count == 1 && updateChannelName)
            {
                bool needsToBeSaved = false;
                if (channels[0].LogicalChannelNumber == null && channel.LogicalChannelNumber.HasValue)
                {
                    channels[0].LogicalChannelNumber = channel.LogicalChannelNumber;
                    needsToBeSaved = true;
                }
                if (channels[0].DisplayName != channel.ChannelName)
                {
                    channels[0].DisplayName = channel.ChannelName;
                    needsToBeSaved = true;
                }

                if (needsToBeSaved)
                {
                    Proxies.SchedulerService.SaveChannel(channels[0]);
                }
            }
            else if(channels.Count == 0)
            {
                // No channels linked to the GuideChannel. If we have an existing channel with the same name, then link it.   
                Channel existingChannel = Proxies.SchedulerService.GetChannelByDisplayName(channelType, channel.ChannelName);
                if (existingChannel != null)
                {
                    existingChannel.LogicalChannelNumber = channel.LogicalChannelNumber;
                    Proxies.SchedulerService.SaveChannel(existingChannel);
                }
                else
                {
                    Proxies.SchedulerService.EnsureDefaultChannel(guideChannelId, channelType, channel.ChannelName, null);
                    channels = Proxies.SchedulerService.GetChannelsForGuideChannel(guideChannelId);
                    if (channels.Count == 1)
                    {
                        channels[0].LogicalChannelNumber = channel.LogicalChannelNumber;
                        Proxies.SchedulerService.SaveChannel(channels[0]);
                    }
                }
            }
            return guideChannelId;
        }
        #endregion

        #region KeepAlive

        public static void KeepImportServiceAlive()
        {
            try
            {
                Debug.WriteLine(String.Format("{0:HH:mm:ss} - KeepImportServiceAlive called", DateTime.Now));

                // Inform the local system we need it.
                SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED);

                // Tell the server we still need it.
                Proxies.CoreService.KeepServerAlive();
            }
            catch { }
        }

        #endregion
    }
}
