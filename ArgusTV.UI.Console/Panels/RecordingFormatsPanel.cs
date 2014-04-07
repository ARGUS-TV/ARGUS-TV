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
using System.Text;
using System.Windows.Forms;
using System.Globalization;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process;
using ArgusTV.WinForms;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Panels
{
    public partial class RecordingFormatsPanel : ContentPanel
    {
        private SortableBindingList<RecordingFileFormat> _recordingFormats;
        private HashSet<Guid> _changedFormatIds = new HashSet<Guid>();
        private List<RecordingFileFormat> _deletedFormats;
        private RecordingFileFormat _displayedFormat;
        private Recording _sampleRecording;

        private const string _defaultName = "-Unnamed-";

        public RecordingFormatsPanel()
        {
            InitializeComponent();
            _sampleRecording = Utility.CreateSampleRecording();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_formatsDataGridView);
            _changedFormatIds.Add(Guid.Empty);
        }

        public override string Title
        {
            get { return "Recording File Name Formats"; }
        }

        private void RecordingFormatsPanel_Load(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                MainForm.SetMenuMode(MainMenuMode.SaveCancel);

                LoadAllFormats();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public override void OnClosed()
        {
            MainForm.SetMenuMode(MainMenuMode.Normal);
            _formatsBindingSource.DataSource = null;
            _formatsBindingSource.ResetBindings(false);
        }

        private void LoadAllFormats()
        {
            try
            {
                _recOneTimeFormatTextBox.Text =
                    Proxies.ConfigurationService.GetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.OneTimeRecordingsFileFormat).Result;
                _recSeriesFormatTextBox.Text =
                    Proxies.ConfigurationService.GetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.SeriesRecordingsFileFormat).Result;
                _recRadioFormatTextBox.Text =
                    Proxies.ConfigurationService.GetStringValue(ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.RadioRecordingsFileFormat).Result;

                _deletedFormats = new List<RecordingFileFormat>();
                _recordingFormats = new SortableBindingList<RecordingFileFormat>(Proxies.SchedulerService.GetAllRecordingFileFormats().Result);
                _formatsBindingSource.DataSource = _recordingFormats;
                _formatsBindingSource.ResetBindings(false);
                UpdateSelectedFormat();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private RecordingFileFormat GetSelectedFormat()
        {
            RecordingFileFormat recordingFileFormat = null;
            if (_formatsDataGridView.SelectedRows.Count > 0)
            {
                recordingFileFormat = _formatsDataGridView.SelectedRows[0].DataBoundItem as RecordingFileFormat;
            }
            return recordingFileFormat;
        }

        private void UpdateSelectedFormat()
        {
            RecordingFileFormat selectedFormat = GetSelectedFormat();
            _deleteButton.Enabled = (selectedFormat != null);
            _nameTextBox.Enabled = (selectedFormat != null);
            _formatTextBox.Enabled = (selectedFormat != null);
            _exampleTextBox.Enabled = (selectedFormat != null);
            _resetCurrentToDefaultButton.Enabled = (selectedFormat != null);
            if (selectedFormat == null)
            {
                _nameTextBox.Text = String.Empty;
                _formatTextBox.Text = String.Empty;
                _exampleTextBox.Text = String.Empty;
            }
            else
            {
                _nameTextBox.Text = selectedFormat.Name;
                _formatTextBox.Text = selectedFormat.Format;
                _nameTextBox.Focus();
            }
            _displayedFormat = selectedFormat;
        }

        private void _formatsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(this, _formatsDataGridView.Columns[e.ColumnIndex].HeaderText + ": " + e.Exception.Message, null,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            e.ThrowException = false;
        }

        private void _formatsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            Apply();
            UpdateSelectedFormat();
        }

        public bool Apply()
        {
            if (_displayedFormat != null)
            {
                string name = _nameTextBox.Text.Trim();
                string format = _formatTextBox.Text.Trim();
                if (_displayedFormat.Format != format
                    && !CheckCustomRecordingFormat(format))
                {
                    return false;
                }
                if (String.IsNullOrEmpty(name)
                    || String.IsNullOrEmpty(_formatTextBox.Text)
                    || name == _defaultName)
                {
                    MessageBox.Show(this, "Name and format must be set.", null,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
                string text = _nameTextBox.Text.Trim();
                if (_displayedFormat.Name != name
                    || _displayedFormat.Format != format)
                {
                    _displayedFormat.Name = name;
                    _displayedFormat.Format = format;
                    _changedFormatIds.Add(_displayedFormat.RecordingFileFormatId);
                }
            }
            return true;
        }

        public override void OnSave()
        {
            if (Apply()
                && CheckDefaultRecordingFormats())
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;

                    Proxies.ConfigurationService.SetStringValue(
                        ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.OneTimeRecordingsFileFormat, _recOneTimeFormatTextBox.Text);
                    Proxies.ConfigurationService.SetStringValue(
                        ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.SeriesRecordingsFileFormat, _recSeriesFormatTextBox.Text);
                    Proxies.ConfigurationService.SetStringValue(
                        ConfigurationModule.Scheduler, ConfigurationKey.Scheduler.RadioRecordingsFileFormat, _recRadioFormatTextBox.Text);

                    foreach (RecordingFileFormat format in _recordingFormats)
                    {
                        if (_changedFormatIds.Contains(format.RecordingFileFormatId)
                            && !String.IsNullOrEmpty(format.Name)
                            && !String.IsNullOrEmpty(format.Format))
                        {
                            Proxies.SchedulerService.SaveRecordingFileFormat(format);
                        }
                    }

                    foreach (RecordingFileFormat format in _deletedFormats)
                    {
                        if (format.RecordingFileFormatId != Guid.Empty)
                        {
                            Proxies.SchedulerService.DeleteRecordingFileFormat(format.RecordingFileFormatId);
                        }
                    }

                    _formatsBindingSource.RemoveSort();
                    ClosePanel();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        public override void OnCancel()
        {
            _displayedFormat = null;
            _formatsBindingSource.RemoveSort();
            base.OnCancel();
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            RecordingFileFormat format = GetSelectedFormat();
            if (format != null)
            {
                _displayedFormat = null;
                _recordingFormats.Remove(format);
                _deletedFormats.Add(format);
            }
        }

        private void _createNewButton_Click(object sender, EventArgs e)
        {
            RecordingFileFormat format = new RecordingFileFormat();
            format.Name = _defaultName;
            format.Format = RecordingFileFormat.DefaultFormat;
            _formatsBindingSource.Add(format);
            _formatsDataGridView.CurrentCell = _formatsDataGridView.Rows[_formatsDataGridView.Rows.Count - 1].Cells[0];
        }

        private void _formatTextBox_TextChanged(object sender, EventArgs e)
        {
            _exampleTextBox.Text = ArgusTV.Common.Utility.BuildRecordingBaseFilePath(_formatTextBox.Text, _sampleRecording);
        }

        private void _helpRecFormatLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Utility.ShowRecordingFormatsHelp(this);
        }

        private void _resetRecFormatButton_Click(object sender, EventArgs e)
        {
            _recSeriesFormatTextBox.Text = RecordingFileFormat.DefaultFormat;
            _recOneTimeFormatTextBox.Text = RecordingFileFormat.DefaultFormat;
            _recRadioFormatTextBox.Text = RecordingFileFormat.DefaultFormat;
        }

        private void _resetCurrentToDefaultButton_Click(object sender, EventArgs e)
        {
            _formatTextBox.Text = RecordingFileFormat.DefaultFormat;
        }

        private void _recSeriesFormatTextBox_TextChanged(object sender, EventArgs e)
        {
            _recSeriesExampleTextBox.Text = ArgusTV.Common.Utility.BuildRecordingBaseFilePath(_recSeriesFormatTextBox.Text, _sampleRecording);
        }

        private void _recOneTimeFormatTextBox_TextChanged(object sender, EventArgs e)
        {
            _recOneTimeExampleTextBox.Text = ArgusTV.Common.Utility.BuildRecordingBaseFilePath(_recOneTimeFormatTextBox.Text, _sampleRecording);
        }

        private void _recRadioFormatTextBox_TextChanged(object sender, EventArgs e)
        {
            _recRadioExampleTextBox.Text = ArgusTV.Common.Utility.BuildRecordingBaseFilePath(_recRadioFormatTextBox.Text, _sampleRecording);
        }

        private bool CheckCustomRecordingFormat(string format)
        {
            return CheckRecordingFormat(format, "custom");
        }

        private bool CheckDefaultRecordingFormats()
        {
            return (CheckRecordingFormat(_recSeriesFormatTextBox.Text, "series")
                && CheckRecordingFormat(_recOneTimeFormatTextBox.Text, "one-time")
                && CheckRecordingFormat(_recRadioFormatTextBox.Text, "radio"));
        }

        private bool CheckRecordingFormat(string format, string formatName)
        {
            if (!FormatContainsHoursAndMinutes(format))
            {
                if (!ShowAreYouSureYouWantFormatWithoutTime(formatName))
                {
                    return false;
                }
            }
            return true;
        }

        private bool FormatContainsHoursAndMinutes(string format)
        {
            return format.Contains("%%MINUTES%%")
                && (format.Contains("%%HOURS%%") || format.Contains("%%HOURS12%%"));
        }

        private bool ShowAreYouSureYouWantFormatWithoutTime(string formatName)
        {
            return DialogResult.Yes == MessageBox.Show(this, "Your " + formatName + " recordings format does not contain hours and minutes." + Environment.NewLine +
                "It is HIGHLY recommended to add these variables to the format" + Environment.NewLine +
                "to avoid accidental overwrites or deletes!" + Environment.NewLine + Environment.NewLine +
                "Are you sure you want to save this setting?", "Bad Recordings Format", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        }
    }
}
