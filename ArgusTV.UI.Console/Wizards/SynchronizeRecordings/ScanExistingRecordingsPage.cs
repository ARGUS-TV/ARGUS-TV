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
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

using ArgusTV.DataContracts;
using ArgusTV.ServiceProxy;

namespace ArgusTV.UI.Console.Wizards.SynchronizeRecordings
{
    internal partial class ScanExistingRecordingsPage : SynchronizeRecordingsPageBase
    {
        public ScanExistingRecordingsPage()
        {
            InitializeComponent();
            _processingBackgroundWorker.RunWorkerCompleted += _processingBackgroundWorker_RunWorkerCompleted;
            _processingBackgroundWorker.DoWork += _processingBackgroundWorker_DoWork;
        }

        public override string PageTitle
        {
            get { return "Processing"; }
        }

        public override string PageInformation
        {
            get { return "Processing all existing recordings."; }
        }

        public override void OnEnterPage(bool isBack)
        {
            if (isBack)
            {
                this.ParentWizard.SwitchToPreviousPanel();
            }
            else
            {
                _processingBackgroundWorker.RunWorkerAsync(this.Context);
            }
        }

        public override bool IsNextAllowed()
        {
            return !_processingBackgroundWorker.IsBusy;
        }

        public override void OnCancel()
        {
            _processingBackgroundWorker.CancelAsync();
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
            DateTime startTime = DateTime.Now;

            SynchronizeRecordingsContext context = e.Argument as SynchronizeRecordingsContext;
            context.ClearRecordings();
            context.MissingRecordings.Clear();
            GetAndProcessRecordings(context, ChannelType.Television);
            GetAndProcessRecordings(context, ChannelType.Radio);

            Utility.EnsureMinimumTime(startTime, 250);
        }

        private void GetAndProcessRecordings(SynchronizeRecordingsContext context, ChannelType channelType)
        {
            var groups = Proxies.ControlService.GetAllRecordingGroups(channelType, RecordingGroupMode.GroupBySchedule);

            foreach (RecordingGroup group in groups)
            {
                var recordings = Proxies.ControlService.GetRecordingsForSchedule(group.ScheduleId, includeNonExisting: true);
                foreach (RecordingSummary recording in recordings)
                {
                    if (!context.ContainsRecording(recording))
                    {
                        if (context.AddRecording(recording))
                        {
                            if (!File.Exists(recording.RecordingFileName))
                            {
                                context.MissingRecordings.Add(recording);
                            }
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
}
