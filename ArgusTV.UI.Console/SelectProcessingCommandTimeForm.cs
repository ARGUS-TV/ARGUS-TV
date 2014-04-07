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
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

using ArgusTV.DataContracts;

namespace ArgusTV.UI.Console
{
    public partial class SelectProcessingCommandTimeForm : Form
    {
        public SelectProcessingCommandTimeForm()
        {
            InitializeComponent();
        }

        private List<RecordingSummary> _recordings;

        public List<RecordingSummary> Recordings
        {
            get { return _recordings; }
            set { _recordings = value; }
        }

        private ProcessingCommand _processingCommand;

        public ProcessingCommand ProcessingCommand
        {
            get { return _processingCommand; }
            set { _processingCommand = value; }
        }

        private DateTime _runAtTime = DateTime.Now;

        public DateTime RunAtTime
        {
            get { return _runAtTime; }
        }

        private void SelectProcessingCommandsForm_Load(object sender, EventArgs e)
        {
            _commandLabel.Text = _processingCommand.Name;
            if (_recordings.Count == 1)
            {
                _recordingLabel.Text = _recordings[0].ProgramStartTime.ToString("g", CultureInfo.CurrentCulture) + " - " + _recordings[0].CreateProgramTitle();
            }
            else
            {
                _recordingLabel.Text = _recordings.Count.ToString() + " recordings";
            }
            _runAtTimePicker.Value = DateTime.Today.AddHours(2);
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if (_nowRadioButton.Checked)
            {
                _runAtTime = DateTime.Now;
            }
            else
            {
                _runAtTime = DateTime.Today;
                _runAtTime.Add(_runAtTimePicker.Value.TimeOfDay);
                if (_runAtTime < DateTime.Now)
                {
                    _runAtTime = _runAtTime.AddDays(1);
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
