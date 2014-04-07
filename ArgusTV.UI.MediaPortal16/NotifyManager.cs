#region Copyright (C) 2007-2014 ARGUS TV

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
 */

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;

using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;

using ArgusTV.ServiceProxy;
using ArgusTV.DataContracts;

namespace ArgusTV.UI.MediaPortal
{
    public class NotifyManager
    {
        private bool _busy = false;
        private bool _enableConflictNotification = false;
        private int _preNotifyConfig = 300;
        private Timer _timer = new Timer();
        private DateTime _nextCheckTimeConflicts;
        private DateTime _nextCheckTimeAlerts;
        private ArrayList AlreadyNotifiedConflict = new ArrayList();
        private ArrayList AlreadyNotifiedAlerts = new ArrayList();

        public NotifyManager()
        {
            _timer.Enabled = true;
            _timer.Interval = 10000;
            _timer.Stop();
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        #region public methods

        public void start()
        {
            Log.Info("TvNotify: start");
            LoadSettings();
            PluginMain.UpcomingAlertsChanged = true;
            PluginMain.UpcomingRecordingsChanged = true;
            _timer.Start();
        }

        public void stop()
        {
            Log.Info("TvNotify: stop");
            _timer.Stop();
        }

        #endregion

        #region private methods

        private void LoadSettings()
        {
            using (Settings xmlreader = new MPSettings())
            {
                _enableConflictNotification = xmlreader.GetValueAsBool("mytv", "enableConflictNotifier", false);
                _preNotifyConfig = xmlreader.GetValueAsInt("mytv", "notifyTVBefore", 300);
            }
        }

        private void ProcessConflicts(DateTime preNotifySecs)
        {
            //TODO
            /*if (_enableConflictNotification)
            {
                using (ControlServiceAgent tvControlAgent = new ControlServiceAgent())
                {
                    List<UpcomingRecording> upcomingRecordings = new List<UpcomingRecording>(
                        tvControlAgent.GetAllUpcomingRecordings(UpcomingRecordingsFilter.Recordings, false));
                    foreach (UpcomingRecording recording in upcomingRecordings)
                    {
                        if (recording.CardChannelAllocation == null
                            && recording.Program.IsCancelled == false
                            && DateTime.Now < recording.ActualStartTime
                            && preNotifySecs > recording.ActualStartTime
                            && AlreadyNotifiedConflict.Contains(recording.Program.ScheduleId) == false)//no free card to record
                        {
                            AlreadyNotifiedConflict.Add(recording.Program.ScheduleId);
                            string heading = (GUILocalizeStrings.Get(605) + " - " + recording.Program.Channel.DisplayName);
                            string text = (GUILocalizeStrings.Get(102017) + ": " + recording.Title + " will be skipped");
                            Notify(heading, text, recording.Program.Channel, true);
                        }
                        else if (DateTime.Now > recording.Program.ActualStartTime
                            && AlreadyNotifiedConflict.Contains(recording.Program.ScheduleId))//cleanup  old notifiers
                        {
                            AlreadyNotifiedConflict.Remove(recording.Program.ScheduleId);
                        }
                    }
                }
            }*/
        }

        private void ProcessAlerts(DateTime preNotifySecs)
        {
            List<UpcomingProgram> upcomingPrograms = Proxies.SchedulerService.GetAllUpcomingPrograms(ScheduleType.Alert, false).Result;
            if (upcomingPrograms.Count > 0)
            {
                DateTime _now = DateTime.Now;
                foreach (UpcomingProgram program in upcomingPrograms)
                {
                    if (preNotifySecs > program.StartTime && _now < program.StopTime
                        && !AlreadyNotifiedAlerts.Contains(program.ScheduleId))
                    {
                        NotifyProgram(program);
                        AlreadyNotifiedAlerts.Add(program.ScheduleId);
                        Log.Debug("NotifyManager: AlreadyNotifiedAlerts.Add {0}", program.Title);
                    }
                    else if (_now >= program.StopTime
                        && AlreadyNotifiedConflict.Contains(program.ScheduleId))
                    {
                        Log.Debug("NotifyManager: AlreadyNotifiedAlerts.Remove");
                        AlreadyNotifiedAlerts.Remove(program.ScheduleId);
                    }

                    if (program.StartTime < _nextCheckTimeAlerts && program.StartTime > _now)
                    {
                        _nextCheckTimeAlerts = program.StartTime;
                    }

                    if (program.StopTime < _nextCheckTimeAlerts && program.StopTime > _now)
                    {
                        _nextCheckTimeAlerts = program.StopTime;
                    }
                }
            }
        }

        private bool NotifyProgram(UpcomingProgram program)
        {
            lock (this)
            {
                GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_NOTIFY_TV_PROGRAM, 0, 0, 0, 0, 0, null);
                msg.Object = program;
                GUIGraphicsContext.SendMessage(msg);
                msg = null;
                return true;
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!_busy)
                {
                    _busy = true;

                    if (PluginMain.UpcomingAlertsChanged || _nextCheckTimeAlerts <= DateTime.Now)
                    {
                        if (!PluginMain.IsConnected())
                        {
                            _busy = false;
                            return;
                        }

                        _nextCheckTimeAlerts = DateTime.Now.AddDays(10);
                        DateTime preNotifySecs = DateTime.Now.AddSeconds(_preNotifyConfig);
                        ProcessAlerts(preNotifySecs);
                        _nextCheckTimeAlerts.AddSeconds(-_preNotifyConfig);
                        PluginMain.UpcomingAlertsChanged = false;
                    }

                    if ((PluginMain.UpcomingRecordingsChanged || _nextCheckTimeConflicts <= DateTime.Now)
                        && _enableConflictNotification)
                    {
                        if (!PluginMain.IsConnected())
                        {
                            _busy = false;
                            return;
                        }

                        _nextCheckTimeConflicts = DateTime.Now.AddDays(10);
                        DateTime preNotifySecs = DateTime.Now.AddSeconds(_preNotifyConfig);
                        ProcessConflicts(preNotifySecs);
                        _nextCheckTimeConflicts.AddSeconds(-_preNotifyConfig);
                        PluginMain.UpcomingRecordingsChanged = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("NotifyManager: error in _timer_Tick - {0}", ex.Message);
            }
            finally
            {
                _busy = false;
            }
        }

        #endregion
    }
}