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
using System.Globalization;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ArgusTV.Common.Recorders.Utility
{
    public static class WakeOnLan
    {
        private const string _defaultSubnetMask = "255.255.255.0";

        public static string GetIPAddress(string hostName)
        {
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ipAddress in addresses)
            {
                if (!IPAddress.IsLoopback(ipAddress)
                    && ipAddress.AddressFamily == AddressFamily.InterNetwork
                    && Ping(ipAddress))
                {
                    return ipAddress.ToString();
                }
            }
            return String.Empty;
        }

        public static void EnsureServerAwake(string ipAddress, string macAddresses, int timeoutSeconds = 60)
        {
            // Check if wake-on-lan is turned on and if the server does *not* respond to a ping.
            // In that case we will try to wake it up.
            if (!String.IsNullOrEmpty(macAddresses)
                && !String.IsNullOrEmpty(ipAddress)
                && !Ping(ipAddress))
            {
                string[] macs = macAddresses.Split(';');
                if (WakeUp(macs, ipAddress, timeoutSeconds))
                {
                    // Wait one additional second after a successful ping.
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private static bool Ping(string ipString)
        {
            IPAddress ipAddress;
            if (IPAddress.TryParse(ipString, out ipAddress))
            {
                using (Ping ping = new Ping())
                {
                    return (ping.Send(ipAddress, 500).Status == IPStatus.Success);
                }
            }
            return false;
        }

        private static bool Ping(IPAddress ipAddress)
        {
            using (Ping ping = new Ping())
            {
                return (ping.Send(ipAddress, 500).Status == IPStatus.Success);
            }
        }

        private static bool WakeUp(string[] macAddresses, string serverIPAddress, int timeoutSeconds)
        {
            bool result = false;
            bool pingAfterWake = true;

            IPAddress ipAddress;
            if (!IPAddress.TryParse(serverIPAddress, out ipAddress))
            {
                ipAddress = IPAddress.Broadcast;
                pingAfterWake = false;
            }

            foreach (string macAddress in macAddresses)
            {
                SendWakeOnLan(macAddress, ipAddress);
            }

            if (pingAfterWake)
            {
                DateTime startPingTime = DateTime.Now;
                for (; ; )
                {
                    if (Ping(ipAddress))
                    {
                        result = true;
                        break;
                    }
                    TimeSpan span = DateTime.Now - startPingTime;
                    if (span.TotalSeconds > timeoutSeconds)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        private static void SendWakeOnLan(string mac, IPAddress ipAddress)
        {
            byte[] macBytes = ConvertMacAddress(mac);
            if (macBytes != null)
            {
                byte[] packet = CreateWakeOnLanPacket(macBytes);

                IPAddress subnetMask = FindSubnetMask(ipAddress);

                IPEndPoint endPoint = new IPEndPoint(ApplySubnetMask(ipAddress, subnetMask), 9);

                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    socket.SendTo(packet, endPoint);
                    socket.Close();
                }
            }
        }

        private static byte[] CreateWakeOnLanPacket(byte[] macBytes)
        {
            byte[] packet = new byte[21 * 6];
            for (int index = 0; index < 6; index++)
            {
                packet[index] = 0xFF;
            }
            for (int count = 1; count <= 20; count++)
            {
                for (int index = 0; index < 6; index++)
                {
                    packet[count * 6 + index] = macBytes[index];
                }
            }
            return packet;
        }

        private static byte[] ConvertMacAddress(string mac)
        {
            if (mac.Length != 12)
            {
                return null;
            }
            byte[] macBytes = new byte[6];
            for (int index = 0; index < 6; index++)
            {
                byte byteValue;
                if (!byte.TryParse(mac.Substring(index * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byteValue))
                {
                    return null;
                }
                macBytes[index] = byteValue;
            }
            return macBytes;
        }

        private static IPAddress FindSubnetMask(IPAddress serverIPAddress)
        {
#if !MONO
            using (ManagementObjectSearcher query = new ManagementObjectSearcher(
                "Select IPAddress,IPSubnet from Win32_NetworkAdapterConfiguration where IPEnabled=TRUE"))
            using (ManagementObjectCollection mgmntObjects = query.Get())
            {
                foreach (ManagementObject mo in mgmntObjects)
                {
                    string[] ipaddresses = (string[])mo["IPAddress"];
                    string[] subnets = (string[])mo["IPSubnet"];
                    for (int index = 0; index < Math.Min(ipaddresses.Length, subnets.Length); index++)
                    {
                        IPAddress localIP;
                        IPAddress subnetIP;
                        if (IPAddress.TryParse(ipaddresses[index], out localIP)
                            && IPAddress.TryParse(subnets[index], out subnetIP))
                        {
                            if (ApplySubnetMask(localIP, subnetIP).Equals(ApplySubnetMask(serverIPAddress, subnetIP)))
                            {
                                return subnetIP;
                            }
                        }
                    }
                }
            }
#endif
            return IPAddress.Parse(_defaultSubnetMask);
        }

        private static IPAddress ApplySubnetMask(IPAddress ipAddress, IPAddress ipNetMask)
        {
            byte[] ipBytes = ipAddress.GetAddressBytes();
            byte[] maskBytes = ipNetMask.GetAddressBytes();
            for (int index = 0; index < 4; index++)
            {
                ipBytes[index] |= (byte)(maskBytes[index] ^ 0xFF);
            }
            return new IPAddress(ipBytes);
        }
    }
}
