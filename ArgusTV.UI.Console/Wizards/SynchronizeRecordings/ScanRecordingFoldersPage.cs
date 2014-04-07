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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

using ArgusTV.DataContracts;
using ArgusTV.Common;

namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    internal partial class ScanRecordingFoldersPage : SynchronizeRecordingsPageBase
    {
        private static XmlSerializer _mediaPortalTagsSerializer;

        private Guid _importGuid;
        private string _importName;

        private Guid _unknownChannelGuid = new Guid("{2184414D-D9C0-414b-B4E1-24EA1110AC78}");
        private string _unknownChannelName = "Unknown";

        private List<string> _extensions;
        private Dictionary<string, Channel> _mediaPortalChannels;

        static ScanRecordingFoldersPage()
        {
            _mediaPortalTagsSerializer = new XmlSerializer(typeof(MPTags));
        }

        public ScanRecordingFoldersPage()
        {
            InitializeComponent();
            _processingBackgroundWorker.RunWorkerCompleted += _processingBackgroundWorker_RunWorkerCompleted;
            _processingBackgroundWorker.DoWork += _processingBackgroundWorker_DoWork;
            _mediaPortalChannels = new Dictionary<string, Channel>();
        }

        public override string PageTitle
        {
            get { return "Scanning Folders"; }
        }

        public override string PageInformation
        {
            get { return "Scanning all selected folders for new and moved recordings."; }
        }

        public override void OnEnterPage(bool isBack)
        {
            if (isBack)
            {
                this.ParentWizard.SwitchToPreviousPanel();
            }
            else
            {
                _extensions = new List<string>();
                foreach (string part in Properties.Settings.Default.SynchronizeExtensions.Split(';'))
                {
                    _extensions.Add("." + part);
                }
                _importGuid = Guid.NewGuid();
                _importName = String.Format(CultureInfo.CurrentCulture, "Manual Import {0:g}", DateTime.Now);
                _processingBackgroundWorker.RunWorkerAsync(this.Context);
            }
        }

        public override bool IsNextAllowed()
        {
            return !_processingBackgroundWorker.IsBusy;
        }

        public override void OnLeavePage(bool isBack)
        {
            if (isBack)
            {
                if (_processingBackgroundWorker.IsBusy)
                {
                    _processingBackgroundWorker.CancelAsync();
                    while (_processingBackgroundWorker.IsBusy)
                    {
                        Thread.Sleep(100);
                        Application.DoEvents();
                    }
                }
            }
        }

        private void _processingBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.ParentWizard.Close();
            }
            else if (!e.Cancelled)
            {
                this.ParentWizard.SwitchToNextPanel();
            }
        }

        private void _processingBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SynchronizeRecordingsContext context = e.Argument as SynchronizeRecordingsContext;

            context.ClearFoundRecordings();
            foreach (string folder in context.RecordingFolders)
            {
                ScanDirectory(context, folder);
            }
        }

        private void ScanDirectory(SynchronizeRecordingsContext context, string folder)
        {
            try
            {
                if (!_processingBackgroundWorker.CancellationPending
                    && Directory.Exists(folder))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(folder);
                    foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
                    {
                        ScanDirectory(context, subDirInfo.FullName);
                    }
                    foreach (FileInfo fileInfo in dirInfo.GetFiles())
                    {
                        foreach (string extension in _extensions)
                        {
                            if (fileInfo.Name.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (!context.ContainsRecording(fileInfo.FullName))
                                {
                                    ImportRecording(context, fileInfo.FullName);
                                    break;
                                }
                            }
                            if (_processingBackgroundWorker.CancellationPending)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // In case of an error, simply skip this directory.
            }
        }

        private void ImportRecording(SynchronizeRecordingsContext context, string recordingFileName)
        {
            Recording recording = Utility.GetRecordingMetadata(recordingFileName);
            if (recording == null)
            {
                recording = new Recording();
                recording.ScheduleId = _importGuid;
                recording.ScheduleName = _importName;
                recording.ChannelId = _unknownChannelGuid;
                recording.ChannelDisplayName = _unknownChannelName;
                recording.RecordingStartTime = File.GetCreationTime(recordingFileName);
                recording.RecordingStopTime = recording.RecordingStartTime;
                recording.ProgramStartTime = recording.RecordingStartTime;
                recording.ProgramStopTime = recording.RecordingStartTime;
                recording.Title = Path.GetFileNameWithoutExtension(recordingFileName);
                recording.RecordingFileName = recordingFileName;

                try
                {
                    // Try the MediaPortal xml format
                    using (StreamReader mpReader = new StreamReader(Path.Combine(Path.GetDirectoryName(recordingFileName),
                        Path.GetFileNameWithoutExtension(recordingFileName) + ".xml")))
                    using (XmlTextReader mpXmlReader = new XmlTextReader(mpReader))
                    {
                        mpXmlReader.Namespaces = false;
                        MPTags tags = (MPTags)_mediaPortalTagsSerializer.Deserialize(mpXmlReader);
                        recording.Title = tags.GetValue("TITLE", recording.Title);
                        recording.Description = tags.GetValue("COMMENT", recording.Title);
                        recording.Category = tags.GetValue("GENRE", recording.Title);
                        string channelName = tags.GetValue("CHANNEL_NAME", null);
                        if (channelName != null)
                        {
                            Channel tvChannel = ResolveMediaPortalChannel(context, channelName);
                            recording.ChannelId = tvChannel.ChannelId;
                            recording.ChannelDisplayName = tvChannel.DisplayName;
                        }
                    }
                }
                catch
                {
                }

                context.AddFoundRecording(recording, false);
            }
            else
            {
                recording.RecordingFileName = recordingFileName;
                context.AddFoundRecording(recording, true);
            }
        }

        private Channel ResolveMediaPortalChannel(SynchronizeRecordingsContext context, string channelName)
        {
            Channel tvChannel;
            string channelKey = channelName.ToLowerInvariant();
            if (!_mediaPortalChannels.ContainsKey(channelKey))
            {
                tvChannel = new Channel();
                tvChannel.ChannelId = Guid.NewGuid();
                tvChannel.DisplayName = channelName;
                foreach (RecordingSummary recording in context.AllRecordings)
                {
                    if (channelName.Equals(recording.ChannelDisplayName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        tvChannel.ChannelId = recording.ChannelId;
                        tvChannel.DisplayName = recording.ChannelDisplayName;
                        break;
                    }
                }
                _mediaPortalChannels.Add(channelKey, tvChannel);
            }
            else
            {
                tvChannel = _mediaPortalChannels[channelKey];
            }
            return tvChannel;
        }
    }
}
