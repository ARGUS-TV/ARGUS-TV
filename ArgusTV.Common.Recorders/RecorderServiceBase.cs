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
using System.Reflection;
using ArgusTV.Common.Logging;
using ArgusTV.Common.StaThreadSyncronizer;
using ArgusTV.DataContracts;
using ArgusTV.DataContracts.Tuning;
using System.Management;

namespace ArgusTV.Common.Recorders
{
    public abstract class RecorderServiceBase : IDisposable
    {
		private static bool _disposed;

        protected bool Disposed
        {
            get { return _disposed; }
        }

        protected abstract string Name
        {
            get;
        }

        protected Guid RecorderId
        {
            get; private set;
        }

        /// <summary>
        /// Check if a channel was already allocated to a card.
        /// </summary>
        /// <param name="alreadyAllocated">The array of previously allocated cards.
        /// <param name="cardId">The ID of the card we want to check.
        /// <param name="channelId">The ID of the channel.
        /// <returns>True if the channel was already allocated to this card, false otherwise.</returns>
        protected bool ChannelAlreadyAllocatedOn(CardChannelAllocation[] alreadyAllocated, string cardId, Guid channelId)
        {
            foreach (CardChannelAllocation allocation in alreadyAllocated)
            {
                if (allocation.CardId == cardId
                    && allocation.ChannelId == channelId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Count the number of times a card has been allocated.
        /// </summary>
        /// <param name="alreadyAllocated">The array of previously allocated cards.
        /// <param name="cardId">The ID of the card we want to check.
        /// <returns>The number of times this card has been allocated.</returns>
        protected int CountNumTimesAllocated(IList<CardChannelAllocation> alreadyAllocated, string cardId)
        {
            int count = 0;
            foreach (CardChannelAllocation allocation in alreadyAllocated)
            {
                if (allocation.CardId == cardId)
                {
                    count++;
                }
            }
            return count;
        }

        public virtual int Ping()
        {
            return Constants.RecorderApiVersion;
        }

        public virtual void KeepAlive()
        {
#if !MONO
            // Inform the local system we need it.
            SetThreadExecutionState(EXECUTION_STATE.ES_SYSTEM_REQUIRED);
#endif
        }

        public virtual List<string> GetMacAddresses()
        {
            List<string> macAddresses = new List<string>();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT
                || Environment.OSVersion.Platform == PlatformID.Win32Windows)
            {
                using (var query = new ManagementObjectSearcher("Select MacAddress from Win32_NetworkAdapterConfiguration where IPEnabled=TRUE"))
                    using (var mgmntObjects = query.Get())
                {
                    foreach (ManagementObject mo in mgmntObjects)
                    {
                        string mac = (string)mo["MacAddress"];
                        if (!String.IsNullOrEmpty(mac)
                            && mac != "00:00:00:00:00:00")
                        {
                            mac = mac.Replace(":", "");
                            if (!macAddresses.Contains(mac))
                            {
                                macAddresses.Add(mac);
                            }
                        }
                        mo.Dispose();
                    }
                }
            }
            else
            {
                System.Net.NetworkInformation.NetworkInterfaceType Type = 0;
                try
                {
                    System.Net.NetworkInformation.NetworkInterface[] theNetworkInterfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                    foreach (System.Net.NetworkInformation.NetworkInterface currentInterface in theNetworkInterfaces)
                    {
                        Type = currentInterface.NetworkInterfaceType;
                        if (Type == System.Net.NetworkInformation.NetworkInterfaceType.Ethernet || Type == System.Net.NetworkInformation.NetworkInterfaceType.GigabitEthernet || Type == System.Net.NetworkInformation.NetworkInterfaceType.FastEthernetFx)
                        {
                            string mac = currentInterface.GetPhysicalAddress().ToString();
                            mac = mac.Replace(":", "");
                            if (!macAddresses.Contains(mac))
                            {
                                macAddresses.Add(mac);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return macAddresses;
        }

        public virtual void Initialize(Guid recorderId, string schedulerBaseUrl)
        {
            RecorderId = recorderId;
            new RecorderCallbackProxy(schedulerBaseUrl).RegisterRecorder(RecorderId, this.Name, Assembly.GetCallingAssembly().GetName().Version.ToString()).Wait();
        }

        public abstract string AllocateCard(Channel channel, IList<CardChannelAllocation> alreadyAllocated, bool useReversePriority);

        public abstract bool StartRecording(string schedulerBaseUrl, CardChannelAllocation channelAllocation, DateTime startTimeUtc, DateTime stopTimeUtc, UpcomingProgram recordingProgram, string suggestedBaseFileName);

        public abstract bool ValidateAndUpdateRecording(CardChannelAllocation channelAllocation, UpcomingProgram recordingProgram, DateTime stopTimeUtc);

        public abstract bool AbortRecording(string schedulerBaseUrl, UpcomingProgram recordingProgram);

        public abstract List<string> GetRecordingShares();

        public abstract List<string> GetTimeshiftShares();

        #region Live TV/Radio

        public virtual LiveStreamResult TuneLiveStream(Channel channel, CardChannelAllocation upcomingRecordingAllocation, ref LiveStream liveStream, StreamingMode mode)
        {
            liveStream = null;
            return LiveStreamResult.NotSupported;
        }

        public virtual void StopLiveStream(LiveStream liveStream)
        {
        }

        public virtual IList<LiveStream> GetLiveStreams()
        {
            return new LiveStream[] { };
        }

        public virtual bool KeepLiveStreamAlive(LiveStream liveTvStream)
        {
            return false;
        }

        public virtual IList<ChannelLiveState> GetChannelsLiveState(IList<Channel> channels, LiveStream liveStream)
        {
            return null;
        }

        public virtual ServiceTuning GetLiveStreamTuningDetails(LiveStream liveStream)
        {
            return null;
        }

        #endregion

        #region TeleText

        public virtual bool HasTeletext(LiveStream liveStream)
        {
            return false;
        }

        public virtual void StartGrabbingTeletext(LiveStream liveStream)
        {         
        }

        public virtual void StopGrabbingTeletext(LiveStream liveStream)
        { 
        }

        public virtual bool IsGrabbingTeletext(LiveStream liveStream)
        {
            return false;
        }

        public virtual byte[] GetTeletextPageBytes(LiveStream liveStream, int pageNumber, int subPageNumber, out int subPageCount)
        {
            subPageCount = 0;
            return null;
        }

        #endregion

		#region IDisposable Pattern

        ~RecorderServiceBase()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					// We have no managed resources.
				}
				// We have no unmanaged resources.
			}
			_disposed = true;
        }

        #endregion

        #region P/Invoke

        [FlagsAttribute]
        private enum EXECUTION_STATE : uint
        {
            ES_SYSTEM_REQUIRED = 0x00000001,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000
        }

        [System.Runtime.InteropServices.DllImport("Kernel32.DLL", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private extern static EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE state);

        #endregion
    }
}
