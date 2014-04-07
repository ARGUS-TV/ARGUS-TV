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
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Net;
using System.ServiceProcess;
using System.Net.NetworkInformation;

using MediaPortal.GUI.Library;
using MediaPortal.Util;
using MediaPortal.Profile;
using MediaPortal.Configuration;
using MediaPortal.Player;
using MediaPortal.Database;
using MediaPortal.Picture.Database;
using MediaPortal.Video.Database;
using MediaPortal.Music.Database;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process.Guide;
using ArgusTV.ServiceProxy;
using ArgusTV.UI.Process;
using ArgusTV.Client.Common;

namespace ArgusTV.UI.MediaPortal
{
    internal static class Utility
    {
        #region Translation/Property

        internal static void ClearProperty(string property)
        {
            if (String.IsNullOrEmpty(GUIPropertyManager.GetProperty(property)))
            {
                GUIPropertyManager.SetProperty(property, " ");
            }
            GUIPropertyManager.SetProperty(property, String.Empty);
        }

        internal static string GetLocalizedText(TextId textId, params object[] args)
        {
            string result = null;
            if ((int)textId > 49000 && (int)textId < 50000)//filter out the mediaportal strings.
            {
                string isoCode = GUILocalizeStrings.GetCultureName(GUILocalizeStrings.CurrentLanguage());
                result = Translator.GetLocalizedText(isoCode, textId);
            }

            if (result == null)
            {
                result = GUILocalizeStrings.Get((int)textId, args);
            }
            return result == null ? String.Empty : result;
        }

        #endregion

        #region Icons/thumbs

        internal static string GetIconImageFileName(ScheduleType scheduleType, GuideUpcomingProgram upcoming)
        {
            return GetIconImageFileName(scheduleType, upcoming.IsPartOfSeries, upcoming.CancellationReason, upcoming.UpcomingRecording);
        }

        internal static string GetIconImageFileName(ScheduleType scheduleType, bool isPartOfSeries, UpcomingCancellationReason cancellationReason,
            UpcomingRecording upcomingRecording)
        {
            if (upcomingRecording != null)
            {
                return GetIconImageFileName(scheduleType, isPartOfSeries, cancellationReason,
                    true, upcomingRecording.CardChannelAllocation, upcomingRecording.ConflictingPrograms);
            }
            else
            {
                return GetIconImageFileName(scheduleType, isPartOfSeries, cancellationReason, false, null, null);
            }
        }

        internal static string GetIconImageFileName(ActiveRecording activeRecording)
        {
            return GetIconImageFileName(ScheduleType.Recording, activeRecording.Program.IsPartOfSeries, activeRecording.Program.CancellationReason,
                true, activeRecording.CardChannelAllocation, activeRecording.ConflictingPrograms);
        }

        private static string GetIconImageFileName(ScheduleType scheduleType, bool isPartOfSeries, UpcomingCancellationReason cancellationReason,
            bool isRecording, CardChannelAllocation cardChannelAllocation, List<Guid> conflictingPrograms)
        {
            string suffix = String.Empty;
            if (isPartOfSeries)
            {
                suffix += "Series";
            }
            if (cancellationReason != UpcomingCancellationReason.None)
            {
                suffix += "Cancelled";
                if (cancellationReason == UpcomingCancellationReason.PreviouslyRecorded
                    || cancellationReason == UpcomingCancellationReason.AlreadyQueued)
                {
                    suffix += "History";
                }
            }
            else
            {
                if (isRecording)
                {
                    if (cardChannelAllocation == null)
                    {
                        suffix += "InConflict";
                        //toolTip = ProcessUtility.CreateConflictingProgramsToolTip(_model.UpcomingRecordings,
                        //    upcoming.UpcomingRecording.ConflictingPrograms, Utility.GetLocalizedText(TextId.ConflictsHeader),
                        //    Utility.GetLocalizedText(TextId.NoCardFoundForRecording));
                    }
                    else if (conflictingPrograms.Count > 0)
                    {
                        suffix += "WithWarning";
                        //toolTip = ProcessUtility.CreateConflictingProgramsToolTip(_model.UpcomingRecordings,
                        //    upcoming.UpcomingRecording.ConflictingPrograms, Utility.GetLocalizedText(TextId.ConflictsHeader),
                        //    Utility.GetLocalizedText((TextId.NoCardFoundForRecording));
                    }
                }
            }
            return GUIGraphicsContext.Skin + @"\Media\ARGUS_" + scheduleType.ToString() + suffix + ".png";
        }

        public static string GetLogoForSchedule(ScheduleType scheduleType, bool isOneTime, bool isActive)
        {
            string suffix = String.Empty;
            if (!isOneTime)
            {
                suffix += "Series";
            }
            if (!isActive)
            {
                suffix += "Cancelled";
            }
            return GUIGraphicsContext.Skin + @"\Media\ARGUS_" + scheduleType.ToString() + suffix + ".png";
        }

        public static string GetLogoImage(Channel channel)
        {
            return GetLogoImage(channel.ChannelId, channel.DisplayName);
        }

        public static string GetLogoImage(Guid channelId, string channelDisplayName)
        {
            return ChannelLogosCache.GetLogoPath(channelId, channelDisplayName, HomeBase.LogoIconWidth, HomeBase.LogoIconHeight);
        }

        public static string GetRecordingThumb(RecordingSummary recording,bool createNewThumbIfNotFound, int size)
        {
            string thumb = string.Format("{0}\\{1}{2}", Thumbs.TVRecorded,recording.RecordingId,".jpg");
            if (Utils.FileExistsInCache(thumb))
            {
                //local thumb
                return thumb;
            }
            else if (createNewThumbIfNotFound)
            {
                //no local thumb found, ask one from the server and save it
                if (size < 0)
                {
                    size = 0;
                }

                byte[] jpegData = Proxies.ControlService.GetRecordingThumbnail(recording.RecordingId, size, size, null, recording.RecordingStartTime).Result;
                if (jpegData != null)
                {
                    try
                    {
                        using (System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(jpegData)))
                        {
                            img.Save(thumb, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                    catch { }
                    jpegData = null;
   
                    if (Utils.FileExistsInCache(thumb))
                    {
                        return thumb;
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Delete the old locally saved recording Thumbs.
        /// </summary>
        public static void CleanupRecordingThumbs()
        {
            Log.Debug("argustv: Cleanup old recording Thumbs");
            try
            {
                string path = Thumbs.TVRecorded;
                string[] files = Directory.GetFiles(path);
                DateTime lowerTimeLimit = DateTime.Now.AddMonths(-2);

                foreach (string file in files)
                {
                    if (File.GetCreationTime(file) < lowerTimeLimit)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("argustv: CleanupRecordingThumbs(),ex message = {0}", ex.Message);
            }
        }

        #endregion

        #region DateTime

        public static string GetShortDayDateString(DateTime dateTime)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            string dateFormat = culture.DateTimeFormat.MonthDayPattern.Replace("MMMM", "MM").Replace("MMM", "MM");
            dateFormat = dateFormat.Replace("dddd", "dd").Replace("ddd", "dd");
            dateFormat = dateFormat.Replace(". ", ".").Replace(" ", culture.DateTimeFormat.DateSeparator);
            return culture.DateTimeFormat.GetShortestDayName(dateTime.DayOfWeek) + " " + dateTime.ToString(dateFormat);
        }

        #endregion

        #region Network

        public static bool Ping(string _serverName)
        {
            try
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;

                string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;

                PingReply reply = pingSender.Send(_serverName, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public static bool IsThisASingleSeatSetup(string _serverName)
        {
            string _hostName = Dns.GetHostName();
            if (_serverName.Equals(_hostName, StringComparison.CurrentCultureIgnoreCase)
                || _serverName.Equals("localhost", StringComparison.CurrentCultureIgnoreCase)
                || _serverName.Equals("127.0.0.1"))
            {
                return true;
            }
            else if (_serverName.StartsWith("1"))
            {
                try
                {
                    IPAddress[] ips = Dns.GetHostAddresses(_hostName);
                    foreach (IPAddress ip in ips)
                    {
                        if (ip.ToString().Equals(_serverName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                catch { }
            }
            return false;
        }

        public static bool InitialiseServerSettings(ServerSettings serverSettings, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool succeeded = true;
            string _settingSection = PluginMain._settingSection;

            try
            {
                Proxies.Initialize(serverSettings, logger: new ProxyLogger());
                using (Settings xmlwriter = new MPSettings())
                {
                    xmlwriter.SetValue(_settingSection, TvHome.SettingName.MacAddresses, serverSettings.WakeOnLan.MacAddresses);
                    xmlwriter.SetValue(_settingSection, TvHome.SettingName.IPAddress, serverSettings.WakeOnLan.IPAddress);
                    xmlwriter.SetValueAsBool(_settingSection, TvHome.SettingName.IsSingleSeat, IsThisASingleSeatSetup(serverSettings.ServerName));
                }
            }
            catch (ArgusTVNotFoundException ex)
            {
                errorMessage = ex.Message;
                succeeded = false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                succeeded = false;
            }
            return succeeded;
        }

        #endregion

        #region RestartMP

        public static void RestartMP()
        {
            bool hideTaskBar = false;
            using (Settings xmlreader = new MPSettings())
            {
                hideTaskBar = xmlreader.GetValueAsBool("general", "hidetaskbar", false);
            }

            if (hideTaskBar)
            {
                Win32API.EnableStartBar(true);
                Win32API.ShowStartBar(true);
            }

            Log.Info("argustv: OnRestart - prepare for restart!");
            File.Delete(Config.GetFile(Config.Dir.Config, "mediaportal.running"));
            Log.Info("argustv: OnRestart - saving settings...");
            Settings.SaveCache();

            Log.Info("argustv: disposing databases.");
            FolderSettings.Dispose();
            PictureDatabase.Dispose();
            VideoDatabase.Dispose();
            MusicDatabase.Dispose();
            VolumeHandler.Dispose();

            System.Diagnostics.Process restartScript = new System.Diagnostics.Process();
            restartScript.EnableRaisingEvents = false;
            restartScript.StartInfo.WorkingDirectory = Config.GetFolder(Config.Dir.Base);
            restartScript.StartInfo.FileName = Config.GetFile(Config.Dir.Base, @"restart.vbs");
            Log.Debug("argustv: OnRestart - executing script {0}", restartScript.StartInfo.FileName);
            restartScript.Start();

            try
            {
                // Maybe the scripting host is not available therefore do not wait infinitely.
                if (!restartScript.HasExited)
                {
                    restartScript.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Log.Error("argustv: OnRestart - WaitForExit: {0}", ex.Message);
            }
        }

        #endregion

        #region StartServices

        public static void StartService(string serviceName, string serverName, int timeoutSec)
        {
            try
            {
                ServiceController service = new ServiceController(serviceName, serverName);
                if (service.Status == ServiceControllerStatus.Stopped || service.Status == ServiceControllerStatus.StartPending)
                {
                    try
                    {
                        if (service.Status == ServiceControllerStatus.Stopped)
                        {
                            Log.Info("argustv: service {0} is stopped, so we try start it now...", serviceName);
                            service.Start();
                        }
                        service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, timeoutSec));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("argustv: starting service {0} failed, {1}", serviceName, ex.Message);
                    }
                }
                Log.Info("argustv: service {0} - current status: {1}", serviceName, service.Status.ToString());
                service.Close();
                service.Dispose();
            }
            catch
            {
                Log.Info("argustv: service {0} not found on {1}", serviceName, serverName);
            }
        }

        #endregion

        internal class ProxyLogger : IServiceProxyLogger
        {
            public void Verbose(string message, params object[] args) { Log.Debug(message, args); }
            public void Info(string message, params object[] args) { Log.Info(message, args); }
            public void Warn(string message, params object[] args) { Log.Warn(message, args); }
            public void Error(string message, params object[] args) { Log.Error(message, args); }
        }
    }
}
