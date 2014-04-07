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
    internal partial class MovedRecordingsPage : SynchronizeRecordingsPageBase
    {
        private List<MovedRecording> _movedRecordings = new List<MovedRecording>();

        public MovedRecordingsPage()
        {
            InitializeComponent();
            WinFormsUtility.ResizeDataGridViewColumnsForCurrentDpi(_recordingsDataGridView);
        }

        public override string PageTitle
        {
            get { return "Moved Recordings"; }
        }

        public override string PageInformation
        {
            get { return "ARGUS TV detected that these recordings were moved to another directory."; }
        }

        public override bool IsPageActive()
        {
            return this.Context.MoveRecordings.Count > 0;
        }

        public override void OnEnterPage(bool isBack)
        {
            _movedRecordings.Clear();
            foreach (MoveRecording moveRecording in this.Context.MoveRecordings)
            {
                _movedRecordings.Add(new MovedRecording(moveRecording.Recording));
            }
            _recordingsBindingSource.DataSource = _movedRecordings;
            _recordingsBindingSource.ResetBindings(true);
        }

        [Obfuscation(Exclude = true)]
        private class MovedRecording
        {
            private Recording _recording;

            public MovedRecording(Recording recording)
            {
                _recording = recording;
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
    }
}
