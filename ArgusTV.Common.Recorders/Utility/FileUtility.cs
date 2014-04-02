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
using System.Security.Cryptography;
using System.IO;
using System.Globalization;
using System.Net.Security;
using System.Management;
using System.Security.Principal;

using ArgusTV.DataContracts;

namespace ArgusTV.Common.Recorders.Utility
{
    public static class FileUtility
    {
        private const int _maxPath = 255;

        public static string BuildRecordingBaseFileName(UpcomingProgram program)
        {
            return BuildRecordingBaseFileName(null, program);
        }

        public static string BuildRecordingBaseFileName(string recordingsPath, UpcomingProgram program)
        {
            StringBuilder filePath = new StringBuilder();
            if (recordingsPath != null)
            {
                filePath.Append(recordingsPath);
                if (filePath[filePath.Length - 1] != Path.DirectorySeparatorChar)
                {
                    filePath.Append(Path.DirectorySeparatorChar);
                }
                filePath.Append(MakeValidFileName(program.Title)).Append(Path.DirectorySeparatorChar);
            }
            string programTitle = program.CreateProgramTitle();
            if (programTitle.Length > 80)
            {
                programTitle = programTitle.Substring(0, 80);
            }
            filePath.Append(MakeValidFileName(programTitle));
            filePath.Append("_");
            filePath.AppendFormat(MakeValidFileName(program.Channel.DisplayName));
            filePath.Append("_");
            filePath.AppendFormat(CultureInfo.InvariantCulture, @"{0:yyyy-MM-dd_HH-mm}", program.StartTime);
            return filePath.ToString();
        }

        public static string GetFreeFileName(string baseFileName, string extension, int extraCharsRequired = 0)
        {
            int maxPath = _maxPath - extraCharsRequired;

            if (!String.IsNullOrEmpty(extension) && !extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            if (baseFileName.Length > maxPath - extension.Length - 1)
            {
                baseFileName = baseFileName.Substring(0, maxPath - extension.Length - 1);
            }

            while (baseFileName.Length < 4)
            {
                baseFileName += '_';
            }

            string fileName = baseFileName + extension;

            int count = 1;
            while (File.Exists(fileName)
                && count < 256 * 8)
            {
                fileName = baseFileName.Substring(0, baseFileName.Length - 3) + count.ToString("X", CultureInfo.InvariantCulture).PadLeft(3, '_') + extension;
                count++;
            }

            return fileName;
        }

        public static string MakeValidFileName(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c.ToString(), String.Empty);
            }
            return fileName.TrimEnd('.', ' ');
        }

        public static string ConvertToUncPath(string recordingsRootUncPath, string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            string uncDirectory = recordingsRootUncPath;

            int index = fileName.LastIndexOf(Path.DirectorySeparatorChar);

            while (index > 0)
            {
                string uncFileName = Path.Combine(uncDirectory, fileName.Substring(index + 1));
                if (File.Exists(uncFileName))
                {
                    FileInfo uncInfo = new FileInfo(uncFileName);
                    FileInfo fileInfo = new FileInfo(fileName);
                    if (uncInfo.CreationTimeUtc == fileInfo.CreationTimeUtc)
                    {
                        return uncFileName;
                    }
                }
                index = fileName.LastIndexOf(Path.DirectorySeparatorChar, index - 1);
            }

            return null;
        }

        public static bool CreateUncShare(string shareName, string localPath)
        {
            ManagementScope scope = new System.Management.ManagementScope(@"root\CIMV2");
            scope.Connect();

            using (ManagementClass wmiShare = new ManagementClass(scope, new ManagementPath("Win32_Share"), null))
            {
                SecurityIdentifier worldSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                byte[] worldSidBytes = new byte[worldSid.BinaryLength];
                worldSid.GetBinaryForm(worldSidBytes, 0);

                using (ManagementObject wmiTrustee =
                    new ManagementClass(scope, new ManagementPath("Win32_Trustee"), null).CreateInstance())
                {
                    wmiTrustee["SID"] = worldSidBytes;

                    using (ManagementObject wmiAce =
                        new ManagementClass(scope, new ManagementPath("Win32_ACE"), null).CreateInstance())
                    {
                        wmiAce["AccessMask"] = 0x0200A9; // 0x1F01FF;
                        wmiAce["AceFlags"] = 3;
                        wmiAce["AceType"] = 0;
                        wmiAce["Trustee"] = wmiTrustee;

                        using (ManagementObject secDescriptor =
                            new ManagementClass(scope, new ManagementPath("Win32_SecurityDescriptor"), null).CreateInstance())
                        {
                            secDescriptor["ControlFlags"] = 4;
                            secDescriptor["DACL"] = new ManagementObject[] { wmiAce };

                            using (ManagementBaseObject inParams = wmiShare.GetMethodParameters("Create"))
                            {
                                inParams["Access"] = secDescriptor;
                                inParams["Path"] = localPath;
                                inParams["Name"] = shareName;
                                inParams["Type"] = 0;
                                inParams["Description"] = "ARGUS TV Recordings";

                                using (ManagementBaseObject outParams = wmiShare.InvokeMethod("Create", inParams, null))
                                {
                                    return ((uint)outParams["ReturnValue"] == 0);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
