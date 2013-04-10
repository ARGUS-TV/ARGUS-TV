/*
 *	Copyright (C) 2007-2013 ARGUS TV
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
using System.Text;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows.Forms;
using System.Net.Security;
using System.ServiceModel.Description;

using ArgusTV.ServiceContracts.Events;
using ArgusTV.Client.Common;
using ArgusTV.DataContracts;

namespace ArgusTV.Messenger
{
    [ServiceBehavior(
#if DEBUG
        IncludeExceptionDetailInFaults = true,
#endif
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]
    public class EventsListenerService : EventsListenerServiceBase
    {
        public delegate void RecordingEventDelegate(Recording recording);
        public delegate void StreamingEventDelegate(LiveStream liveStream);
        public delegate void ConfigurationChangedDelegate(string module, string key);

        public static event RecordingEventDelegate OnRecordingStarted;
        public static event RecordingEventDelegate OnRecordingEnded;
        public static event StreamingEventDelegate OnLiveStreamStarted;
        public static event StreamingEventDelegate OnLiveStreamTuned;
        public static event StreamingEventDelegate OnLiveStreamEnded;
        public static event EventHandler OnUpcomingRecordingsChanged;
        public static event EventHandler OnUpcomingAlertsChanged;
        public static event ConfigurationChangedDelegate OnConfigurationChanged;

        public static ServiceHost CreateServiceHost(string eventsServiceBaseUrl)
        {
            return CreateServiceHost(typeof(EventsListenerService), eventsServiceBaseUrl,
                typeof(IRecordingEventsListener), typeof(IScheduleEventsListener), typeof(ISystemEventsListener));
        }

        public override void RecordingStarted(Recording recording)
        {
            if (OnRecordingStarted != null)
            {
                OnRecordingStarted(recording);
            }
        }

        public override void RecordingEnded(Recording recording)
        {
            if (OnRecordingEnded != null)
            {
                OnRecordingEnded(recording);
            }
        }

        public override void LiveStreamTuned(LiveStream liveStream)
        {
            if (OnLiveStreamTuned != null)
            {
                OnLiveStreamTuned(liveStream);
            }
        }

        public override void LiveStreamStarted(LiveStream liveStream)
        {
            if (OnLiveStreamStarted != null)
            {
                OnLiveStreamStarted(liveStream);
            }
        }

        public override void LiveStreamEnded(LiveStream liveStream)
        {
            if (OnLiveStreamEnded != null)
            {
                OnLiveStreamEnded(liveStream);
            }
        }

        public override void UpcomingRecordingsChanged()
        {
            if (OnUpcomingRecordingsChanged != null)
            {
                OnUpcomingRecordingsChanged(this, EventArgs.Empty);
            }
        }

        public override void UpcomingAlertsChanged()
        {
            if (OnUpcomingAlertsChanged != null)
            {
                OnUpcomingAlertsChanged(this, EventArgs.Empty);
            }
        }

        public override void ConfigurationChanged(string moduleName, string key)
        {
            if (OnConfigurationChanged != null)
            {
                OnConfigurationChanged(moduleName, key);
            }
        }
    }
}
