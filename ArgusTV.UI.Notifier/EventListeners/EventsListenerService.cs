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
using System.ServiceModel;

using ArgusTV.ServiceContracts.Events;
using ArgusTV.DataContracts;
using ArgusTV.Client.Common;

namespace ArgusTV.UI.Notifier.EventListeners
{
    //
    // NOTE: methods will run on our main thread thanks to the WCF thread synchronization context!
    //
    [ServiceBehavior(
#if DEBUG
        IncludeExceptionDetailInFaults = true,
#endif
        ConcurrencyMode = ConcurrencyMode.Single,
        InstanceContextMode = InstanceContextMode.Single)]
    public class EventsListenerService : EventsListenerServiceBase
    {
        private static StatusForm _statusForm;

        public static StatusForm StatusForm
        {
            get { return _statusForm; }
            set { _statusForm = value; }
        }

        public static ServiceHost CreateServiceHost(string eventsServiceBaseUrl)
        {
            return CreateServiceHost(typeof(EventsListenerService), eventsServiceBaseUrl,
                typeof(IRecordingEventsListener));
        }

        public override void UpcomingRecordingsChanged()
        {
            if (_statusForm != null)
            {
                _statusForm.OnUpcomingRecordingsChanged();
            }
        }

        public override void ActiveRecordingsChanged()
        {
            if (_statusForm != null)
            {
                _statusForm.OnActiveRecordingsChanged();
            }
        }

        public override void RecordingStarted(Recording recording)
        {
            if (_statusForm != null)
            {
                _statusForm.OnRecordingStarted(recording);
            }
        }

        public override void RecordingEnded(Recording recording)
        {
            if (_statusForm != null)
            {
                _statusForm.OnRecordingEnded(recording);
            }
        }

        public override void LiveStreamStarted(LiveStream liveStream)
        {
            if (_statusForm != null)
            {
                _statusForm.OnLiveStreamStarted(liveStream);
            }
        }

        public override void LiveStreamTuned(LiveStream liveStream)
        {
            if (_statusForm != null)
            {
                _statusForm.OnLiveStreamStarted(liveStream);
            }
        }

        public override void LiveStreamEnded(LiveStream liveStream)
        {
            if (_statusForm != null)
            {
                _statusForm.OnLiveStreamEnded(liveStream);
            }
        }
    }
}
