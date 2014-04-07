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
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

using ArgusTV.DataContracts;
using ArgusTV.Common;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    internal partial class FinishPage : SynchronizeRecordingsPageBase
    {
        public FinishPage()
        {
            InitializeComponent();
        }

        public override string PageTitle
        {
            get { return "Finish"; }
        }

        public override string PageInformation
        {
            get { return "Apply new, moved and missing recordings in ARGUS TV."; }
        }

        public override void OnEnterPage(bool isBack)
        {
            if (this.Context.DeleteRecordings.Count == 0
                && this.Context.MoveRecordings.Count == 0
                && this.Context.ImportRecordings.Count == 0)
            {
                _noWorkPanel.Visible = true;
                _finishPanel.Visible = false;
            }
            else
            {
                _noWorkPanel.Visible = false;
                _finishPanel.Visible = true;
                _importLabel.Text = FormatRecordingsLabel("1 new recording", "{0} new recordings", this.Context.ImportRecordings.Count);
                _moveLabel.Text = FormatRecordingsLabel("1 moved recording", "{0} moved recordings", this.Context.MoveRecordings.Count);
                _cleanupLabel.Text = FormatRecordingsLabel("1 missing recording", "{0} missing recordings", this.Context.DeleteRecordings.Count);
            }
        }

        private string FormatRecordingsLabel(string format1, string format, int count)
        {
            return String.Format(CultureInfo.CurrentCulture, count == 1 ? format1 : format, count);
        }

        public override bool OnFinish()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                foreach (RecordingSummary recording in this.Context.DeleteRecordings)
                {
                    Proxies.ControlService.DeleteRecording(recording.RecordingFileName, false).Wait();
                }
                foreach (MoveRecording moveRecording in this.Context.MoveRecordings)
                {
                    Proxies.ControlService.ChangeRecordingFile(moveRecording.PreviousRecordingFileName, moveRecording.Recording.RecordingFileName, null, null).Wait();
                }
                foreach (Recording recording in this.Context.ImportRecordings)
                {
                    Proxies.ControlService.ImportRecording(recording).Wait();
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(this, ex.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
            return true;
        }
    }
}
