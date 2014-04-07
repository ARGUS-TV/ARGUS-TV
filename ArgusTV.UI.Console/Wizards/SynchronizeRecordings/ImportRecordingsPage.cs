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
using ArgusTV.WinForms;

namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    internal partial class ImportRecordingsPage : SynchronizeRecordingsPageBase
    {
        private List<FoundRecording> _foundRecordings;

        public ImportRecordingsPage()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_recordingsDataGridView);
        }

        public override string PageTitle
        {
            get { return "Import Recordings"; }
        }

        public override string PageInformation
        {
            get { return "Select all recordings you would like to import into ARGUS TV. Edit the title and/or episode title of unrecognized recordings."; }
        }

        public override bool IsPageActive()
        {
            return this.Context.FoundRecordings.Count > 0;
        }

        public override void OnEnterPage(bool isBack)
        {
            _foundRecordings = new List<FoundRecording>();
            foreach (Recording recording in this.Context.FoundRecordings)
            {
                _foundRecordings.Add(new FoundRecording(recording, this.Context.ImportRecordings.Contains(recording)));
            }
            _recordingsBindingSource.DataSource = _foundRecordings;
            _recordingsBindingSource.ResetBindings(true);
        }

        public override void OnLeavePage(bool isBack)
        {
            this.Context.ImportRecordings.Clear();
            foreach (FoundRecording foundRecording in _foundRecordings)
            {
                if (foundRecording.ImportRecording)
                {
                    this.Context.ImportRecordings.Add(foundRecording.Recording);
                }
            }
        }

        [Obfuscation(Exclude = true)]
        private class FoundRecording
        {
            private bool _importRecording;
            private Recording _recording;

            public FoundRecording(Recording recording, bool importRecording)
            {
                _recording = recording;
                _importRecording = importRecording;
            }

            public bool ImportRecording
            {
                get { return _importRecording; }
                set { _importRecording = value; }
            }

            public Recording Recording
            {
                get { return _recording; }
                set { _recording = value; }
            }

            public string Title
            {
                get { return _recording.Title; }
                set { _recording.Title = value; }
            }

            public string SubTitle
            {
                get { return _recording.SubTitle; }
                set { _recording.SubTitle = value; }
            }

            public string ChannelDisplayName
            {
                get { return _recording.ChannelDisplayName; }
            }

            public string RecordingFileName
            {
                get { return _recording.RecordingFileName; }
            }
        }

        private void _allButton_Click(object sender, EventArgs e)
        {
            SelectAllRecordings(true);
        }

        private void _noneButton_Click(object sender, EventArgs e)
        {
            SelectAllRecordings(false);
        }

        private void SelectAllRecordings(bool selected)
        {
            foreach (FoundRecording foundRecording in _foundRecordings)
            {
                foundRecording.ImportRecording = selected;
            }
            _recordingsBindingSource.ResetBindings(false);
        }
    }
}
