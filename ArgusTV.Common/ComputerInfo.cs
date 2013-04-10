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
using System.Linq;
using System.Text;
using System.Management;
using System.Diagnostics;

using ArgusTV.Common.Logging;

namespace ArgusTV.Common
{
    /// <summary>
    /// ComputerInfo
    /// </summary>
    public static class ComputerInfo
    {
        private static string _rootKey = "\\\\.\\ROOT\\cimv2";
        private static string _notDetected = "Not detected";

        #region Properties

        /// <summary>
        /// The name of the computer.
        /// </summary>
        public static string Name { get { return Environment.MachineName; } }

        /// <summary>
        /// The name of this computers manufacturer.
        /// </summary>
        public static string Manufacturer { get { return RetrieveSingleProperty("Win32_ComputerSystem", "Manufacturer", _notDetected); } }

        /// <summary>
        /// The model of the computer.
        /// </summary>
        public static string Model { get { return RetrieveSingleProperty("Win32_ComputerSystem", "Model", _notDetected); } }

        /// <summary>
        /// The name of the owner of the computer.
        /// </summary>
        public static string PrimaryOwnerName { get { return RetrieveSingleProperty("Win32_ComputerSystem", "PrimaryOwnerName", _notDetected); } }

        /// <summary>
        /// The full name of the operating system running on the computer.
        /// </summary>
        public static string OperatingSystemAsString
        {
            get
            {
                string[] parts = new string[3];
                parts[0] = RetrieveSingleProperty("Win32_OperatingSystem", "Caption", String.Empty).Trim();
                parts[1] = RetrieveSingleProperty("Win32_OperatingSystem", "CSDVersion", String.Empty).Trim();
                parts[2] = RetrieveSingleProperty("Win32_OperatingSystem", "OSArchitecture", String.Empty).Trim();
                return parts.Aggregate((s1, s2) => String.IsNullOrEmpty(s2) ? s1 : String.Concat(s1, " - ", s2));
            }
        }

        /// <summary>
        /// The cpu present in the computer.
        /// </summary>
        public static string Cpu { get { return RetrieveSingleProperty("Win32_Processor", "Name", _notDetected); } }

        /// <summary>
        /// The number of cores in the computer.
        /// </summary>
        public static string NumberOfCores { get { return RetrieveSingleProperty("Win32_Processor", "NumberOfCores", String.Empty); } }

        /// <summary>
        /// The Id of the computers processor.
        /// </summary>
        public static string ProcessorId { get { return RetrieveSingleProperty("Win32_Processor", "ProcessorId", String.Empty); } }

        /// <summary>
        /// The name of the videocontroller in the computer.
        /// </summary>
        public static string VideoControllerName { get { return RetrieveSingleProperty("Win32_VideoController", "Caption", _notDetected); } }

        /// <summary>
        /// The date of the videocontroller's driver in the computer.
        /// </summary>
        public static string VideoControllerDriverDate { get { return RetrieveSingleProperty("Win32_VideoController", "DriverDate", _notDetected); } }

        /// <summary>
        /// The version of the videocontroller's driver in the computer.
        /// </summary>
        public static string VideoControllerDriverVersion { get { return RetrieveSingleProperty("Win32_VideoController", "DriverVersion", _notDetected); } }

        /// <summary>
        /// The mode of the videocontroller present in the computer.
        /// </summary>
        public static string VideoModeDescription { get { return RetrieveSingleProperty("Win32_VideoController", "VideoModeDescription", _notDetected); } }

        /// <summary>
        /// The operating system running on the computer, as flags.
        /// </summary>
        public static OperatingSystem OperatingSystem
        {
            get
            {
                OperatingSystem returnValue = OperatingSystem.Unknown;

                System.OperatingSystem osInfo = System.Environment.OSVersion;

                // Architecture
                returnValue |= (Environment.Is64BitOperatingSystem ? OperatingSystem.Windows_64Bit : OperatingSystem.Windows_32Bit);

                // Platform
                switch (osInfo.Platform)
                {
                    case System.PlatformID.Win32S:
                        returnValue |= OperatingSystem.WindowsWin32s;
                        break;

                    case System.PlatformID.WinCE:
                        returnValue |= OperatingSystem.WindowsCE;
                        break;

                    // Windows 95, Windows 98, Windows 98 Second Edition, or Windows Me
                    case System.PlatformID.Win32Windows:

                        switch (osInfo.Version.Minor)
                        {
                            case 0:
                                returnValue |= OperatingSystem.Windows95;
                                break;
                            case 10:
                                if (osInfo.Version.Revision.ToString() == "2222A")
                                    returnValue |= OperatingSystem.Windows98 | OperatingSystem.Windows98_SE;
                                else
                                    returnValue |= OperatingSystem.Windows98;
                                break;
                            case 90:
                                returnValue |= OperatingSystem.WindowsME;
                                break;
                        }
                        break;

                    // Windows NT 3.51, Windows NT 4.0, Windows 2000 or Windows XP.
                    case System.PlatformID.Win32NT:

                        switch (osInfo.Version.Major)
                        {
                            default:
                                if (osInfo.Version.Major > 6)
                                    returnValue |= OperatingSystem.LaterVersion;
                                break;
                            case 3:
                                returnValue |= OperatingSystem.WindowsNT | OperatingSystem.WindowsNT_351;
                                break;
                            case 4:
                                returnValue |= OperatingSystem.WindowsNT | OperatingSystem.WindowsNT_40;
                                break;
                            case 5:
                                if (osInfo.Version.Minor == 0)
                                    returnValue |=
                                        OperatingSystem.Windows2000 |
                                        AddServicePacks(
                                            returnValue,
                                            osInfo.ServicePack,
                                            new string[] { "1", "2", "3", "4" },
                                            new OperatingSystem[] 
                                            { 
                                                OperatingSystem.Windows2000_SP1, 
                                                OperatingSystem.Windows2000_SP2, 
                                                OperatingSystem.Windows2000_SP3,
                                                OperatingSystem.Windows2000_SP4
                                            });
                                else
                                {
                                    returnValue |=
                                        OperatingSystem.WindowsXP |
                                        AddServicePacks(
                                            returnValue,
                                            osInfo.ServicePack,
                                            new string[] { "1", "2", "3" },
                                            new OperatingSystem[] 
                                            { 
                                                OperatingSystem.WindowsXP_SP1, 
                                                OperatingSystem.WindowsXP_SP2, 
                                                OperatingSystem.WindowsXP_SP3 
                                            });
                                }
                                break;
                            case 6:
                                if (osInfo.Version.Minor == 0)
                                    returnValue |=
                                        OperatingSystem.WindowsVista |
                                        AddServicePacks(
                                            returnValue,
                                            osInfo.ServicePack,
                                            new string[] { "1", "2" },
                                            new OperatingSystem[] 
                                            { 
                                                OperatingSystem.WindowsVista_SP1, 
                                                OperatingSystem.WindowsVista_SP2 
                                            });
                                else
                                    returnValue |=
                                        OperatingSystem.Windows7 |
                                        AddServicePacks(
                                            returnValue,
                                            osInfo.ServicePack,
                                            new string[] { "1" },
                                            new OperatingSystem[] { OperatingSystem.Windows7_SP1 });
                                break;
                        }

                        break;
                }
                return returnValue;
            }
        }

        /// <summary>
        /// Is the computer running on an NT platform ?
        /// </summary>
        public static bool IsWindowsNT
        {
            get
            {
                return (PlatformID.Win32NT == Environment.OSVersion.Platform);
            }
        }

        /// <summary>
        /// Returns true if the computer is running Windows 2000 or higher
        /// </summary>
        public static bool IsWindows2KUp
        {
            get
            {
                return (PlatformID.Win32NT == Environment.OSVersion.Platform && Environment.OSVersion.Version.Major >= 5);
            }
        }

        #endregion

        /// <summary>
        /// Is the computer running the given operatingsystem ?
        /// </summary>
        /// <param name="operatingSystem"></param>
        /// <returns></returns>
        public static bool IsRunningOperatingSystem(OperatingSystem operatingSystem)
        {
            return ((OperatingSystem & operatingSystem) == operatingSystem);
        }

        public static void Log()
        {
            try
            {
                Logger.Write(TraceEventType.Information, "Computer name: " + Name + ", manufacturer: " + Manufacturer + ", model: " + Model);
                Logger.Write(TraceEventType.Information, "  OS: " + OperatingSystemAsString);
                Logger.Write(TraceEventType.Information, "  CPU: " + Cpu);
                Logger.Write(TraceEventType.Verbose, "  Video controller: " + VideoControllerName);
                Logger.Write(TraceEventType.Verbose, "   Driver version: " + VideoControllerDriverVersion + ", date: " + VideoControllerDriverDate);
                Logger.Write(TraceEventType.Verbose, "   Video mode: " + VideoModeDescription);
            }
            catch { }
        }

        private static string RetrieveSingleProperty(string sectionKey, string propertyKey, string defaultValue)
        {
            try
            {
                EnumerationOptions options = new EnumerationOptions() { ReturnImmediately = true, Rewindable = false };
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(new ManagementScope(_rootKey), new ObjectQuery("SELECT * FROM " + sectionKey), options))
                using (ManagementObjectCollection queryCollection = searcher.Get())
                {
                    foreach (ManagementObject managementObject in queryCollection)
                    {
                        foreach (PropertyData propertyData in managementObject.Properties)
                        {
                            if (propertyData.Name == propertyKey
                                && propertyData.Value != null)
                            {
                                string result = propertyData.Value.ToString();
                                if (!String.IsNullOrEmpty(result))
                                {
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            return defaultValue;
        }

        private static OperatingSystem AddServicePacks(OperatingSystem operatingSystem, string servicePack, string[] conditions, OperatingSystem[] values)
        {
            for (int liCounter = 0; liCounter < Math.Min(conditions.Length, values.Length); liCounter++)
            {
                if (servicePack.Contains(conditions[liCounter]))
                {
                    operatingSystem |= values[liCounter];
                }
            }
            return operatingSystem;
        }
    }
}
