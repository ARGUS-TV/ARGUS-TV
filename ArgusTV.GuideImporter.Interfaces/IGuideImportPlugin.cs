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

using ArgusTV.DataContracts;
using System.Windows.Forms;

namespace ArgusTV.GuideImporter.Interfaces
{
    public delegate void ImportDataCallback(ImportGuideChannel channel, ChannelType channelType, GuideProgram[] guidePrograms, bool updateChannelName);
    public delegate void ProgressCallback(int percentageDone);
    public delegate void FeedbackCallback(string message);
    public delegate void KeepImportServiceAliveCallback();

    public interface IGuideImportPlugin : INamedPlugin
    {
        
        /// <summary>
        /// Allows the plugin to show a configuration dialog.
        /// </summary>
        /// <returns></returns>
        void ShowConfigurationDialog(Form parentDialog);

        /// <summary>
        /// Allows to ask the plugin if it is fully configured, and so ready to do imports
        /// </summary>
        /// <returns>True if plugin is configured</returns>
        bool IsConfigured();

        /// <summary>
        /// Get all the channels for which the plugin can provide guide data.
        /// </summary>
        /// <param name="reload">Allows to reload the channels from the plugins dataprovider</param>
        /// <param name="progressCallback">Callback method to indicate the progress of the import proces</param>
        /// <param name="feedbackCallback">Callback method to give feedback during the import proces</param>
        /// <returns>A list of GuideChannels</returns>
        List<ImportGuideChannel> GetAllImportChannels(bool reload, ProgressCallback progressCallback, FeedbackCallback feedbackCallback);

        /// <summary>
        /// Allows the plugin to take preparing actions, before the import will start.
        /// </summary>
        /// <param name="feedbackCallback">Callback method to give feedback during the prepareImport</param>
        /// <param name="keepImportServiceAliveCallback">Callback method to keep the service alive (avoids that machine(s) goes into standby)</param>
        void PrepareImport(FeedbackCallback feedbackCallback, KeepImportServiceAliveCallback keepImportServiceAliveCallback);

        /// <summary>
        /// Import guide program data, using the provided callback method.
        /// </summary>
        /// <param name="skipChannels">Channels to skip during the import</param>
        /// <param name="importDataCallback">Callback method to call with the data to import</param>
        /// <param name="progressCallback">Callback method to indicate the progress of the import proces</param>
        /// <param name="feedbackCallback">Callback method to give feedback during the import proces</param>
        /// <param name="keepImportServiceAliveCallback">Callback method to keep the service alive (avoids that machine(s) goes into standby)</param>
        void Import(List<ImportGuideChannel> skipChannels, ImportDataCallback importDataCallback, ProgressCallback progressCallback, FeedbackCallback feedbackCallback, KeepImportServiceAliveCallback keepImportServiceAliveCallback);
    }
}
