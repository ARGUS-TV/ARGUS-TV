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
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using ArgusTV.Common;
using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Wizards.ExportRecordings
{
    internal partial class FinishPage : ExportRecordingsPageBase
    {
        private const string _metaDataStreamName = "ArgusTV.xml";
        private const string _metaDataExtension = ".arg";

        private static class DirectoryNameIndex
        {
            public const int Title = 0;
            public const int Schedule = 1;
            public const int Channel = 2;
            public const int Date = 3;
        }

        public FinishPage()
        {
            InitializeComponent();
            _directoryNameComboBox.SelectedIndex = DirectoryNameIndex.Title;
        }

        public override string PageTitle
        {
            get { return "Finish"; }
        }

        public override string PageInformation
        {
            get { return "Export recordings from ARGUS TV."; }
        }

        public override void OnEnterPage(bool isBack)
        {
            if (this.Context.ExportRecordings.Count == 0)
            {
                _noWorkPanel.Visible = true;
                _finishPanel.Visible = false;
            }
            else
            {
                _noWorkPanel.Visible = false;
                _finishPanel.Visible = true;
                _exportLabel.Text = FormatRecordingsLabel("1 recording", "{0} recordings", this.Context.ExportRecordings.Count);
            }
        }

        private string FormatRecordingsLabel(string format1, string format, int count)
        {
            return String.Format(CultureInfo.CurrentCulture, count == 1 ? format1 : format, count);
        }

        public override bool IsNextAllowed()
        {
            if (CheckDestinationWriteable())
            {
                if (!_deleteRecordingsCheckBox.Checked
                    || MessageBox.Show(this, "Are you sure you want to delete recordings after exporting them?", "Export",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool OnFinish()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _destinationPanel.Enabled = false;
                _exportProgressBar.Minimum = 0;
                _exportProgressBar.Value = 0;
                _exportProgressBar.Maximum = this.Context.ExportRecordings.Count;
                _exportProgressBar.Visible = true;
                Application.DoEvents();

                int count = 0;
                foreach (RecordingSummary recording in this.Context.ExportRecordings)
                {
                    _exportingFileLabel.Text = recording.CreateProgramTitle();
                    Application.DoEvents();
                    ExportRecording(recording);
                    if (_deleteRecordingsCheckBox.Checked)
                    {
                        Proxies.ControlService.DeleteRecording(recording.RecordingFileName, true);
                    }
                    _exportProgressBar.Value = ++count;
                }

                return true;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                _destinationPanel.Enabled = true;
                _exportProgressBar.Visible = false;
            }
            finally
            {
                _exportingFileLabel.Text = String.Empty;
                Cursor.Current = Cursors.Default;
            }
            return false;
        }

        private void ExportRecording(RecordingSummary recording)
        {
            Recording metadata = Utility.GetRecordingMetadataFromAds(recording.RecordingFileName);

            string destinationPath = _destinationTextBox.Text;

            if (_createDirectoriesCheckBox.Checked)
            {
                string directoryName = null;

                switch (_directoryNameComboBox.SelectedIndex)
                {
                    case DirectoryNameIndex.Title:
                        directoryName = recording.Title;
                        break;

                    case DirectoryNameIndex.Schedule:
                        directoryName = recording.ScheduleName;
                        break;

                    case DirectoryNameIndex.Channel:
                        directoryName = recording.ChannelDisplayName;
                        break;

                    case DirectoryNameIndex.Date:
                        directoryName = String.Format(CultureInfo.CurrentCulture,
                            "{0}-{1:00}-{2:00}", recording.ProgramStartTime.Year, recording.ProgramStartTime.Month, recording.ProgramStartTime.Day);
                        break;
                }

                if (!String.IsNullOrEmpty(directoryName))
                {
                    destinationPath = Path.Combine(destinationPath, ArgusTV.Common.Utility.MakeValidFileName(directoryName));
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                }
            }

            SortedList<long, string> filesToCopy = new SortedList<long, string>();

            string sourceDirectory = Path.GetDirectoryName(recording.RecordingFileName);
            string sourceNameWithoutExtension = Path.GetFileNameWithoutExtension(recording.RecordingFileName);

            DirectoryInfo dirInfo = new DirectoryInfo(sourceDirectory);
            foreach (FileInfo fileInfo in dirInfo.GetFiles(sourceNameWithoutExtension + "*"))
            {
                long size = fileInfo.Length;
                while (filesToCopy.ContainsKey(size))
                {
                    size++;
                }
                filesToCopy.Add(size, fileInfo.FullName);
            }

            foreach (string fileName in filesToCopy.Values)
            {
                string destinationFileName = Path.Combine(destinationPath, Path.GetFileName(fileName));
                Microsoft.VisualBasic.Devices.Computer computer = new Microsoft.VisualBasic.Devices.Computer();
                computer.FileSystem.CopyFile(fileName, destinationFileName,
                    Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.ThrowException);
            }

            string destinationRecordingFileName = Path.Combine(destinationPath, Path.GetFileName(recording.RecordingFileName));
            if (_exportMetadataCheckBox.Checked
                || (metadata != null && !CheckMetadataCopied(destinationRecordingFileName)))
            {
                metadata = Utility.GetRecordingMetadata(recording.RecordingFileName);
                if (metadata == null)
                {
                    metadata = Proxies.ControlService.GetRecordingById(recording.RecordingId);
                }
                if (metadata != null)
                {
                    Utility.WriteRecordingMetadataFile(destinationRecordingFileName, metadata);
                }
            }
        }

        private bool CheckMetadataCopied(string destinationFileName)
        {
            Recording recording = Utility.GetRecordingMetadataFromAds(destinationFileName);

            try
            {
                FileAdsStreams adsStreams = new FileAdsStreams(destinationFileName);
                adsStreams.Add(_metaDataStreamName);
                using (Stream stream = adsStreams[_metaDataStreamName].Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    stream.ReadByte();
                    stream.Close();
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }

        private bool CheckDestinationWriteable()
        {
            if (String.IsNullOrEmpty(_destinationTextBox.Text.Trim()))
            {
                MessageBox.Show(this, "No destination entered.", null, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    Guid guid = Guid.NewGuid();
                    string testFilePath = Path.Combine(_destinationTextBox.Text, "_ATV_write_test_" + guid.ToString() + ".txt");
                    using (FileStream testStream = new FileStream(testFilePath, FileMode.Create))
                    {
                        testStream.Write(guid.ToByteArray(), 0, 16);
                    }
                    File.Delete(testFilePath);
                    return true;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }

        private void _browseDestinationButton_Click(object sender, EventArgs e)
        {
            if (_folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                _destinationTextBox.Text = _folderBrowserDialog.SelectedPath;
            }
        }

        private void _createDirectoriesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _directoryNameComboBox.Enabled = _createDirectoriesCheckBox.Checked;
        }
    }
}
