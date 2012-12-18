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
