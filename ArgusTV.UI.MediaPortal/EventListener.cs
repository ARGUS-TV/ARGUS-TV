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
using System.Net;
using System.ServiceModel;

using MediaPortal.GUI.Library;
using MediaPortal.Player;

using ArgusTV.ServiceContracts.Events;
using ArgusTV.ServiceAgents;
using ArgusTV.DataContracts;
using ArgusTV.Client.Common;

namespace ArgusTV.UI.MediaPortal
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]

    public class EventListener : EventsListenerServiceBase
    {
        private string _serviceUrl = string.Empty;
        private ServiceHost _serviceHost = null;
        private bool _started = false;

        # region EventListener Members

        public override void LiveStreamAborted(LiveStream abortedStream, LiveStreamAbortReason reason, UpcomingProgram program)
        {
            Log.Debug("Eventlistener: Livestreamaborted, stream = {0}, reason = {1}", abortedStream.RtspUrl, reason.ToString());
            OnLiveStreamEnded(abortedStream, reason, program);
        }

        public override void LiveStreamEnded(LiveStream liveStream)
        {
            Log.Debug("EventListener: LiveStreamEnded()");
            OnLiveStreamEnded(liveStream,LiveStreamAbortReason.Unknown,null);
        }

        public override void UpcomingRecordingsChanged()
        {
            Log.Debug("EventListener: UpcomingRecordingsChanged()");
            PluginMain.UpcomingRecordingsChanged = true;
            UpdateGuide();
        }

        public override void RecordingStarted(Recording recording)
        {
            Log.Debug("EventListener: recording started: {0}", recording.Title);
            PluginMain.ActiveRecordingsChanged = true;
            OnrecordingStarted(recording);
        }

        public override void RecordingEnded(Recording recording)
        {
            Log.Debug("EventListener: recording ended: {0}", recording.Title);
            PluginMain.ActiveRecordingsChanged = true;
            OnRecordingEnded(recording);
            UpdateRecordings();
        }

        public override void UpcomingAlertsChanged()
        {
            Log.Debug("EventListener: UpcomingAlertsChanged()");
            PluginMain.UpcomingAlertsChanged = true;
            UpdateGuide();
        }

        public override void UpcomingSuggestionsChanged()
        {
            Log.Debug("EventListener: UpcomingSuggestionsChanged()");
            UpdateGuide();
        }

        # endregion

        # region Start/Stop

        public void EnsureListener()
        {
            if (!_started)
            {
                StartListener();
            }
        }

        public void StartListener()
        {
            if (PluginMain.IsConnected())
            {
                int _port = GetFreeTcpPort(49800);
                if (_port == -1)
                {
                    Log.Error("EventListener: No free port found!");
                    return;
                }

                _serviceUrl = "net.tcp://" + Dns.GetHostName() + ":" + _port + "/MpClient/";
                StopListener();

                Log.Debug("EventListener: start()");
                _serviceHost = CreateServiceHost(_serviceUrl);
                _serviceHost.Open();

                _started = true;
                try
                {
                    using (CoreServiceAgent agent = new CoreServiceAgent())
                    {
                        agent.EnsureEventListener(EventGroup.RecordingEvents | EventGroup.ScheduleEvents, _serviceUrl, Constants.EventListenerApiVersion);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("EventListener: StartListener() error = {0}", ex.Message);
                    _started = false;
                }
            }
        }

        public void StopListener()
        {
            if (PluginMain.IsConnected())
            {
                Log.Debug("EventListener: stop()");
                try
                {
                    using (CoreServiceAgent agent = new CoreServiceAgent())
                    {
                        agent.RemoveEventListener(_serviceUrl);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("EventListener: StopListener() error = {0}", ex.Message);
                }

                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                }
                _started = false;
            }
        }

        #endregion

        #region Private Methods

        private void OnLiveStreamEnded(LiveStream liveStream, LiveStreamAbortReason reason, UpcomingProgram program)
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_STOP_SERVER_TIMESHIFTING, 0, 0, 0, 0, 0, null);
            msg.Object = liveStream;
            msg.Object2 = program;
            msg.Label = reason.ToString();
            msg.Param1 = 4321;//indentification
            GUIGraphicsContext.SendMessage(msg);
            msg = null;
        }

        private void OnRecordingEnded(Recording recording)
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_NOTIFY_REC, 0, 0, 0, 0, 0, null);
            msg.Param1 = 0;//ended
            msg.Object = recording;
            GUIGraphicsContext.SendMessage(msg);
            msg = null;
        }

        private void OnrecordingStarted(Recording recording)
        {
            GUIMessage msg = new GUIMessage(GUIMessage.MessageType.GUI_MSG_NOTIFY_REC, 0, 0, 0, 0, 0, null);
            msg.Param1 = 1;//started
            msg.Object = recording;
            GUIGraphicsContext.SendMessage(msg);
            msg = null;
        }

        private void UpdateGuide()
        {
            if (GUIWindowManager.ActiveWindow == WindowId.TvGuide
                || GUIWindowManager.ActiveWindow == WindowId.RadioGuide)
            {
                GuideBase.ReloadSchedules = true;
            }
        }

        private void UpdateRecordings()
        {
            if (GUIWindowManager.ActiveWindow == WindowId.RecordedRadio
                || GUIWindowManager.ActiveWindow == WindowId.RecordedTv)
            {
                RecordedBase.NeedUpdate = true;
            }
        }

        private static int GetFreeTcpPort(int port)
        {
            int _port = -1;
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    System.Net.Sockets.TcpListener tcpListener = new System.Net.Sockets.TcpListener(IPAddress.Any, port);
                    tcpListener.Start();
                    tcpListener.Stop();
                    _port = port;
                    break;
                }
                catch (System.Net.Sockets.SocketException)
                {
                    port--;
                }
            }
            Log.Debug("EventListener: listener port = {0}", port);
            return _port;
        }

        private static ServiceHost CreateServiceHost(string eventsServiceBaseUrl)
        {
            return CreateServiceHost(typeof(EventListener), eventsServiceBaseUrl, typeof(IRecordingEventsListener), typeof(IScheduleEventsListener));
        }

        #endregion
    }
}
