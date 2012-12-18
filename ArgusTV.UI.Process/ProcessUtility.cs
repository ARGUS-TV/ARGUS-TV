/*
 *	Copyright (C) 2007-2012 ARGUS TV
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
using System.Globalization;
using System.Text;
using System.IO;

using ArgusTV.DataContracts;
using ArgusTV.UI.Process.Guide;

namespace ArgusTV.UI.Process
{
    public static class ProcessUtility
    {
        public static string MakeValidFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c.ToString(), String.Empty);
            }
            return fileName.TrimEnd('.', ' ');
        }

        public static string CreateConflictingProgramsToolTip(UpcomingOrActiveProgramsList upcomingRecordings, List<Guid> programIds,
            string conflictsText, string noCardFoundText)
        {
            StringBuilder toolTip = new StringBuilder();
            foreach (Guid programId in programIds)
            {
                if (toolTip.Length > 0)
                {
                    toolTip.AppendLine();
                }
                else
                {
                    toolTip.AppendLine(conflictsText);
                }
                UpcomingProgram upcomingProgram = upcomingRecordings.FindProgramById(programId);
                if (upcomingProgram != null)
                {
                    toolTip.AppendFormat("● {0} {1:g}-{2:t} {3}", upcomingProgram.Channel.DisplayName,
                        upcomingProgram.StartTime, upcomingProgram.StopTime, upcomingProgram.CreateProgramTitle());
                }
                else
                {
                    toolTip.Append("● ?");
                }
            }
            if (toolTip.Length == 0)
            {
                toolTip.Append(noCardFoundText);
            }
            return toolTip.ToString();
        }

        public static string BuildRecordingInfoToolTip(ActiveRecording activeRecording, string onText)
        {
            return BuildRecordingInfoToolTip(activeRecording.ActualStartTime, activeRecording.ActualStopTime,
                activeRecording.CardChannelAllocation, onText);
        }

        public static string BuildRecordingInfoToolTip(UpcomingRecording upcomingRecording, string onText)
        {
            return BuildRecordingInfoToolTip(upcomingRecording.ActualStartTime, upcomingRecording.ActualStopTime,
                upcomingRecording.CardChannelAllocation, onText);
        }

        private static string BuildRecordingInfoToolTip(DateTime actualStartTime, DateTime actualStopTime,
            CardChannelAllocation allocation, string onText)
        {
            if (allocation != null)
            {
                PluginService pluginService = RecorderTunersCache.GetRecorderTunerById(allocation.RecorderTunerId);
                if (pluginService != null)
                {
                    return String.Format(CultureInfo.CurrentCulture, "{0} - {1} {2} {3} ({4})",
                        actualStartTime.ToShortTimeString(), actualStopTime.ToShortTimeString(),
                        onText, pluginService.Name, allocation.CardId);
                }
                else
                {
                    return String.Format(CultureInfo.CurrentCulture, "{0} - {1} on ?",
                        actualStartTime.ToShortTimeString(), actualStopTime.ToShortTimeString());
                }
            }
            return null;
        }
    }
}
