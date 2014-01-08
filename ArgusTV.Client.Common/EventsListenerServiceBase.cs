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
using System.Text;
using System.ServiceModel;
using System.Net.Security;

using ArgusTV.ServiceContracts.Events;
using ArgusTV.DataContracts;

namespace ArgusTV.Client.Common
{
    public abstract class EventsListenerServiceBase : ISystemEventsListener, IScheduleEventsListener, IGuideEventsListener, IRecordingEventsListener
    {
        public static ServiceHost CreateServiceHost(Type serviceType, string eventsServiceBaseUrl, params Type[] interfaceTypes)
        {
            ServiceHost host = new ServiceHost(serviceType, new Uri(eventsServiceBaseUrl));
            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.None);
            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
            tcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.None;
            tcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            foreach (Type interfaceType in interfaceTypes)
            {
                host.AddServiceEndpoint(interfaceType, tcpBinding, interfaceType.Name.Substring(1));
            }
            return host;
        }

        #region ISystemEventsListener Members

        public virtual void EnteringStandby()
        {
        }

        public virtual void SystemResumed()
        {
        }

        public virtual void ConfigurationChanged(string moduleName, string key)
        {
        }

        #endregion

        #region IScheduleEventsListener Members

        public virtual void UpcomingAlertsChanged()
        {
        }

        public virtual void UpcomingSuggestionsChanged()
        {
        }

        public virtual void ScheduleChanged(Guid scheduleId)
        {
        }

        #endregion

        #region IGuideEventsListener Members

        public virtual void NewGuideData()
        {
        }

        #endregion

        #region IRecordingEventsListener Members

        public virtual void UpcomingRecordingsChanged()
        {
        }

        public virtual void ActiveRecordingsChanged()
        {
        }

        public virtual void RecordingStarted(Recording recording)
        {
        }

        public virtual void RecordingEnded(Recording recording)
        {
        }

        public virtual void LiveStreamStarted(LiveStream liveStream)
        {
        }

        public virtual void LiveStreamTuned(LiveStream liveStream)
        {
        }

        public virtual void LiveStreamEnded(LiveStream liveStream)
        {
        }

        public virtual void LiveStreamAborted(LiveStream abortedStream, LiveStreamAbortReason reason, UpcomingProgram program)
        {
        }

        #endregion
    }
}
