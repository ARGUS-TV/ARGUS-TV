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
using System.Threading;
using RestSharp;
using ArgusTV.DataContracts;

namespace ArgusTV.ServiceProxy
{
    /// <summary>
    /// Core service proxy.
    /// </summary>
    public partial class CoreServiceProxy : RestProxyBase
    {
        /// <summary>
        /// Constructs a channel to the service.
        /// </summary>
        public CoreServiceProxy()
            : base("Core")
        {
        }

        #region Initialization

        /// <summary>
        /// Ping ARGUS TV server and test the API version.
        /// </summary>
        /// <param name="requestedApiVersion">The API version the client needs, pass in Constants.CurrentApiVersion.</param>
        /// <returns>0 if client and server are compatible, -1 if the client is too old and +1 if the client is newer than the server.</returns>
        public int Ping(int requestedApiVersion)
        {
            var request = NewRequest("/Ping/{requestedApiVersion}", Method.GET);
            request.AddParameter("requestedApiVersion", requestedApiVersion, ParameterType.UrlSegment);
            return Execute<PingResult>(request).Result;
        }

        private class PingResult
        {
            public int Result { get; set; }
        }

        /// <summary>
        /// Get the server's MAC address(es).  These can be stored on the client after a successful
        /// connect and later used to re-connect with wake-on-lan.
        /// </summary>
        /// <returns>An array containing one or more MAC addresses in HEX string format (e.g. "A1B2C3D4E5F6").</returns>
        public IEnumerable<string> GetMacAddresses()
        {
            var request = NewRequest("/GetMacAddresses", Method.GET);
            return Execute<List<string>>(request);
        }

        /// <summary>
        /// Check to see if there is a newer version of ARGUS TV available online.
        /// </summary>
        /// <returns>A NewVersionInfo object if there is a newer version available, null if the current installation is up-to-date.</returns>
        public NewVersionInfo IsNewerVersionAvailable()
        {
            var request = NewRequest("/IsNewerVersionAvailable", Method.GET);
            return Execute<NewVersionInfo>(request);
        }

        #endregion

        #region Event Listeners

        /// <summary>
        /// Subscribe your client to a group of ARGUS TV events using a polling mechanism.
        /// </summary>
        /// <param name="uniqueClientId">The unique ID (e.g. your DNS hostname combined with a constant GUID) to identify your client.</param>
        /// <param name="eventGroups">The event group(s) to subscribe to (flags can be OR'd).</param>
        public void SubscribeServiceEvents(string uniqueClientId, EventGroup eventGroups)
        {
            var request = NewRequest("/ServiceEvents/{uniqueClientId}/Subscribe/{eventGroups}", Method.POST);
            request.AddParameter("uniqueClientId", uniqueClientId, ParameterType.UrlSegment);
            request.AddParameter("eventGroups", (int)eventGroups, ParameterType.UrlSegment);
            Execute(request);
        }

        /// <summary>
        /// Unsubscribe your client from all ARGUS TV events.
        /// </summary>
        /// <param name="uniqueClientId">The unique ID (e.g. your DNS hostname combined with a constant GUID) to identify your client.</param>
        public void UnsubscribeServiceEvents(string uniqueClientId)
        {
            var request = NewRequest("/ServiceEvents/{uniqueClientId}/Unsubscribe", Method.POST);
            request.AddParameter("uniqueClientId", uniqueClientId, ParameterType.UrlSegment);
            Execute(request);
        }

        /// <summary>
        /// Get all queued ARGUS TV events for your client. Call this every X seconds to get informed at a regular interval about what happened.
        /// </summary>
        /// <param name="uniqueClientId">The unique ID (e.g. your DNS hostname combined with a constant GUID) to identify your client.</param>
        /// <returns>Zero or more service events, or null in case your subscription has expired.</returns>
        public List<ServiceEvent> GetServiceEvents(string uniqueClientId, WaitHandle cancellationWaitHandle = null, int timeoutSeconds = 300)
        {
            var request = NewRequest("/ServiceEvents/{uniqueClientId}/{timeout}", Method.GET);
            request.AddParameter("uniqueClientId", uniqueClientId, ParameterType.UrlSegment);
            request.AddParameter("timeout", timeoutSeconds, ParameterType.UrlSegment);

            request.Timeout = Math.Min(timeoutSeconds, 30) * 1000;

            // By default return an empty list (e.g. in case of a timeout or abort), the client will simply need to call this again.
            GetServiceEventsResult result = new GetServiceEventsResult()
            {
                Events = new List<ServiceEvent>()
            };

            using (ManualResetEvent doneEvent = new ManualResetEvent(false))
            {
                var asyncHandle = _client.ExecuteAsync(request, r =>
                {
                    if (r != null
                        && r.ResponseStatus == ResponseStatus.Completed)
                    {
                        result = DeserializeResponseContent<GetServiceEventsResult>(r);
                    }
                    doneEvent.Set();
                });

                List<WaitHandle> waitHandles = new List<WaitHandle>();
                waitHandles.Add(doneEvent);
                if (cancellationWaitHandle != null)
                {
                    waitHandles.Add(cancellationWaitHandle);
                }

                if (WaitHandle.WaitAny(waitHandles.ToArray()) == 1)
                {
                    asyncHandle.Abort();
                    return new List<ServiceEvent>();
                }
            }

            if (result == null
                || result.Expired)
            {
                return null;
            }
            return result.Events;
        }

        private class GetServiceEventsResult
        {
            public bool Expired { get; set; }
            public List<ServiceEvent> Events { get; set; }
        }

        #endregion

        #region Power Management

        /// <summary>
        /// Tell the server we'd like to keep it alive for a little longer.  A client
        /// should call this method every two minutes or so to keep the server from
        /// entering standby (if it is configured to do so).
        /// </summary>
        public void KeepServerAlive()
        {
            var request = NewRequest("/KeepServerAlive", Method.POST);
            ExecuteAsync(request);
        }

        #endregion
    }
}
